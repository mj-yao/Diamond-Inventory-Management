using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Services
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
        public int InsertDocLine(DocLineDto docLine)
        {
            return _repository.InsertDocLine(docLine);
        }
        public bool UpdateDocLine(DocLineDto docLine)
        {
            return _repository.UpdateDocLine(docLine);
        }
    }
}
