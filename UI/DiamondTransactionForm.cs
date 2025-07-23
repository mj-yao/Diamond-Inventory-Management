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
using System.Xml.Linq;
using DiamondTransaction.Properties;
using DiamondTransaction.DataAccess;
using DiamondTransaction.UseCases.Models;
using DiamondTransaction.UseCases.Services;
using DiamondTransaction.Domain.Entities;
using DiamondTransaction.UI.Utilities;
using DiamondTransaction.UI.Controls;
using DiamondTransaction.UI.GridConfig;
using DiamondTransaction.UI.Event;
using static DiamondTransaction.UI.GridConfig.ColumnConfigurations;
using System.Reflection;
//using System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace DiamondTransaction
{
    public partial class DiamondTransactionForm : Form
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
        private DocHeaderControls DocDetail = null;
        private DocHeaderControls NewDocDetail = null;

        public decimal USDRate, GBPRate; public string docCurrency;
        private string originalUSDExRate;

        private int MaxDocID; string MaxDocCode;
        public int DocTypeID,  DocTypeIDNew;
        public int dgv_LinesMaxLotID;

        public bool IsCreatingNewDoc = false;
        public int MaxItemID = 0, MaxLotID = 0, MaxCertificateID = 0;

        private DiamondDocDataAccess _docRepository;
        private DocHeaderDataAccess _docHeaderRepository;
        private DocLineDataAccess _docLineRepository;

        private List<DocHeaderDto> _docHeaders;
        private List<DocLineDto> _docLines;
        private List<Supplier> _suppliers;
        private List<Customer> _customers;
        private List<DocType> _docTypes;
        private DocHeaderMaxInfo _docHeaderMaxInfo;
        private DiamondLotMaxID _diamondLotMaxID;
        private DocLineDto _docLineDto = new DocLineDto();
        private DocHeaderDto _docHeaderNew = new DocHeaderDto();

        private DocLineDto _docLineDtoNew;
        private CertificateDto _certificate = new CertificateDto();
        private CertificateDto _certificateNew;
        private SourceLotChangeDto _sourceLotChangeDto = new SourceLotChangeDto();
        private SourceLotChangeDto _sourceLotChangeDtoNew;
        private WorkingLineDto _workingLineDtoNew;





        //Application Dependency
        private CustomerService _customerService;
        private SupplierService _supplierService;
        private DiamondDocService _diamondDocService;
        private DocCreationService _docCreationService;
        private DocHeaderService _docHeaderService;
        private DocLineService _docLineService;
        private CertificateService _certificateService;



        public DiamondTransactionForm()
        {
            InitializeComponent();
            //Helper class
            DocDetail = new DocHeaderControls(cb_DocTypeDesc, tb_DocTypeID, cb_DocSubTypeID, cb_AccountCode, cb_AccountName, cb_AccountBranchCode, cb_PaymentTerm);
            NewDocDetail = new DocHeaderControls(cb_DocTypeDescNew, tb_DocTypeIDNew, cb_DocSubTypeIDNew, cb_AccountCodeNew, cb_AccountNameNew, cb_AccountBranchCodeNew, cb_PaymentTermNew);

            
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

            var certificateRepository = new CertificateDataAccess(connectionString);
            _certificateService = new CertificateService(certificateRepository);

            _docCreationService = new DocCreationService(_diamondDocService);
        }

        private async void fmDiamondTransaction_Load(object sender, EventArgs e)
        {         
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
                    MessageBox.Show("No document selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void ChangeDocType(DocHeaderControls controls)
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
        private void FillAccountName(DocHeaderControls controls)
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


        private void FillAccountCode(DocHeaderControls controls)
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
            PopulateDocLineGrid(dgv_DocLine, _docLines, docTypeID);

            //SetDocLineColumnAndFormat(dgv_DocLine,docTypeID);



        }

        private void SetDocLineColumnAndFormat(DataGridView grid, int docTypeID, string currency = null)
        {
            var columnConfig = new DocLineColumnConfiguration();
            var builder = new GridColumnBuilder(columnConfig);
            builder.ConfigureGrid(grid, docTypeID);
            builder.ConfigureGridFormat(grid, docTypeID, currency); // Default currency as USD      
        }

        private async void PopulateDocLineGrid(DataGridView grid, List<DocLineDto> docLines, int docTypeId, string currency = null)
        {
            // 1. Set up columns using your builder/config
            var columnConfig = new ColumnConfigurations.DocLineColumnConfiguration();
            var builder = new GridColumnBuilder(columnConfig);
            builder.ConfigureGrid(grid, docTypeId);
            builder.ConfigureGridFormat(grid, docTypeId, currency);

            // 2. Clear any existing rows
            grid.Rows.Clear();

            // 3. Get the list of columns to show for this docType
            var columns = GridColumns.DocLineColumnByDocType.TryGetValue(docTypeId, out var cols) ? cols : new List<string>();

            // 4. For each DocLineDto, add a row and set cell values
            foreach (var dto in docLines)
            {

                int rowIdx = grid.Rows.Add();
                var row = grid.Rows[rowIdx];

                foreach (var col in columns)
                {
                    // Use reflection to get the property value by name
                    var prop = typeof(DocLineDto).GetProperty(col, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (prop != null)
                    {
                        var value = prop.GetValue(dto, null);
                        row.Cells[col].Value = value ?? "";
                    }
                    else
                    {
                        // If the property doesn't exist, leave cell empty or handle as needed
                        row.Cells[col].Value = "";
                    }
                }

                // Attach the WorkingLineDTO to the row              
                CertificateDto certificate = new CertificateDto();
                SourceLotChangeDto sourceLotChange = new SourceLotChangeDto();

                if (dto.CertificateID != 0)
                {
                    certificate = await _certificateService.GetCertificateAsync(dto.CertificateID);
                }

                WorkingLineDto workingLine = new WorkingLineDto(dto, certificate, sourceLotChange);               
                row.Tag = workingLine;

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
            InitializeDocLineNewColumns(1001,"GBP");
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
            InitializeDocLineNewColumns(1002, "GBP");
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
            InitializeDocLineNewColumns(1003,"GBP");
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
            InitializeDocLineNewColumns(1005, "GBP");
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
            InitializeDocLineNewColumns(1006, "USD");
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
            InitializeDocLineNewColumns(1007,"USD");
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
            InitializeDocLineNewColumns(1009, "USD");
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
            InitializeDocLineNewColumns(1010,"USD");
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

            _docHeaderMaxInfo = _diamondDocService.AccessDocHeaderMaxDocIDAndDocCodeFor(docTypeDesc);
            _diamondLotMaxID = _diamondDocService.AccessDiamondLotMaxItemLotIDAndCertificateID();

            MaxDocID = _docHeaderMaxInfo.MaxDocID;
            MaxDocCode = _docHeaderMaxInfo.MaxDocCode;
            MaxItemID = _diamondLotMaxID.MaxItemID;
            MaxCertificateID = _diamondLotMaxID.MaxCertificateID;
            MaxLotID = _diamondLotMaxID.MaxLotID;
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

        private void InitializeDocLineNewColumns(int newDocTypeID, string currency = null)
        {
            SetDocLineColumnAndFormat(dgv_DocLineNew, newDocTypeID, currency);

            //this.Controls.Add(dgv_Lines);
        }


        private void cb_CurrencyNew_TextChanged(object sender, EventArgs e)
        {
            string currency = cb_CurrencyNew.Text.Trim();
            int docTypeID = FormControlHelper.GetIntValue(tb_DocTypeIDNew.Text);

            if (currency == "USD")
            {
                tb_USDRateNew.Text = "1.000";
                tb_GBPRateNew.Text = GBPRate.ToString();
                docCurrency = "USD";
            }
            else if (currency == "GBP")
            {
                tb_GBPRateNew.Text = "1.000";
                tb_USDRateNew.Text = USDRate.ToString();
                docCurrency = "GBP";
            }

            var columnConfig = new DocLineColumnConfiguration();
            var builder = new GridColumnBuilder(columnConfig);
            builder.ConfigureGridFormat(dgv_DocLineNew, docTypeID, currency); 
           
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
     
            WorkingLineForm InWorkingLine = new WorkingLineForm(_diamondLotMaxID, _workingLineDtoNew);
            InWorkingLine.Show();
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

            _certificateNew = new CertificateDto();
            _sourceLotChangeDtoNew = new SourceLotChangeDto();
            _workingLineDtoNew = new WorkingLineDto(_docLineDtoNew, _certificateNew, _sourceLotChangeDtoNew);

            WorkingLineForm workingLineForm = new WorkingLineForm(_diamondLotMaxID, _workingLineDtoNew);            

            workingLineForm.SaveWorkingLine += AddDocLineFromWorkingLine;
            workingLineForm.Show();
        }



        private void AddDocLineFromWorkingLine (object sender, SaveWorkingLineEventArgs e)
        {
            _diamondLotMaxID = e.DiamondLotMaxID;

            //New Method
            var newData = e.WorkingLineDto;
         

            int rowIndex = dgv_DocLineNew.Rows.Add();
            var newRow = dgv_DocLineNew.Rows[rowIndex];

            cb_CurrencyNew.Enabled = false;
            int lineID = GenerateNewLineID();

            newRow.Cells["DocLine"].Value = lineID;
            newRow.Cells["LotID"].Value = newData.DocLineDto.LotID;
            newRow.Cells["ItemID"].Value = newData.DocLineDto.ItemID;
            newRow.Cells["LotName"].Value = newData.DocLineDto.LotName;
            newRow.Cells["Quantity"].Value = newData.DocLineDto.Quantity;
            newRow.Cells["Weight"].Value = newData.DocLineDto.Weight;
            newRow.Cells["Cost"].Value = newData.DocLineDto.Cost;
            newRow.Cells["TotalCost"].Value = newData.DocLineDto.TotalCost;
            newRow.Cells["Shape"].Value = newData.DocLineDto.Shape;
            newRow.Cells["Size"].Value = newData.DocLineDto.Size;
            newRow.Cells["Color"].Value = newData.DocLineDto.Color;
            newRow.Cells["Clarity"].Value = newData.DocLineDto.Clarity;
            newRow.Cells["ParcelOrStone"].Value = newData.DocLineDto.ParcelOrStone;
            newRow.Cells["HoldingType"].Value = newData.DocLineDto.HoldingType;

            if (newData.DocLineDto.ParcelOrStone == "Stone")
            {
                newRow.Cells["CertificateID"].Value = MaxCertificateID + 1;
                MaxCertificateID = MaxCertificateID + 1;
            }

            if (docCurrency == "USD")
            {
                newRow.Cells["DocPrice"].Value = newData.DocLineDto.Cost;
                newRow.Cells["TotalDocPrice"].Value = newData.DocLineDto.TotalCost;
            }
            else if (docCurrency == "GBP")
            {
                newRow.Cells["DocPrice"].Value = newData.DocLineDto.SecCost;
                newRow.Cells["TotalDocPrice"].Value = newData.DocLineDto.SecTotalCost;
            }

            /*
            _maxLotID = Math.Max(_maxLotID, newData.DocLineDto.LotID);

            if (newData.DocLineDto.ParcelOrStone == "Parcel" && newData.DocLineDto.HoldingType == "Permanent")
                _maxItemID = Math.Max(_maxItemID, newData.DocLineDto.ItemID);
            */

            newRow.Cells["DocLineStatus"].Value = "A";
            newData.DocLineDto.DocLineStatus = "A"; 

            newRow.Tag = newData; // Store the WorkingLineDto in the row's Tag

            tb_CertNo.Text = newData.Certificate.CertificateNo;
            
            CalculateSumsAndPositionTextBoxes();
            CalculateGrandTotalPrice(null, null);

            //MessageBox.Show("Row added successfully to dgv_Lines!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void dgv_DocLineNew_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgv_DocLineNew.Rows[e.RowIndex];
                if (row.Tag is WorkingLineDto data)
                {
                    WorkingLineForm form = new WorkingLineForm(_diamondLotMaxID, data);

                    form.SaveWorkingLine += (s, args) =>
                    {
                        // Update the DTOs in memory
                        _diamondLotMaxID = args.DiamondLotMaxID;
                        data = args.WorkingLineDto;
                        
                        // Update the row cells with new values
                        row.Cells["LotID"].Value = data.DocLineDto.LotID;
                        row.Cells["ItemID"].Value = data.DocLineDto.ItemID;
                        row.Cells["LotName"].Value = data.DocLineDto.LotName;
                        row.Cells["Quantity"].Value = data.DocLineDto.Quantity;
                        row.Cells["Weight"].Value = data.DocLineDto.Weight;
                        row.Cells["Cost"].Value = data.DocLineDto.Cost;
                        row.Cells["TotalCost"].Value = data.DocLineDto.TotalCost;
                        row.Cells["Shape"].Value = data.DocLineDto.Shape;
                        row.Cells["Size"].Value = data.DocLineDto.Size;
                        row.Cells["Color"].Value = data.DocLineDto.Color;
                        row.Cells["Clarity"].Value = data.DocLineDto.Clarity;
                        row.Cells["ParcelOrStone"].Value = data.DocLineDto.ParcelOrStone;
                        row.Cells["HoldingType"].Value = data.DocLineDto.HoldingType;
                        row.Cells["DocLineStatus"].Value = "A"; 

                        // Certificate fields if displayed
                        row.Cells["CertificateID"].Value = data.Certificate.CertificateID;

                        // Currency logic (if needed)
                        if (docCurrency == "USD")
                        {
                            row.Cells["DocPrice"].Value = data.DocLineDto.Cost;
                            row.Cells["TotalDocPrice"].Value = data.DocLineDto.TotalCost;
                        }
                        else if (docCurrency == "GBP")
                        {
                            row.Cells["DocPrice"].Value = data.DocLineDto.SecCost;
                            row.Cells["TotalDocPrice"].Value = data.DocLineDto.SecTotalCost;
                        }
                                               
                        tb_CertNo.Text = data.Certificate.CertificateNo;
                       
                        CalculateSumsAndPositionTextBoxes();
                        CalculateGrandTotalPrice(null, null);

                    };

                    form.Show();
                }
            }
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

            if (e.Value != null && columnName != "DocLine" && columnName != "LotID" && columnName != "ItemID" && columnName != "Quantity" && columnName != "CertificateID")
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
            return dgv_DocLineNew.Rows.Count; 
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

            tb_SumQuantityNew.Text = sumQuantity.ToString("N0");
            tb_SumWeightNew.Text = sumWeight.ToString("N2"); 
            if(sumWeight != 0)
            tb_AvgDocPriceNew.Text = Math.Round(sumTotalDocPrice / sumWeight, 2).ToString("N2");
            else
                tb_AvgDocPriceNew.Text = "0.00";
            tb_SumTotalDocPriceNew.Text = sumTotalDocPrice.ToString("N2");
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
            tb_SumQuantityNew.Left = dgv_DocLineNew.GetColumnDisplayRectangle(quantityColumnIndex, true).Left;
            tb_SumQuantityNew.Top = dgv_DocLineNew.Bottom + 2;
            tb_SumQuantityNew.Width = dgv_DocLineNew.GetColumnDisplayRectangle(quantityColumnIndex, true).Width;

            tb_SumWeightNew.Left = dgv_DocLineNew.GetColumnDisplayRectangle(weightColumnIndex, true).Left;
            tb_SumWeightNew.Top = dgv_DocLineNew.Bottom + 2;  // 10 pixels below the DataGridView
            tb_SumWeightNew.Width = dgv_DocLineNew.GetColumnDisplayRectangle(weightColumnIndex, true).Width;

            tb_AvgDocPriceNew.Left = dgv_DocLineNew.GetColumnDisplayRectangle(docPriceColumnIndex, true).Left;
            tb_AvgDocPriceNew.Top = dgv_DocLineNew.Bottom + 2;  // 10 pixels below the DataGridView
            tb_AvgDocPriceNew.Width = dgv_DocLineNew.GetColumnDisplayRectangle(docPriceColumnIndex, true).Width;

            tb_SumTotalDocPriceNew.Left = dgv_DocLineNew.GetColumnDisplayRectangle(totalDocPriceColumnIndex, true).Left;
            tb_SumTotalDocPriceNew.Top = dgv_DocLineNew.Bottom + 2;  // 10 pixels below the DataGridView
            tb_SumTotalDocPriceNew.Width = dgv_DocLineNew.GetColumnDisplayRectangle(totalDocPriceColumnIndex, true).Width;
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
                ResetNewTransaction();
            }
        }

        private void ResetNewTransaction()
        {
            FormControlHelper.ResetControls(this.tab_TrsNew);
            cb_CurrencyNew.Enabled = true;
            tab_Transaction.SelectedIndex = 0;
            IsCreatingNewDoc = false;
            _docLineDtoNew = null;
            _workingLineDtoNew = null;
            _sourceLotChangeDtoNew = null;
            _certificateNew = null;
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

        private void bt_DeleteWorkingLineNew_Click(object sender, EventArgs e)
        {
            DeleteSelectedRow(dgv_DocLineNew);
            CalculateSumsAndPositionTextBoxes();
            CalculateGrandTotalPrice(null, null);
        }

        private void DeleteSelectedRow(DataGridView dataGridView)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var row = dataGridView.SelectedRows[0];
                var tag = row.Tag as WorkingLineDto;
                DialogResult result = MessageBox.Show(
                    "Are you sure to delete this line?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    dataGridView.Rows.Remove(row);
                    ReassignLineIDs(dataGridView);
                }

            }
        }

        private void ReassignLineIDs(DataGridView dataGridView)
        {
            int lineID = 1;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                row.Cells["DocLine"].Value = lineID;

                if (row.Tag is WorkingLineDto workingLineDto && workingLineDto.DocLineDto != null)
                {
                    workingLineDto.DocLineDto.DocLine = lineID;
                }

                lineID++;
            }
        }

        //Section----Update Document Details ----------------------------------------------------------------------------------------------------------
        private void bt_SaveNewTransaction_Click0(object sender, EventArgs e)
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
                {"@DateTime_Modified", DateTime.Now }
            };     

            if (IsCreatingNewDoc)
            {
                errorMessageTitle = "Create New DiamondDocHeader";
                query = "INSERT INTO THXADiamondDocHeader " +
                    "(DocTypeID, DocTypeDesc, DocID, DocCode, DocAccountRefNo, DocSubType, DocStatus, DocDate, " +
                    "RefDocDate, DueDate, DateTime_Modified, AccountCode, AccountBranchCode, AccountName, AccountLongName, Currency, " +
                    "VatCode, VatRate, PaymentTerm, Remark1, Remark2, InternalRemark, UserID, Quantity, Weight, " +
                    "MainLinesTotalPrice, SecLinesTotalPrice, DocLinesTotalPrice, MainDiscountPrice, SecDiscountPrice, DocDiscountPrice, DiscountPercent, DiscountRemark, " +
                    "MainSubTotalPrice, SecSubTotalPrice, DocSubTotalPrice, MainAdditionalPrice, SecAdditionalPrice, DocAdditionalPrice, AdditionalRemark, " +
                    "MainTaxPrice, SecTaxPrice, DocTaxPrice, MainGrandTotalPrice, SecGrandTotalPrice, DocGrandTotalPrice, RateDocToMain, RateDocToSec) " +
                    "VALUES " +
                    "(@DocTypeID, @DocTypeDesc, @DocID, @DocCode, @DocAccountRefNo, @DocSubType, @DocStatus, @DocDate, " +
                    "@RefDocDate, @DueDate, @DateTime_Modified, @AccountCode, @AccountBranchCode, @AccountName, @AccountName, @Currency, " +
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

        private async void bt_SaveNewTransaction_Click(object sender, EventArgs e)
        {
            if (!IsAuthorisedUser())
            {
                MessageBox.Show("This account is not allowed to update DiamondItemMaintenance.", "Message");
                return;
            }

            if (!IsInputValid("Parcel"))
            {
                MessageBox.Show("Parcel Detail is not completed.", "Message");
                return;
            }

            try
            {
                try
                {
                    PrepareDocHeaderForInsert();
                    InsertDocHeader();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while saving DocHeader: {ex.Message}", "DocHeader Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                PrepareCertificate(dgv_DocLineNew);
                AddLineIDsToDocLineDto(dgv_DocLineNew);

                foreach (DataGridViewRow row in dgv_DocLineNew.Rows)
                {
                    if (row.Tag is WorkingLineDto workingLine && workingLine.DocLineDto != null)
                    {
                        int certificateID = 0;
                        if (row.Cells["ParcelOrStone"].Value?.ToString() == "Stone")
                        {
                            try
                            {
                                DebugCertificate(workingLine.Certificate);
                                certificateID = InsertCertificate(workingLine.Certificate);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error while saving Certificate: {ex.Message}", "Certificate Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                        }

                        try
                        {
                            PrepareDocLineForInsert(workingLine.DocLineDto, certificateID);
                            DebugDocLine(workingLine.DocLineDto);
                            InsertDocLine(workingLine);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error while saving DocLine: {ex.Message}", "DocLine Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                }

                MessageBox.Show("Create Document successfully", "Message");

                          
                await LoadAndBindDocHeaders();
                ResetNewTransaction();
                                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while saving: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrepareDocHeaderForInsert()
        {
            _docHeaderNew = new DocHeaderDto
            {
                DocTypeID = FormControlHelper.GetIntValue(tb_DocTypeIDNew.Text),
                DocTypeDesc = cb_DocTypeDescNew.Text.Trim(),
                DocID = FormControlHelper.GetIntValue(tb_DocIDNew.Text),
                DocCode = tb_DocCodeNew.Text.Trim(),
                DocAccountRefNo = tb_DocAccountRefNoNew.Text.Trim(),
                DocSubType = cb_DocSubTypeIDNew.Text.Trim(),
                DocStatus = cb_DocStatusNew.Text.Trim(),
                DocDate = DateTime.Parse(dateTime_DocDateNew.Text.Trim()),
                RefDocDate = DateTime.Parse(dateTime_RefDocDateNew.Text.Trim()),
                DueDate = DateTime.Parse(dateTime_DueDateNew.Text.Trim()),
                DateTime_Modified = DateTime.Now,
                AccountCode = cb_AccountCodeNew.Text.Trim(),
                AccountBranchCode = cb_AccountBranchCodeNew.Text.Trim(),
                AccountName = cb_AccountNameNew.Text.Trim(),
                AccountLongName = cb_AccountNameNew.Text.Trim(),
                Currency = cb_CurrencyNew.Text.Trim(),
                VatCode = "", // replace with actual control if needed
                VatRate = 0,  // or use FormControlHelper.GetDecimalValue(...) if applicable
                PaymentTerm = FormControlHelper.GetIntValue(cb_PaymentTermNew.Text),
                Remark1 = tb_Remark1New.Text.Trim(),
                Remark2 = "", // replace with actual control if needed
                InternalRemark = tb_InternalRemarkNew.Text.Trim(),
                UserID = User,

                Quantity = FormControlHelper.GetIntValue(tb_SumQuantityNew.Text),
                Weight = FormControlHelper.GetDecimalValue(tb_SumWeightNew.Text),

                MainLinesTotalPrice = 0,
                SecLinesTotalPrice = 0,
                DocLinesTotalPrice = FormControlHelper.GetDecimalValue(tb_TotalDocPriceNew.Text),

                MainDiscountPrice = 0,
                SecDiscountPrice = 0,
                DocDiscountPrice = FormControlHelper.GetDecimalValue(tb_DiscountNew.Text),
                DiscountPercent = 0,
                DiscountRemark = "",

                MainSubTotalPrice = 0,
                SecSubTotalPrice = 0,
                DocSubTotalPrice = FormControlHelper.GetDecimalValue(tb_SubTotalNew.Text),

                MainAdditionalPrice = 0,
                SecAdditionalPrice = 0,
                DocAdditionalPrice = FormControlHelper.GetDecimalValue(tb_AdditionalNew.Text),
                AdditionalRemark = "",

                MainTaxPrice = 0,
                SecTaxPrice = 0,
                DocTaxPrice = FormControlHelper.GetDecimalValue(tb_TaxNew.Text),

                MainGrandTotalPrice = 0,
                SecGrandTotalPrice = 0,
                DocGrandTotalPrice = FormControlHelper.GetDecimalValue(tb_GrandTotalPriceNew.Text),
                RateDocToMain = decimal.TryParse(tb_USDRateNew.Text.Trim(), out var usd) ? usd : 1,
                RateDocToSec = decimal.TryParse(tb_GBPRateNew.Text.Trim(), out var gbp) ? gbp : 1
                         
            };
            PrepareTotalPricesForDocHeaderDto(_docHeaderNew, cb_CurrencyNew.Text.Trim(), USDRate, GBPRate);

        }


        public void PrepareTotalPricesForDocHeaderDto(DocHeaderDto dto, string currency, decimal usdRate, decimal gbpRate)
        {
            // Fallback/default assignments
            dto.MainLinesTotalPrice = dto.DocLinesTotalPrice;
            dto.SecLinesTotalPrice = dto.DocLinesTotalPrice;

            dto.MainDiscountPrice = dto.DocDiscountPrice;
            dto.SecDiscountPrice = dto.DocDiscountPrice;

            dto.MainSubTotalPrice = dto.DocSubTotalPrice;
            dto.SecSubTotalPrice = dto.DocSubTotalPrice;

            dto.MainAdditionalPrice = dto.DocAdditionalPrice;
            dto.SecAdditionalPrice = dto.DocAdditionalPrice;

            dto.MainTaxPrice = dto.DocTaxPrice;
            dto.SecTaxPrice = dto.DocTaxPrice;

            dto.MainGrandTotalPrice = dto.DocGrandTotalPrice;
            dto.SecGrandTotalPrice = dto.DocGrandTotalPrice;

            if (currency == "USD")
            {
                dto.MainLinesTotalPrice = dto.DocLinesTotalPrice;
                dto.SecLinesTotalPrice = Math.Round(dto.DocLinesTotalPrice * gbpRate, 2);

                dto.MainDiscountPrice = dto.DocDiscountPrice;
                dto.SecDiscountPrice = Math.Round(dto.DocDiscountPrice * gbpRate, 2);

                dto.MainSubTotalPrice = dto.DocSubTotalPrice;
                dto.SecSubTotalPrice = Math.Round(dto.DocSubTotalPrice * gbpRate, 2);

                dto.MainAdditionalPrice = dto.DocAdditionalPrice;
                dto.SecAdditionalPrice = Math.Round(dto.DocAdditionalPrice * gbpRate, 2);

                dto.MainTaxPrice = dto.DocTaxPrice;
                dto.SecTaxPrice = Math.Round(dto.DocTaxPrice * gbpRate, 2);

                dto.MainGrandTotalPrice = dto.DocGrandTotalPrice;
                dto.SecGrandTotalPrice = Math.Round(dto.DocGrandTotalPrice * gbpRate, 2);
            }
            else if (currency == "GBP")
            {
                dto.SecLinesTotalPrice = dto.DocLinesTotalPrice;
                dto.MainLinesTotalPrice = Math.Round(dto.DocLinesTotalPrice * usdRate, 2);

                dto.SecDiscountPrice = dto.DocDiscountPrice;
                dto.MainDiscountPrice = Math.Round(dto.DocDiscountPrice * usdRate, 2);

                dto.SecSubTotalPrice = dto.DocSubTotalPrice;
                dto.MainSubTotalPrice = Math.Round(dto.DocSubTotalPrice * usdRate, 2);

                dto.SecAdditionalPrice = dto.DocAdditionalPrice;
                dto.MainAdditionalPrice = Math.Round(dto.DocAdditionalPrice * usdRate, 2);

                dto.SecTaxPrice = dto.DocTaxPrice;
                dto.MainTaxPrice = Math.Round(dto.DocTaxPrice * usdRate, 2);

                dto.SecGrandTotalPrice = dto.DocGrandTotalPrice;
                dto.MainGrandTotalPrice = Math.Round(dto.DocGrandTotalPrice * usdRate, 2);
            }

        }

        private int InsertDocHeader()
        {
            var docHeaderID = _docHeaderService.InsertDiamondDocHeader(_docHeaderNew);
            _docHeaderNew.DocHeaderID = docHeaderID;
            return docHeaderID;
        }


        private void PrepareCertificate(DataGridView dgv) 
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Tag is WorkingLineDto workingLine && workingLine.DocLineDto != null)
                {                   
                    workingLine.Certificate.Created_By = User;
                    workingLine.Certificate.DateTime_Created = DateTime.Now;
                }
            }
        }


        private int InsertCertificate(CertificateDto certificateDto) 
        {

            var certificateID = _certificateService.InsertCertificate(certificateDto);
            return certificateID;

        }

        private void DebugCertificate(CertificateDto dto)
        {
            var sb = new StringBuilder();

            void Check(string fieldName, string value, int maxLength)
            {
                if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
                {
                    sb.AppendLine($"{fieldName} exceeds max length {maxLength}: {value.Length} - Value: '{value}'");
                }
            }

            Check("CertificateLabName", dto.CertificateLabName, 50);
            Check("CertificateType", dto.CertificateType, 20);
            Check("CertificateTypeDesc", dto.CertificateTypeDesc, 50);
            Check("CertificateNo", dto.CertificateNo, 20);
            Check("Shape", dto.Shape, 20);
            Check("Size", dto.Size, 20);
            Check("Color", dto.Color, 20);
            Check("Clarity", dto.Clarity, 20);
            Check("Measurements", dto.Measurements, 50);
            Check("GirdleMinMax", dto.GirdleMinMax, 50);
            Check("GirdleCondition", dto.GirdleCondition, 20);
            Check("Girdle", dto.Girdle, 20);
            Check("Culet", dto.Culet, 20);
            Check("Polish", dto.Polish, 20);
            Check("Symmetry", dto.Symmetry, 20);
            Check("Fluorescence", dto.Fluorescence, 20);
            Check("Cut", dto.Cut, 20);
            Check("Inscription", dto.Inscription, 200);
            Check("LabComment", dto.LabComment, 1000);
            Check("DownloadStatus", dto.DownloadStatus, 10);
            Check("Status", dto.Status, 20);
            Check("Remark", dto.Remark, 1000);
            Check("Created_By", dto.Created_By, 30);

            if (sb.Length > 0)
            {
                throw new Exception("Field length violation in CertificateDto:\n" + sb.ToString());
            }
        }



        private void AddLineIDsToDocLineDto(DataGridView dgv)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Tag is WorkingLineDto workingLine && workingLine.DocLineDto != null)
                {
                    if (int.TryParse(row.Cells["DocLine"].Value?.ToString(), out int lineID))
                    {
                        workingLine.DocLineDto.DocLine = lineID;
                    }
                    else
                    {
                        workingLine.DocLineDto.DocLine = 0; 
                    }
                }
            }
        }

        private DiamondLotDto PrepareDiamondLot( DocHeaderDto docHeaderDto,  DocLineDto docLineDto, CertificateDto certificateDto)
        {
            var diamondLotDto = new DiamondLotDto
            {
                // Required identifiers
                LotID = docLineDto.LotID,
                LotName = docLineDto.LotName,
                ItemID = docLineDto.ItemID,
                ItemName = docLineDto.LotName,
                ItemDescription = string.Empty,
                ItemDescription1 = string.Empty,
                ParcelOrStone = docLineDto.ParcelOrStone,
                HoldingType = docLineDto.HoldingType,
                //todo StockStatus Status, function to give status
                StockStatus = "IN", // Example default; update based on logic
                Status = "Active",


                // Basic properties
                Shape = docLineDto.Shape,
                Size = docLineDto.Size,
                Color = docLineDto.Color,
                Clarity = docLineDto.Clarity,
                Cut = certificateDto.Cut,
                Polish = certificateDto.Polish,
                Symmetry = certificateDto.Symmetry,
                Fluorescence = certificateDto.Fluorescence,
                Inscription = certificateDto.Inscription,
                CertificateID = certificateDto.CertificateID,
                CertificateLabName = certificateDto.CertificateLabName,
                CertificateType = certificateDto.CertificateType,
                CertificateNo = certificateDto.CertificateNo,
                CertificateDate = certificateDto.CertificateDate,


                Weight = docLineDto.Weight,
                //todo WeightLoss, ScrapWeight ( not to update the diamondLot weightlosdd here)
                WeightLoss = docLineDto.WeightLoss ?? 0,
                //ScrapWeight = docLineDto.ScrapWeight ?? 0,

                //todo: function to write data to-------------------------------
                Cost = docLineDto.Cost,
                TotalCost = docLineDto.TotalCost,

                // List-related
                List = docLineDto.List,
                TotalList = docLineDto.TotalList,
                ListCostDiscount = docLineDto.ListCostDiscount ?? 0,
                ListSaleDiscount = docLineDto.ListSaleDiscount ?? 0,

                // Sale related
                Sale = docLineDto.Sale,
                TotalSale = docLineDto.TotalSale,

                // Pricing (main/sec/doc)
                //MainPrice = docLineDto.MainPrice,
                //TotalMainPrice = docLineDto.TotalMainPrice,
                //SecPrice = docLineDto.SecPrice,
                //TotalSecPrice = docLineDto.TotalSecPrice,
                //DocPrice = docLineDto.DocPrice,
                //TotalDocPrice = docLineDto.TotalDocPrice,
                //------------------------------------------

                //todo: function: based on document type to assign Location or Vendor
                LocationAccountCode = docHeaderDto.AccountCode,
                LocationAccountName = docHeaderDto.AccountName,
                VendorAccountCode = docHeaderDto.AccountCode,
                VendorAccountName = docHeaderDto.AccountName,


                // Audit
                Remark = docLineDto.Remark,
                Created_By = User,
                DateTime_Created = DateTime.Now,
                Modified_By = string.Empty,
                LastStockStatusUpdate = DateTime.Now,
                ReferenceDocCode = docHeaderDto.DocCode,


                // System-assigned or defaults
                LastTrsDate = DateTime.Now,
                LastTrsID = docLineDto.TrsUnionID,
                LastTrsTypeID = docLineDto.DocTypeID,
                LastTrsTypeDesc = docLineDto.DocTypeDesc,

            };

            return diamondLotDto;
        }


        private void PrepareDocLineForInsert(DocLineDto docLineDto, int certificateID)
        {
            docLineDto.DocHeaderID = _docHeaderNew.DocHeaderID;
            docLineDto.DocID = _docHeaderNew.DocID;
            docLineDto.DocTypeDesc = _docHeaderNew.DocTypeDesc;
            docLineDto.CertificateID = certificateID;
            docLineDto.UserID = User;
            docLineDto.DateTime_Created = DateTime.Now;
            docLineDto.DateTime_Modified = DateTime.Now;
            docLineDto.ParcelOrStone = FormControlHelper.MapParcelOrStone(docLineDto.ParcelOrStone);
            docLineDto.HoldingType = FormControlHelper.MapHoldingType(docLineDto.HoldingType);
            FormControlHelper.ReplaceNullsInDto(docLineDto);
        }


        private void DebugDocLine(DocLineDto dto)
        {
            var sb = new StringBuilder();
            void Check(string fieldName, string value, int maxLength)
            {
                if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
                    sb.AppendLine($"{fieldName} is too long: {value.Length} chars (max {maxLength}). Value: {value}");
            }

            Check("DocTypeDesc", dto.DocTypeDesc, 50);
            Check("LotName", dto.LotName, 50);
            Check("Shape", dto.Shape, 20);
            Check("Size", dto.Size, 20);
            Check("Color", dto.Color, 20);
            Check("Clarity", dto.Clarity, 20);
            Check("Remark", dto.Remark, 1000);
            Check("SourceDocTypeDesc", dto.SourceDocTypeDesc, 50);
            Check("SourceDocCode", dto.SourceDocCode, 50);
            Check("UserID", dto.UserID, 30);
            Check("DocLineStatus", dto.DocLineStatus, 1);
            Check("ParcelOrStone", dto.ParcelOrStone, 1);
            Check("HoldingType", dto.HoldingType, 1);

            if (sb.Length > 0)
                throw new Exception("Field length violation in DocLineDto:\n" + sb.ToString());
        }

        private int InsertDocLine(WorkingLineDto workingLine)
        {
            int docLineID = _docLineService.InsertDocLine(workingLine.DocLineDto);
            return docLineID;
        }







    }
}
