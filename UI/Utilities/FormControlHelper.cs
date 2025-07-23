using DiamondTransaction.DataAccess;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiamondTransaction.UI.Utilities
{
    public static class FormControlHelper
    {
        public static decimal GetDecimalValue(string text)
        {
            return string.IsNullOrEmpty(text) ? 0 : Convert.ToDecimal(text);
        }

        public static decimal GetDecimalFromObject(object value)
        {
            decimal.TryParse(value?.ToString(), out var result);
            return result;
        }
        
        public static decimal? GetNullableDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (decimal.TryParse(input.Trim(), out decimal result))
                return result;

            return null; 
        }
        public static double GetDoubleValue(string text)
        {
            return string.IsNullOrEmpty(text) ? 0 : Convert.ToDouble(text);
        }

        public static int GetIntValue(string text)
        {
            return string.IsNullOrEmpty(text) ? 0 : Convert.ToInt32(text);
        }

        public static void SetComboBoxItem(ComboBox comboBox, List<string> items)
        {
            if (comboBox == null || items == null) return;
            comboBox.Items.Clear();
            comboBox.Items.AddRange(items.ToArray());
        }

        public static void SetupAutoComplete(ComboBox comboBox, List<string> items)
        {

            if (items == null || items.Count == 0) return;

            // Enable AutoComplete
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection autoCompleteData = new AutoCompleteStringCollection();
            autoCompleteData.AddRange(items.ToArray());

            comboBox.AutoCompleteCustomSource = autoCompleteData;

        }

        public static void SetComboBoxWithAutoComplete(ComboBox comboBox, List<string> items)
        {
            if (comboBox == null || items == null) return;

            comboBox.BeginUpdate();
            SetComboBoxItem(comboBox, items);
            SetupAutoComplete(comboBox, items);
            comboBox.EndUpdate();
        }

        public static void ApplyFilter(StringBuilder filterExpression, Dictionary<string, (Control Control, bool IsExactMatch)> filters)
        {
            foreach (var filter in filters)
            {
                string value = GetControlValue(filter.Value.Control);
                if (!string.IsNullOrEmpty(value))
                {
                    if (filterExpression.Length > 0)
                        filterExpression.Append(" AND ");

                    if (!filter.Value.IsExactMatch)
                        filterExpression.Append($"{filter.Key} LIKE '%{value}%'");
                    else
                        filterExpression.Append($"{filter.Key} = '{value}'");
                }
            }
        }

        public static List<T> FilterDtoList<T>(IEnumerable<T> data, Dictionary<string, (Control Control, bool IsExactMatch)> filters)
        {
            // Use LINQ to filter the input list based on all provided filters
            return data.Where(item =>
            {
                // Check each filter condition
                foreach (var filter in filters)
                {
                    var propertyName = filter.Key;
                    var (control, isExactMatch) = filter.Value;

                    // Get the user input from the control (TextBox or ComboBox)
                    string input = GetControlValue(control);
                    if (string.IsNullOrWhiteSpace(input))
                        continue; // Skip empty filters

                    // Use reflection to get the property info by name (e.g., "DocCode", "AccountName")
                    var prop = typeof(T).GetProperty(propertyName);
                    if (prop == null)
                        continue; // Skip if the property doesn't exist on the DTO

                    // Get the value of the property from the current item and convert to string
                    var value = prop.GetValue(item)?.ToString() ?? "";

                    
                    if (isExactMatch)
                    {
                        if (!string.Equals(value, input, StringComparison.OrdinalIgnoreCase))
                            return false; 
                    }
                    else
                    {
                        // If partial match is allowed, check if input is contained in the value (case-insensitive)
                        if (!value.ToLower().Contains(input.ToLower()))
                            return false; 
                    }
                }

                return true; // Include this item if it passed all filter conditions
            }).ToList(); 
        }

        public static string GetControlValue(Control control)
        {
            if (control is TextBox textBox)
                return textBox.Text;
            if (control is ComboBox comboBox)
                return comboBox.Text;
            return string.Empty;
        }


        public static void ResetControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                switch (c)
                {
                    case TextBox textBox:
                        textBox.Clear();
                        break;

                    case ComboBox comboBox:
                        comboBox.SelectedIndex = -1;
                        break;

                    case DataGridView dataGridView:
                        if (dataGridView.DataSource != null)
                        {
                            dataGridView.DataSource = null;  // Unbind and clear the table
                        }
                        else
                        {
                            dataGridView.Rows.Clear();
                            dataGridView.Columns.Clear();
                        }
                        break;

                    case CheckBox checkBox:
                        checkBox.Checked = false;
                        break;

                    case RadioButton radioButton:
                        radioButton.Checked = false;
                        break;

                    case ListBox listBox:
                        listBox.ClearSelected();
                        break;

                    case NumericUpDown numericUpDown:
                        numericUpDown.Value = numericUpDown.Minimum;
                        break;
                }

                if (c.HasChildren)
                {
                    ResetControls(c);  // Recursive call for nested controls (e.g., inside GroupBox)
                }
            }
        }

        public static void comboBox_DynamicDropDownWidth(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            int maxWidth = combo.Width;
            foreach (var item in combo.Items)
            {
                int itemWidth = TextRenderer.MeasureText(item.ToString(), combo.Font).Width;
                if (itemWidth > maxWidth)
                {
                    maxWidth = itemWidth;
                }
            }
            combo.DropDownWidth = maxWidth;
        }

        public static void setDecimalPlacesFormat(object sender, int DecimalPlace)
        {
            TextBox tb = (TextBox)sender;
            try
            {
                double.TryParse(tb.Text, out double TextInDouble);
                if (DecimalPlace == 0)
                    tb.Text = string.Format("{0:0}", TextInDouble);
                else if (DecimalPlace == 1)
                    tb.Text = string.Format("{0:0.0}", TextInDouble);
                else if (DecimalPlace == 2)
                    tb.Text = string.Format("{0:0.00}", TextInDouble);
                else if (DecimalPlace == 3)
                    tb.Text = string.Format("{0:0.000}", TextInDouble);
                else if (DecimalPlace == 4)
                    tb.Text = string.Format("{0:0.0000}", TextInDouble);
            }
            catch (Exception e1)
            {
                SqlQueryExecutor.ShowErrorMessage(e1, "Text Parse to Double fails");
            }
        }


        public static void SetUpdateButtonOn(Control updateBtn, Color color)
        {
            updateBtn.Enabled = true;
            SetControlBackColor(updateBtn, color);
        }
        public static void SetUpdateButtonOff(Control updateBtn, Color color)
        {
            updateBtn.Enabled = false;
            SetControlBackColor(updateBtn, color);
        }
        public static void SetControlBackColor(Control control, Color color)
        {
            control.BackColor = color;
        }

        public static void EnableTextBoxEditing(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.ReadOnly = false;
                tb.BackColor = System.Drawing.Color.Yellow;
            }
        }

        public static bool ValidateControl(Control control, string message)
        {
            bool isEmpty = false;

            switch (control)
            {
                case TextBox tb:
                    isEmpty = string.IsNullOrWhiteSpace(tb.Text);
                    break;

                case ComboBox cb:
                    isEmpty = string.IsNullOrWhiteSpace(cb.Text);
                    break;

                default:
                    isEmpty = true; 
                    break;
            }

            if (isEmpty)
            {
                MessageBox.Show(message, "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                control.Focus();
                return false;
            }

            return true;
        }

        public static bool HasIntValue(string text)
        {
            return int.TryParse(text, out _);
        }
        public static bool HasDecimalValue(string text)
        {
            return decimal.TryParse(text, out _);
        }

        private static readonly Dictionary<string, string> ParcelOrStoneMap = new Dictionary<string, string>
        {
            { "Stone", "S" },
            { "Parcel", "P" }
        };

        private static readonly Dictionary<string, string> HoldingTypeMap = new Dictionary<string, string>
        {
            { "Permanent", "P" },
            { "Temporary", "T" }
        };


        public static string MapParcelOrStone(string parcelOrStone)
        {
            if (!ParcelOrStoneMap.TryGetValue(parcelOrStone, out string mappedParcelOrStone))
                throw new ArgumentException($"Invalid ParcelOrStone: {parcelOrStone}");

            return mappedParcelOrStone;
        }
        public static string MapHoldingType(string holdingType)
        { 

            if (!HoldingTypeMap.TryGetValue(holdingType, out string mappedHoldingType))
                throw new ArgumentException($"Invalid HoldingType: {holdingType}");

            return mappedHoldingType;
        }
        /*
        public static void ReplaceNullStringsWithEmpty(DocLineDto dto)
        {
            var stringProperties = typeof(DocLineDto)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            foreach (var prop in stringProperties)
            {
                var currentValue = (string)prop.GetValue(dto);
                if (currentValue == null)
                {
                    prop.SetValue(dto, "");
                }
            }
        }
        */

        public static void ReplaceNullStringsWithEmpty<T>(T dto) where T : class
        {
            if (dto == null) return;

            var stringProperties = typeof(T)
                .GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite);

            foreach (var prop in stringProperties)
            {
                var currentValue = (string)prop.GetValue(dto);
                if (currentValue == null)
                {
                    prop.SetValue(dto, "");
                }
            }
        }

        public static void ReplaceNullsInDto<T>(T dto) where T : class
        {
            if (dto == null) return;

            var properties = typeof(T)
                .GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var prop in properties)
            {
                object value = prop.GetValue(dto);

                if (value == null)
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(dto, "");
                    }
                    else if (prop.PropertyType == typeof(int?))
                    {
                        prop.SetValue(dto, 0);
                    }
                    else if (prop.PropertyType == typeof(decimal?))
                    {
                        prop.SetValue(dto, 0m);
                    }
                    else if (prop.PropertyType == typeof(double?))
                    {
                        prop.SetValue(dto, 0.0);
                    }
                    else if (prop.PropertyType == typeof(DateTime?))
                    {
                        // Optional: set default date if you want — otherwise, leave null
                        // prop.SetValue(dto, DateTime.MinValue);
                    }
                }
            }
        }




    }
}
