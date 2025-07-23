using DiamondTransaction.Domain.Interfaces;
using DiamondTransaction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases
{
    public class DocLineService
    {
        private readonly IDocLineRepository _repository;
        public DocLineService(IDocLineRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DocLineDto>> GetDiamondDocLinesAsync(int docTypeID, int docID)
        {
            return await _repository.GetDiamondDocLinesAsync(docTypeID, docID);
        }
    }
}
