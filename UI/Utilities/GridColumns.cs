using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction
{
    public static class GridColumns
    {
      
        public static  List<string> DocHeaderColumn = new List<string>
        {
            "DocTypeDesc", "DocID", "DocCode", "DocAccountRefNo", "DocSubType", "DocStatus",
            "DocDate","AccountCode", "AccountName", "Currency", "Weight", "DocLinesTotalPrice",
            "DocDiscountPrice", "DocSubTotalPrice", "DocTaxPrice", "DocGrandTotalPrice"
        };

        public static Dictionary<int, List<string>> DocLineColumnByDocType = new Dictionary<int, List<string>>()
        {
            { 1001, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "LabAccountName", "CertificateNo", "OutList", "OutListMainDiscount", "OutMainPrice", "OutMainTotalPrice", "DocPriceGross", "TotalDocPriceGross", "LoanDays", "CertificateID", "DocLineStatus" , "ParcelOrStone", "HoldingType" } },
            { 1002, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "LabAccountName", "CertificateNo", "OutList", "OutListMainDiscount", "OutMainPrice", "OutMainTotalPrice", "DocPriceGross", "TotalDocPriceGross", "ParentLotID", "LoanDays", "CertificateID", "DocLineStatus","ParcelOrStone", "HoldingType" } },
            { 1003, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "LabAccountName", "CertificateNo", "OutList", "OutListMainDiscount", "OutMainPrice", "OutMainTotalPrice", "DocPriceGross", "TotalDocPriceGross", "CertificateID", "DocLineStatus","ParcelOrStone", "HoldingType" } },
            { 1005, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "LabAccountName", "CertificateNo", "OutList", "OutListMainDiscount", "OutMainPrice", "OutMainTotalPrice", "DocPriceGross", "TotalDocPriceGross", "ParentLotID", "CertificateID", "DocLineStatus", "ParcelOrStone", "HoldingType" } },
            { 1006, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "Cost", "TotalCost", "DocPrice", "TotalDocPrice", "CertificateID", "DocLineStatus", "ParcelOrStone", "HoldingType" } },
            { 1007, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "Cost", "TotalCost", "DocPrice", "TotalDocPrice", "CertificateID", "ParentLotID", "DocLineStatus","ParcelOrStone", "HoldingType" } },
            { 1009, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "Cost", "TotalCost", "DocPrice", "TotalDocPrice", "CertificateID", "DocLineStatus", "ParcelOrStone", "HoldingType" } },
            { 1010, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "Cost", "TotalCost", "DocPrice", "TotalDocPrice", "CertificateID", "ParentLotID", "DocLineStatus", "ParcelOrStone", "HoldingType" } }
        };


        public static Dictionary<int, List<string>> WorkingLineColumnByDocType = new Dictionary<int, List<string>>() 
        {
            { 1001, new List<string> { "LotID", "LotName", "Quantity", "Weight", "OutMainPrice", "OutMainTotalPrice", "OutListMainDiscount", "OutList", "OutSecPrice", "OutSecTotalPrice",  } },
            { 1002, new List<string> { "LotID", "LotName", "Quantity", "Weight", "OutMainPrice", "OutMainTotalPrice", "OutListMainDiscount", "OutList", "OutSecPrice", "OutSecTotalPrice",  } },
            { 1003, new List<string> { "LotID", "LotName", "Quantity", "Weight", "OutMainPrice", "OutMainTotalPrice", "OutListMainDiscount", "OutList", "OutSecPrice", "OutSecTotalPrice",  } },
            { 1005, new List<string> { "LotID", "LotName", "Quantity", "Weight", "OutMainPrice", "OutMainTotalPrice", "OutListMainDiscount", "OutList", "OutSecPrice", "OutSecTotalPrice",  } },
            { 1006, new List<string> { "LotID", "LotName", "Quantity", "Weight", "List", "ListCostDiscount", "Cost", "TotalCost", "SecCost", "SecTotalCost",  } },
            { 1007, new List<string> { "LotID", "LotName", "Quantity", "Weight", "List", "ListCostDiscount", "Cost", "TotalCost", "SecCost", "SecTotalCost",  } },
            { 1009, new List<string> { "LotID", "LotName", "Quantity", "Weight", "List", "ListCostDiscount", "Cost", "TotalCost", "SecCost", "SecTotalCost",  } },
            { 1010, new List<string> { "LotID", "LotName", "Quantity", "Weight", "List", "ListCostDiscount", "Cost", "TotalCost", "SecCost", "SecTotalCost",  } }
        };

        public static Dictionary<int, List<string>> WorkingLineHiddenColumnsByDocType = new Dictionary<int, List<string>>()
        {
            { 1001, new List<string> {   } },
            { 1002, new List<string> {   } },
            { 1003, new List<string> {   } },
            { 1005, new List<string> {   } },
            { 1006, new List<string> {   } },
            { 1007, new List<string> {   } },
            { 1009, new List<string> {   } },
            { 1010, new List<string> {   } }
        };

    }
}
