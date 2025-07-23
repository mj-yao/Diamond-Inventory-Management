using DiamondTransaction.Domain.Interfaces;
using DiamondTransaction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases
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
