using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using DiamondTransaction.UseCases.Interfaces;

namespace DiamondTransaction.DataAccess
{
    public class PriceStockHistoryDataAccess : IPriceStockHistoryRepository
    {
        private readonly string _connectionString;
        public PriceStockHistoryDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<string>> GetExistingLotNamesAsync()
        {
            const string sql = @"
            SELECT DISTINCT LotName 
            FROM THXADiamondPriceStockHistory 
            WHERE LotName IS NOT NULL AND LTRIM(RTRIM(LotName)) <> ''
            ORDER BY LotName ASC";

            using (var connection = new SqlConnection(_connectionString))
            {
                var lotNames = await connection.QueryAsync<string>(sql);
                return lotNames.ToList();
            }
        }


        public async Task<IEnumerable<PriceStockHistoryDto>> GetByLotNameAsync(string lotName)
        {
            const string sql = @"
            SELECT * 
            FROM THXADiamondPriceStockHistory
            WHERE LotName = @LotName
            ORDER BY TrsDate DESC";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<PriceStockHistoryDto>(sql, new { LotName = lotName });
            }
        }

    }
}
