namespace DiamondTransaction
{
    partial class fmCertificate
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bt_Save = new System.Windows.Forms.Button();
            this.certificateDetailView = new DiamondTransaction.CertificateDetailView();
            this.bt_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bt_Save
            // 
            this.bt_Save.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.bt_Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.bt_Save.Location = new System.Drawing.Point(889, 382);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(94, 34);
            this.bt_Save.TabIndex = 1;
            this.bt_Save.Text = "Save";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // certificateDetailView
            // 
            this.certificateDetailView.IsNewCertificate = false;
            this.certificateDetailView.Location = new System.Drawing.Point(13, 2);
            this.certificateDetailView.Name = "certificateDetailView";
            this.certificateDetailView.Size = new System.Drawing.Size(970, 414);
            this.certificateDetailView.TabIndex = 0;
            // 
            // bt_Cancel
            // 
            this.bt_Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.bt_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.bt_Cancel.Location = new System.Drawing.Point(708, 382);
            this.bt_Cancel.Name = "bt_Cancel";
            this.bt_Cancel.Size = new System.Drawing.Size(94, 34);
            this.bt_Cancel.TabIndex = 2;
            this.bt_Cancel.Text = "Cancel";
            this.bt_Cancel.UseVisualStyleBackColor = true;
            this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
            // 
            // Certificate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 432);
            this.Controls.Add(this.bt_Cancel);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.certificateDetailView);
            this.Name = "Certificate";
            this.Text = "Certificate";
            this.ResumeLayout(false);

        }

        #endregion

        public CertificateDetailView certificateDetailView;
        public System.Windows.Forms.Button bt_Save;
        public System.Windows.Forms.Button bt_Cancel;
    }
}