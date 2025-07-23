using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Services
{
    public class CertificateService
    {
        private readonly ICertificateRepository _certificateRepository;

        public CertificateService(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }
        public async Task<CertificateDto> GetCertificateAsync(int certificateID)
        {
            return await _certificateRepository.GetCertificateAsync(certificateID);
        }
        public bool UpdateCertificate(CertificateDto certificateDto)
        {
            return _certificateRepository.UpdateCertificate(certificateDto);
        }
        public int InsertCertificate(CertificateDto cert)
        {
            return _certificateRepository.InsertCertificate(cert);
        }



    }
}
