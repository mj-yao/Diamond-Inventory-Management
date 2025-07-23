using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Globalization; //for parsing datetime

namespace DiamondTransaction
{
    public partial class DiamondTransaction : Form
    {
        public static string User = "FLEUR";
        public string Name = "FLEUR";
        SqlConnection Connection = null;
        public string connectionString = @"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql";

        public int currentRowIndex;
        AutoCompleteStringCollection supplierAutoComplete = new AutoCompleteStringCollection();
        List<string> SupplierCodeList = new List<string>();
        List<string> CustomerCodeList = new List<string>();
        List<string> supplierNames = new List<string>();
        List<string> customerNames = new List<string>();
        List<decimal> customerPaymentDays = new List<decimal>();
        Dictionary<string, string> docTypeDict = new Dictionary<string, string>();
        //MEMO IN 1009, MEMO IN RETURN 1010,PURCHASE MEMO IN 1011,PURCHASE NOTE 1006,PURCHASE NOTE RETURN 1007
        HashSet<int> supplierDocTypes = new HashSet<int> { 1006, 1007, 1009, 1010, 1011 };
        private bool isProgrammaticChange = false;
        private DocDetailControls DocDetail = null;
        private DocDetailControls NewDocDetail = null;
        /*
        class DocDetailControls
        {
            public ComboBox DocTypeDescComboBox { get; set; }
            public TextBox DocTypeIDTextBox { get; set; }
            public ComboBox DocSubTypeComboBox { get; set; }
            public ComboBox AccountCodeComboBox { get; set; }
            public ComboBox AccountNameComboBox { get; set; }
            public ComboBox AccountBranchCodeComboBox{ get; set; }
            public ComboBox CustomerPaymentTermComboBox { get; set; }


            public  DocDetailControls(ComboBox cb_docTypeDesc, TextBox tb_docTypeID, ComboBox cb_docSubType,
                                   ComboBox cb_accountCode, ComboBox cb_accountName,
                                   ComboBox cb_accountBranchCode, ComboBox cb_customerPaymentTerm)
            {                               
                DocTypeDescComboBox = cb_docTypeDesc;
                DocTypeIDTextBox = tb_docTypeID;
                DocSubTypeComboBox= cb_docSubType;
                AccountCodeComboBox = cb_accountCode;
                AccountNameComboBox = cb_accountName;
                AccountBranchCodeComboBox = cb_accountBranchCode;
                CustomerPaymentTermComboBox = cb_customerPaymentTerm;
            }
        }
        */


        public DiamondTransaction()
        {
            InitializeComponent();
            AccessDiamondDocHeader();
            InitialiseDocTypeComboBox();
            InitialiseCustomerComboBox(cb_FilterAccountCode, cb_FilterAccountName);

            DocDetail =    new DocDetailControls(cb_DocTypeDesc, tb_DocTypeID, cb_DocSubTypeID, cb_AccountCode, cb_AccountName, cb_AccountBranchCode, cb_PaymentTerm);
            NewDocDetail = new DocDetailControls(cb_DocTypeDescNew, tb_DocTypeIDNew, cb_DocSubTypeIDNew, cb_AccountCodeNew, cb_AccountNameNew, cb_AccountBranchCodeNew, cb_PaymentTermNew);


        }
        private void SetConnectionString()
        {
            Connection = new SqlConnection(connectionString);
            GlobalClass.setConnectionString(connectionString);
            //Connection = new SqlConnection(@"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql");
            //GlobalClass.setConnectionString(@"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql");

        }


        private void bt_GoodsInList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "PURCHASE";
            tab_Transaction.SelectedIndex = 1;
        }
        private void bt_InvoiceList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "INVOICE";
            tab_Transaction.SelectedIndex = 1;
        }
        private void bt_MemoInList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "MEMO IN";
            tab_Transaction.SelectedIndex = 1;
        }

        private void bt_MemoOutList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "MEMO OUT";
            tab_Transaction.SelectedIndex = 1;
        }

        private List<string> SearchDocTypeByKeyword(string keyword)
        {
            string lowerKeyword = keyword.ToLower();
            var matchingDocTypeIDs = docTypeDict
                .Where(kvp => kvp.Value.ToLower().Contains(lowerKeyword))
                .Select(kvp => kvp.Key)
                .ToList();

            return matchingDocTypeIDs;
        }

        private void comboBox_DynamicDropDownWidth(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            int maxWidth = combo.DropDownWidth;
            foreach (var item in combo.Items)
            {
                int itemWidth = TextRenderer.MeasureText(item.ToString(), combo.Font).Width;
                if (itemWidth > maxWidth)
                {
                    maxWidth = itemWidth;
                }
            }
            combo.DropDownWidth = maxWidth;
        }

        private void AccessDiamondDocHeader()
        {
            try
            {
                string errorMessageTitle = "Access DiamondDocHeader";
                string query = "SELECT DocHeaderID, DocTypeID, DocTypeDesc, DocID, DocCode, DocAccountRefNo, DocSubType, DocStatus, DocDate, " +
                "RefDocDate, RefDocDate2, DueDate, LastUpdate, AccountCode, AccountBranchCode, AccountName, AccountLongName, Currency, " +
                "VatCode, VatRate, PaymentTerm, Remark1, Remark2, InternalRemark, UserID, Quantity, Weight, " +
                "MainLinesTotalPrice, SecLinesTotalPrice, DocLinesTotalPrice, MainDiscountPrice, SecDiscountPrice, DocDiscountPrice, DiscountPercent, DiscountRemark, " +
                "MainSubTotalPrice, SecSubTotalPrice, DocSubTotalPrice, MainAdditionalPrice, SecAdditionalPrice, DocAdditionalPrice, AdditionalRemark, " +
                "MainTaxPrice, SecTaxPrice, DocTaxPrice, MainGrandTotalPrice, SecGrandTotalPrice, DocGrandTotalPrice, RateDocToMain, RateDocToSec FROM THXADiamondDocHeader Order by DocDate ";

             
                DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
                if (GlobalClass.IsValid(sqlResult))
                {
                    dgv_DocHeader.DataSource = sqlResult.DefaultView;
                    dgv_DocHeaderFormat();
                }
            }
            catch (Exception e1)
            {
                GlobalClass.ShowErrorMessage(e1, "Access Stone Detail failure.");
            }
        }


        private void dgv_DocHeaderFormat()
        {


            dgv_DocHeader.Columns["DocTypeID"].HeaderText = "DocType";
            dgv_DocHeader.Columns["DocStatus"].HeaderText = "Status";

            dgv_DocHeader.Columns["DocID"].Width = 50;
            dgv_DocHeader.Columns["DocStatus"].Width = 50;
            dgv_DocHeader.Columns["Currency"].Width = 65;
            dgv_DocHeader.Columns["DocLinesTotalPrice"].Width = 110;
            dgv_DocHeader.Columns["DocGrandTotalPrice"].Width = 110;

            dgv_DocHeader.Columns["DocHeaderID"].Visible = false;
            dgv_DocHeader.Columns["DocTypeID"].Visible = false;
            dgv_DocHeader.Columns["RefDocDate"].Visible = false;
            dgv_DocHeader.Columns["RefDocDate2"].Visible = false;
            dgv_DocHeader.Columns["DueDate"].Visible = false;
            dgv_DocHeader.Columns["LastUpdate"].Visible = false;
            //dgv_DocHeader.Columns["AccountCode"].Visible = false;
            dgv_DocHeader.Columns["AccountBranchCode"].Visible = false;
            dgv_DocHeader.Columns["AccountLongName"].Visible = false;
            dgv_DocHeader.Columns["VatCode"].Visible = false;
            dgv_DocHeader.Columns["VatRate"].Visible = false;
            dgv_DocHeader.Columns["PaymentTerm"].Visible = false;
            dgv_DocHeader.Columns["Remark1"].Visible = false;
            dgv_DocHeader.Columns["Remark2"].Visible = false;
            dgv_DocHeader.Columns["InternalRemark"].Visible = false;
            dgv_DocHeader.Columns["Quantity"].Visible = false;
            dgv_DocHeader.Columns["UserID"].Visible = false;
            dgv_DocHeader.Columns["MainLinesTotalPrice"].Visible = false;
            dgv_DocHeader.Columns["SecLinesTotalPrice"].Visible = false;
            dgv_DocHeader.Columns["MainDiscountPrice"].Visible = false;
            dgv_DocHeader.Columns["SecDiscountPrice"].Visible = false;
            dgv_DocHeader.Columns["DiscountPercent"].Visible = false;
            dgv_DocHeader.Columns["DiscountRemark"].Visible = false;
            dgv_DocHeader.Columns["MainSubTotalPrice"].Visible = false;
            dgv_DocHeader.Columns["SecSubTotalPrice"].Visible = false;
            dgv_DocHeader.Columns["MainAdditionalPrice"].Visible = false;
            dgv_DocHeader.Columns["SecAdditionalPrice"].Visible = false;
            dgv_DocHeader.Columns["DocAdditionalPrice"].Visible = false;
            dgv_DocHeader.Columns["AdditionalRemark"].Visible = false;
            dgv_DocHeader.Columns["MainTaxPrice"].Visible = false;
            dgv_DocHeader.Columns["SecTaxPrice"].Visible = false;
            dgv_DocHeader.Columns["MainGrandTotalPrice"].Visible = false;
            dgv_DocHeader.Columns["SecGrandTotalPrice"].Visible = false;
            dgv_DocHeader.Columns["RateDocToMain"].Visible = false;
            dgv_DocHeader.Columns["RateDocToSec"].Visible = false;
        }

        private void dgv_DocHeader_DoubleClick(object sender, EventArgs e)
        {
            tab_Transaction.SelectedIndex = 2;
        }

        private void tab_Transaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgv_DocHeader.SelectedRows.Count > 0)
            {
                currentRowIndex = dgv_DocHeader.SelectedRows[0].Index;
                FillTransactionDetail();
                AccessDiamondDocLine();
            }
            else
            {
                // Optional: Handle the case where no row is selected
                currentRowIndex = -1; // Or any default value
                MessageBox.Show("No row selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FillTransactionDetail()
        {
            //IsParcelControlNameAndTextsAdded = false;

            if (dgv_DocHeader.SelectedRows.Count > 0)
            {
                int selectedIndex = dgv_DocHeader.SelectedRows[0].Index;
                currentRowIndex = selectedIndex;

                /*
                string dateString = dgv_DocHeader.Rows[selectedIndex].Cells["DateTime_Created"].Value.ToString();
                tb_ParcelIntroDate.Text = dateString;
                DateTime dateValue = DateTime.Parse(dateString);
                ParcelIntroDate.Value = dateValue;
                */


                cb_DocTypeDesc.Text = dgv_DocHeader.Rows[selectedIndex].Cells["DocTypeDesc"].Value.ToString();
                tb_DocTypeID.Text = dgv_DocHeader.Rows[selectedIndex].Cells["DocTypeID"].Value.ToString();
                tb_DocID.Text = dgv_DocHeader.Rows[selectedIndex].Cells["DocID"].Value.ToString();
                tb_DocCode.Text = dgv_DocHeader.Rows[selectedIndex].Cells["DocCode"].Value.ToString();
                cb_DocSubTypeID.Text = dgv_DocHeader.Rows[selectedIndex].Cells["DocSubType"].Value.ToString();
                cb_AccountCode.Text = dgv_DocHeader.Rows[selectedIndex].Cells["AccountCode"].Value.ToString();
                cb_AccountBranchCode.Text = dgv_DocHeader.Rows[selectedIndex].Cells["AccountBranchCode"].Value.ToString();
                cb_AccountName.Text = dgv_DocHeader.Rows[selectedIndex].Cells["AccountName"].Value.ToString();
                dateTime_RefDocDate.Text = dgv_DocHeader.Rows[selectedIndex].Cells["RefDocDate"].Value.ToString();
                tb_Remark1.Text = dgv_DocHeader.Rows[selectedIndex].Cells["Remark1"].Value.ToString();
                cb_DocStatus.Text = dgv_DocHeader.Rows[selectedIndex].Cells["DocStatus"].Value.ToString();

                dateTime_DocDate.Text = dgv_DocHeader.Rows[selectedIndex].Cells["DocDate"].Value.ToString();
                cb_PaymentTerm.Text = dgv_DocHeader.Rows[selectedIndex].Cells["PaymentTerm"].Value.ToString();
                dateTime_DueDate.Text = dgv_DocHeader.Rows[selectedIndex].Cells["DueDate"].Value.ToString();
                cb_Currency.Text = dgv_DocHeader.Rows[selectedIndex].Cells["Currency"].Value.ToString();
                tb_USDRate.Text = dgv_DocHeader.Rows[selectedIndex].Cells["RateDocToMain"].Value.ToString();
                tb_GBPRate.Text = dgv_DocHeader.Rows[selectedIndex].Cells["RateDocToSec"].Value.ToString();
                tb_InternalRemark.Text = dgv_DocHeader.Rows[selectedIndex].Cells["InternalRemark"].Value.ToString();

                /*
                tb_ParcelLocationCode.Text = dgv_DocHeader.Rows[selectedIndex].Cells["LocationAccountCode"].Value.ToString();
                tb_ParcelLocationName.Text = dgv_DocHeader.Rows[selectedIndex].Cells["LocationAccountName"].Value.ToString();
                tb_ParcelVendorCode.Text = dgv_DocHeader.Rows[selectedIndex].Cells["VendorAccountCode"].Value.ToString();
                tb_ParcelVendorName.Text = dgv_DocHeader.Rows[selectedIndex].Cells["VendorAccountName"].Value.ToString();
                ParcelCostAtLoad = dgv_DocHeader.Rows[selectedIndex].Cells["Cost"].Value.ToString();
                ParcelPriceAtLoad = dgv_DocHeader.Rows[selectedIndex].Cells["GBPSale"].Value.ToString();
                tb_ParcelDetailCopy.Text = string.Empty;

                bt_UpdateParcelDetail.BackColor = SystemColors.Control;
                bt_UpdateParcelDetail.Enabled = false;
                IsParcelEditing = false;

                ParcelControlListManager = new ControlNameAndTextManager();
                ParcelControlListManager.FillInitAndEditedControlListFromPanel(gb_ParcelDetailGeneral);
                ParcelControlListManager.FillInitAndEditedControlListFromPanel(gb_ParcelDetailPrice);
                IsParcelControlNameAndTextsAdded = true;
                */



               /*
                DateTime dateValue;

                // Try parsing the date string, if it succeeds, assign it to the DateTimePicker
                if (DateTime.TryParse(dateString, out dateValue))
                {
                    datetime_ParcelIntro.Value = dateValue;
                }
                else
                {
                    // Handle the invalid date string scenario (e.g., show an error, set to a default value)
                    MessageBox.Show("Invalid date format. Please check the date.");
                    datetime_ParcelIntro.Value = DateTime.Now;  // You can set a default value like current date/time
                }
               */

            }
        }

        private void dateTime_RefDocDate_ValueChanged(object sender, EventArgs e)
        {
            //tb_ParcelIntroDate.Text = dateTime_RefDocDate.Value.ToString();
        }

        private void ApplyFilterDocHeader(object sender, EventArgs e)
        {
            try
            {
                StringBuilder filterExpression = new StringBuilder();
                var filters = new Dictionary<string, (Control, bool)>
                {
                    { "DocTypeDesc", (cb_FilterDocTypeDesc, false) },
                    { "DocCode", (tb_FilterDocCode, false) },
                    { "DocSubType", (cb_FilterDocSubType, false) },
                    { "AccountCode", (cb_FilterAccountCode, false) },
                    { "AccountName", (cb_FilterAccountName, false) },
                };

                GlobalClass.ApplyFilter(filterExpression, filters);

                ((DataView)dgv_DocHeader.DataSource).RowFilter = filterExpression.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void InitialiseDocTypeComboBox()
        {

            string errorMessageTitle = "Access DocType";
            string query = "SELECT DISTINCT DocTypeDesc,DocTypeID FROM THXADiamondDocType ";

            DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
            if (GlobalClass.IsValid(sqlResult))
            {

                docTypeDict.Clear();
                foreach (DataRow dr in sqlResult.Rows)
                {
                    docTypeDict[dr["DocTypeID"].ToString()] = dr["DocTypeDesc"].ToString();
                }
                
                List<string> DocTypeList = docTypeDict.Values.ToList();

                SetUpComboBoxItem(cb_FilterDocTypeDesc, DocTypeList);
                SetUpComboBoxItem(cb_DocTypeDesc, DocTypeList);
                SetUpComboBoxItem(cb_DocTypeDescNew, DocTypeList);

            }
        }


        private void DocHeaderFilterByDocTypeDesc(string docTypeDesc)
        {         
            //InitialseAccountCode(docTypeDesc, cb_FilterAccountCode);
            InitialiseDocSubType(docTypeDesc, cb_FilterDocSubType);
        }

        private void cb_FilterDocTypeDesc_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb_sender = (ComboBox)sender;
            string docTypeDesc = cb_sender.Text;
            string docTypeID = GetDocTypeID(cb_DocTypeDesc);
            InitialseAccountCode(docTypeID, cb_FilterAccountCode, cb_FilterAccountName);
            InitialiseDocSubType(docTypeDesc, cb_FilterDocSubType);

        }
        private void cb_DocTypeDesc_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDocType(DocDetail);
        }


        private void ChangeDocType(DocDetailControls controls)
        {            
            string docTypeDesc = controls.DocTypeDescComboBox.Text;
            string docTypeID = GetDocTypeID(controls.DocTypeDescComboBox);
            controls.DocTypeIDTextBox.Text = docTypeID;

            InitialseAccountCode(docTypeID, controls.AccountCodeComboBox, controls.AccountNameComboBox);
            InitialiseDocSubType(docTypeDesc, controls.DocSubTypeComboBox);
            
        }

       
        private void InitialiseDocSubType(string docTypeDesc, ComboBox cb_docSubType)
        {
            
            string errorMessageTitle = "Access DocType";
            string query = "SELECT DISTINCT DocSubType FROM THXADiamondDocHeader where DocTypeDesc = @DocTypeDesc ";

            var parameters = new Dictionary<string, object>
                {
                    { "@DocTypeDesc", docTypeDesc },        
                };

            DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle, parameters);
            if (GlobalClass.IsValid(sqlResult))
            {
                List<string> DocSubTypeList = new List<string>();
                DocSubTypeList = (from item in sqlResult.AsEnumerable()
                               select item.Field<string>("DocSubType")).ToList();

                SetUpComboBoxItem(cb_docSubType, DocSubTypeList);

            }
        }

        private void InitialseAccountCode(string docTypeID, ComboBox cb_accountCode, ComboBox cb_accountName) {
                      
            if (int.TryParse(docTypeID, out int docTypeIDInt)) {
                
                if (supplierDocTypes.Contains(docTypeIDInt))
                {
                    InitialiseSupplierComboBox(cb_accountCode, cb_accountName);
                }
                else
                {
                    InitialiseCustomerComboBox(cb_accountCode, cb_accountName);
                }
                       
            }      
        }

        private void InitialiseSupplierComboBox(ComboBox cb_accountCode, ComboBox cb_accountName)
        {
            string errorMessageTitle = "Access Supplier";
            string query = "SELECT SupplierCode, SupplierName FROM THPUSupplier ";

            DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
            if (GlobalClass.IsValid(sqlResult))
            {
                SupplierCodeList = new List<string>();
                supplierNames = new List<string>();

                SupplierCodeList = (from code in sqlResult.AsEnumerable()
                                    select code.Field<string>("SupplierCode")).ToList();

                supplierNames = (from desc in sqlResult.AsEnumerable()
                                 select desc.Field<string>("SupplierName")).ToList();


                SetUpComboBoxItem(cb_accountCode, SupplierCodeList);
                SetUpComboBoxItem(cb_accountName, supplierNames);

            }
        }

        private void InitialiseCustomerComboBox(ComboBox cb_accountCode, ComboBox cb_accountName)
        {
            string errorMessageTitle = "Access Customer";
            string query = "SELECT CustomerCode, CustomerName, PaymentDays FROM THSLCustomer ";

            DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
            if (GlobalClass.IsValid(sqlResult))
            {
                CustomerCodeList = new List<string>();
                customerNames = new List<string>();

                CustomerCodeList = (from code in sqlResult.AsEnumerable()
                                    select code.Field<string>("CustomerCode")).ToList();

                customerNames = (from desc in sqlResult.AsEnumerable()
                                 select desc.Field<string>("CustomerName")).ToList();

                customerPaymentDays = (from term in sqlResult.AsEnumerable()
                                    select term.Field<decimal>("PaymentDays")).ToList();


                SetUpComboBoxItem(cb_accountCode, CustomerCodeList);
                SetUpComboBoxItem(cb_accountName, customerNames);

            }
        }
        //----------------------Modify Doc accountcode and accountname------------------------------

        //---------------------- Utility Methods ------------------------------
        private void SetUpComboBoxItem(ComboBox comboBox, List<string> items)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(items.ToArray());
        }

        private void HandleProgrammaticChange(Action action)
        {
            if (isProgrammaticChange) return;

            isProgrammaticChange = true;
            action.Invoke();
            isProgrammaticChange = false;
        }

        private string GetDocTypeID(ComboBox cb_docTypeDesc)
        {
            return docTypeDict.FirstOrDefault(x => x.Value == cb_docTypeDesc.Text.Trim()).Key;
        }

        //---------------------- Supplier-Specific Methods ------------------------------
        private int GetSupplierCodeIndex(string supplierCode)
        {
            return SupplierCodeList.IndexOf(supplierCode);
        }

        private int GetSupplierNameIndex(string supplierName)
        {
            return supplierNames.IndexOf(supplierName);
        }

        private void FillSupplierName(ComboBox accountCode, ComboBox accountName)
        {
            int index = GetSupplierCodeIndex(accountCode.Text.Trim());
            if (index >= 0)
            {
                accountName.Text = supplierNames[index];
            }
        }

        private void FillSupplierCode(ComboBox accountName, ComboBox accountCode)
        {
            int index = GetSupplierNameIndex(accountName.Text.Trim());
            if (index >= 0)
            {
                accountCode.Text = SupplierCodeList[index];
            }
        }

        //---------------------- Customer-Specific Methods ------------------------------
        private int GetCustomerCodeIndex(string customerCode)
        {
            return CustomerCodeList.IndexOf(customerCode);
        }

        private int GetCustomerNameIndex(string customerName)
        {
            return customerNames.IndexOf(customerName);
        }

        private void FillCustomerNameFromCode(ComboBox accountCode, ComboBox accountName)
        {
            int index = GetCustomerCodeIndex(accountCode.Text.Trim());
            if (index >= 0)
            {
                accountName.Text = customerNames[index];
            }
        }

        private void FillCustomerCodeFromName(ComboBox accountName, ComboBox accountCode)
        {
            int index = GetCustomerNameIndex(accountName.Text.Trim());
            if (index >= 0)
            {
                accountCode.Text = CustomerCodeList[index];
            }
        }

        private void FillCustomerPaymentTerm(ComboBox accountCode, ComboBox paymentTerm)
        {
            int index = GetCustomerCodeIndex(accountCode.Text.Trim());
            if (index >= 0)
            {
                paymentTerm.Text = customerPaymentDays[index].ToString();
            }
        }

        private void FillCustomerBranch(string customerCode, ComboBox accountBranchCode)
        {
            try
            {
                string errorMessageTitle = "Access CustomerBranch";
                string query = @"SELECT CustomerCode, BranchCode FROM THSLCustomerBranch WHERE CustomerCode = @CustomerCode";
                var parameters = new Dictionary<string, object>
            {
                { "@CustomerCode", customerCode }
            };

                DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle, parameters);
                if (GlobalClass.IsValid(sqlResult))
                {
                    List<string> customerBranchList = sqlResult.AsEnumerable()
                        .Select(branch => branch.Field<string>("BranchCode"))
                        .ToList();

                    SetUpComboBoxItem(accountBranchCode, customerBranchList);
                    accountBranchCode.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                GlobalClass.ShowErrorMessage(e, "Access CustomerBranch failure.");
            }
        }

        //---------------------- Shared Account Methods ------------------------------
        private void FillAccountName(DocDetailControls controls)
        {
            if (controls.AccountCodeComboBox.Text.Trim() == "")
            {
                controls.AccountNameComboBox.Text = "";
                return;
            }
            if (int.TryParse(controls.DocTypeIDTextBox.Text, out int docTypeIDInt))
            {
                if (supplierDocTypes.Contains(docTypeIDInt))
                {
                    FillSupplierName(controls.AccountCodeComboBox, controls.AccountNameComboBox);
                }
                else 
                {
                    FillCustomerBranch(controls.AccountCodeComboBox.Text, controls.AccountBranchCodeComboBox);
                    FillCustomerNameFromCode(controls.AccountCodeComboBox, controls.AccountNameComboBox);
                    FillCustomerPaymentTerm(controls.AccountCodeComboBox, controls.CustomerPaymentTermComboBox);
                }
            }
        }

        private void FillAccountCode(DocDetailControls controls)
        {
            
            if (controls.AccountNameComboBox.Text.Trim() == "")
            {
                controls.AccountCodeComboBox.Text = "";
                return;
            }

            if (int.TryParse(controls.DocTypeIDTextBox.Text, out int docTypeIDInt))
            {
                if (supplierDocTypes.Contains(docTypeIDInt))
                {
                    FillSupplierCode(controls.AccountNameComboBox, controls.AccountCodeComboBox);
                }
                else
                {
                    FillCustomerCodeFromName(controls.AccountNameComboBox, controls.AccountCodeComboBox);
                    FillCustomerBranch(controls.AccountCodeComboBox.Text, controls.AccountBranchCodeComboBox);
                    FillCustomerPaymentTerm(controls.AccountCodeComboBox, controls.CustomerPaymentTermComboBox);
                }
            }
        }

        //---------------------- Event Handlers ------------------------------
        private void cb_AccountCode_TextChanged(object sender, EventArgs e)
        {
            HandleProgrammaticChange(() => FillAccountName(DocDetail));           
        }

        private void cb_AccountName_TextChanged(object sender, EventArgs e)
        {
            HandleProgrammaticChange(() => FillAccountCode(DocDetail));          
        }

        private void AccessDiamondDocLine()
        {
            try
            {
                string docTypeID = tb_DocTypeID.Text;
                string docID = tb_DocID.Text;                
                string query = @"SELECT DocHeaderID, DocLineID, DocTypeID, DocTypeDesc, DocID, DocLine, ItemID, LotName, 
                    ParcelOrStone, CertificateID, Shape, Size, Color, Clarity,
                    Weight, WeightLoss, Cost, TotalCost, SecCost, SecTotalCost, Sale, TotalSale, SecSale, SecTotalSale,
                    Additional, TotalAdditional, MainPrice, TotalMainPrice, SecPrice, TotalSecPrice, DocPrice,
                    TotalDocPrice, RateDocToMain, RateDocToSec, List, TotalList, ListCostDiscount, ListSaleDiscount,
                    ListMainDiscount, OutList, OutTotalList, OutMainPrice, OutTotalMainPrice, OutListMainDiscount,
                    OutSecPrice, OutSecTotalPrice, ReturnWeight, ReturnTotalCost, ReturnTotalSale, InvoicedWeight, InvoicedTotalCost,
                    InvoicedTotalSale, MainPriceGross,TotalMainPriceGross, SecPriceGross, TotalSecPriceGross, DocPriceGross, TotalDocPriceGross, 
                    DocPriceGrossWithTax, TotalDocPriceGrossWithTax, DocPriceWithTax,
                    TotalDocPriceWithTax, DocTotalDiscountWithTax, MarkupPercent, DiscountPercent, DocTotalDiscountAmount,
                    MainTotalDiscountAmount, SecTotalDiscountAmount, LotID, ParentLotID, 
                    TrsUnionID, SourceDocTypeID, SourceDocTypeDesc, SourceDocID,
                    SourceDocLine, SourceDocCode, DocLineStatus, LastUpdate, UserID,Remark 
                    FROM THXADiamondDocLine 
                    WHERE DocTypeID = @DocTypeID AND DocID = @DocID";
                              
                var parameters = new Dictionary<string, object>
                {
                    { "@DocTypeID", docTypeID },
                    { "@DocID", docID }
                };

                string errorMessageTitle = "Access Diamond DocLine";

                DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle, parameters);
                if (GlobalClass.IsValid(sqlResult))
                {
                    dgv_DocLine.DataSource = sqlResult.DefaultView;
                    dgv_DocLineFormat();
                }
            }
            catch (Exception e1)
            {
                GlobalClass.ShowErrorMessage(e1, "Access DocLine failure.");
            }


        }
        private void dgv_DocLineFormat()
        {
            //dgv_DocLine.Columns["DocLineID"].HeaderText = "Line";
            dgv_DocLine.Columns["DocLineStatus"].HeaderText = "LineStatus";
            dgv_DocLine.Columns["CertificateID"].HeaderText = "CertificateNo";
            dgv_DocLine.Columns["Cost"].HeaderText = "Cost ($)";
            dgv_DocLine.Columns["TotalCost"].HeaderText = "T.Cost ($)";
            dgv_DocLine.Columns["MainPrice"].HeaderText = "Main Price ($)";
            dgv_DocLine.Columns["TotalMainPrice"].HeaderText = "T.Main Price ($)";
            dgv_DocLine.Columns["SecPrice"].HeaderText = "GBP Price";
            dgv_DocLine.Columns["TotalSecPrice"].HeaderText = "T.GBP Price";
            dgv_DocLine.Columns["ListCostDiscount"].HeaderText = "Cost Disc%";
            dgv_DocLine.Columns["DocPriceGross"].HeaderText = "Doc Price Gross";
            dgv_DocLine.Columns["TotalDocPriceGross"].HeaderText = "T.Doc Price Gross";
            dgv_DocLine.Columns["DocTotalDiscountAmount"].HeaderText = "T.Discount";
            dgv_DocLine.Columns["OutList"].HeaderText = "Out List ($)";
            dgv_DocLine.Columns["OutListMainDiscount"].HeaderText = "Out Disc%";
            dgv_DocLine.Columns["MainPriceGross"].HeaderText = "MainPriceGross($)";
            dgv_DocLine.Columns["DocPriceGross"].HeaderText = "Doc Price Gross";
            dgv_DocLine.Columns["TotalMainPriceGross"].HeaderText = "T.MainPriceGross($)";

            SetColumnHeaderText("DocID", "DocID", 55);
            SetColumnHeaderText("DocLine", "Line", 35);
            SetColumnHeaderText("DocLineStatus", "Status", 45);
            SetColumnHeaderText("LotID", "LotID", 65);
            SetColumnHeaderText("ParentLotID", "ParentLotID", 70);
            SetColumnHeaderText("LotName", "LotName", 80);
            SetColumnHeaderText("CertificateID", "CertificateNo", 80);
            SetColumnHeaderText("Shape", "Shape", 70);
            SetColumnHeaderText("Size", "Size", 60);
            SetColumnHeaderText("Color", "Color", 50);
            SetColumnHeaderText("Clarity", "Clarity", 60);
            SetColumnHeaderText("Weight", "Weight", 60);
            SetColumnHeaderText("Cost", "Cost($)", 70);
            SetColumnHeaderText("TotalCost", "T.Cost($)", 70);
            SetColumnHeaderText("MainPrice", "MainPrice", 70);
            SetColumnHeaderText("TotalMainPrice", "T.MainPrice", 85);
            SetColumnHeaderText("SecPrice", "GBP Price", 85);
            SetColumnHeaderText("TotalSecPrice", "T.GBP Price", 95);
            SetColumnHeaderText("DocPrice", "DocPrice", 70);
            SetColumnHeaderText("TotalDocPrice", "TotalDocPrice", 85);
            SetColumnHeaderText("ListCostDiscount", "CostDisc%", 70);
            SetColumnHeaderText("DocPriceGross", "DocPriceGross", 80);
            SetColumnHeaderText("TotalDocPriceGross", "T. DocPriceGross", 100);
            SetColumnHeaderText("DocTotalDiscountAmount", "T.Discount", 75);
            SetColumnHeaderText("OutList", "OutList($)", 60);
            SetColumnHeaderText("OutMainPrice", "OutGross($)", 70);
            SetColumnHeaderText("OutTotalMainPrice", "T.OutGross($)", 90);
            SetColumnHeaderText("OutListMainDiscount", "OutDisc%", 65);
            SetColumnHeaderText("MainPriceGross", "MainPriceGross", 95);
            SetColumnHeaderText("DocPriceGross", "DocPriceGross", 90);
            SetColumnHeaderText("TotalMainPriceGross", "T.MainPriceGross", 100); 
            SetColumnHeaderText("SourceDocTypeDesc", "SourceDocType", 100);
            SetColumnHeaderText("SourceDocID", "SourceDocID", 80);
            SetColumnHeaderText("SourceDocCode", "SourceDocCode", 100);
            // Hide all columns by default
            foreach (DataGridViewColumn column in dgv_DocLine.Columns)
            {
                column.Visible = false;
            }

            // Get the DocTypeID
            string docTypeID = tb_DocTypeID.Text;
            List<string> MemoInColumns = new List<string> { "DocLine", "LotName", "CertificateID", "Shape", "Size", "Color", "Clarity", "Weight",
                "Cost", "TotalCost", "MainPrice", "TotalMainPrice", "SecPrice", "TotalSecPrice", "DocPrice", "TotalDocPrice", "ListCostDiscount",
                "DocPriceGross", "TotalDocPriceGross", "DocTotalDiscountAmount", "LotID", "ParentLotID", "DocLineStatus" };
            List<string> InvoiceColumns = new List<string> { "DocLine", "LotName", "CertificateID", "Shape", "Size", "Color", "Clarity", "Weight",
                "OutList", "OutMainPrice", "OutTotalMainPrice", "OutListMainDiscount", "DocPriceGross", "TotalDocPriceGross", "LotID", "ParentLotID", "SourceDocTypeDesc", "SourceDocID" ,"SourceDocCode", "DocLineStatus" };
            List<string> GoodsInColumns = new List<string> { "DocLine", "LotName", "CertificateID", "Shape", "Size", "Color", "Clarity", "Weight", "DocPrice",
                "TotalDocPrice", "LotID", "ParentLotID", "SourceDocTypeDesc",  "SourceDocID" , "SourceDocCode", "DocLineStatus", "Remark" };
            List<string> MemoOutColumns = new List<string> { "DocLine", "LotName", "CertificateID", "Shape", "Size", "Color", "Clarity", "Weight", "OutList", "OutListMainDiscount", 
                "DocPriceGross", "MainPriceGross", "TotalDocPriceGross", "TotalMainPriceGross", "LotID", "ParentLotID", "DocLineStatus" };

        // Define columns to show for each DocTypeID
        Dictionary<string, List<string>> columnsToShow = new Dictionary<string, List<string>>
            {
                { "1006", GoodsInColumns},  //GoodsIn
                { "1007", GoodsInColumns},  //GoodsIn Return               
                { "1009", MemoInColumns },  //MemoIn 
                { "1010", new List<string>(MemoInColumns) { "SourceDocTypeDesc", "SourceDocID", "SourceDocCode" } }, //MemoIn Return
                { "1001", MemoOutColumns }, //MemoOut
                { "1002", new List<string>(MemoOutColumns) { "SourceDocTypeDesc", "SourceDocID", "SourceDocCode"} }, //MemoOut Return
                { "1003", InvoiceColumns }, //Invoice Local 
                { "1004", InvoiceColumns }, //Invoie Export
                { "1005", InvoiceColumns }  //Invoice Return

            };

            // Show columns based on DocTypeID
            if (columnsToShow.ContainsKey(docTypeID))
            {
                foreach (string columnName in columnsToShow[docTypeID])
                {
                    if (dgv_DocLine.Columns.Contains(columnName))
                    {
                        dgv_DocLine.Columns[columnName].Visible = true;
                    }
                }
            }
            else
            {
                // If DocTypeID is not in the dictionary, show default columns
                List<string> defaultColumns = new List<string> {  "DocLine", "LotName", "CertificateID", "Shape", "Size", "Color", "Clarity", "Weight", "LotID", "ParentLotID", "DocLineStatus" };
                foreach (string columnName in defaultColumns)
                {
                    if (dgv_DocLine.Columns.Contains(columnName))
                    {
                        dgv_DocLine.Columns[columnName].Visible = true;
                    }
                }
            }
        }

        private void SetColumnHeaderText(string columnName, string headerText, int width)
        {
            DataGridViewColumn column = dgv_DocLine.Columns[columnName];
            column.HeaderText = headerText;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = width;
        }

        private void tb_DocHFilterClean_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = string.Empty;
            tb_FilterDocCode.Text = string.Empty;
            cb_FilterDocSubType.Text = string.Empty;
            cb_FilterAccountCode.Text = string.Empty;
            cb_FilterAccountName.Text = string.Empty;
        }
        //-------------------------------------- New Transaction---------------------------------------
        private void cb_DocTypeDescNew_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDocType(NewDocDetail);
        }

        private void cb_AccountCodeNew_TextChanged(object sender, EventArgs e)
        {
            HandleProgrammaticChange(() => FillAccountName(NewDocDetail));            
        }

        private void cb_AccountNameNew_TextChanged(object sender, EventArgs e)
        {
            HandleProgrammaticChange(() => FillAccountCode(NewDocDetail));
        }
    }
}
