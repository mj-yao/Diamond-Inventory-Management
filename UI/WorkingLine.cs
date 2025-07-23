using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DiamondTransaction.GlobalDataManager;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using DiamondTransaction.Models;
using DiamondTransaction.DataAccess;
using DiamondTransaction.UseCases;
using DiamondTransaction.Domain.Entities;
using DiamondTransaction.UI.Utilities;
namespace DiamondTransaction
{
    public partial class WorkingLine : Form
    {
        private int newDocTypeID;
        //public string connectionString = @"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql";
        public event EventHandler<List<DataGridViewRow>> RowCalculated;
        public event EventHandler<CertificateData> AddCertificate;
        public event EventHandler<RowCalculatedEventArgs> RowCalculatedOrCertificateAdded;

        private List<DataGridViewRow> storedRows = new List<DataGridViewRow>();
        private fmDiamondTransaction transactionForm;
        public int MaxItemID, MaxLotID, MaxCertificateID;
        public decimal USDRate, GBPRate; string docCurrency;
        public CertificateData certificateData = new CertificateData();

        private DiamondGrades _parcelGrades;
        private List<string> _lotNames;


        //Application dependency
        private RapaportPriceService _rapaportPriceService;
        private DiamondDocService _diamondDocService;


        public WorkingLine(fmDiamondTransaction parentForm)
        {
            InitializeComponent();
            transactionForm = parentForm;
            this.newDocTypeID = parentForm.docTypeIDNew;
            this.USDRate = parentForm.USDRate;
            this.GBPRate = parentForm.GBPRate;
            this.docCurrency = parentForm.docCurrency;

            InitializeServices();
            InitializeDataAndControls();

            InitializeWorkingLineTable();
            ApplyColumnVisibility(dgv_WorkingLine, newDocTypeID);

        }

        private void InitializeServices() 
        {
            var _rapaportRepository = new RapaportDataAccess(SqlQueryExecutor.getConnectionString());
            _rapaportPriceService = new RapaportPriceService(_rapaportRepository);

            var diamondDocRepository = new DiamondDocDataAccess(SqlQueryExecutor.getConnectionString());
            _diamondDocService = new DiamondDocService(diamondDocRepository);
        }

        private void InitializeDataAndControls()
        {
            LoadData();
            InitialiseControls();
        }

        private void LoadData()
        {
            LoadParcelGrades();
            LoadLotNames();
        }

        private void LoadParcelGrades()
        {
            _parcelGrades = _diamondDocService.GetAllParcelGradingScales();
        }
        private void LoadLotNames()
        {
            _lotNames = _diamondDocService.GetExistingLotNames();
        }
        private void InitialiseControls()
        {
            InitialiseParcelGradingComboBox(cb_Shape, cb_Size, cb_Color, cb_Clarity);
            InitialiseLotNameComboBox(cb_LotName);
        }

        private void InitialiseParcelGradingComboBox(ComboBox cb_shape, ComboBox cb_size, ComboBox cb_color, ComboBox cb_clarity)
        {

            List<string> shapes = _parcelGrades.Shapes;
            List<string> sizes = _parcelGrades.Sizes;
            List<string> colors = _parcelGrades.Colors;
            List<string> clarities = _parcelGrades.Clarities;

            FormControlHelper.SetUpComboBoxItem(cb_shape, shapes);
            FormControlHelper.SetUpComboBoxItem(cb_size, sizes);
            FormControlHelper.SetUpComboBoxItem(cb_color, colors);
            FormControlHelper.SetUpComboBoxItem(cb_clarity, clarities);

        }

        private void InitialiseLotNameComboBox(ComboBox cb_lotName)
        {
            FormControlHelper.SetUpComboBoxItem(cb_lotName, _lotNames);
            FormControlHelper.SetupAutoComplete(cb_lotName, _lotNames);
        }

        private void InitializeWorkingLineTable()
        {
            List<string> columnNames = new List<string>();
            if (ColumnManager.WorkingLineColumnByDocType.ContainsKey(newDocTypeID))
            {
                columnNames = ColumnManager.WorkingLineColumnByDocType[newDocTypeID];
            }
        
            if (ColumnManager.WorkingLineColumnByDocType.ContainsKey(newDocTypeID))
            {
                foreach (var columnName in columnNames)
                {
                    dgv_WorkingLine.Columns.Add(columnName, columnName);
                }
            }
            else
            {
                MessageBox.Show("Unknown Document Type");
                this.Close();
            }

            dgv_WorkingLine.Rows.Add(3); // 3 rows (Row 0, Row 1, and Row 2 for calculations result)

            this.Controls.Add(dgv_WorkingLine);

           // dgv_WorkingLine.Columns["Shape"].Visible = false;
           // dgv_WorkingLine.Columns["Size"].Visible = false;
           // dgv_WorkingLine.Columns["Color"].Visible = false;
           // dgv_WorkingLine.Columns["Clarity"].Visible = false;
        }

        private void WorkingLine_Load(object sender, EventArgs e)
        {
        
    

          


        }

        private void tHXADiamondLotBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();


        }

        private void WorkingLine_Load_1(object sender, EventArgs e)
        {


        }


        private void bt_LotFinder_Click(object sender, EventArgs e)
        {
            LotFinder lotFinder = new LotFinder();

            lotFinder.OnLotSelected += (lotID) =>
            {
                //FetchLotData(lotID); // Fetch data from DB
                if (!ck_NewLot.Checked)
                {
                    tb_LotID.Text = lotID;
                    InitializeDgvWorkingLineWithDefault0();
                    AccessDiamondLotParcel(lotID, SearchType.LotID);
                }
                else {
                    MessageBox.Show("Lot can not be loaded when 'New Lot' checkbox is marked.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                
                
            };
            
            lotFinder.Show();
        }
        private void FetchLotData(string lotID)
        {
            string query = @"
            SELECT LotName, ItemName,Weight, Shape, Size, Color, Clarity, ItemID, LotID, ParcelOrStone, Cost,TotalCost, ListCostDiscount, 
            List, TotalList, ListSaleDiscount, Sale, TotalSale, GBPSale, GBPTotalSale, 
            StockStatus, LocationAccountName, LocationAccountCode, HoldingType, Status FROM THXADiamondLot              
            WHERE LotID = @LotID";


            string errorMessageTitle = "Fetch Lot Data";

           
            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(SqlQueryExecutor.getConnectionString(), query, errorMessageTitle, new Dictionary<string, object>
            {
                { "@LotID", lotID }
            });

            if (SqlQueryExecutor.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
            {
                DataRow row = sqlResult.Rows[0]; // Get first row1 of result

                cb_LotName.Text = row["LotName"].ToString();
                tb_LotID.Text = row["LotID"].ToString();
                cb_HoldingType.Text = row["HoldingType"].ToString();
                cb_ParcelOrStone.Text = row["ParcelOrStone"].ToString();
                cb_Shape.Text = row["Shape"].ToString();
                cb_Size.Text = row["Size"].ToString();
                cb_Color.Text = row["Color"].ToString();
                cb_Clarity.Text = row["Clarity"].ToString();
                                                         
            }
            else
            {
                MessageBox.Show("No data found for LotID: " + lotID, "Data Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        private void bt_Save_Click(object sender, EventArgs e)
        {
            if (int.TryParse(tb_LotID.Text.Trim(), out int lotID) && lotID == 0)
            {
                MessageBox.Show("Missing LotID", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (String.IsNullOrEmpty(cb_LotName.Text.Trim()))
            {
                MessageBox.Show("Missing LotName", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ck_NewLot.Checked)
            {
                UpdateMaxLotIDs(tb_LotID.Text.Trim());
                //transactionForm.dgv_LinesMaxLotID = lotID;
            }

            if (dgv_WorkingLine.Rows.Count >= 3)
            {
                dgv_WorkingLine.Rows[2].Cells["LotName"].Value = cb_LotName.Text.ToString(); 
                dgv_WorkingLine.Rows[2].Cells["ItemID"].Value = tb_ItemID.Text.ToString();
                dgv_WorkingLine.Rows[2].Cells["Shape"].Value = cb_Shape.SelectedItem.ToString();
                dgv_WorkingLine.Rows[2].Cells["Size"].Value = cb_Size.SelectedItem.ToString();
                dgv_WorkingLine.Rows[2].Cells["Color"].Value = cb_Color.SelectedItem.ToString();
                dgv_WorkingLine.Rows[2].Cells["Clarity"].Value = cb_Clarity.SelectedItem.ToString();
                dgv_WorkingLine.Rows[2].Cells["ParcelOrStone"].Value = cb_ParcelOrStone.SelectedItem.ToString();
                dgv_WorkingLine.Rows[2].Cells["HoldingType"].Value = cb_HoldingType.SelectedItem.ToString();
            }
            storedRows = new List<DataGridViewRow>();

            storedRows.Add(dgv_WorkingLine.Rows[0]);
            storedRows.Add(dgv_WorkingLine.Rows[1]);
            storedRows.Add(dgv_WorkingLine.Rows[2]);
            RowCalculated?.Invoke(this, storedRows);
            AddCertificate?.Invoke(this, certificateData);
            RowCalculatedOrCertificateAdded?.Invoke(this, new RowCalculatedEventArgs(storedRows, certificateData));


            this.Close();
            

        }

     
        //Todo
        public void UpdateMaxLotIDs(string lotID)
        {
            // Check if LotID is a 6-digit number
            if (lotID.Length == 6)
            {
                // Compare and set MaxLotID if it's smaller
                if (MaxLotID < int.Parse(lotID))
                {
                    transactionForm.dgv_LinesMaxLotID = int.Parse(lotID);
                }
            }
            // Check if LotID is a 4-digit number
            else if (lotID.Length == 4)
            {
                // Compare and set MaxItemID if it's smaller
                if (MaxItemID < int.Parse(lotID))
                {
                    MaxItemID = int.Parse(lotID);
                }
            }
        }



        private void bt_CancelWorkingLine_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
            "Are you sure you want to cancel the document?",
            "Confirm Cancel",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                //GlobalClass.ResetControls(this);
                this.Close();
            }
        }

        private void cb_LotName_DropDown(object sender, EventArgs e)
        {
            FormControlHelper.comboBox_DynamicDropDownWidth(sender, e);
        }


        private void cb_LotName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter ||  e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true; // Prevent beep sound on Enter key
                ConfirmLotNameInput();
            }
        }
        private void tb_LotID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true; // Prevent beep sound on Enter key
                ConfirmLotIDInput();
            }
        }
        private void cb_LotName_Leave(object sender, EventArgs e)
        {
            ConfirmLotNameInput();
        }

        private void tb_LotID_Leave(object sender, EventArgs e)
        {
            if (int.TryParse(tb_LotID.Text.Trim(), out int lotID) && lotID != 0)
                ConfirmLotIDInput();
        }


        private void ConfirmLotNameInput()
        {
            string lotName = cb_LotName.Text.Trim();
           
            if (String.IsNullOrEmpty(lotName) || ck_NewLot.Checked)
            {
                //Do nothing
            }
            // Check if user lotName exists in the list
            else if (_diamondDocService.GetExistingLotNames().Contains(lotName, StringComparer.OrdinalIgnoreCase))
            {
                InitializeDgvWorkingLineWithDefault0();
                AccessDiamondLotParcel(lotName, SearchType.LotName);
            }
            else
            {
                MessageBox.Show("Invalid LotName. Please select a valid one.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void ConfirmLotIDInput()
        {           
            string lotID = tb_LotID.Text.Trim();
            if (String.IsNullOrEmpty(lotID) || ck_NewLot.Checked)
            {
                //Do nothing
            }
            else 
            { 
                InitializeDgvWorkingLineWithDefault0();
                AccessDiamondLotParcel(lotID, SearchType.LotID);
            }


        }

        enum SearchType { LotID, LotName }
        private void AccessDiamondLotParcel(string identifier, SearchType searchType)
        {
            try
            {
                
                string query = $"SELECT  LotName, ItemDescription, ParcelOrStone, Weight, Shape, Size, Color, Clarity, ItemID, LotID, Cost, GBPSale, Sale, GBPOutPrice, StockStatus, " +
                    "LocationAccountName, LocationAccountCode, VendorAccountCode, VendorAccountName, HoldingType, Status, convert(varchar(10), DateTime_Created,103) as DateTime_Created, " +
                    "WeightLoss, ScrapWeight, PriceStockHistory.ParcelAvgCost as ParcelAvgCost FROM THXADiamondLot " +
                    "OUTER APPLY (SELECT TOP 1 ParcelAvgCost FROM THXADiamondPriceStockHistory WHERE THXADiamondPriceStockHistory.LotID = THXADiamondLot.LotID " +
                    "ORDER BY TrsDate DESC) AS PriceStockHistory " +
                    "where " + (searchType == SearchType.LotID? "LotID" : "LotName" )+ " = @Identifier  ORDER BY ItemName ";

                var parameters = new Dictionary<string, object>
                {
                    { "@Identifier", identifier },
                };

                /*"SELECT LotID,	LotName,	ItemID,	ItemName,	ItemDescription,	ItemDescription1, 	" +
                    "ParcelOrStone,	HoldingType,	Status,	StockStatus,	Weight,	Shape,	Size,	Color,	Clarity,	" +
                    "Cut,	Polish,	Symmetry,	Fluorescence,	Inscription,	CertificateID,	CertificateLabName,	" +
                    "CertificateType,	CertificateNo,	CertificateDate,	Cost,	TotalCost,	ListCostDiscount,	" +
                    "List,	TotalList,	ListSaleDiscount,	Sale,	TotalSale,	GBPSale,	GBPTotalSale,	" +
                    "OutList,	ListOutDiscount,	OutPrice,	OutTotalPrice,	GBPOutPrice,	GBPOutTotalPrice,	" +
                    "LocationAccountCode,	LocationAccountName,	VendorAccountCode,	VendorAccountName,	WeightLoss,	ScrapWeight,	" +
                    "Remark,	Created_By,	DateTime_Created,	Modified_By,	LastStockStatusUpdate,	ReferenceDocCode,	" +
                    "LastTrsID,	LastTrsDate,	LastTrsTypeID,	LastTrsTypeDesc " +
                    "FROM THXADiamondLot  where ParcelOrStone = 'P' ";
                 
                 */


                string errorMessageTitle = "Access DiamondLotParcel";

                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(SqlQueryExecutor.getConnectionString(),  query, errorMessageTitle, parameters);
                if (SqlQueryExecutor.IsValid(sqlResult))
                {
                    DataRow row = sqlResult.Rows[0];

                    PopulateDiamondLotData(row, searchType);

                    /*
                     * dgv_ParcelList.DataSource = sqlResult.DefaultView;
                    if (!IsRefreshParcel)
                        cb_ParcelFilterStockStatus.SelectedIndex = 0;
                    dgv_ParcelListFormat();
                    */

                }
                else if (searchType == SearchType.LotID) {
                    MessageBox.Show($"Parcel with LotID {identifier} Is not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tb_LotID.Text = "0";
                    tb_LotID.Focus();
                }
                //dgv_ParcelList.DataBindingComplete += dgv_ParcelList_DataBindingComplete;
            }
            catch (Exception e1)
            {
                SqlQueryExecutor.ShowErrorMessage(e1, "Access Stone Detail failure.");

            }
        }


        private void PopulateDiamondLotData(DataRow row, SearchType searchType)
        {
            if (row == null) return; 
     
            dgv_WorkingLine.Rows[0].Cells["LotID"].Value = row["LotID"];
            dgv_WorkingLine.Rows[2].Cells["LotID"].Value = row["LotID"];
            dgv_WorkingLine.Rows[0].Cells["Weight"].Value = row["Weight"];
            dgv_WorkingLine.Rows[0].Cells["Cost"].Value = row["ParcelAvgCost"];

            tb_ItemID.Text = row["ItemID"].ToString();
            cb_Shape.Text = row["Shape"].ToString();
            cb_Size.Text = row["Size"].ToString();
            cb_Color.Text = row["Color"].ToString();
            cb_Clarity.Text = row["Clarity"].ToString();
            //cb_HoldingType.Text = row1["HoldingType"].ToString();
            //cb_ParcelOrStone.Text = row1["ParcelOrStone"].ToString();

            if ( row["HoldingType"].ToString() == "P")
                cb_HoldingType.SelectedItem = "Permanent";
            else if (row["HoldingType"].ToString() == "T")
                cb_HoldingType.SelectedItem = "Temporary";

            if (row["ParcelOrStone"].ToString() == "P")
                cb_ParcelOrStone.SelectedItem = "Parcel";
            else if (row["ParcelOrStone"].ToString() == "S")
                cb_ParcelOrStone.SelectedItem = "Stone";



            if (searchType == SearchType.LotID)
            {
                cb_LotName.Text = row["LotName"].ToString();
            }
            else if (searchType == SearchType.LotName)
            { 
                tb_LotID.Text = row["LotID"].ToString();
            }
        }


        private void AssignIntegerZeroToIDColumns()
        {
            
            foreach (DataGridViewRow row in dgv_WorkingLine.Rows)
            {
                if (!row.IsNewRow) // Ensures it doesn't modify the new empty row
                {
                    row.Cells["LotID"].Value = Convert.ToInt32(0);
                    row.Cells["ItemID"].Value = Convert.ToInt32(0);
                    row.Cells["Quantity"].Value = Convert.ToInt32(0);
                }
            }
        }

        private void InitializeDgvWorkingLineWithDefault0()
        {

            AssignIntegerZeroToIDColumns();
            dgv_WorkingLine.Columns["LotID"].ValueType = typeof(int);
            dgv_WorkingLine.Columns["ItemID"].ValueType = typeof(int);
            dgv_WorkingLine.Columns["Quantity"].ValueType = typeof(int);

            dgv_WorkingLine.Columns["Weight"].DefaultCellStyle.Format = "0.00";
            dgv_WorkingLine.Columns["Cost"].DefaultCellStyle.Format = "0.00";
            dgv_WorkingLine.Columns["TotalCost"].DefaultCellStyle.Format = "0.00";
            dgv_WorkingLine.Columns["List"].DefaultCellStyle.Format = "0.00";
            dgv_WorkingLine.Columns["%(-)Cost"].DefaultCellStyle.Format = "0.00";

            foreach (DataGridViewRow row in dgv_WorkingLine.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Value = 0;
                }
            }
        }

        
        private void dgv_WorkingLine_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            decimal weightRow1 = 0; decimal costDiscRow1 = 0; decimal costRow1 = 0; decimal listRow1 = 0;

            decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Weight"].Value?.ToString(), out weightRow1);
            decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Cost"].Value?.ToString(), out costRow1);
            decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["List"].Value?.ToString(), out listRow1);
            decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["%(-)Cost"].Value?.ToString(), out costDiscRow1);

            //calculating cost discount
            if (e.RowIndex == 1)
            {
                if (dgv_WorkingLine.Columns[e.ColumnIndex].Name == "List" || dgv_WorkingLine.Columns[e.ColumnIndex].Name == "Cost")
                {
                    if (listRow1 == 0)
                    {
                        dgv_WorkingLine.Rows[1].Cells["%(-)Cost"].Value = 0;
                    }
                    else if (costRow1 == 0)
                    {
                        dgv_WorkingLine.Rows[1].Cells["%(-)Cost"].Value = -100;
                    }
                    else
                    {
                        dgv_WorkingLine.Rows[1].Cells["%(-)Cost"].Value = Math.Round((costRow1 / listRow1 - 1) * 100, 2);
                    }

                }
                //UpdateWeightRow3(e);
                //UpdateTotalCostRow1(e);
                UpdateAllCostRow1(e); 
            }
        }

        /*
         private void dgv_WorkingLine_CellValueChanged(object sender, DataGridViewCellEventArgs e)
         {
             // Check if the changed cell belongs to Row 1 and Column "Weight"
             if (e.RowIndex == 1 && dgv_WorkingLine.Columns[e.ColumnIndex].Name == "Weight")
             {
                 UpdateWeightRow3();
             }
         }
        */

        // Function to calculate and update Row 3's "Weight"
        private void UpdateWeightRow3(DataGridViewCellEventArgs e)
        {
            if (dgv_WorkingLine.Rows.Count >= 3)
            {
                decimal weightRow0 = 0, weightRow1 = 0;

                decimal.TryParse(dgv_WorkingLine.Rows[0].Cells["Weight"].Value?.ToString(), out weightRow0);
                decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Weight"].Value?.ToString(), out weightRow1);

                decimal weightRow2 = weightRow0 + weightRow1;
                dgv_WorkingLine.Rows[2].Cells["Weight"].Value = weightRow2;
            }
        }
        
        private void UpdateTotalCostRow1(DataGridViewCellEventArgs e)
        {
            if (dgv_WorkingLine.Rows.Count >= 3)
            {
                decimal costRow0 = 0, weightRow0 = 0; decimal costRow1 = 0, weightRow1 = 0;

                decimal.TryParse(dgv_WorkingLine.Rows[0].Cells["Cost"].Value?.ToString(), out costRow0);
                decimal.TryParse(dgv_WorkingLine.Rows[0].Cells["Weight"].Value?.ToString(), out weightRow0);
                decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Cost"].Value?.ToString(), out costRow1);
                decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Weight"].Value?.ToString(), out weightRow1);
                decimal totalcostRow1 = costRow1 * weightRow1;               
                dgv_WorkingLine.Rows[1].Cells["TotalCost"].Value = totalcostRow1;
                decimal totalWeight = weightRow0 + weightRow1;
                if (totalWeight > 0)
                {
                    decimal costRow2 = Math.Round((costRow0 * weightRow0 + costRow1 * weightRow1) / (totalWeight), 2);
                    dgv_WorkingLine.Rows[2].Cells["Cost"].Value = costRow2;
                }
                else {
                    dgv_WorkingLine.Rows[2].Cells["Cost"].Value = 0;
                }
                
            }
        }


        private void UpdateAllCostRow1(DataGridViewCellEventArgs e)
        {
            // update cost, total cost when cost, weight, cost(£) is changed
            decimal costRow0 = 0, weightRow0 = 0, costRow1 = 0, weightRow1 = 0;
            if (dgv_WorkingLine.Rows.Count >= 3)
            {
                if (e.RowIndex == 1)
                {

                    DataGridViewRow row1 = dgv_WorkingLine.Rows[e.RowIndex];
                    DataGridViewRow row2 = dgv_WorkingLine.Rows[2];

                    if (e.ColumnIndex == dgv_WorkingLine.Columns["Cost"].Index || e.ColumnIndex == dgv_WorkingLine.Columns["Weight"].Index|| e.ColumnIndex == dgv_WorkingLine.Columns["%(-)Cost"].Index)
                    {
                        decimal.TryParse(dgv_WorkingLine.Rows[0].Cells["Cost"].Value?.ToString(), out costRow0);
                        decimal.TryParse(dgv_WorkingLine.Rows[0].Cells["Weight"].Value?.ToString(), out weightRow0);
                        decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Cost"].Value?.ToString(), out costRow1);
                        decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Weight"].Value?.ToString(), out weightRow1);
                        decimal totalcostRow1 = costRow1 * weightRow1;

                        row1.Cells["TotalCost"].Value = totalcostRow1;
                        row1.Cells["Cost(£)"].Value = Math.Round(costRow1 * GBPRate, 2);
                        row1.Cells["TotalCost(£)"].Value = Math.Round(totalcostRow1 * GBPRate, 2);

                        //Average Parcel Cost
                        decimal totalWeight = weightRow0 + weightRow1;
                        if (totalWeight > 0)
                        {
                            decimal costRow2 = Math.Round((costRow0 * weightRow0 + costRow1 * weightRow1) / (totalWeight), 2);
                            row2.Cells["Cost"].Value = costRow2;
                        }
                        else
                        {
                            row2.Cells["Cost"].Value = 0;
                        }
                    }
                    else if (e.ColumnIndex == dgv_WorkingLine.Columns["Cost(£)"].Index)
                    {
                        decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Cost(£)"].Value?.ToString(), out costRow1);
                        decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Weight"].Value?.ToString(), out weightRow1);
                        decimal totalcostRow1 = costRow1 * weightRow1;

                        row1.Cells["TotalCost(£)"].Value = totalcostRow1;
                        row1.Cells["Cost"].Value = Math.Round(costRow1 * USDRate, 2);
                        row1.Cells["TotalCost"].Value = Math.Round(totalcostRow1 * USDRate, 2);

                    }


                }


            }
           
        }
        


        private void dgv_WorkingLine_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string selectedParcelOrStone = cb_ParcelOrStone.SelectedItem.ToString() ?? "";
            decimal listRow1, costRow1, costDiscRow1;
            decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["List"].Value?.ToString(), out listRow1);
            decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Cost"].Value?.ToString(), out costRow1);
            decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["%(-)Cost"].Value?.ToString(), out costDiscRow1);
            if (selectedParcelOrStone == "Stone")
            {
                if (e.RowIndex == 1 && dgv_WorkingLine.Columns[e.ColumnIndex].Name == "Weight")
                {
                    UpdateWeightRow3(e);

                }
                if (listRow1 != 0)
                {
                    if (e.RowIndex == 1 && dgv_WorkingLine.Columns[e.ColumnIndex].Name == "Cost")
                    {
                        dgv_WorkingLine.Rows[1].Cells["%(-)Cost"].Value = Math.Round((costRow1 / listRow1 - 1) * 100, 2);
                    }
                    else if (e.RowIndex == 1 && dgv_WorkingLine.Columns[e.ColumnIndex].Name == "%(-)Cost")
                    {
                        dgv_WorkingLine.Rows[1].Cells["Cost"].Value = listRow1 * (1 + Math.Round(costDiscRow1 / 100, 2));
                    }
                    UpdateAllCostRow1(e);
                }

            }
            else if (selectedParcelOrStone == "Parcel")
            { 
            if (e.RowIndex == 1 && dgv_WorkingLine.Columns[e.ColumnIndex].Name == "Weight")
            {
                UpdateWeightRow3(e);
                //UpdateTotalCostRow1(e);
                UpdateAllCostRow1(e);

            }
            if (e.RowIndex == 1 && dgv_WorkingLine.Columns[e.ColumnIndex].Name == "Cost")
            {
                //UpdateTotalCostRow1(e);
                UpdateAllCostRow1(e);
            }
            
                                 
            }

        }

        private void ck_NewLot_CheckedChanged(object sender, EventArgs e)
        {

            if (ck_NewLot.Checked)
            {
                ck_NewLotChecked();
            }
            else
            {
                ck_NewLotUnchecked();
            }

            


        }

        private void ck_NewLotChecked() { 
            if (cb_ParcelOrStone.SelectedIndex == -1)
            {
                ck_NewLot.Checked = false;
                MessageBox.Show("Please select Parcel or Stone", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (cb_HoldingType.SelectedIndex == -1)
            {
                ck_NewLot.Checked = false;
                MessageBox.Show("Please select HoldingType", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            InitializeDgvWorkingLineWithDefault0();

            string selectedParcelOrStone = cb_ParcelOrStone.SelectedItem.ToString() ?? "";
            string selectedHoldingType = cb_HoldingType.SelectedItem.ToString() ?? "";
            int newLotID = transactionForm.MaxLotID + 1;
            int newItemID = transactionForm.MaxItemID + 1;
            if (selectedParcelOrStone == "Parcel")
            {

                if (selectedHoldingType == "Permanent")
                {
                    tb_LotID.Text = newItemID.ToString();
                    tb_ItemID.Text = newItemID.ToString();
                    dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newItemID;
                    dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newItemID;
                }
                else if (selectedHoldingType == "Temporary")
                {
                    tb_LotID.Text = newLotID.ToString();
                    tb_ItemID.Text = "400";
                    dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newLotID;
                    dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newLotID;
                }

            }
            else if (selectedParcelOrStone == "Stone")
            {
                tb_LotID.Text = newLotID.ToString();
                tb_ItemID.Text = "100";
                dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newLotID;
                dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newLotID;



            }
            

        }

        private void ck_NewLotUnchecked()
        {
            InitializeDgvWorkingLineWithDefault0();
            tb_LotID.Text = "0";
            tb_ItemID.Text = "0";
            dgv_WorkingLine.Rows[0].Cells["LotID"].Value = 0;
            dgv_WorkingLine.Rows[2].Cells["LotID"].Value = 0;
            
        }

        private void cb_HoldingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_ParcelOrStone.SelectedIndex == -1)
            {
                ck_NewLot.Checked = false;
                MessageBox.Show("Please select Parcel or Stone", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cb_ParcelOrStone.Focus();
                return;
            }

            string selectedParcelOrStone = cb_ParcelOrStone.SelectedItem.ToString() ?? "";
            string selectedHoldingType = cb_HoldingType.SelectedItem.ToString() ?? "";
            int newLotID = transactionForm.MaxLotID + 1;
            int newItemID = transactionForm.MaxItemID + 1;

            if (selectedParcelOrStone == "Parcel")
            {

                if (selectedHoldingType == "Permanent" && ck_NewLot.Checked == true)
                {
                    tb_LotID.Text = newItemID.ToString();
                    tb_ItemID.Text = newItemID.ToString();
                    dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newItemID;
                    dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newItemID;
                }
                else if (selectedHoldingType == "Temporary" && ck_NewLot.Checked == true) {
                    tb_LotID.Text = newLotID.ToString();         
                    tb_ItemID.Text = "400";
                    dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newLotID;
                    dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newLotID;
                }

            }
            else if (selectedParcelOrStone == "Stone" && ck_NewLot.Checked == true)
            {
                tb_LotID.Text = newLotID.ToString();
                tb_ItemID.Text = "100";
                dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newLotID;
                dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newLotID;
                dgv_WorkingLine.Rows[0].Cells["LotName"].Value = "Cert";
                dgv_WorkingLine.Rows[2].Cells["LotName"].Value = "Cert";

            }

        }

        private void dgv_WorkingLine_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string columnName = dgv_WorkingLine.Columns[e.ColumnIndex].Name;
            string parcelOrStone = dgv_WorkingLine.Rows[e.RowIndex].Cells["ParcelOrStone"].Value?.ToString();

            if (columnName == "Weight")
            {
                e.CellStyle.Font = new Font(dgv_WorkingLine.Font, FontStyle.Bold);
            }

            if (columnName == "Cost")
            {
                e.CellStyle.ForeColor = Color.Red;
            }

            if (columnName == "Size" && parcelOrStone == "Parcel")
            {
                return; 
            }

            if (e.Value != null && columnName != "LotID" && columnName != "ItemID" && columnName != "Quantity")
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal result))
                {
                    e.Value = result.ToString("N2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void cb_ParcelOrStone_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedParcelOrStone = cb_ParcelOrStone.SelectedItem?.ToString() ?? "";
            string selectedLotName = cb_LotName.SelectedItem?.ToString() ?? "";
            string selectedHoldingType =string.Empty;
            if (cb_HoldingType.Text.Trim() != ""){ 
                selectedHoldingType = cb_HoldingType.SelectedItem.ToString() ?? "";
            }
            

            int newLotID = transactionForm.MaxLotID + 1;
            int newItemID = transactionForm.MaxItemID + 1;

            if (selectedParcelOrStone == "Stone")
            {
                tb_ItemID.Text = "100";
                cb_LotName.SelectedItem = "Cert";   
                
                if (ck_NewLot.Checked == true)
                {
                    tb_LotID.Text = newLotID.ToString();
                    dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newLotID;
                    dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newLotID;
                }
            }
            else if (selectedParcelOrStone == "Parcel" && selectedLotName == "Cert")
            {
                cb_LotName.SelectedIndex = -1;

                if (selectedHoldingType == "Permanent" && ck_NewLot.Checked == true)
                {
                    tb_LotID.Text = newItemID.ToString();
                    tb_ItemID.Text = newItemID.ToString();
                    dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newItemID;
                    dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newItemID;
                }
                else if (selectedHoldingType == "Temporary" && ck_NewLot.Checked == true)
                {
                    tb_LotID.Text = newLotID.ToString();
                    tb_ItemID.Text = "400";
                    dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newLotID;
                    dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newLotID;
                }
            }

        }


        //NewLot
        private void AccessDiamondLotMaxItemOrLotIDFor(string ParcelOrStone)
        {
            try
            {
                string query, errorMessageTitle;
                //string columnName = ParcelOrStone == "Parcel" ? "MAXItemID" : "MAXLotID";
                if (ParcelOrStone == "Parcel")
                {

                    query = @"
                    SELECT 
                    (SELECT MAX(ItemID) as MAXItemID FROM THXADiamondLot where ItemID = LotID) AS MaxItemID,
                    (SELECT MAX(LotID) FROM THXADiamondLot) AS MaxLotID";
                    errorMessageTitle = "Access DiamondLot MAXLotID and MaxItemID ";

                }
                else if (ParcelOrStone == "Stone")
                {
                    query = @"
                    SELECT 
                    (SELECT MAX(LotID) FROM THXADiamondLot) AS MaxLotID,
                    (SELECT MAX(CertificateID) FROM THXADiamondCertificate) AS MaxCertificateID";
                    errorMessageTitle = "Access DiamondLot MAXLotID and Max CertificateID ";
                }
                else
                {
                    throw new ArgumentException("Invalid input for ParcelOrStone");
                }


                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(SqlQueryExecutor.getConnectionString(), query, errorMessageTitle);
                if (SqlQueryExecutor.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
                {
                    //object maxIDValue = sqlResult.WorkingLine[0][columnName];
                    if (SqlQueryExecutor.IsValid(sqlResult))
                    {
                        if (ParcelOrStone == "Parcel")
                        {
                            MaxItemID = Convert.ToInt32(sqlResult.Rows[0]["MaxItemID"]);
                            MaxLotID = Convert.ToInt32(sqlResult.Rows[0]["MaxLotID"]);
                        }
                        else if (ParcelOrStone == "Stone")
                        {
                            MaxLotID = Convert.ToInt32(sqlResult.Rows[0]["MaxLotID"]);
                            MaxCertificateID = Convert.ToInt32(sqlResult.Rows[0]["MaxCertificateID"]);
                        }
                    }
                }

            }
            catch (Exception e1)
            {
                SqlQueryExecutor.ShowErrorMessage(e1, "Access MAXItemID in THXADiamondLot failure.");

            }
        }

        private void bt_AddCertificate_Click(object sender, EventArgs e)
        {
            Certificate certificate = new Certificate(newDocTypeID, ref certificateData);
            certificate.Show();
        }

        public void ApplyColumnVisibility(DataGridView dgv, int docType)
        {
            // Ensure the document type exists in the hidden columns dictionary
            if (ColumnManager.WorkingLineHiddenColumnsByDocType.ContainsKey(docType))
            {
                List<string> columnsToHide = ColumnManager.WorkingLineHiddenColumnsByDocType[docType];

                foreach (string columnName in columnsToHide)
                {
                    if (dgv.Columns.Contains(columnName))
                    {
                        dgv.Columns[columnName].Visible = false; //Hide the column
                    }
                }
            }
        }


        private void bt_FindList_Click(object sender, EventArgs e)
        {
            string shape = cb_Shape.Text.ToString();
            string size = cb_Size.Text.ToString();
            string color = cb_Color.Text.ToString();
            string clarity = cb_Clarity.Text.ToString();

            decimal costDiscRow1, costRow1;
            decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Cost"].Value?.ToString(), out costRow1);
            decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["%(-)Cost"].Value?.ToString(), out costDiscRow1);

            if (!_rapaportPriceService.IsValidRapShapeScale(shape))
            {
                MessageBox.Show($"Cannot find Rapaport shape {shape}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cb_Shape.Focus();
            }
            else if (!_rapaportPriceService.IsValidRapGradingScale("size", size))
            {
                MessageBox.Show($"Cannot find Rapaport size {size}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cb_Size.Focus();
            }

            else if (!_rapaportPriceService.IsValidRapGradingScale("color", color))
            {
                MessageBox.Show($"Cannot find Rapaport color {color}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cb_Color.Focus();
            }

            else if (!_rapaportPriceService.IsValidRapGradingScale("clarity", clarity))
            {
                MessageBox.Show($"Cannot find Rapaport clarity {clarity}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cb_Clarity.Focus();
            }
            else { 
            
                decimal listPrice = Convert.ToDecimal(FindDiamondListPrice(shape, size, color, clarity));
                if (dgv_WorkingLine.Rows.Count >= 3)
                { 
                    dgv_WorkingLine.Rows[1].Cells["List"].Value = listPrice;
                    if(costRow1 == 0)
                        dgv_WorkingLine.Rows[1].Cells["%(-)Cost"].Value = -100;
                    else
                        dgv_WorkingLine.Rows[1].Cells["%(-)Cost"].Value = Math.Round((costRow1 / listPrice - 1) * 100, 2);
                   
                }
                       
            }


        }

        private string FindDiamondListPrice(string shape, string size, string color, string clarity)
        {
            if (shape != "BR") shape = "PS"; 

            try
            {
                string query = $"SELECT ListPrice FROM THXADiamondRapList WHERE Shape_Abbr = @Shape AND Size = @Size AND Color = @Color AND Clarity = @Clarity ";
               
                string errorMessageTitle = "Find List Price for the Diamond ";


                var parameters = new Dictionary<string, object>
                {
                    { "@Shape", shape },
                    { "@Size", size },
                    { "@Color", color },
                    { "@Clarity", clarity }                   
                };

                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(SqlQueryExecutor.getConnectionString(), query,errorMessageTitle, parameters);
                if (SqlQueryExecutor.IsValid(sqlResult))
                {
                    return (sqlResult.Rows[0]["ListPrice"].ToString());
                }
                else
                {
                    MessageBox.Show("List Price is not found, check the diamond grading detail. ", "Message");
                    return string.Empty;
                }
            }
            catch (Exception e1)
            {
                SqlQueryExecutor.ShowErrorMessage(e1, "Access Stone Detail failure.");
                return string.Empty;

            }
        }



    }
}
