using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Models
{
    public class DiamondLotDto
    {
        public int LotID { get; set; }
        public string LotName { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemDescription1 { get; set; }
        public string ParcelOrStone { get; set; }
        public string HoldingType { get; set; }
        public string Status { get; set; }
        public string StockStatus { get; set; }
        public decimal Weight { get; set; }
        public string Shape { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Cut { get; set; }
        public string Polish { get; set; }
        public string Symmetry { get; set; }
        public string Fluorescence { get; set; }
        public string Inscription { get; set; }
        public int CertificateID { get; set; }
        public string CertificateLabName { get; set; }
        public string CertificateType { get; set; }
        public string CertificateNo { get; set; }
        public DateTime? CertificateDate { get; set; }
        public decimal Cost { get; set; }
        public decimal TotalCost { get; set; }
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
        public string LocationAccountCode { get; set; }
        public string LocationAccountName { get; set; }
        public string VendorAccountCode { get; set; }
        public string VendorAccountName { get; set; }
        public decimal WeightLoss { get; set; }
        public decimal ScrapWeight { get; set; }
        public string Remark { get; set; }
        public string Created_By { get; set; }
        public DateTime? DateTime_Created { get; set; }
        public string Modified_By { get; set; }
        public DateTime? LastStockStatusUpdate { get; set; }
        public string ReferenceDocCode { get; set; }
        public int LastTrsID { get; set; }
        public DateTime? LastTrsDate { get; set; }
        public int LastTrsTypeID { get; set; }
        public string LastTrsTypeDesc { get; set; }

        public decimal? ParcelAvgCost { get; set; }  // From PriceStockHistory 
    }
}
