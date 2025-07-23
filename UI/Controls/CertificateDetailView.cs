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
using DiamondTransaction.UseCases.Models;
using DiamondTransaction.UI.Utilities;
using DiamondTransaction.UseCases.Services;
using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.Domain.Entities;




namespace DiamondTransaction
{
    
    public partial class CertificateDetailView : UserControl
    {

        public static string User = "FLEUR";
        public string connectionString = @"server=THLDB1SER\THLSTORE; database=devthlstore;User ID=devthlsqlluser; password=D34vth5ql";
        private ControlNameAndTextManager CertControlListManager = new ControlNameAndTextManager();
        public bool IsCertControlNameAndTextsAdded = false;
        public bool IsRefreshCert, IsRefreshParcel, IsNewParcel, IsNewCert, IsParcelEditing, IsCertEditing;
        AutoCompleteStringCollection certificateTypeAutoComplete = new AutoCompleteStringCollection();
        List<string> certTypeDesc = new List<string>();
        private bool _isProgrammaticChange = false;

        public bool IsNewCertificate { get; set; }
        public CertificateDto _certificate = new CertificateDto();
        private List<CertificateTypeDto> _certificateTypes;
        
        private List<DiamondGrades> _shapeGrades;
        private List<DiamondSizeDto> _sizeGrades;
        private List<DiamondGrades> _colorGrades;
        private List<DiamondGrades> _clarityGrades;        
        private List<DiamondGrades> _symmetryGrades;
        private List<DiamondGrades> _polishGrades;
        private List<DiamondGrades> _culetGrades;
        private List<DiamondGrades> _fluorescenceGrades;
        private List<DiamondGrades> _girdleGrades;
        private List<DiamondGrades> _girdleConditionGrades;
        private List<DiamondGrades> _cutGrades;

        public CertificateService _certificateService;
        public CertificateTypeService _certificateTypeService;
        public DiamondGradingService _diamondGradingService;

        public CertificateDetailView()
        {
            InitializeComponent();

            if (IsNewCertificate)
            {

                CreateBlankCertificate();
            }
            else
            {
                //LoadData();
            }

            CertControlListManager = new ControlNameAndTextManager();
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Report);
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Grading);
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Proportions);
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_CertificateStatus);

            IsCertControlNameAndTextsAdded = true;
        }

        private void CreateBlankCertificate()
        {

        }

        public async Task InitializeDataAndControls()
        {
            await LoadData();
            InitializeControls();
        }

        private async Task LoadData()
        {
            await LoadCertificateTypesAsync();
            await LoadDiamondGradesWithoutLabNameAsync();
        }

        private async Task LoadCertificateTypesAsync()
        {
            _certificateTypes = (await _certificateTypeService.GetAllCertificateTypesAsync()).ToList();
        }


      
        private async Task LoadDiamondGradesWithoutLabNameAsync()
        {
            await LoadAndAssignGradesWithoutLabNameAsync("Symmetry", grades => _symmetryGrades = grades);
            await LoadAndAssignGradesWithoutLabNameAsync("Polish", grades => _polishGrades = grades);
            await LoadAndAssignGradesWithoutLabNameAsync("Culet", grades => _culetGrades = grades);
            await LoadAndAssignGradesWithoutLabNameAsync("Fluorescence", grades => _fluorescenceGrades = grades);
            await LoadAndAssignGradesWithoutLabNameAsync("Girdle", grades => _girdleGrades = grades);
            await LoadAndAssignGradesWithoutLabNameAsync("GirdleCondition", grades => _girdleConditionGrades = grades);
        }

        private async Task LoadAndAssignGradesWithoutLabNameAsync(string gradeType, Action<List<DiamondGrades>> assignAction)
        {
            var grades = await _diamondGradingService.GetGradesByGradeTypeAndLabAsync(gradeType);
            //(?)If grades is null, return null.If grades is not null, convert it to a list. then (??) If grades is null, assign an empty list instead.
            assignAction(grades?.ToList() ?? new List<DiamondGrades>());
        }

        private async Task LoadDiamondGradesWithLabNameAsync(string labName)
        {
            await LoadAndAssignGradesWithLabNameAsync("Shape", labName, grades => _shapeGrades = grades);
            await LoadAndAssignGradesWithLabNameAsync("Color", labName, grades => _colorGrades = grades);
            await LoadAndAssignGradesWithLabNameAsync("Clarity", labName, grades => _clarityGrades = grades);
            await LoadAndAssignGradesWithLabNameAsync("Cut", labName, grades => _cutGrades = grades);
            await LoadAndAssignSizesWithLabNameAsync(labName);
        }

        private async Task LoadAndAssignGradesWithLabNameAsync(string gradeType, string labName, Action<List<DiamondGrades>> assignAction)
        {
            var grades = await _diamondGradingService.GetGradesByGradeTypeAndLabAsync(gradeType,labName);
            //(?)If grades is null, return null.If grades is not null, convert it to a list. then (??) If grades is null, assign an empty list instead.
            assignAction(grades?.ToList() ?? new List<DiamondGrades>());
        }

        private async Task LoadAndAssignSizesWithLabNameAsync(string labName)
        {
            _sizeGrades = (await _diamondGradingService.GetDiamondSizesAsync(labName)).ToList();
        }


        public void SetCertificateDto(CertificateDto certificate)
        {
            _certificate = certificate;
            PopulateCertificateData(certificate);
        }


        private void InitializeControls()
        {        
            InitialiseLabNameComboBox(cb_CertLabName);
            InitialiseGradingComboBox(cb_CertPolish, _polishGrades);
            InitialiseGradingComboBox(cb_CertSymmetry, _symmetryGrades);
            InitialiseGradingComboBox(cb_CertFluorescence, _fluorescenceGrades);
            InitialiseGradingComboBox(cb_CertGirdle,_girdleGrades);
            InitialiseGradingComboBox(cb_CertGirdleCondition, _girdleConditionGrades);
            InitialiseGradingComboBox(cb_CertCulet, _culetGrades);
            
        }

        public async Task InitializeLabBasedControls(string labName) 
        {
            if (labName == string.Empty)
                return;
            await LoadDiamondGradesWithLabNameAsync(labName);
            InitialiseGradingComboBox(cb_CertShape, _shapeGrades);
            InitialiseGradingDescComboBox(cb_CertShapeDesc, _shapeGrades);
            InitialiseSizeComboBox(cb_CertSize, _sizeGrades);
            InitialiseGradingComboBox(cb_CertColor, _colorGrades);
            InitialiseGradingComboBox(cb_CertClarity, _clarityGrades);
            InitialiseGradingComboBox(cb_CertCut, _cutGrades);                      
            InitialiseCertificateTypeComboBoxByLabName(cb_CertCertificateType,labName);       
        }

        private async void cb_CertLabName_SelectedIndexChanged(object sender, EventArgs e)
        {
            await InitializeLabBasedControls(cb_CertLabName.Text);
        }

        private void InitialiseCertificateTypeComboBoxByLabName(ComboBox cb_certType,string selectedLabName)
        { 

            if (cb_certType == null || string.IsNullOrWhiteSpace(selectedLabName)) return;
            var certificateTypes = GetCertificateTypesByLabName(selectedLabName);
            FormControlHelper.SetComboBoxItem(cb_certType, certificateTypes);

        }

        private List<string> GetCertificateTypesByLabName(string labName)
        {
            return _certificateTypes
                .Where(c => c.LabAccountName.Equals(labName, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.CertificateType)
                .Distinct() 
                .ToList();
        }


        private void InitialiseLabNameComboBox(ComboBox cb_labName)
        {
            cb_labName.Items.Clear();
            var distinctLabs = _certificateTypes
                .Where(c => !string.IsNullOrWhiteSpace(c.LabAccountName))
                .GroupBy(c => c.LabAccountName) 
                .Select(group => group.First()) 
                .OrderBy(c => c.LabAccountID) 
                .ToList();

            foreach (var lab in distinctLabs)
            {
                cb_labName.Items.Add(lab.LabAccountName);
            }
        }
   

        private void InitialiseGradingComboBox(ComboBox cb_grading, List<DiamondGrades> gradesList)
        {
            var gradingScale = gradesList.Select(g => g.Grade).ToList();
            FormControlHelper.SetComboBoxItem(cb_grading, gradingScale);
        }
        private void InitialiseSizeComboBox(ComboBox cb_grading, List<DiamondSizeDto> gradesList)
        {
            var gradingScale = gradesList.Select(g => g.Size).ToList();
            FormControlHelper.SetComboBoxItem(cb_grading, gradingScale);
        }


        private void InitialiseGradingDescComboBox(ComboBox cb_grading, List<DiamondGrades> gradesList)
        {
            var desc = gradesList.Select(g => g.GradeDesc).ToList();
            FormControlHelper.SetComboBoxItem(cb_grading, desc);
        }




        private void FillCertTypeDesc(object sender, EventArgs e)
        {
            ComboBox certType = (ComboBox)sender;
            string selectedText = certType.Text;

            var match = _certificateTypes.FirstOrDefault(c => c.CertificateType.Equals(selectedText, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                tb_CertCertificateTypeDesc.Text = match.CertificateTypeDesc;
            }
            else
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
                {
                    cb_CertShapeDesc.Text = GetShapeDesc(cb_CertShape.Text);
                    cb_CertShapeDesc.Focus();
                }                 
            }
        }

        private string GetShapeDesc(string shape)
        {
            if (_shapeGrades == null|| shape == "")
                return string.Empty;

            var match = _shapeGrades.FirstOrDefault(g => g.Grade == shape);
            return match?.GradeDesc ?? string.Empty;            
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
        private async void cb_CertLabName_Validated(object sender, EventArgs e)
        {

            ComboBox LabNameTextBox = (ComboBox)sender;
            LabNameTextBox.Text = LabNameTextBox.Text.ToUpper();
            await InitializeLabBasedControls(cb_CertLabName.Text);

            UpdateCertValidation(LabNameTextBox, bt_Update);
        }


        private void UpdateCertValidation(Control editedControl, Control updateButton)
        {
            UpdateCertificateData();
            //CertControlListManager.UpdateTextInEditedControlList(editedControl);
            //CertControlListManager.SetEditedControlBackColor(editedControl);

            if (!IsNewCertificate && IsAuthorisedUser() && CertControlListManager.IsAnyControlTextChanged())
            {
                FormControlHelper.SetUpdateButtonOn(updateButton, Color.LightGreen);
                IsCertEditing = true;
            }
            else
            {
                FormControlHelper.SetUpdateButtonOff(updateButton, SystemColors.Control);
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



        private void setDecimalPlaceTo2(object sender, EventArgs e)
        {
            FormControlHelper.setDecimalPlacesFormat(sender, 2);
        }

        private void setDecimalPlaceTo3(object sender, EventArgs e)
        {
            FormControlHelper.setDecimalPlacesFormat(sender, 3);
        }



        private void TextBox_DoubleClick(object sender, EventArgs e)
        {
            FormControlHelper.EnableTextBoxEditing(sender, e);
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
            CertControlListManager.setControlTextEmptyInPanel(pnl_CertificateStatus);


            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Report);
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Grading);
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_Proportions);
            CertControlListManager.FillInitAndEditedControlListFromPanel(pnl_CertificateStatus);

            IsCertControlNameAndTextsAdded = true;

            bt_Update.BackColor = SystemColors.Control;
            bt_Update.Enabled = false;

        }

        public CertificateDto GetCertificateData()
        {
            //UpdateCertificateData();  
            return _certificate;
        }

        public void UpdateCertificateData() { 
        
            _certificate.CertificateLabName = cb_CertLabName.Text.Trim();
            _certificate.CertificateType = cb_CertCertificateType.Text.Trim();
            _certificate.CertificateTypeDesc = tb_CertCertificateTypeDesc.Text.Trim();
            _certificate.CertificateNo = tb_CertNo.Text.Trim();
            _certificate.CertificateDate = dateTime_CertDate.Value;
            _certificate.Shape = cb_CertShape.Text.Trim();
            _certificate.Size = cb_CertSize.Text.Trim();
            _certificate.Color = cb_CertColor.Text.Trim();
            _certificate.Clarity = cb_CertClarity.Text.Trim();
            _certificate.Measurements = tb_Measurements.Text.Trim();
            _certificate.Weight = FormControlHelper.GetDecimalValue(tb_CertWeight.Text);
            _certificate.Depth = FormControlHelper.GetNullableDecimal(tb_Depth.Text);
            _certificate.StoneTable = FormControlHelper.GetNullableDecimal(tb_StoneTable.Text);
            _certificate.GirdleMinMax = tb_GirdleMinMax.Text.Trim();
            _certificate.GirdleCondition = cb_CertGirdleCondition.Text.Trim();
            _certificate.Girdle = cb_CertGirdle.Text.Trim();
            _certificate.Culet = cb_CertCulet.Text.Trim();
            _certificate.Polish = cb_CertPolish.Text.Trim();
            _certificate.Symmetry = cb_CertSymmetry.Text.Trim();
            _certificate.Fluorescence = cb_CertFluorescence.Text.Trim();
            _certificate.Cut = cb_CertCut.Text.Trim();
            _certificate.CrownAngle = FormControlHelper.GetNullableDecimal(tb_CrownAngle.Text);
            _certificate.CrownHeight = FormControlHelper.GetNullableDecimal(tb_CrownHeight.Text);
            _certificate.PavillionAngle = FormControlHelper.GetNullableDecimal(tb_PavillionAngle.Text);
            _certificate.PavillionDepth = FormControlHelper.GetNullableDecimal(tb_PavillionDepth.Text);
            _certificate.StarLength = FormControlHelper.GetNullableDecimal(tb_StarLength.Text);
            _certificate.LowerHalf = FormControlHelper.GetNullableDecimal(tb_LowerHalf.Text);
            _certificate.Inscription = tb_CertInscription.Text.Trim();
            _certificate.LabComment = tb_LabComment.Text.Trim();
            _certificate.DownloadStatus = "None"; // Default value, can be changed later
            _certificate.Remark = tb_CertRemark.Text.Trim();
            _certificate.Status = cb_CertStatus.Text.Trim(); 

        }

        public void PopulateCertificateData(CertificateDto certificate)
        {
           
            if (certificate == null) return;
            _isProgrammaticChange = true;

            cb_CertLabName.Text = certificate.CertificateLabName ?? "";
            cb_CertCertificateType.Text = certificate.CertificateType ?? "";
            tb_CertCertificateTypeDesc.Text = certificate.CertificateTypeDesc ?? "";
            tb_CertNo.Text = certificate.CertificateNo ?? "";
            dateTime_CertDate.Value = certificate.CertificateDate == DateTime.MinValue ? DateTime.Now : certificate.CertificateDate;
            cb_CertShape.Text = certificate.Shape ?? "";
            cb_CertShapeDesc.Text = GetShapeDesc(certificate.Shape);
            cb_CertSize.Text = certificate.Size ?? "";
            cb_CertColor.Text = certificate.Color ?? "";
            cb_CertClarity.Text = certificate.Clarity ?? "";
            tb_Measurements.Text = certificate.Measurements ?? "";
            tb_CertWeight.Text = certificate.Weight?.ToString() ?? "";
            tb_Depth.Text = certificate.Depth?.ToString() ?? "";
            tb_StoneTable.Text = certificate.StoneTable?.ToString() ?? "";
            tb_GirdleMinMax.Text = certificate.GirdleMinMax ?? "";
            cb_CertGirdleCondition.Text = certificate.GirdleCondition ?? "";
            cb_CertGirdle.Text = certificate.Girdle ?? "";
            cb_CertCulet.Text = certificate.Culet ?? "";
            cb_CertPolish.Text = certificate.Polish ?? "";
            cb_CertSymmetry.Text = certificate.Symmetry ?? "";
            cb_CertFluorescence.Text = certificate.Fluorescence ?? "";
            cb_CertCut.Text = certificate.Cut ?? "";
            tb_CrownAngle.Text = certificate.CrownAngle?.ToString() ?? "";
            tb_CrownHeight.Text = certificate.CrownHeight?.ToString() ?? "";
            tb_PavillionAngle.Text = certificate.PavillionAngle?.ToString() ?? "";
            tb_PavillionDepth.Text = certificate.PavillionDepth?.ToString() ?? "";
            tb_StarLength.Text = certificate.StarLength?.ToString() ?? "";
            tb_LowerHalf.Text = certificate.LowerHalf?.ToString() ?? "";
            tb_CertInscription.Text = certificate.Inscription ?? "";
            tb_LabComment.Text = certificate.LabComment ?? "";
            tb_CertRemark.Text = certificate.Remark ?? "";
            cb_CertStatus.Text = certificate.Status ?? ""; 
            

            _isProgrammaticChange = false;
        }




        //--------- Get Certificate Detail

        private async void bt_FetchCertificate_Click(object sender, EventArgs e)
        {
            bt_FetchCertificate.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try 
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
                            cb_CertStatus.SelectedItem = "ACTIVE";
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
                            cb_CertStatus.SelectedItem = "ACTIVE";
                            bt_Update.Enabled = true;

                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while fetching the IGI certificate: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                bt_FetchCertificate.Enabled = true;
                Cursor = Cursors.Default;
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
                        MessageBox.Show("The certificate certificate is empty.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show($"Error parsing certificate certificate: {e.Message}", "JSON Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


            // Write all certificate to the console
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

            MappingCertGrading(cb_CertShape, _shapeGrades);
            MappingCertGrading(cb_CertCut,_cutGrades);
            MappingCertGrading(cb_CertPolish, _polishGrades);
            MappingCertGrading(cb_CertSymmetry, _symmetryGrades);
            MappingCertGrading(cb_CertFluorescence, _fluorescenceGrades);
            MappingCertGrading(cb_CertCulet, _culetGrades);


            ProcessSplitValue(tb_CrownHeight, tb_CrownHeight, tb_CrownAngle);
            ProcessSplitValue(tb_PavillionDepth, tb_PavillionDepth, tb_PavillionAngle);
            
            ProcessGirdleComboBox();
            MappingCertGrading(cb_CertGirdle,_girdleGrades);
            MappingCertType(cb_CertLabName.Text, cb_CertCertificateType, tb_CertCertificateTypeDesc, _certificateTypes);
            //MappingCertGrading("CertificateType", cb_CertCertificateType);

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
            string size = GetSizeRangeByLabNameAndWeight(labName, weight);
            cb_CertSize.Text = size;
        }

        private string GetSizeRangeByLabNameAndWeight(string labName, string weightStr)
        {
             
            if (_sizeGrades == null || !_sizeGrades.Any())
            {
                MessageBox.Show("Please reload the form to get Diamond size range again.", "DiamondSize is not loaded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return string.Empty; 
            }

            if (decimal.TryParse(weightStr, out decimal weight))
            {
                var match = _sizeGrades.FirstOrDefault(d =>
                    d.LabAccountName == labName &&
                    d.SizeMin <= weight &&
                    d.SizeMax >= weight);

                return match?.Size ?? string.Empty;
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

        private void pnl_banner3_Paint(object sender, PaintEventArgs e)
        {

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



        private void MappingCertGrading(ComboBox cb_grading, List<DiamondGrades> gradesList)
        {
            var match = gradesList.FirstOrDefault(g => g.GradeDesc.Equals(cb_grading.Text, StringComparison.OrdinalIgnoreCase));

            if (match != null)
            {
                cb_grading.Text = match.Grade;
            }            
        }

        private void MappingCertType(string labName, ComboBox cb_certType, TextBox tb_certTypeDesc, List<CertificateTypeDto> certTypeList)
        {
            //Loose match of certificate type on certificate downloaded to certificate type description stored in database

            var match = certTypeList.FirstOrDefault(g => g.LabAccountName.Equals(labName, StringComparison.OrdinalIgnoreCase) && 
            g.CertificateTypeDesc.IndexOf(tb_certTypeDesc.Text, StringComparison.OrdinalIgnoreCase) >= 0);

            if (match != null)
            {
                cb_certType.Text = match.CertificateType;
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
