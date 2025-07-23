using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Models
{
    public class DocLineWithCertificate
    {
        public DocLineDto DocLine { get; set; }
        public CertificateDto Certificate { get; set; }

        public DocLineWithCertificate(DocLineDto docLine, CertificateDto certificate)
        {
            DocLine = docLine;
            Certificate = certificate;
        }
    }
}
