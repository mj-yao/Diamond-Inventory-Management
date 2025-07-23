using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;

namespace DiamondTransaction.DataAccess
{
    public class CustomerDataAccess : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerDataAccess(string connectionString)
        {
            _connectionString = connectionString;         
        }

        public List<Customer> GetAllCustomers()
        {
            string query = "SELECT CustomerCode, CustomerName, PaymentDays FROM THSLCustomer";
            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, "Access Customer");

            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                return sqlResult.AsEnumerable()
                    .Select(row => new Customer
                    {
                        CustomerCode = row.Field<string>("CustomerCode"),
                        CustomerName = row.Field<string>("CustomerName"),
                        PaymentDays = row.Field<decimal>("PaymentDays")
                    }).ToList();
            }
            return new List<Customer>();
        }

        public List<CustomerBranch> GetBranchesByCustomer(string customerCode)
        {
            string query = "SELECT CustomerCode, BranchCode FROM THSLCustomerBranch WHERE CustomerCode = @CustomerCode";
            var parameters = new Dictionary<string, object>
            {
                { "@CustomerCode", customerCode }
            };

            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, "Access CustomerBranch", parameters);

            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                return sqlResult.AsEnumerable()
                    .Select(row => new CustomerBranch
                    {
                        CustomerCode = row.Field<string>("CustomerCode"),
                        BranchCode = row.Field<string>("BranchCode")
                    }).ToList();
            }

            return new List<CustomerBranch>();
        }


    }
}
