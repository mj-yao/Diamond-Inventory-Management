using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiamondTransaction.UI.Controls
{
    internal class DocHeaderControls
    {
        public ComboBox DocTypeDescComboBox { get; set; }
        public TextBox DocTypeIDTextBox { get; set; }
        public ComboBox DocSubTypeComboBox { get; set; }
        public ComboBox AccountCodeComboBox { get; set; }
        public ComboBox AccountNameComboBox { get; set; }
        public ComboBox AccountBranchCodeComboBox { get; set; }
        public ComboBox CustomerPaymentTermComboBox { get; set; }


        public DocHeaderControls(ComboBox cb_docTypeDesc, TextBox tb_docTypeID, ComboBox cb_docSubType,
                               ComboBox cb_accountCode, ComboBox cb_accountName,
                               ComboBox cb_accountBranchCode, ComboBox cb_customerPaymentTerm)
        {
            DocTypeDescComboBox = cb_docTypeDesc;
            DocTypeIDTextBox = tb_docTypeID;
            DocSubTypeComboBox = cb_docSubType;
            AccountCodeComboBox = cb_accountCode;
            AccountNameComboBox = cb_accountName;
            AccountBranchCodeComboBox = cb_accountBranchCode;
            CustomerPaymentTermComboBox = cb_customerPaymentTerm;
        }
    }
}
