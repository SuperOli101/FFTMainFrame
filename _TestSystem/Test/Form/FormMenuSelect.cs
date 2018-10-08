using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Honeywell.Test;

namespace Honeywell.Forms
{
    public partial class CFormMenuSelect : Form
    {
        public CFormMenuSelect(CTest Test)
        {
            this.test = Test;
            InitializeComponent();
        }

        private void FormMenuSelect_Load(object sender, EventArgs e)
        {
            foreach (CTest.ModeFFT enumModeFFF in Enum.GetValues(typeof(CTest.ModeFFT)))
            {
                this.comboBoxTestMode.Items.Add(enumModeFFF.ToString());
            }
            this.comboBoxTestMode.SelectedIndex = this.test.Data.FFTMode;

            
            if (this.test.Data.MenuSorted == 1)
                this.listViewMenu.Sorting = System.Windows.Forms.SortOrder.Ascending;
            else
                this.listViewMenu.Sorting = System.Windows.Forms.SortOrder.None;

            if (this.test.Data.Scanner!=1)
                this.listViewMenu.Items[0].Selected = true; 
            if (this.test.Data.Scanner != 0)
            {
                this.textBoxOSNumber.Visible = true;
                this.textBoxOSNumber.Select();
            }
            else
            {
                this.textBoxOSNumber.Visible = false; 
            }                                   
        }

        private void listViewMenu_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.SetCurrentID_Menu();
            this.Close();
        }

        private void SetCurrentID_Menu()
        {
            int iIndex;
            String strOSNumber;
            System.Windows.Forms.ListView.SelectedListViewItemCollection hItemColection;
            System.Windows.Forms.ListViewItem hItem;

            if (this.comboBoxTestMode.SelectedIndex < 0)
                this.test.Data.FFTMode = 0;
            else
                this.test.Data.FFTMode = this.comboBoxTestMode.SelectedIndex;

            hItemColection = this.listViewMenu.SelectedItems;
            if (hItemColection.Count == 1)
            {
                hItem = this.listViewMenu.SelectedItems[0];
                strOSNumber = hItem.Text;              
                iIndex = this.test.Data.ID_MenuLIST.IndexOf(strOSNumber);
                if (iIndex != -1)
                {
                    this.test.CurrentID_Menu = iIndex + 1;
                }
                else
                    this.test.CurrentID_Menu = 0;
            }
            else
                this.test.CurrentID_Menu = 0;
        }

        private void FormMenuSelect_KeyDown(object sender, KeyEventArgs e)
        {
         
        }

        private void listViewMenu_DoubleClick(object sender, System.EventArgs e)
        {
            this.SetCurrentID_Menu();
            this.Close();
        }

        public void AddItem(System.String[] ItemARRAY)
        {
            System.Windows.Forms.ListViewItem hItem;

            hItem = new System.Windows.Forms.ListViewItem(ItemARRAY);
            hItem.Name = ItemARRAY[0];
            this.listViewMenu.Items.Add(hItem);

            this.columnHeaderOSNumber.Width = -1;//an das längste Element in der Spalte anpassen
            this.columnHeaderDescription.Width = -2;//die Breite automatisch an die Breite der Spaltenüberschrift angepasst werden soll            
        }

        private void textBoxOSNumber_TextChanged(object sender, EventArgs e)
        {                        
            if (this.test.Data.Scanner == -1)//Eingabe von der Tastatur
            {
                String strOSNumber, strName;
                int i;

                strOSNumber = this.textBoxOSNumber.Text;
                System.Windows.Forms.ListView.ListViewItemCollection hItemColection;
                hItemColection = this.listViewMenu.Items;
                i = 0;
                foreach (ListViewItem hItem in this.listViewMenu.Items)
                {
                    strName = hItem.Name;
                    if (strName.IndexOf(strOSNumber) == 0)
                    {
                        this.listViewMenu.Items[i].Selected = true;
                        break;
                    }
                    i++;
                }
            }
        }

        private void textBoxOSNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {
                if (this.test.Data.Scanner == -1)//Eingabe von der Tastatur
                {
                    this.SetCurrentID_Menu();
                    this.Close();
                }
                else//Eingabe mit dem Scanner
                {
                    String strOSNumber, strMsg;
                    foreach (ListViewItem hItem in this.listViewMenu.Items)
                    {
                        strOSNumber = hItem.Name;
                        if (strOSNumber.CompareTo(this.textBoxOSNumber.Text) == 0)
                        {
                            hItem.Selected = true;
                            this.SetCurrentID_Menu();
                            this.Close();
                            return;
                        }
                    }
                    strMsg = String.Format("Variant {0} is not available", this.textBoxOSNumber.Text);
                    MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.test.CurrentID_Menu = 0;
                    //this.Close();  
                }

            }
        }

        private void listViewMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {
                this.SetCurrentID_Menu();
                this.Close();
            }
        }
    }
}