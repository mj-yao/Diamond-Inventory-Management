using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.Domain.Entities
{
    public class ParcelGrades
    {
        public List<string> Shapes { get; set; }
        public List<string> Sizes { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Clarities { get; set; }
    }

}
