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
using DiamondTransaction.UseCases;
using DiamondTransaction.Domain.Interfaces;

namespace DiamondTransaction
{
    public partial class fmCertificate : Form
    {
        public bool IsNewCertificate { get; private set; }
        private int _docTypeID;

        private List<CertificateTypeDto> _certificateTypes;
        public CertificateService _certificateService;
        public DiamondGradingService _diamondGradingService;
        public CertificateTypeService _certificateTypeService;



        public CertificateDto _certificateData { get; private set; }

        public fmCertificate(int newDocTypeID, ref CertificateDto certificateData)
        {
            InitializeComponent();
            InitializeServices();
            _docTypeID = newDocTypeID;
            IsNewCertificate = (newDocTypeID == 1006 || newDocTypeID == 1009);
            certificateDetailView.IsNewCertificate = IsNewCertificate;
            this._certificateData = certificateData;
            this.Load += fmCertificate_Load;

        }
        private void InitializeServices()
        {
            // Initialize the CertificateService
            var certificateRepository = new CertificateDataAccess(SqlQueryExecutor.getConnectionString());           
            certificateDetailView._certificateService = _certificateService;
            _certificateService = new CertificateService(certificateRepository);

            // Initialize the CertificateTypeService
            var certificateTypeRepository = new CertificateTypeDataAccess(SqlQueryExecutor.getConnectionString());
            _certificateTypeService = new CertificateTypeService(certificateTypeRepository);           
            certificateDetailView._certificateTypeService = _certificateTypeService;

            // Initialize the DiamondGradingService
            var diamondGradingRepository = new DiamondGradingDataAccess(SqlQueryExecutor.getConnectionString());            
            _diamondGradingService = new DiamondGradingService(diamondGradingRepository);
            certificateDetailView._diamondGradingService = _diamondGradingService;

        }

        private async void fmCertificate_Load(object sender, EventArgs e)
        {            
            await certificateDetailView.InitializeDataAndControls();
            certificateDetailView.SetCertificateDto(_certificateData);          

        }



        private void bt_Save_Click(object sender, EventArgs e)
        {
            certificateDetailView.UpdateCertificateData();

            _certificateData.CertificateLabName = certificateDetailView._certificateData.CertificateLabName;
            _certificateData.CertificateType = certificateDetailView._certificateData.CertificateType;
            _certificateData.CertificateTypeDesc = certificateDetailView._certificateData.CertificateTypeDesc;
            _certificateData.CertificateNo = certificateDetailView._certificateData.CertificateNo;
            _certificateData.CertificateDate = certificateDetailView._certificateData.CertificateDate;
            _certificateData.Shape = certificateDetailView._certificateData.Shape;
            _certificateData.Size = certificateDetailView._certificateData.Size;
            _certificateData.Color = certificateDetailView._certificateData.Color;
            _certificateData.Clarity = certificateDetailView._certificateData.Clarity;
            _certificateData.Measurements = certificateDetailView._certificateData.Measurements;
            _certificateData.Weight = certificateDetailView._certificateData.Weight;
            _certificateData.Depth = certificateDetailView._certificateData.Depth;
            _certificateData.StoneTable = certificateDetailView._certificateData.StoneTable;
            _certificateData.GirdleMinMax = certificateDetailView._certificateData.GirdleMinMax;
            _certificateData.GirdleCondition = certificateDetailView._certificateData.GirdleCondition;
            _certificateData.Girdle = certificateDetailView._certificateData.Girdle;
            _certificateData.Culet = certificateDetailView._certificateData.Culet;
            _certificateData.Polish = certificateDetailView._certificateData.Polish;
            _certificateData.Symmetry = certificateDetailView._certificateData.Symmetry;
            _certificateData.Fluorescence = certificateDetailView._certificateData.Fluorescence;
            _certificateData.Cut = certificateDetailView._certificateData.Cut;
            _certificateData.CrownAngle = certificateDetailView._certificateData.CrownAngle;
            _certificateData.CrownHeight = certificateDetailView._certificateData.CrownHeight;
            _certificateData.PavillionAngle = certificateDetailView._certificateData.PavillionAngle;
            _certificateData.PavillionDepth = certificateDetailView._certificateData.PavillionDepth;
            _certificateData.StarLength = certificateDetailView._certificateData.StarLength;
            _certificateData.LowerHalf = certificateDetailView._certificateData.LowerHalf;
            _certificateData.Inscription = certificateDetailView._certificateData.Inscription;
            _certificateData.LabComment = certificateDetailView._certificateData.LabComment;

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
                FormControlHelper.ResetControls(certificateDetailView);                
                this.Close();
            }
        }
    }
}
