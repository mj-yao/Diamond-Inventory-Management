using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.Domain.Entities
{
    public class ExchangeRate
    {
        public decimal USDRate { get; set; }
        public decimal GBPRate => Math.Round(1 / USDRate, 3);
    }

}
