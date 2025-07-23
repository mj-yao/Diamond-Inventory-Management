using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction
{
    public static class ColumnManager
    {
        public static Dictionary<int, List<string>> DocDetailColumnByDocType = new Dictionary<int, List<string>>()
        {
            { 1001, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "LabAccountName", "CertificateNo", "OutList", "OutListMainDiscount", "OutMainPrice", "OutTotalMainPrice", "DocPriceGross", "TotalDocPriceGross", "LoanDays", "CertificateID", "LineStatus" , "ParcelOrStone", "HoldingType" } },
            { 1002, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "LabAccountName", "CertificateNo", "OutList", "OutListMainDiscount", "OutMainPrice", "OutTotalMainPrice", "DocPriceGross", "TotalDocPriceGross", "ParentLotID", "LoanDays", "CertificateID", "LineStatus","ParcelOrStone", "HoldingType" } },
            { 1003, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "LabAccountName", "CertificateNo", "OutList", "OutListMainDiscount", "OutMainPrice", "OutTotalMainPrice", "DocPriceGross", "TotalDocPriceGross", "CertificateID", "LineStatus","ParcelOrStone", "HoldingType" } },
            { 1005, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "LabAccountName", "CertificateNo", "OutList", "OutListMainDiscount", "OutMainPrice", "OutTotalMainPrice", "DocPriceGross", "TotalDocPriceGross", "ParentLotID", "CertificateID", "LineStatus", "ParcelOrStone", "HoldingType" } },
            { 1006, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "Cost", "TotalCost", "DocPrice", "TotalDocPrice", "CertificateID", "LineStatus", "ParcelOrStone", "HoldingType" } },
            { 1007, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "Cost", "TotalCost", "DocPrice", "TotalDocPrice", "CertificateID", "ParentLotID", "LineStatus","ParcelOrStone", "HoldingType" } },
            { 1009, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "Cost", "TotalCost", "DocPrice", "TotalDocPrice", "CertificateID", "LineStatus", "ParcelOrStone", "HoldingType" } },
            { 1010, new List<string> { "LineID", "ItemID", "LotID", "LotName", "Quantity", "Weight", "Shape", "Size", "Color", "Clarity", "Cost", "TotalCost", "DocPrice", "TotalDocPrice", "CertificateID", "ParentLotID", "LineStatus", "ParcelOrStone", "HoldingType" } }
        };


        /*
        public static Dictionary<int, List<string>> DocDetailColumnByDocType = new Dictionary<int, List<string>>()
        {           
            { 1001, new List<string> { "LotID", "LotName", "Weight", "Shape", "Size", "Color", "Clarity", "OutList($)", "OutDisc(%)","OutGross", "Out T.Gross", "DocPriceGross", "T. DocPriceGross", "CertificateID" } },
            { 1002, new List<string> { "LotID", "LotName", "Weight", "Shape", "Size", "Color", "Clarity", "OutList($)", "OutDisc(%)","OutGross", "Out T.Gross", "Doc Price (GBP)", "Total Doc Price (GBP)", "DocPriceGross", "T. DocPriceGross", "ParentLotID", "CertificateID" } },
            { 1003, new List<string> { "LotID", "LotName", "Weight", "Shape", "Size", "Metal", "Color", "Clarity", "LabAccountName", "Certificate No", "Out List ($)", "Out Disc %", "Out Gross", "Out T. Gross", "DocPriceGross", "T. Doc Price Gross", "CertificateID" } },
            { 1005, new List<string> { "LotID", "LotName", "Remark", "Weight", "Shape", "Size", "Color", "Clarity", "OutGross", "Out T. Gross", "DocPrice(GBP)", "Doc Price Gross", "Total Doc Price (GBP)", "T. DocPriceGross", "ParentLotID", "CertificateID" } },
            { 1006, new List<string> { "LotID", "LotName", "Weight", "Shape", "Size", "Color", "Clarity", "DocPrice($)", "TotalDocPrice($)","LineStatus", "CertificateID" } },
            { 1007, new List<string> { "LotID", "LotName", "Weight", "Shape", "Size", "Color", "Clarity", "DocPrice($)", "TotalDocPrice($)","LineStatus", "CertificateID" } },
            { 1009, new List<string> { "LotID", "LotName", "Item",  "Weight", "Shape", "Size", "Color", "Clarity", "DocPrice", "Total Doc Price", "MainPrice ($)", "Total Main Price ($)", "Out Price ($)", "GBP Price (GBP)", "Out T. Price ($)", "GBPTotalPrice (GBP)", "Department Account", "DocPriceGross", "DocLine Status", "Parent Lot ID", "T. Doc Price Gross", "Cost Price($)", "Hard Cost ($)",  "Total Hard Cost ($)", "% Hard Cost", "Current Weight",  "Doc T. Discount", "Remark", "Total CostPrice ($)", "% Cost" } },
            { 1010, new List<string> { "LotID", "LotName", "Weight", "Shape", "Size", "Color", "Clarity", "LabAccountName", "CertificateNo", "Out List ($)", "Out Disc %", "Out Price ($)", "Out T. Price ($)", "DocPrice", "DocPriceGross", "TotalDocPrice", "CertificateID", "T. DocPriceGross", "ParentLotID", "CostPrice ($)", "Hard Cost ($)", "Total Hard Cost ($)", "% Hard Cost", "Current Weight",  "Doc T. Discount", "Remark", "Total Cost Price ($)", "% Cost"  } }
        };
        */

        public static Dictionary<int, List<string>> WorkingLineColumnByDocType = new Dictionary<int, List<string>>()
        {
            { 1001, new List<string> { "LotID", "LotName", "Quantity", "Weight", "OutGross($)", "Out T.Gross($)", "OutDisc(%)", "OutList", "OutGross(£)", "Out T.Gross(£)", "LoanDays", "Shape", "Size", "Color", "Clarity", "ParcelOrStone", "HoldingType", "ItemID" } },
            { 1002, new List<string> { "LotID", "LotName", "Quantity", "Weight", "OutGross($)", "Out T.Gross($)", "OutDisc(%)", "OutList", "OutGross(£)", "Out T.Gross(£)", "LoanDays", "Shape", "Size", "Color", "Clarity", "ParcelOrStone", "HoldingType", "ItemID" } },
            { 1003, new List<string> { "LotID", "LotName", "Quantity", "Weight", "OutGross($)", "Out T.Gross($)", "OutDisc(%)", "OutList", "OutGross(£)", "Out T.Gross(£)", "Shape", "Size", "Color", "Clarity", "ParcelOrStone", "HoldingType", "ItemID" } },
            { 1005, new List<string> { "LotID", "LotName", "Quantity", "Weight", "OutGross($)", "Out T.Gross($)", "OutDisc(%)", "OutList", "OutGross(£)", "Out T.Gross(£)", "Shape", "Size", "Color", "Clarity", "ParcelOrStone", "HoldingType", "ItemID" } },
            { 1006, new List<string> { "LotID", "LotName", "Quantity", "Weight", "List", "%(-)Cost", "Cost", "TotalCost", "Cost(£)", "TotalCost(£)", "Shape", "Size", "Color", "Clarity", "ParcelOrStone", "HoldingType", "ItemID" } },
            { 1007, new List<string> { "LotID", "LotName", "Quantity", "Weight", "List", "%(-)Cost", "Cost", "TotalCost", "Cost(£)", "TotalCost(£)", "Shape", "Size", "Color", "Clarity", "ParcelOrStone", "HoldingType", "ItemID" } },
            { 1009, new List<string> { "LotID", "LotName", "Quantity", "Weight", "List", "%(-)Cost", "Cost", "TotalCost", "Cost(£)", "TotalCost(£)", "Shape", "Size", "Color", "Clarity", "ParcelOrStone", "HoldingType", "ItemID" } },
            { 1010, new List<string> { "LotID", "LotName", "Quantity", "Weight", "List", "%(-)Cost", "Cost", "TotalCost", "Cost(£)", "TotalCost(£)", "Shape", "Size", "Color", "Clarity", "ParcelOrStone", "HoldingType", "ItemID" } }
        };

        public static Dictionary<int, List<string>> WorkingLineHiddenColumnsByDocType = new Dictionary<int, List<string>>()
        {
            { 1001, new List<string> { "LineStatus", "HoldingType" } },
            { 1002, new List<string> { "ParentLotID", "OutListMainDiscount" } },
            { 1003, new List<string> { "CertificateNo" } },
            { 1005, new List<string> { "OutTotalMainPrice", "DocPriceGross" } },
            { 1006, new List<string> { "LotName", "Quantity", "Shape", "Size", "Color", "Clarity", "ParcelOrStone", "HoldingType", "ItemID" } },
            { 1007, new List<string> { "LotID", "LotName", "Quantity", "Weight", "List", "%(-)Cost", "Cost", "TotalCost", "Cost(£)", "TotalCost(£)", "Shape", "Size", "Color", "Clarity", "ParcelOrStone", "HoldingType", "ItemID" } },
            { 1009, new List<string> { "TotalDocPrice", "CertificateID" } },
            { 1010, new List<string> { "HoldingType", "Size" } }
        };

    }
}
