using DiamondTransaction.UI.Utilities;
using System.Drawing;

namespace DiamondTransaction
{
    partial class CertificateDetailView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cb_CertCulet = new System.Windows.Forms.ComboBox();
            this.cb_CertGirdleCondition = new System.Windows.Forms.ComboBox();
            this.cb_CertGirdle = new System.Windows.Forms.ComboBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label63 = new System.Windows.Forms.Label();
            this.tb_CrownAngle = new System.Windows.Forms.TextBox();
            this.label57 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.tb_CrownHeight = new System.Windows.Forms.TextBox();
            this.label59 = new System.Windows.Forms.Label();
            this.tb_PavillionAngle = new System.Windows.Forms.TextBox();
            this.label60 = new System.Windows.Forms.Label();
            this.tb_PavillionDepth = new System.Windows.Forms.TextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.tb_StarLength = new System.Windows.Forms.TextBox();
            this.label64 = new System.Windows.Forms.Label();
            this.tb_LowerHalf = new System.Windows.Forms.TextBox();
            this.tb_LabComment = new System.Windows.Forms.TextBox();
            this.tb_Measurements = new System.Windows.Forms.TextBox();
            this.tb_GirdleMinMax = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.tb_StoneTable = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.tb_Depth = new System.Windows.Forms.TextBox();
            this.label43 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.cb_CertShapeDesc = new System.Windows.Forms.ComboBox();
            this.label66 = new System.Windows.Forms.Label();
            this.cb_CertSize = new System.Windows.Forms.ComboBox();
            this.label27 = new System.Windows.Forms.Label();
            this.tb_CertInscription = new System.Windows.Forms.TextBox();
            this.cb_CertFluorescence = new System.Windows.Forms.ComboBox();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.tb_CertWeight = new System.Windows.Forms.TextBox();
            this.cb_CertSymmetry = new System.Windows.Forms.ComboBox();
            this.label40 = new System.Windows.Forms.Label();
            this.cb_CertPolish = new System.Windows.Forms.ComboBox();
            this.label39 = new System.Windows.Forms.Label();
            this.cb_CertCut = new System.Windows.Forms.ComboBox();
            this.label38 = new System.Windows.Forms.Label();
            this.cb_CertColor = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.cb_CertClarity = new System.Windows.Forms.ComboBox();
            this.cb_CertShape = new System.Windows.Forms.ComboBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.dateTime_CertDate = new System.Windows.Forms.DateTimePicker();
            this.bt_FetchCertificate = new System.Windows.Forms.Button();
            this.label129 = new System.Windows.Forms.Label();
            this.tb_CertCertificateTypeDesc = new System.Windows.Forms.TextBox();
            this.cb_CertLabName = new System.Windows.Forms.ComboBox();
            this.label44 = new System.Windows.Forms.Label();
            this.tb_CertNo = new System.Windows.Forms.TextBox();
            this.label45 = new System.Windows.Forms.Label();
            this.cb_CertCertificateType = new System.Windows.Forms.ComboBox();
            this.label46 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.pnl_Report = new System.Windows.Forms.Panel();
            this.pnl_banner1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnl_Grading = new System.Windows.Forms.Panel();
            this.pnl_banner2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pnl_Proportions = new System.Windows.Forms.Panel();
            this.pnl_banner3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pnl_CertificateStatus = new System.Windows.Forms.Panel();
            this.cb_CertStatus = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_CertRemark = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.bt_Update = new System.Windows.Forms.Button();
            this.pnl_Report.SuspendLayout();
            this.pnl_banner1.SuspendLayout();
            this.pnl_Grading.SuspendLayout();
            this.pnl_banner2.SuspendLayout();
            this.pnl_Proportions.SuspendLayout();
            this.pnl_banner3.SuspendLayout();
            this.pnl_CertificateStatus.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_CertCulet
            // 
            this.cb_CertCulet.FormattingEnabled = true;
            this.cb_CertCulet.Location = new System.Drawing.Point(113, 183);
            this.cb_CertCulet.Name = "cb_CertCulet";
            this.cb_CertCulet.Size = new System.Drawing.Size(94, 21);
            this.cb_CertCulet.TabIndex = 73;
            this.cb_CertCulet.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertCulet.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertCulet.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // cb_CertGirdleCondition
            // 
            this.cb_CertGirdleCondition.FormattingEnabled = true;
            this.cb_CertGirdleCondition.Location = new System.Drawing.Point(113, 156);
            this.cb_CertGirdleCondition.Name = "cb_CertGirdleCondition";
            this.cb_CertGirdleCondition.Size = new System.Drawing.Size(94, 21);
            this.cb_CertGirdleCondition.TabIndex = 72;
            this.cb_CertGirdleCondition.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedByEnter);
            this.cb_CertGirdleCondition.Leave += new System.EventHandler(this.CertComboBox_Validated);
            this.cb_CertGirdleCondition.Validated += new System.EventHandler(this.CertComboBox_Validated);
            // 
            // cb_CertGirdle
            // 
            this.cb_CertGirdle.FormattingEnabled = true;
            this.cb_CertGirdle.Location = new System.Drawing.Point(113, 99);
            this.cb_CertGirdle.Name = "cb_CertGirdle";
            this.cb_CertGirdle.Size = new System.Drawing.Size(94, 21);
            this.cb_CertGirdle.TabIndex = 71;
            this.cb_CertGirdle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertGirdle.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertGirdle.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.BackColor = System.Drawing.Color.Transparent;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label32.Location = new System.Drawing.Point(247, 46);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(78, 15);
            this.label32.TabIndex = 40;
            this.label32.Text = "CrownAngle°";
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label63.Location = new System.Drawing.Point(13, 214);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(82, 15);
            this.label63.TabIndex = 69;
            this.label63.Text = "LabComment";
            // 
            // tb_CrownAngle
            // 
            this.tb_CrownAngle.Location = new System.Drawing.Point(347, 46);
            this.tb_CrownAngle.Name = "tb_CrownAngle";
            this.tb_CrownAngle.ReadOnly = true;
            this.tb_CrownAngle.Size = new System.Drawing.Size(94, 20);
            this.tb_CrownAngle.TabIndex = 38;
            this.tb_CrownAngle.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_CrownAngle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_CrownAngle.Leave += new System.EventHandler(this.setDecimalPlaceTo2);
            this.tb_CrownAngle.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label57.Location = new System.Drawing.Point(13, 186);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(35, 15);
            this.label57.TabIndex = 66;
            this.label57.Text = "Culet";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label58.Location = new System.Drawing.Point(13, 102);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(40, 15);
            this.label58.TabIndex = 65;
            this.label58.Text = "Girdle";
            // 
            // tb_CrownHeight
            // 
            this.tb_CrownHeight.Location = new System.Drawing.Point(347, 74);
            this.tb_CrownHeight.Name = "tb_CrownHeight";
            this.tb_CrownHeight.ReadOnly = true;
            this.tb_CrownHeight.Size = new System.Drawing.Size(94, 20);
            this.tb_CrownHeight.TabIndex = 39;
            this.tb_CrownHeight.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_CrownHeight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_CrownHeight.Leave += new System.EventHandler(this.setDecimalPlaceTo2);
            this.tb_CrownHeight.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label59.Location = new System.Drawing.Point(13, 158);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(92, 15);
            this.label59.TabIndex = 64;
            this.label59.Text = "GirdleCondition";
            // 
            // tb_PavillionAngle
            // 
            this.tb_PavillionAngle.Location = new System.Drawing.Point(347, 102);
            this.tb_PavillionAngle.Name = "tb_PavillionAngle";
            this.tb_PavillionAngle.ReadOnly = true;
            this.tb_PavillionAngle.Size = new System.Drawing.Size(94, 20);
            this.tb_PavillionAngle.TabIndex = 40;
            this.tb_PavillionAngle.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_PavillionAngle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_PavillionAngle.Leave += new System.EventHandler(this.setDecimalPlaceTo2);
            this.tb_PavillionAngle.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label60.Location = new System.Drawing.Point(13, 130);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(96, 15);
            this.label60.TabIndex = 63;
            this.label60.Text = "GirdleMinMax%";
            // 
            // tb_PavillionDepth
            // 
            this.tb_PavillionDepth.Location = new System.Drawing.Point(347, 130);
            this.tb_PavillionDepth.Name = "tb_PavillionDepth";
            this.tb_PavillionDepth.ReadOnly = true;
            this.tb_PavillionDepth.Size = new System.Drawing.Size(94, 20);
            this.tb_PavillionDepth.TabIndex = 41;
            this.tb_PavillionDepth.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_PavillionDepth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_PavillionDepth.Leave += new System.EventHandler(this.setDecimalPlaceTo2);
            this.tb_PavillionDepth.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label52.Location = new System.Drawing.Point(13, 46);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(81, 15);
            this.label52.TabIndex = 62;
            this.label52.Text = "StoneTable%";
            // 
            // tb_StarLength
            // 
            this.tb_StarLength.Location = new System.Drawing.Point(347, 158);
            this.tb_StarLength.Name = "tb_StarLength";
            this.tb_StarLength.ReadOnly = true;
            this.tb_StarLength.Size = new System.Drawing.Size(94, 20);
            this.tb_StarLength.TabIndex = 42;
            this.tb_StarLength.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_StarLength.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_StarLength.Leave += new System.EventHandler(this.setDecimalPlaceTo2);
            this.tb_StarLength.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.BackColor = System.Drawing.Color.Transparent;
            this.label64.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label64.Location = new System.Drawing.Point(13, 74);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(51, 15);
            this.label64.TabIndex = 56;
            this.label64.Text = "Depth%";
            // 
            // tb_LowerHalf
            // 
            this.tb_LowerHalf.Location = new System.Drawing.Point(347, 186);
            this.tb_LowerHalf.Name = "tb_LowerHalf";
            this.tb_LowerHalf.ReadOnly = true;
            this.tb_LowerHalf.Size = new System.Drawing.Size(94, 20);
            this.tb_LowerHalf.TabIndex = 43;
            this.tb_LowerHalf.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_LowerHalf.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_LowerHalf.Leave += new System.EventHandler(this.setDecimalPlaceTo2);
            this.tb_LowerHalf.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // tb_LabComment
            // 
            this.tb_LabComment.Location = new System.Drawing.Point(15, 239);
            this.tb_LabComment.Multiline = true;
            this.tb_LabComment.Name = "tb_LabComment";
            this.tb_LabComment.ReadOnly = true;
            this.tb_LabComment.Size = new System.Drawing.Size(426, 47);
            this.tb_LabComment.TabIndex = 44;
            this.tb_LabComment.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_LabComment.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_LabComment.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // tb_Measurements
            // 
            this.tb_Measurements.Location = new System.Drawing.Point(347, 47);
            this.tb_Measurements.Name = "tb_Measurements";
            this.tb_Measurements.ReadOnly = true;
            this.tb_Measurements.Size = new System.Drawing.Size(125, 20);
            this.tb_Measurements.TabIndex = 45;
            this.tb_Measurements.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_Measurements.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_Measurements.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // tb_GirdleMinMax
            // 
            this.tb_GirdleMinMax.Location = new System.Drawing.Point(113, 127);
            this.tb_GirdleMinMax.Name = "tb_GirdleMinMax";
            this.tb_GirdleMinMax.ReadOnly = true;
            this.tb_GirdleMinMax.Size = new System.Drawing.Size(94, 20);
            this.tb_GirdleMinMax.TabIndex = 57;
            this.tb_GirdleMinMax.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_GirdleMinMax.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_GirdleMinMax.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label33.Location = new System.Drawing.Point(247, 74);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(89, 15);
            this.label33.TabIndex = 46;
            this.label33.Text = "CrownHeight%";
            // 
            // tb_StoneTable
            // 
            this.tb_StoneTable.Location = new System.Drawing.Point(113, 45);
            this.tb_StoneTable.Name = "tb_StoneTable";
            this.tb_StoneTable.ReadOnly = true;
            this.tb_StoneTable.Size = new System.Drawing.Size(94, 20);
            this.tb_StoneTable.TabIndex = 54;
            this.tb_StoneTable.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_StoneTable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_StoneTable.Leave += new System.EventHandler(this.setDecimalPlaceTo2);
            this.tb_StoneTable.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label34.Location = new System.Drawing.Point(247, 102);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(89, 15);
            this.label34.TabIndex = 47;
            this.label34.Text = "PavillionAngle°";
            // 
            // tb_Depth
            // 
            this.tb_Depth.Location = new System.Drawing.Point(113, 72);
            this.tb_Depth.Name = "tb_Depth";
            this.tb_Depth.ReadOnly = true;
            this.tb_Depth.Size = new System.Drawing.Size(94, 20);
            this.tb_Depth.TabIndex = 53;
            this.tb_Depth.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_Depth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_Depth.Leave += new System.EventHandler(this.setDecimalPlaceTo2);
            this.tb_Depth.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label43.Location = new System.Drawing.Point(247, 130);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(97, 15);
            this.label43.TabIndex = 48;
            this.label43.Text = "PavillionDepth%";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label48.Location = new System.Drawing.Point(247, 158);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(78, 15);
            this.label48.TabIndex = 49;
            this.label48.Text = "StarLength%";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label51.Location = new System.Drawing.Point(246, 48);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(90, 15);
            this.label51.TabIndex = 52;
            this.label51.Text = "Measurements";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label49.Location = new System.Drawing.Point(247, 186);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(74, 15);
            this.label49.TabIndex = 50;
            this.label49.Text = "LowerHalf%";
            // 
            // cb_CertShapeDesc
            // 
            this.cb_CertShapeDesc.DropDownWidth = 150;
            this.cb_CertShapeDesc.FormattingEnabled = true;
            this.cb_CertShapeDesc.Location = new System.Drawing.Point(113, 103);
            this.cb_CertShapeDesc.Name = "cb_CertShapeDesc";
            this.cb_CertShapeDesc.Size = new System.Drawing.Size(90, 21);
            this.cb_CertShapeDesc.TabIndex = 56;
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label66.Location = new System.Drawing.Point(13, 106);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(71, 15);
            this.label66.TabIndex = 55;
            this.label66.Text = "ShapeDesc";
            // 
            // cb_CertSize
            // 
            this.cb_CertSize.FormattingEnabled = true;
            this.cb_CertSize.Location = new System.Drawing.Point(113, 130);
            this.cb_CertSize.Name = "cb_CertSize";
            this.cb_CertSize.Size = new System.Drawing.Size(90, 21);
            this.cb_CertSize.TabIndex = 39;
            this.cb_CertSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertSize.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertSize.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label27.Location = new System.Drawing.Point(13, 133);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(31, 15);
            this.label27.TabIndex = 38;
            this.label27.Text = "Size";
            // 
            // tb_CertInscription
            // 
            this.tb_CertInscription.Location = new System.Drawing.Point(347, 189);
            this.tb_CertInscription.Name = "tb_CertInscription";
            this.tb_CertInscription.ReadOnly = true;
            this.tb_CertInscription.Size = new System.Drawing.Size(94, 20);
            this.tb_CertInscription.TabIndex = 37;
            this.tb_CertInscription.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_CertInscription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_CertInscription.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // cb_CertFluorescence
            // 
            this.cb_CertFluorescence.FormattingEnabled = true;
            this.cb_CertFluorescence.Location = new System.Drawing.Point(347, 161);
            this.cb_CertFluorescence.Name = "cb_CertFluorescence";
            this.cb_CertFluorescence.Size = new System.Drawing.Size(94, 21);
            this.cb_CertFluorescence.TabIndex = 15;
            this.cb_CertFluorescence.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertFluorescence.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertFluorescence.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label41.Location = new System.Drawing.Point(247, 162);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(81, 15);
            this.label41.TabIndex = 14;
            this.label41.Text = "Fluorescence";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label42.Location = new System.Drawing.Point(247, 191);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(63, 15);
            this.label42.TabIndex = 8;
            this.label42.Text = "Inscription";
            // 
            // tb_CertWeight
            // 
            this.tb_CertWeight.Location = new System.Drawing.Point(113, 47);
            this.tb_CertWeight.Name = "tb_CertWeight";
            this.tb_CertWeight.ReadOnly = true;
            this.tb_CertWeight.Size = new System.Drawing.Size(90, 20);
            this.tb_CertWeight.TabIndex = 10;
            this.tb_CertWeight.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_CertWeight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_CertWeight.Leave += new System.EventHandler(this.setDecimalPlaceTo3);
            this.tb_CertWeight.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // cb_CertSymmetry
            // 
            this.cb_CertSymmetry.FormattingEnabled = true;
            this.cb_CertSymmetry.Location = new System.Drawing.Point(347, 132);
            this.cb_CertSymmetry.Name = "cb_CertSymmetry";
            this.cb_CertSymmetry.Size = new System.Drawing.Size(94, 21);
            this.cb_CertSymmetry.TabIndex = 13;
            this.cb_CertSymmetry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertSymmetry.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertSymmetry.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label40.Location = new System.Drawing.Point(247, 133);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(61, 15);
            this.label40.TabIndex = 12;
            this.label40.Text = "Symmetry";
            // 
            // cb_CertPolish
            // 
            this.cb_CertPolish.FormattingEnabled = true;
            this.cb_CertPolish.Location = new System.Drawing.Point(347, 103);
            this.cb_CertPolish.Name = "cb_CertPolish";
            this.cb_CertPolish.Size = new System.Drawing.Size(94, 21);
            this.cb_CertPolish.TabIndex = 11;
            this.cb_CertPolish.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertPolish.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertPolish.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label39.Location = new System.Drawing.Point(247, 106);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(41, 15);
            this.label39.TabIndex = 10;
            this.label39.Text = "Polish";
            // 
            // cb_CertCut
            // 
            this.cb_CertCut.FormattingEnabled = true;
            this.cb_CertCut.Location = new System.Drawing.Point(347, 75);
            this.cb_CertCut.Name = "cb_CertCut";
            this.cb_CertCut.Size = new System.Drawing.Size(94, 21);
            this.cb_CertCut.TabIndex = 9;
            this.cb_CertCut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertCut.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertCut.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label38.Location = new System.Drawing.Point(247, 76);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(25, 15);
            this.label38.TabIndex = 8;
            this.label38.Text = "Cut";
            // 
            // cb_CertColor
            // 
            this.cb_CertColor.FormattingEnabled = true;
            this.cb_CertColor.Location = new System.Drawing.Point(113, 159);
            this.cb_CertColor.Name = "cb_CertColor";
            this.cb_CertColor.Size = new System.Drawing.Size(90, 21);
            this.cb_CertColor.TabIndex = 7;
            this.cb_CertColor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertColor.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertColor.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label25.Location = new System.Drawing.Point(13, 48);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(45, 15);
            this.label25.TabIndex = 3;
            this.label25.Text = "Weight";
            // 
            // cb_CertClarity
            // 
            this.cb_CertClarity.FormattingEnabled = true;
            this.cb_CertClarity.Location = new System.Drawing.Point(113, 189);
            this.cb_CertClarity.Name = "cb_CertClarity";
            this.cb_CertClarity.Size = new System.Drawing.Size(90, 21);
            this.cb_CertClarity.TabIndex = 6;
            this.cb_CertClarity.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertClarity.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertClarity.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // cb_CertShape
            // 
            this.cb_CertShape.FormattingEnabled = true;
            this.cb_CertShape.Location = new System.Drawing.Point(113, 73);
            this.cb_CertShape.Name = "cb_CertShape";
            this.cb_CertShape.Size = new System.Drawing.Size(90, 21);
            this.cb_CertShape.TabIndex = 4;
            this.cb_CertShape.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertShape.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertShape.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label29.Location = new System.Drawing.Point(13, 191);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(40, 15);
            this.label29.TabIndex = 3;
            this.label29.Text = "Clarity";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label30.Location = new System.Drawing.Point(13, 162);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(36, 15);
            this.label30.TabIndex = 2;
            this.label30.Text = "Color";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label36.Location = new System.Drawing.Point(13, 76);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(43, 15);
            this.label36.TabIndex = 0;
            this.label36.Text = "Shape";
            // 
            // dateTime_CertDate
            // 
            this.dateTime_CertDate.CustomFormat = "";
            this.dateTime_CertDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTime_CertDate.Location = new System.Drawing.Point(346, 84);
            this.dateTime_CertDate.Name = "dateTime_CertDate";
            this.dateTime_CertDate.Size = new System.Drawing.Size(94, 20);
            this.dateTime_CertDate.TabIndex = 77;
            this.dateTime_CertDate.Value = new System.DateTime(2024, 8, 29, 0, 0, 0, 0);
            this.dateTime_CertDate.Leave += new System.EventHandler(this.CertComboBox_Validated);
            this.dateTime_CertDate.Validated += new System.EventHandler(this.CertComboBox_Validated);
            // 
            // bt_FetchCertificate
            // 
            this.bt_FetchCertificate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.bt_FetchCertificate.Location = new System.Drawing.Point(346, 43);
            this.bt_FetchCertificate.Name = "bt_FetchCertificate";
            this.bt_FetchCertificate.Size = new System.Drawing.Size(95, 27);
            this.bt_FetchCertificate.TabIndex = 30;
            this.bt_FetchCertificate.Text = "Download";
            this.bt_FetchCertificate.UseVisualStyleBackColor = true;
            this.bt_FetchCertificate.Click += new System.EventHandler(this.bt_FetchCertificate_Click);
            // 
            // label129
            // 
            this.label129.AutoSize = true;
            this.label129.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label129.Location = new System.Drawing.Point(12, 119);
            this.label129.Name = "label129";
            this.label129.Size = new System.Drawing.Size(61, 15);
            this.label129.TabIndex = 74;
            this.label129.Text = "TypeDesc";
            // 
            // tb_CertCertificateTypeDesc
            // 
            this.tb_CertCertificateTypeDesc.Location = new System.Drawing.Point(112, 116);
            this.tb_CertCertificateTypeDesc.Name = "tb_CertCertificateTypeDesc";
            this.tb_CertCertificateTypeDesc.ReadOnly = true;
            this.tb_CertCertificateTypeDesc.Size = new System.Drawing.Size(328, 20);
            this.tb_CertCertificateTypeDesc.TabIndex = 75;
            this.tb_CertCertificateTypeDesc.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_CertCertificateTypeDesc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_CertCertificateTypeDesc.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // cb_CertLabName
            // 
            this.cb_CertLabName.FormattingEnabled = true;
            this.cb_CertLabName.Items.AddRange(new object[] {
            "ACTIVE",
            "ARCHIVED"});
            this.cb_CertLabName.Location = new System.Drawing.Point(58, 46);
            this.cb_CertLabName.Name = "cb_CertLabName";
            this.cb_CertLabName.Size = new System.Drawing.Size(67, 21);
            this.cb_CertLabName.TabIndex = 44;
            this.cb_CertLabName.SelectedIndexChanged += new System.EventHandler(this.cb_CertLabName_SelectedIndexChanged);
            this.cb_CertLabName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cb_CertLabName_ValidatedByEnter);
            this.cb_CertLabName.Validated += new System.EventHandler(this.cb_CertLabName_Validated);
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label44.Location = new System.Drawing.Point(246, 87);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(90, 15);
            this.label44.TabIndex = 2;
            this.label44.Text = "Certificate Date";
            // 
            // tb_CertNo
            // 
            this.tb_CertNo.BackColor = System.Drawing.SystemColors.Control;
            this.tb_CertNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.tb_CertNo.Location = new System.Drawing.Point(227, 45);
            this.tb_CertNo.Name = "tb_CertNo";
            this.tb_CertNo.Size = new System.Drawing.Size(94, 22);
            this.tb_CertNo.TabIndex = 12;
            this.tb_CertNo.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_CertNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_CertNo.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label45.Location = new System.Drawing.Point(141, 49);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(80, 15);
            this.label45.TabIndex = 1;
            this.label45.Text = "Certificate No";
            // 
            // cb_CertCertificateType
            // 
            this.cb_CertCertificateType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cb_CertCertificateType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cb_CertCertificateType.FormattingEnabled = true;
            this.cb_CertCertificateType.Location = new System.Drawing.Point(112, 84);
            this.cb_CertCertificateType.Name = "cb_CertCertificateType";
            this.cb_CertCertificateType.Size = new System.Drawing.Size(103, 21);
            this.cb_CertCertificateType.TabIndex = 40;
            this.cb_CertCertificateType.SelectedIndexChanged += new System.EventHandler(this.FillCertTypeDesc);
            this.cb_CertCertificateType.TextChanged += new System.EventHandler(this.FillCertTypeDesc);
            this.cb_CertCertificateType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedByEnter);
            this.cb_CertCertificateType.Leave += new System.EventHandler(this.CertComboBox_Validated);
            this.cb_CertCertificateType.Validated += new System.EventHandler(this.CertComboBox_Validated);
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label46.Location = new System.Drawing.Point(13, 49);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(28, 15);
            this.label46.TabIndex = 0;
            this.label46.Text = "Lab";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label50.Location = new System.Drawing.Point(12, 87);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(87, 15);
            this.label50.TabIndex = 51;
            this.label50.Text = "CertificateType";
            // 
            // pnl_Report
            // 
            this.pnl_Report.BackColor = System.Drawing.Color.White;
            this.pnl_Report.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_Report.Controls.Add(this.pnl_banner1);
            this.pnl_Report.Controls.Add(this.tb_CertCertificateTypeDesc);
            this.pnl_Report.Controls.Add(this.dateTime_CertDate);
            this.pnl_Report.Controls.Add(this.label44);
            this.pnl_Report.Controls.Add(this.bt_FetchCertificate);
            this.pnl_Report.Controls.Add(this.label50);
            this.pnl_Report.Controls.Add(this.label46);
            this.pnl_Report.Controls.Add(this.cb_CertCertificateType);
            this.pnl_Report.Controls.Add(this.label129);
            this.pnl_Report.Controls.Add(this.label45);
            this.pnl_Report.Controls.Add(this.tb_CertNo);
            this.pnl_Report.Controls.Add(this.cb_CertLabName);
            this.pnl_Report.Location = new System.Drawing.Point(4, 4);
            this.pnl_Report.Name = "pnl_Report";
            this.pnl_Report.Size = new System.Drawing.Size(496, 177);
            this.pnl_Report.TabIndex = 11;
            // 
            // pnl_banner1
            // 
            this.pnl_banner1.BackColor = System.Drawing.Color.Tan;
            this.pnl_banner1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_banner1.Controls.Add(this.label1);
            this.pnl_banner1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_banner1.Location = new System.Drawing.Point(0, 0);
            this.pnl_banner1.Name = "pnl_banner1";
            this.pnl_banner1.Size = new System.Drawing.Size(494, 30);
            this.pnl_banner1.TabIndex = 78;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(175, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "DIAMOND REPORT";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnl_Grading
            // 
            this.pnl_Grading.BackColor = System.Drawing.Color.White;
            this.pnl_Grading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_Grading.Controls.Add(this.pnl_banner2);
            this.pnl_Grading.Controls.Add(this.cb_CertShapeDesc);
            this.pnl_Grading.Controls.Add(this.label66);
            this.pnl_Grading.Controls.Add(this.tb_CertInscription);
            this.pnl_Grading.Controls.Add(this.label42);
            this.pnl_Grading.Controls.Add(this.label51);
            this.pnl_Grading.Controls.Add(this.label36);
            this.pnl_Grading.Controls.Add(this.cb_CertFluorescence);
            this.pnl_Grading.Controls.Add(this.label41);
            this.pnl_Grading.Controls.Add(this.label30);
            this.pnl_Grading.Controls.Add(this.label29);
            this.pnl_Grading.Controls.Add(this.cb_CertSymmetry);
            this.pnl_Grading.Controls.Add(this.cb_CertSize);
            this.pnl_Grading.Controls.Add(this.label40);
            this.pnl_Grading.Controls.Add(this.cb_CertShape);
            this.pnl_Grading.Controls.Add(this.cb_CertPolish);
            this.pnl_Grading.Controls.Add(this.label27);
            this.pnl_Grading.Controls.Add(this.label39);
            this.pnl_Grading.Controls.Add(this.cb_CertClarity);
            this.pnl_Grading.Controls.Add(this.cb_CertCut);
            this.pnl_Grading.Controls.Add(this.label25);
            this.pnl_Grading.Controls.Add(this.label38);
            this.pnl_Grading.Controls.Add(this.cb_CertColor);
            this.pnl_Grading.Controls.Add(this.tb_Measurements);
            this.pnl_Grading.Controls.Add(this.tb_CertWeight);
            this.pnl_Grading.Location = new System.Drawing.Point(4, 187);
            this.pnl_Grading.Name = "pnl_Grading";
            this.pnl_Grading.Size = new System.Drawing.Size(496, 220);
            this.pnl_Grading.TabIndex = 79;
            // 
            // pnl_banner2
            // 
            this.pnl_banner2.BackColor = System.Drawing.Color.Tan;
            this.pnl_banner2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_banner2.Controls.Add(this.label2);
            this.pnl_banner2.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_banner2.Location = new System.Drawing.Point(0, 0);
            this.pnl_banner2.Name = "pnl_banner2";
            this.pnl_banner2.Size = new System.Drawing.Size(494, 30);
            this.pnl_banner2.TabIndex = 78;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(173, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "GRADING RESULTS";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnl_Proportions
            // 
            this.pnl_Proportions.BackColor = System.Drawing.Color.White;
            this.pnl_Proportions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_Proportions.Controls.Add(this.label63);
            this.pnl_Proportions.Controls.Add(this.cb_CertCulet);
            this.pnl_Proportions.Controls.Add(this.pnl_banner3);
            this.pnl_Proportions.Controls.Add(this.cb_CertGirdleCondition);
            this.pnl_Proportions.Controls.Add(this.tb_LabComment);
            this.pnl_Proportions.Controls.Add(this.cb_CertGirdle);
            this.pnl_Proportions.Controls.Add(this.label49);
            this.pnl_Proportions.Controls.Add(this.label32);
            this.pnl_Proportions.Controls.Add(this.label48);
            this.pnl_Proportions.Controls.Add(this.label43);
            this.pnl_Proportions.Controls.Add(this.tb_CrownAngle);
            this.pnl_Proportions.Controls.Add(this.tb_Depth);
            this.pnl_Proportions.Controls.Add(this.label57);
            this.pnl_Proportions.Controls.Add(this.label34);
            this.pnl_Proportions.Controls.Add(this.label58);
            this.pnl_Proportions.Controls.Add(this.tb_StoneTable);
            this.pnl_Proportions.Controls.Add(this.tb_CrownHeight);
            this.pnl_Proportions.Controls.Add(this.label33);
            this.pnl_Proportions.Controls.Add(this.label59);
            this.pnl_Proportions.Controls.Add(this.tb_GirdleMinMax);
            this.pnl_Proportions.Controls.Add(this.tb_PavillionAngle);
            this.pnl_Proportions.Controls.Add(this.label60);
            this.pnl_Proportions.Controls.Add(this.tb_LowerHalf);
            this.pnl_Proportions.Controls.Add(this.tb_PavillionDepth);
            this.pnl_Proportions.Controls.Add(this.label64);
            this.pnl_Proportions.Controls.Add(this.label52);
            this.pnl_Proportions.Controls.Add(this.tb_StarLength);
            this.pnl_Proportions.Location = new System.Drawing.Point(506, 4);
            this.pnl_Proportions.Name = "pnl_Proportions";
            this.pnl_Proportions.Size = new System.Drawing.Size(462, 303);
            this.pnl_Proportions.TabIndex = 80;
            // 
            // pnl_banner3
            // 
            this.pnl_banner3.BackColor = System.Drawing.Color.Tan;
            this.pnl_banner3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_banner3.Controls.Add(this.label3);
            this.pnl_banner3.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_banner3.Location = new System.Drawing.Point(0, 0);
            this.pnl_banner3.Name = "pnl_banner3";
            this.pnl_banner3.Size = new System.Drawing.Size(460, 30);
            this.pnl_banner3.TabIndex = 78;
            this.pnl_banner3.Paint += new System.Windows.Forms.PaintEventHandler(this.pnl_banner3_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(190, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "PROPORTIONS";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnl_CertificateStatus
            // 
            this.pnl_CertificateStatus.BackColor = System.Drawing.Color.White;
            this.pnl_CertificateStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_CertificateStatus.Controls.Add(this.cb_CertStatus);
            this.pnl_CertificateStatus.Controls.Add(this.label5);
            this.pnl_CertificateStatus.Controls.Add(this.panel2);
            this.pnl_CertificateStatus.Controls.Add(this.tb_CertRemark);
            this.pnl_CertificateStatus.Controls.Add(this.label10);
            this.pnl_CertificateStatus.Location = new System.Drawing.Point(506, 313);
            this.pnl_CertificateStatus.Name = "pnl_CertificateStatus";
            this.pnl_CertificateStatus.Size = new System.Drawing.Size(462, 94);
            this.pnl_CertificateStatus.TabIndex = 89;
            // 
            // cb_CertStatus
            // 
            this.cb_CertStatus.FormattingEnabled = true;
            this.cb_CertStatus.Items.AddRange(new object[] {
            "ACTIVE",
            "ARCHIVED"});
            this.cb_CertStatus.Location = new System.Drawing.Point(113, 63);
            this.cb_CertStatus.Name = "cb_CertStatus";
            this.cb_CertStatus.Size = new System.Drawing.Size(94, 21);
            this.cb_CertStatus.TabIndex = 81;
            this.cb_CertStatus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertComboBox_ValidatedToUpperByEnter);
            this.cb_CertStatus.Leave += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            this.cb_CertStatus.Validated += new System.EventHandler(this.CertComboBox_ValidatedToUpper);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label5.Location = new System.Drawing.Point(14, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 15);
            this.label5.TabIndex = 79;
            this.label5.Text = "Status";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Tan;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label6);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(460, 30);
            this.panel2.TabIndex = 78;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(175, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(137, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "CERTIFICATE STATUS";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tb_CertRemark
            // 
            this.tb_CertRemark.Location = new System.Drawing.Point(113, 36);
            this.tb_CertRemark.Name = "tb_CertRemark";
            this.tb_CertRemark.ReadOnly = true;
            this.tb_CertRemark.Size = new System.Drawing.Size(328, 20);
            this.tb_CertRemark.TabIndex = 75;
            this.tb_CertRemark.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            this.tb_CertRemark.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CertTextBox_ValidatedByEnter);
            this.tb_CertRemark.Validated += new System.EventHandler(this.CertTextBox_Validated);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label10.Location = new System.Drawing.Point(13, 39);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 15);
            this.label10.TabIndex = 74;
            this.label10.Text = "Remark";
            // 
            // bt_Update
            // 
            this.bt_Update.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.bt_Update.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.bt_Update.Location = new System.Drawing.Point(874, 413);
            this.bt_Update.Name = "bt_Update";
            this.bt_Update.Size = new System.Drawing.Size(94, 34);
            this.bt_Update.TabIndex = 90;
            this.bt_Update.Text = "Update";
            this.bt_Update.UseVisualStyleBackColor = true;
            // 
            // CertificateDetailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bt_Update);
            this.Controls.Add(this.pnl_CertificateStatus);
            this.Controls.Add(this.pnl_Proportions);
            this.Controls.Add(this.pnl_Grading);
            this.Controls.Add(this.pnl_Report);
            this.Name = "CertificateDetailView";
            this.Size = new System.Drawing.Size(974, 459);
            this.pnl_Report.ResumeLayout(false);
            this.pnl_Report.PerformLayout();
            this.pnl_banner1.ResumeLayout(false);
            this.pnl_banner1.PerformLayout();
            this.pnl_Grading.ResumeLayout(false);
            this.pnl_Grading.PerformLayout();
            this.pnl_banner2.ResumeLayout(false);
            this.pnl_banner2.PerformLayout();
            this.pnl_Proportions.ResumeLayout(false);
            this.pnl_Proportions.PerformLayout();
            this.pnl_banner3.ResumeLayout(false);
            this.pnl_banner3.PerformLayout();
            this.pnl_CertificateStatus.ResumeLayout(false);
            this.pnl_CertificateStatus.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox cb_CertShapeDesc;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.ComboBox cb_CertLabName;
        private System.Windows.Forms.ComboBox cb_CertSize;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox tb_CertInscription;
        private System.Windows.Forms.Button bt_FetchCertificate;
        private System.Windows.Forms.ComboBox cb_CertFluorescence;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.TextBox tb_CertWeight;
        private System.Windows.Forms.ComboBox cb_CertSymmetry;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.ComboBox cb_CertPolish;
        private System.Windows.Forms.TextBox tb_CertNo;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.ComboBox cb_CertCut;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.ComboBox cb_CertColor;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.ComboBox cb_CertClarity;
        private System.Windows.Forms.ComboBox cb_CertShape;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.DateTimePicker dateTime_CertDate;
        private System.Windows.Forms.Label label129;
        private System.Windows.Forms.TextBox tb_CertCertificateTypeDesc;
        private System.Windows.Forms.ComboBox cb_CertCulet;
        private System.Windows.Forms.ComboBox cb_CertGirdleCondition;
        private System.Windows.Forms.ComboBox cb_CertGirdle;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.TextBox tb_CrownAngle;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.TextBox tb_CrownHeight;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.TextBox tb_PavillionAngle;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.TextBox tb_PavillionDepth;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.TextBox tb_StarLength;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.TextBox tb_LowerHalf;
        private System.Windows.Forms.TextBox tb_LabComment;
        private System.Windows.Forms.TextBox tb_Measurements;
        private System.Windows.Forms.TextBox tb_GirdleMinMax;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox tb_StoneTable;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox tb_Depth;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.ComboBox cb_CertCertificateType;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Panel pnl_Report;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnl_banner1;
        private System.Windows.Forms.Panel pnl_Grading;
        private System.Windows.Forms.Panel pnl_banner2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnl_Proportions;
        private System.Windows.Forms.Panel pnl_banner3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnl_CertificateStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_CertRemark;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cb_CertStatus;
        public System.Windows.Forms.Button bt_Update;
    }
}
