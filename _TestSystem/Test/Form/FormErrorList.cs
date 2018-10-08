using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Honeywell.Test;

namespace Honeywell.Forms
{
	public partial class CFormErrorList : Form
	{
		public CFormErrorList(CTest Test)
		{
			this.test = Test;
			InitializeComponent();
		}

		protected CTest test;

		private void CFormErrorList_Load(object sender, EventArgs e)
		{
			string strBuffer;
			System.Windows.Forms.ListViewItem Item;
			String[] textARRAY;

			textARRAY = new String[2];

            this.test.FlagDoNotCheckCancel = true;
			if(this.test.PrinterError == 1)
			{
				foreach(string strLinePrint in this.test.ErrorReportPrintHeadLineLIST)
				{
					this.test.DoPrintError(strLinePrint);
				}                
			}
            
			foreach(CTestStep.CErrorTestStep errorTestStep in this.test.ErrorAttributeLIST)
			{
				textARRAY[0] = errorTestStep.ErrorCode;
                textARRAY[1] = errorTestStep.ErrorMsg.Replace("\r\n", "");		
				Item = new System.Windows.Forms.ListViewItem(textARRAY);
				this.listViewErrorList.Items.Add(Item);

				if(this.test.PrinterError == 1)
				{
					strBuffer = string.Format("[{0}] {1}", errorTestStep.ErrorCode, errorTestStep.ErrorMsgMeasurement);
					this.test.DoPrintError(strBuffer);
					strBuffer = errorTestStep.ErrorMsgPrint;
					this.test.DoPrintError(strBuffer);
				}				
			}            

			this.listViewErrorList.Items[0].Selected=true;

			if(this.test.PrinterError == 1)
			{
				foreach(string strLinePrint in this.test.ErrorReportPrintFootLineLIST)
				{
					this.test.DoPrintError(strLinePrint);
				}                

				this.test.DoFeedErrorPrinter(this.test.CountFeedErrorPrinter);
			}

            this.test.FlagDoNotCheckCancel = false;
            
			//this.listViewErrorList.SelectedIndex = this.test.Data.FFTMode;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
