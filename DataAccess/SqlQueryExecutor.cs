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
using DiamondTransaction.UseCases.Models;

namespace DiamondTransaction.DataAccess
{
    public class SqlQueryExecutor
    {
        static string userName = "";
        static string connectionString;
        public static void setConnectionString(string connString)
        {
            if (string.IsNullOrWhiteSpace(connString))
                throw new ArgumentException("Connection string cannot be empty.");

            connectionString = connString;
        }
        public static string getConnectionString()
        {
            return connectionString;
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
    }
}
