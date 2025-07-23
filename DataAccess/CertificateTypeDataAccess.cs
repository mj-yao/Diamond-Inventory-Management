using DiamondTransaction.UseCases.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DiamondTransaction.UseCases.Models;

namespace DiamondTransaction.DataAccess
{
    public class CertificateTypeDataAccess:ICertificateTypeRepository
    {
        private readonly string _connectionString;

        public CertificateTypeDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<CertificateTypeDto>> GetAllCertificateTypesAsync()
        {
            const string sql = "SELECT LabAccountID, LabAccountName, CertificateType, CertificateTypeDesc, CertificateTypeOrder FROM THXADiamondLabCertificate order by CertificateTypeOrder ";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<CertificateTypeDto>(sql);
            }
        }
        /*
        public async Task<CertificateTypeDto> GetCertificateTypeByTypeAsync(string certificateType)
        {
            const string sql = "SELECT * FROM THXADiamondLabCertificate WHERE CertificateType = @CertificateType";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<CertificateTypeDto>(sql, new { CertificateType = certificateType });
            }
        }
        */
    }
}
