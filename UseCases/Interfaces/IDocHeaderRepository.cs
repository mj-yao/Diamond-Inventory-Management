using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Interfaces
{
    public interface IDocHeaderRepository
    {
        Task<IEnumerable<DocHeaderDto>> GetAllDiamondDocHeadersAsync();
        int InsertDiamondDocHeader(DocHeaderDto docHeader);
        bool UpdateDiamondDocHeader(DocHeaderDto docHeader);

    }
}
