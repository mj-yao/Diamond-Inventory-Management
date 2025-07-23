using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondTransaction.Domain.Entities;
using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;

namespace DiamondTransaction.UseCases.Services
{
    public class DiamondDocService
    {
        private readonly IDiamondDocRepository _repository;
        public DiamondDocService(IDiamondDocRepository repository)
        {
            _repository = repository;
        }


        public void LoadDocTypes() {
            _repository.LoadDocTypes();
        }
        public string GetDocTypeIDByDocTypeDesc(string description)
        {
            return _repository.GetDocTypeIDByDocTypeDesc(description);
        }
        public string GetDocTypeDescById(int docTypeId)
        {
            return _repository.GetDocTypeDescById(docTypeId);
        }
        public List<DocType> GetAllDocTypes()
        {
            return _repository.GetAllDocTypes();
        }
        public List<int> GetSupplierDocTypeIDs()
        {
            return _repository.GetSupplierDocTypeIDs();
        }
        public List<string> GetDocSubTypesByDocType(string docTypeDesc)
        {
            return _repository.GetDocSubTypesByDocType(docTypeDesc);
        }
        public DocHeaderMaxInfo AccessDocHeaderMaxDocIDAndDocCodeFor(string docTypeDesc)
        {
            return _repository.AccessDocHeaderMaxDocIDAndDocCodeFor(docTypeDesc);
        }
        public DiamondLotMaxID AccessDiamondLotMaxItemLotIDAndCertificateID()
        {
            return _repository.AccessDiamondLotMaxItemLotIDAndCertificateID();
        }
        public ExchangeRate GetLatestExchangeRate()
        { 
            return _repository.GetLatestExchangeRate();
        }

        public List<string> GetParcelGradingScale(string gradeType) 
        {
            return _repository.GetParcelGradingScale(gradeType);
        }

        public ParcelGrades GetAllParcelGradingScales() 
        {
            return _repository.GetAllParcelGradingScales();
        }



    }
}
