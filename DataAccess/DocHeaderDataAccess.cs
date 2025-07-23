using DiamondTransaction.UseCases.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DiamondTransaction.UseCases.Models;
using System.Data.SqlClient;

namespace DiamondTransaction.DataAccess
{
    public class DocHeaderDataAccess : IDocHeaderRepository
    {
        private readonly string _connectionString;
        public DocHeaderDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<DocHeaderDto>> GetAllDiamondDocHeadersAsync()
        {
            const string sql = @"SELECT DocHeaderID, DocTypeID, DocTypeDesc, DocID, DocCode, DocAccountRefNo, DocSubType, DocStatus, DocDate,
                                RefDocDate, RefDocDate2, DueDate, DateTime_Modified, AccountCode, AccountBranchCode, AccountName, AccountLongName, Currency,
                                VatCode, VatRate, PaymentTerm, Loandays, Remark1, Remark2, InternalRemark, UserID, Quantity, Weight,
                                MainLinesTotalPrice, SecLinesTotalPrice, DocLinesTotalPrice, MainDiscountPrice, SecDiscountPrice, DocDiscountPrice, DiscountPercent, DiscountRemark,
                                MainSubTotalPrice, SecSubTotalPrice, DocSubTotalPrice, MainAdditionalPrice, SecAdditionalPrice, DocAdditionalPrice, AdditionalRemark,
                                MainTaxPrice, SecTaxPrice, DocTaxPrice, MainGrandTotalPrice, SecGrandTotalPrice, DocGrandTotalPrice, RateDocToMain, RateDocToSec
                         FROM THXADiamondDocHeader
                         ORDER BY DocDate desc, DocID DESC ";

            using (var connection = new SqlConnection(_connectionString))
            {                 
                return await connection.QueryAsync<DocHeaderDto>(sql);;
            }
        }

        public int InsertDiamondDocHeader(DocHeaderDto docHeader)
        {
            const string sql = @"
                INSERT INTO THXADiamondDocHeader (
                    DocTypeID, DocTypeDesc, DocID, DocCode, DocAccountRefNo, DocSubType, DocStatus, DocDate,
                    RefDocDate, RefDocDate2, DueDate, DateTime_Modified, AccountCode, AccountBranchCode, AccountName, AccountLongName, Currency,
                    VatCode, VatRate, PaymentTerm, Loandays, Remark1, Remark2, InternalRemark, UserID, Quantity, Weight,
                    MainLinesTotalPrice, SecLinesTotalPrice, DocLinesTotalPrice, MainDiscountPrice, SecDiscountPrice, DocDiscountPrice, DiscountPercent, DiscountRemark,
                    MainSubTotalPrice, SecSubTotalPrice, DocSubTotalPrice, MainAdditionalPrice, SecAdditionalPrice, DocAdditionalPrice, AdditionalRemark,
                    MainTaxPrice, SecTaxPrice, DocTaxPrice, MainGrandTotalPrice, SecGrandTotalPrice, DocGrandTotalPrice, RateDocToMain, RateDocToSec
                )
                VALUES (
                    @DocTypeID, @DocTypeDesc, @DocID, @DocCode, @DocAccountRefNo, @DocSubType, @DocStatus, @DocDate,
                    @RefDocDate, @RefDocDate2, @DueDate, @DateTime_Modified, @AccountCode, @AccountBranchCode, @AccountName, @AccountLongName, @Currency,
                    @VatCode, @VatRate, @PaymentTerm, @Loandays, @Remark1, @Remark2, @InternalRemark, @UserID, @Quantity, @Weight,
                    @MainLinesTotalPrice, @SecLinesTotalPrice, @DocLinesTotalPrice, @MainDiscountPrice, @SecDiscountPrice, @DocDiscountPrice, @DiscountPercent, @DiscountRemark,
                    @MainSubTotalPrice, @SecSubTotalPrice, @DocSubTotalPrice, @MainAdditionalPrice, @SecAdditionalPrice, @DocAdditionalPrice, @AdditionalRemark,
                    @MainTaxPrice, @SecTaxPrice, @DocTaxPrice, @MainGrandTotalPrice, @SecGrandTotalPrice, @DocGrandTotalPrice, @RateDocToMain, @RateDocToSec
                );

                SELECT CAST(SCOPE_IDENTITY() AS int);
            ";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var docHeaderId = connection.ExecuteScalar<int>(sql, docHeader);
                return docHeaderId;
            }
        }

        public bool UpdateDiamondDocHeader(DocHeaderDto docHeader)
        {
            const string sql = @"
                UPDATE THXADiamondDocHeader SET
                    DocTypeID = @DocTypeID,
                    DocTypeDesc = @DocTypeDesc,
                    DocID = @DocID,
                    DocCode = @DocCode,
                    DocAccountRefNo = @DocAccountRefNo,
                    DocSubType = @DocSubType,
                    DocStatus = @DocStatus,
                    DocDate = @DocDate,
                    RefDocDate = @RefDocDate,
                    RefDocDate2 = @RefDocDate2,
                    DueDate = @DueDate,
                    DateTime_Modified = @DateTime_Modified,
                    AccountCode = @AccountCode,
                    AccountBranchCode = @AccountBranchCode,
                    AccountName = @AccountName,
                    AccountLongName = @AccountLongName,
                    Currency = @Currency,
                    VatCode = @VatCode,
                    VatRate = @VatRate,
                    PaymentTerm = @PaymentTerm,
                    Loandays = @Loandays,
                    Remark1 = @Remark1,
                    Remark2 = @Remark2,
                    InternalRemark = @InternalRemark,
                    UserID = @UserID,
                    Quantity = @Quantity,
                    Weight = @Weight,
                    MainLinesTotalPrice = @MainLinesTotalPrice,
                    SecLinesTotalPrice = @SecLinesTotalPrice,
                    DocLinesTotalPrice = @DocLinesTotalPrice,
                    MainDiscountPrice = @MainDiscountPrice,
                    SecDiscountPrice = @SecDiscountPrice,
                    DocDiscountPrice = @DocDiscountPrice,
                    DiscountPercent = @DiscountPercent,
                    DiscountRemark = @DiscountRemark,
                    MainSubTotalPrice = @MainSubTotalPrice,
                    SecSubTotalPrice = @SecSubTotalPrice,
                    DocSubTotalPrice = @DocSubTotalPrice,
                    MainAdditionalPrice = @MainAdditionalPrice,
                    SecAdditionalPrice = @SecAdditionalPrice,
                    DocAdditionalPrice = @DocAdditionalPrice,
                    AdditionalRemark = @AdditionalRemark,
                    MainTaxPrice = @MainTaxPrice,
                    SecTaxPrice = @SecTaxPrice,
                    DocTaxPrice = @DocTaxPrice,
                    MainGrandTotalPrice = @MainGrandTotalPrice,
                    SecGrandTotalPrice = @SecGrandTotalPrice,
                    DocGrandTotalPrice = @DocGrandTotalPrice,
                    RateDocToMain = @RateDocToMain,
                    RateDocToSec = @RateDocToSec
                WHERE DocHeaderID = @DocHeaderID
            ";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                int affectedRows = connection.Execute(sql, docHeader);
                return affectedRows > 0;
            }
        }



        /*
        private void OldAccessDiamondDocHeader()
        {
            try
            {
                string errorMessageTitle = "Access DiamondDocHeader";
                string query = "SELECT DocHeaderID, DocTypeID, DocTypeDesc, DocID, DocCode, DocAccountRefNo, DocSubType, DocStatus, DocDate, " +
                "RefDocDate, RefDocDate2, DueDate, DateTime_Modified, AccountCode, AccountBranchCode, AccountName, AccountLongName, Currency, " +
                "VatCode, VatRate, PaymentTerm, Remark1, Remark2, InternalRemark, UserID, Quantity, Weight, " +
                "MainLinesTotalPrice, SecLinesTotalPrice, DocLinesTotalPrice, MainDiscountPrice, SecDiscountPrice, DocDiscountPrice, DiscountPercent, DiscountRemark, " +
                "MainSubTotalPrice, SecSubTotalPrice, DocSubTotalPrice, MainAdditionalPrice, SecAdditionalPrice, DocAdditionalPrice, AdditionalRemark, " +
                "MainTaxPrice, SecTaxPrice, DocTaxPrice, MainGrandTotalPrice, SecGrandTotalPrice, DocGrandTotalPrice, RateDocToMain, RateDocToSec FROM THXADiamondDocHeader Order by DocDate ";

             
                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(connectionString, query, errorMessageTitle);
                if (SqlQueryExecutor.IsValid(sqlResult))
                {
                    dgv_DocHeader.DataSource = sqlResult.DefaultView;
                    dgv_DocHeaderFormat();
                }
            }
            catch (Exception e1)
            {
                SqlQueryExecutor.ShowErrorMessage(e1, "Access Stone Detail failure.");
            }
        }
         
         */


    }

}
