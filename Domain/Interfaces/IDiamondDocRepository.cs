using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondTransaction.Domain.Entities;
using DiamondTransaction.Models;

namespace DiamondTransaction.Domain.Interfaces
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
        DiamondLotMaxInfo AccessDiamondLotMaxItemLotIDAndCertificateID();
        ExchangeRate GetLatestExchangeRate();
        List<string> GetExistingLotNames();

        List<string> GetParcelGradingScale(string gradeType);

        ParcelGrades GetAllParcelGradingScales();
    }
}
