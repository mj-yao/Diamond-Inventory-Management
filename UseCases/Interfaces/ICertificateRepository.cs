using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Interfaces
{
    public interface ICertificateRepository
    {
        Task<CertificateDto> GetCertificateAsync(int certificateID);
        bool UpdateCertificate(CertificateDto certificateDto);
        int InsertCertificate(CertificateDto cert);

    }
}
