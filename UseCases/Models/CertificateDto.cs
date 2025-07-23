using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction
{
    public class CertificateDto
    {        
        public int CertificateID { get; set; }
        public int LotID { get; set; }
        public string CertificateLabName { get; set; }
        public string CertificateType { get; set; }
        public string CertificateTypeDesc { get; set; }
        public string CertificateNo { get; set; }
        public DateTime CertificateDate { get; set; }
        public string Shape { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Measurements { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Depth { get; set; }
        public decimal? StoneTable { get; set; }
        public string GirdleMinMax { get; set; }
        public string GirdleCondition { get; set; }
        public string Girdle { get; set; }
        public string Culet { get; set; }
        public string Polish { get; set; }
        public string Symmetry { get; set; }
        public string Fluorescence { get; set; }
        public string Cut { get; set; }
        public decimal? CrownAngle { get; set; }
        public decimal? CrownHeight { get; set; }
        public decimal? PavillionAngle { get; set; }
        public decimal? PavillionDepth { get; set; }
        public decimal? StarLength { get; set; }
        public decimal? LowerHalf { get; set; }
        public string Inscription { get; set; }
        public string LabComment { get; set; }
        public string DownloadStatus { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string Created_By { get; set; }
        public DateTime? DateTime_Created { get; set; }

        // Constructor for easy initialization
        public CertificateDto()
        {
            // Initialize all fields with default values, if necessary
        }
    }
}
