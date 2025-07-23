using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiamondTransaction.DataAccess;
using DiamondTransaction.Models;
using DiamondTransaction.UI.Utilities;

namespace DiamondTransaction
{
    public partial class Certificate : Form
    {
        public bool IsNewCertificate { get; private set; }
        private int _docTypeID;

        public CertificateData _certificateData { get; private set; }

        public Certificate(int newDocTypeID, ref CertificateData certificateData)
        {
            InitializeComponent();
            _docTypeID = newDocTypeID;
            IsNewCertificate = (newDocTypeID == 1006 || newDocTypeID == 1009);
            certificateDetail1.IsNewCertificate = IsNewCertificate;
            this._certificateData = certificateData;

        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            certificateDetail1.UpdateCertificateData();

            _certificateData.CertificateLabName = certificateDetail1._certificateData.CertificateLabName;
            _certificateData.CertificateType = certificateDetail1._certificateData.CertificateType;
            _certificateData.CertificateTypeDesc = certificateDetail1._certificateData.CertificateTypeDesc;
            _certificateData.CertificateNo = certificateDetail1._certificateData.CertificateNo;
            _certificateData.CertificateDate = certificateDetail1._certificateData.CertificateDate;
            _certificateData.Shape = certificateDetail1._certificateData.Shape;
            _certificateData.Size = certificateDetail1._certificateData.Size;
            _certificateData.Color = certificateDetail1._certificateData.Color;
            _certificateData.Clarity = certificateDetail1._certificateData.Clarity;
            _certificateData.Measurements = certificateDetail1._certificateData.Measurements;
            _certificateData.Weight = certificateDetail1._certificateData.Weight;
            _certificateData.Depth = certificateDetail1._certificateData.Depth;
            _certificateData.StoneTable = certificateDetail1._certificateData.StoneTable;
            _certificateData.GirdleMinMax = certificateDetail1._certificateData.GirdleMinMax;
            _certificateData.GirdleCondition = certificateDetail1._certificateData.GirdleCondition;
            _certificateData.Girdle = certificateDetail1._certificateData.Girdle;
            _certificateData.Culet = certificateDetail1._certificateData.Culet;
            _certificateData.Polish = certificateDetail1._certificateData.Polish;
            _certificateData.Symmetry = certificateDetail1._certificateData.Symmetry;
            _certificateData.Fluorescence = certificateDetail1._certificateData.Fluorescence;
            _certificateData.Cut = certificateDetail1._certificateData.Cut;
            _certificateData.CrownAngle = certificateDetail1._certificateData.CrownAngle;
            _certificateData.CrownHeight = certificateDetail1._certificateData.CrownHeight;
            _certificateData.PavillionAngle = certificateDetail1._certificateData.PavillionAngle;
            _certificateData.PavillionDepth = certificateDetail1._certificateData.PavillionDepth;
            _certificateData.StarLength = certificateDetail1._certificateData.StarLength;
            _certificateData.LowerHalf = certificateDetail1._certificateData.LowerHalf;
            _certificateData.Inscription = certificateDetail1._certificateData.Inscription;
            _certificateData.LabComment = certificateDetail1._certificateData.LabComment;

            this.Close();
        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
            "Are you sure you want to cancel the document?",
            "Confirm Cancel",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                FormControlHelper.ResetControls(certificateDetail1);                
                this.Close();
            }
        }
    }
}
