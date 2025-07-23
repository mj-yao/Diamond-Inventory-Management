using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Models
{
    public class DocLineDto
    {
        public int DocHeaderID { get; set; }
        public int DocLineID { get; set; }
        public int DocTypeID { get; set; }
        public string DocTypeDesc { get; set; }
        public int DocID { get; set; }
        public int DocLine { get; set; }
        public string DocLineStatus { get; set; }
        public int LotID { get; set; }
        public int ParentLotID { get; set; }
        public int ItemID { get; set; }
        public string LotName { get; set; }
        public string ParcelOrStone { get; set; }
        public string HoldingType { get; set; }
        public int CertificateID { get; set; }
        public string Shape { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
        public decimal? WeightLoss { get; set; } = 0m;
        public decimal Cost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal SecCost { get; set; }
        public decimal SecTotalCost { get; set; }
        public decimal Sale { get; set; }
        public decimal TotalSale { get; set; }
        public decimal SecSale { get; set; }
        public decimal SecTotalSale { get; set; }
        public decimal? Additional { get; set; } = 0m;
        public decimal? TotalAdditional { get; set; } = 0m;
        public decimal MainPrice { get; set; }
        public decimal TotalMainPrice { get; set; }
        public decimal SecPrice { get; set; }
        public decimal TotalSecPrice { get; set; }
        public decimal DocPrice { get; set; }
        public decimal TotalDocPrice { get; set; }
        public decimal RateDocToMain { get; set; }
        public decimal RateDocToSec { get; set; }
        public decimal List { get; set; }
        public decimal TotalList { get; set; }
        public decimal? ListCostDiscount { get; set; } = 0m;
        public decimal? ListSaleDiscount { get; set; } = 0m;
        public decimal? ListMainDiscount { get; set; } = 0m;
        public string Remark { get; set; }
        public decimal OutList { get; set; }
        public decimal OutTotalList { get; set; }
        public decimal OutMainPrice { get; set; }
        public decimal OutMainTotalPrice { get; set; }
        public decimal? OutListMainDiscount { get; set; } = 0m;
        public decimal OutSecPrice { get; set; }
        public decimal OutSecTotalPrice { get; set; }
        public decimal ReturnWeight { get; set; }
        public decimal ReturnTotalCost { get; set; }
        public decimal ReturnTotalSale { get; set; }
        public decimal InvoicedWeight { get; set; }
        public decimal InvoicedTotalCost { get; set; }
        public decimal InvoicedTotalSale { get; set; }
        public decimal DocPriceGross { get; set; }
        public decimal MainPriceGross { get; set; }
        public decimal SecPriceGross { get; set; }      
        public decimal TotalDocPriceGross { get; set; }
        public decimal TotalMainPriceGross { get; set; }
        public decimal TotalSecPriceGross { get; set; }
        public decimal? DocPriceGrossWithTax { get; set; } 
        public decimal? TotalDocPriceGrossWithTax { get; set; }
        public decimal? DocPriceWithTax { get; set; }
        public decimal? TotalDocPriceWithTax { get; set; }
        public decimal? DocTotalDiscountWithTax { get; set; }
        public decimal? MarkupPercent { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DocTotalDiscountAmount { get; set; }
        public decimal? MainTotalDiscountAmount { get; set; }
        public decimal? SecTotalDiscountAmount { get; set; }
        public int TrsUnionID { get; set; }
        public int? SourceDocTypeID { get; set; }
        public string SourceDocTypeDesc { get; set; }
        public int SourceDocID { get; set; }
        public int SourceDocLine { get; set; }
        public DateTime? DateTime_Created { get; set; }
        public DateTime? DateTime_Modified { get; set; }
        public string UserID { get; set; }
        public string SourceDocCode { get; set; }

    }

}
