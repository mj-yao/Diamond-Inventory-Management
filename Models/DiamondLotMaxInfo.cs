using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.Models
{
    public class DiamondLotMaxInfo
    {
        public int MaxItemID { get; set; }
        public int MaxLotID { get; set; }
        public int MaxCertificateID { get; set; }

        public DiamondLotMaxInfo(int maxItemID, int maxLotID, int maxCertificateID)
        {
            MaxItemID = maxItemID;
            MaxLotID = maxLotID;
            MaxCertificateID = maxCertificateID;
        }
    }
}
