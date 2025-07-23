using DiamondTransaction.Domain.Interfaces;
using DiamondTransaction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases
{
    public class CustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public List<Customer> GetAllCustomers()
        {
            return _repository.GetAllCustomers();
        }

        public List<CustomerBranch> GetBranchesByCustomer(string customerCode)
        {
            return _repository.GetBranchesByCustomer(customerCode);
        }
    }
}
