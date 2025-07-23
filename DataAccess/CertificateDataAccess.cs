using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;

namespace DiamondTransaction.DataAccess
{
    public class CertificateDataAccess : ICertificateRepository
    {
        private readonly string _connectionString;
        public CertificateDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CertificateDto> GetCertificateAsync(int certificateID)
        {
            const string sql = @"SELECT * FROM THXADiamondCertificate WHERE CertificateID = @CertificateID";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<CertificateDto>(sql, new { CertificateID = certificateID });
            }
        }

        public int InsertCertificate(CertificateDto cert)
        {
            const string sql = @"
                INSERT INTO THXADiamondCertificate (
                    LotID, CertificateLabName, CertificateType, CertificateTypeDesc, CertificateNo, CertificateDate, 
                    Shape, Size, Color, Clarity, Measurements, Weight, Depth, StoneTable,
                    GirdleMinMax, GirdleCondition, Girdle, Culet, Polish, Symmetry, Fluorescence, Cut,
                    CrownAngle, CrownHeight, PavillionAngle, PavillionDepth, StarLength, LowerHalf,
                    Inscription, LabComment, DownloadStatus, Status, Remark, Created_By, DateTime_Created
                )
                VALUES (
                    @LotID, @CertificateLabName, @CertificateType, @CertificateTypeDesc, @CertificateNo, @CertificateDate, 
                    @Shape, @Size, @Color, @Clarity, @Measurements, @Weight, @Depth, @StoneTable,
                    @GirdleMinMax, @GirdleCondition, @Girdle, @Culet, @Polish, @Symmetry, @Fluorescence, @Cut,
                    @CrownAngle, @CrownHeight, @PavillionAngle, @PavillionDepth, @StarLength, @LowerHalf,
                    @Inscription, @LabComment, @DownloadStatus, @Status, @Remark, @Created_By, GETDATE()
                );

                SELECT CAST(SCOPE_IDENTITY() AS int);
            ";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.ExecuteScalar<int>(sql, cert);
            }
        }

        public bool UpdateCertificate(CertificateDto certificateDto)
        {
            const string sql = @"
                UPDATE THXADiamondCertificate SET
                    LotID = @LotID,
                    CertificateLabName = @CertificateLabName,
                    CertificateType = @CertificateType,
                    CertificateTypeDesc = @CertificateTypeDesc,
                    CertificateNo = @CertificateNo,
                    CertificateDate = @CertificateDate,
                    Shape = @Shape,
                    Size = @Size,
                    Color = @Color,
                    Clarity = @Clarity,
                    Measurements = @Measurements,
                    Weight = @Weight,
                    Depth = @Depth,
                    StoneTable = @StoneTable,
                    GirdleMinMax = @GirdleMinMax,
                    GirdleCondition = @GirdleCondition,
                    Girdle = @Girdle,
                    Culet = @Culet,
                    Polish = @Polish,
                    Symmetry = @Symmetry,
                    Fluorescence = @Fluorescence,
                    Cut = @Cut,
                    CrownAngle = @CrownAngle,
                    CrownHeight = @CrownHeight,
                    PavillionAngle = @PavillionAngle,
                    PavillionDepth = @PavillionDepth,
                    StarLength = @StarLength,
                    LowerHalf = @LowerHalf,
                    Inscription = @Inscription,
                    LabComment = @LabComment,
                    DownloadStatus = @DownloadStatus,
                    Status = @Status,
                    Remark = @Remark
                WHERE CertificateID = @CertificateID";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                int rowsAffected = connection.Execute(sql, certificateDto);
                return rowsAffected > 0;
            }
        }






    }
}
