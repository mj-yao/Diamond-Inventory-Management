using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Interfaces
{
    public interface IDiamondLotRepository
    {
        Task<DiamondLotDto> GetDiamondLotAsync(string identifier, string searchType);
        Task<int> UpsertDiamondLotAsync(DiamondLotDto dto);
        Task<int> InsertDiamondLotAsync(DiamondLotDto dto);
        Task<bool> UpdateDiamondLotAsync(DiamondLotDto dto);

    }
}
