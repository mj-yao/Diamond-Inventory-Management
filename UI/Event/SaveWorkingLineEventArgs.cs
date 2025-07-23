using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiamondTransaction.UI.Event
{

    public class SaveWorkingLineEventArgs : EventArgs
    {
   
        public DiamondLotMaxID DiamondLotMaxID { get; set; }
        public WorkingLineDto WorkingLineDto { get; set; }

        public SaveWorkingLineEventArgs(DiamondLotMaxID diamondLotMaxID, WorkingLineDto workingLineDto)
        {
            DiamondLotMaxID = diamondLotMaxID;
            WorkingLineDto = workingLineDto ;
        } 
    }

    public class SaveCertificateEventArgs : EventArgs
        {
            public CertificateDto Certificate { get; set; }

            public SaveCertificateEventArgs(CertificateDto certificate)
            {
                Certificate = certificate;
            }
        }

}
