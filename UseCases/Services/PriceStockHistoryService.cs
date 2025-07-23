using DiamondTransaction.UseCases.Models;
using DiamondTransaction.UseCases.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DiamondTransaction.UseCases.Services
{
    public class PriceStockHistoryService
    {
        private readonly IPriceStockHistoryRepository _repository;
        public PriceStockHistoryService(IPriceStockHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<string>> GetExistingLotNamesAsync()
        {
            return await _repository.GetExistingLotNamesAsync();
        }

        public async Task<IEnumerable<PriceStockHistoryDto>> GetByLotNameAsync(string lotName)
        {
            return await _repository.GetByLotNameAsync(lotName);
        }
    }
}
