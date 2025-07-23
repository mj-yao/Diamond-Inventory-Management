using DiamondTransaction.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.DataAccess
{
    public class DiamondDocRepository
    {
        private readonly string _connectionString;
        //private Dictionary<string, string> docTypeDict = new Dictionary<string, string>(); // Cache
        private List<DocType> _docTypes;

        public DiamondDocRepository(string connectionString)
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
                DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(_connectionString, query, "Loading DocType Dictionary");

                docTypeDict.Clear();
                foreach (DataRow dr in sqlResult.Rows)
                {
                    docTypeDict[dr["DocTypeDesc"].ToString()] = dr["DocTypeID"].ToString();
                }
            }
            catch (Exception e)
            {
                GlobalClass.ShowErrorMessage(e, "Failed to load DocType dictionary.");
            }
        }
        */

        private void LoadDocTypes()
        {
            try
            {
                string query = "SELECT DocTypeID, DocTypeDesc FROM THXADiamondDocType";
                DataTable result = GlobalClass.AccessAndGetSqlResult(_connectionString, query, "Loading Doc Types");

                _docTypes = new List<DocType>();

                foreach (DataRow row in result.Rows)
                {
                    _docTypes.Add(new DocType
                    {
                        DocTypeID = Convert.ToInt32(row["DocTypeID"]),
                        DocTypeDesc = row["DocTypeDesc"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                GlobalClass.ShowErrorMessage(ex, "Failed to load DocTypes.");
                _docTypes = new List<DocType>();
            }
        }


        /*
        private string GetDocTypeIDByDocTypeDesc0(string description)
        {
            if (docTypeDict.TryGetValue(description, out string docTypeID))
            {
                return docTypeID; // Return cached value
            }

            // If not in cache, query database
            try
            {
                string query = $@"
                SELECT DocTypeID FROM THXADiamondDocType 
                WHERE DocTypeDesc = '{description}' 
                LIMIT 1";

                DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(_connectionString, query, "Access DocType ID");

                if (GlobalClass.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
                {
                    docTypeID = sqlResult.Rows[0]["DocTypeID"].ToString();
                    docTypeDict[description] = docTypeID; // Add to cache
                    return docTypeID;
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                GlobalClass.ShowErrorMessage(e, "Failed to retrieve DocTypeID.");
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

            DataTable result = GlobalClass.AccessAndGetSqlResult(_connectionString, query, errorMessageTitle, parameters);

            if (GlobalClass.IsValid(result))
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
                DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(_connectionString, query, "Access DocHeader latest DocID and DocCode");

                if (GlobalClass.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
                {
                    object maxDocIDValue = sqlResult.Rows[0]["MAXDocID"];
                    object maxDocCodeValue = sqlResult.Rows[0]["MAXDocCode"];

                    if (maxDocIDValue != DBNull.Value)
                    {
                        int maxDocID = Convert.ToInt32(maxDocIDValue);
                        string maxDocCode = maxDocCodeValue.ToString();

                        return new DocHeaderMaxInfo(maxDocID, maxDocCode);
                    }
                }

                return null; // Handle case where no data is found
            }
            catch (Exception e)
            {
                GlobalClass.ShowErrorMessage(e, "Access MAXDocID in THXADiamondDocHeader failure.");
                return null; // Handle error gracefully
            }
        }

        public DiamondLotMaxInfo AccessDiamondLotMaxItemLotIDAndCertificateID()
        {
            try
            {
                string query = @"
                    SELECT 
                        (SELECT MAX(ItemID) as MAXItemID FROM THXADiamondLot WHERE ItemID = LotID) AS MaxItemID,
                        (SELECT MAX(LotID) FROM THXADiamondLot) AS MaxLotID,
                        (SELECT MAX(CertificateID) FROM THXADiamondCertificate) AS MaxCertificateID";

                DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(_connectionString, query, "Access DiamondLot MAXLotID, MaxItemID, MaxCertificateID");

                if (GlobalClass.IsValid(sqlResult) && sqlResult.Rows.Count > 0)
                {
                    int maxItemID = Convert.ToInt32(sqlResult.Rows[0]["MaxItemID"]);
                    int maxLotID = Convert.ToInt32(sqlResult.Rows[0]["MaxLotID"]);
                    int maxCertificateID = Convert.ToInt32(sqlResult.Rows[0]["MaxCertificateID"]);

                    return new DiamondLotMaxInfo(maxItemID, maxLotID, maxCertificateID);
                }

                return null; // Handle case where no data is found
            }
            catch (Exception e)
            {
                GlobalClass.ShowErrorMessage(e, "Access ItemID, LotID, CertificateID in THXADiamondLot failure.");
                return null; // Handle error gracefully
            }
        }

    }
}
