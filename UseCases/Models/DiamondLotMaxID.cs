using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Models
{
    public class DiamondLotMaxID
    {
        public int MaxItemID { get; set; }
        public int MaxLotID { get; set; }
        public int MaxCertificateID { get; set; }

        public DiamondLotMaxID(int maxItemID, int maxLotID, int maxCertificateID)
        {
            MaxItemID = maxItemID;
            MaxLotID = maxLotID;
            MaxCertificateID = maxCertificateID;
        }
        public void UpdateMaxLotID(int lotID)
        {
            if (lotID > MaxLotID)
                MaxLotID = lotID;
        }

        public void UpdateMaxItemID(int itemID)
        {
            if (itemID > MaxItemID)
                MaxItemID = itemID;
        }
        public void UpdateMaxCertificateID(int certificateID)
        {
            if (certificateID > MaxCertificateID)
                MaxCertificateID = certificateID;
        }
        public bool IsUpdateMaxCertificateID(int certificateID)
        {
            if (certificateID > MaxCertificateID)
                return true;
            return false;
        }

    }
}
