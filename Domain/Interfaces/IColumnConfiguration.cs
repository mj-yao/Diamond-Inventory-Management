using DiamondTransaction.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.Domain.Interfaces
{
    public interface IColumnConfiguration
    {
        List<string> GetVisibleColumns(int docType);
        List<string> GetHiddenColumns(int docType);
        List<ColumnFormat> GetColumnFormat();
    }
}
