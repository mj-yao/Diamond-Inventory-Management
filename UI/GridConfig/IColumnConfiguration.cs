using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UI.GridConfig
{
    public interface IColumnConfiguration
    {
        List<string> GetVisibleColumns(int docType);
        List<string> GetHiddenColumns(int docType, string currency = null);
        List<ColumnFormat> GetColumnFormat();
    }
}
