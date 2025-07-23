using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UI.Utilities
{
    public class ColumnFormat
    {
        public string Name { get; set; }
        public string HeaderText { get; set; }
        public int? Width { get; set; }

        public ColumnFormat()
        {
            // Parameterless constructor needed for object initializer syntax
        }
        public ColumnFormat(string name, string headerText = null, int width = 0)
        {
            Name = name;
            HeaderText = headerText ?? name;
            Width = width;
        }
    }
}
