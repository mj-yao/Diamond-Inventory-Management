using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction
{

    public class SetDocumentInfo
    {
        private readonly List<int> supplierDocTypes;
        private readonly string connectionString;
        private readonly Dictionary<string, string> docTypeDict;
        private readonly List<string> SupplierCodeList;
        private readonly List<string> supplierNames;
        private readonly List<string> CustomerCodeList;
        private readonly List<string> customerNames;
        private readonly List<int> customerPaymentDays;
        private bool isProgrammaticChange = false;

        // Constructor
        public SetDocumentInfo(
            List<int> supplierDocTypes,
            string connectionString,
            Dictionary<string, string> docTypeDict,
            List<string> supplierCodeList,
            List<string> supplierNames,
            List<string> customerCodeList,
            List<string> customerNames,
            List<int> customerPaymentDays)
        {
            this.supplierDocTypes = supplierDocTypes;
            this.connectionString = connectionString;
            this.docTypeDict = docTypeDict;
            this.SupplierCodeList = supplierCodeList;
            this.supplierNames = supplierNames;
            this.CustomerCodeList = customerCodeList;
            this.customerNames = customerNames;
            this.customerPaymentDays = customerPaymentDays;
        }

        // Utility Methods
        public void SetUpComboBoxItem(ComboBox comboBox, List<string> items)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(items.ToArray());
        }

        private void HandleProgrammaticChange(Action action)
        {
            if (isProgrammaticChange) return;

            isProgrammaticChange = true;
            action.Invoke();
            isProgrammaticChange = false;
        }

        private string GetDocTypeID(ComboBox docTypeDesc)
        {
            return docTypeDict.FirstOrDefault(x => x.Value == docTypeDesc.Text.Trim()).Key;
        }

        // Supplier-Specific Methods
        private int GetSupplierCodeIndex(string supplierCode) => SupplierCodeList.IndexOf(supplierCode);

        private int GetSupplierNameIndex(string supplierName) => supplierNames.IndexOf(supplierName);

        private void FillSupplierName(ComboBox accountCode, ComboBox accountName)
        {
            int index = GetSupplierCodeIndex(accountCode.Text.Trim());
            accountName.Text = index >= 0 ? supplierNames[index] : string.Empty;
        }

        private void FillSupplierCode(ComboBox accountName, ComboBox accountCode)
        {
            int index = GetSupplierNameIndex(accountName.Text);
            accountCode.Text = index >= 0 ? SupplierCodeList[index] : string.Empty;
        }

        // Customer-Specific Methods
        private int GetCustomerCodeIndex(string customerCode) => CustomerCodeList.IndexOf(customerCode);

        private int GetCustomerNameIndex(string customerName) => customerNames.IndexOf(customerName);

        private void FillCustomerNameFromCode(ComboBox accountCode, ComboBox accountName)
        {
            int index = GetCustomerCodeIndex(accountCode.Text.Trim());
            accountName.Text = index >= 0 ? customerNames[index] : string.Empty;
        }

        private void FillCustomerCodeFromName(ComboBox accountName, ComboBox accountCode)
        {
            int index = GetCustomerNameIndex(accountName.Text);
            accountCode.Text = index >= 0 ? CustomerCodeList[index] : string.Empty;
        }

        private void FillCustomerPaymentTerm(ComboBox accountCode, ComboBox paymentTerm)
        {
            int index = GetCustomerCodeIndex(accountCode.Text.Trim());
            paymentTerm.Text = index >= 0 ? customerPaymentDays[index].ToString() : string.Empty;
        }

        private void FillCustomerBranch(string customerCode, ComboBox accountBranchCode)
        {
            try
            {
                string query = @"SELECT CustomerCode, BranchCode FROM THSLCustomerBranch WHERE CustomerCode = @CustomerCode";
                var parameters = new Dictionary<string, object>
            {
                { "@CustomerCode", customerCode },
            };

                DataTable sqlResult = GlobalClass.AccessAndGetSqlResult(connectionString, query, "Access CustomerBranch", parameters);

                if (GlobalClass.IsValid(sqlResult))
                {
                    var customerBranchList = sqlResult.AsEnumerable()
                        .Select(row => row.Field<string>("BranchCode"))
                        .ToList();

                    SetUpComboBoxItem(accountBranchCode, customerBranchList);
                    accountBranchCode.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                GlobalClass.ShowErrorMessage(ex, "Access CustomerBranch failure.");
            }
        }

        // Event Handlers
        public void HandleAccountCodeTextChanged(ComboBox doctypeDesc,ComboBox accountCode, ComboBox accountName, ComboBox accountBranchCode, ComboBox paymentTerm)
        {
            if (accountCode.Text.Trim() == "")
            {
                accountName.Text = "";
            }
            else
            {
                HandleProgrammaticChange(() => FillAccountName(doctypeDesc, accountCode, accountName, accountBranchCode, paymentTerm));
            }
        }

        public void HandleAccountNameTextChanged(ComboBox doctypeDesc,ComboBox accountName, ComboBox accountCode, ComboBox accountBranchCode, ComboBox paymentTerm)
        {
            if (accountName.Text.Trim() == "")
            {
                accountCode.Text = "";
            }
            else
            {
                HandleProgrammaticChange(() => FillAccountCode(doctypeDesc,accountName, accountCode, accountBranchCode, paymentTerm));
            }
        }

        private void FillAccountName(ComboBox doctypeDesc, ComboBox accountCode, ComboBox accountName, ComboBox accountBranchCode, ComboBox paymentTerm)
        {
            string docTypeID = GetDocTypeID(doctypeDesc);

            if (int.TryParse(docTypeID, out int docTypeIDInt))
            {
                if (supplierDocTypes.Contains(docTypeIDInt))
                {
                    FillSupplierName(accountCode, accountName);
                }
                else
                {
                    FillCustomerBranch(accountCode.Text, accountBranchCode);
                    FillCustomerNameFromCode(accountCode, accountName);
                    FillCustomerPaymentTerm(accountCode, paymentTerm);
                }
            }
        }

        private void FillAccountCode( ComboBox doctypeDesc, ComboBox accountName, ComboBox accountCode, ComboBox accountBranchCode, ComboBox paymentTerm)
        {
            string docTypeID = GetDocTypeID(doctypeDesc);

            if (int.TryParse(docTypeID, out int docTypeIDInt))
            {
                if (supplierDocTypes.Contains(docTypeIDInt))
                {
                    FillSupplierCode(accountName, accountCode);
                }
                else
                {
                    FillCustomerCodeFromName(accountName, accountCode);
                    FillCustomerBranch(accountCode.Text, accountBranchCode);
                    FillCustomerPaymentTerm(accountCode, paymentTerm);
                }
            }
        }
    }

}
