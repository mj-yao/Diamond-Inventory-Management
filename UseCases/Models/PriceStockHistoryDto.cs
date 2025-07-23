using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Models
{
    public class PriceStockHistoryDto
    {
        public int TrsID { get; set; }
        public int TrsTypeID { get; set; }
        public string TrsTypeDesc { get; set; }
        public DateTime? TrsDate { get; set; }
        public string ReferenceCode { get; set; }
        public int ReferenceSrNo { get; set; }
        public int LotID { get; set; }
        public int ItemID { get; set; }
        public string LotName { get; set; }
        public string ParcelOrStone { get; set; }
        public string NaturalOrLabGrown { get; set; }
        public int CertificateID { get; set; }
        public string CertificateLabName { get; set; }
        public string CertificateType { get; set; }
        public string CertificateNo { get; set; }
        public string Shape { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public decimal OriginalWeight { get; set; }
        public decimal WeightIn { get; set; }
        public decimal WeightOut { get; set; }
        public decimal WeightBalance { get; set; }
        public decimal WeightLoss { get; set; }
        public decimal ScrapWeight { get; set; }
        public decimal Cost { get; set; }
        public decimal TrsTotalCost { get; set; }
        public decimal ParcelAvgCost { get; set; }
        public decimal ListCostDiscount { get; set; }
        public decimal List { get; set; }
        public decimal TotalList { get; set; }
        public decimal ListSaleDiscount { get; set; }
        public decimal Sale { get; set; }
        public decimal TotalSale { get; set; }
        public decimal GBPSale { get; set; }
        public decimal GBPTotalSale { get; set; }
        public decimal OutList { get; set; }
        public decimal ListOutDiscount { get; set; }
        public decimal OutPrice { get; set; }
        public decimal OutTotalPrice { get; set; }
        public decimal GBPOutPrice { get; set; }
        public decimal GBPOutTotalPrice { get; set; }
        public decimal ExchangeRate { get; set; }
        public string LocationAccountCode { get; set; }
        public string LocationAccountName { get; set; }
        public string VendorAccountCode { get; set; }
        public string VendorAccountName { get; set; }
        public string Remark { get; set; }
        public string Created_By { get; set; }
        public decimal TotalCost { get; set; }
        public int? TrsDocID { get; set; }
        public int? TrsDocLine { get; set; }
        public int? DocTypeID { get; set; }
        public int? DocID { get; set; }
        public int? DocLine { get; set; }
        public DateTime? DocDate { get; set; }
        public int? SourceDocTypeID { get; set; }
        public int? SourceDocID { get; set; }
        public int? SourceDocLine { get; set; }
        public int? TrsUnionID { get; set; }
        public string DocTypeDesc { get; set; }
    }

}
