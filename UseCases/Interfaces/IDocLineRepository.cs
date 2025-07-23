using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Interfaces
{
    public interface IDocLineRepository
    {
        Task<IEnumerable<DocLineDto>> GetDiamondDocLinesAsync(int docTypeID, int docID);
        int InsertDocLine(DocLineDto docLine);
        bool UpdateDocLine(DocLineDto docLine);
    }
}
