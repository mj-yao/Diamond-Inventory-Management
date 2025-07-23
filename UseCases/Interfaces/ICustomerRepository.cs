using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Interfaces
{
    public interface ICustomerRepository
    {
        List<Customer> GetAllCustomers();
        List<CustomerBranch> GetBranchesByCustomer(string customerCode);
    }
}
