using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiamondTransaction
{
    public class GlobalDataManager
    {
        public class RowCalculatedEventArgs : EventArgs
        {
            public List<DataGridViewRow> WorkingLine { get; set; }
            public CertificateDto Certificate { get; set; }

            public RowCalculatedEventArgs(List<DataGridViewRow> rows, CertificateDto certificate)
            {
                WorkingLine = rows;
                Certificate = certificate;
            }
        }

    }
}
