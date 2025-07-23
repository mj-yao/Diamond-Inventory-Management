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
using System.Globalization;
using System.Web; //for parsing datetime
using DiamondTransaction.DataAccess;
using DiamondTransaction.Models;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace DiamondTransaction
{
    public partial class fmDiamondTransaction : Form
    {
        public static string User = "FLEUR";
        public string Name = "FLEUR";
        SqlConnection Connection = null;
        public string connectionString = @"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql";
        

        public int currentRowIndex;
        AutoCompleteStringCollection supplierAutoComplete = new AutoCompleteStringCollection();
        public List<string> SupplierCodeList = new List<string>();
        public List<string> CustomerCodeList = new List<string>();
        public List<string> supplierNames = new List<string>();
        public List<string> customerNames = new List<string>();
        public List<decimal> customerPaymentDays = new List<decimal>();
        private readonly List<int> supplierDocTypeIDs;
        Dictionary<string, string> docTypeDict = new Dictionary<string, string>();
        //MEMO IN 1009, MEMO IN RETURN 1010,PURCHASE MEMO IN 1011,PURCHASE NOTE 1006,PURCHASE NOTE RETURN 1007
        //HashSet<int> supplierDocTypes = new HashSet<int> { 1006, 1007, 1009, 1010, 1011 };
        private bool isProgrammaticChange = false;
        private DocDetailControls DocDetail = null;
        private DocDetailControls NewDocDetail = null;

        public decimal USDRate, GBPRate; public string docCurrency;
        private string originalUSDExRate;

        private int MaxDocID; string MaxDocCode;
        public int docTypeID,  docTypeIDNew;
        public int dgv_LinesMaxLotID;

        public bool IsCreatingNewDoc = false;
        public int MaxItemID = 0, MaxLotID = 0, MaxCertificateID = 0;

        private Dictionary<int, CertificateData> certificateDataDictionary = new Dictionary<int, CertificateData>();
        private DiamondDocRepository _docRepository;
        
        private List<Supplier> _suppliers;
        private SupplierDataAccess _supplierData;   
        private List<Customer> _customers;
        private CustomerDataAccess _customerData;

        public List<Supplier> Suppliers { get; private set; }
        public List<Customer> Customers { get; private set; }
        



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


        public fmDiamondTransaction()
        {
            InitializeComponent();
            AccessDiamondDocHeader();
            InitialiseDocTypeComboBox();           
            InitialiseExRate();

            DocDetail =    new DocDetailControls(cb_DocTypeDesc, tb_DocTypeID, cb_DocSubTypeID, cb_AccountCode, cb_AccountName, cb_AccountBranchCode, cb_PaymentTerm);
            NewDocDetail = new DocDetailControls(cb_DocTypeDescNew, tb_DocTypeIDNew, cb_DocSubTypeIDNew, cb_AccountCodeNew, cb_AccountNameNew, cb_AccountBranchCodeNew, cb_PaymentTermNew);
            AccountDataManager accountManager = new AccountDataManager(connectionString);
            _docRepository = new DiamondDocRepository(connectionString);
            _supplierData = new SupplierDataAccess(connectionString);
            LoadSupplierData();
            _customerData = new CustomerDataAccess(connectionString);
            LoadCustomerData();
            InitialiseCustomerComboBox(cb_FilterAccountCode, cb_FilterAccountName);

            supplierDocTypeIDs = _docRepository.GetSupplierDocTypeIDs();


        }
        private void SetConnectionString()
        {
            Connection = new SqlConnection(connectionString);
            GlobalClass.setConnectionString(connectionString);
            //Connection = new SqlConnection(@"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql");
            //GlobalClass.setConnectionString(@"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql");

        }

        private static bool IsAuthorisedUser()
        {
            if (User.Equals("MARTA") ||
                User.Equals("DANNY") ||
                User.Equals("ENON") ||
                User.Equals("MUNSON") ||
                User.Equals("JIN") ||
                User.Equals("FLEUR"))
                return true;
            else
                return false;
        }

        private void LoadCustomerData()
        {
            _customers = _customerData.GetAllCustomers();
        }
        private void LoadSupplierData()
        {
            _suppliers = _supplierData.GetAllSuppliers();
        }


        private void InitialiseExRate()
        {

            string errorMessageTitle = "Access Exchange Rate ";
            string query = "SELECT TOP 1 USD AS USDRate FROM THXADiamondExchangeRate ORDER BY DateTime_Created DESC ";

            DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
            if (GlobalClass.IsValid(sqlResult))
            {
                USDRate = GlobalClass.GetDecimalValue(sqlResult.Rows[0]["USDRate"].ToString());
                GBPRate = GBPRate = Math.Round(1 / USDRate, 3);

                originalUSDExRate = USDRate.ToString();
            }
        }

        private void setDecimalPlaceTo2(object sender, EventArgs e)
        {
            setDecimalPlacesFormat(sender, 2);
        }

        private void setDecimalPlaceTo3(object sender, EventArgs e)
        {
            setDecimalPlacesFormat(sender, 3);
        }


        public void setDecimalPlacesFormat(object sender, int DecimalPlace)
        {
            TextBox tb = (TextBox)sender;
            try
            {
                double.TryParse(tb.Text, out double TextInDouble);
                if (DecimalPlace == 0)
                    tb.Text = string.Format("{0:0}", TextInDouble);
                else if (DecimalPlace == 1)
                    tb.Text = string.Format("{0:0.0}", TextInDouble);
                else if (DecimalPlace == 2)
                    tb.Text = string.Format("{0:0.00}", TextInDouble);
                else if (DecimalPlace == 3)
                    tb.Text = string.Format("{0:0.000}", TextInDouble);
                else if (DecimalPlace == 4)
                    tb.Text = string.Format("{0:0.0000}", TextInDouble);
            }
            catch (Exception e1)
            {
                GlobalClass.ShowErrorMessage(e1, "Text Parse to Double fails");
            }
        }



        private List<string> GetDocTypeIDsByDescKeyword(string keyword)
        {
            string lowerKeyword = keyword.ToLower();
            var matchDocTypeIDs = docTypeDict
                .Where(kvp => kvp.Value.ToLower().Contains(lowerKeyword))
                .Select(kvp => kvp.Key)
                .ToList();

            return matchDocTypeIDs; // Returns a list of matching DocTypeIDs
        }

        private string GetDocTypeIDByDocTypeDesc(string description)
        {
            var match = docTypeDict.FirstOrDefault(kvp => kvp.Value.Equals(description, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(match.Key))
            {
                return match.Key;
            }

            return string.Empty;
        }

        private void bt_GoodsInList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "PURCHASE";
            tab_Transaction.SelectedIndex = 1;
        }
        private void bt_DebitNoteList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "PURCHASE NOTE RETURN";
            tab_Transaction.SelectedIndex = 1;
        }
        private void bt_InvoiceList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "INVOICE";
            tab_Transaction.SelectedIndex = 1;
        }        
        private void bt_CreditNoteList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "INVOICE RETURN";
            tab_Transaction.SelectedIndex = 1;
        }

        private void bt_MemoInList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "MEMO IN";
            tab_Transaction.SelectedIndex = 1;
        }

        private void bt_MemoInReturnList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "MEMO IN RETURN";
            tab_Transaction.SelectedIndex = 1;
        }

        private void bt_MemoOutList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "MEMO OUT";
            tab_Transaction.SelectedIndex = 1;
        }
        private void bt_MemoOutReturnList_Click(object sender, EventArgs e)
        {
            cb_FilterDocTypeDesc.Text = "MEMO OUT RETURN";
            tab_Transaction.SelectedIndex = 1;
        }


        private void cb_FilterAccountName_DropDown(object sender, EventArgs e)
        {
            GlobalClass.comboBox_DynamicDropDownWidth(sender, e);
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
            if (tab_Transaction.SelectedIndex == 2) 
            { 
                if (dgv_DocHeader.SelectedRows.Count > 0 )
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
            
        }

        private void FillTransactionDetail()
        {
            //IsParcelControlNameAndTextsAdded = false;

            if (dgv_DocHeader.SelectedRows.Count > 0)
            {
                int selectedIndex = dgv_DocHeader.SelectedRows[0].Index;
                currentRowIndex = selectedIndex;

                /*
                string dateString = dgv_DocHeader.WorkingLine[selectedIndex].Cells["DateTime_Created"].Value.ToString();
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
                tb_ParcelLocationCode.Text = dgv_DocHeader.WorkingLine[selectedIndex].Cells["LocationAccountCode"].Value.ToString();
                tb_ParcelLocationName.Text = dgv_DocHeader.WorkingLine[selectedIndex].Cells["LocationAccountName"].Value.ToString();
                tb_ParcelVendorCode.Text = dgv_DocHeader.WorkingLine[selectedIndex].Cells["VendorAccountCode"].Value.ToString();
                tb_ParcelVendorName.Text = dgv_DocHeader.WorkingLine[selectedIndex].Cells["VendorAccountName"].Value.ToString();
                ParcelCostAtLoad = dgv_DocHeader.WorkingLine[selectedIndex].Cells["Cost"].Value.ToString();
                ParcelPriceAtLoad = dgv_DocHeader.WorkingLine[selectedIndex].Cells["GBPSale"].Value.ToString();
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
                    // Handle the invalid date string scenario (rowList.g., show an error, set to a default value)
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
            string docTypeID = _docRepository.GetDocTypeIDByDocTypeDesc(docTypeDesc);
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
            string docTypeID = _docRepository.GetDocTypeIDByDocTypeDesc(docTypeDesc);
            controls.DocTypeIDTextBox.Text = docTypeID;

            InitialseAccountCode(docTypeID, controls.AccountCodeComboBox, controls.AccountNameComboBox);
            InitialiseDocSubType(docTypeDesc, controls.DocSubTypeComboBox);
            
        }

        private void InitialiseDocSubType(string docTypeDesc, ComboBox cb_docSubType)
        {
            List<string> docSubTypes = _docRepository.GetDocSubTypesByDocType(docTypeDesc);
            SetUpComboBoxItem(cb_docSubType, docSubTypes);
        }


        private void InitialiseDocSubType0(string docTypeDesc, ComboBox cb_docSubType)
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
                
                if (supplierDocTypeIDs.Contains(docTypeIDInt))
                    {
                    InitialiseSupplierComboBox(cb_accountCode, cb_accountName);
                }
                else
                {
                    InitialiseCustomerComboBox(cb_accountCode, cb_accountName);
                }
                       
            }      
        }

        private void InitialiseSupplierComboBox0(ComboBox cb_accountCode, ComboBox cb_accountName)
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

        private void InitialiseSupplierComboBox(ComboBox cb_accountCode, ComboBox cb_accountName)
        {
            var suppliers = _supplierData.GetAllSuppliers();
            List<string> supplierCodes = suppliers.Select(s => s.SupplierCode).ToList();
            List<string> supplierNames = suppliers.Select(s => s.SupplierName).ToList();

            SetUpComboBoxItem(cb_accountCode, supplierCodes);
            SetUpComboBoxItem(cb_accountName, supplierNames);
          
        }

        private void InitialiseCustomerComboBox0(ComboBox cb_accountCode, ComboBox cb_accountName)
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

        private void InitialiseCustomerComboBox(ComboBox cb_accountCode, ComboBox cb_accountName)
        {
            var customers = _customerData.GetAllCustomers();
            List<string> customerCodes = customers.Select(c => c.CustomerCode).ToList();
            List<string> customerNames = customers.Select(c => c.CustomerName).ToList();

            SetUpComboBoxItem(cb_accountCode, customerCodes);
            SetUpComboBoxItem(cb_accountName, customerNames);

        }
        //----------------------Modify Doc accountcode and accountname------------------------------

        //---------------------- Utility Methods ------------------------------
        public void SetUpComboBoxItem(ComboBox comboBox, List<string> items)
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
        private int GetSupplierCodeIndex0(string supplierCode)
        {
            return SupplierCodeList.IndexOf(supplierCode);
        }

        private int GetSupplierNameIndex0(string supplierName)
        {
            return supplierNames.IndexOf(supplierName);
        }

        private void FillSupplierName0(ComboBox accountCode, ComboBox accountName)
        {
            int index = GetSupplierCodeIndex0(accountCode.Text.Trim());
            if (index >= 0)
            {
                accountName.Text = supplierNames[index];
            }
        }

        private void FillSupplierCode0(ComboBox accountName, ComboBox accountCode)
        {
            int index = GetSupplierNameIndex0(accountName.Text.Trim());
            if (index >= 0)
            {
                accountCode.Text = SupplierCodeList[index];
            }
        }

        private int GetSupplierIndexByCode(string supplierCode)
        {
            return _suppliers.FindIndex(s => s.SupplierCode == supplierCode.Trim());
        }

        private int GetSupplierIndexByName(string supplierName)
        {
            return _suppliers.FindIndex(s => s.SupplierName == supplierName.Trim());
        }

        private void FillSupplierNameFromCode(ComboBox accountCode, ComboBox accountName)
        {
            int index = GetSupplierIndexByCode(accountCode.Text.Trim());
            if (index >= 0)
            {
                accountName.Text = _suppliers[index].SupplierName;
            }
        }

        private void FillSupplierCodeFromName(ComboBox accountName, ComboBox accountCode)
        {
            int index = GetSupplierIndexByName(accountName.Text.Trim());
            if (index >= 0)
            {
                accountCode.Text = _suppliers[index].SupplierCode;
            }
        }




        //---------------------- Customer-Specific Methods ------------------------------
        private int GetCustomerCodeIndex0(string customerCode)
        {
            return CustomerCodeList.IndexOf(customerCode);
        }

        private int GetCustomerNameIndex0(string customerName)
        {
            return customerNames.IndexOf(customerName);
        }

        private void FillCustomerNameFromCode0(ComboBox accountCode, ComboBox accountName)
        {
            int index = GetCustomerCodeIndex0(accountCode.Text.Trim());
            if (index >= 0)
            {
                accountName.Text = customerNames[index];
            }
        }

        private void FillCustomerCodeFromName0(ComboBox accountName, ComboBox accountCode)
        {
            int index = GetCustomerNameIndex0(accountName.Text.Trim());
            if (index >= 0)
            {
                accountCode.Text = CustomerCodeList[index];
            }
        }

        private void FillCustomerPaymentTerm0(ComboBox accountCode, ComboBox paymentTerm)
        {
            int index = GetCustomerCodeIndex0(accountCode.Text.Trim());
            if (index >= 0)
            {
                paymentTerm.Text = customerPaymentDays[index].ToString();
            }
        }

        private int GetCustomerIndexByCode(string customerCode)
        {
            return _customers.FindIndex(c => c.CustomerCode == customerCode.Trim());
        }

        private int GetCustomerIndexByName(string customerName)
        {
            return _customers.FindIndex(c => c.CustomerName == customerName.Trim());
        }

        private void FillCustomerNameFromCode(ComboBox accountCode, ComboBox accountName)
        {
            int index = GetCustomerIndexByCode(accountCode.Text);
            if (index >= 0)
            {
                accountName.Text = _customers[index].CustomerName;
            }
        }

        private void FillCustomerCodeFromName(ComboBox accountName, ComboBox accountCode)
        {
            int index = GetCustomerIndexByName(accountName.Text);
            if (index >= 0)
            {
                accountCode.Text = _customers[index].CustomerCode;
            }
        }

        private void FillCustomerPaymentTerm(ComboBox accountCode, ComboBox paymentTerm)
        {
            int index = GetCustomerIndexByCode(accountCode.Text);
            if (index >= 0)
            {
                paymentTerm.Text = _customers[index].PaymentDays.ToString();
            }
        }


        private void FillCustomerBranch0(string customerCode, ComboBox accountBranchCode)
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

        private void FillCustomerBranch(string customerCode, ComboBox cb_accountBranchCode)
        {
            try
            {
                var branches = _customerData.GetBranchesByCustomer(customerCode);

                if (branches.Any())
                {
                    cb_accountBranchCode.DataSource = branches;
                    cb_accountBranchCode.DisplayMember = "BranchCode";
                    cb_accountBranchCode.ValueMember = "BranchCode";
                    cb_accountBranchCode.SelectedIndex = 0;
                }
                else
                {
                    cb_accountBranchCode.DataSource = null;
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
                if (supplierDocTypeIDs.Contains(docTypeIDInt))
                {
                    FillSupplierNameFromCode(controls.AccountCodeComboBox, controls.AccountNameComboBox);
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
                if (supplierDocTypeIDs.Contains(docTypeIDInt))
                {
                    FillSupplierCodeFromName(controls.AccountNameComboBox, controls.AccountCodeComboBox);
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
                    DocTypeID, CertificateID, Shape, Size, Color, Clarity,
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
            dgv_DocLine.Columns["CertificateID"].HeaderText = "CertificateID";
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
            SetColumnHeaderText("CertificateID", "CertificateID", 80);
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
                { "1010", new List<string>(MemoInColumns) { "SourceDocTypDesce", "SourceDocID", "SourceDocCode" } }, //MemoIn Return
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

        private void bt_NewGoodsIn_Click(object sender, EventArgs e)
        {
            if (IsCreatingNewDoc)
            {
                MessageBox.Show("A new transaction is already in progress. Please complete or cancel it before starting a new one.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
                          
            IsCreatingNewDoc = true;
            CreateNewDocSupplier("PURCHASE NOTE");
            InitializeDgv_LinesTable(1006);           
        }

        private void bt_NewMemoIn_Click(object sender, EventArgs e)
        {
            if (IsCreatingNewDoc)
            {
                MessageBox.Show("A new transaction is already in progress. Please complete or cancel it before starting a new one.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            IsCreatingNewDoc = true;
            CreateNewDocSupplier("MEMO IN");
            InitializeDgv_LinesTable(1009);
        }

        private void bt_NewMemoOut_Click(object sender, EventArgs e)
        {
            if (IsCreatingNewDoc)
            {
                MessageBox.Show("A new transaction is already in progress. Please complete or cancel it before starting a new one.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            IsCreatingNewDoc = true;
            CreateNewDocCustomer("MEMO OUT");
            InitializeDgv_LinesTable(1001);
        }

        private void bt_NewInvoice_Click(object sender, EventArgs e)
        {
            if (IsCreatingNewDoc)
            {
                MessageBox.Show("A new transaction is already in progress. Please complete or cancel it before starting a new one.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            IsCreatingNewDoc = true;
            CreateNewDocCustomer("INVOICE LOCAL");
            InitializeDgv_LinesTable(1003);
        }

        private void bt_NewDebitNote_Click(object sender, EventArgs e)
        {
            if (IsCreatingNewDoc)
            {
                MessageBox.Show("A new transaction is already in progress. Please complete or cancel it before starting a new one.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            IsCreatingNewDoc = true;
            CreateNewDocSupplier("PURCHASE NOTE RETURN");
            InitializeDgv_LinesTable(1007);
        }

        private void bt_NewCreditNote_Click(object sender, EventArgs e)
        {
            if (IsCreatingNewDoc)
            {
                MessageBox.Show("A new transaction is already in progress. Please complete or cancel it before starting a new one.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            IsCreatingNewDoc = true;
            CreateNewDocCustomer("INVOICE RETURN");
            InitializeDgv_LinesTable(1005);
        }

        private void bt_NewMemoInReturn_Click(object sender, EventArgs e)
        {
            if (IsCreatingNewDoc)
            {
                MessageBox.Show("A new transaction is already in progress. Please complete or cancel it before starting a new one.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            IsCreatingNewDoc = true;
            CreateNewDocSupplier("MEMO IN RETURN");
            InitializeDgv_LinesTable(1010);
        }

        private void bt_NewMemoOutReturn_Click(object sender, EventArgs e)
        {
            if (IsCreatingNewDoc)
            {
                MessageBox.Show("A new transaction is already in progress. Please complete or cancel it before starting a new one.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            IsCreatingNewDoc = true;
            CreateNewDocCustomer("MEMO OUT RETURN");
            InitializeDgv_LinesTable(1002);
        }

        private void CreateNewDocSupplier(string docTypeDesc) {
            //AccessDocHeaderMaxDocIDAndDocCodeFor(docTypeDesc);
            //AccessDiamondLotMaxItemLotIDAndCertificateID();
            var docHeaderMaxInfo = _docRepository.AccessDocHeaderMaxDocIDAndDocCodeFor(docTypeDesc);
            var diamondLotMaxInfo = _docRepository.AccessDiamondLotMaxItemLotIDAndCertificateID();

            MaxDocID = docHeaderMaxInfo.MaxDocID;
            MaxDocCode = docHeaderMaxInfo.MaxDocCode;
            MaxItemID = diamondLotMaxInfo.MaxItemID;
            MaxCertificateID = diamondLotMaxInfo.MaxCertificateID;
            MaxLotID = diamondLotMaxInfo.MaxLotID;

            tab_Transaction.SelectedIndex = 3;
            cb_DocTypeDescNew.Text = docTypeDesc;
            cb_DocStatusNew.Text = "A";
            cb_CurrencyNew.Text = "USD";
            tb_DocIDNew.Text = (MaxDocID + 1).ToString();
            tb_DocCodeNew.Text = GenerateNewDocCode(MaxDocCode);
            cb_DocSubTypeIDNew.Text = "";
            cb_DocSubTypeIDNew.Enabled = false;
            cb_AccountBranchCodeNew.Text = "";
            cb_AccountBranchCodeNew.Enabled = false;
            tb_DiscountNew.Text = "0.00";
            tb_AdditionalNew.Text = "0.00";
            tb_TaxNew.Text = "0.00";
        }

        private void CreateNewDocCustomer(string docTypeDesc)
        {
            //AccessDocHeaderMaxDocIDAndDocCodeFor(docTypeDesc);
            //AccessDiamondLotMaxItemLotIDAndCertificateID();
            var docHeaderMaxInfo = _docRepository.AccessDocHeaderMaxDocIDAndDocCodeFor(docTypeDesc);
            var diamondLotMaxInfo = _docRepository.AccessDiamondLotMaxItemLotIDAndCertificateID();

            MaxDocID = docHeaderMaxInfo.MaxDocID;
            MaxDocCode = docHeaderMaxInfo.MaxDocCode;
            MaxItemID = diamondLotMaxInfo.MaxItemID;
            MaxCertificateID = diamondLotMaxInfo.MaxCertificateID;
            MaxLotID = diamondLotMaxInfo.MaxLotID;

            tab_Transaction.SelectedIndex = 3;
            cb_DocTypeDescNew.Text = docTypeDesc;
            cb_DocStatusNew.Text = "A";
            cb_CurrencyNew.Text = "GBP";
            tb_DocIDNew.Text = (MaxDocID + 1).ToString();
            tb_DocCodeNew.Text = GenerateNewDocCode(MaxDocCode);
            cb_DocSubTypeIDNew.Text = "";
            cb_DocSubTypeIDNew.Enabled = false;
            cb_AccountBranchCodeNew.Text = "";
            cb_AccountBranchCodeNew.Enabled = true;
            tb_DiscountNew.Text = "0.00";
            tb_AdditionalNew.Text = "0.00";
            tb_TaxNew.Text = "0.00";
        }

        private void cb_CurrencyNew_TextChanged(object sender, EventArgs e)
        {
            if (cb_CurrencyNew.Text == "USD")
            {
                tb_USDRateNew.Text = "1.000";
                tb_GBPRateNew.Text = GBPRate.ToString();
                docCurrency = "USD";
            }
            else if (cb_CurrencyNew.Text == "GBP")
            {
                tb_GBPRateNew.Text = "1.000";
                tb_USDRateNew.Text = USDRate.ToString();
                docCurrency = "GBP";
            }
        }

        
        private void AccessDocHeaderMaxDocIDAndDocCodeFor(string docTypeDesc)
        {
            string DocTypeID = GetDocTypeIDByDocTypeDesc(docTypeDesc);
            try
            {
                string query = $"SELECT TOP 1 DocID as MAXDocID, DocCode as MAXDocCode FROM THXADiamondDocHeader WHERE DocTypeID = '{DocTypeID}' ORDER BY DocID DESC ";
                string errorMessageTitle = "Access DocHeader latest DocID and DocCode";

                DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
                if (GlobalClass.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
                {
                    object maxDocIDValue = sqlResult.Rows[0]["MAXDocID"];
                    object maxDocCodeValue = sqlResult.Rows[0]["MAXDocCode"];
                    if (maxDocIDValue != DBNull.Value)
                    {

                        MaxDocID = Convert.ToInt32(maxDocIDValue);
                        MaxDocCode = maxDocCodeValue.ToString();

                    }
                }
            }
            catch (Exception e1)
            {
                GlobalClass.ShowErrorMessage(e1, "Access MAXDocID in THXADiamondDocHeader failure.");
            }
        }

        private string GenerateNewDocCode(string maxDocCode)
        {
            string prefix = "GI";
            string currentYearSuffix = DateTime.Now.Year.ToString().Substring(2, 2); // Get last two digits of the current year

            //If the maxDocCode is empty or does not start with the prefix, start from the beginning
            if (string.IsNullOrEmpty(maxDocCode) || !maxDocCode.StartsWith(prefix))
            {
               
                return $"{prefix}{currentYearSuffix}000001";
            }
       
            string yearPart = maxDocCode.Substring(2, 2); 
            string numberPart = maxDocCode.Substring(4);  

            int nextNumber;
            if (yearPart == currentYearSuffix)
            {
                nextNumber = int.Parse(numberPart) + 1;
            }
            else
            {
                nextNumber = 1;
            }

            return $"{prefix}{currentYearSuffix}{nextNumber:D6}";
        }  

        private void bt_AddWorkingLine_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(tb_DocTypeID.Text.Trim(), out int DocTypeID))
            {
                MessageBox.Show("Invalid Document Type");
                return;
            }
            if (IsCreatingNewDoc)
            {
                MessageBox.Show("New row can not be added while a new document is on editing.\nPlease save or cancel the new document then try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            docTypeID = DocTypeID;
            WorkingLine InWorkingLine = new WorkingLine(this);
            InWorkingLine.Show();
        }

        private void tHXADiamondLotBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.tHXADiamondLotBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.devTHLStoreDataSet);

        }

        private void DiamondTransaction_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'devTHLStoreDataSet.THXADiamondLot' table. You can move, or remove it, as needed.
            this.tHXADiamondLotTableAdapter.Fill(this.devTHLStoreDataSet.THXADiamondLot);
            
        }

        private void bt_AddWorkingLineNew_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(tb_DocTypeIDNew.Text.Trim(), out int DocTypeIDNew))
            {
                MessageBox.Show("Invalid Document Type","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            docTypeIDNew = DocTypeIDNew;
            WorkingLine InWorkingLine = new WorkingLine(this);            
            //InWorkingLine.RowCalculated += WorkingLines_RowCalculated;
            //InWorkingLine.AddCertificate += WorkingLines_StoreCertificateData;
            InWorkingLine.RowCalculatedOrCertificateAdded += WorkingLines_RowCalculated2;
            InWorkingLine.Show();
        }

        private void InitializeDgv_LinesTable(int newDocTypeID)
        {
           
            List<string> columnNames = new List<string>();
            if (ColumnManager.DocDetailColumnByDocType.ContainsKey(newDocTypeID))
            {
                columnNames = ColumnManager.DocDetailColumnByDocType[newDocTypeID];
            }

            if (ColumnManager.DocDetailColumnByDocType.ContainsKey(newDocTypeID))
            {
                foreach (var columnName in columnNames)
                {
                    dgv_Lines.Columns.Add(columnName, columnName);
                }
            }
            else
            {
                MessageBox.Show("Unknown Document Type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Close();
            }

            //dgv_Lines.WorkingLine.Add(3); // 3 rows (Row 0, Row 1, and Row 2 for calculations result)
            //this.Controls.Add(dgv_WorkingLine);
            
        }

        private void WorkingLines_RowCalculated2(object sender, GlobalDataManager.RowCalculatedEventArgs e)
        {
            List<DataGridViewRow> rowList = e.WorkingLine;
            CertificateData certificateData = e.Certificate;


            if (rowList == null)
            {
                MessageBox.Show("No row data received from working line", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cb_CurrencyNew.Enabled = false;
            int lineID = GenerateNewLineID();
            int newRowIdx = dgv_Lines.Rows.Add();
            DataGridViewRow newRow = dgv_Lines.Rows[newRowIdx];

            // Mapping columns between dgv_WorkingLine and dgv_Lines
            newRow.Cells["LineID"].Value = lineID;
            newRow.Cells["LotID"].Value = rowList[2].Cells["LotID"].Value;
            newRow.Cells["ItemID"].Value = rowList[2].Cells["ItemID"].Value;
            newRow.Cells["LotName"].Value = rowList[2].Cells["LotName"].Value;
            newRow.Cells["Quantity"].Value = rowList[1].Cells["Quantity"].Value;
            newRow.Cells["Weight"].Value = rowList[1].Cells["Weight"].Value;
            newRow.Cells["Cost"].Value = rowList[1].Cells["Cost"].Value;
            newRow.Cells["TotalCost"].Value = rowList[1].Cells["TotalCost"].Value;
            newRow.Cells["Shape"].Value = rowList[2].Cells["Shape"].Value;
            newRow.Cells["Size"].Value = rowList[2].Cells["Size"].Value;
            newRow.Cells["Color"].Value = rowList[2].Cells["Color"].Value;
            newRow.Cells["Clarity"].Value = rowList[2].Cells["Clarity"].Value;
            newRow.Cells["ParcelOrStone"].Value = rowList[2].Cells["ParcelOrStone"].Value;
            newRow.Cells["HoldingType"].Value = rowList[2].Cells["HoldingType"].Value;

            if (rowList[2].Cells["ParcelOrStone"].Value.ToString() == "Stone")
            {
                newRow.Cells["CertificateID"].Value = MaxCertificateID + 1;
                MaxCertificateID = MaxCertificateID + 1;
            }



            if (docCurrency == "USD")
            {
                newRow.Cells["DocPrice"].Value = rowList[1].Cells["Cost"].Value;
                newRow.Cells["TotalDocPrice"].Value = rowList[1].Cells["TotalCost"].Value;
            }
            else if (docCurrency == "GBP")
            {
                newRow.Cells["DocPrice"].Value = rowList[1].Cells["Cost(£)"].Value;
                newRow.Cells["TotalDocPrice"].Value = rowList[1].Cells["TotalCost(£)"].Value;
            }

            MaxLotID = Math.Max(MaxLotID, Convert.ToInt32(rowList[2].Cells["LotID"].Value));
            if (rowList[2].Cells["ParcelOrStone"].Value.ToString() == "Parcel" && rowList[2].Cells["HoldingType"].Value.ToString() == "Permanent")
                MaxItemID = Math.Max(MaxItemID, Convert.ToInt32(rowList[2].Cells["ItemID"].Value));

            newRow.Cells["LineStatus"].Value = "A";

            CalculateSumsAndPositionTextBoxes();

            tb_CertNo.Text = certificateData.CertificateNo;
            certificateDataDictionary.Add(lineID, certificateData);

            CalculateGrandTotalPrice(null,null);

            /*
            // Additional Columns (if needed)
            if (dgv_Lines.Columns.Contains("LineID"))   // Custom function for unique ID
            if (dgv_Lines.Columns.Contains("LotName")) newRow.Cells["LotName"].Value = "Default Name"; // Set a default or get from another source
            if (dgv_Lines.Columns.Contains("Shape")) newRow.Cells["Shape"].Value = "Unknown"; // Set a default value
            if (dgv_Lines.Columns.Contains("Size")) newRow.Cells["Size"].Value = "N/A";
            if (dgv_Lines.Columns.Contains("Color")) newRow.Cells["Color"].Value = "N/A";
            if (dgv_Lines.Columns.Contains("Clarity")) newRow.Cells["Clarity"].Value = "N/A";
            if (dgv_Lines.Columns.Contains("DocPrice($)")) newRow.Cells["DocPrice($)"].Value = 0.00m;
            if (dgv_Lines.Columns.Contains("TotalDocPrice($)")) newRow.Cells["TotalDocPrice($)"].Value = 0.00m;
            if (dgv_Lines.Columns.Contains("CertificateID")) newRow.Cells["CertificateID"].Value = "Not Available";
            if (dgv_Lines.Columns.Contains("LineStatus")) newRow.Cells["LineStatus"].Value = "Pending"; // Default status
            */

            MessageBox.Show("Row added successfully to dgv_Lines!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void WorkingLines_RowCalculated(object sender, List<DataGridViewRow> e)
        {

            if (e == null)
            {
                MessageBox.Show("No row data received from working line", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cb_CurrencyNew.Enabled = false;
            int lineID = GenerateNewLineID();
            int newRowIdx = dgv_Lines.Rows.Add();
            DataGridViewRow newRow = dgv_Lines.Rows[newRowIdx];

            // Mapping columns between dgv_WorkingLine and dgv_Lines
            newRow.Cells["LineID"].Value = lineID;
            newRow.Cells["LotID"].Value = e[2].Cells["LotID"].Value;
            newRow.Cells["ItemID"].Value = e[2].Cells["ItemID"].Value;
            newRow.Cells["LotName"].Value = e[2].Cells["LotName"].Value;
            newRow.Cells["Quantity"].Value = e[1].Cells["Quantity"].Value;
            newRow.Cells["Weight"].Value = e[1].Cells["Weight"].Value;
            newRow.Cells["Cost"].Value = e[1].Cells["Cost"].Value;
            newRow.Cells["TotalCost"].Value = e[1].Cells["TotalCost"].Value;
            newRow.Cells["Shape"].Value = e[2].Cells["Shape"].Value;
            newRow.Cells["Size"].Value = e[2].Cells["Size"].Value;
            newRow.Cells["Color"].Value = e[2].Cells["Color"].Value;
            newRow.Cells["Clarity"].Value = e[2].Cells["Clarity"].Value;
            newRow.Cells["ParcelOrStone"].Value = e[2].Cells["ParcelOrStone"].Value;
            newRow.Cells["HoldingType"].Value = e[2].Cells["HoldingType"].Value;

            if (e[2].Cells["ParcelOrStone"].Value.ToString() == "Stone")
            { 
                newRow.Cells["CertificateID"].Value = MaxCertificateID + 1;
                MaxCertificateID = MaxCertificateID + 1;
            }
                


            if (docCurrency == "USD")
            {
                newRow.Cells["DocPrice"].Value = e[1].Cells["Cost"].Value;
                newRow.Cells["TotalDocPrice"].Value = e[1].Cells["TotalCost"].Value;
            }
            else if (docCurrency == "GBP")
            {
                newRow.Cells["DocPrice"].Value = e[1].Cells["Cost(£)"].Value;
                newRow.Cells["TotalDocPrice"].Value = e[1].Cells["TotalCost(£)"].Value;
            }

            MaxLotID = Math.Max(MaxLotID, Convert.ToInt32(e[2].Cells["LotID"].Value));
            if (e[2].Cells["ParcelOrStone"].Value.ToString() == "Parcel" && e[2].Cells["HoldingType"].Value.ToString() == "Permanent")
            MaxItemID = Math.Max(MaxItemID, Convert.ToInt32(e[2].Cells["ItemID"].Value));

            newRow.Cells["LineStatus"].Value = "A";

            CalculateSumsAndPositionTextBoxes();

            /*
            // Additional Columns (if needed)
            if (dgv_Lines.Columns.Contains("LineID"))   // Custom function for unique ID
            if (dgv_Lines.Columns.Contains("LotName")) newRow.Cells["LotName"].Value = "Default Name"; // Set a default or get from another source
            if (dgv_Lines.Columns.Contains("Shape")) newRow.Cells["Shape"].Value = "Unknown"; // Set a default value
            if (dgv_Lines.Columns.Contains("Size")) newRow.Cells["Size"].Value = "N/A";
            if (dgv_Lines.Columns.Contains("Color")) newRow.Cells["Color"].Value = "N/A";
            if (dgv_Lines.Columns.Contains("Clarity")) newRow.Cells["Clarity"].Value = "N/A";
            if (dgv_Lines.Columns.Contains("DocPrice($)")) newRow.Cells["DocPrice($)"].Value = 0.00m;
            if (dgv_Lines.Columns.Contains("TotalDocPrice($)")) newRow.Cells["TotalDocPrice($)"].Value = 0.00m;
            if (dgv_Lines.Columns.Contains("CertificateID")) newRow.Cells["CertificateID"].Value = "Not Available";
            if (dgv_Lines.Columns.Contains("LineStatus")) newRow.Cells["LineStatus"].Value = "Pending"; // Default status
            */

            MessageBox.Show("Row added successfully to dgv_Lines!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
        }

        private void dgv_Lines_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string columnName = dgv_Lines.Columns[e.ColumnIndex].Name;
            string parcelOrStone = dgv_Lines.Rows[e.RowIndex].Cells["ParcelOrStone"].Value?.ToString();

            if (columnName == "Weight" || columnName == "LotName")
            {
                e.CellStyle.Font = new Font(dgv_Lines.Font, FontStyle.Bold);
            }

            if (columnName == "DocPrice" || columnName == "TotalDocPrice")
            {
                e.CellStyle.ForeColor = Color.Red;
            }


            if (columnName == "Size" && parcelOrStone == "Parcel")
            {
                return;
            }

            if (e.Value != null && columnName != "LineID" && columnName != "LotID" && columnName != "ItemID" && columnName != "Quantity" && columnName != "CertificateID")
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal result))
                {
                    e.Value = result.ToString("N2");
                    e.FormattingApplied = true;
                }
            }
        }

        private int GenerateNewLineID()
        {
            return dgv_Lines.Rows.Count + 1; 
        }


        private void CalculateSumsAndPositionTextBoxes()
        {
            dgv_Lines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            int sumQuantity = 0;
            decimal sumWeight = 0;
            decimal sumTotalDocPrice = 0;


            // Loop through the rows to calculate sums for each column
            foreach (DataGridViewRow row in dgv_Lines.Rows)
            {
                if (row.IsNewRow) continue; // Skip the new row

                sumQuantity += Convert.ToInt32(row.Cells["Quantity"].Value);
                sumWeight += Convert.ToDecimal(row.Cells["Weight"].Value);
                sumTotalDocPrice += Convert.ToDecimal(row.Cells["TotalDocPrice"].Value);
            }

            tb_SumQuantity.Text = sumQuantity.ToString("N0");
            tb_SumWeight.Text = sumWeight.ToString("N2");
            tb_AvgDocPrice.Text = Math.Round(sumTotalDocPrice / sumWeight, 2).ToString("N2");
            tb_SumTotalDocPrice.Text = sumTotalDocPrice.ToString("N2");
            tb_TotalDocPriceNew.Text = sumTotalDocPrice.ToString("N2");

            // Position the textboxes below the DataGridView columns
            PositionColumnSumTextBoxes();
        }

        private void PositionColumnSumTextBoxes()
        {

            int quantityColumnIndex = dgv_Lines.Columns["Quantity"].Index;
            int weightColumnIndex = dgv_Lines.Columns["Weight"].Index;
            int docPriceColumnIndex = dgv_Lines.Columns["DocPrice"].Index;
            int totalDocPriceColumnIndex = dgv_Lines.Columns["TotalDocPrice"].Index;

            // Position TextBoxes below each column
            tb_SumQuantity.Left = dgv_Lines.GetColumnDisplayRectangle(quantityColumnIndex, true).Left;
            tb_SumQuantity.Top = dgv_Lines.Bottom + 2;
            tb_SumQuantity.Width = dgv_Lines.GetColumnDisplayRectangle(quantityColumnIndex, true).Width;

            tb_SumWeight.Left = dgv_Lines.GetColumnDisplayRectangle(weightColumnIndex, true).Left;
            tb_SumWeight.Top = dgv_Lines.Bottom + 2;  // 10 pixels below the DataGridView
            tb_SumWeight.Width = dgv_Lines.GetColumnDisplayRectangle(weightColumnIndex, true).Width;

            tb_AvgDocPrice.Left = dgv_Lines.GetColumnDisplayRectangle(docPriceColumnIndex, true).Left;
            tb_AvgDocPrice.Top = dgv_Lines.Bottom + 2;  // 10 pixels below the DataGridView
            tb_AvgDocPrice.Width = dgv_Lines.GetColumnDisplayRectangle(docPriceColumnIndex, true).Width;

            tb_SumTotalDocPrice.Left = dgv_Lines.GetColumnDisplayRectangle(totalDocPriceColumnIndex, true).Left;
            tb_SumTotalDocPrice.Top = dgv_Lines.Bottom + 2;  // 10 pixels below the DataGridView
            tb_SumTotalDocPrice.Width = dgv_Lines.GetColumnDisplayRectangle(totalDocPriceColumnIndex, true).Width;
        }

        private void CalculateGrandTotalPrice(object sender, EventArgs e)
        {
            decimal totalDocPrice;  
            decimal discount = 0;
            decimal subTotal;
            decimal additionalCost = 0;
            decimal tax = 0;
            decimal grandTotalDocPrice;

            if (!decimal.TryParse(tb_TotalDocPriceNew.Text.Trim(), out totalDocPrice))
            {
                MessageBox.Show("Invalid TotalDocPrice", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal.TryParse(tb_DiscountNew.Text.Trim(), out discount);
            decimal.TryParse(tb_AdditionalNew.Text.Trim(), out additionalCost);
            decimal.TryParse(tb_TaxNew.Text.Trim(), out tax);

            subTotal = totalDocPrice - discount;
            grandTotalDocPrice = subTotal + additionalCost + tax;

            tb_SubTotalNew.Text = subTotal.ToString("N2");
            tb_GrandTotalPriceNew.Text = grandTotalDocPrice.ToString("N2");
        }

        //certificate dictionary
        private void WorkingLines_StoreCertificateData(object sender, CertificateData e)
        {
           // tb_CertNo.Text = e.CertificateNo;

            
        }

        private void bt_CancelNewTransaction_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show (
            "Are you sure you want to cancel the document?",
            "Confirm Cancel",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                GlobalClass.ResetControls(this.tab_TrsNew);
                cb_CurrencyNew.Enabled = true;
                tab_Transaction.SelectedIndex = 0;
            }

            IsCreatingNewDoc = false;
        }

        //MaxLotID,MaxItemID,MaxCertificateID
        private void AccessDiamondLotMaxItemLotIDAndCertificateID()
        {
            try
            {

                string query = @"
                SELECT 
                (SELECT MAX(ItemID) as MAXItemID FROM THXADiamondLot where ItemID = LotID) AS MaxItemID,
                (SELECT MAX(LotID) FROM THXADiamondLot) AS MaxLotID,
                (SELECT MAX(CertificateID) FROM THXADiamondCertificate) AS MaxCertificateID";
                string errorMessageTitle = "Access DiamondLot MAXLotID, MaxItemID, MaxCertificateID ";

                DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
                if (GlobalClass.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
                {
                    MaxItemID = Convert.ToInt32(sqlResult.Rows[0]["MaxItemID"]);
                    MaxLotID = Convert.ToInt32(sqlResult.Rows[0]["MaxLotID"]);
                    MaxCertificateID = Convert.ToInt32(sqlResult.Rows[0]["MaxCertificateID"]);
                }

            }
            catch (Exception e1)
            {

                GlobalClass.ShowErrorMessage(e1, "Access ItemID, LotID, CertificateID in THXADiamondLot failure.");

            }
        }

        private bool IsInputValid(string ParcelOrStone)
        {
            Control[] controlsToCheck;
            if (ParcelOrStone == "Parcel")
            {
                controlsToCheck = new Control[]
                {

                };
            }
            else if (ParcelOrStone == "Cert")
            {
                controlsToCheck = new Control[]
                {

                };
            }
            else
            {
                throw new ArgumentException("Invalid argument at IsInPutValid function", nameof(ParcelOrStone));

            }



            foreach (var control in controlsToCheck)
            {
                if (string.IsNullOrWhiteSpace(control.Text))
                {
                    control.Focus();
                    control.BackColor = Color.Yellow;
                    return false;
                }
            }


            return true;
        }

        //Section----Update Document Details ----------------------------------------------------------------------------------------------------------
        private void bt_SaveNewTransaction_Click(object sender, EventArgs e)
        {
            string errorMessageTitle, query;
            if (!IsAuthorisedUser())
            {
                MessageBox.Show("This acccount is not allowed to update DiamondItemMaintenance.", "Message");
                return;
            }

            if (!IsInputValid("Parcel"))
            {
                MessageBox.Show("Parcel Detail is not completed.", "Message");
                return;
            }

            var parameters = new Dictionary<string, object>
            {
                {"@DocTypeDesc", cb_DocTypeDescNew.Text.Trim()},
                {"@DocTypeID", tb_DocTypeIDNew.Text.Trim()},
                {"@DocID", tb_DocIDNew.Text.Trim()},
                {"@DocCode", tb_DocCodeNew.Text.Trim()},
                {"@DocSubTypeID", cb_DocSubTypeIDNew.Text.Trim()},
                {"@AccountCode", cb_AccountCodeNew.Text.Trim()},
                {"@AccountBranchCode", cb_AccountBranchCodeNew.Text.Trim()},
                {"@AccountName", cb_AccountNameNew.Text.Trim()},
                {"@DocAccountRefNo", tb_DocAccountRefNoNew.Text.Trim()},
                {"@RefDocDate", dateTime_RefDocDateNew.Text.Trim()},
                {"@Remark1", tb_Remark1New.Text.Trim()},
                {"@DocStatus", cb_DocStatusNew.Text.Trim()},
                {"@DocDate", dateTime_DocDateNew.Text.Trim()},
                {"@VatCode", ""},
                {"@VatRate", "0"},
                {"@PaymentTerm", cb_PaymentTermNew.Text.Trim()},
                {"@DueDate", dateTime_DueDateNew.Text.Trim()},
                {"@Currency", cb_CurrencyNew.Text.Trim()},
                {"@USDRate", tb_USDRateNew.Text.Trim()},
                {"@GBPRate", tb_GBPRateNew.Text.Trim()},
                {"@InternalRemark", tb_InternalRemarkNew.Text.Trim()},
                {"@LastUpdate", DateTime.Now }
            };     

            if (IsCreatingNewDoc)
            {
                errorMessageTitle = "Create New DiamondDocHeader";
                query = "INSERT INTO THXADiamondDocHeader " +
                    "(DocTypeID, DocTypeDesc, DocID, DocCode, DocAccountRefNo, DocSubType, DocStatus, DocDate, " +
                    "RefDocDate, DueDate, LastUpdate, AccountCode, AccountBranchCode, AccountName, AccountLongName, Currency, " +
                    "VatCode, VatRate, PaymentTerm, Remark1, Remark2, InternalRemark, UserID, Quantity, Weight, " +
                    "MainLinesTotalPrice, SecLinesTotalPrice, DocLinesTotalPrice, MainDiscountPrice, SecDiscountPrice, DocDiscountPrice, DiscountPercent, DiscountRemark, " +
                    "MainSubTotalPrice, SecSubTotalPrice, DocSubTotalPrice, MainAdditionalPrice, SecAdditionalPrice, DocAdditionalPrice, AdditionalRemark, " +
                    "MainTaxPrice, SecTaxPrice, DocTaxPrice, MainGrandTotalPrice, SecGrandTotalPrice, DocGrandTotalPrice, RateDocToMain, RateDocToSec) " +
                    "VALUES " +
                    "(@DocTypeID, @DocTypeDesc, @DocID, @DocCode, @DocAccountRefNo, @DocSubType, @DocStatus, @DocDate, " +
                    "@RefDocDate, @DueDate, @LastUpdate, @AccountCode, @AccountBranchCode, @AccountName, @AccountName, @Currency, " +
                    "@VatCode, @VatRate, @PaymentTerm, @Remark1, @Remark2, @InternalRemark, @UserID, @Quantity, @Weight, " +
                    "@MainLinesTotalPrice, @SecLinesTotalPrice, @DocLinesTotalPrice, @MainDiscountPrice, @SecDiscountPrice, @DocDiscountPrice, @DiscountPercent, @DiscountRemark, " +
                    "@MainSubTotalPrice, @SecSubTotalPrice, @DocSubTotalPrice, @MainAdditionalPrice, @SecAdditionalPrice, @DocAdditionalPrice, @AdditionalRemark, " +
                    "@MainTaxPrice, @SecTaxPrice, @DocTaxPrice, @MainGrandTotalPrice, @SecGrandTotalPrice, @DocGrandTotalPrice, @RateDocToMain, @RateDocToSec)";

                SetConnectionString();
                if (GlobalClass.UpdateTableData(ref Connection, query, parameters, errorMessageTitle))
                {
                    MessageBox.Show("Create parcel successfully", "Message");
                }


            }

            RefreshDoc();
            if (IsCreatingNewDoc) IsCreatingNewDoc = false;
        }

        private void RefreshDoc() { 
        
        }

    }
}
