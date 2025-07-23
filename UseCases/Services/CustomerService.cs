using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Services
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
