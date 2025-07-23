using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.Domain.Entities;


namespace DiamondTransaction.DataAccess
{
    public class DiamondGradingDataAccess : IDiamondGradingRepository
    {
        private readonly string _connectionString;

        public DiamondGradingDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<DiamondSizeDto>> GetDiamondSizesAsync(string labname = null)
        {
            var query = $@"SELECT LabAccountID, LabAccountName, Size, SizeMin, SizeMax, SizeOrder FROM THXADiamondSize
            {(string.IsNullOrEmpty(labname) ? "" : " WHERE LabAccountName = @LabAccountName")}
            ORDER BY SizeOrder ";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<DiamondSizeDto>(
                    query, 
                    new { LabAccountName = labname }
                );
                return result;
            }            
        }

        public async Task<IEnumerable<DiamondGrades>> GetGradesByGradeTypeAndLabAsync(string gradeType, string labname = null)
        {
            string tableName = $"THXADiamond{gradeType}";
            string orderBy = $"{gradeType}Order";

            var query = $@"
            SELECT 
                {(string.IsNullOrEmpty(labname) ? "" : "LabAccountID, LabAccountName,")}
                {gradeType} AS Grade,
                {gradeType}Desc AS GradeDesc,
                {gradeType}Order AS GradeOrder 
            FROM {tableName}
            {(string.IsNullOrEmpty(labname) ? "" : " WHERE LabAccountName = @LabAccountName")}
            ORDER BY {orderBy}";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<DiamondGrades>(
                    query,
                    new { LabAccountName = labname }
                );

                return result;
            }
        }

    }

}
