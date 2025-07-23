using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.Domain.Entities
{
    public class DocType
    {
        public int DocTypeID { get; set; }
        public string DocTypeDesc { get; set; }
        public string DocTypeName { get; set; }
    }
}
