using DiamondTransaction.Domain.Interfaces;
using DiamondTransaction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases
{
    public class DocHeaderService
    {
        private readonly IDocHeaderRepository _repository;
        public DocHeaderService(IDocHeaderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DocHeaderDto>> GetAllDiamondDocHeadersAsync()
        {
            return await _repository.GetAllDiamondDocHeadersAsync();
        }
    }
}
