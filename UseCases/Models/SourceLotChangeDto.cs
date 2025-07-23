using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Models
{
    public class SourceLotChangeDto
    {
        public int LotID { get; set; }
        public int? SourceQuantity { get; set; }
        public decimal SourceWeight { get; set; }


        // In (Purchasing)
        public decimal? SourceList { get; set; }
        public decimal? SourceListCostDiscount { get; set; }
        public decimal? SourceCost { get; set; }
        public decimal? SourceTotalCost { get; set; }
        public decimal? SourceSecCost { get; set; }
        public decimal? SourceSecTotalCost { get; set; }


        public int? FinalQuantity { get; set; }
        public decimal FinalWeight { get; set; }

        // In (Purchasing)
        public decimal? FinalCost { get; set; }
        public decimal? FinalTotalCost { get; set; }
        public decimal? FinalSecCost { get; set; }
        public decimal? FinalSecTotalCost { get; set; }

    }
}
