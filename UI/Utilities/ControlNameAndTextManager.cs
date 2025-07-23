using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;


namespace DiamondTransaction
{
    public class ControlNameAndTextManager
    {
        public List<ControlNameAndText> InitialControlNameAndTexts { get; set; } = new List<ControlNameAndText>();
        public List<ControlNameAndText> EditedControlNameAndTexts { get; set; } = new List<ControlNameAndText>();


        public class ControlNameAndText
        {
            public string Name { get; set; }
            public string Text { get; set; }
        }


        //Fill in the ControlNameAndText List
        public void FillInitAndEditedControlListFromPanel(Panel panel)
        {
            appendTextControlsInPanel(panel);
            setDefaultTextControlBackColorInPanel(panel);
        }

        public void appendTextControlsInPanel(Panel panel)
        {

            foreach (Control control in panel.Controls)
            {
                if (control.GetType() == typeof(Panel) ||
                    control.GetType() == typeof(GroupBox))
                {
                    foreach (Control subcontrol in control.Controls)
                    {
                        if (IsTextControl(subcontrol))
                            addControlToInitialAndEditedList(subcontrol);
                    }
                }
                else if (IsTextControl(control))
                    addControlToInitialAndEditedList(control);
            }
        }
        private static bool IsTextControl(Control control)
        {
            if (control.GetType() == typeof(TextBox) ||
                control.GetType() == typeof(MaskedTextBox) ||
                control.GetType() == typeof(ComboBox) ||
                control.GetType() == typeof(DateTimePicker))
                return true;
            else
                return false;
        }

        public void addControlToInitialAndEditedList(Control control)
        {

            ControlNameAndText controlForInitialList = new ControlNameAndText
            {
                Name = control.Name,
                Text = control.Text
            };
            ControlNameAndText controlForEditedList = new ControlNameAndText
            {
                Name = control.Name,
                Text = control.Text
            };
            InitialControlNameAndTexts.Add(controlForInitialList);
            EditedControlNameAndTexts.Add(controlForEditedList);
        }

        private static void setDefaultTextControlBackColorInPanel(Control container)
        {
            foreach (Control control in container.Controls)
            {
                if (control.GetType() == typeof(Panel) || control.GetType() == typeof(GroupBox))
                {

                    foreach (Control subcontrol in control.Controls)
                    {
                        if (IsTextControl(subcontrol))
                            subcontrol.BackColor = SystemColors.Control;
                    }
                }
                else if (IsTextControl(control))
                    control.BackColor = SystemColors.Control;
            }

        }


        public void setControlTextEmptyInPanel(Control container)
        {
            foreach (Control control in container.Controls)
            {
                if (control.GetType() == typeof(Panel) || control.GetType() == typeof(GroupBox))
                {

                    foreach (Control subcontrol in control.Controls)
                    {
                        if (IsTextControl(subcontrol))
                            subcontrol.Text = String.Empty;
                        if (subcontrol is TextBox)
                        {
                            TextBox tb_subcontrol = (TextBox)subcontrol;
                            tb_subcontrol.ReadOnly = false;
                        }
                    }
                }
                else if (IsTextControl(control))
                {
                    control.Text = String.Empty;
                    if (control is TextBox)
                    {
                        TextBox tb_control = (TextBox)control;
                        tb_control.ReadOnly = false;
                    }
                }

            }

        }

        //UpdateText in the ControlNameAndText List

        public void UpdateTextInEditedControlList(Control EditedControl)//
        {
            int nameIndex = FindEditedTextBoxNameIndex(EditedControl.Name);
            if (nameIndex != -1)
            {
                EditedControlNameAndTexts[nameIndex].Text = EditedControl.Text;
                //MessageBox.Show(string.Format("after init{0}, edit{1}, index{2}", InitialControlNameAndTexts[nameIndex].Text, EditedControlNameAndTexts[nameIndex].Text,nameIndex));
            }
            else
            {
                MessageBox.Show("FindEditedControlNameIndex Error");
            }

        }
        public void SetEditedControlBackColor(Control EditedControl)
        {
            if (isEditedTextDifferentFromInitial(EditedControl))
            {
                setControlBackColor(EditedControl, Color.LightGreen);
            }
            else
            {
                setControlBackColor(EditedControl, SystemColors.Control);
            }
        }
        public void SetEditedComboBoxBackColor(ComboBox EditedComboBox)
        {
            if (isEditedTextDifferentFromInitial(EditedComboBox))
            {
                setControlBackColor(EditedComboBox, Color.LightGreen);
            }
            else
            {
                setControlBackColor(EditedComboBox, SystemColors.Control);
            }
        }
        public void SetRelatedTextBoxBackColor(Control EditedControl, TextBox RelatedTextBox)
        {
            if (isEditedTextDifferentFromInitial(EditedControl))
            {
                setControlBackColor(RelatedTextBox, Color.LightGreen);
            }
            else
            {
                setControlBackColor(RelatedTextBox, SystemColors.Control);
            }
        }
        public bool isEditedTextDifferentFromInitial(Control control)
        {
            int nameIndex = FindEditedTextBoxNameIndex(control.Name);

            if (nameIndex != -1)
            {
                //if (!tb.Text.Equals(InitialControlNameAndTexts[nameIndex].Text))
                //MessageBox.Show(string.Format("before init{0}, edit{1}, index{2}", InitialControlNameAndTexts[nameIndex].Text, EditedControlNameAndTexts[nameIndex].Text, nameIndex));
                if (!InitialControlNameAndTexts[nameIndex].Text.Equals(control.Text))
                    return true;
                else
                    return false;
            }
            else
            {
                MessageBox.Show("FindEditedControlNameIndex Error");
                return false;
            }
        }
        private int FindEditedTextBoxNameIndex(string name)//
        {
            int controlNameIndex = EditedControlNameAndTexts.FindIndex(0, EditedControlNameAndTexts.Count,
                delegate (ControlNameAndText controlNameAndText) {
                    return controlNameAndText.Name == name;
                });
            return controlNameIndex;
        }

        public static void setControlBackColor(Control control, Color color)
        {
            control.BackColor = color;
        }

        public bool IsAnyControlTextChanged()
        {

            for (int i = 0; i < InitialControlNameAndTexts.Count; i++)
            {
                if (InitialControlNameAndTexts[i].Name.Equals(EditedControlNameAndTexts[i].Name) &&
                    !InitialControlNameAndTexts[i].Text.Equals(EditedControlNameAndTexts[i].Text))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
