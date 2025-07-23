using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.Models
{
    public class DocHeaderMaxInfo
    {
        public int MaxDocID { get; set; }
        public string MaxDocCode { get; set; }

        public DocHeaderMaxInfo(int maxDocID, string maxDocCode)
        {
            MaxDocID = maxDocID;
            MaxDocCode = maxDocCode;
        }
    }
}
