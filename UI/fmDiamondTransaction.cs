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
using DiamondTransaction.UseCases;
using DiamondTransaction.Domain.Entities;
using DiamondTransaction.UI.Utilities;
using DiamondTransaction.Properties;
using System.Xml.Linq;
using static DiamondTransaction.UI.ColumnConfigurations;
//using System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace DiamondTransaction
{
    public partial class fmDiamondTransaction : Form
    {
        public string User = "FLEUR";
        public string Name = "FLEUR";
        SqlConnection Connection = null;
        public string connectionString = @"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql";
        

        public int currentRowIndex;

        public List<string> supplierNames = new List<string>();
        public List<string> customerNames = new List<string>();
        public List<int> supplierDocTypeIDs;

        //MEMO IN 1009, MEMO IN RETURN 1010,PURCHASE MEMO IN 1011,PURCHASE NOTE 1006,PURCHASE NOTE RETURN 1007
        //HashSet<int> supplierDocTypes = new HashSet<int> { 1006, 1007, 1009, 1010, 1011 };
        private bool isProgrammaticChange = false;
        private DocDetailControls DocDetail = null;
        private DocDetailControls NewDocDetail = null;

        public decimal USDRate, GBPRate; public string docCurrency;
        private string originalUSDExRate;

        private int MaxDocID; string MaxDocCode;
        public int DocTypeID,  DocTypeIDNew;
        public int dgv_LinesMaxLotID;

        public bool IsCreatingNewDoc = false;
        public int MaxItemID = 0, MaxLotID = 0, MaxCertificateID = 0;

        private Dictionary<int, CertificateDto> certificateDataDictionary = new Dictionary<int, CertificateDto>();
        private DiamondDocDataAccess _docRepository;
        private DocHeaderDataAccess _docHeaderRepository;
        private DocLineDataAccess _docLineRepository;

        private List<DocHeaderDto> _docHeaders;
        private List<DocLineDto> _docLines;
        private List<Supplier> _suppliers;
        private List<Customer> _customers;
        private List<DocType> _docTypes;
        private DocHeaderMaxInfo _docHeaderMaxInfo;
        private DiamondLotMaxInfo _diamondLotMaxInfo;
        private DocLineDto _docLineDto = new DocLineDto();
        private DocLineDto _docLineDtoNew;
        private CertificateDto _certificateDto = new CertificateDto();
        private CertificateDto _certificateDtoNew;




        //Application Dependency
        private CustomerService _customerService;
        private SupplierService _supplierService;
        private DiamondDocService _diamondDocService;
        private DocCreationService _docCreationService;
        private DocHeaderService _docHeaderService;
        private DocLineService _docLineService;



        public fmDiamondTransaction()
        {
            InitializeComponent();
            //Helper class
            DocDetail = new DocDetailControls(cb_DocTypeDesc, tb_DocTypeID, cb_DocSubTypeID, cb_AccountCode, cb_AccountName, cb_AccountBranchCode, cb_PaymentTerm);
            NewDocDetail = new DocDetailControls(cb_DocTypeDescNew, tb_DocTypeIDNew, cb_DocSubTypeIDNew, cb_AccountCodeNew, cb_AccountNameNew, cb_AccountBranchCodeNew, cb_PaymentTermNew);

            
            SetConnectionString(); //When build as a library, this line should move to SetParameter()
            //Application Dependency-----
            InitializeServices(); //When build as a library, this line should move to SetParameter()

            this.Load += fmDiamondTransaction_Load;
       

        }
        private void SetConnectionString()
        {
            Connection = new SqlConnection(connectionString);
            SqlQueryExecutor.setConnectionString(connectionString);
            //Connection = new SqlConnection(@"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql");
            //GlobalClass.setConnectionString(@"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql");

        }

        public void SetParameter(string connectionString, string user, string name)
        {
            this.connectionString = connectionString;
            SqlQueryExecutor.setConnectionString(connectionString);
            this.User = user;
            this.Name = name;
            // InitializeServices(); Added it here when go live

        }

        private bool IsAuthorisedUser()
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

        private void InitializeServices()
        {
            var docHeaderRepository = new DocHeaderDataAccess(connectionString);
            _docHeaderService = new DocHeaderService(docHeaderRepository);

            var docLineRepository = new DocLineDataAccess(connectionString);
            _docLineService = new DocLineService(docLineRepository);

            var customerRepository = new CustomerDataAccess(connectionString);
            _customerService = new CustomerService(customerRepository);

            var supplierRepository = new SupplierDataAccess(connectionString);
            _supplierService = new SupplierService(supplierRepository);

            var diamondDocRepository = new DiamondDocDataAccess(connectionString);
            _diamondDocService = new DiamondDocService(diamondDocRepository);

            _docCreationService = new DocCreationService(_diamondDocService);
        }

        private async void fmDiamondTransaction_Load(object sender, EventArgs e)
        {         
            //OldAccessDiamondDocHeader();          
            await InitializeDataAndControls();      
        }

        private async Task InitializeDataAndControls()
        {
            await LoadData();
            InitialiseControls(); 
        }

        private async Task LoadData() {

            await LoadAndBindDocHeaders();
            LoadExRate();
            LoadDocTypes();
            LoadCustomerData();
            LoadSupplierData();
            LoadSupplierDocTypeIDs();
            


        }

        private void InitialiseControls()
        {
            
            InitialiseDocTypeDescComboBox(cb_FilterDocTypeDesc);
            InitialiseDocTypeDescComboBox(cb_DocTypeDesc);
            InitialiseDocTypeDescComboBox(cb_DocTypeDescNew);
            InitialiseCustomerComboBox(cb_FilterAccountCode, cb_FilterAccountName);          
        }

        private async Task LoadAndBindDocHeaders()
        {
            _docHeaders = (await _docHeaderService.GetAllDiamondDocHeadersAsync()).ToList();
            dgv_DocHeader.DataSource = _docHeaders;
            SetDocHeaderColumnFormat(dgv_DocHeader);
        }

        private void SetDocHeaderColumnFormat(DataGridView grid)
        {
            var columnConfig = new DocHeaderColumnConfiguration();
            var builder = new GridColumnBuilder(columnConfig);
            builder.ConfigureGridFormat(grid, 0);
        }

        private void LoadCustomerData()
        {
            _customers = _customerService.GetAllCustomers();
        }
        private void LoadSupplierData()
        {
            _suppliers = _supplierService.GetAllSuppliers();
        }
        private void LoadSupplierDocTypeIDs() { 
            
            supplierDocTypeIDs = _diamondDocService.GetSupplierDocTypeIDs();        
        }

        private void LoadDocTypes() {
            _docTypes = _diamondDocService.GetAllDocTypes();
        }

        private void LoadExRate()
        {
            var exRate = _diamondDocService.GetLatestExchangeRate();
            if (exRate != null)
            {
                USDRate = exRate.USDRate;
                GBPRate = exRate.GBPRate;
                originalUSDExRate = USDRate.ToString();
            }
        }

        private void setDecimalPlaceTo2(object sender, EventArgs e)
        {
            FormControlHelper.setDecimalPlacesFormat(sender, 2);
        }

        private void setDecimalPlaceTo3(object sender, EventArgs e)
        {
            FormControlHelper.setDecimalPlacesFormat(sender, 3);
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
            FormControlHelper.comboBox_DynamicDropDownWidth(sender, e);
        }


        private void dgv_DocHeader_DoubleClick(object sender, EventArgs e)
        {
            tab_Transaction.SelectedIndex = 2;
        }

        private async void tab_Transaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (tab_Transaction.SelectedIndex == 2) 
            { 
                if (dgv_DocHeader.SelectedRows.Count > 0 )
                {
                    currentRowIndex = dgv_DocHeader.SelectedRows[0].Index;
                    FillTransactionDetail();
                    await LoadDocLinesAndSetFormat();
                }

                else
                {
                    // Optional: Handle the case where no row is selected
                    currentRowIndex = -1; // Or any default value
                    MessageBox.Show("No row selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }           
            }                
            else if(tab_Transaction.SelectedIndex == 3)
            {

                
            }

            
        }


        


        private void FillTransactionDetail() 
        {
            if (dgv_DocHeader.SelectedRows.Count > 0)
            {
                int selectedIndex = dgv_DocHeader.SelectedRows[0].Index;
                currentRowIndex = selectedIndex;

                var dto = dgv_DocHeader.Rows[selectedIndex].DataBoundItem as DocHeaderDto; //Safe cast – returns null if not castable
                if (dto == null) return;

                cb_DocTypeDesc.Text = dto.DocTypeDesc;
                tb_DocTypeID.Text = dto.DocTypeID.ToString();
                tb_DocID.Text = dto.DocID.ToString();
                tb_DocCode.Text = dto.DocCode;
                cb_DocSubTypeID.Text = dto.DocSubType;
                cb_AccountCode.Text = dto.AccountCode;
                cb_AccountBranchCode.Text = dto.AccountBranchCode;
                cb_AccountName.Text = dto.AccountName;
                dateTime_RefDocDate.Text = dto.RefDocDate?.ToString("yyyy-MM-dd") ?? "";
                tb_Remark1.Text = dto.Remark1;
                cb_DocStatus.Text = dto.DocStatus;
                dateTime_DocDate.Text = dto.DocDate?.ToString("yyyy-MM-dd") ?? "";
                cb_PaymentTerm.Text = dto.PaymentTerm.ToString();
                dateTime_DueDate.Text = dto.DueDate?.ToString("yyyy-MM-dd") ?? "";
                cb_Currency.Text = dto.Currency;
                tb_USDRate.Text = dto.RateDocToMain?.ToString();
                tb_GBPRate.Text = dto.RateDocToSec?.ToString();
                tb_InternalRemark.Text = dto.InternalRemark;
            }
        }



        private void OldFillTransactionDetail()
        {
            
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

        private void OldApplyFilterDocHeader(object sender, EventArgs e)
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

                FormControlHelper.ApplyFilter(filterExpression, filters);

                ((DataView)dgv_DocHeader.DataSource).RowFilter = filterExpression.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

                var filteredList = FormControlHelper.FilterDtoList(_docHeaders, filters);
                dgv_DocHeader.DataSource = null;
                dgv_DocHeader.DataSource = filteredList;
                SetDocHeaderColumnFormat(dgv_DocHeader);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //Not In Use Filter -----------------------------------------------------------------------------------------------------
        private List<DocHeaderDto> FilterDocHeaderDtoList(IEnumerable<DocHeaderDto> data, Dictionary<string, (Control Control, bool IsExactMatch)> filters)
        {
            return data.Where(item =>
            {
                foreach (var filter in filters)
                {
                    var propertyName = filter.Key;
                    var (control, isExactMatch) = filter.Value;

                    string input = FormControlHelper.GetControlValue(control);
                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    var prop = typeof(DocHeaderDto).GetProperty(propertyName);
                    if (prop == null)
                        continue;

                    var value = prop.GetValue(item)?.ToString() ?? "";

                    if (isExactMatch)
                    {
                        if (!string.Equals(value, input, StringComparison.OrdinalIgnoreCase))
                            return false;
                    }
                    else
                    {
                        if (!value.ToLower().Contains(input.ToLower()))
                            return false;
                    }
                }
                return true;
            }).ToList();
        }


        private List<T> FilterGenericDtoList<T>(IEnumerable<T> data, Dictionary<string, (Control Control, bool IsExactMatch)> filters)
        {
            return data.Where(item =>
            {
                foreach (var filter in filters)
                {
                    var propertyName = filter.Key;
                    var (control, isExactMatch) = filter.Value;

                    string input = FormControlHelper.GetControlValue(control);
                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    var prop = typeof(T).GetProperty(propertyName);
                    if (prop == null)
                        continue;

                    var value = prop.GetValue(item)?.ToString() ?? "";

                    if (isExactMatch)
                    {
                        if (!string.Equals(value, input, StringComparison.OrdinalIgnoreCase))
                            return false;
                    }
                    else
                    {
                        if (!value.ToLower().Contains(input.ToLower()))
                            return false;
                    }
                }
                return true;
            }).ToList();
        }

        //-------------------------------------------------------------------------------------------


        private void InitialiseDocTypeDescComboBox(ComboBox cb_docTypeDesc)
        {            
            List<string> docTypeDesc = _docTypes.Select(d => d.DocTypeDesc).ToList();
            FormControlHelper.SetComboBoxItem(cb_docTypeDesc, docTypeDesc);
        }


        private void cb_FilterDocTypeDesc_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb_sender = (ComboBox)sender;
            string docTypeDesc = cb_sender.Text;
            string docTypeID = _diamondDocService.GetDocTypeIDByDocTypeDesc(docTypeDesc);
            InitialseAccountCode(docTypeID, cb_FilterAccountCode, cb_FilterAccountName);
            InitialiseDocSubTypeComboBox(docTypeDesc, cb_FilterDocSubType);

        }
        private void cb_DocTypeDesc_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDocType(DocDetail);
        }

        private void ChangeDocType(DocDetailControls controls)
        {            
            string docTypeDesc = controls.DocTypeDescComboBox.Text;
            string docTypeID = _diamondDocService.GetDocTypeIDByDocTypeDesc(docTypeDesc);
            controls.DocTypeIDTextBox.Text = docTypeID;

            InitialseAccountCode(docTypeID, controls.AccountCodeComboBox, controls.AccountNameComboBox);
            InitialiseDocSubTypeComboBox(docTypeDesc, controls.DocSubTypeComboBox);
            
        }

        private void InitialiseDocSubTypeComboBox(string docTypeDesc, ComboBox cb_docSubType)
        {
            List<string> docSubTypes = _diamondDocService.GetDocSubTypesByDocType(docTypeDesc);
            FormControlHelper.SetComboBoxItem(cb_docSubType, docSubTypes);
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

        private void InitialiseSupplierComboBox(ComboBox cb_accountCode, ComboBox cb_accountName)
        {
            List<string> supplierCodes = _suppliers.Select(s => s.SupplierCode).ToList();
            List<string> supplierNames = _suppliers.Select(s => s.SupplierName).ToList();

            FormControlHelper.SetComboBoxItem(cb_accountCode, supplierCodes);
            FormControlHelper.SetComboBoxItem(cb_accountName, supplierNames);
          
        }

        private void InitialiseCustomerComboBox(ComboBox cb_accountCode, ComboBox cb_accountName)
        {
            
            List<string> customerCodes = _customers.Select(c => c.CustomerCode).ToList();
            List<string> customerNames = _customers.Select(c => c.CustomerName).ToList();

            FormControlHelper.SetComboBoxItem(cb_accountCode, customerCodes);
            FormControlHelper.SetComboBoxItem(cb_accountName, customerNames);

        }
        //----------------------Modify Doc accountcode and accountname------------------------------

        //---------------------- Utility Methods ------------------------------

        private void HandleProgrammaticChange(Action action)
        {
            if (isProgrammaticChange) return;

            isProgrammaticChange = true;
            action.Invoke();
            isProgrammaticChange = false;
        }

        //---------------------- Supplier-Specific Methods ------------------------------

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

        private void FillCustomerBranch(string customerCode, ComboBox cb_accountBranchCode)
        {
            try
            {
                var branches = _customerService.GetBranchesByCustomer(customerCode);

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
                SqlQueryExecutor.ShowErrorMessage(e, "Access CustomerBranch failure.");
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


        private async Task LoadDocLinesAndSetFormat()
        {
            int docTypeID = FormControlHelper.GetIntValue(tb_DocTypeID.Text);
            int docID = FormControlHelper.GetIntValue(tb_DocID.Text);

            _docLines = (await _docLineService.GetDiamondDocLinesAsync(docTypeID, docID)).ToList();
            dgv_DocLine.DataSource = _docLines;
            SetDocLineColumnFormat(dgv_DocLine,docTypeID);

        }

        private void SetDocLineColumnFormat(DataGridView grid,int docTypeID)
        {
            var columnConfig = new DocLineColumnConfiguration();
            var builder = new GridColumnBuilder(columnConfig);
            builder.ConfigureGridFormat(grid, docTypeID);          
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
            InitializeDocLineNewColumns(1006);
            //OldAddDocLineTableColumns(1006);           
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
            InitializeDocLineNewColumns(1009);
            //OldAddDocLineTableColumns(1009);
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
            InitializeDocLineNewColumns(1001);
            //OldAddDocLineTableColumns(1001);
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
            //OldAddDocLineTableColumns(1006);
            InitializeDocLineNewColumns(1006);
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
            InitializeDocLineNewColumns(1007);
            //OldAddDocLineTableColumns(1007);
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
            InitializeDocLineNewColumns(1005);
            //OldAddDocLineTableColumns(1005);
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
            InitializeDocLineNewColumns(1010);
            //OldAddDocLineTableColumns(1010);
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
            InitializeDocLineNewColumns(1002);
            //OldAddDocLineTableColumns(1002);
        }

        private void CreateNewDocSupplier(string docTypeDesc) {

            LoadDocMaxIDs(docTypeDesc);

            var dto = _docCreationService.PrepareNewSupplierDoc(docTypeDesc);
            FillNewDocControls(dto);

            tab_Transaction.SelectedIndex = 3;
            cb_AccountBranchCodeNew.Enabled = false;
        }

        private void CreateNewDocCustomer(string docTypeDesc)
        {
            LoadDocMaxIDs(docTypeDesc);

            var dto = _docCreationService.PrepareNewCustomerDoc(docTypeDesc);
            FillNewDocControls(dto);

            tab_Transaction.SelectedIndex = 3;
            cb_AccountBranchCodeNew.Enabled = true;
        }

        private void LoadDocMaxIDs(string docTypeDesc)
        {

            var docHeaderMaxInfo = _diamondDocService.AccessDocHeaderMaxDocIDAndDocCodeFor(docTypeDesc);
            var diamondLotMaxInfo = _diamondDocService.AccessDiamondLotMaxItemLotIDAndCertificateID();

            MaxDocID = docHeaderMaxInfo.MaxDocID;
            MaxDocCode = docHeaderMaxInfo.MaxDocCode;
            MaxItemID = diamondLotMaxInfo.MaxItemID;
            MaxCertificateID = diamondLotMaxInfo.MaxCertificateID;
            MaxLotID = diamondLotMaxInfo.MaxLotID;
        }

        private void FillNewDocControls(DocHeaderSubDto dto)
        {
            cb_DocTypeDescNew.Text = dto.DocTypeDesc;
            cb_DocStatusNew.Text = dto.DocStatus;
            cb_CurrencyNew.Text = dto.Currency;
            tb_DocIDNew.Text = dto.DocID.ToString();
            tb_DocCodeNew.Text = dto.DocCode;
            cb_DocSubTypeIDNew.Text = dto.DocSubType;
            cb_AccountBranchCodeNew.Text = dto.AccountBranchCode;
            tb_DiscountNew.Text = dto.Discount.ToString("F2");
            tb_AdditionalNew.Text = dto.Additional.ToString("F2");
            tb_TaxNew.Text = dto.Tax.ToString("F2");

        }

        private void InitializeDocLineNewColumns(int newDocTypeID)
        {
            var columnConfig = new DocLineColumnConfiguration();
            var builder = new GridColumnBuilder(columnConfig);
            builder.ConfigureGrid(dgv_DocLineNew, newDocTypeID);
            SetDocLineColumnFormat(dgv_DocLineNew, newDocTypeID);

            //this.Controls.Add(dgv_Lines);
        }

        private void OldAddDocLineTableColumns(int newDocTypeID)
        {

            List<string> columnNames = new List<string>();
            if (GridColumns.DocLineColumnByDocType.ContainsKey(newDocTypeID))
            {
                columnNames = GridColumns.DocLineColumnByDocType[newDocTypeID];
            }

            if (GridColumns.DocLineColumnByDocType.ContainsKey(newDocTypeID))
            {
                foreach (var columnName in columnNames)
                {
                    dgv_DocLineNew.Columns.Add(columnName, columnName);
                }
            }
            else
            {
                MessageBox.Show("Unknown Document Type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Close();
            }

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

        
        private void bt_AddWorkingLine_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(tb_DocTypeID.Text.Trim(), out int docTypeID))
            {
                MessageBox.Show("Invalid Document Type");
                return;
            }
            if (IsCreatingNewDoc)
            {
                MessageBox.Show("New row can not be added while a new document is on editing.\nPlease save or cancel the new document then try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
     
            fmWorkingLine InWorkingLine = new fmWorkingLine(_docLineDto, _certificateDto);
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
            if (!int.TryParse(tb_DocTypeIDNew.Text.Trim(), out int docTypeIDNew))
            {
                MessageBox.Show("Invalid Document Type","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            DocTypeIDNew = docTypeIDNew;
            _docLineDtoNew = new DocLineDto
            {
                DocTypeID = docTypeIDNew
            };

            _certificateDtoNew = new CertificateDto(); 

            
            fmWorkingLine InWorkingLine = new fmWorkingLine(_docLineDtoNew, _certificateDtoNew);            
            //InWorkingLine.RowCalculated += WorkingLines_RowCalculated;
            //InWorkingLine.AddCertificate += WorkingLines_StoreCertificateData;
            InWorkingLine.RowCalculatedOrCertificateAdded += WorkingLines_RowCalculated;
            InWorkingLine.Show();
        }



        private void WorkingLines_RowCalculated(object sender, GlobalDataManager.RowCalculatedEventArgs e)
        {
            List<DataGridViewRow> rowList = e.WorkingLine;
            CertificateDto certificateData = e.Certificate;


            if (rowList == null)
            {
                MessageBox.Show("No row data received from working line", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cb_CurrencyNew.Enabled = false;
            int lineID = GenerateNewLineID();
            int newRowIdx = dgv_DocLineNew.Rows.Add();
            DataGridViewRow newRow = dgv_DocLineNew.Rows[newRowIdx];

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

        private void WorkingLines_RowCalculated0(object sender, List<DataGridViewRow> e)
        {

            if (e == null)
            {
                MessageBox.Show("No row data received from working line", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cb_CurrencyNew.Enabled = false;
            int lineID = GenerateNewLineID();
            int newRowIdx = dgv_DocLineNew.Rows.Add();
            DataGridViewRow newRow = dgv_DocLineNew.Rows[newRowIdx];

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
                newRow.Cells["DocPrice"].Value = e[1].Cells["SecCost"].Value;
                newRow.Cells["TotalDocPrice"].Value = e[1].Cells["SecTotalCost"].Value;
            }

            MaxLotID = Math.Max(MaxLotID, Convert.ToInt32(e[2].Cells["LotID"].Value));
            if (e[2].Cells["ParcelOrStone"].Value.ToString() == "Parcel" && e[2].Cells["HoldingType"].Value.ToString() == "Permanent")
            MaxItemID = Math.Max(MaxItemID, Convert.ToInt32(e[2].Cells["ItemID"].Value));

            newRow.Cells["DocLineStatus"].Value = "A";

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
            string columnName = dgv_DocLineNew.Columns[e.ColumnIndex].Name;
            string parcelOrStone = dgv_DocLineNew.Rows[e.RowIndex].Cells["ParcelOrStone"].Value?.ToString();

            if (columnName == "Weight" || columnName == "LotName")
            {
                e.CellStyle.Font = new Font(dgv_DocLineNew.Font, FontStyle.Bold);
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
            return dgv_DocLineNew.Rows.Count + 1; 
        }


        private void CalculateSumsAndPositionTextBoxes()
        {
            dgv_DocLineNew.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            int sumQuantity = 0;
            decimal sumWeight = 0;
            decimal sumTotalDocPrice = 0;


            // Loop through the rows to calculate sums for each column
            foreach (DataGridViewRow row in dgv_DocLineNew.Rows)
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

            int quantityColumnIndex = dgv_DocLineNew.Columns["Quantity"].Index;
            int weightColumnIndex = dgv_DocLineNew.Columns["Weight"].Index;
            int docPriceColumnIndex = dgv_DocLineNew.Columns["DocPrice"].Index;
            int totalDocPriceColumnIndex = dgv_DocLineNew.Columns["TotalDocPrice"].Index;

            // Position TextBoxes below each column
            tb_SumQuantity.Left = dgv_DocLineNew.GetColumnDisplayRectangle(quantityColumnIndex, true).Left;
            tb_SumQuantity.Top = dgv_DocLineNew.Bottom + 2;
            tb_SumQuantity.Width = dgv_DocLineNew.GetColumnDisplayRectangle(quantityColumnIndex, true).Width;

            tb_SumWeight.Left = dgv_DocLineNew.GetColumnDisplayRectangle(weightColumnIndex, true).Left;
            tb_SumWeight.Top = dgv_DocLineNew.Bottom + 2;  // 10 pixels below the DataGridView
            tb_SumWeight.Width = dgv_DocLineNew.GetColumnDisplayRectangle(weightColumnIndex, true).Width;

            tb_AvgDocPrice.Left = dgv_DocLineNew.GetColumnDisplayRectangle(docPriceColumnIndex, true).Left;
            tb_AvgDocPrice.Top = dgv_DocLineNew.Bottom + 2;  // 10 pixels below the DataGridView
            tb_AvgDocPrice.Width = dgv_DocLineNew.GetColumnDisplayRectangle(docPriceColumnIndex, true).Width;

            tb_SumTotalDocPrice.Left = dgv_DocLineNew.GetColumnDisplayRectangle(totalDocPriceColumnIndex, true).Left;
            tb_SumTotalDocPrice.Top = dgv_DocLineNew.Bottom + 2;  // 10 pixels below the DataGridView
            tb_SumTotalDocPrice.Width = dgv_DocLineNew.GetColumnDisplayRectangle(totalDocPriceColumnIndex, true).Width;
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
        private void WorkingLines_StoreCertificateData(object sender, CertificateDto e)
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
                FormControlHelper.ResetControls(this.tab_TrsNew);
                cb_CurrencyNew.Enabled = true;
                tab_Transaction.SelectedIndex = 0;
            }

            IsCreatingNewDoc = false;
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
                if (SqlQueryExecutor.UpdateTableData(ref Connection, query, parameters, errorMessageTitle))
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
