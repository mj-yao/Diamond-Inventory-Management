using DiamondTransaction.Domain.Interfaces;
using DiamondTransaction.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UI
{
    public class ColumnConfigurations
    {
        public class DocHeaderColumnConfiguration : IColumnConfiguration
        {
            public List<string> GetVisibleColumns(int docType)
            {
                return GridColumns.DocHeaderColumn;
                //return new List<string>();
            }
   
            public List<string> GetHiddenColumns(int docType)
            {
                return new List<string>();
            }

            public List<ColumnFormat> GetColumnFormat()
            {
                return GridColumnFormat.DocHeaderColumnFormat;
            }
        }



        public class DocLineColumnConfiguration : IColumnConfiguration
        {
            public List<string> GetVisibleColumns(int docType)
                => GridColumns.DocLineColumnByDocType.TryGetValue(docType, out var cols) ? cols : new List<string>();

            public List<string> GetHiddenColumns(int docType)
            {
                return new List<string>();
            }

            public List<ColumnFormat> GetColumnFormat()
            {
                return GridColumnFormat.DocLineColumnFormat;
            }
        }

        public class WorkingLineColumnConfiguration : IColumnConfiguration
        {
            public List<string> GetVisibleColumns(int docType)
                => GridColumns.WorkingLineColumnByDocType.TryGetValue(docType, out var cols) ? cols : new List<string>();

            public List<string> GetHiddenColumns(int docType)
                => GridColumns.WorkingLineHiddenColumnsByDocType.TryGetValue(docType, out var cols) ? cols : new List<string>();

            public List<ColumnFormat> GetColumnFormat()
            {
                return GridColumnFormat.WorkingLineColumnFormat;
            }
        }


    }
}
