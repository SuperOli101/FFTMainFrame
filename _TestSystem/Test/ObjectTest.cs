using System;
using System.Collections.Generic;
using Honeywell.Data;
using Honeywell.Test;
using Honeywell.Device;

using Honeywell.Forms;
namespace Honeywell
{
    namespace Test
    {
        /// <summary>
        /// von der Klasse werden Klassen wie CTest, CDevice und CData abgeleitet
        /// Diese Klasse erleichtert die Inintialisierung von den Objekten, die hier initialisiert werden.
        /// </summary>
        public class CObjectContainer : CObjectTest
        {
            public CObjectContainer():base()
            {
                this.Create();             
            }

			public CObjectContainer(string Name, int ModeReport)
				: base(Name, ModeReport)
			{
				this.Create();
			}

            /// <summary>
            ///Wird im Konstrukter aufgerufen
            /// </summary>
            public new bool Create()
            {
				//this.Timer = new CTimer();

                this.dlgInstalRessource = null;            
                return (true);
            }

            /// <summary>
            /// Zeigt die Initialisierung vom Gerät im FormDeviceInstal
            /// </summary>
            /// <param name="Name">
            /// Gerätenname
            /// </param>
            public void AddRessource(string Name)
            {
                string strMsg, strCode;

                this.dlgInstalRessource.Text = this.Name;
				this.NameDeviceToInstall = Name;

                strCode = string.Format("{0}{1:d3}: ", this.IDType, this.ID);
                if (this.Data.Report != null)
                {
                    strMsg = "";
                    strMsg = strMsg.PadRight(this.Data.LengthScroll, '-');
                    this.WriteLineToReport(strMsg);
                    strMsg = string.Format("--- {0} ---", Name);
                    strMsg = strMsg.PadRight(this.Data.LengthScroll, '-');
                    this.WriteLineToReport(strMsg);
                }
                if (this.Data.Msg == null)
                {
                    this.dlgInstalRessource.LabelMsg.Text = string.Format("{0} {1}", Name, "is being installed ...");
                }
                else
                {
                    //9		= "wird initialisiert ..."
                    this.dlgInstalRessource.LabelMsg.Text = string.Format("{0} {1}", Name, this.Data.Msg[9]);
                }
                if (this.dlgInstalRessource.ProgressBarDeviceInstal.Value == this.dlgInstalRessource.ProgressBarDeviceInstal.Maximum)
                    return;                
                this.dlgInstalRessource.ProgressBarDeviceInstal.Value++;
				this.dlgInstalRessource.Update();
            }
            /// <summary>
            /// Schreib zusätliche Instalationsmeldungen zu dem dlgInstalRessource
            /// </summary>
            /// <param name="Msg">
            /// Meldung zum Schreiben
            /// </param>
            public void WriteMsgToDlgRessourceInstall(string Msg)
            {
                string strMsg, strCode;

                this.dlgInstalRessource.Text = this.Name;

                strCode = string.Format("{0}{1:d3}: ", this.IDType, this.ID);
                if (this.Data.Report != null)
                {
                    strMsg = "";
                    strMsg = strMsg.PadRight(this.Data.LengthScroll, '-');
                    this.WriteLineToReport(strMsg);
                    strMsg = string.Format("--- {0} ---", Msg);
                    strMsg = strMsg.PadRight(this.Data.LengthScroll, '-');
                    this.WriteLineToReport(strMsg);
                }

                strMsg = Msg;
                object[] parameter = new object[1];
                parameter[0] = strMsg;

                SetTextHandle d = this.dlgInstalRessource.WriteToLabel;
                this.dlgInstalRessource.Invoke(d, parameter);
                //this.Test.FormFFT.Invoke(new System.Windows.Forms.MethodInvoker(this.Test.FormFFT.SendEnterToButtonFFTStart));
                this.dlgInstalRessource.LabelMsg.Text = string.Format("{0}\r\r{1}", this.NameDeviceToInstall, strMsg);
                
                this.dlgInstalRessource.Update();
            }
            delegate void SetTextHandle(string text);

            /// <summary>
            /// Startet den Dialog für Initialisierung von Geräten oder Daten
            /// </summary>
            public void StartInstalRessource()
            {
                if (this.dlgInstalRessource == null)
                {
                    this.dlgInstalRessource = new CFormInstalRessource();
                    this.dlgInstalRessource.ProgressBarDeviceInstal.Minimum = 0;
                    this.dlgInstalRessource.ProgressBarDeviceInstal.Maximum = this.CountResourceForInstalling;
                    this.dlgInstalRessource.ProgressBarDeviceInstal.Value = 0;

                    this.dlgInstalRessource.Show();
                }
            }

            /// <summary>
            /// Beendet den Dialog für Initialisierung von Geräten oder Daten
            /// </summary>
            public void StopInstalRessource()
            {
                if (this.dlgInstalRessource != null)
                {
                    this.dlgInstalRessource.Close();
                    this.dlgInstalRessource = null;
                }
            }

            public void HideInstalRessource()
            {
                if (this.dlgInstalRessource != null)
                {
                    //this.dlgInstalRessource.Hide();
                    this.dlgInstalRessource.Visible = false;
                }
            }

            public void ShowInstalRessource()
            {
                if (this.dlgInstalRessource != null)
                {
                    //this.dlgInstalRessource.Hide();
                    this.dlgInstalRessource.Visible = true;
                }
            }

            /// <summary>
            /// Anzahl von Geräten zum Installieren
            /// </summary>
            private int countResourceForInstalling = 0;
            /// <summary>
            /// Anzahl von Geräten zum Installieren
            /// </summary>
            public int CountResourceForInstalling
            {
                get
                {
                    return this.countResourceForInstalling;
                }
                set
                {
                    this.countResourceForInstalling = value;
                    if (this.dlgInstalRessource != null)
                    {
                        this.dlgInstalRessource.ProgressBarDeviceInstal.Maximum = this.countResourceForInstalling;
                    }
                }
            }

            /// <summary>
            /// Referenz auf den Dialog für Geräteninitialisierung
            /// </summary>
            private CFormInstalRessource dlgInstalRessource;

			/// <summary>
			/// Fügt die Error Message zusammen mit Namen des instalierenden Gerätes in die error Array ein.
			/// </summary>
			/// <param name="Error">
			/// Error Message
			/// </param>
			public new void SetError(String Error)
			{
				this.Error += this.NameDeviceToInstall;
				this.Error += Error;
				this.Error += "\r\n";
			}

            public new int GetError()
            {

                 return this.Error.Length;
            }

			/// <summary>
			/// Gerät, das gerade inizialisiert wird
			/// </summary>
			public string NameDeviceToInstall;
        }

		
        /// <summary>
        /// Alle FFT-Klassen sind von der Klasse abgeleitet
        /// </summary>
        public class CObjectTest
        {
			//-------------------------------------------------------------------------------------------
            public CObjectTest()
            {
                this.Create();                
            }
			public CObjectTest(string Name, int ModeReport)
			{
				this.Create();
				this.Name = Name;
				this.FlagReportOn = ModeReport;
			}

			/// <summary>
			/// Wird im Konstrukter aufgerufen
			/// </summary>
			public bool Create()
			{
				this.Name = "";
				this.FlagReportOn = 1;
				this.FlagCancelLock = false;
				this.ErrorLIST = new List<String>(20);
				this.ErrorCodeLIST = new List<int>(20);
				this.IDType = 0;
				this.ID = 0;

				if(this as CTimer == null)
					this.Timer = new CTimer();
				this.Error = "";

				this.RemoveError();


				return (true);
			}

			//-------------------------------------------------------------------------------------------
            public virtual bool Initialize()
            {
                return (true);
            }
			public virtual void Deinstall()
			{
                if (this.Data.DB != null)
                {
                    this.Data.DB.DbSql.Close();
                    this.Data.DB = null;
                }

				return;
			}               

			//-------------------------------------------------------------------------------------------
            public void RemoveError()
            {
                this.ErrorCode = 0;
                this.Error = "";

                this.ErrorLIST.Clear();
                this.ErrorCodeLIST.Clear();

                return;

            }
            /// <summary>
            /// Setzt die variable Test.FlagDoCancelTest auf true.
            /// Das heißt der Test wird abgebrochen.
            /// </summary>
            public void SetCancelTest()
            {
                if (this.Test.FlagCancelLock == false)//wird nicht registriert wenn this.Test.FlagCancelLock == true
                {
                    if (!this.Test.FlagDoCancelTest)
                    {
                        this.Test.FlagTestCanceledByOperator = true;
                        this.Test.FlagDoCancelTest = true;
                        ((this.Test).CurrentTestStep).SetResultFault(999);
                    }
                }                
            }
			public bool CancelTest()
			{
				if(this.Test.FlagDoNotCheckCancel == false)//wird nicht geprüft wenn this.Test.FlagDoNotCheckCancel == true
				{
					if(this.Test.FlagDoCancelTest)
					{
						if(this.FlagDoCancelTestRecorded==false)
						{
							this.FlagDoCancelTestRecorded = true;
							this.Error += "Test is canceled by operator";
							this.WriteLineToReport(this.Error);
						}						
						return (true);
					}
				}
				return (false);
			}
			
			/// <summary>
			/// Kann man mit ESC unterbrechen
			/// </summary>
			/// <param name="Delay">
			/// Delay Dauer
			/// </param>
			/// <returns>
			/// true - Sleep wurde unterbrochen
			/// false - Sleep wurde vollständig ausgeführt
			/// </returns>
			public bool Sleep(long Delay)
			{
				bool bResult;
				string strNameTemp;
				int iIDTemp, iIDTypeTemp, iReportTemp;

				strNameTemp = this.Timer.Name;
				this.Timer.Name = this.Name;
				iIDTemp = this.Timer.ID;
				this.Timer.ID = this.ID;
				iIDTypeTemp = this.Timer.IDType;
				this.Timer.IDType = this.IDType;
                iReportTemp = this.Timer.FlagReportOn;
                this.Timer.FlagReportOn = this.FlagReportOn;

				bResult = this.Timer.SleepAsDelay(Delay);

				this.Timer.Name = strNameTemp;
				this.Timer.ID = iIDTemp;
				this.Timer.IDType = iIDTypeTemp;
                this.Timer.FlagReportOn = iReportTemp;

				return bResult;
			}
			
			/// <summary>
			/// Kann man nicht mit ESC unterbrechen
			/// </summary>
			/// <param name="Delay">
			/// Delay Dauer
			/// </param>
			public void Delay(long Delay, int Mode=0)
			{
				CObjectTest.DelayTotal_Sec += Delay;
				System.Windows.Forms.Application.DoEvents();
				System.Threading.Thread.Sleep((int)Delay);
				if(Mode==0)
					this.WriteLineToReport(string.Format("::Delay from {0} = {1} mSec", this.Name, Delay));
				return;
			}
			
			/*
            public void CreateError(String Error, int _ErrorCode)
            {
                this.Error = Error;
                this.ErrorLIST.Add(this.Error);
                if (_ErrorCode != 0)
                {
                    this.ErrorCodeString = _ErrorCode;
                    this.ErrorCodeLIST.Add(this.ErrorCodeString);
                }

                return;
            }
			 * */

			//-------------------------------------------------------------------------------------------
            /// <summary>
            ///Überlegen zu löschen
            /// </summary>
            /// <param name="Error">
            /// Errortext
            /// </param>
            public void SetError(String Error)
            {                
				this.Error = Error;

                return;
            }

            /// <summary>
            ///Gibt Anzahl von den error (this.ErrorLIST.Count)
            /// </summary>
            public int GetError()
            {
                if (this.ErrorLIST == null)
                    return (0);
                else
                    return (this.ErrorLIST.Count);
            }

            /// <summary>
            ///Gibt Errorliste mit der Komma getrennt
            /// </summary>
            public String GetErrorList()
            {
                String strErrorList, strBuffer;
                strErrorList = "";
                foreach (String strError in this.ErrorLIST)
                {
                    strBuffer = String.Format("{0},", strError);
                    strErrorList += strBuffer;
                }
                if (strErrorList.Length != 0)
                {
                    strBuffer = strErrorList.Substring(0, strErrorList.Length - 1);
                    if (this.Test.FlagDoCancelTest == true)
                        strErrorList = "0," + strBuffer;
                    else
                        strErrorList = strBuffer;

                    return (strBuffer);
                }
                if (this.Test.FlagDoCancelTest == true)
                    return "0";
                else
                    return "";
            }

			//-------------------------------------------------------------------------------------------
			/// <summary>
			/// Schreibt Msg in den Buffer "Buffer" von CDataReport kein \r\n nur angehängt, nur wenn FlagReportOn=1				
			/// </summary>
			/// <param name="Msg">
			/// Text, welches geschrieben soll
			/// </param>
			public void WriteToReport(String Msg)
			{
				this.WriteToReport(Msg, 0);

				return;
			}
			/// <summary>
			///Schreibt Msg in den Buffer "Buffer" von CDataReport kein \r\n nur angehängt, nur wenn FlagReportOn=1            
			/// </summary>
			/// <param name="Msg">
			/// Text, welches geschrieben soll
			/// </param>
			/// <param name="Mode">
			/// Mode-0 einfach anhängen 
			/// Mode-1 . wird durch , ersetzt und dann einfach anhängen
			/// </param>
			public void WriteToReport(String Msg, int Mode)
			{
				string strMsg;
				string[] strMsgArray;


				if(this.FlagReportOn == 1)
				{
					if(this.Data != null)
					{
						if(this.Data.Report != null)
						{
                            strMsg = Msg;
                            ///////////////////////////////////////////////////////////////
                            #region//Ersetzt einzelne "\r" oder "\n" in strMsg durch "\r\n"
                            if (strMsg.IndexOf("\r\n") != -1)
                            {
                                strMsg = strMsg.Replace("\r\n", "__TEMP__CR____NL__TEMP__");
                            }
                            if (strMsg.IndexOf("\n") != -1)
                            {
                                strMsg = strMsg.Replace("\n", "\r");
                            }
                            if (strMsg.IndexOf("\r") != -1)
                            {
                                strMsg = strMsg.Replace("\r", "__TEMP__CR____NL__TEMP__");
                            }
                            if (strMsg.IndexOf("__TEMP__CR____NL__TEMP__") != -1)
                            {
                                strMsg = strMsg.Replace("__TEMP__CR____NL__TEMP__", "\r\n");
                            }
                            #endregion
                            ///////////////////////////////////////////////////////////////
                            strMsgArray = System.Text.RegularExpressions.Regex.Split(strMsg, "\r\n");

							foreach(string strMsgTemp in strMsgArray)
							{
								//strMsg = Msg;
								strMsg = strMsgTemp.PadLeft(strMsgTemp.Length + this.ShiftReport);
								strMsg = string.Format("{0}{1:d3}|:| {2}", this.IDType, this.ID, strMsg);
								this.Data.Report.Write(strMsg, Mode);
							}
						}
					}
				}
				return;
			}
			/// <summary>
			/// Schreibt Msg in den Buffer "Buffer" und hängt \r\n an, nur wenn FlagReportOn=1
			/// </summary>
			/// <param name="Msg">
			/// Text, welches geschrieben soll
			/// </param>
			public void WriteLineToReport(String Msg)
			{
				this.WriteLineToReport(Msg, 0);

				return;
			}
			/// <summary>
			/// Schreibt Msg in den Buffer "Buffer" und hängt \r\n an, nur wenn FlagReportOn=1           
			/// </summary>
			/// <param name="Msg">
			/// Text, welches geschrieben soll
			/// </param>
			/// <param name="Mode">
			/// Mode-0 wie void WriteLine(String Msg);
			/// Mode-1 . wird durch , ersetzt	und dann wie void WriteLine(String Msg);
			/// </param>
			public void WriteLineToReport(String Msg, int Mode)
			{
				string strMsg;
				string[] strMsgArray;

				if(this.FlagReportOn == 1)
				{
					if(this.Data != null)
					{
						if(this.Data.Report != null)
						{
                            strMsg = Msg;
                            ///////////////////////////////////////////////////////////////
                            #region//Ersetzt einzelne "\r" oder "\n" in strMsg durch "\r\n"
                            if (strMsg.IndexOf("\r\n") != -1)
                            {
                                strMsg = strMsg.Replace("\r\n", "__TEMP__CR____NL__TEMP__");
                            }
                            if (strMsg.IndexOf("\n") != -1)
                            {
                                strMsg = strMsg.Replace("\n", "\r");
                            }
                            if (strMsg.IndexOf("\r") != -1)
                            {
                                strMsg = strMsg.Replace("\r", "__TEMP__CR____NL__TEMP__");
                            }
                            if (strMsg.IndexOf("__TEMP__CR____NL__TEMP__") != -1)
                            {
                                strMsg = strMsg.Replace("__TEMP__CR____NL__TEMP__", "\r\n");
                            }
                            #endregion
                            ///////////////////////////////////////////////////////////////
                            strMsgArray = System.Text.RegularExpressions.Regex.Split(strMsg, "\r\n");

							foreach(string strMsgTemp in strMsgArray)
							{
								//strMsg = Msg;
								strMsg = strMsgTemp.PadLeft(strMsgTemp.Length + this.ShiftReport);
								strMsg = string.Format("{0}{1:d3}|:| {2}", this.IDType, this.ID, strMsg);
								this.Data.Report.WriteLine(strMsg, Mode);
							}
						}
					}
				}

				return;
			}

			/// <summary>
			/// Startet ein expert Dialog 
			/// </summary>
			public virtual void ShowDialogExpert()
			{
				return;
			}
			/// <summary>
			/// Schließet ein expert Dialog 
			/// </summary>
			public virtual void CloseDialogExpert()
			{
				return;
			}

			/*
            /// <summary>
            ///Überlegen zu löschen
            /// </summary>
            public void AddError(String Error, int _ErrorCode)
            {
                this.Error += "\r\n";
                this.Error += Error;

                if (_ErrorCode != 0)
                    this.ErrorCodeString = _ErrorCode;

                return;
            }

            /// <summary>
            ///Überlegen zu löschen
            /// </summary>
            public void AddError(String Error)
            {
                this.AddError(Error, 0);
                return;
            }
			 * */
			//-------------------------------------------------------------------------------------------            
			public CTimer Timer = null;


            public static CData data=null;
            public CData Data
            {
                get
                {
                    return (CObjectTest.data);
                }
                set
                {
                    CObjectTest.data = value;                    
                }
            }

            public static CDevice device = null;
            public CDevice Device
            {
                get
                {
                    return (CObjectTest.device);
                }
                set
                {
                    CObjectTest.device = value;
                }
            }

            public static CTest test=null;
            public CTest Test
            {
                get
                {
                    return (CObjectTest.test);
                }
                set
                {
                    CObjectTest.test = value;                    
                }
            }

            public String Name;

            public String Error
            {
                get
                {                    
                    if (this.ErrorLIST.Count == 0)
                        return ("");
                    return (String.Join("\r\n",this.ErrorLIST.ToArray()));
                }
                set
                {					
                    if (value.Length == 0)
                    {
                        //if (this.ErrorLIST != null)
						this.FlagDoCancelTestRecorded = false;
						this.ErrorCode = 0;
                        this.ErrorLIST.Clear();
                    }
                    else
                    {
						if(this.ErrorLIST.Count == 0)
						{
							this.ErrorLIST.Add(value);
						}
						else
						{
							//um unterscheiden zu können, ob der Wert mit ++ oder mit + zugewiesen wude /////////////
							if(value.IndexOf(this.Error) == 0)//mit ++; In value ist leider alle alten Werte und der neue Wert (Neuer Wert ausfiltern)
							{								
								this.ErrorLIST.Add(value.Substring(this.Error.Length, value.Length - this.Error.Length));
							}
							else//mit +; alle vorherige Beiträge löschen und dann zuweisen
							{								
								this.ErrorLIST.Clear();
								this.ErrorLIST.Add(value);
								this.FlagDoCancelTestRecorded = false;
								this.ErrorCode = 0;
							}
						}  						
                    }
                }
            }
			
			/// <summary>
			/// Get - Anzahl von Errorcodes
			/// Set - Error code zu dem this.ErrorCodeLIST; 0 - Liste gelöschst; = Errorcode hingefügt zur Liste; += funktioniert nicht 
			/// Muss immer nach this.Error zugewiesen werden
			/// </summary>
            public int ErrorCode
            {
			
                get
                {
					/*
                    if (this.ErrorCodeLIST.Count == 0)
                        return (0);

                    return (this.ErrorCodeLIST[0]);
					 */
					return (this.ErrorCodeLIST.Count);
                }
			
                set
                {
					if(value== 0)
                        this.ErrorCodeLIST.Clear();
                    else
						this.ErrorCodeLIST.Add(value);                    
                }
            }
			public string ErrorCodeString
			{

				get
				{					
					return (String.Join(",", this.ErrorCodeLIST.ToArray()));
				}
			}
            public List<int> ErrorCodeLIST;
            public List<String> ErrorLIST;

            /// <summary>
            ///1-erlaubt schreiben in den Buffer m_strBuffer Default Setting 1
            /// </summary>
            public int FlagReportOn;
			/// <summary>
			/// FlagDoCancelTest ist von Gerät schon notiert und wird nicht zweites mal von CancelTest gestezt
			/// </summary>
			public bool FlagDoCancelTestRecorded=false;



           /// <summary>
           /// Verschiebt Die Zeile um ReportShift mal Zeichen rechts.
           /// </summary>
           public int ShiftReport=0;            

            /// <summary>
            /// true-wird nicht regestriert und nicht getestet ob der Test abgebrochen werden soll
            /// </summary>
            public bool FlagCancelLock;

            /// <summary>
            /// Jedes FFT-Object in FFT-System hat eine TypeID,
            /// Folgende Type IDs sind zugelassen
            /// 0 - Test oder FFT-Teststeps
            /// 1 - Datenobjekten wie CData, CDataIni, CDataDBF
            /// 2 - Gerätenobjekten CDevice
            /// </summary>
            public int IDType;
            public int ID;

            /// <summary>
            /// IDType für die Geräte wird von diesem Pool erstellt
            /// </summary>
            public static int PoolIDDevice = 0;
            /// <summary>
            /// IDType für die Daten wird von diesem Pool erstellt
            /// </summary>
            public static int PoolIDData = 0;

			/// <summary>
			/// Gesamte DelayZeit in einem FKT-Lauf (wird vor jedem Teststart auf 0 gesetzt) 
			/// </summary>
			public static long DelayTotal_Sec = 0;			        
		}
    }
}

	

	
	
	
	
	

	
	
	
	


