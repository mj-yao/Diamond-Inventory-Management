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
using DiamondTransaction.UseCases.Models;
using DiamondTransaction.UI.Controls;
using DiamondTransaction.UI.Utilities;
using DiamondTransaction.UseCases.Services;
using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UI.Event;

namespace DiamondTransaction
{
    public partial class CertificateForm : Form
    {
        public bool IsNewCertificate { get; private set; }
        public CertificateDto Certificate { get; set; }
        private int _docTypeID;

        private List<CertificateTypeDto> _certificateTypes;
        private CertificateService _certificateService;
        private DiamondGradingService _diamondGradingService;
        private CertificateTypeService _certificateTypeService;

        public event EventHandler<SaveCertificateEventArgs> SaveCertificate;


        public CertificateForm(int newDocTypeID, CertificateDto certificate)
        {
            InitializeComponent();
            InitializeServices();
            _docTypeID = newDocTypeID;
            IsNewCertificate = (newDocTypeID == 1006 || newDocTypeID == 1009);
            certificateDetailView1.IsNewCertificate = IsNewCertificate;
            Certificate = certificate ?? new CertificateDto();
            this.Load += fmCertificate_Load;

        }
        private void InitializeServices()
        {
            // Initialize the CertificateService
            var certificateRepository = new CertificateDataAccess(SqlQueryExecutor.getConnectionString());           
            certificateDetailView1._certificateService = _certificateService;
            _certificateService = new CertificateService(certificateRepository);

            // Initialize the CertificateTypeService
            var certificateTypeRepository = new CertificateTypeDataAccess(SqlQueryExecutor.getConnectionString());
            _certificateTypeService = new CertificateTypeService(certificateTypeRepository);           
            certificateDetailView1._certificateTypeService = _certificateTypeService;

            // Initialize the DiamondGradingService
            var diamondGradingRepository = new DiamondGradingDataAccess(SqlQueryExecutor.getConnectionString());            
            _diamondGradingService = new DiamondGradingService(diamondGradingRepository);
            certificateDetailView1._diamondGradingService = _diamondGradingService;

        }

        private async void fmCertificate_Load(object sender, EventArgs e)
        {            
            await certificateDetailView1.InitializeDataAndControls();
            await certificateDetailView1.InitializeLabBasedControls(Certificate.CertificateLabName);
            certificateDetailView1.SetCertificateDto(Certificate);          

        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            Certificate = certificateDetailView1.GetCertificateData();

            var agrs = new SaveCertificateEventArgs(Certificate);
            SaveCertificate?.Invoke(this, agrs);

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
                FormControlHelper.ResetControls(certificateDetailView1);                
                this.Close();
            }
        }

        private void CertificateForm_Load(object sender, EventArgs e)
        {

        }
    }
}
