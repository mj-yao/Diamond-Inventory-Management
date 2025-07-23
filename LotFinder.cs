using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace DiamondTransaction
{
    public partial class LotFinder : Form
    {
        public string connectionString = @"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql";
        public event Action<string> OnLotSelected;
        public LotFinder()
        {           
            InitializeComponent();
            //InitialiseParcelComboBoxGrading("Shape");
            InitialiseParcelComboBoxGrading("Color");
            InitialiseParcelComboBoxGrading("Size");
            InitialiseParcelComboBoxGrading("Clarity");
            AccountDataManager accountManager = new AccountDataManager(connectionString);
            accountManager.InitialiseCustomerComboBox(null, cb_Location);
            accountManager.InitialiseLotNameComboBox(cb_LotName);
            accountManager.InitialiseParcelComboBoxGrading("Shape", cb_Shape);



        }

        private void tHXADiamondLotBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.tHXADiamondLotBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.devTHLStoreDataSet);

        }

        private void LotFinder_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'devTHLStoreDataSet.THXADiamondLot' table. You can move, or remove it, as needed.
            this.tHXADiamondLotTableAdapter.Fill(this.devTHLStoreDataSet.THXADiamondLot);
            dgv_DiamondLot.DataSource = new DataView(this.devTHLStoreDataSet.THXADiamondLot);
            //this.tHXADiamondLotTableAdapter.FillByLot(this.devTHLStoreDataSet.THXADiamondLot);

        }

        private void InitialiseParcelComboBoxGrading(string grading)
        {

            string errorMessageTitle = "Access " + grading;
            string query = "SELECT " + grading + " FROM THXADiamondParcel" + grading + " ORDER BY " + grading + "Order";

            DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
            if (GlobalClass.IsValid(sqlResult))
            {
                List<string> gradeList = new List<string>();
                gradeList = (from item in sqlResult.AsEnumerable()
                             select item.Field<string>(grading)).ToList();

               
                ComboBox comboBox = this.Controls.Find("cb_" + grading, true).FirstOrDefault() as ComboBox;
                if (comboBox != null)
                {
                    comboBox.Items.Clear();
                    for (int i = 0; i < gradeList.Count; i++)
                        comboBox.Items.Add(gradeList[i]);
                }
            }

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
              

                ApplyFilter(filterExpression, filters);

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

        private void ApplyFilter(StringBuilder filterExpression, Dictionary<string, (Control Control, bool IsExactMatch)> filters)
        {
            foreach (var filter in filters)
            {
                string value = GetControlValue(filter.Value.Control);
                if (!string.IsNullOrEmpty(value))
                {
                    if (filterExpression.Length > 0)
                        filterExpression.Append(" AND ");

                    if (!filter.Value.IsExactMatch)
                        filterExpression.Append($"{filter.Key} LIKE '%{value}%'");
                    else
                        filterExpression.Append($"{filter.Key} = '{value}'");
                }
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

        private string GetControlValue(Control control)
        {
            if (control is TextBox textBox)
                return textBox.Text;
            if (control is ComboBox comboBox)
                return comboBox.Text;
            return string.Empty;
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
            GlobalClass.comboBox_DynamicDropDownWidth(sender, e);
        }

        private void ApplyFilterLotOnLeave(object sender, EventArgs e)
        {
            ApplyFilterLot(sender, e);
        }
    }
}
