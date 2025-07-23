using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DiamondTransaction.DataAccess
{
    public class DiamondLotDataAccess: IDiamondLotRepository
    {
        private readonly string _connectionString;
        public DiamondLotDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<DiamondLotDto> GetDiamondLotAsync(string identifier, string searchType)
        {
            string columnName = searchType == "LotID" ? "LotID" : "LotName";
            string query = $@"
            SELECT  
                LotName, ItemDescription, ParcelOrStone, Weight, Shape, Size, Color, Clarity, 
                ItemID, LotID, Cost, GBPSale, Sale, GBPOutPrice, StockStatus,
                LocationAccountName, LocationAccountCode, VendorAccountCode, VendorAccountName,
                HoldingType, Status, DateTime_Created,
                WeightLoss, ScrapWeight,
                PriceStockHistory.ParcelAvgCost as ParcelAvgCost
            FROM THXADiamondLot
            OUTER APPLY (
                SELECT TOP 1 ParcelAvgCost 
                FROM THXADiamondPriceStockHistory 
                WHERE THXADiamondPriceStockHistory.LotID = THXADiamondLot.LotID 
                ORDER BY TrsDate DESC
            ) AS PriceStockHistory
            WHERE {columnName} = @Identifier
            ORDER BY ItemName";

            using (var connection = new SqlConnection(_connectionString))
                return (await connection.QueryFirstOrDefaultAsync<DiamondLotDto>(query, new { Identifier = identifier }));
        }

        public async Task<int> InsertDiamondLotAsync(DiamondLotDto dto)
        {
            string sql = @"
            INSERT INTO THXADiamondLot (
                LotID, LotName, ItemID, ItemName, ItemDescription, ItemDescription1, ParcelOrStone, HoldingType, 
                Status, StockStatus, Weight, Shape, Size, Color, Clarity, Cut, Polish, Symmetry, Fluorescence, 
                Inscription, CertificateID, CertificateLabName, CertificateType, CertificateNo, CertificateDate, 
                Cost, TotalCost, ListCostDiscount, List, TotalList, ListSaleDiscount, Sale, TotalSale, GBPSale, 
                GBPTotalSale, OutList, ListOutDiscount, OutPrice, OutTotalPrice, GBPOutPrice, GBPOutTotalPrice, 
                LocationAccountCode, LocationAccountName, VendorAccountCode, VendorAccountName, 
                WeightLoss, ScrapWeight, Remark, Created_By, DateTime_Created, Modified_By, 
                LastStockStatusUpdate, ReferenceDocCode, LastTrsID, LastTrsDate, LastTrsTypeID, LastTrsTypeDesc
            )
            VALUES (
                @LotID, @LotName, @ItemID, @ItemName, @ItemDescription, @ItemDescription1, @ParcelOrStone, @HoldingType, 
                @Status, @StockStatus, @Weight, @Shape, @Size, @Color, @Clarity, @Cut, @Polish, @Symmetry, @Fluorescence, 
                @Inscription, @CertificateID, @CertificateLabName, @CertificateType, @CertificateNo, @CertificateDate, 
                @Cost, @TotalCost, @ListCostDiscount, @List, @TotalList, @ListSaleDiscount, @Sale, @TotalSale, @GBPSale, 
                @GBPTotalSale, @OutList, @ListOutDiscount, @OutPrice, @OutTotalPrice, @GBPOutPrice, @GBPOutTotalPrice, 
                @LocationAccountCode, @LocationAccountName, @VendorAccountCode, @VendorAccountName, 
                @WeightLoss, @ScrapWeight, @Remark, @Created_By, @DateTime_Created, @Modified_By, 
                @LastStockStatusUpdate, @ReferenceDocCode, @LastTrsID, @LastTrsDate, @LastTrsTypeID, @LastTrsTypeDesc
            );
            SELECT @LotID;
        ";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                int lotId = await connection.ExecuteScalarAsync<int>(sql, dto);
                return lotId;
            }
        }

        public async Task<bool> UpdateDiamondLotAsync(DiamondLotDto dto)
        {
            string sql = @"
            UPDATE THXADiamondLot
            SET 
                LotName = @LotName,
                ItemID = @ItemID,
                ItemName = @ItemName,
                ItemDescription = @ItemDescription,
                ItemDescription1 = @ItemDescription1,
                ParcelOrStone = @ParcelOrStone,
                HoldingType = @HoldingType,
                Status = @Status,
                StockStatus = @StockStatus,
                Weight = @Weight,
                Shape = @Shape,
                Size = @Size,
                Color = @Color,
                Clarity = @Clarity,
                Cut = @Cut,
                Polish = @Polish,
                Symmetry = @Symmetry,
                Fluorescence = @Fluorescence,
                Inscription = @Inscription,
                CertificateID = @CertificateID,
                CertificateLabName = @CertificateLabName,
                CertificateType = @CertificateType,
                CertificateNo = @CertificateNo,
                CertificateDate = @CertificateDate,
                Cost = @Cost,
                TotalCost = @TotalCost,
                ListCostDiscount = @ListCostDiscount,
                List = @List,
                TotalList = @TotalList,
                ListSaleDiscount = @ListSaleDiscount,
                Sale = @Sale,
                TotalSale = @TotalSale,
                GBPSale = @GBPSale,
                GBPTotalSale = @GBPTotalSale,
                OutList = @OutList,
                ListOutDiscount = @ListOutDiscount,
                OutPrice = @OutPrice,
                OutTotalPrice = @OutTotalPrice,
                GBPOutPrice = @GBPOutPrice,
                GBPOutTotalPrice = @GBPOutTotalPrice,
                LocationAccountCode = @LocationAccountCode,
                LocationAccountName = @LocationAccountName,
                VendorAccountCode = @VendorAccountCode,
                VendorAccountName = @VendorAccountName,
                WeightLoss = @WeightLoss,
                ScrapWeight = @ScrapWeight,
                Remark = @Remark,
                Modified_By = @Modified_By,
                LastStockStatusUpdate = @LastStockStatusUpdate,
                ReferenceDocCode = @ReferenceDocCode,
                LastTrsID = @LastTrsID,
                LastTrsDate = @LastTrsDate,
                LastTrsTypeID = @LastTrsTypeID,
                LastTrsTypeDesc = @LastTrsTypeDesc
            WHERE LotID = @LotID;
        ";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                int affectedRows = await connection.ExecuteAsync(sql, dto);
                return affectedRows > 0;
            }
        }


        public async Task<int> UpsertDiamondLotAsync(DiamondLotDto dto)
        {
            string sql = @"
                IF EXISTS (SELECT 1 FROM THXADiamondLot WHERE LotID = @LotID)
                BEGIN
                    UPDATE THXADiamondLot
                    SET 
                        LotName = @LotName,
                        ItemID = @ItemID,
                        ItemName = @ItemName,
                        ItemDescription = @ItemDescription,
                        ItemDescription1 = @ItemDescription1,
                        ParcelOrStone = @ParcelOrStone,
                        HoldingType = @HoldingType,
                        Status = @Status,
                        StockStatus = @StockStatus,
                        Weight = @Weight,
                        Shape = @Shape,
                        Size = @Size,
                        Color = @Color,
                        Clarity = @Clarity,
                        Cut = @Cut,
                        Polish = @Polish,
                        Symmetry = @Symmetry,
                        Fluorescence = @Fluorescence,
                        Inscription = @Inscription,
                        CertificateID = @CertificateID,
                        CertificateLabName = @CertificateLabName,
                        CertificateType = @CertificateType,
                        CertificateNo = @CertificateNo,
                        CertificateDate = @CertificateDate,
                        Cost = @Cost,
                        TotalCost = @TotalCost,
                        ListCostDiscount = @ListCostDiscount,
                        List = @List,
                        TotalList = @TotalList,
                        ListSaleDiscount = @ListSaleDiscount,
                        Sale = @Sale,
                        TotalSale = @TotalSale,
                        GBPSale = @GBPSale,
                        GBPTotalSale = @GBPTotalSale,
                        OutList = @OutList,
                        ListOutDiscount = @ListOutDiscount,
                        OutPrice = @OutPrice,
                        OutTotalPrice = @OutTotalPrice,
                        GBPOutPrice = @GBPOutPrice,
                        GBPOutTotalPrice = @GBPOutTotalPrice,
                        LocationAccountCode = @LocationAccountCode,
                        LocationAccountName = @LocationAccountName,
                        VendorAccountCode = @VendorAccountCode,
                        VendorAccountName = @VendorAccountName,
                        WeightLoss = @WeightLoss,
                        ScrapWeight = @ScrapWeight,
                        Remark = @Remark,
                        Modified_By = @Modified_By,
                        LastStockStatusUpdate = @LastStockStatusUpdate,
                        ReferenceDocCode = @ReferenceDocCode,
                        LastTrsID = @LastTrsID,
                        LastTrsDate = @LastTrsDate,
                        LastTrsTypeID = @LastTrsTypeID,
                        LastTrsTypeDesc = @LastTrsTypeDesc
                    WHERE LotID = @LotID;
                END
                ELSE
                BEGIN
                    INSERT INTO THXADiamondLot (
                        LotID, LotName, ItemID, ItemName, ItemDescription, ItemDescription1, ParcelOrStone, HoldingType, 
                        Status, StockStatus, Weight, Shape, Size, Color, Clarity, Cut, Polish, Symmetry, Fluorescence, 
                        Inscription, CertificateID, CertificateLabName, CertificateType, CertificateNo, CertificateDate, 
                        Cost, TotalCost, ListCostDiscount, List, TotalList, ListSaleDiscount, Sale, TotalSale, GBPSale, 
                        GBPTotalSale, OutList, ListOutDiscount, OutPrice, OutTotalPrice, GBPOutPrice, GBPOutTotalPrice, 
                        LocationAccountCode, LocationAccountName, VendorAccountCode, VendorAccountName, 
                        WeightLoss, ScrapWeight, Remark, Created_By, DateTime_Created, Modified_By, 
                        LastStockStatusUpdate, ReferenceDocCode, LastTrsID, LastTrsDate, LastTrsTypeID, LastTrsTypeDesc
                    ) VALUES (
                        @LotID, @LotName, @ItemID, @ItemName, @ItemDescription, @ItemDescription1, @ParcelOrStone, @HoldingType, 
                        @Status, @StockStatus, @Weight, @Shape, @Size, @Color, @Clarity, @Cut, @Polish, @Symmetry, @Fluorescence, 
                        @Inscription, @CertificateID, @CertificateLabName, @CertificateType, @CertificateNo, @CertificateDate, 
                        @Cost, @TotalCost, @ListCostDiscount, @List, @TotalList, @ListSaleDiscount, @Sale, @TotalSale, @GBPSale, 
                        @GBPTotalSale, @OutList, @ListOutDiscount, @OutPrice, @OutTotalPrice, @GBPOutPrice, @GBPOutTotalPrice, 
                        @LocationAccountCode, @LocationAccountName, @VendorAccountCode, @VendorAccountName, 
                        @WeightLoss, @ScrapWeight, @Remark, @Created_By, @DateTime_Created, @Modified_By, 
                        @LastStockStatusUpdate, @ReferenceDocCode, @LastTrsID, @LastTrsDate, @LastTrsTypeID, @LastTrsTypeDesc
                    );
                END;

                SELECT @LotID;
            ";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                int lotId = await connection.ExecuteScalarAsync<int>(sql, dto);
                return lotId;
            }
        }

    }
}
