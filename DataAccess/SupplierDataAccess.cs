using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DiamondTransaction.UseCases.Interfaces;

namespace DiamondTransaction.DataAccess
{
    public class SupplierDataAccess: ISupplierRepository
    {
            
        private readonly string _connectionString;

        public SupplierDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Supplier> GetAllSuppliers()
        {
            string query = "SELECT SupplierCode, SupplierName FROM THPUSupplier";
            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, "Access Supplier");

            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                return sqlResult.AsEnumerable()
                    .Select(row => new Supplier
                    {
                        SupplierCode = row.Field<string>("SupplierCode"),
                        SupplierName = row.Field<string>("SupplierName")
                    }).ToList();
            }
            return new List<Supplier>();
        }
    }

}
