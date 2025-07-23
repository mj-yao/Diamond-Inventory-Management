using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiamondTransaction.UI.GridConfig
{
    public class GridColumnBuilder
    {
        private readonly IColumnConfiguration _columnConfig;

        public GridColumnBuilder(IColumnConfiguration columnConfig)
        {
            _columnConfig = columnConfig;
        }

        public void ConfigureGrid(DataGridView grid, int docType)
        {
            grid.Columns.Clear();

            var visibleColumns = _columnConfig.GetVisibleColumns(docType);
            var hiddenColumns = _columnConfig.GetHiddenColumns(docType);

            foreach (var columnName in visibleColumns)
            {                             
                var column = new DataGridViewTextBoxColumn
                {
                    Name = columnName,
                    HeaderText = columnName,
                    Visible = !hiddenColumns.Contains(columnName)
                };
                grid.Columns.Add(column);
            }
        }

        public void ConfigureGridFormat(DataGridView grid, int docType, string currency = null)
        {
            var visibleColumns = _columnConfig.GetVisibleColumns(docType);
            var hiddenColumns = _columnConfig.GetHiddenColumns(docType, currency);
            var columnDefinitionDict = _columnConfig.GetColumnFormat().ToDictionary(def => def.Name);

            //hide all columns first
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.Visible = false;
            }

            foreach (var columnName in visibleColumns)
            {
                //check if the grid contains the column
                if (!grid.Columns.Contains(columnName))
                {
                    Console.WriteLine($"Warning: Column '{columnName}' does not exist in the DataGridView.");
                    continue;
                }

                //set column visible, HeaderText, and Width if defined
                var column = grid.Columns[columnName];
                column.Visible = !hiddenColumns.Contains(columnName);

                if (columnDefinitionDict.TryGetValue(columnName, out var def))
                {
                    column.HeaderText = def.HeaderText ?? columnName;
                    if (def.Width > 0)
                        column.Width = (int)def.Width;
                }

            }
        }
    }
}
