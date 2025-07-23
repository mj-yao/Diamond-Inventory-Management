using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Models
{
    public class DiamondSizeDto
    {
        public int LabAccountID { get; set; }
        public string LabAccountName { get; set; }
        public string Size { get; set; }
        public decimal SizeMin { get; set; }
        public decimal SizeMax { get; set; }
        public int SizeOrder { get; set; }
    }
}
