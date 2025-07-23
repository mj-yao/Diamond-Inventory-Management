using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Models
{
    public class DocHeaderDto
    {
        public int DocHeaderID { get; set; }
        public int DocTypeID { get; set; }
        public string DocTypeDesc { get; set; }
        public int DocID { get; set; }
        public string DocCode { get; set; }
        public string DocAccountRefNo { get; set; }
        public string DocSubType { get; set; }
        public string DocStatus { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? RefDocDate { get; set; }
        public DateTime? RefDocDate2 { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DateTime_Modified { get; set; }
        public string AccountCode { get; set; }
        public string AccountBranchCode { get; set; }
        public string AccountName { get; set; }
        public string AccountLongName { get; set; }
        public string Currency { get; set; }
        public string VatCode { get; set; }
        public decimal VatRate { get; set; }
        public int PaymentTerm { get; set; }
        public int Loandays { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string InternalRemark { get; set; }
        public string UserID { get; set; }
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
        public decimal MainLinesTotalPrice { get; set; }
        public decimal SecLinesTotalPrice { get; set; }
        public decimal DocLinesTotalPrice { get; set; }
        public decimal MainDiscountPrice { get; set; }
        public decimal SecDiscountPrice { get; set; }
        public decimal DocDiscountPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public string DiscountRemark { get; set; }
        public decimal MainSubTotalPrice { get; set; }
        public decimal SecSubTotalPrice { get; set; }
        public decimal DocSubTotalPrice { get; set; }
        public decimal MainAdditionalPrice { get; set; }
        public decimal SecAdditionalPrice { get; set; }
        public decimal DocAdditionalPrice { get; set; }
        public string AdditionalRemark { get; set; }
        public decimal MainTaxPrice { get; set; }
        public decimal SecTaxPrice { get; set; }
        public decimal DocTaxPrice { get; set; }
        public decimal MainGrandTotalPrice { get; set; }
        public decimal SecGrandTotalPrice { get; set; }
        public decimal DocGrandTotalPrice { get; set; }
        public decimal? RateDocToMain { get; set; }
        public decimal? RateDocToSec { get; set; }
    }
}
