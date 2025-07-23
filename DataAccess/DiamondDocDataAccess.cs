using DiamondTransaction.UseCases.Models;
using DiamondTransaction.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondTransaction.UseCases.Interfaces;
using System.Windows.Forms;
using DiamondTransaction.UI.Utilities;

namespace DiamondTransaction.DataAccess
{
    public class DiamondDocDataAccess: IDiamondDocRepository
    {
        private readonly string _connectionString;
        //private Dictionary<string, string> docTypeDict = new Dictionary<string, string>(); // Cache
        private List<DocType> _docTypes;

        public DiamondDocDataAccess(string connectionString)
        {
            _connectionString = connectionString;        
            LoadDocTypes();
        }

        /*
        private void LoadDocTypeDictionary()
        {
            try
            {
                string query = "SELECT DISTINCT DocTypeID, DocTypeDesc FROM THXADiamondDocType";
                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, "Loading DocType Dictionary");

                docTypeDict.Clear();
                foreach (DataRow dr in sqlResult.Rows)
                {
                    docTypeDict[dr["DocTypeDesc"].ToString()] = dr["DocTypeID"].ToString();
                }
            }
            catch (Exception e)
            {
                SqlQueryExecutor.ShowErrorMessage(e, "Failed to load DocType dictionary.");
            }
        }
        */

        public void LoadDocTypes()
        {
            try
            {
                string query = "SELECT DocTypeID, DocTypeDesc, DocBaseTypeID FROM THXADiamondDocType";
                DataTable result = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, "Loading Doc Types");

                _docTypes = new List<DocType>();

                foreach (DataRow row in result.Rows)
                {
                    _docTypes.Add(new DocType
                    {
                        DocTypeID = Convert.ToInt32(row["DocTypeID"]),
                        DocTypeDesc = row["DocTypeDesc"].ToString(),
                        DocTypeName = row["DocBaseTypeID"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                SqlQueryExecutor.ShowErrorMessage(ex, "Failed to load DocTypes.");
                _docTypes = new List<DocType>();
            }
        }


        /*
        private string GetDocTypeIDByDocTypeDesc0(string description)
        {
            if (docTypeDict.TryGetValue(description, out string DocTypeID))
            {
                return DocTypeID; // Return cached value
            }

            // If not in cache, query database
            try
            {
                string query = $@"
                SELECT DocTypeID FROM THXADiamondDocType 
                WHERE DocTypeDesc = '{description}' 
                LIMIT 1";

                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, "Access DocType ID");

                if (SqlQueryExecutor.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
                {
                    DocTypeID = sqlResult.Rows[0]["DocTypeID"].ToString();
                    docTypeDict[description] = DocTypeID; // Add to cache
                    return DocTypeID;
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                SqlQueryExecutor.ShowErrorMessage(e, "Failed to retrieve DocTypeID.");
                return string.Empty;
            }
        }
        */

        public string GetDocTypeIDByDocTypeDesc(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return string.Empty;

            var match = _docTypes.FirstOrDefault(d =>
                string.Equals(d.DocTypeDesc, description, StringComparison.OrdinalIgnoreCase));

            return match?.DocTypeID.ToString() ?? string.Empty;
        }

        public string GetDocTypeDescById(int docTypeId)
        {
            return _docTypes.FirstOrDefault(d => d.DocTypeID == docTypeId)?.DocTypeDesc;
        }

        public List<DocType> GetAllDocTypes()
        {
            return _docTypes;
        }

        public List<int> GetSupplierDocTypeIDs()
        {
            var keywords = new[] { "PURCHASE NOTE", "PURCHASE NOTE RETURN", "MEMO IN", "MEMO IN RETURN", "PURCHASE MEMO IN" };

            return _docTypes
                .Where(d => keywords.Any(k => d.DocTypeDesc?.ToUpper().Contains(k) ?? false))
                .Select(d => d.DocTypeID)
                .ToList();
        }

        public List<string> GetDocSubTypesByDocType(string docTypeDesc)
        {
            var docSubTypes = new List<string>();

            string query = "SELECT DISTINCT DocSubType FROM THXADiamondDocHeader WHERE DocTypeDesc = @DocTypeDesc";
            string errorMessageTitle = "Access DocType";

            var parameters = new Dictionary<string, object>
            {
                { "@DocTypeDesc", docTypeDesc },
            };

            DataTable result = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, errorMessageTitle, parameters);

            if (SqlQueryExecutor.IsValid(result))
            {
                docSubTypes = (from row in result.AsEnumerable()
                               select row.Field<string>("DocSubType")).ToList();
            }

            return docSubTypes;
        }



        public DocHeaderMaxInfo AccessDocHeaderMaxDocIDAndDocCodeFor(string docTypeDesc)
        {
            string DocTypeID = GetDocTypeIDByDocTypeDesc(docTypeDesc);
            try
            {
                string query = $"SELECT TOP 1 DocID as MAXDocID, DocCode as MAXDocCode FROM THXADiamondDocHeader WHERE DocTypeID = '{DocTypeID}' ORDER BY DocID DESC ";
                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, "Access DocHeader latest DocID and DocCode");

                if (SqlQueryExecutor.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
                {
                    object maxDocIDValue = sqlResult.Rows[0]["MAXDocID"];
                    object maxDocCodeValue = sqlResult.Rows[0]["MAXDocCode"];

                    if (maxDocIDValue != DBNull.Value)
                    {
                        int maxDocID = Convert.ToInt32(maxDocIDValue);
                        string maxDocCode = maxDocCodeValue.ToString();

                        return new DocHeaderMaxInfo(maxDocID, maxDocCode);
                    }
                    else 
                    {                     
                        return new DocHeaderMaxInfo(0, string.Empty);
                    }
                }

                return new DocHeaderMaxInfo(0, string.Empty); 
            }
            catch (Exception e)
            {
                SqlQueryExecutor.ShowErrorMessage(e, "Access MAXDocID in THXADiamondDocHeader failure.");
                return new DocHeaderMaxInfo(0, string.Empty);
            }
        }

        public DiamondLotMaxID AccessDiamondLotMaxItemLotIDAndCertificateID()
        {
            try
            {
                string query = @"
                    SELECT 
                        (SELECT MAX(ItemID) as MAXItemID FROM THXADiamondLot WHERE ItemID = LotID) AS _maxItemID,
                        (SELECT MAX(LotID) FROM THXADiamondLot) AS _maxLotID,
                        (SELECT MAX(CertificateID) FROM THXADiamondCertificate) AS _maxCertificateID";

                DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, "Access DiamondLot MAXLotID, _maxItemID, _maxCertificateID");

                if (SqlQueryExecutor.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
                {
                    int maxItemID = Convert.ToInt32(sqlResult.Rows[0]["_maxItemID"]);
                    int maxLotID = Convert.ToInt32(sqlResult.Rows[0]["_maxLotID"]);
                    int maxCertificateID = Convert.ToInt32(sqlResult.Rows[0]["_maxCertificateID"]);

                    return new DiamondLotMaxID(maxItemID, maxLotID, maxCertificateID);
                }

                return new DiamondLotMaxID(0,0,0);
            }
            catch (Exception e)
            {
                SqlQueryExecutor.ShowErrorMessage(e, "Access ItemID, LotID, CertificateID in THXADiamondLot failure.");
                return new DiamondLotMaxID(0,0,0); 
            }
        }

        public ExchangeRate GetLatestExchangeRate()
        {
            try
            {
                string query = "SELECT TOP 1 USD AS USDRate FROM THXADiamondExchangeRate ORDER BY DateTime_Created DESC";
                var result = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, "Access Exchange Rate");

                if (SqlQueryExecutor.IsValid(result) && result.Rows.Count > 0)
                {
                    decimal usdRate = FormControlHelper.GetDecimalValue(result.Rows[0]["USDRate"].ToString());
                    return new ExchangeRate { USDRate = usdRate };
                }

                return new ExchangeRate { USDRate = 0 };
            }
            catch (Exception ex)
            {
                SqlQueryExecutor.ShowErrorMessage(ex, "Access Exchange rate failure.");
                return new ExchangeRate { USDRate = 0 };
            }
        }


        public List<string> GetParcelGradingScale(string gradeType)
        {

            string errorMessageTitle = "Access " + gradeType;
            string query = "SELECT " + gradeType + " FROM THXADiamondParcel" + gradeType + " ORDER BY " + gradeType + "Order";

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

        public ParcelGrades GetAllParcelGradingScales()
        {
            return new ParcelGrades
            {
                Shapes = GetParcelGradingScale("Shape"),
                Sizes = GetParcelGradingScale("Size"),
                Colors = GetParcelGradingScale("Color"),
                Clarities = GetParcelGradingScale("Clarity")
            };
        }

    }
}
