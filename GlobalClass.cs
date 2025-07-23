using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Configuration;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DiamondTransaction
{
    public class GlobalClass
    {
        static string userName = "";
        static string connectionString;
        public static void setConnectionString(string connString)
        {
            connectionString = connString;
        }
        public static string getConnectionString()
        {
            return connectionString;
        }

        public static bool AccessTableData(ref SqlConnection Connection, ref string query, ref DataTable datatable, ref string errorMessageTitle)
        {
            Connection.Open();
            SqlCommand command = null;
            SqlDataAdapter adapter = null;
            try
            {
                command = new SqlCommand(query, Connection);
                command.CommandTimeout = 240;  //180 -> 240 25/04/2016
                adapter = new SqlDataAdapter(command);
                adapter.Fill(datatable);

                adapter = null;
                command = null;
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                return true;
            }
            catch (Exception e1)
            {
                adapter = null;
                command = null;
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                ShowErrorMessage(e1, errorMessageTitle);
                return false;
            }
        }

        public static DataTable AccessAndGetSqlResult(string connectionString, string query, string errorMessageTitle, Dictionary<string, object> parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                                   
                    command.CommandTimeout = 240;

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable sqlResult = new DataTable();
                        connection.Open();
                        adapter.Fill(sqlResult);
                        return sqlResult;
                    }
                }
                catch (Exception e)
                {
                    ShowErrorMessage(e, errorMessageTitle);
                    return new DataTable(); // Return an empty DataTable instead of null
                }
            }
        }

        public static bool IsValid(DataTable sqlResult)
        {
            if (sqlResult != null && sqlResult.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public static void ShowErrorMessage(Exception e, string errorMessageTitle)
        {
            string error = string.Format("Error type:{0}, Error Message:{1}", e.GetType(), e.Message);
            MessageBox.Show(error, errorMessageTitle);
        }

        public static bool UpdateTableData1(ref SqlConnection Connection, string query, string errorMessageTitle, DateTime date, string SCName, string Add1, byte type)
        {
            Connection.Open();

            SqlCommand myCommand = Connection.CreateCommand();
            SqlTransaction tran = Connection.BeginTransaction("cls");
            myCommand.Transaction = tran;

            try
            {
                myCommand.CommandTimeout = 180;
                myCommand.CommandText = query;

                if (type == 1)
                    myCommand.Parameters.AddWithValue("@_DDDate", date);
                else if (type == 2)
                {
                    myCommand.Parameters.AddWithValue("@_DDDate", date);
                    myCommand.Parameters.AddWithValue("@_SCName", SCName);
                }
                else if (type == 3)
                {
                    myCommand.Parameters.AddWithValue("@_DDDate", date);
                    myCommand.Parameters.AddWithValue("@_SCName", SCName);
                    myCommand.Parameters.AddWithValue("@_sAdd1", Add1);
                }

                myCommand.ExecuteNonQuery();

                tran.Commit();
                myCommand = null;
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                return true;
            }
            catch (Exception e1)
            {
                ShowErrorMessage(e1, errorMessageTitle);
                try
                {
                    myCommand = null;
                    tran.Rollback();
                    if (Connection.State == ConnectionState.Open)
                        Connection.Close();

                }
                catch (Exception ex1)
                {
                    ShowErrorMessage(ex1, "Commit Warning");
                    myCommand = null;
                    if (Connection.State == ConnectionState.Open)
                        Connection.Close();
                }
                return false;
            }
        }


        public static bool UpdateTableData(ref SqlConnection Connection, string query, Dictionary<string, object> parameters, string errorMessageTitle)
        {
            Connection.Open();

            SqlCommand command = Connection.CreateCommand();
            SqlTransaction transaction = Connection.BeginTransaction("");
            command.Transaction = transaction;

            try
            {
                command.CommandTimeout = 180;
                command.CommandText = query;

                // Add parameters to the command
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }

                command.ExecuteNonQuery();

                transaction.Commit();
                command = null;
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                return true;
            }
            catch (Exception e1)
            {
                ShowErrorMessage(e1, errorMessageTitle);
                try
                {
                    command = null;
                    transaction.Rollback();
                    if (Connection.State == ConnectionState.Open)
                        Connection.Close();

                }
                catch (Exception ex1)
                {
                    ShowErrorMessage(ex1, "Update data rollback failed");
                    command = null;
                    if (Connection.State == ConnectionState.Open)
                        Connection.Close();
                }
                return false;
            }
        }
        public static bool UpdateTableDataUse2Query(ref SqlConnection Connection, string query1, string query2, Dictionary<string, object> parameters, string errorMessageTitle)
        {
            SqlTransaction transaction = null;
            SqlCommand command = null;

            try
            {
                Connection.Open();
                transaction = Connection.BeginTransaction();
                command = Connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandTimeout = 180;

                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }


                command.CommandText = query1;
                command.ExecuteNonQuery();

                command.CommandText = query2;
                command.ExecuteNonQuery();

                transaction.Commit();

                return true;
            }
            catch (Exception e1)
            {
                ShowErrorMessage(e1, errorMessageTitle);

                try
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception ex1)
                {
                    ShowErrorMessage(ex1, "Update data rollback failed");
                }

                return false;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }
            }
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


    }
}

