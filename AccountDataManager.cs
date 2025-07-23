using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiamondTransaction.DataAccess;
using DiamondTransaction.Models;

namespace DiamondTransaction
{
    public class AccountDataManager
    {
        private readonly string _connectionString;

        // Properties to store data
        public List<string> SupplierCodeList { get; private set; }
        public List<string> CustomerCodeList { get; private set; }
        public List<string> SupplierNames { get; private set; }
        public List<string> CustomerNames { get; private set; }
        public List<decimal> CustomerPaymentDays { get; private set; }
        public List<string> LotNameList { get; private set; }

        public List<string> RapShapeList { get; private set; }
        public List<string> RapSizeList { get; private set; }
        public List<string> RapColorList { get; private set; }
        public List<string> RapClarityList { get; private set; }

        public int ExRateUSD2GBP { get; private set; }



        // Constructor
        public AccountDataManager(string connectionString)
        {
            _connectionString = connectionString;
            RapShapeList = InitialiseRapListShape();
            RapSizeList = InitialiseRapList("Size");
            RapColorList = InitialiseRapList("Color");
            RapClarityList = InitialiseRapList("Clarity");

        }

        public void InitialiseSupplierData()
        {
            string errorMessageTitle = "Access Supplier";
            string query = "SELECT SupplierCode, SupplierName FROM THPUSupplier";

            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, errorMessageTitle);
            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                SupplierCodeList = sqlResult.AsEnumerable()
                    .Select(row => row.Field<string>("SupplierCode"))
                    .ToList();

                SupplierNames = sqlResult.AsEnumerable()
                    .Select(row => row.Field<string>("SupplierName"))
                    .ToList();
            }
        }

        public void InitialiseCustomerData()
        {
            string errorMessageTitle = "Access Customer";
            string query = "SELECT CustomerCode, CustomerName, PaymentDays FROM THSLCustomer";

            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, errorMessageTitle);
            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                CustomerCodeList = sqlResult.AsEnumerable()
                    .Select(row => row.Field<string>("CustomerCode"))
                    .ToList();

                CustomerNames = sqlResult.AsEnumerable()
                    .Select(row => row.Field<string>("CustomerName"))
                    .ToList();

                CustomerPaymentDays = sqlResult.AsEnumerable()
                    .Select(row => row.Field<decimal>("PaymentDays"))
                    .ToList();
            }
        }

        public void InitialiseLotNameData()
        {
            string errorMessageTitle = "Access LotName";
            string query = "SELECT DISTINCT LotName FROM THXADiamondPriceStockHistory ORDER BY LotName ASC";

            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, errorMessageTitle);
            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                LotNameList = sqlResult.AsEnumerable()
                    .Select(row => row.Field<string>("LotName"))
                    .ToList();
            }
        }

        public List<string> InitialiseRapListShape()
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


        public List<string> InitialiseRapList(string grade)
        {
            string errorMessageTitle = "Access LotName";
            string query = $@"SELECT DISTINCT {grade} FROM THXADiamondRapList ORDER BY {grade} ASC";

            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, errorMessageTitle);
            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                return sqlResult.AsEnumerable()
                    .Select(row => row.Field<string>(grade))
                    .Where(value => !string.IsNullOrEmpty(value)) 
                    .ToList();
            }
            return new List<string>(); 
        }


       

        public static void SetUpComboBoxItem(ComboBox comboBox, List<string> items)
        {
            if (comboBox == null || items == null) return;

            comboBox.Items.Clear();
            comboBox.Items.AddRange(items.ToArray());
        }

        public void InitialiseSupplierComboBox(ComboBox cb_accountCode, ComboBox cb_accountName)
        {
            InitialiseSupplierData();
            SetUpComboBoxItem(cb_accountCode, SupplierCodeList);
            SetUpComboBoxItem(cb_accountName, SupplierNames);
        }

        public void InitialiseCustomerComboBox(ComboBox cb_accountCode, ComboBox cb_accountName)
        {
            InitialiseCustomerData();
            SetUpComboBoxItem(cb_accountCode, CustomerCodeList);
            SetUpComboBoxItem(cb_accountName, CustomerNames);
        }
        public void InitialiseLotNameComboBox(ComboBox cb_lotName)
        {
            InitialiseLotNameData();
            SetUpComboBoxItem(cb_lotName, LotNameList);
            SetupLotNameAutoComplete(cb_lotName);

        }
        private void SetupLotNameAutoComplete(ComboBox cb_lotName)
        {

            if (LotNameList == null || LotNameList.Count == 0) return;

            // Enable AutoComplete
            cb_lotName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb_lotName.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection autoCompleteData = new AutoCompleteStringCollection();
            autoCompleteData.AddRange(LotNameList.ToArray());

            cb_lotName.AutoCompleteCustomSource = autoCompleteData;

        }

        public void InitialiseParcelComboBoxGrading(string grading, ComboBox cb_grading)
        {

            string errorMessageTitle = "Access " + grading;
            string query = "SELECT " + grading + " FROM THXADiamondParcel" + grading + " ORDER BY " + grading + "Order";

            DataTable sqlResult = SqlQueryExecutor.AccessAndGetSqlResult(_connectionString, query, errorMessageTitle);
            if (SqlQueryExecutor.IsValid(sqlResult))
            {
                List<string> gradeList = new List<string>();
                gradeList = (from item in sqlResult.AsEnumerable()
                             select item.Field<string>(grading)).ToList();


                ComboBox comboBox = cb_grading;
                if (comboBox != null)
                {
                    comboBox.Items.Clear();
                    for (int i = 0; i < gradeList.Count; i++)
                        comboBox.Items.Add(gradeList[i]);
                }
            }

        }

    }
}

