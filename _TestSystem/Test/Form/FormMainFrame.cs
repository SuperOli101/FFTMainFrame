using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Honeywell.Forms
{
    public partial class CFormMainFrame : Form
    {
        public CFormMainFrame(Honeywell.Test.CTest Test)
        {
            this.editorFileFullname = null;
            this.editorFlagToSave = false;
            

            this.Test = Test;
            this.Test.FormFFT = this;
            this.DisplayARRAY = new String[10];
            for (int i = 0; i < this.DisplayARRAY.Length;i++)
            {
                this.DisplayARRAY[i] = "";
            }
            
            InitializeComponent();
            Test.SetTestTimeMax(Test.TestTimeMax);
            this.StateDruckenToolStripButton(false);
        }

        private void buttonFFTStart_Click(object sender, EventArgs e)
        {

            this.Test.Execute();
            /*
            this.pictureBoxFFTState.Image = global::_MainFrame.Properties.Resources.run;

            String[] strMsgARRAY = new String[]{"Oliver","Gebert1111111111111","Oli","GO"};
            this.AddRowToListViewFFT(strMsgARRAY);
            this.AddRowToListViewFFT("Matze", "", "", "");
             * */
        }

        private void FormMainFrame_Load(object sender, EventArgs e)
        {
            int iIndex;
            String strVariant;

            this.speichernToolStripButton.Enabled = false;
            this.speichernToolStripMenuItem.Enabled = false;
            this.speichernunterToolStripMenuItem.Enabled = false;
            this.toolStripButtonFileClose.Visible = false;

            //CommBox in Menu setzen auf SelectMenu Index //////////////////////////
            foreach (String Variant in this.Test.Data.ID_MenuLIST)
            {
                this.toolStripComboBoxSelectVariant.Items.Add(Variant);               
            }
            this.toolStripComboBoxSelectVariant.Sorted = true;
            strVariant=this.Test.Data.ID_MenuLIST[this.Test.CurrentID_Menu - 1];
            iIndex = this.toolStripComboBoxSelectVariant.Items.IndexOf(strVariant);
            this.toolStripComboBoxSelectVariant.SelectedIndex = iIndex;            
            //////////////////////////////////////////////////////////////////////////////

            //Name und Versionsnummer in Form zeigen ////////////////////////////////////////////////////////////////////////////
            //a. If Assembly version is not explictly specified, it takes the value of 0.0.0.0.
            //b. If File version is not explictly specified, it takes the value of Assembly version.
            //c. If Product version is not explictly specified, it takes the value of File version.

            String strFullName;
            this.Test.GetInfoExecutingAssembly(out strFullName);
            String strName = System.Reflection.Assembly.LoadFile(strFullName).GetName().Name;
            String strFileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(strFullName).FileVersion;

            //String strName=System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;               
            //String strAssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //String strFileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
            //String strProductVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).ProductVersion;

            this.Text = String.Format("{0} - [FFT Ver.: {1}]", strName, strFileVersion);
            this.createMenuItemsFFTFixture();//Erstellt Menüeinträge für FFT-Fixture
            this.createMenuItemsSwCategory();//Erstellt Menüeinträge für DB-SW-Category
            //////////////////////////////////////////////////////////////////////////////

            if(this.Test.Data.FFTMode == 0)
                this.TextBoxFFT.BackColor = System.Drawing.SystemColors.Control;
            else
                this.TextBoxFFT.BackColor = System.Drawing.Color.Yellow;
        }

        /// <summary>
        /// Setzt die Mainframe Einstellungen vor dem FFT
        /// </summary>
        public void DoSettingBeforeFFT()
        {
            this.StateDruckenToolStripButton(false);
            this.toolStripComboBoxSelectVariant.Enabled = false;
            this.toolStripMenuItemChangeVariant.Enabled = false;
            this.ShowPictureFFTState(Honeywell.Test.CTest.StateFFT.Run);
            this.SetEnableButtonFFTStart(false);
            this.SetEnableButtonFFTCancel(true);
        }

        /// <summary>
        /// Setzt die Mainframe Einstellungen, die vom Testergebnis abhängig nach dem FFT zurück
        /// </summary>
        /// <returns>
        /// 0 - Keine Fehler
        /// 1 - Fehler
        /// -1 - Fehler mit dem Abbruch des Testes
        /// </returns>
        public int DoSettingAfterFFT()
        {
            int iResult;           

            this.SetEnableButtonFFTCancel(false);
            if (this.Test.ErrorFirst__ID_TestStep == 0)
            {                
                this.ShowPictureFFTState(Honeywell.Test.CTest.StateFFT.Good);
                iResult = 0;
            }
            else
            {
                this.SetBarTestTime(true);
                this.ShowPictureFFTState(Honeywell.Test.CTest.StateFFT.Error);

                CFormErrorList dlg;
                dlg = new CFormErrorList(this.Test);
                dlg.ShowDialog();

                if (this.Test.FlagDoCancelTest)//wird immer geprüft egal was für ein Flag gesetzt ist
                    iResult = -1;
                else
                    iResult = 1;
            }
            this.toolStripComboBoxSelectVariant.Enabled = true;
            this.toolStripMenuItemChangeVariant.Enabled = true;
            this.SetEnableButtonFFTStart(true);

            return iResult;
        }

        public void ShowPictureFFTStateNeutral()
        {
            this.ShowPictureFFTState(Honeywell.Test.CTest.StateFFT.Neutral);
        }        
        public void ShowPictureFFTState(Honeywell.Test.CTest.StateFFT State)
        {
            switch (State)
            {
                case Honeywell.Test.CTest.StateFFT.Neutral:
                    this.pictureBoxFFTState.Image = null;
                    this.Test.StateOfPictureResult = null;
            	    break;
                case Honeywell.Test.CTest.StateFFT.Run:
                    this.pictureBoxFFTState.Image = global::MainFrame.Properties.Resources.run;
                    this.Test.StateOfPictureResult = 1;
                    break;
                case Honeywell.Test.CTest.StateFFT.Error:
                    this.pictureBoxFFTState.Image = global::MainFrame.Properties.Resources.error;
                    this.Test.StateOfPictureResult = -1;
                    break;
                case Honeywell.Test.CTest.StateFFT.Good:
                    if (this.Test.StateOfPictureResult == 1)
                        this.pictureBoxFFTState.Image = global::MainFrame.Properties.Resources.good;
                    this.Test.StateOfPictureResult = 0;
                    break;

                default:
                    this.Test.StateOfPictureResult = null;
                    this.pictureBoxFFTState.Image = null;
                    break;
            }

        }

        private void buttonFFTCancel_Click(object sender, EventArgs e)
        {
            this.buttonFFTCancel.Enabled = false;
            this.Test.SetCancelTest();
            this.listViewFFT.Select();
        }

        private void FormMainFrame_Paint(object sender, PaintEventArgs e)
        {
            double dWidthEntire, dWidth;
            dWidthEntire = this.listViewFFT.Width;
            dWidth = dWidthEntire * 0.7;
            this.columnHeaderDesription.Width = (int)dWidth;
            dWidthEntire=this.listViewFFT.Width;
            this.columnHeaderValue.Width = (int)(dWidthEntire * 0.1);
            dWidthEntire = this.listViewFFT.Width - this.columnHeaderDesription.Width - this.columnHeaderValue.Width;
            this.columnHeaderLSL.Width = (int)(dWidthEntire*0.98 * 0.5);
            dWidthEntire = this.listViewFFT.Width - this.columnHeaderDesription.Width - this.columnHeaderValue.Width - this.columnHeaderLSL.Width;
            this.columnHeaderUSL.Width = this.columnHeaderLSL.Width;

            this.buttonFFTStart.Location = new System.Drawing.Point(this.tabControlFFT.Location.X, this.buttonFFTStart.Location.Y);
            this.buttonFFTStart.Width = this.splitContainerFFT.Panel1.Width + this.listViewFFT.Margin.Left + this.listViewFFT.Margin.Right;
            this.buttonFFTCancel.Width = this.splitContainerFFT.Panel2.Width + this.pictureBoxFFTState.Left + this.pictureBoxFFTState.Margin.Right;           
            this.buttonFFTCancel.Location = new System.Drawing.Point(this.tabControlFFT.Location.X + this.tabControlFFT.Width - this.buttonFFTCancel.Width, this.buttonFFTCancel.Location.Y);
        }


        public void DeleteScroll(int Nr)
        //Nr=0-Alle; Nr=1-Letzte; Nr=2-Letzte 2
        {
            if (Nr == 0)
            {
                this.listViewFFT.Items.Clear();
                return;
            }
            for (int i = 0; i < Nr; i++)
                this.listViewFFT.Items[this.listViewFFT.Items.Count - 1].Remove();
        }

        public String[] GetRowFromListBox(int Number)
        //Number=0 wird letzter Item geholt; 1-Erster Item
        {
            System.Windows.Forms.ListViewItem hItem;
            String[] strItemARRAY=null;
            int iNumber;
            if (Number <= 0)
                iNumber = this.listViewFFT.Items.Count - 1;
            else
                iNumber = Number - 1;

            if (iNumber<0)
                return strItemARRAY;

            hItem = this.listViewFFT.Items[iNumber];
            strItemARRAY = new String[hItem.SubItems.Count];
            for (int i = 0; i < hItem.SubItems.Count; i++)
            {
                strItemARRAY[i] = hItem.SubItems[i].Text;
            }            
            
            return strItemARRAY;
        }

        public int GetCountItemListViewFFT()
        {
            return this.listViewFFT.Items.Count;
        }

        public void AddRowToListViewFFT(params String[] RowARRAY)
        //String Description, String Value, String LSL, String USL
        {
            System.Windows.Forms.ListViewItem hItem;
            String[] strItemARRAY = new String[4];
            String strBuffer;
            int i;

            for (i = 0; i < strItemARRAY.Length; i++)
            {
                if (i < RowARRAY.Length)
                    strBuffer = RowARRAY[i];
                else
                    strBuffer = "";
                strItemARRAY[i] = strBuffer;
            }


            hItem = new System.Windows.Forms.ListViewItem(strItemARRAY);
            i = this.listViewFFT.Items.Count + 1;
            hItem.Name = i.ToString();
            if (this.BackColorOfListBoxItem != System.Drawing.Color.Empty)
            {
                hItem.BackColor = this.BackColorOfListBoxItem;
            }            
            this.listViewFFT.Items.Add(hItem);

            
            
            if (this.listViewFFT.Items.Count > 1)
                this.listViewFFT.Items[this.listViewFFT.Items.Count - 2].Selected=false;
            this.listViewFFT.Items[this.listViewFFT.Items.Count - 1].Selected = true;
            

            this.listViewFFT.Items[this.listViewFFT.Items.Count - 1].Focused = true;

            this.listViewFFT.EnsureVisible(this.listViewFFT.Items.Count - 1);
			this.listViewFFT.Update();

        }


        public void SetEnableButtonFFTStart(bool State)
        {
            this.buttonFFTStart.Enabled = State;            
            
            this.listViewFFT.Select();
        }

        public void SendEnterToButtonFFTStart()
        {
            //this.buttonFFTStart_Click(this, new EventArgs());
            
            this.buttonFFTStart.Focus();
            SendKeys.Send("{ENTER}");                        
        }

        public void SetEnableButtonFFTCancel(bool State)
        {
            this.buttonFFTCancel.Enabled = State;
            
            this.listViewFFT.Select();

        }

        private void FormMainFrame_KeyDown(object sender, KeyEventArgs e)
        {
            
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (this.Test.FlagStateTest != 0)//FFT läuft  
                    {
                        this.Test.SetCancelTest();
                    }
            	    break;
                case Keys.F10:
                    KeyHitLast_F10_F5 = Keys.F10;
                    this.Test.FlagBreakPointStop = true;
            	    break;
                case Keys.F5:
                    KeyHitLast_F10_F5 = Keys.F5;
                    break;
                case Keys.Enter:
                    if (this.Test.FlagStateTest == 0)//FFT läuft nicht  
                    {
                        this.Test.Execute();
                    }
                    break;
            }
        }


        public void DeleteDisplay(int Row)
        //Row=0-Alle; Row=1-1 Zeile; Row max. 10
        {
            if (Row == 0)
            {
                for (int i = 1; i < 11; i++)
                    this.WriteToDisplay("", i);
            }
            else
            {
                this.WriteToDisplay("", Row);
            }

        }

        public void DeleteStatus()
        {
            this.WriteToStatus("");
        }

        public void WriteToStatus(String Msg)
        {
            this.toolStripStatusLabelStatus.Text = Msg;			
        }

        public void WriteToDisplay(String Msg, int Row)
        //Row max. 10
        {
            String strMsg="";
            if(Row<this.DisplayARRAY.Length+1)
            {
                this.DisplayARRAY[Row-1]=Msg;
                for (int i=0;i<this.DisplayARRAY.Length;i++)
                {
                    strMsg = strMsg + this.DisplayARRAY[i] + "\r\n";
                }
                strMsg = strMsg.Substring(0, strMsg.Length - 2);
                this.TextBoxFFT.Text = strMsg;
				this.TextBoxFFT.Update();
            }           
        }

        public void SetTestTimeMax(double TestTimeMax)
        {
            this.toolStripProgressBarTestTime.Minimum = 0;
            this.toolStripProgressBarTestTime.Maximum = (int)TestTimeMax;
            this.toolStripProgressBarTestTime.Value = 0;
            TimeSpan timeSpan;
            
            timeSpan = new TimeSpan(0, 0, (int)TestTimeMax);
            this.toolStripStatusLabelTestTime.Text = timeSpan.ToString();
            //TimeSpan timeSpan;
            //timeSpan = new TimeSpan(0, 0, (int)173);
            //strBuffer = timeSpan.ToString(@"mm\:ss");

        }
        public void SetCurrentTestTime(double TestTime)
        {
            if (this.toolStripProgressBarTestTime.Maximum != 0)
            {
                if (TestTime >= this.toolStripProgressBarTestTime.Minimum && TestTime <= this.toolStripProgressBarTestTime.Maximum)
                {
                    this.toolStripProgressBarTestTime.Value = (int)TestTime;
                    TimeSpan timeSpan;

                    timeSpan = new TimeSpan(0, 0, this.toolStripProgressBarTestTime.Maximum - (int)TestTime);
                    this.toolStripStatusLabelTestTime.Text = timeSpan.ToString();
                    this.Update();
                }
            }
        }
        public void SetTestTime(string TestTime)
        {
            this.toolStripStatusLabelTestTime.Text = TestTime;
        }
        public void SetBarTestTime(bool Result)
        {
            if (Result)
                this.toolStripProgressBarTestTime.ForeColor = SystemColors.Highlight;
            else
                this.toolStripProgressBarTestTime.ForeColor = Color.Red;

            this.Update();
        }

        protected String[] DisplayARRAY;

        public System.Drawing.Color BackColorOfListBoxItem = System.Drawing.Color.Empty;

        private void toolStripComboBoxSelectVariant_DropDownClosed(object sender, EventArgs e)
        {
            int iIndex;
            String strVariant, strVariantOld, strMsg;

            if (this.Test.Data.Scanner == 0)
            {
                strVariantOld = this.Test.Data.ID_MenuLIST[this.Test.CurrentID_Menu - 1];
                strVariant = this.toolStripComboBoxSelectVariant.Items[this.toolStripComboBoxSelectVariant.SelectedIndex].ToString();
                if (strVariant != strVariantOld)
                {
                    //6		= "Wollen Sie von der Variante {0} zu der Variante{1} wechseln"
                    strMsg = String.Format(this.Test.Data.Msg[6], strVariantOld, strVariant);
                    if (MessageBox.Show(strMsg, "Variantenwechseln", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        iIndex = this.toolStripComboBoxSelectVariant.Items.IndexOf(strVariant);

                        if (!this.Test.ChangeVariant(strVariant))
                        {
                            this.Close();
                            return;
                        }

                        /*
                        this.Test.Deinstall();
                        this.Test.CurrentID_Menu = this.Test.Data.ID_MenuLIST.IndexOf(strVariant)+1;
                        //Alle Alias zu current Varian eintragen ////////////////////////////////////////////////////             
                        this.Test.CurrentID_MenuWithAllAliasARRAY[0] = this.Test.CurrentID_Menu.ToString();							//Position in der Menü von 1 angefangen
                        this.Test.CurrentID_MenuWithAllAliasARRAY[1] = this.Test.Data.ID_MenuLIST[this.Test.CurrentID_Menu - 1];				//OS-Number
                        this.Test.CurrentID_MenuWithAllAliasARRAY[2] = this.Test.Data.LinkID_MenuToVariantLIST[this.Test.CurrentID_Menu - 1];	//Variantenname
                        /////////////////////////////////////////////////////////////////////////////
                        if (!this.Test.Initialize())
                        {
                            this.Test.ShowErrorInitialize();
                            this.Close();
                            return;
                        }
                        this.ShowPictureFFTState(Honeywell.Test.CTest.StateFFT.Neutral);
                        this.Test.SetTestTimeMax(this.Test.TestTimeMax);
                        */

                    }
                    else
                    {
                        iIndex = this.toolStripComboBoxSelectVariant.Items.IndexOf(strVariantOld);
                    }
                    this.toolStripComboBoxSelectVariant.SelectedIndex = iIndex;
                    this.Test.Reset();
                    this.buttonFFTStart.Select();
                }
            }
            else
            {
                strVariantOld = this.Test.Data.ID_MenuLIST[this.Test.CurrentID_Menu - 1];
                iIndex = this.toolStripComboBoxSelectVariant.Items.IndexOf(strVariantOld);
                this.toolStripComboBoxSelectVariant.SelectedIndex = iIndex;
            }

        }

        private void FormMainFrame_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.EditorHasNothingToSave();
			this.Test.Deinstall();
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void öffnenToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.EditorHasNothingToSave())
                return;

            System.Windows.Forms.OpenFileDialog dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            dlgOpenFile.InitialDirectory = this.Test.Data.Path;
            dlgOpenFile.Filter = "Alle-Dateien (*.*)|*.*|"
                                + "Ini-Dateien (*.ini)|*.ini|"
                                + "Sys-Dateien (*.sys)|*.sys|"
                                + "Log-Dateien (*.log)|*.log|"
                                + "Report-Dateien (*.rpt)|*.rpt|"
                                + "Text-Dateien (*.txt)|*.txt";
            dlgOpenFile.FilterIndex = 1;
            dlgOpenFile.Multiselect = false;

            if(dlgOpenFile.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                this.EditorOpen(dlgOpenFile.FileName);
            }

            this.tabControlFFT.SelectedTab=this.tabPageEditor;
            
           
            //this.tabPageEditor.Select();
            //this.tabPageEditor.Focus();


        }

        private void EditorSetTitleTab()
        {
            String strTitle;
            //this.tabPageEditor
            if (this.editorFileFullname == null)
                strTitle = "Unbenannt";
            else
            {
                strTitle = this.editorFileFullname.Substring(this.editorFileFullname.LastIndexOf('\\') + 1);
            }
            this.tabPageEditor.Text = strTitle;
        }
        private bool EditorHasNothingToSave()
        {
            if (!this.editorFlagToSave)
                return (true);
            switch(MessageBox.Show("Änderungen speichern?","Texteditor",MessageBoxButtons.YesNoCancel))
            {
                case System.Windows.Forms.DialogResult.Yes:
                    return (this.EditorSave(this.editorFileFullname));
                case System.Windows.Forms.DialogResult.No:
                    return (true);
                case System.Windows.Forms.DialogResult.Cancel:
                    return (false);
            }

            return (false);
        }

        private bool EditorSave(String FullName)
        {
            System.Windows.Forms.SaveFileDialog dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            dlgSaveFile.InitialDirectory = this.Test.Data.Path;
            dlgSaveFile.Filter = "Alle-Dateien (*.*)|*.*|"
                                + "Ini-Dateien (*.ini)|*.ini|"
                                + "Sys-Dateien (*.sys)|*.sys|"
                                + "Log-Dateien (*.log)|*.log|"
                                + "Report-Dateien (*.rpt)|*.rpt|"
                                + "Text-Dateien (*.txt)|*.txt";
            dlgSaveFile.FilterIndex = 1;

            if (FullName == null)
            {
                if(dlgSaveFile.ShowDialog()==System.Windows.Forms.DialogResult.OK)
                {
                    this.editorFileFullname = dlgSaveFile.FileName;
                }
                else
                {
                    return (false);
                }
            }
            try
            {
                String strBuffer;
                /*
                System.IO.FileStream hFileStream = new System.IO.FileStream(FullName, System.IO.FileMode.Create);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(hFileStream);
                 * */
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(FullName, false, Encoding.GetEncoding("windows-1252"));
                System.IO.StreamWriter sw = new System.IO.StreamWriter(FullName, false, System.Text.Encoding.UTF8);
                
                strBuffer = this.richTextBoxEditor.Text;
                strBuffer=strBuffer.Replace("\n", "\r\n");
                sw.Write(strBuffer);
                sw.Close();

                this.editorFileFullname = FullName;
                this.editorFlagToSave = false;
                this.speichernToolStripButton.Enabled = false;
                this.speichernToolStripMenuItem.Enabled = false;
                this.speichernunterToolStripMenuItem.Enabled = false;
                this.EditorSetTitleTab();

                int iCurrentID_MenuTemp;
                iCurrentID_MenuTemp = this.Test.CurrentID_Menu;
                this.Test.Deinstall();
                this.Test.CurrentID_Menu = iCurrentID_MenuTemp;
                if (!this.Test.Initialize())
                {
                    this.Test.ShowErrorInitialize();
                    this.Close();
                    return(false);
                }
                this.ShowPictureFFTState(Honeywell.Test.CTest.StateFFT.Neutral);
                this.Test.SetTestTimeMax(this.Test.TestTimeMax);

                return (true);
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("Datei konnte nicht gespeichert werden\n" + ex.Message, "Error");                    
            }
            

            return (false);            
        }

        private void EditorOpen(String FullName)
        {
            try
            {
                /*
                System.IO.FileStream hFileStream = new System.IO.FileStream(FullName, System.IO.FileMode.Open);
                System.IO.StreamReader sr = new System.IO.StreamReader(hFileStream);
                 * */
                //System.IO.StreamReader sr = new System.IO.StreamReader(FullName,Encoding.GetEncoding("windows-1252"),false);
                System.IO.StreamReader sr = new System.IO.StreamReader(FullName, System.Text.Encoding.UTF8, false);
                this.richTextBoxEditor.Text = sr.ReadToEnd();
                sr.Close();

                this.editorFileFullname = FullName;
                this.editorFlagToSave = false;
                this.speichernToolStripButton.Enabled = false;
                this.speichernToolStripMenuItem.Enabled = false;
                this.speichernunterToolStripMenuItem.Enabled = false;

                this.toolStripButtonFileClose.Visible = true;
                this.EditorSetTitleTab();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Datei konnte nicht geöffnet werden\n" + ex.Message, "Error");
            }
        }

        private bool editorFlagToSave;
        private String editorFileFullname;

        private void richTextBoxEditor_TextChanged(object sender, EventArgs e)
        {
            if(!this.editorFlagToSave)
            {
                this.editorFlagToSave = true;
                this.speichernToolStripButton.Enabled = true;
                this.speichernToolStripMenuItem.Enabled = true;
                this.speichernunterToolStripMenuItem.Enabled = true;
            }
        }

        private void speichernToolStripButton_Click(object sender, EventArgs e)
        {
            this.EditorSave(this.editorFileFullname);
        }

        private void speichernunterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.EditorSave(null);
        }

        private void neuToolStripButton_Click(object sender, EventArgs e)
        {
            if(this.EditorHasNothingToSave())
            {
                this.richTextBoxEditor.Text = "";

                this.editorFileFullname = null;

                this.editorFlagToSave = false;
                this.speichernToolStripButton.Enabled = false;
                this.speichernToolStripMenuItem.Enabled = false;
                this.speichernunterToolStripMenuItem.Enabled = false;

                this.tabControlFFT.SelectedTab = this.tabPageEditor;
                this.toolStripButtonFileClose.Visible = true;

                this.EditorSetTitleTab();
            }
        }

        private void toolStripButtonFileClose_Click(object sender, EventArgs e)
        {
            if (this.EditorHasNothingToSave())
            {
                this.richTextBoxEditor.Text = "";

                this.editorFileFullname = null;

                this.editorFlagToSave = false;
                this.speichernToolStripButton.Enabled = false;
                this.speichernToolStripMenuItem.Enabled = false;
                this.speichernunterToolStripMenuItem.Enabled = false;

                this.tabControlFFT.SelectedTab = this.tabPageFFT;
                this.toolStripButtonFileClose.Visible = false;

                this.tabPageEditor.Text = "Editor";
                //this.EditorSetTitleTab();
            }
        }

        private void FormMainFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
        /// <summary>
        /// Funktion wird aufgerufen, wenn Items aus Test -> FFT Mode-> FFT-Fixture geklickt werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleFFTFixtureItemsClick(object sender, EventArgs e)
        {
            if (this.fFTFixtureToolStripMenuItem.Enabled == true)
            {
                int iMenuId;
                string[] strNameArray;
                strNameArray = ((ToolStripMenuItem)sender).Name.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                iMenuId = Convert.ToInt32(strNameArray[1], 10);
                System.Reflection.ParameterInfo[] parameters;
                parameters = this.Test.Device.FFTester.MethodsForMenu[iMenuId].GetParameters();
                if (parameters.Length == 1)
                {
                    strNameArray = parameters[0].Name.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    if (strNameArray.Length > 2)
                    {
                        FormInvokeFunction form;
                        form = new FormInvokeFunction();
                        string strParameters = "";
                        foreach (System.Reflection.ParameterInfo parameter in parameters)
                        {
                            if (strParameters.Length != 0)
                                strParameters += ", ";
                            strParameters += parameter.ParameterType.Name + " " + parameter.Name;
                        }
                        form.TextBoxFunctionName.Text = this.Test.Device.FFTester.MethodsForMenu[iMenuId].Name + string.Format("({0})", strParameters);
                        form.Button1.Text = strNameArray[0] + " " + strNameArray[1];
                        form.Button2.Text = strNameArray[0] + " " + strNameArray[2];
                        form.ShowDialog();

                        if (form.Mode > 0)
                        {
                            object[] parameterArray = new object[1];
                            if (form.Mode == 1)
                            {
                                parameterArray[0] = this.createParameter(strNameArray[1], parameters[0].ParameterType);
                            }
                            else
                            {
                                parameterArray[0] = this.createParameter(strNameArray[2], parameters[0].ParameterType);
                            }
                            if (parameterArray[0] != null)
                            {
                                object result;
                                result = this.Test.Device.FFTester.MethodsForMenu[iMenuId].Invoke(this.Test.Device.FFTester, parameterArray);
                                try
                                {
                                    if (!Convert.ToBoolean(result))
                                    {
                                        //13		= "!!! Achtung !!!"
                                        MessageBox.Show(this.Test.Device.FFTester.Error, this.Test.Data.Msg[13], MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                catch
                                {
                                }
                                return;
                            }
                        }
                        else
                            return;

                    }
                }
                if (parameters.Length == 0)
                {
                    object result;
                    object[] parameterArray = null;
                    result = this.Test.Device.FFTester.MethodsForMenu[iMenuId].Invoke(this.Test.Device.FFTester, parameterArray);
  

                    return;
                }

                //13		= "!!! Achtung !!!"
                //18		= "Keine Aktionen für diese Routine sind festgelegt"
                MessageBox.Show(this.Test.Data.Msg[18], this.Test.Data.Msg[13], MessageBoxButtons.OK);
            }
            return;
        }
        /// <summary>
        /// Convertiert übergebene String-Werte in object mit Berücksichtigung von type
        /// </summary>
        /// <param name="Value">
        /// Value als String
        /// </param>
        /// <param name="type">
        /// wirkliche Type von Value
        /// </param>
        /// <returns>
        /// null - Error
        /// Value als object
        /// </returns>
        private object createParameter(string Value, Type type)
        {
            object value = null;

            try
            {
                if (type == typeof(Int32))
                {
                    Int32 valueInt32;
                    valueInt32 = Convert.ToInt32(Value);
                    value = valueInt32;
                }
                else if (type == typeof(string))
                {
                    value = Value;
                }
            }
            catch
            {
                value = null;
            }     

            return value;
        }
        /// <summary>
        /// Erstellt Menüeinträge für FFT-Fixture in Test -> FFT Mode-> FFT-Fixture
        /// </summary>
        private void createMenuItemsFFTFixture()
        {
            if (this.Test != null && this.Test.Device != null && this.Test.Device.FFTester != null && this.Test.Device.FFTester.MethodsForMenu != null)
            {
                System.Windows.Forms.ToolStripMenuItem[] menuItems = null;
                int i;
                if (this.Test.Device.FFTester.MethodsForMenu.Length != 0)
                {
                    System.Reflection.ParameterInfo[] parameters;
                    menuItems = new ToolStripMenuItem[this.Test.Device.FFTester.MethodsForMenu.Length];
                    string strParameters;
                    for (i = 0; i < this.Test.Device.FFTester.MethodsForMenu.Length; i++)
                    {
                        menuItems[i] = new ToolStripMenuItem();
                        menuItems[i].Name = this.Test.Device.FFTester.MethodsForMenu[i].Name + "__" + i;
                        parameters = this.Test.Device.FFTester.MethodsForMenu[i].GetParameters();
                        strParameters = "";
                        foreach (System.Reflection.ParameterInfo parameter in parameters)
                        {
                            if(strParameters.Length != 0)
                                strParameters +=  ", ";
                            strParameters += parameter.Name;
                        }
                        menuItems[i].Text = this.Test.Device.FFTester.MethodsForMenu[i].Name + string.Format("({0})", strParameters);
                        menuItems[i].Click += new System.EventHandler(this.handleFFTFixtureItemsClick);
                        if(i < 9)
                            menuItems[i].ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1 + i)));
                        else if(i == 9)
                            menuItems[i].ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));                        
                    }
                    this.fFTFixtureToolStripMenuItem.Enabled = true;
                    this.fFTFixtureToolStripMenuItem.DropDownItems.AddRange(menuItems);
                }
                else
                    this.fFTFixtureToolStripMenuItem.Enabled = false;
            }
            else
                this.fFTFixtureToolStripMenuItem.Enabled = false;

            //this.fFTFixtureToolStripMenuItem.Enabled = true;
        }
        private void createMenuItemsSwCategory()
        {
            if (this.Test != null && this.Test.Data != null  && this.Test.Data.DB != null)
            {
                if (this.Test.TestSoftwareCategoryArray != null && this.Test.TestSoftwareCategoryArray.Length != 0)
                {
                    System.Windows.Forms.ToolStripMenuItem[] menuItems = null;
                    int i;
                    menuItems = new ToolStripMenuItem[this.Test.TestSoftwareCategoryArray.Length + 1];
                    menuItems[0] = new ToolStripMenuItem();
                    menuItems[0].Name = "0";
                    menuItems[0].Text = string.Format("Current SW-Category ({0})", this.Test.Data.DB.Tab_ProductionCategory.Name);
                    menuItems[0].Click += new System.EventHandler(this.sQLDataBaseToolStripMenuItem_Click);
                    for (i = 0; i < this.Test.TestSoftwareCategoryArray.Length; i++)
                    {
                        menuItems[i + 1] = new ToolStripMenuItem();

                        menuItems[i + 1].Name = this.Test.TestSoftwareCategoryArray[i].ProductionCategoryId.ToString();
                        menuItems[i + 1].Text = string.Format("{1}--{0}", this.Test.TestSoftwareCategoryArray[i].Description, this.Test.TestSoftwareCategoryArray[i].Name);
                        menuItems[i + 1].Click += new System.EventHandler(this.sQLDataBaseToolStripMenuItem_Click);
                    }
                    this.sQLDataBaseToolStripMenuItem.DropDownItems.AddRange(menuItems);
                }
                else
                    this.sQLDataBaseToolStripMenuItem.Enabled = false;
            }           
        }

        private void fFTModeToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if (this.Test != null && this.Test.Data != null)
            {
                if (this.Test.Data.FFTMode != 0)
                {
                    this.fFTFixtureToolStripMenuItem.Enabled = true;                    
                }
                else
                {
                    this.fFTFixtureToolStripMenuItem.Enabled = false;
                }
                this.setFFTModeToolStripMenuItem.Text = "Set FFT mode" + string.Format(" ({0})", this.Test.Data.FFTMode);                
            }
        }

        private void setFFTModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Test.Data.FFTMode == 0)
            {
                this.Test.Data.FFTMode = 1;
                this.TextBoxFFT.BackColor = System.Drawing.Color.Yellow;                
            }
            else
            {                
                this.Test.Data.FFTMode = 0;
                this.TextBoxFFT.BackColor = System.Drawing.SystemColors.Control;
            }
        }

        private void testToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if (this.Test != null && this.Test.Data != null)
            {
                if (this.Test.Data.OrderNameCurrent != null)
                {
                    this.productionOrderToolStripMenuItem.Enabled = false;
                    this.productionOrderToolStripMenuItem.Text = "Order: " + this.Test.Data.OrderNameCurrent;
                }
                else
                {
                    this.productionOrderToolStripMenuItem.Enabled = false;
                    this.productionOrderToolStripMenuItem.Text = "New Production Order";
                }
            }

        }

    

        private void extrasToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if (this.Test != null)
            {
                if (this.Test.Data != null && this.Test.Data.DB != null)
                {
                    if (this.Test.TestSoftwareCategoryArray != null)
                    {
                        this.sQLDataBaseToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        this.sQLDataBaseToolStripMenuItem.Enabled = false;
                    }
                }
                else
                {
                    this.sQLDataBaseToolStripMenuItem.Enabled = false;
                }
            }            
        }

        private void sQLDataBaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Test.Data != null && this.Test.Data.DB != null)
            {
                const string nameSWEvaluation = "DataEvaluation.exe";
                string fullNameSWTest, fullNameSWEvaluation, strError, fullNameSWTestAssembly;
                int iFlagKindOfSWTest;//0-Exe; 1-Debug; 2-Release
                
                this.Test.GetInfoExecutingAssembly(out fullNameSWTestAssembly);
                fullNameSWTest = System.IO.Path.Combine(this.Test.Path, System.IO.Path.GetFileNameWithoutExtension(fullNameSWTestAssembly) + ".exe");
                if (fullNameSWTestAssembly.ToUpper() != fullNameSWTest.ToUpper())
                {
                    if(fullNameSWTestAssembly.ToUpper().IndexOf("\\BIN\\DEBUG\\") >= 0)
                    {
                        iFlagKindOfSWTest = 1;
                    }
                    else
                    {
                        iFlagKindOfSWTest = 2;
                    }
                }
                else
                {
                    iFlagKindOfSWTest = 0;
                }

                string strName, strNameMenu;
                string[] strNameArray;
                strNameMenu = ((ToolStripMenuItem)sender).Name;
                strName = ((ToolStripMenuItem)sender).Text;
                strNameArray = strName.Split(new string[] { "--" }, StringSplitOptions.RemoveEmptyEntries);
                if (iFlagKindOfSWTest == 0)
                {
                    fullNameSWEvaluation = System.IO.Path.Combine(this.Test.Path, nameSWEvaluation);
                }
                else
                {
                    fullNameSWEvaluation = System.IO.Path.Combine(this.Test.Data.Path, nameSWEvaluation);
                }
                if (System.IO.File.Exists(fullNameSWEvaluation) && System.IO.File.Exists(fullNameSWEvaluation + ".config"))
                {
                    if (iFlagKindOfSWTest != 0)//SW is started from Project 
                    {
                        string strBuffer;
                        strBuffer = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fullNameSWTestAssembly), nameSWEvaluation);
                        System.IO.File.Copy(fullNameSWEvaluation, strBuffer, true);
                        System.IO.File.Copy(fullNameSWEvaluation + ".config", strBuffer + ".config", true);
                        fullNameSWEvaluation = strBuffer;                        
                    }
                    if (strNameMenu == "0")
                    {                                                
                        fullNameSWTest = System.IO.Path.Combine(this.Test.Path, System.IO.Path.GetFileNameWithoutExtension(fullNameSWTest) + ".exe");
                        System.Diagnostics.Process process = System.Diagnostics.Process.Start(fullNameSWEvaluation, fullNameSWTest);                     
                    }
                    else
                    {
                        if (strNameArray.Length > 0)
                        {
                            System.Diagnostics.Process process = System.Diagnostics.Process.Start(fullNameSWEvaluation, strNameArray[0]); 
                        }
                        else
                        {
                            System.Diagnostics.Process process = System.Diagnostics.Process.Start(fullNameSWEvaluation); 
                        }
                    }
                }
                else
                {
                    strError = string.Format("Evaluation SW\r'{0}' or '{1}'\rwas not found", fullNameSWEvaluation, fullNameSWEvaluation + ".config");
                    MessageBox.Show(strError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        private void sQLDataBaseToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private void toolStripComboBoxSelectVariant_Click(object sender, EventArgs e)
        {
            
            if (this.Test.Data.Scanner != 0)
            {
                                
                this.toolStripComboBoxSelectVariant.Enabled = false;

                this.changeVariant();

                this.toolStripComboBoxSelectVariant.Enabled = true;
            }
             
        }

        private void changeVariant()
        {
            int iCurrentID_MenuOld, iIndex;
            iCurrentID_MenuOld = this.Test.CurrentID_Menu;
                
            this.Test.SetVariant();
            if (this.Test.CurrentID_Menu != 0)
            {
                if (iCurrentID_MenuOld != this.Test.CurrentID_Menu)
                {
                    string strVariant = this.Test.Data.ID_MenuLIST[this.Test.CurrentID_Menu - 1];
                    if (!this.Test.ChangeVariant(strVariant))
                    {
                        this.Close();
                        return;
                    }
                    strVariant = this.Test.Data.ID_MenuLIST[this.Test.CurrentID_Menu - 1];
                    iIndex = this.toolStripComboBoxSelectVariant.Items.IndexOf(strVariant);
                    this.toolStripComboBoxSelectVariant.SelectedIndex = iIndex;
                    this.Test.Reset();
                    this.buttonFFTStart.Select();
                }
            }                
        }

        private void toolStripComboBoxSelectVariant_DropDown(object sender, EventArgs e)
        {
            if (this.Test.Data.Scanner != 0)
            {
                
            }
            
        }

     

        private void toolStripButtonChangeVariant_Click(object sender, EventArgs e)
        {
            this.changeVariant();
        }

        private void varianteWechselnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.changeVariant();
        }

        private void sQLDataBaseToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {            

        }

        private void druckenToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.Test != null)
            {
                this.Test.PrintFromFFT();
            }

        }
         
        public void StateDruckenToolStripButton(bool State)
        {
            this.druckenToolStripButton.Enabled = State;
            this.druckenToolStripMenuItem.Enabled = State;
        }

        private void druckenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Test != null)
            {
                this.Test.PrintFromFFT();
            }
        }
    }
}