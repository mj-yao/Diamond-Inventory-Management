using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Services
{
    public class DiamondLotService
    {
        private readonly IDiamondLotRepository _repository;
        public DiamondLotService(IDiamondLotRepository repository)
        {
            _repository = repository;
        }
        public async Task<DiamondLotDto> GetDiamondLotAsync(string identifier, string searchType)
        {
            return await _repository.GetDiamondLotAsync(identifier, searchType);
        }
        public async Task<int> UpsertDiamondLotAsync(DiamondLotDto dto)
        {
            return await _repository.UpsertDiamondLotAsync(dto);
        }
        public async Task<int> InsertDiamondLotAsync(DiamondLotDto dto)
        {
            return await _repository.InsertDiamondLotAsync(dto);
        }
        public async Task<bool> UpdateDiamondLotAsync(DiamondLotDto dto)
        {
            return await _repository.UpdateDiamondLotAsync(dto);
        }



    }
}
