using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondTransaction.UseCases.Models;

namespace DiamondTransaction.UseCases.Services
{
    public class DocCreationService
    {
        private readonly DiamondDocService _diamondDocService;

        private readonly Dictionary<string, string> _prefixMap = new Dictionary<string, string>()
        {
            { "MEMO OUT", "MO" },
            { "MEMO OUT RETURN", "OR" },
            { "INVOICE LOCAL", "SL" },
            { "INVOICE RETURN", "SR" },
            { "PURCHASE NOTE", "GI" },
            { "PURCHASE NOTE RETURN", "DN" },
            { "MEMO IN", "MI" },
            { "MEMO IN RETURN", "IR" }
        };

        public DocCreationService(DiamondDocService diamondDocService)
        {
            _diamondDocService = diamondDocService;
        }

        public DocHeaderSubDto PrepareNewSupplierDoc(string docTypeDesc)
        {
            var docHeaderMaxInfo = _diamondDocService.AccessDocHeaderMaxDocIDAndDocCodeFor(docTypeDesc);
            var diamondLotMaxInfo = _diamondDocService.AccessDiamondLotMaxItemLotIDAndCertificateID();

            return new DocHeaderSubDto
            {
                DocTypeDesc = docTypeDesc,
                DocStatus = "A",
                Currency = "USD",
                DocID = docHeaderMaxInfo.MaxDocID + 1,
                DocCode = GenerateNewDocCode(docTypeDesc, docHeaderMaxInfo.MaxDocCode),
                DocSubType = "",
                AccountBranchCode = "",
                Discount = 0.00m,
                Additional = 0.00m,
                Tax = 0.00m

            };
        }

        public DocHeaderSubDto PrepareNewCustomerDoc(string docTypeDesc)
        {
            var docHeaderMaxInfo = _diamondDocService.AccessDocHeaderMaxDocIDAndDocCodeFor(docTypeDesc);
            var diamondLotMaxInfo = _diamondDocService.AccessDiamondLotMaxItemLotIDAndCertificateID();

            return new DocHeaderSubDto
            {
                DocTypeDesc = docTypeDesc,
                DocStatus = "A",
                Currency = "GBP",
                DocID = docHeaderMaxInfo.MaxDocID + 1,
                DocCode = GenerateNewDocCode(docTypeDesc, docHeaderMaxInfo.MaxDocCode),
                DocSubType = "",
                AccountBranchCode = "",
                Discount = 0.00m,
                Additional = 0.00m,
                Tax = 0.00m

            };
        }

     
        private string GenerateNewDocCode(string docTypeDesc, string maxDocCode)
        {
            if (!_prefixMap.TryGetValue(docTypeDesc, out var prefix))
                throw new ArgumentException($"Unknown document type: {docTypeDesc}");

            string currentYearSuffix = DateTime.Now.Year.ToString().Substring(2, 2); // Get last two digits of the current year

            //If the maxDocCode is empty or does not start with the prefix, start from the beginning
            if (string.IsNullOrEmpty(maxDocCode) || !maxDocCode.StartsWith(prefix))
            {

                return $"{prefix}{currentYearSuffix}000001";
            }

            string yearPart = maxDocCode.Substring(2, 2);
            string numberPart = maxDocCode.Substring(4);

            int nextNumber;
            if (yearPart == currentYearSuffix)
            {
                nextNumber = int.Parse(numberPart) + 1;
            }
            else
            {
                nextNumber = 1;
            }

            return $"{prefix}{currentYearSuffix}{nextNumber:D6}";

        }
    }
}
