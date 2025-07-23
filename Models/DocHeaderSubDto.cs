using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.Models
{
    public class DocHeaderSubDto
    {
        public string DocTypeDesc { get; set; }
        public int DocTypeID { get; set; }
        public int DocID { get; set; }
        public string DocCode { get; set; }
        public string DocSubType { get; set; }
        public DateTime DocDate { get; set; }
        public string DocStatus { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string AccountBranchCode { get; set; }
        public int CustomerPaymentTerm { get; set; }
        public DateTime DueDate { get; set; }
        public string DocAccountRefNo { get; set; }       
        public DateTime RefDocDate { get; set; }
        public string RefRemark { get; set; }

        public string Currency { get; set; }
        public decimal USDRate { get; set; }
        public decimal GBPRate { get; set; }

        public decimal Discount { get; set; }
        public decimal Additional { get; set; }
        public decimal Tax { get; set; }


    }
}
