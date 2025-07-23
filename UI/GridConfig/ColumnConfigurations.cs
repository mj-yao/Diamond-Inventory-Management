using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UI.GridConfig
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
   
            public List<string> GetHiddenColumns(int docType, string currency = null)
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


            public List<string> GetHiddenColumns(int docType, string currency = null)
            {
                // Start with default hidden columns by DocType
                var hidden = new List<string>();
                if (GridColumns.DocLineHiddenColumnsByDocType.TryGetValue(docType, out var baseHidden))
                    hidden.AddRange(baseHidden);

                
                // Define docTypes using Cost/SecCost
                var costBasedDocTypes = new HashSet<int> { 1006, 1007, 1009, 1010 };

                if (costBasedDocTypes.Contains(docType))
                {
                    // Cost/SecCost logic
                    if (currency == "USD" || string.IsNullOrEmpty(currency))
                    {
                        hidden.Add("SecCost");
                        hidden.Add("SecTotalCost");
                    }
                    else
                    {
                        hidden.Add("Cost");
                        hidden.Add("TotalCost");
                    }
                }
                else
                {
                    // OutMainPrice/OutSecPrice logic
                    if (currency == "USD" || string.IsNullOrEmpty(currency))
                    {
                        hidden.Add("OutSecPrice");
                        hidden.Add("OutSecTotalPrice");
                    }
                    else
                    {
                        hidden.Add("OutMainPrice");
                        hidden.Add("OutMainTotalPrice");
                    }
                }

                return hidden.Distinct().ToList(); // ensure no duplicates
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

            public List<string> GetHiddenColumns(int docType, string currency =  null)
                => GridColumns.WorkingLineHiddenColumnsByDocType.TryGetValue(docType, out var cols) ? cols : new List<string>();

            public List<ColumnFormat> GetColumnFormat()
            {
                return GridColumnFormat.WorkingLineColumnFormat;
            }
        }


    }
}
