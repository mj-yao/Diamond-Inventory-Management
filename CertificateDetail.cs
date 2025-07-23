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
using System.Text.Json;
using System.Net;
using System.Globalization; //for parsing datetime
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using DiamondTransaction.DataAccess;
using DiamondTransaction.Models;
using DiamondTransaction.UI.Utilities;



namespace DiamondTransaction
{
    
    public partial class CertificateDetail : UserControl
    {

        public static string User = "FLEUR";
        public string connectionString = @"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql";
        private ControlNameAndTextManager CertControlListManager = new ControlNameAndTextManager();
        public bool IsCertControlNameAndTextsAdded = false;
        public bool IsRefreshCert, IsRefreshParcel, IsNewParcel, IsNewCert, IsParcelEditing, IsCertEditing;
        AutoCompleteStringCollection certificateTypeAutoComplete = new AutoCompleteStringCollection();
        List<string> certTypeDesc = new List<string>();
        public bool IsNewCertificate { get; set; }
        public CertificateData _certificateData = new CertificateData();

        public CertificateDetail()
        {
            InitializeComponent();
            InitialiseLabNameComboBox();
            InitialiseCertComboBox("Polish", String.Empty);
            InitialiseCertComboBox("Symmetry", String.Empty);
            InitialiseCertComboBox("Fluorescence", String.Empty);
            InitialiseCertComboBox("Girdle", String.Empty);
            InitialiseCertComboBox("GirdleCondition", String.Empty);
            InitialiseCertComboBox("Culet", String.Empty);


            if (IsNewCertificate)
            {

                CreateBlankCertificate();
            }
            else
            {

                LoadCertificateData();
            }

                
            CertControlListManager = new ControlNameAndTextManager();
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Report);
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Grading);
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Proportions);

            IsCertControlNameAndTextsAdded = true;
        }


        private void CreateBlankCertificate()
        {

        }

        private void LoadCertificateData()
        {

        }


        private void cb_CertLabName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string labname = cb_CertLabName.Text;
            InitialiseCertComboBox("Shape", labname);
            InitialiseCertComboBox("ShapeDesc", labname);
            InitialiseCertComboBox("Color", labname);
            InitialiseCertComboBox("Size", labname);
            InitialiseCertComboBox("Clarity", labname);
            InitialiseCertComboBox("Cut", labname);         
            InitialiseCertComboBox("CertificateType", labname);
        }



        private void InitialiseCertComboBox(string grading, string labname)
        {

            string errorMessageTitle = "Access " + grading;

            string query = "";
            if (grading == "Shape" || grading == "Color" || grading == "Size" || grading == "Clarity" || grading == "Cut")
                query = "SELECT " + grading + " FROM THXADiamond" + grading + " WHERE LabAccountName = '" + labname + "' ORDER BY " + grading + "Order";
            else if (grading == "ShapeDesc")
            {
                query = "SELECT " + grading + " FROM THXADiamondShape WHERE LabAccountName = '" + labname + "' ORDER BY ShapeOrder ";
            }
            else if (grading == "CertificateType")
                query = "SELECT CertificateType, CertificateTypeDesc FROM THXADiamondLabCertificate WHERE LabAccountName = '" + labname + "' ORDER BY " + grading + "Order";
            else
                query = "SELECT " + grading + " FROM THXADiamond" + grading + " ORDER BY " + grading + "Order";


            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                List<string> gradeList = new List<string>();
                gradeList = (from item in sqlResult.AsEnumerable()
                             select item.Field<string>(grading)).ToList();

                ComboBox comboBox = this.Controls.Find("cb_Cert" + grading, true).FirstOrDefault() as ComboBox;
                comboBox.Items.Clear();
                for (int i = 0; i < gradeList.Count; i++)
                    comboBox.Items.Add(gradeList[i]);

                if (grading == "CertificateType")
                {
                    certTypeDesc = new List<string>();
                    certTypeDesc = (from item in sqlResult.AsEnumerable()
                                    select item.Field<string>("CertificateTypeDesc")).ToList();


                    certificateTypeAutoComplete = new AutoCompleteStringCollection();
                    certificateTypeAutoComplete.AddRange(gradeList.ToArray());
                    cb_CertCertificateType.AutoCompleteCustomSource = certificateTypeAutoComplete;

                }

            }
        }

        private void InitialiseLabNameComboBox()
        {

            string errorMessageTitle = "Access LabName Option ";
            string query = "SELECT DISTINCT CertificateLabName FROM THXADiamondCertificate WHERE CertificateLabName != '' ";

            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                List<string> labNameList = new List<string>();
                labNameList = (from item in sqlResult.AsEnumerable()
                               select item.Field<string>("CertificateLabName")).ToList();


                ComboBox comboBox = cb_CertLabName;
                comboBox.Items.Clear();
                for (int i = 0; i < labNameList.Count; i++)
                    comboBox.Items.Add(labNameList[i]);
            }
        }

        private void FillCertShapeDesc()
        {
            string labname = cb_CertLabName.Text;
            string shape = cb_CertShape.Text;

            string errorMessageTitle = "Access THXADiamondShape for ShapeDesc ";
            string query = "SELECT ShapeDesc FROM THXADiamondShape WHERE LabAccountName = '" + labname + "' and Shape  = '" + shape + "'ORDER BY ShapeOrder ";

            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult( connectionString, query, errorMessageTitle);
            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                cb_CertShapeDesc.Text = sqlResult.Rows[0]["ShapeDesc"].ToString();
                cb_CertShapeDesc.Focus();

            }
        }

        private void cb_CertShapeDesc_DropDown(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox != null)
            {
                using (Graphics g = comboBox.CreateGraphics())
                {
                    int maxWidth = comboBox.Width;
                    foreach (var item in comboBox.Items)
                    {
                        int itemWidth = (int)g.MeasureString(item.ToString(), comboBox.Font).Width;
                        if (itemWidth > maxWidth)
                        {
                            maxWidth = itemWidth;
                        }
                    }

                    comboBox.DropDownWidth = maxWidth + SystemInformation.VerticalScrollBarWidth; // Add space for scrollbar
                }
            }
        }

        private void FillCertTypeDesc(object sender, EventArgs e)
        {
            ComboBox certType = (ComboBox)sender;
            int codeIndex = certificateTypeAutoComplete.IndexOf(certType.Text);
            if (codeIndex >= 0)
            {
                tb_CertCertificateTypeDesc.Text = certTypeDesc[codeIndex];
            }
            else if (certType.Text.Trim() == string.Empty)
            {
                tb_CertCertificateTypeDesc.Text = string.Empty;
            }

        }


        private void CertComboBox_ValidatedToUpperByEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ComboBox EditedComboBox = (ComboBox)sender;
                this.ActiveControl = null;
                EditedComboBox.Text = EditedComboBox.Text.ToUpper();
                UpdateCertValidation(EditedComboBox, bt_Update);

            }
        }


        private void CertComboBox_ValidatedToUpper(object sender, EventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBox EditedComboBox = (ComboBox)sender;
                EditedComboBox.Text = EditedComboBox.Text.ToUpper();
                UpdateCertValidation(EditedComboBox, bt_Update);

                if (EditedComboBox == cb_CertShape)
                    FillCertShapeDesc();

            }
        }

        private void CertComboBox_ValidatedByEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ComboBox EditedComboBox = (ComboBox)sender;
                this.ActiveControl = null;
                UpdateCertValidation(EditedComboBox, bt_Update);

            }
        }
        private void CertComboBox_Validated(object sender, EventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBox EditedComboBox = (ComboBox)sender;
                UpdateCertValidation(EditedComboBox, bt_Update);
            }
            else if (sender is DateTimePicker)
            {
                DateTimePicker dateTimePicker = (DateTimePicker)sender;
                UpdateCertValidation(dateTimePicker, bt_Update);

            }
        }
        private void cb_CertLabName_ValidatedByEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ComboBox EditedComboBox = (ComboBox)sender;
                this.ActiveControl = null;
                cb_CertLabName_Validated(EditedComboBox, null);

            }
        }
        private void cb_CertLabName_Validated(object sender, EventArgs e)
        {

            ComboBox LabNameTextBox = (ComboBox)sender;
            LabNameTextBox.Text = LabNameTextBox.Text.ToUpper();
            string labname = cb_CertLabName.Text;

            InitialiseCertComboBox("Shape", labname);
            InitialiseCertComboBox("ShapeDesc", labname);
            InitialiseCertComboBox("Color", labname);
            InitialiseCertComboBox("Size", labname);
            InitialiseCertComboBox("Clarity", labname);
            InitialiseCertComboBox("Cut", labname);
            InitialiseCertComboBox("CertificateType", labname);

            UpdateCertValidation(LabNameTextBox, bt_Update);
        }



        private void UpdateCertValidation(Control editedControl, Control updateButton)
        {
            CertControlListManager.UpdateTextInEditedControlList(editedControl);
            CertControlListManager.SetEditedControlBackColor(editedControl);

            if (!IsNewCertificate && IsAuthorisedUser() && CertControlListManager.IsAnyControlTextChanged())
            {
                SetUpdateButtonOn(updateButton, Color.LightGreen);
                IsCertEditing = true;
            }
            else
            {
                SetUpdateButtonOff(updateButton, SystemColors.Control);
            }
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
        public static void SetUpdateButtonOn(Control updateBtn, Color color)
        {
            updateBtn.Enabled = true;
            SetControlBackColor(updateBtn, color);
        }
        public static void SetUpdateButtonOff(Control updateBtn, Color color)
        {
            updateBtn.Enabled = false;
            SetControlBackColor(updateBtn, color);
        }
        public static void SetControlBackColor(Control control, Color color)
        {
            control.BackColor = color;
        }

        private void setDecimalPlaceTo0(object sender, EventArgs e)
        {
            setDecimalPlacesFormat(sender, 0);
        }

        private void setDecimalPlaceTo1(object sender, EventArgs e)
        {
            setDecimalPlacesFormat(sender, 1);
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
                SqlQueryExecutor.ShowErrorMessage(e1, "Text Parse to Double fails");
            }
        }

        private double GetDoubleValue(string text)
        {
            return string.IsNullOrEmpty(text) ? 0 : Convert.ToDouble(text);
        }



        private double GetIntValue(string text)
        {
            return string.IsNullOrEmpty(text) ? 0 : Convert.ToInt32(text);
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

        private void TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = (TextBox)sender;
                tb.ReadOnly = false;
                tb.BackColor = System.Drawing.Color.Yellow;
            }
        }

        private void CertTextBox_ValidatedByEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox tb = (TextBox)sender;
                this.ActiveControl = null;
                CertTextBox_Validated(tb, e);
            }
        }
        private void CertTextBox_Validated(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox EditedTextBox = (TextBox)sender;
                UpdateCertValidation(EditedTextBox, bt_Update);

            }
        }

        private void FillEmptyCertDetailAndControlList()
        {

            IsCertControlNameAndTextsAdded = false;
            CertControlListManager = new ControlNameAndTextManager();
            CertControlListManager.setControlTextEmptyInPanel(pnl_Report);
            CertControlListManager.setControlTextEmptyInPanel(pnl_Grading);
            CertControlListManager.setControlTextEmptyInPanel(pnl_Proportions);


            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Report);
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Grading);
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Proportions);

            IsCertControlNameAndTextsAdded = true;

            bt_Update.BackColor = SystemColors.Control;
            bt_Update.Enabled = false;

        }

        public void UpdateCertificateData() { 
        
            _certificateData.CertificateLabName = cb_CertLabName.Text.Trim();
            _certificateData.CertificateType = cb_CertCertificateType.Text.Trim();
            _certificateData.CertificateTypeDesc = tb_CertCertificateTypeDesc.Text.Trim();
            _certificateData.CertificateNo = tb_CertNo.Text.Trim();
            _certificateData.CertificateDate = dateTime_CertDate.Value;
            _certificateData.Shape = cb_CertShapeDesc.Text.Trim();
            _certificateData.Size = cb_CertSize.Text.Trim();
            _certificateData.Color = cb_CertColor.Text.Trim();
            _certificateData.Clarity = cb_CertClarity.Text.Trim();
            _certificateData.Measurements = tb_Measurements.Text.Trim();
            _certificateData.Weight = FormControlHelper.GetDecimalValue(tb_CertWeight.Text);
            _certificateData.Depth = FormControlHelper.GetDecimalValue(tb_Depth.Text);
            _certificateData.StoneTable = FormControlHelper.GetDecimalValue(tb_StoneTable.Text);
            _certificateData.GirdleMinMax = tb_GirdleMinMax.Text.Trim();
            _certificateData.GirdleCondition = cb_CertGirdleCondition.Text.Trim();
            _certificateData.Girdle = cb_CertGirdle.Text.Trim();
            _certificateData.Culet = cb_CertCulet.Text.Trim();
            _certificateData.Polish = cb_CertPolish.Text.Trim();
            _certificateData.Symmetry = cb_CertSymmetry.Text.Trim();
            _certificateData.Fluorescence = cb_CertFluorescence.Text.Trim();
            _certificateData.Cut = cb_CertCut.Text.Trim();
            _certificateData.CrownAngle = FormControlHelper.GetDecimalValue(tb_CrownAngle.Text);
            _certificateData.CrownHeight = FormControlHelper.GetDecimalValue(tb_CrownHeight.Text);
            _certificateData.PavillionAngle = FormControlHelper.GetDecimalValue(tb_PavillionAngle.Text);
            _certificateData.PavillionDepth = FormControlHelper.GetDecimalValue(tb_PavillionDepth.Text);
            _certificateData.StarLength = FormControlHelper.GetDecimalValue(tb_StarLength.Text);
            _certificateData.LowerHalf = FormControlHelper.GetDecimalValue(tb_LowerHalf.Text);
            _certificateData.Inscription = tb_CertInscription.Text.Trim();
            _certificateData.LabComment = tb_LabComment.Text.Trim();
        
        }



        // Get Certificate

        public void bt_Update_Click(object sender, EventArgs e)
        {
            //_certificateData.CertificateNo = "1234"; //2141438178


        }

        //--------- Get Certificate Detail

        private async void bt_FetchCertificate_Click(object sender, EventArgs e)
        {
            if (cb_CertLabName.Text != "GIA" && cb_CertLabName.Text != "IGI" && cb_CertLabName.Text != "HRD")
            {
                MessageBox.Show("The certificate lab is not valid. Please choose certificate lab.", "Invalid Lab", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cb_CertLabName.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(tb_CertNo.Text))
            {
                MessageBox.Show("The certificate number is empty. Please enter a valid certificate number.", "Empty Certificate Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb_CertNo.Focus();
                return;
            }


            if (cb_CertLabName.Text == "GIA")
            {
                string[] certificateNumber = new string[] { tb_CertNo.Text };
                try
                {
                    bool success = await GetGIACertificate(certificateNumber);
                    if (success)
                    {
                        PostProcessCertificateDetail();
                        bt_Update.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while fetching the GIA certificate: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (cb_CertLabName.Text == "IGI")
            {
                string[] certificateNumber = new string[] { tb_CertNo.Text };
                try
                {
                    bool success = await GetIGICertificate(certificateNumber[0]);
                    //await GetIGICertificateWithoutKey(certificateNumber[0]);
                    if (success)
                    {
                        PostProcessCertificateDetail();
                        bt_Update.Enabled = true;

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while fetching the IGI certificate: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }

        async Task<bool> GetIGICertificate(string certificateNumber)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "54f54f52781c4cbeb79f7b5f674a258a");

                    var response = await client.GetAsync($"https://tools.igi.org/apireport/details?r={certificateNumber}");

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"API request failed. Status code: {response.StatusCode}\nError content: {errorContent}",
                                        "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Parse the JSON response
                    var certificateData = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(responseBody);

                    if (certificateData == null || certificateData.Count == 0)
                    {
                        MessageBox.Show("The certificate data is empty.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    //check if the report is not found
                    if (certificateData.Count == 1 && certificateData[0].ContainsKey("Value") && certificateData[0]["Value"].ToString() == "Not Found")
                    {
                        MessageBox.Show("Report is not found.", "Report Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }


                    // Mapping for the second dataset
                    var reportMapping = new Dictionary<string, string>
                    {
                        {"REPORT NUMBER", "tb_CertNo"},
                        {"REPORT DATE", "dateTime_CertDate"},
                        {"DESCRIPTION", "tb_CertCertificateTypeDesc"},
                        {"SHAPE AND CUT", "cb_CertShapeDesc"},
                        {"CARAT WEIGHT", "tb_CertWeight"},
                        {"COLOR GRADE", "cb_CertColor"},
                        {"CLARITY GRADE", "cb_CertClarity"},
                        {"CUT GRADE", "cb_CertCut"},
                        {"Measurements", "tb_Measurements"},
                        {"Table Size", "tb_StoneTable"},
                        {"Crown Height", "tb_CrownHeight"},
                        {"Pavilion Depth", "tb_PavillionDepth"},
                        {"Girdle Thickness", "cb_CertGirdle"},
                        {"Culet", "cb_CertCulet"},
                        {"FLUORESCENCE", "cb_CertFluorescence"},
                        {"POLISH", "cb_CertPolish"},
                        {"SYMMETRY", "cb_CertSymmetry"}
                    };


                    // Mapping for the first dataset
                    var webLabelMapping = new Dictionary<string, string>
                    {
                        {"Number", "tb_CertNo"},
                        {"Igi Report Date", "dateTime_CertDate"},
                        {"Description", "tb_CertCertificateTypeDesc"},
                        {"Shape And Cut", "cb_CertShapeDesc"},
                        {"Carat Weight", "tb_CertWeight"},
                        {"Color Grade", "cb_CertColor"},
                        {"Clarity Grade", "cb_CertClarity"},
                        {"Cut Grade", "cb_CertCut"},
                        {"Measurements", "tb_Measurements"},
                        {"Table", "tb_StoneTable"},
                        {"Crown Height - Angle", "tb_CrownHeight"},
                        {"Pavilion Depth - Angle", "tb_PavillionDepth"},
                        {"Girdle Thickness", "cb_CertGirdle"},
                        {"Culet", "cb_CertCulet"},
                        {"Fluorescence", "cb_CertFluorescence"},
                        {"Comments", "tb_LabComment"},
                        {"Polish", "cb_CertPolish"},
                        {"Symmetry", "cb_CertSymmetry"}
                    };

                    // Only to create a new cert with empty existing cert detail
                    FillEmptyCertDetailAndControlList();

                    cb_CertLabName.Text = "IGI";

                    // Determine dataset format based on the first row
                    var isWebLabelFormat = certificateData[0].ContainsKey("WEB_LABLE");

                    // Select the appropriate mapping
                    var mapping = isWebLabelFormat ? webLabelMapping : reportMapping;

                    // Process all rows
                    foreach (var item in certificateData)
                    {
                        if (isWebLabelFormat)
                        {
                            // First dataset format
                            string webLabel = item["WEB_LABLE"].ToString();
                            string webValue = item["WEB_VALUE"].ToString();

                            if (mapping.TryGetValue(webLabel, out string controlName))
                            {
                                SetControlValue(controlName, webLabel, webValue);
                            }
                        }
                        else
                        {
                            // Second dataset format
                            foreach (var key in reportMapping.Keys)
                            {
                                if (item.ContainsKey(key) && reportMapping.TryGetValue(key, out string controlName))
                                {
                                    string value = item[key]?.ToString() ?? string.Empty;
                                    SetControlValue(controlName, key, value);
                                }
                            }

                        }
                    }
                }
                return true;
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show($"Error fetching IGI certificate: {e.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.Text.Json.JsonException e)
            {
                MessageBox.Show($"Error parsing certificate data: {e.Message}", "JSON Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        // Helper function to set control values
        void SetControlValue(string controlName, string label, string value)
        {
            var control = this.Controls.Find(controlName, true).FirstOrDefault();
            if (label == "Shape And Cut" || label == "SHAPE AND CUT") // Special case
            {
                cb_CertShape.Text = value;
                cb_CertShapeDesc.Text = value;
            }
            else if (control is TextBox textBox)
            {
                textBox.Text = value;
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.Text = value;
            }
            else if (control is DateTimePicker dateTimePicker)
            {
                if (DateTime.TryParse(value, out DateTime parsedDate))
                {
                    dateTimePicker.Value = parsedDate;
                }
                else
                {
                    // Handle invalid date format
                    MessageBox.Show($"Invalid date format for {controlName}: {value}", "Date Format Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }



        public const string DEFAULT_REPORT_NUMBER = "1206489210";
        public const string GRAPHQL_QUERY_FILE = "dotnet/report_results.graphql";

        async Task<bool> GetGIACertificate(string[] args)
        {
            // Get parameters from environmental variables. Do not store secrets in code!
            string url = "https://api.reportresults.gia.edu/";
            string key = "6aa99fe0-c101-40c4-857d-2f8611493965";

            // Confirm that url and key are available
            if (string.IsNullOrEmpty(url) | string.IsNullOrEmpty(key))
            {
                Console.WriteLine("You must provide environment variables REPORT_RESULTS_API_ENDPOINT and REPORT_RESULTS_API_KEY.");
                System.Environment.Exit(1);
            }

            // Set the report number to lookup
            string reportNumber;
            if (args.Length == 0)
            {
                reportNumber = DEFAULT_REPORT_NUMBER;
            }
            else
            {
                reportNumber = args[0];
            }
            Console.WriteLine("Looking up report number: " + reportNumber + "\n");

            // Construct the payload to be POSTed to the graphql server
            var query_variables = new Dictionary<string, string> {
            { "ReportNumber", reportNumber}
        };


            //https://gialaboratory.github.io/report-results/reference/diamondgradingreportresults.doc.html API documentation

            var payload = new Dictionary<string, object> {
            { "query", @"
            query ReportQuery($ReportNumber: String!) {
                getReport(report_number: $ReportNumber){
                    report_number
                    report_date
                    report_type_code
                    report_type
                    results { 
                        __typename
                        ... on DiamondGradingReportResults {
                            shape_and_cutting_style
                            carat_weight
                            clarity_grade
                            color_grade
                            cut_grade
                            polish
                            symmetry
                            fluorescence
                            measurements
                            inscriptions
                            report_comments
                            data    { 
                            shape{ shape_group_code shape_group shape_code }
                            weight{ weight }
                            cut
                            polish
                            symmetry
                            girdle { girdle_condition girdle_pct girdle_size_code }
                            culet { culet_code }
                            fluorescence{ fluorescence_intensity }
                            
                            }
                            proportions{ 
                            depth_pct
                            table_pct
                            crown_angle
                            crown_height
                            pavilion_angle
                            pavilion_depth
                            star_length
                            lower_half
                            }
    
                        }
                    }
                    quota {
                        remaining
                    }
                }
            }"
            },
            { "variables", query_variables }
        };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Convert the payload to JSON
            string json = System.Text.Json.JsonSerializer.Serialize(payload, options);

            // Write the payload to the console
            Console.WriteLine("JSON PAYLOAD TO BE POSTED TO THE SERVER");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine(json + "\n");

            // The results will be saved in this dictionary
            Dictionary<string, string> reportResults = new Dictionary<string, string>();

            using (var client = new HttpClient())
            {
                // Set headers for the api key and content-type
                client.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), key);
                client.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), "application/json");

                HttpResponseMessage response;
                string jsonResponse = "";
                try
                {
                    var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                    // Send the payload as a JSON to the endpoint
                    response = await client.PostAsync(url, content);
                    // read the response
                    jsonResponse = await response.Content.ReadAsStringAsync();
                }
                catch (WebException e)
                {
                    Console.Write("Error accessing " + url + ": ");
                    Console.WriteLine(e.Message);
                    Environment.Exit(1);
                }

                Console.WriteLine("JSON RESPONSE RECEIVED FROM THE API");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine(jsonResponse + "\n");

                // Parse the response (a string) into a JsonDocument so we can
                // traverse the fields
                JsonDocument document = JsonDocument.Parse(jsonResponse);

                // Recursively flatten the JSON document into a dictionary
                flattenJsonDoc(document.RootElement, reportResults);
            }

            // Check for errors in the response
            if (reportResults.ContainsKey("/errors/0/message"))
            {
                string errorType = reportResults.ContainsKey("/errors/0/errorType")
                    ? reportResults["/errors/0/errorType"]
                    : "Error";
                string errorMessage = reportResults["/errors/0/message"];

                MessageBox.Show(errorMessage, errorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            // Write all data to the console
            Console.WriteLine("PARSED REPORT RESULTS");
            Console.WriteLine("---------------------");
            foreach (KeyValuePair<string, string> entry in reportResults)
            {
                Console.WriteLine(entry.Key + ": " + entry.Value);
            }

            var controlMapping = new Dictionary<string, string>
            {
                {"/data/getReport/report_number", "tb_CertNo"},
                {"/data/getReport/report_date", "dateTime_CertDate"},
                {"/data/getReport/report_type_code", "cb_CertCertificateType"},
                {"/data/getReport/report_type", "tb_CertCertificateTypeDesc"},
                {"/data/getReport/results/clarity_grade", "cb_CertClarity"},
                {"/data/getReport/results/color_grade", "cb_CertColor"},
                {"/data/getReport/results/measurements", "tb_Measurements"},
                {"/data/getReport/results/inscriptions", "tb_CertInscription"},
                {"/data/getReport/results/report_comments", "tb_LabComment"},
                {"/data/getReport/results/data/shape/shape_code", "cb_CertShape"},
               // {"/data/getReport/results/data/shape/shape_group_code", "cb_CertShape"},
                {"/data/getReport/results/data/shape/shape_group", "cb_CertShapeDesc"},
                {"/data/getReport/results/data/weight/weight", "tb_CertWeight"},
                {"/data/getReport/results/data/cut", "cb_CertCut"},
                {"/data/getReport/results/data/polish", "cb_CertPolish"},
                {"/data/getReport/results/data/symmetry", "cb_CertSymmetry"},
                {"/data/getReport/results/data/girdle/girdle_condition", "cb_CertGirdleCondition"},
                {"/data/getReport/results/data/girdle/girdle_pct", "tb_GirdleMinMax"},
                {"/data/getReport/results/data/girdle/girdle_size_code", "cb_CertGirdle"},
                {"/data/getReport/results/data/culet/culet_code", "cb_CertCulet"},
                {"/data/getReport/results/data/fluorescence/fluorescence_intensity", "cb_CertFluorescence"},
                {"/data/getReport/results/proportions/depth_pct", "tb_Depth"},
                {"/data/getReport/results/proportions/table_pct", "tb_StoneTable"},
                {"/data/getReport/results/proportions/crown_angle", "tb_CrownAngle"},
                {"/data/getReport/results/proportions/crown_height", "tb_CrownHeight"},
                {"/data/getReport/results/proportions/pavilion_angle", "tb_PavillionAngle"},
                {"/data/getReport/results/proportions/pavilion_depth", "tb_PavillionDepth"},
                {"/data/getReport/results/proportions/star_length", "tb_StarLength"},
                {"/data/getReport/results/proportions/lower_half", "tb_LowerHalf"}
            };

            //Only to create new cert will empty existing cert detail
            FillEmptyCertDetailAndControlList();

            cb_CertLabName.Text = "GIA";

            foreach (KeyValuePair<string, string> entry in reportResults)
            {
                string label = entry.Key;
                string value = entry.Value.ToString();
                if (value != string.Empty && value != "null")
                {
                    if (controlMapping.TryGetValue(label, out string controlName))
                    {
                        var control = this.Controls.Find(controlName, true).FirstOrDefault();
                        if (control is TextBox textBox)
                        {
                            textBox.Text = value;
                        }
                        else if (control is ComboBox comboBox)
                        {
                            comboBox.Text = value;
                        }
                        else if (control is DateTimePicker dateTimePicker)
                        {
                            if (DateTime.TryParse(value, out DateTime parsedDate))
                            {
                                dateTimePicker.Value = parsedDate;
                            }
                            else
                            {
                                // Handle the case where the date string is not in a valid format
                                MessageBox.Show($"Invalid date format for {controlName}: {value}", "Date Format Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            return true;
            void flattenJsonDoc(JsonElement element, Dictionary<string, string> dict, string path = "")
            // This method flattens a JSON document into a dictionary of strings
            // element: The root element of the JSON to be flattened
            // dict: The dictionary to receive the flattened elements
            {
                // JSON has two structures: a collection and an ordered list. All other elements are leaf nodes on the tree.
                switch (element.ValueKind)
                {
                    // A JSON object has properties that must be enumerated. 
                    case JsonValueKind.Object:
                        {
                            foreach (JsonProperty property in element.EnumerateObject())
                            {
                                flattenJsonDoc(property.Value, dict, path + "/" + property.Name);
                            }
                            break;
                        }

                    // A JSON array holds a number of JSON elements
                    case JsonValueKind.Array:
                        {
                            int index = 0;
                            foreach (JsonElement child in element.EnumerateArray())
                            {
                                flattenJsonDoc(child, dict, path + "/" + index);
                                index++;
                            }
                            break;
                        }

                    // The other elements are leaf nodes that can be added to the dictionary with their paths
                    case JsonValueKind.String:
                        {
                            dict.Add(path, element.ToString());
                            break;
                        }
                    case JsonValueKind.Number:
                        {
                            dict.Add(path, element.ToString());
                            break;
                        }
                    case JsonValueKind.Null:
                        {
                            dict.Add(path, "null");
                            break;
                        }
                    case JsonValueKind.False:
                        {
                            dict.Add(path, "false");
                            break;
                        }
                    case JsonValueKind.True:
                        {
                            dict.Add(path, "true");
                            break;
                        }
                    case JsonValueKind.Undefined:
                        {
                            dict.Add(path, "undefined");
                            break;
                        }
                    default:
                        {
                            dict.Add(path, element.ToString());
                            break;
                        }
                }
            }
        }


        private void PostProcessCertificateDetail()
        {
            SetCertWeight();
            SetCertShape();
            SetCertShapeDesc();
            SetCertSize(cb_CertLabName.Text, tb_CertWeight.Text);
            ParseGirdleSizeCode(cb_CertGirdle.Text);
            SetCertificateType(cb_CertLabName.Text, cb_CertCertificateType.Text);
            RemovePercentAndSpaces(cb_CertClarity);
            RemovePercentAndSpaces(tb_StoneTable);
            MappingCertGrading("Shape", cb_CertShape);
            MappingCertGrading("Cut", cb_CertCut);
            MappingCertGrading("Polish", cb_CertPolish);
            MappingCertGrading("Symmetry", cb_CertSymmetry);
            MappingCertGrading("Fluorescence", cb_CertFluorescence);
            MappingCertGrading("Culet", cb_CertCulet);
            ProcessSplitValue(tb_CrownHeight, tb_CrownHeight, tb_CrownAngle);
            ProcessSplitValue(tb_PavillionDepth, tb_PavillionDepth, tb_PavillionAngle);
            ProcessGirdleComboBox();
            MappingCertGrading("Girdle", cb_CertGirdle);
            MappingCertGrading("CertificateType", cb_CertCertificateType);

        }

        private void SetCertWeight()
        {

            string input = tb_CertWeight.Text;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\d+(\.\d+)?");
            var match = regex.Match(input);

            if (match.Success)
            {
                tb_CertWeight.Text = match.Value;
            }
            else
            {
                tb_CertWeight.Text = string.Empty;
            }
        }


        private void SetCertShape()
        {
            string shape = cb_CertShape.Text.ToString();
            if (shape == "RBC" || shape == "R")
            {
                cb_CertShape.Text = "BR";
                cb_CertShapeDesc.Text = "Round";
            }

        }
        private void SetCertShapeDesc()
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string shapeDesc = textInfo.ToTitleCase(cb_CertShapeDesc.Text.ToLower());
            cb_CertShapeDesc.Text = shapeDesc;
        }

        private void SetCertSize(string labName, string weight)
        {
            string size = AccessDiamondSize(labName, weight);
            cb_CertSize.Text = size;
        }
        private string AccessDiamondSize(string labName, string weight)
        {
            try
            {
                string errorMessageTitle = "Access DiamondSize";
                string query = "SELECT LabAccountName,Size,SizeMin,SizeMax FROM THXADiamondSize where LabAccountName = @LabAccountName AND SizeMin <= @size AND  SizeMax >= @size ";

                var parameters = new Dictionary<string, object>
                {
                    { "@LabAccountName", labName },
                    { "@size", weight },
                };

                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(connectionString, query, errorMessageTitle, parameters);
                if (SqlQueryExecutor.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
                {
                    return sqlResult.Rows[0]["Size"].ToString();
                }
            }
            catch (Exception e1)
            {
                SqlQueryExecutor.ShowErrorMessage(e1, "Access Stone Detail failure.");

            }
            return string.Empty;
        }

        private void ParseGirdleSizeCode(string girdleSizeCode)
        {
            if (girdleSizeCode.Length <= 3)
            {
                return; // No need to parse
            }

            string[] parts = girdleSizeCode.Split(new[] { "_TO_" }, StringSplitOptions.None);
            cb_CertGirdle.Text = parts.Length == 2 ? string.Join(" - ", parts) : girdleSizeCode;
        }

        private void SetCertificateType(string labName, string certType)
        {
            if (labName == "GIA")
                cb_CertCertificateType.Text = $"{labName}_{certType}";
            else if (labName == "IGI")
            {

            }
        }

        private void ParseCertificateDate(string inputDate) //Not In Use
        {
            if (string.IsNullOrEmpty(inputDate)) return;

            string[] formatStrings = { "MMMM d, yyyy", "M/d/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy" };
            if (DateTime.TryParseExact(inputDate.Trim(), formatStrings, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                dateTime_CertDate.Value = parsedDate;
                dateTime_CertDate.ForeColor = Color.Black;
            }
            else
            {
                MessageBox.Show("Invalid date format. Please enter the date in a recognized format.", "Date Format Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RemovePercentAndSpaces(Control control)
        {
            if (control != null && !string.IsNullOrEmpty(control.Text))
            {
                control.Text = control.Text.Replace("%", "").Replace(" ", "");
            }
        }

        private void MappingCertGrading(string grading, ComboBox cb_grading)
        {
            try
            {

                string query = "";
                string gradingDesc = grading + "Desc";
                string labName = cb_CertLabName.Text;
                string IGIcertType = tb_CertCertificateTypeDesc.Text;
                if (grading == "Shape" || grading == "Color" || grading == "Size" || grading == "Clarity" || grading == "Cut")
                    query = $"SELECT {grading},{gradingDesc} FROM THXADiamond{grading} where LabAccountName = '{labName}' ";
                else if (grading == "CertificateType")
                    query = $"SELECT {grading},{gradingDesc} FROM THXADiamondLabCertificate WHERE LabAccountName = '{labName}' ";
                else
                    query = $"SELECT {grading},{gradingDesc} FROM THXADiamond{grading} ";

                string errorMessageTitle = "Access Diamond" + grading;


                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
                if (SqlQueryExecutor.IsValid(sqlResult))
                {
                    if (grading == "CertificateType")
                    {
                        foreach (DataRow row in sqlResult.Rows)
                        {
                            string certificateTypeDesc = row["CertificateTypeDesc"].ToString();
                            if (certificateTypeDesc.IndexOf(IGIcertType, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                cb_grading.Text = row["CertificateType"].ToString();
                                tb_CertCertificateTypeDesc.Text = certificateTypeDesc;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow row in sqlResult.Rows)
                        {
                            if (string.Equals(cb_grading.Text, row[gradingDesc].ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                cb_grading.Text = row[grading].ToString();
                                break;
                            }
                        }
                    }


                }
            }
            catch (Exception e1)
            {
                SqlQueryExecutor.ShowErrorMessage(e1, "Access Diamond" + grading + " failure.");

            }
        }


        private void ProcessSplitValue(TextBox sourceTextBox, TextBox targetTextBox1, TextBox targetTextBox2)
        {
            if (sourceTextBox != null && !string.IsNullOrEmpty(sourceTextBox.Text))
            {
                // Split the text into parts using " - " as the delimiter
                var parts = sourceTextBox.Text.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    // Extract the first part and clean it (e.g., remove '%')
                    string firstPart = parts[0].Replace("%", "").Trim();

                    // Extract the second part and clean it (e.g., remove '°')
                    string secondPart = parts[1].Replace("°", "").Trim();

                    // Assign cleaned values to the target textboxes
                    if (targetTextBox1 != null)
                    {
                        targetTextBox1.Text = firstPart;
                    }
                    if (targetTextBox2 != null)
                    {
                        targetTextBox2.Text = secondPart;
                    }
                }
            }
        }

        private void ProcessGirdleComboBox()
        {
            if (cb_CertGirdle != null && !string.IsNullOrEmpty(cb_CertGirdle.Text))
            {
                string girdleText = cb_CertGirdle.Text;

                // Extract the part inside parentheses
                int startIndex = girdleText.IndexOf('(');
                int endIndex = girdleText.IndexOf(')');

                if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
                {
                    string condition = girdleText.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
                    string mainText = girdleText.Substring(0, startIndex).Trim();

                    //parse condition to title case
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    condition = textInfo.ToTitleCase(condition.ToLower());

                    // Set the extracted values to their respective combo boxes
                    cb_CertGirdleCondition.Text = condition;
                    cb_CertGirdle.Text = mainText;
                }
            }
        }


    }
}
