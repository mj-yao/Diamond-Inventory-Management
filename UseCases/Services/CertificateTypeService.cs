using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Services
{
    public class CertificateTypeService
    {
        private readonly ICertificateTypeRepository _certificateTypeRepository;

        public CertificateTypeService(ICertificateTypeRepository certificateTypeRepository)
        {
            _certificateTypeRepository = certificateTypeRepository;
        }

        public async Task<IEnumerable<CertificateTypeDto>> GetAllCertificateTypesAsync()
        {
            return await _certificateTypeRepository.GetAllCertificateTypesAsync();
        }
    }
}
