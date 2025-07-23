using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Services
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
        public int InsertDiamondDocHeader(DocHeaderDto docHeader)
        {
            return _repository.InsertDiamondDocHeader(docHeader);
        }
        public bool UpdateDiamondDocHeader(DocHeaderDto docHeader)
        {
            return _repository.UpdateDiamondDocHeader(docHeader);
        }
    }
}
