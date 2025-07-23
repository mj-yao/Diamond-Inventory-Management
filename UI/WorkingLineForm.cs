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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using DiamondTransaction.UseCases.Models;
using DiamondTransaction.DataAccess;
using DiamondTransaction.UseCases.Services;
using DiamondTransaction.Domain.Entities;
using DiamondTransaction.UI.Event;
using DiamondTransaction.UI.Utilities;
using DiamondTransaction.UI.GridConfig;
using static DiamondTransaction.UI.GridConfig.ColumnConfigurations;
using System.Data.Common;

namespace DiamondTransaction
{
    public partial class WorkingLineForm : Form
    {
        private int _docTypeID;

        //public string connectionString = @"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql";
        public event EventHandler<SaveWorkingLineEventArgs> SaveWorkingLine;


        private int _maxItemID, _maxLotID, _maxCertificateID;
        public decimal USDRate, GBPRate; 
        private int _parentLotID = 0; // Used to store the parent LotID for new Lot creation
        private bool _isProgrammaticChange = false;
        private string lastEditedColumn = "";


        enum SearchType { LotID, LotName }

        private DiamondLotMaxID _diamondLotMaxID; // Used to store the max LotID, ItemID, and CertificateID
        private DocLineDto _docLineDto;
        private CertificateDto _certificate;
        private SourceLotChangeDto _sourceLotChangeDto;
        private WorkingLineDto _workingLineDto; 
        private DiamondLotDto _diamondLotSelected;

        private ParcelGrades _parcelGrades;
        private List<string> _lotNames;


        //Application dependency
        private RapaportPriceService _rapaportPriceService;
        private DiamondDocService _diamondDocService;
        private PriceStockHistoryService _priceStockHistoryService;
        private DiamondLotService _diamondLotService;



        public WorkingLineForm (DiamondLotMaxID diamondLotMaxID, WorkingLineDto workingLineDto)
        {
            InitializeComponent();
            _diamondLotMaxID = diamondLotMaxID ?? new DiamondLotMaxID(0, 0, 0);
            _maxItemID = _diamondLotMaxID.MaxItemID;
            _maxLotID = _diamondLotMaxID.MaxLotID;
            _maxCertificateID = _diamondLotMaxID.MaxCertificateID;
            _docLineDto = workingLineDto.DocLineDto ?? new DocLineDto();
            _certificate = workingLineDto.Certificate ?? new CertificateDto();
            _sourceLotChangeDto = workingLineDto.SourceLotChangeDto ?? new SourceLotChangeDto();
            _docTypeID = _docLineDto.DocTypeID;

            InitializeServices();
            this.Load += WorkingLineForm_Load;

            InitializeWorkingLineTable(dgv_WorkingLine);
            PopulateFormFromDto();
            ConfigureWorkingLineGridColumns(dgv_WorkingLine, _docTypeID);

        }

        private void InitializeServices() 
        {
            var rapaportRepository = new RapaportDataAccess(SqlQueryExecutor.getConnectionString());
            _rapaportPriceService = new RapaportPriceService(rapaportRepository);

            var diamondDocRepository = new DiamondDocDataAccess(SqlQueryExecutor.getConnectionString());
            _diamondDocService = new DiamondDocService(diamondDocRepository);

            var priceStockHistoryRepository = new PriceStockHistoryDataAccess(SqlQueryExecutor.getConnectionString());
            _priceStockHistoryService = new PriceStockHistoryService(priceStockHistoryRepository);

            var diamondLotRepository = new DiamondLotDataAccess(SqlQueryExecutor.getConnectionString());
            _diamondLotService = new DiamondLotService(diamondLotRepository);
        }

        private async void WorkingLineForm_Load(object sender, EventArgs e)
        {
            try
            {
                await InitializeDataAndControls();  
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: " + ex.Message);
                //Close(); // optionally close the form if critical data is missing
            }
        }


        private async Task InitializeDataAndControls()
        {
            await LoadData();
            InitialiseControls();
        }

        private async Task LoadData()
        {
            LoadParcelGrades();
            await LoadLotNames();
            LoadExRate();
        }

        private void LoadParcelGrades()
        {
            _parcelGrades = _diamondDocService.GetAllParcelGradingScales();
        }
        private async Task LoadLotNames()
        {
            _lotNames = await _priceStockHistoryService.GetExistingLotNamesAsync();
        }
        private void LoadExRate()
        {
            var exRate = _diamondDocService.GetLatestExchangeRate();
            if (exRate != null)
            {
                USDRate = exRate.USDRate;
                GBPRate = exRate.GBPRate;
            }
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

            FormControlHelper.SetComboBoxItem(cb_shape, shapes);
            FormControlHelper.SetComboBoxItem(cb_size, sizes);
            FormControlHelper.SetComboBoxItem(cb_color, colors);
            FormControlHelper.SetComboBoxItem(cb_clarity, clarities);

        }

        private void InitialiseLotNameComboBox(ComboBox cb_lotName)
        {
            FormControlHelper.SetComboBoxItem(cb_lotName, _lotNames);
            FormControlHelper.SetupAutoComplete(cb_lotName, _lotNames);
        }

        //----------------------------------------------------------------------------------------------------
        private void InitializeWorkingLineTable(DataGridView grid)
        {
            var columnConfig = new WorkingLineColumnConfiguration();
            var builder  = new GridColumnBuilder(columnConfig);
            builder.ConfigureGrid(grid, _docTypeID);
            builder.ConfigureGridFormat(grid, _docTypeID);
            dgv_WorkingLine.Rows.Add(3); // 3 rows (Row 0, Row 1, and Row 2 for calculations result)
            this.Controls.Add(dgv_WorkingLine);

            ConfigureWorkingLineGridColumns(dgv_WorkingLine, _docTypeID);
            InitializeWorkingLineGridValues(dgv_WorkingLine);
                     
        }

        private void PopulateFormFromDto()
        {
            _isProgrammaticChange = true;
            // DocLineDto fields
            tb_LotID.Text = _docLineDto.LotID.ToString();
            tb_ItemID.Text = _docLineDto.ItemID.ToString();
            cb_LotName.Text = _docLineDto.LotName;
            cb_HoldingType.Text = _docLineDto.HoldingType;
            cb_ParcelOrStone.Text = _docLineDto.ParcelOrStone;
            cb_Shape.Text = _docLineDto.Shape;
            cb_Size.Text = _docLineDto.Size;
            cb_Color.Text = _docLineDto.Color;
            cb_Clarity.Text = _docLineDto.Clarity;
            tb_Remark.Text = _docLineDto.Remark;


            DataGridViewRow sourceRow = dgv_WorkingLine.Rows[0];
            DataGridViewRow editRow = dgv_WorkingLine.Rows[1];
            DataGridViewRow finalRow = dgv_WorkingLine.Rows[2];

            if (_docTypeID == 1006 || _docTypeID == 1007 || _docTypeID == 1009 || _docTypeID == 1010)
            {
                sourceRow.Cells["LotID"].Value = _sourceLotChangeDto.LotID;
                sourceRow.Cells["Quantity"].Value = _sourceLotChangeDto.SourceQuantity;
                sourceRow.Cells["Weight"].Value = _sourceLotChangeDto.SourceWeight;
                sourceRow.Cells["List"].Value = _sourceLotChangeDto.SourceList;
                sourceRow.Cells["ListCostDiscount"].Value = _sourceLotChangeDto.SourceListCostDiscount ?? 0m;
                sourceRow.Cells["Cost"].Value = _sourceLotChangeDto.SourceCost;
                sourceRow.Cells["TotalCost"].Value = _sourceLotChangeDto.SourceTotalCost;
                sourceRow.Cells["SecCost"].Value = _sourceLotChangeDto.SourceSecCost;
                sourceRow.Cells["SecTotalCost"].Value = _sourceLotChangeDto.SourceSecTotalCost;

                editRow.Cells["LotID"].Value = _docLineDto.LotID;
                editRow.Cells["Quantity"].Value = _docLineDto.Quantity;
                editRow.Cells["Weight"].Value = _docLineDto.Weight;
                editRow.Cells["List"].Value = _docLineDto.List;
                editRow.Cells["ListCostDiscount"].Value = _docLineDto.ListCostDiscount ?? 0m;
                editRow.Cells["Cost"].Value = _docLineDto.Cost;
                editRow.Cells["TotalCost"].Value = _docLineDto.TotalCost;
                editRow.Cells["SecCost"].Value = _docLineDto.SecCost;
                editRow.Cells["SecTotalCost"].Value = _docLineDto.SecTotalCost;

                finalRow.Cells["LotID"].Value = _docLineDto.LotID;
                finalRow.Cells["Quantity"].Value = _sourceLotChangeDto.FinalQuantity;
                finalRow.Cells["Weight"].Value = _sourceLotChangeDto.FinalWeight;
                finalRow.Cells["List"].Value = _sourceLotChangeDto.SourceList;
                finalRow.Cells["ListCostDiscount"].Value = _sourceLotChangeDto.SourceListCostDiscount ?? 0m;
                finalRow.Cells["Cost"].Value = _sourceLotChangeDto.FinalCost;
                finalRow.Cells["TotalCost"].Value = _sourceLotChangeDto.FinalTotalCost;
                finalRow.Cells["SecCost"].Value = _sourceLotChangeDto.FinalSecCost;
                finalRow.Cells["SecTotalCost"].Value = _sourceLotChangeDto.FinalSecTotalCost;
                

            }
            else if (_docTypeID == 1001 || _docTypeID == 1002 || _docTypeID == 1003 || _docTypeID == 1005)
            {
                sourceRow.Cells["LotID"].Value = _sourceLotChangeDto.LotID;
                sourceRow.Cells["Quantity"].Value = _sourceLotChangeDto.SourceQuantity;
                sourceRow.Cells["Weight"].Value = _sourceLotChangeDto.SourceWeight;

                editRow.Cells["LotID"].Value = _docLineDto.LotID;
                editRow.Cells["Quantity"].Value = _docLineDto.Quantity;
                editRow.Cells["Weight"].Value = _docLineDto.Weight;
                editRow.Cells["OutMainPrice"].Value = _docLineDto.OutMainPrice;
                editRow.Cells["OutMainTotalPrice"].Value = _docLineDto.OutMainTotalPrice;
                editRow.Cells["OutListMainDiscount"].Value = _docLineDto.OutListMainDiscount ?? 0m;
                editRow.Cells["OutList"].Value = _docLineDto.OutList;
                editRow.Cells["OutSecPrice"].Value = _docLineDto.OutSecPrice;
                editRow.Cells["OutSecTotalPrice"].Value = _docLineDto.OutSecTotalPrice;

                finalRow.Cells["LotID"].Value = _docLineDto.LotID;
                finalRow.Cells["Quantity"].Value = _sourceLotChangeDto.FinalQuantity;
                finalRow.Cells["Weight"].Value = _sourceLotChangeDto.FinalWeight;
            }

            _isProgrammaticChange = false;

        }


        private void bt_LotFinder_Click(object sender, EventArgs e)
        {
            LotFinderForm lotFinder = new LotFinderForm();

            lotFinder.OnLotSelected += async (lotID) =>
            {

                if (!ck_NewLot.Checked)
                {
                    tb_LotID.Text = lotID;
                    await ConfirmLotIDInputAsync();
                }
                else {
                    MessageBox.Show("Lot can not be loaded when 'New Lot' checkbox is marked.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }               
                
            };
            
            lotFinder.Show();
        }


        private void bt_Save_Click(object sender, EventArgs e)
        {
            
            if (!FormControlHelper.ValidateControl(cb_LotName, "Please enter Lot Name.")) return;
            if (!FormControlHelper.HasIntValue(tb_LotID.Text)) { MessageBox.Show("Please enter a valid Lot ID."); tb_LotID.Focus(); return; }
            if (!FormControlHelper.HasIntValue(tb_ItemID.Text)) { MessageBox.Show("Please enter a valid Item ID."); tb_ItemID.Focus(); return; }
            if (!FormControlHelper.ValidateControl(cb_Shape, "Please enter Shape.")) return;
            if (!FormControlHelper.ValidateControl(cb_Size, "Please enter Size.")) return;
            if (!FormControlHelper.ValidateControl(cb_Color, "Please enter Color.")) return;
            if (!FormControlHelper.ValidateControl(cb_Clarity, "Please enter Clarity.")) return;
            if (!FormControlHelper.ValidateControl(cb_ParcelOrStone, "Please enter Parcel or Stone.")) return;
            if (!FormControlHelper.ValidateControl(cb_HoldingType, "Please enter Holding Type.")) return;
            //Check certificate info is added
            if (cb_ParcelOrStone.Text.ToString() == "Stone") { 
                if (string.IsNullOrEmpty(_certificate.CertificateLabName) || string.IsNullOrEmpty(_certificate.CertificateNo))
                {
                    MessageBox.Show("Certificate lab name and number are required.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (dgv_WorkingLine.Rows.Count >= 3)
            {               
                _docLineDto.LotName = cb_LotName.Text.ToString();
                _docLineDto.LotID = FormControlHelper.GetIntValue(tb_LotID.Text);
                _docLineDto.ItemID = FormControlHelper.GetIntValue(tb_ItemID.Text);
                _docLineDto.Shape = cb_Shape.Text.ToString();
                _docLineDto.Size = cb_Size.Text.ToString();
                _docLineDto.Color = cb_Color.Text.ToString();
                _docLineDto.Clarity = cb_Clarity.Text.ToString();
                _docLineDto.ParcelOrStone = cb_ParcelOrStone.Text.ToString();
                _docLineDto.HoldingType = cb_HoldingType.Text.ToString();
                _docLineDto.Remark = tb_Remark.Text.Trim();

                _sourceLotChangeDto.LotID = FormControlHelper.GetIntValue(tb_LotID.Text);
                _certificate.LotID = FormControlHelper.GetIntValue(tb_LotID.Text);


            }

            if (_docTypeID == 1006 || _docTypeID == 1007 || _docTypeID == 1009 || _docTypeID == 1010)
            {               
                _docLineDto.LotID = Convert.ToInt32(dgv_WorkingLine.Rows[1].Cells["LotID"].Value);                
                _docLineDto.Quantity = Convert.ToInt32(dgv_WorkingLine.Rows[1].Cells["Quantity"].Value);
                _docLineDto.Weight = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["Weight"].Value);
                _docLineDto.List = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["List"].Value);
                _docLineDto.ListCostDiscount = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["ListCostDiscount"].Value);
                _docLineDto.Cost = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["Cost"].Value);
                _docLineDto.TotalCost = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["TotalCost"].Value);
                _docLineDto.SecCost = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["SecCost"].Value);
                _docLineDto.SecTotalCost = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["SecTotalCost"].Value);

                _sourceLotChangeDto.SourceWeight = Convert.ToDecimal(dgv_WorkingLine.Rows[0].Cells["Weight"].Value);
                _sourceLotChangeDto.SourceQuantity = Convert.ToInt32(dgv_WorkingLine.Rows[0].Cells["Quantity"].Value);
                _sourceLotChangeDto.SourceList = Convert.ToDecimal(dgv_WorkingLine.Rows[0].Cells["List"].Value);
                _sourceLotChangeDto.SourceListCostDiscount = Convert.ToDecimal(dgv_WorkingLine.Rows[0].Cells["ListCostDiscount"].Value);
                _sourceLotChangeDto.SourceCost = Convert.ToDecimal(dgv_WorkingLine.Rows[0].Cells["Cost"].Value);
                _sourceLotChangeDto.SourceTotalCost = Convert.ToDecimal(dgv_WorkingLine.Rows[0].Cells["TotalCost"].Value);
                _sourceLotChangeDto.SourceSecCost = Convert.ToDecimal(dgv_WorkingLine.Rows[0].Cells["SecCost"].Value);
                _sourceLotChangeDto.SourceSecTotalCost = Convert.ToDecimal(dgv_WorkingLine.Rows[0].Cells["SecTotalCost"].Value);
                _sourceLotChangeDto.FinalWeight = Convert.ToDecimal(dgv_WorkingLine.Rows[2].Cells["Weight"].Value);
                _sourceLotChangeDto.FinalQuantity = Convert.ToInt32(dgv_WorkingLine.Rows[2].Cells["Quantity"].Value);
                _sourceLotChangeDto.FinalCost = Convert.ToDecimal(dgv_WorkingLine.Rows[2].Cells["Cost"].Value);
                _sourceLotChangeDto.FinalTotalCost = Convert.ToDecimal(dgv_WorkingLine.Rows[2].Cells["TotalCost"].Value);
                _sourceLotChangeDto.FinalSecCost = Convert.ToDecimal(dgv_WorkingLine.Rows[2].Cells["SecCost"].Value);
                _sourceLotChangeDto.FinalSecTotalCost = Convert.ToDecimal(dgv_WorkingLine.Rows[2].Cells["SecTotalCost"].Value);


            }
            else if (_docTypeID == 1001 || _docTypeID == 1002 || _docTypeID == 1003 || _docTypeID == 1005)
            {
                _docLineDto.LotID = Convert.ToInt32(dgv_WorkingLine.Rows[1].Cells["LotID"].Value);
                _docLineDto.Quantity = Convert.ToInt32(dgv_WorkingLine.Rows[1].Cells["Quantity"].Value);
                _docLineDto.Weight = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["Weight"].Value);
                _docLineDto.OutMainPrice = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["OutMainPrice"].Value);
                _docLineDto.OutMainTotalPrice = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["OutMainTotalPrice"].Value);
                _docLineDto.OutListMainDiscount = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["OutListMainDiscount"].Value);
                _docLineDto.OutList = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["OutList"].Value);
                _docLineDto.OutSecPrice = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["OutSecPrice"].Value);
                _docLineDto.OutSecTotalPrice = Convert.ToDecimal(dgv_WorkingLine.Rows[1].Cells["OutSecTotalPrice"].Value);

                _sourceLotChangeDto.SourceWeight = Convert.ToDecimal(dgv_WorkingLine.Rows[0].Cells["Weight"].Value);
                _sourceLotChangeDto.SourceQuantity = Convert.ToInt32(dgv_WorkingLine.Rows[0].Cells["Quantity"].Value);
                _sourceLotChangeDto.FinalWeight = Convert.ToDecimal(dgv_WorkingLine.Rows[2].Cells["Weight"].Value);
                _sourceLotChangeDto.FinalQuantity = Convert.ToInt32(dgv_WorkingLine.Rows[2].Cells["Quantity"].Value);

            }

            _diamondLotMaxID.UpdateMaxItemID(_maxItemID);
            _diamondLotMaxID.UpdateMaxLotID(_maxLotID);

            _workingLineDto = new WorkingLineDto(_docLineDto, _certificate, _sourceLotChangeDto);

            SaveWorkingLine?.Invoke(this, new SaveWorkingLineEventArgs(_diamondLotMaxID, _workingLineDto ));

            this.Close();
            
        }

        private DocLineDto GetWorkingLineDataTo(DocLineDto docLineDto)
        {
            docLineDto.ParcelOrStone = cb_ParcelOrStone.SelectedItem.ToString();
            docLineDto.HoldingType = cb_HoldingType.SelectedItem.ToString();
            docLineDto.LotName = cb_LotName.Text.ToString();
            docLineDto.LotID = FormControlHelper.GetIntValue(tb_LotID.Text);          
            docLineDto.ItemID = FormControlHelper.GetIntValue(tb_ItemID.Text);
            docLineDto.Shape = cb_Shape.SelectedItem.ToString();
            docLineDto.Size = cb_Size.SelectedItem.ToString();
            docLineDto.Color = cb_Color.SelectedItem.ToString();
            docLineDto.Clarity = cb_Clarity.SelectedItem.ToString();


            return docLineDto == null ? null : new DocLineDto();
        
        }


 
        public void OldUpdateMaxLotIDs(string lotID)
        {
            // Check if LotID is a 6-digit number
            if (lotID.Length == 6)
            {
                // Compare and set _maxLotID if it's smaller
                if (_maxLotID < int.Parse(lotID))
                {
                    _diamondLotMaxID.MaxLotID = int.Parse(lotID);
                }
            }
            // Check if LotID is a 4-digit number
            else if (lotID.Length == 4)
            {
                // Compare and set _maxItemID if it's smaller
                if (_maxItemID < int.Parse(lotID))
                {
                    _maxItemID = int.Parse(lotID);
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


        private async void cb_LotName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter ||  e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true; 
                await ConfirmLotNameInputAsync();
            }
        }
        private async void cb_LotName_Leave(object sender, EventArgs e)
        {
            await ConfirmLotNameInputAsync();
        }


        private async void tb_LotID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true; 
                await ConfirmLotIDInputAsync();
            }
        }


        private async void tb_LotID_Leave(object sender, EventArgs e)
        {
            if (int.TryParse(tb_LotID.Text.Trim(), out int lotID) && lotID != 0)
                await ConfirmLotIDInputAsync();
        }


        private async Task ConfirmLotNameInputAsync()
        {
            string lotName = cb_LotName.Text.Trim();

            //Case 1
            if (string.IsNullOrEmpty(lotName))
                return;
            //Case 2
            if (lotName == "Cert")
            {
                string selectedParcelOrStone = cb_ParcelOrStone.SelectedItem.ToString() ?? "";
                if (selectedParcelOrStone == "Parcel")
                {
                    MessageBox.Show("Cert cannot be the name of a parcel, please choose other name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cb_LotName.Text = string.Empty;
                    cb_LotName.Focus();
                    return;
                }
                else
                    //For Stone, Choose Cert as LotName does not fetch anything
                    return; 
            }
            //Case 3 For new lot, a used LotName is not allowed    
            if (ck_NewLot.Checked)
            {
                if (cb_LotName.SelectedIndex >= 0 || cb_LotName.Items.Contains(lotName))
                {
                    MessageBox.Show("LotName is already used, please choose other name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cb_LotName.Text = string.Empty;
                }
                return;
            }

            //Case 4 Loading the Lot by LotName which is not Cert
            if (_lotNames.Contains(lotName, StringComparer.OrdinalIgnoreCase))
            {
                var lot = await LoadDiamondLotSelectedBy(lotName, SearchType.LotName);

                if (lot != null)
                {
                    PopulateDiamondLotDtoData(lot, SearchType.LotName);
                    _diamondLotSelected = lot;
                }
            }
            else
            {
                MessageBox.Show("LotName does not exist. Please select a valid one.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private async Task ConfirmLotIDInputAsync()
        {
            string lotID = tb_LotID.Text.Trim();

            if (string.IsNullOrEmpty(lotID) || ck_NewLot.Checked)
                return;

            var lot = await LoadDiamondLotSelectedBy(lotID, SearchType.LotID);

            if (lot != null)
            {
                PopulateDiamondLotDtoData(lot, SearchType.LotID);
                _diamondLotSelected = lot;
            }
            else
            {
                HandleLotNotFound(lotID);
            }
        }


        private async Task<DiamondLotDto> LoadDiamondLotSelectedBy(string identifier, SearchType searchType)
        {
            try
            {               
                return await _diamondLotService.GetDiamondLotAsync(identifier, searchType.ToString());
            }
            catch (Exception ex)
            {
                SqlQueryExecutor.ShowErrorMessage(ex, "Failed to load diamond lot.");
                return null;
            }
        }

        private void HandleLotNotFound(string lotID)
        {
            MessageBox.Show($"Parcel with LotID {lotID} is not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            tb_LotID.Text = "0";
            tb_LotID.Focus();
        }


        private void PopulateDiamondLotDtoData(DiamondLotDto dto, SearchType searchType)
        {
            if (dto == null) return;

            dgv_WorkingLine.Rows[0].Cells["LotID"].Value = dto.LotID;
            dgv_WorkingLine.Rows[1].Cells["LotID"].Value = dto.LotID;
            dgv_WorkingLine.Rows[2].Cells["LotID"].Value = dto.LotID;
            dgv_WorkingLine.Rows[0].Cells["Weight"].Value = dto.Weight;
            dgv_WorkingLine.Rows[0].Cells["Cost"].Value = dto.ParcelAvgCost ?? dto.Cost;

            tb_ItemID.Text = dto.ItemID.ToString();
            cb_Shape.Text = dto.Shape;
            cb_Size.Text = dto.Size;
            cb_Color.Text = dto.Color;
            cb_Clarity.Text = dto.Clarity;

            cb_HoldingType.SelectedItem = dto.HoldingType == "P" ? "Permanent" :
                                          dto.HoldingType == "T" ? "Temporary" : null;

            cb_ParcelOrStone.SelectedItem = dto.ParcelOrStone == "P" ? "Parcel" :
                                            dto.ParcelOrStone == "S" ? "Stone" : null;

            if (searchType == SearchType.LotID)
                cb_LotName.Text = dto.LotName;
            else if (searchType == SearchType.LotName)
                tb_LotID.Text = dto.LotID.ToString();

        }




        private void ConfigureWorkingLineGridColumns(DataGridView grid, int docTypeID)
        {
            var intColumns = new HashSet<string> { "LotID", "Quantity" };
            const string weightColumn = "Weight";

            HashSet<string> decimalColumns = new HashSet<string>();
            if (GridColumns.DocLineColumnByDocType.ContainsKey(docTypeID))
            {
                decimalColumns = GridColumns.DocLineColumnByDocType[docTypeID]
                .Except(intColumns)
                .ToHashSet();
            }


            foreach (DataGridViewColumn column in grid.Columns)
            {
                string columnName = column.Name;

                if (intColumns.Contains(columnName))
                {
                    column.ValueType = typeof(int);
                    column.DefaultCellStyle.Format = "0";  
                }
                else if (columnName == weightColumn)
                {
                    column.ValueType = typeof(decimal);
                    column.DefaultCellStyle.Format = "0.000";
                }
                else if (decimalColumns.Contains(columnName))
                {
                    column.ValueType = typeof(decimal);
                    column.DefaultCellStyle.Format = "0.00";
                }
            }
        }

        private void InitializeWorkingLineGridValues(DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Value = 0;
                    
                    var column = grid.Columns[cell.ColumnIndex];
                    if (column.ValueType == typeof(int))
                        cell.Value = 0;
                    else if (column.ValueType == typeof(decimal))                   
                        cell.Value = 0m;                       
                    else
                        cell.Value = 0; // fallback for unknown types
                    
                }
            }
        }



        //------------------------------WorkingLine Calculation------------------------------------    

        private void dgv_WorkingLine_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            lastEditedColumn = dgv_WorkingLine.Columns[e.ColumnIndex].Name;
        }

        private void dgv_WorkingLine_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (String.IsNullOrEmpty(cb_ParcelOrStone.Text.Trim()))
            {
                MessageBox.Show("Missing ParcelOrStone", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgv_WorkingLine.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0.00;

                return;
            }
            if (e.RowIndex == 1)
                RecalculateTransactionRows();
        }

        private void RecalculateTransactionRows()
        {
            if (String.IsNullOrEmpty(cb_ParcelOrStone.Text.Trim()))
            {
                MessageBox.Show("Missing ParcelOrStone", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string selectedParcelOrStone = cb_ParcelOrStone.SelectedItem.ToString() ?? "";

            if (dgv_WorkingLine.Rows.Count < 3) return;

            var row0 = dgv_WorkingLine.Rows[0];
            var row1 = dgv_WorkingLine.Rows[1];
            var row2 = dgv_WorkingLine.Rows[2];

            // === Extract Values ===
            decimal cost0 = FormControlHelper.GetDecimalValue(row0.Cells["Cost"].Value.ToString().Trim());
            decimal weight0 = FormControlHelper.GetDecimalValue(row0.Cells["Weight"].Value.ToString().Trim());

            decimal cost1 = FormControlHelper.GetDecimalValue(row1.Cells["Cost"].Value.ToString().Trim());
            decimal weight1 = FormControlHelper.GetDecimalValue(row1.Cells["Weight"].Value.ToString().Trim());
            decimal list1 = FormControlHelper.GetDecimalValue(row1.Cells["List"].Value.ToString().Trim());
            decimal discount1 = FormControlHelper.GetDecimalValue(row1.Cells["ListCostDiscount"].Value.ToString().Trim());
            decimal secCost1 = FormControlHelper.GetDecimalValue(row1.Cells["SecCost"].Value.ToString().Trim());

            decimal totalWeight = weight0 + weight1;

            if (selectedParcelOrStone == "Stone")
            {
                // === Phase 1: Apply Field Dependencies ===
                switch (lastEditedColumn)
                {
                    case "List":
                    case "ListCostDiscount":
                        if (list1 != 0)
                        {
                            cost1 = Math.Round(list1 * (1 + discount1 / 100), 2);
                            row1.Cells["Cost"].Value = cost1;
                        }
                        break;

                    case "SecCost":
                        cost1 = Math.Round(secCost1 * USDRate, 2);
                        row1.Cells["Cost"].Value = cost1;
                        discount1 = Math.Round((cost1 / list1 - 1) * 100, 2);
                        row1.Cells["ListCostDiscount"].Value = discount1;
                        break;

                    case "Cost":
                        if (list1 != 0)
                        {
                            discount1 = Math.Round((cost1 / list1 - 1) * 100, 2);
                            row1.Cells["ListCostDiscount"].Value = discount1;
                        }
                        break;
                }

            }
            else if (selectedParcelOrStone == "Parcel")
            {
                switch (lastEditedColumn)
                {
                    case "SecCost":
                        cost1 = Math.Round(secCost1 * USDRate, 2);
                        row1.Cells["Cost"].Value = cost1;
                        break;
                }
            }

            // === Phase 2: Recalculate All Dependent Fields ===

            // Refresh updated cost/weight values
            cost1 = FormControlHelper.GetDecimalValue(row1.Cells["Cost"].Value.ToString().Trim());
            weight1 = FormControlHelper.GetDecimalValue(row1.Cells["Weight"].Value.ToString().Trim());
            decimal quantity0 = FormControlHelper.GetDecimalValue(row0.Cells["Quantity"].Value.ToString().Trim());
            decimal quantity1 = FormControlHelper.GetDecimalValue(row1.Cells["Quantity"].Value.ToString().Trim());
            totalWeight = weight0 + weight1;

            // Row 1
            decimal totalCost1 = cost1 * weight1;
            row1.Cells["TotalCost"].Value = Math.Round(totalCost1, 2);
            row1.Cells["SecCost"].Value = Math.Round(cost1 * GBPRate, 2);
            row1.Cells["SecTotalCost"].Value = Math.Round(totalCost1 * GBPRate, 2);

            // Row 2
            if (selectedParcelOrStone == "Parcel")
            {
                // Weighted average cost
                decimal averageCost = 0;
                if (totalWeight > 0)
                    averageCost = (cost0 * weight0 + cost1 * weight1) / totalWeight;

                row2.Cells["Weight"].Value = totalWeight;
                row2.Cells["Cost"].Value = Math.Round(averageCost, 2);

                decimal totalCost2 = averageCost * totalWeight;
                row2.Cells["TotalCost"].Value = Math.Round(totalCost2, 2);
                row2.Cells["SecCost"].Value = Math.Round(averageCost * GBPRate, 2);
                row2.Cells["SecTotalCost"].Value = Math.Round(totalCost2 * GBPRate, 2);
            }
            else if (selectedParcelOrStone == "Stone")
            {
                // Copy all values from row1
                row2.Cells["Quantity"].Value = quantity0 + quantity1;
                row2.Cells["Weight"].Value = weight0 + weight1;
                row2.Cells["List"].Value = list1;
                row2.Cells["ListCostDiscount"].Value = discount1;
                row2.Cells["Cost"].Value = cost1;
                row2.Cells["TotalCost"].Value = Math.Round(totalCost1, 2);
                row2.Cells["SecCost"].Value = Math.Round(cost1 * GBPRate, 2);
                row2.Cells["SecTotalCost"].Value = Math.Round(totalCost1 * GBPRate, 2);

            }

        }

        //NotInt Use----------------------------------------------------------------------------
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
                else
                {
                    dgv_WorkingLine.Rows[2].Cells["Cost"].Value = 0;
                }

            }
        }

        private void UpdateAllCostRow1(DataGridViewCellEventArgs e)
        {
            // update cost, total cost when cost, weight, cost(£) is changed
            decimal cost0 = 0, weight0 = 0, cost1 = 0, weight1 = 0;
            var row0 = dgv_WorkingLine.Rows[0];
            var row1 = dgv_WorkingLine.Rows[1];
            var row2 = dgv_WorkingLine.Rows[2];

            if (dgv_WorkingLine.Rows.Count >= 3)
            {
                if (e.RowIndex == 1)
                {

                    if (e.ColumnIndex == dgv_WorkingLine.Columns["Cost"].Index || e.ColumnIndex == dgv_WorkingLine.Columns["Weight"].Index || e.ColumnIndex == dgv_WorkingLine.Columns["ListCostDiscount"].Index)
                    {

                        cost0 = FormControlHelper.GetDecimalValue(row0.Cells["Cost"].Value.ToString().Trim());
                        weight0 = FormControlHelper.GetDecimalValue(row0.Cells["Weight"].Value.ToString().Trim());
                        cost1 = FormControlHelper.GetDecimalValue(row1.Cells["Cost"].Value.ToString().Trim());
                        weight1 = FormControlHelper.GetDecimalValue(row1.Cells["Weight"].Value.ToString().Trim());

                        decimal totalcostRow1 = cost1 * weight1;

                        row1.Cells["TotalCost"].Value = totalcostRow1;
                        row1.Cells["SecCost"].Value = Math.Round(cost1 * GBPRate, 2);
                        row1.Cells["SecTotalCost"].Value = Math.Round(totalcostRow1 * GBPRate, 2);

                        //Average Parcel Cost
                        decimal totalWeight = weight0 + weight1;
                        if (totalWeight > 0)
                        {
                            decimal costRow2 = Math.Round((cost0 * weight0 + cost1 * weight1) / (totalWeight), 2);
                            row2.Cells["Cost"].Value = costRow2;
                        }
                        else
                        {
                            row2.Cells["Cost"].Value = 0;
                        }
                    }
                    else if (e.ColumnIndex == dgv_WorkingLine.Columns["SecCost"].Index)
                    {
                        decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["SecCost"].Value?.ToString(), out cost1);
                        decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["Weight"].Value?.ToString(), out weight1);
                        decimal totalcostRow1 = cost1 * weight1;

                        row1.Cells["SecTotalCost"].Value = totalcostRow1;
                        row1.Cells["Cost"].Value = Math.Round(cost1 * USDRate, 2);
                        row1.Cells["TotalCost"].Value = Math.Round(totalcostRow1 * USDRate, 2);

                    }


                }


            }

        }
        //NotInt Use----------------------------------------------------------------------------

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
        private void UpdateLotAndItemFields()
        {
            string selectedParcelOrStone = cb_ParcelOrStone.SelectedItem.ToString() ?? "";
            string selectedHoldingType = cb_HoldingType.SelectedItem.ToString() ?? "";

            int newLotID = _diamondLotMaxID.MaxLotID + 1;
            int newItemID = _diamondLotMaxID.MaxItemID + 1;

            if (selectedParcelOrStone == "Parcel" && selectedHoldingType == "Permanent")
            {
                if (ck_NewLot.Checked)
                {
                    if (cb_LotName.Items.Contains(cb_LotName.Text))
                    {
                        MessageBox.Show("LotName already existed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cb_LotName.Text = string.Empty;                    
                    }
                    tb_LotID.Text = newItemID.ToString();
                    tb_ItemID.Text = newItemID.ToString();
                    InitializeWorkingLineGridValues(dgv_WorkingLine);
                    dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newItemID;
                    dgv_WorkingLine.Rows[1].Cells["LotID"].Value = newItemID;
                    dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newItemID;
                    _maxItemID = newItemID;
                }
                else
                    InitializeWorkingLineGridValues(dgv_WorkingLine);

            }
            else if (selectedParcelOrStone == "Stone")
            {
                tb_LotID.Text = newLotID.ToString();
                tb_ItemID.Text = "100";
                cb_HoldingType.SelectedItem = "Temporary"; // Default to Temporary for Stone
                InitializeWorkingLineGridValues(dgv_WorkingLine);
                dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newLotID;
                dgv_WorkingLine.Rows[1].Cells["LotID"].Value = newLotID;
                dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newLotID;
                _maxLotID = newLotID;

            }
            else
            {
                tb_LotID.Text = newLotID.ToString();
                tb_ItemID.Text = newLotID.ToString();
                InitializeWorkingLineGridValues(dgv_WorkingLine);
                dgv_WorkingLine.Rows[0].Cells["LotID"].Value = newLotID;
                dgv_WorkingLine.Rows[1].Cells["LotID"].Value = newLotID;
                dgv_WorkingLine.Rows[2].Cells["LotID"].Value = newLotID;
                _maxLotID = newLotID;
            }

        }

        private void ck_NewLotChecked() 
        {
          
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

            UpdateLotAndItemFields();

        }

        private void ck_NewLotUnchecked()
        {
            //Unchecked happend only when user wants to purchase Permanent or temporary parcel
            tb_LotID.Text = "0";
            tb_ItemID.Text = "0";
            dgv_WorkingLine.Rows[0].Cells["LotID"].Value = 0;
            dgv_WorkingLine.Rows[1].Cells["LotID"].Value = 0;
            dgv_WorkingLine.Rows[2].Cells["LotID"].Value = 0;
        }

        private void cb_ParcelOrStone_SelectedIndexChanged(object sender, EventArgs e)
        {

            string selectedParcelOrStone = cb_ParcelOrStone.SelectedItem?.ToString() ?? "";
            string selectedLotName = cb_LotName.SelectedItem?.ToString() ?? "";
            string selectedHoldingType = string.Empty;
            if (cb_HoldingType.Text.Trim() != "")
            {
                selectedHoldingType = cb_HoldingType.SelectedItem.ToString() ?? "";
            }

            if (_docTypeID == 1006)
            {
                if (selectedParcelOrStone == "Stone")
                {
                    cb_HoldingType.SelectedItem = "Temporary";
                    cb_LotName.SelectedItem = "Cert";
                    ck_NewLot.Checked = true;

                }
                else if (selectedParcelOrStone == "Parcel")
                {
                    cb_HoldingType.SelectedItem = "Permanent";
                    cb_LotName.SelectedIndex = -1; // Clear LotName selection
                    ck_NewLot.Checked = false;
                }
                UpdateLotAndItemFields();
            }
        }
        private void cb_HoldingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (_isProgrammaticChange) return;
            if (cb_ParcelOrStone.SelectedIndex == -1)
            {
                ck_NewLot.Checked = false;
                MessageBox.Show("Please select Parcel or Stone", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cb_ParcelOrStone.Focus();
                return;
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



        private void bt_AddCertificate_Click(object sender, EventArgs e)
        {
            CertificateForm certificateForm = new CertificateForm(_docTypeID, _certificate);
            certificateForm.SaveCertificate += AssignCertificate;
            certificateForm.Show();
        }

        private void AssignCertificate(object sender, SaveCertificateEventArgs e) { 
            _certificate = e.Certificate;
            if (!string.IsNullOrEmpty(_certificate.CertificateLabName) || !string.IsNullOrEmpty(_certificate.CertificateNo))
            {
                int newCertificateID = _diamondLotMaxID.MaxCertificateID + 1;

                if (_diamondLotMaxID.IsUpdateMaxCertificateID(newCertificateID))
                { 
                    _certificate.CertificateID = newCertificateID;
                    _diamondLotMaxID.UpdateMaxCertificateID(newCertificateID);
                }

            }
            else 
            {
                MessageBox.Show("Certificate Lab Name or Certificate No is empty.\nNo Certificate added.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            decimal.TryParse(dgv_WorkingLine.Rows[1].Cells["ListCostDiscount"].Value?.ToString(), out costDiscRow1);

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
            
                //decimal listPrice = Convert.ToDecimal(FindDiamondListPrice(shape, size, color, clarity));
                decimal listPrice = FormControlHelper.GetDecimalValue(_rapaportPriceService.GetRapaportListPrice(shape, size, color, clarity));
                
                dgv_WorkingLine.Columns["List"].DefaultCellStyle.Format = "0.00";

                if (dgv_WorkingLine.Rows.Count >= 3)
                { 
                    dgv_WorkingLine.Rows[1].Cells["List"].Value = listPrice;
                    if(costRow1 == 0)
                        dgv_WorkingLine.Rows[1].Cells["ListCostDiscount"].Value = -100;
                    else
                        dgv_WorkingLine.Rows[1].Cells["ListCostDiscount"].Value = Math.Round((costRow1 / listPrice - 1) * 100, 2);
                    
                    lastEditedColumn = "List";
                    RecalculateTransactionRows();  

                }
                       
            }


        }



    }
}
