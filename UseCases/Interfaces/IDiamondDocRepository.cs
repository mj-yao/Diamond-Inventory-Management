using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondTransaction.Domain.Entities;
using DiamondTransaction.UseCases.Models;

namespace DiamondTransaction.UseCases.Interfaces
{
    public interface IDiamondDocRepository
    {
        void LoadDocTypes();
        string GetDocTypeIDByDocTypeDesc(string description);
        string GetDocTypeDescById(int docTypeId);
        List<DocType> GetAllDocTypes();
        List<int> GetSupplierDocTypeIDs();
        List<string> GetDocSubTypesByDocType(string docTypeDesc);
        DocHeaderMaxInfo AccessDocHeaderMaxDocIDAndDocCodeFor(string docTypeDesc);
        DiamondLotMaxID AccessDiamondLotMaxItemLotIDAndCertificateID();
        ExchangeRate GetLatestExchangeRate();

        List<string> GetParcelGradingScale(string gradeType);

        ParcelGrades GetAllParcelGradingScales();
    }
}
