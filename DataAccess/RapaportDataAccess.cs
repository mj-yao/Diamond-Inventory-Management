using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.Domain.Entities;
using DiamondTransaction.UseCases.Models;
using System.Data;
using System.Windows.Forms;

namespace DiamondTransaction.DataAccess
{
    public class RapaportDataAccess : IRapaportRepository
    {
        private readonly string _connectionString;

        public RapaportDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<string> GetRapaportShapeScale()
        {
            string errorMessageTitle = "Access Shape";
            string query = $@"SELECT DISTINCT Shape FROM THXADiamondShape where LabAccountName != 'Parcel' ORDER BY Shape ASC";

            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, errorMessageTitle);
            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                return sqlResult.AsEnumerable()
                    .Select(row => row.Field<string>("Shape"))
                    .Where(value => !string.IsNullOrEmpty(value))
                    .ToList();
            }
            return new List<string>();
        }

        public List<string> GetRapaportGradingScale(string gradeType)
        {
            string errorMessageTitle = "Access RapaportGradeScale";
            string query = $@"SELECT DISTINCT {gradeType} FROM THXADiamondRapList ORDER BY {gradeType} ASC";

            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, errorMessageTitle);
            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                return sqlResult.AsEnumerable()
                    .Select(row => row.Field<string>(gradeType))
                    .Where(value => !string.IsNullOrEmpty(value))
                    .ToList();
            }
            return new List<string>();
        }

        public string GetRapaportListPrice(string shape, string size, string color, string clarity)
        {
            if (shape != "BR") shape = "PS";

            try
            {
                string query = $"SELECT ListPrice FROM THXADiamondRapList WHERE Shape_Abbr = @Shape AND Size = @Size AND Color = @Color AND Clarity = @Clarity ";

                string errorMessageTitle = "Find List Price for the Diamond ";


                var parameters = new Dictionary<string, object>
                {
                    { "@Shape", shape },
                    { "@Size", size },
                    { "@Color", color },
                    { "@Clarity", clarity }
                };

                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(SqlQueryExecutor.getConnectionString(), query, errorMessageTitle, parameters);
                if (SqlQueryExecutor.IsValid(sqlResult))
                {
                    return (sqlResult.Rows[0]["ListPrice"].ToString());
                }
                else
                {
                    MessageBox.Show("List Price is not found, check the diamond grading detail. ", "Message");
                    return string.Empty;
                }
            }
            catch (Exception e1)
            {
                SqlQueryExecutor.ShowErrorMessage(e1, "Access Stone Detail failure.");
                return string.Empty;

            }
        }

    }
}
