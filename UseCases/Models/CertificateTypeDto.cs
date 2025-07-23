using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Models
{
    public class CertificateTypeDto
    {
        public int LabAccountID { get; set; }
        public string LabAccountName { get; set; }
        public string CertificateType { get; set; }
        public string CertificateTypeDesc { get; set; }
        public int CertificateTypeOrder { get; set; }
    }
}
