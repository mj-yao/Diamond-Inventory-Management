using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace DiamondTransaction.DataAccess
{
    public class DocLineDataAccess:IDocLineRepository
    {
        private readonly string _connectionString;
        public DocLineDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<DocLineDto>> GetDiamondDocLinesAsync(int docTypeID, int docID)
        {
            const string sql = @"SELECT * FROM THXADiamondDocLine 
                         WHERE DocTypeID = @DocTypeID AND DocID = @DocID
                         ORDER BY DocLine";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<DocLineDto>(sql, new { DocTypeID = docTypeID, DocID = docID });
            }
        }

        public int InsertDocLine(DocLineDto docLine)
        {
            const string sql = @"
                INSERT INTO THXADiamondDocLine (
                    DocHeaderID, DocTypeID, DocTypeDesc, DocID, DocLine, DocLineStatus, LotID, ParentLotID, ItemID, LotName,
                    ParcelOrStone, HoldingType, CertificateID, Shape, Size, Color, Clarity, Quantity, Weight, WeightLoss, Cost, 
                    TotalCost, SecCost, SecTotalCost, Sale, TotalSale, SecSale, SecTotalSale, Additional, TotalAdditional, 
                    MainPrice, TotalMainPrice, SecPrice, TotalSecPrice, DocPrice, TotalDocPrice, RateDocToMain, RateDocToSec, List, 
                    TotalList, ListCostDiscount, ListSaleDiscount, ListMainDiscount, Remark, OutList, OutTotalList, OutMainPrice, 
                    OutMainTotalPrice, OutListMainDiscount, OutSecPrice, OutSecTotalPrice, ReturnWeight, ReturnTotalCost, ReturnTotalSale, 
                    InvoicedWeight, InvoicedTotalCost, InvoicedTotalSale, DocPriceGross, MainPriceGross, SecPriceGross, TotalDocPriceGross, 
                    TotalMainPriceGross, TotalSecPriceGross, DocPriceGrossWithTax, TotalDocPriceGrossWithTax, DocPriceWithTax, 
                    TotalDocPriceWithTax, DocTotalDiscountWithTax, MarkupPercent, DiscountPercent, DocTotalDiscountAmount, MainTotalDiscountAmount, 
                    SecTotalDiscountAmount, TrsUnionID, SourceDocTypeID, SourceDocTypeDesc, SourceDocID, SourceDocLine, 
                    DateTime_Created, DateTime_Modified, UserID, SourceDocCode
                )
                VALUES (
                    @DocHeaderID, @DocTypeID, @DocTypeDesc, @DocID, @DocLine, @DocLineStatus, @LotID, @ParentLotID, @ItemID, @LotName,
                    @ParcelOrStone, @HoldingType, @CertificateID, @Shape, @Size, @Color, @Clarity, @Quantity, @Weight, @WeightLoss, @Cost, 
                    @TotalCost, @SecCost, @SecTotalCost, @Sale, @TotalSale, @SecSale, @SecTotalSale, @Additional, @TotalAdditional, 
                    @MainPrice, @TotalMainPrice, @SecPrice, @TotalSecPrice, @DocPrice, @TotalDocPrice, @RateDocToMain, @RateDocToSec, @List, 
                    @TotalList, @ListCostDiscount, @ListSaleDiscount, @ListMainDiscount, @Remark, @OutList, @OutTotalList, @OutMainPrice, 
                    @OutMainTotalPrice, @OutListMainDiscount, @OutSecPrice, @OutSecTotalPrice, @ReturnWeight, @ReturnTotalCost, @ReturnTotalSale, 
                    @InvoicedWeight, @InvoicedTotalCost, @InvoicedTotalSale, @DocPriceGross, @MainPriceGross, @SecPriceGross, @TotalDocPriceGross, 
                    @TotalMainPriceGross, @TotalSecPriceGross, @DocPriceGrossWithTax, @TotalDocPriceGrossWithTax, @DocPriceWithTax, 
                    @TotalDocPriceWithTax, @DocTotalDiscountWithTax, @MarkupPercent, @DiscountPercent, @DocTotalDiscountAmount, @MainTotalDiscountAmount, 
                    @SecTotalDiscountAmount, @TrsUnionID, @SourceDocTypeID, @SourceDocTypeDesc, @SourceDocID, @SourceDocLine,  
                    @DateTime_Created, @DateTime_Modified,@UserID, @SourceDocCode
                );

                SELECT CAST(SCOPE_IDENTITY() AS int);
            ";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var docLineId = connection.ExecuteScalar<int>(sql, docLine);
                return docLineId;
            }
        }

        public bool UpdateDocLine(DocLineDto docLine)
        {
            const string sql = @"
                UPDATE THXADiamondDocLine SET
                    DocHeaderID = @DocHeaderID,
                    DocTypeID = @DocTypeID,
                    DocTypeDesc = @DocTypeDesc,
                    DocID = @DocID,
                    DocLine = @DocLine,
                    DocLineStatus = @DocLineStatus,
                    LotID = @LotID,
                    ParentLotID = @ParentLotID,
                    ItemID = @ItemID,
                    LotName = @LotName,
                    ParcelOrStone = @ParcelOrStone,
                    HoldingType = @HoldingType,
                    CertificateID = @CertificateID,
                    Shape = @Shape,
                    Size = @Size,
                    Color = @Color,
                    Clarity = @Clarity,
                    Quantity = @Quantity,
                    Weight = @Weight,
                    WeightLoss = @WeightLoss,
                    Cost = @Cost,
                    TotalCost = @TotalCost,
                    SecCost = @SecCost,
                    SecTotalCost = @SecTotalCost,
                    Sale = @Sale,
                    TotalSale = @TotalSale,
                    SecSale = @SecSale,
                    SecTotalSale = @SecTotalSale,
                    Additional = @Additional,
                    TotalAdditional = @TotalAdditional,
                    MainPrice = @MainPrice,
                    TotalMainPrice = @TotalMainPrice,
                    SecPrice = @SecPrice,
                    TotalSecPrice = @TotalSecPrice,
                    DocPrice = @DocPrice,
                    TotalDocPrice = @TotalDocPrice,
                    RateDocToMain = @RateDocToMain,
                    RateDocToSec = @RateDocToSec,
                    List = @List,
                    TotalList = @TotalList,
                    ListCostDiscount = @ListCostDiscount,
                    ListSaleDiscount = @ListSaleDiscount,
                    ListMainDiscount = @ListMainDiscount,
                    Remark = @Remark,
                    OutList = @OutList,
                    OutTotalList = @OutTotalList,
                    OutMainPrice = @OutMainPrice,
                    OutMainTotalPrice = @OutMainTotalPrice,
                    OutListMainDiscount = @OutListMainDiscount,
                    OutSecPrice = @OutSecPrice,
                    OutSecTotalPrice = @OutSecTotalPrice,
                    ReturnWeight = @ReturnWeight,
                    ReturnTotalCost = @ReturnTotalCost,
                    ReturnTotalSale = @ReturnTotalSale,
                    InvoicedWeight = @InvoicedWeight,
                    InvoicedTotalCost = @InvoicedTotalCost,
                    InvoicedTotalSale = @InvoicedTotalSale,
                    DocPriceGross = @DocPriceGross,
                    MainPriceGross = @MainPriceGross,
                    SecPriceGross = @SecPriceGross,
                    TotalDocPriceGross = @TotalDocPriceGross,
                    TotalMainPriceGross = @TotalMainPriceGross,
                    TotalSecPriceGross = @TotalSecPriceGross,
                    DocPriceGrossWithTax = @DocPriceGrossWithTax,
                    TotalDocPriceGrossWithTax = @TotalDocPriceGrossWithTax,
                    DocPriceWithTax = @DocPriceWithTax,
                    TotalDocPriceWithTax = @TotalDocPriceWithTax,
                    DocTotalDiscountWithTax = @DocTotalDiscountWithTax,
                    MarkupPercent = @MarkupPercent,
                    DiscountPercent = @DiscountPercent,
                    DocTotalDiscountAmount = @DocTotalDiscountAmount,
                    MainTotalDiscountAmount = @MainTotalDiscountAmount,
                    SecTotalDiscountAmount = @SecTotalDiscountAmount,
                    TrsUnionID = @TrsUnionID,
                    SourceDocTypeID = @SourceDocTypeID,
                    SourceDocTypeDesc = @SourceDocTypeDesc,
                    SourceDocID = @SourceDocID,
                    SourceDocLine = @SourceDocLine,
                    DateTime_Created = @DateTime_Created,
                    DateTime_Modified = @DateTime_Modified,
                    UserID = @UserID,
                    SourceDocCode = @SourceDocCode
                WHERE DocLineID = @DocLineID;
            ";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                int affectedRows = connection.Execute(sql, docLine);
                return affectedRows > 0;
            }
        }


        /*
        private void OldAccessDiamondDocLine(string DocTypeID, string docID)
        {
            try
            {

                string query = @"SELECT DocHeaderID, DocLineID, DocTypeID, DocTypeDesc, DocID, DocLineDto, ItemID, LotName, 
                    DocTypeID, CertificateID, Shape, Size, Color, Clarity,
                    Weight, WeightLoss, Cost, TotalCost, SecCost, SecTotalCost, Sale, TotalSale, SecSale, SecTotalSale,
                    Additional, TotalAdditional, MainPrice, TotalMainPrice, SecPrice, TotalSecPrice, DocPrice,
                    TotalDocPrice, RateDocToMain, RateDocToSec, List, TotalList, ListCostDiscount, ListSaleDiscount,
                    ListMainDiscount, OutList, OutTotalList, OutMainPrice, OutTotalMainPrice, OutListMainDiscount,
                    OutSecPrice, OutSecTotalPrice, ReturnWeight, ReturnTotalCost, ReturnTotalSale, InvoicedWeight, InvoicedTotalCost,
                    InvoicedTotalSale, MainPriceGross,TotalMainPriceGross, SecPriceGross, TotalSecPriceGross, DocPriceGross, TotalDocPriceGross, 
                    DocPriceGrossWithTax, TotalDocPriceGrossWithTax, DocPriceWithTax,
                    TotalDocPriceWithTax, DocTotalDiscountWithTax, MarkupPercent, DiscountPercent, DocTotalDiscountAmount,
                    MainTotalDiscountAmount, SecTotalDiscountAmount, LotID, ParentLotID, 
                    TrsUnionID, SourceDocTypeID, SourceDocTypeDesc, SourceDocID,
                    SourceDocLine, SourceDocCode, DocLineStatus, DateTime_Modified, UserID,Remark 
                    FROM THXADiamondDocLine 
                    WHERE DocTypeID = @DocTypeID AND DocID = @DocID";

                var parameters = new Dictionary<string, object>
                {
                    { "@DocTypeID", DocTypeID },
                    { "@DocID", docID }
                };

                string errorMessageTitle = "Access Diamond DocLineDto";

                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(connectionString, query, errorMessageTitle, parameters);
                if (SqlQueryExecutor.IsValid(sqlResult))
                {
                
                }
            }
            catch (Exception e1)
            {
                SqlQueryExecutor.ShowErrorMessage(e1, "Access DocLineDto failure.");
            }


        }
        */


    }
}
