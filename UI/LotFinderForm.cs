using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiamondTransaction.DataAccess;
using DiamondTransaction.Domain.Entities;
using DiamondTransaction.UseCases.Models;
using DiamondTransaction.UI.Utilities;
using DiamondTransaction.UseCases.Services;


namespace DiamondTransaction
{
    public partial class LotFinderForm : Form
    {
        //public string connectionString = @"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql";
        public event Action<string> OnLotSelected;
        private ParcelGrades _parcelGrades;
        private List<Customer> _customers;
        private List<string> _lotNames;


        //Application dependency
        private DiamondDocService _diamondDocService;
        private CustomerService _customerService;
        private PriceStockHistoryService _priceStockHistoryService;

        public LotFinderForm()
        {           
            InitializeComponent();
            InitializeServices();
            this.Load += LotFinder_Load;
    
        }

        private void InitializeServices()
        {
            var customerRepository = new CustomerDataAccess(SqlQueryExecutor.getConnectionString());
            _customerService = new CustomerService(customerRepository);

            var diamondDocRepository = new DiamondDocDataAccess(SqlQueryExecutor.getConnectionString());
            _diamondDocService = new DiamondDocService(diamondDocRepository);

            var priceStockHistoryRepository = new PriceStockHistoryDataAccess(SqlQueryExecutor.getConnectionString());
            _priceStockHistoryService = new PriceStockHistoryService(priceStockHistoryRepository);
        }

        private async void LotFinder_Load(object sender, EventArgs e)
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

            //This line of code loads data into the 'devTHLStoreDataSet.THXADiamondLot' table.
            this.tHXADiamondLotTableAdapter.Fill(this.devTHLStoreDataSet.THXADiamondLot);
            dgv_DiamondLot.DataSource = new DataView(this.devTHLStoreDataSet.THXADiamondLot);

        }

        private async Task InitializeDataAndControls()
        {
            await LoadData();
            InitialiseControls();
        }

        private async Task LoadData()
        {
            LoadCustomerData();
            LoadParcelGrades();
            await LoadLotNames();
        }
        private void LoadCustomerData()
        {
            _customers = _customerService.GetAllCustomers();
        }
        private void LoadParcelGrades()
        {
            _parcelGrades = _diamondDocService.GetAllParcelGradingScales();
        }

        private async Task LoadLotNames()
        {
            _lotNames = await _priceStockHistoryService.GetExistingLotNamesAsync();
        }

        private void InitialiseControls()
        {
            InitialiseCustomerComboBox(cb_Location);
            InitialiseParcelGradingComboBox(cb_Shape, cb_Size, cb_Color, cb_Clarity);
            InitialiseLotNameComboBox(cb_LotName);           

        }

        private void InitialiseCustomerComboBox(ComboBox cb_accountName)
        {
            List<string> customerNames = _customers.Select(c => c.CustomerName).ToList();            
            FormControlHelper.SetComboBoxItem(cb_accountName, customerNames);
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


        private void tHXADiamondLotBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.tHXADiamondLotBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.devTHLStoreDataSet);

        }

    
        private void ApplyFilterLot(object sender, EventArgs e)
        {
            try
            {
                StringBuilder filterExpression = new StringBuilder();               
                int.TryParse(tb_LotID.Text, out int LotID);
                var filters = new Dictionary<string, (Control, bool)>
                {
                    
                    { "LotID", (tb_LotID, true) },
                    { "LotName", (cb_LotName, false) },
                    { "ParcelOrStone", (cb_ParcelOrStone, false) },
                    { "Shape", (cb_Shape, false) },
                    { "Size", (cb_Size, false) },
                    { "Color", (cb_Color, false) },
                    { "Clarity", (cb_Clarity, false) },
                    { "Status", (cb_LotStatus, false) },
                    { "HoldingType", (cb_HoldingType, false) },
                    { "LocationAccountName", (cb_Location, false) }
                };
              

                FormControlHelper.ApplyFilter(filterExpression, filters);

                DataView dv = dgv_DiamondLot.DataSource as DataView;
                if (dv != null)
                {
                    dv.RowFilter = filterExpression.ToString();
                }
                else
                {
                    MessageBox.Show("Filtering failed: DataSource is not a DataView.");
                }


               // ((DataView)dgv_DiamondLot.DataSource).RowFilter = filterExpression.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        private void ApplyFilterLotOnEnterKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ApplyFilterLot(sender, e);
            }
        }



        private void bt_Search_Click(object sender, EventArgs e)
        {
            ApplyFilterLot(sender, e);
        }

        private void bt_ClearFilter_Click(object sender, EventArgs e)
        {
            tb_LotID.Text = string.Empty;
            cb_LotName.Text = string.Empty;
            cb_Shape.Text = string.Empty;
            cb_Size.Text = string.Empty;
            cb_Color.Text = string.Empty;
            cb_Clarity.Text = string.Empty;
            cb_LotStatus.Text = string.Empty;
            cb_HoldingType.Text = string.Empty;
            cb_Location.Text = string.Empty;
            ApplyFilterLot(sender, e);
        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();       
        }

        private void bt_Confirm_Click(object sender, EventArgs e)
        {
            if (dgv_DiamondLot.SelectedRows.Count > 0)
            {
                string selectedLotID = dgv_DiamondLot.SelectedRows[0].Cells["LotID"].Value.ToString();

                OnLotSelected?.Invoke(selectedLotID);
                this.Close();
            }
        }

        private void cb_LotName_DropDown(object sender, EventArgs e)
        {
            FormControlHelper.comboBox_DynamicDropDownWidth(sender, e);
        }

        private void ApplyFilterLotOnLeave(object sender, EventArgs e)
        {
            ApplyFilterLot(sender, e);
        }
    }
}
