using DiamondTransaction.Domain.Interfaces;
using DiamondTransaction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases
{
    public class SupplierService
    {
        private readonly ISupplierRepository _repository;
        public SupplierService(ISupplierRepository repository)
        {
            _repository = repository;
        }

        public List<Supplier> GetAllSuppliers()
        {
            return _repository.GetAllSuppliers();
        }


    }
}
