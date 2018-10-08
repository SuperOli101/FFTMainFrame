using System;
using System.Linq;
using System.Collections.Generic;
using Honeywell.Test;
using Honeywell.Data;
using System.IO;
using System.Windows.Forms;
using Honeywell.Forms;
using Honeywell.Device;

//Version Format: Major.Minor.Build.Revision
//System.Diagnostics.FileVersionInfo v = System.Diagnostics.FileVersionInfo.GetVersionInfo("Test.exe");
//Version V1 = new Version(v.FileVersion);
//Version V2 = new Version(1,2,3,4);
//if(V1<V2)
//    return;
namespace Honeywell
{
    namespace Test
    {
        /// <summary>
        /// CTest-Klasse ist eigentlich die Hauptklasse, die auf FormMainFrame (FFT-Windows) Zugriff hat. 
        /// Die CTest-Klasse kümmert sich um die Initialisierung von CData, CDevice und CTestStep („bool Initialize()“ „CTestStep CreateTestStep(int ID_TestStep)“) 
        /// und steuert den Ablauf von den Testschritten (int Execute()) im Programm.
        /// CTest hat den Zugriff auf jeden Testschritt. (CurrentTestSequencingLIST - die TestSteps, die von Test.Execute ausgeführt werden und
        /// CurrentTestStep - Testschritt, der gerade ausgeführt wird) 
        /// Nach dem Ausführen des letzten Testschrittes wird ErrorFirst__ID_TestStep (0-O.K. Nicht 0 - Fehler;) ausgewertet und
        /// das Ergebnis auf dem Bildschirm angezeigt.
        /// </summary>
        public class CTest : CObjectContainer
        {
            public const int FFT_ENDE = 0;

            public const int FFT_Template_Start = 1;
            public const int FFT_Template_Test1 = 2;
            public const int FFT_Template_Test2 = 3;
            public const int FFT_Template_Finish = 4;

            public Honeywell.Forms.CFormMainFrame FormFFT;
			
        						
	        public CTest():base()
	        {
				
	            this.Create();
            }


			//--------------------------------------------------------------------------------------
			/// <summary>
			/// Gibt Base ID zurück, Das heist FFT_Template{2} wir als FFT_Template behandelt
			/// </summary>
			/// <param name="ID">
			/// Name von Testschritt aus Sys-File
			/// Wenn Testschritt doppel in der Testsequenz vorkommt sein zweiter Name ist FFT_Template{2} und nicht FFT_Template
			/// </param>
			/// <returns>
			/// Base ID, Das heist FFT_Template{2} wir als FFT_Template behandelt
			/// </returns>
			public int GetID_TestStep(String ID)
			{
				int iIndexBracketClosing, iIndexBracketOpening, iLength, iID;
				String strID;

				iID = 0;
				iIndexBracketOpening = ID.IndexOf("{");
				iIndexBracketClosing = ID.IndexOf("}");
				iLength = ID.Length;
				if(iLength != 0)
				{
					if(iIndexBracketClosing == iLength - 1 && iIndexBracketOpening != -1)
					{
						strID = ID.Substring(0, iIndexBracketOpening);
					}
					else
					{
						strID = ID;
					}
					iID = this.Data.ID_TestStepLIST.IndexOf(strID);
					iID++;
				}

				return (iID);
			}
			/// <summary>
			/// Gibt Referenz auf den Testschritt, War der Testschritt nicht angelegt(Variantenmässig), wird der Testschritt neu angeleg; 
			/// null wenn es keinen Testschritt gibt.
			/// </summary>
			/// <param name="ID">
			/// TestStep ID
			/// </param>
			/// <returns>
			/// Referenz auf den Testschritt
			/// </returns>
			public CTestStep GetTestStepInstallIfNotThere(int ID)
			{
				CTestStep hTestStep;

				hTestStep = this.linkID_TestStepToTestStepLIST[ID - 1];
				if(hTestStep == null)
				{
					hTestStep = this.CreateTestStep(ID);
					if(hTestStep != null)
					{
						hTestStep.ID = ID;
						hTestStep.FlagDoExecuteFromTest = false;
					}
					this.linkID_TestStepToTestStepLIST[ID - 1] = hTestStep;
				}
				return (hTestStep);
			}
			/// <summary>
			/// Gibt Referenz auf den Testschritt, War der Testschritt nicht angelegt(Variantenmässig), wird null zurückgegeben; 
			/// </summary>
			/// <param name="ID">
			/// TestStep ID
			/// </param>
			/// <returns>
			/// Referenz auf den Testschritt
			/// </returns>
			public CTestStep GetTestStep(int ID)
			{
				CTestStep hTestStep;

				hTestStep = this.linkID_TestStepToTestStepLIST[ID - 1];				
				return (hTestStep);
			}

			//--------------------------------------------------------------------------------------
			/// <summary>
			/// Alle verwendete Namen für die gewählte Variante
			/// 0 - Position in dem Menu (von 1); 1 - OS-number; 2 - Variantenname
			/// </summary>
			public String[] CurrentID_MenuWithAllAliasARRAY;
			/// <summary>
			/// Gewählte Variante von 1 angefangen
			/// Default = 0 (nicht gewählt)
			/// </summary>
			public int CurrentID_Menu;
			/// <summary>
			/// Gibt aktuelle OSNummer zurück (aus der Menü gewählte Nummer)
			/// </summary>
			public string OSNumber
			{
				get { return CurrentID_MenuWithAllAliasARRAY[1]; }
			}
			/// <summary>
			/// Gibt aktuelle Variante zurück (aus der Menü gewählte Variante)
			/// </summary>
			public string Variant
			{
				get { return CurrentID_MenuWithAllAliasARRAY[2]; }
			}

			//--------------------------------------------------------------------------------------
			/// <summary>
			/// Führt die TestSchritte aus
			/// </summary>
            /// <returns>
			/// "-1" - Error mit Testabbruch; "0" - Teststep O.K.;	 "1" - Error, Test darf weitergemacht werden
			/// </returns>
	        public int Execute()
			{
				int iResult, iMax, i;
				double dYield;
				CTestStep hCurrentTestStep = null;
				string strMsgResult, strMsg, strYield;

                ////////////////////////////////////////////////////////////////////////
                #region Vorbereitung Test

                this.FormFFT.DoSettingBeforeFFT();
                this.Reset();

                this.FlagStateTest = 1;
                // Event, abonierbar
                this.OnBeforeTestStart();
                // Lampen, beide aus
                if (this.SetStartStopButtonLightHandler != null)
                    this.SetStartStopButtonLightHandler(false, false);

				iResult = 0;

                if (this.Data.DB == null)
                    this.ErrorReportPrintHeadLineLIST.Add(this.OSNumber + " " + DateTime.Now.ToString());
                if (this.CountTestRun != 0)
                {
                    dYield = 100D - ((double)(this.CountTestFail) * 100D / (double)(this.CountTestRun));
                    //11		= "Testläufe gesamt: {0}   Testläufe fehlerhaft: {1}  Yield: {2} %"
                    this.FormFFT.WriteToDisplay(string.Format(this.Data.Msg[11], this.CountTestRun + 1, this.CountTestFail, dYield.ToString("0.00")), 5);
                }
                this.Timer.SetTimer(11, Honeywell.Device.CTimer.Unit.Sec);
                this.FormFFT.SetCurrentTestTime(this.Timer.GetTimer(11));

                #endregion
                ////////////////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////////////////////////////
                #region Schleife Testschritte

                iMax = this.currentTestSequencingLIST.Count;			
				for(i = 0; i < iMax; i++)
				{
					hCurrentTestStep = this.currentTestSequencingLIST[i];
                    if (this.Data.FlagTestStepRunOnlyIfGoodLIST[hCurrentTestStep.ID - 1] && this.ErrorFirst__ID_TestStep != 0)//TestStep darf ausgeführt werden, wenn vorher keine Fehler aufgetretten sind
                    {
                        if (hCurrentTestStep.ID != this.currentTestSequencingLIST[iMax - 1].ID)//hCurrentTestStep ist nicht der letzte Testschritt in TestSequencing
                        {
                            continue;
                        }
                    }
					if(this.Data.DB != null && i == 0)
					{
                        
						if(!this.Data.DB.WriteToDBBeforeFFT())
						{
							hCurrentTestStep.WriteToScroll(this.Data.DB.Error);
							hCurrentTestStep.SetResultFault(997, false);
							break;
						}
                        this.ErrorReportPrintHeadLineLIST.Add(this.OSNumber + " " + this.Data.DB.Tab_ProductionStep.Date.ToString());                        
					}

					hCurrentTestStep.FlagExecutedFromTest = true;
                    this.OnBeforeTestStep(hCurrentTestStep);
					iResult = hCurrentTestStep.Execute();
                    this.OnAfterTestStep(hCurrentTestStep, iResult);
					hCurrentTestStep.FlagExecutedFromTest = false;

                    // Stoplampe ein
                    if (iResult != 0 && this.SetStartStopButtonLightHandler != null)
                        this.SetStartStopButtonLightHandler(false, true);

					if(iResult < 0 && this.FlagDoNotCheckCancel == false)//Test soll abgebrochen werden
					{
						if(hCurrentTestStep.ID != this.currentTestSequencingLIST[iMax - 1].ID)//hCurrentTestStep ist nicht der letzte Testschritt in TestSequencing
						{
							hCurrentTestStep = this.currentTestSequencingLIST[iMax - 1];
							hCurrentTestStep.FlagExecutedFromTest = true;
							this.SetModeDoNotCheckCancel(true);//Letzter Testschritt wird immer ausgeführt, Daswegen überprüfen von Abbrechen des Testschrittes abschalten
							iResult = hCurrentTestStep.Execute();
							this.SetModeDoNotCheckCancel(false);
							hCurrentTestStep.FlagExecutedFromTest = false;
							break;
						}
					}					
				}			

                #endregion
                ////////////////////////////////////////////////////////////////////////

                this.FormFFT.SetCurrentTestTime(0);
                //15		= "Testzeit       : {0} Sec"
                strMsg = String.Format(this.Data.Msg[15], this.Timer.GetTimer(11).ToString("0.0"));
                this.FormFFT.AddRowToListViewFFT(strMsg);
                this.WriteLineToReport(strMsg);

                if (this.Data.DB != null && hCurrentTestStep != null)
                {
                    bool bResult = true; ;
                    if (this.ErrorFirst__ID_TestStep != 0)
                        bResult = false;
                    if (!this.Data.DB.WriteToDBAfterFFT(bResult, this.Data.FFTMode))
                    {
                        hCurrentTestStep.WriteToScroll(this.Data.DB.Error);
                        hCurrentTestStep.SetResultFault(997, false);                        
                    }
                }


                if (!this.FlagTestCanceledByOperator)//Beim Esc Beenden wird nicht gezählt als Testlauf für Yield
				    this.CountTestRun++;
				if(this.ErrorFirst__ID_TestStep == 0)
				{
                    // Lampe grün an
                    if (this.SetStartStopButtonLightHandler != null)
                        this.SetStartStopButtonLightHandler(true, false);

					this.CountTestGood++;
					strMsgResult = " Testresult: TRUE ";

                    if (this.Data.Print == 1 && this.Print == 1)
                        this.FormFFT.StateDruckenToolStripButton(true);
				}
				else
				{
                    if (!this.FlagTestCanceledByOperator)//Beim Esc Beenden wird nicht gezählt als Testlauf für Yield
					    this.CountTestFail++;
					strMsgResult = " Testresult: FALSE ";
				}

                

                if (this.CountTestRun != 0)
                {
                    dYield = 100D - ((double)(this.CountTestFail) * 100D / (double)(this.CountTestRun));
                    strYield = string.Format(this.Data.Msg[11], this.CountTestRun, this.CountTestFail, dYield.ToString("0.00"));
                    //11		= "Testläufe gesamt: {0}   Testläufe fehlerhaft: {1}  Yield: {2} %"
                    this.FormFFT.WriteToDisplay(strYield, 5);                    
                }
                else
                    strYield = "";

                ////////////////////////////////////////////////////////////////////////
                #region//Testergebnis ins Report file schreiben
                this.writeTestResultToReport(strMsgResult, strYield);
                if (this.ErrorFirst__ID_TestStep != 0 && this.Data.DirectoryOfErrorFile.Length != 0)//Wenn Ergebnis fehlerhaft ist, dann zusätliche Errordatei erstellen (wenn this.Data.DirectoryOfErrorFile != "")
                {
                    if (!this.FlagTestCanceledByOperator)
                    {
                        System.Text.StringBuilder tempBuffer;
                        tempBuffer = new System.Text.StringBuilder(this.Data.Report.Buffer.ToString());
                        string nameFile;
                        DateTime.Now.ToString("yyyyMMddHHmmss");
                        nameFile = string.Format("{0}\\{1}__{2}_{3}_{4}.txt", this.Data.DirectoryOfErrorFile, this.OSNumber, this.ErrorFirst__ID_TestStep, this.ErrorFirst__CodeErrorTestStep, DateTime.Now.ToString("yyyyMMddHHmmss"));
                        this.Data.Report.WriteToFileOverwrite(nameFile);
                        this.Data.Report.Buffer = tempBuffer;
                    }
                }
                this.Data.Report.WriteToFileAppend(this.Data.Report.NameFull);
                #endregion
                ////////////////////////////////////////////////////////////////////////

                iResult = this.FormFFT.DoSettingAfterFFT();
                // Event nach dem Test
                this.OnAfterTestEnded(iResult);

                this.FlagStateTest = 0;
				return iResult;
			}
			/// <summary>
			/// Initialisiert CData, CDevice und CTestStep
			/// </summary>
			/// <returns>
			/// Ergebnis
			/// </returns>
			public override bool Initialize()
			{
				String strFullNameSysFile;

				if(this.Data == null)
				{                    
					//Wenn über _mainFrame Projekt gestartet wurde (nicht über _MyTest)
					this.Test = this;
                    if (!this.GetInfoSystemFile(out strFullNameSysFile))
                    {
                        return (false);
                    }
					//strFullNameSysFile = "D:\\Mein\\Projekte\\_Probe\\Probe_2005\\MainFrameCSharp\\MyProject\\_MainFrame\\_TestSystem\\MainFrame.sys";
					this.Data = new CData(strFullNameSysFile);
                    this.Data.StartInstalRessource();
					if(!this.Data.Initialize())
					{
                        this.Data.StopInstalRessource();
						this.Error = this.Data.Error;
						return (false);
					}
					///////////////////////////////////////////////////////////////                    

                    this.Data.StopInstalRessource();
				}

                if (this.CurrentID_Menu != 0)
                {
                    if (!this.CreateTestSteps())
                        return (false);
                }
				this.Clear();

                if (this.Data.DB_FlagCreate)
                {
                    if (this.Data.DB == null)
                    {
                        CDbSql.WriteLineToReportFileHandle writeLineToReportFile;
                        if (this.Data.DB_Report != 0)
                            writeLineToReportFile = this.Data.WriteLineToReport;
                        else
                            writeLineToReportFile = null;

                        this.Data.DB = new CPartProductionDBforFFT(this.Data.DB_ServerName, this.Data.DB_DataBaseName, this.Data.DB_Login, this.Data.DB_Password, writeLineToReportFile);
                        this.Data.DB.DbSql.ShowMessageBox = this.ShowMessageBox;
                        if (this.Data.DB.Error.Length != 0)
                        {
                            this.SetError(this.Data.DB.Error);
                            this.Data.DB = null;
                            return (false);
                        }
                        else
                        {                                                        
                            string strNameSW;
                            this.Test.GetInfoExecutingAssembly(out strNameSW);
                            this.Data.WriteLineToReport(strNameSW);
                            //strNameSW = System.IO.Path.Combine(this.Test.Path, System.IO.Path.GetFileNameWithoutExtension(strNameSWDebug) + ".exe");
                            string strVersionSW;
                            strVersionSW = System.Diagnostics.FileVersionInfo.GetVersionInfo(strNameSW).FileVersion;

                            try
                            {
                                Honeywell.Data.CPartProductionDBforFFT.CConfigurationParameterFFTSW configurationParameterSW = new Honeywell.Data.CPartProductionDBforFFT.CConfigurationParameterFFTSW
                                    (strNameSW, strVersionSW, this.Data.DB_DescriptionFFTSoftware, this.Data.DB_SoftwareCategory, this.Data.DB_DescriptionSoftwareCategory,
                                        this.Data.DB_Department, this.Data.DB_SiteCode, this.Data.FFTMode, this.GetErrorParameter, this.GetTestStepParameter);
                                Honeywell.Data.CPartProductionDBforFFT.CConfigurationParameterFFTBatch ConfigurationParameterBatch = new CPartProductionDBforFFT.CConfigurationParameterFFTBatch
                                    (this.OSNumber, this.Data.LinkID_MenuToDescriptionSequencingLIST[this.CurrentID_Menu - 1], this.Data.DB_OrderName);
                                if (!this.Data.DB.SetConfigurationFFT(configurationParameterSW, ConfigurationParameterBatch))
                                {
                                    this.SetError(this.Data.DB.Error);
                                    this.Data.DB = null;
                                    return (false);
                                }

                                this.Data.OrderNameCurrent = ConfigurationParameterBatch.OrderNameCurrent;
                                this.TestSoftwareCategoryArray = this.Data.DB.Tab_ProductionCategory.GetProductionCategory();
                            }
                            catch (Exception ex)
                            {
                                this.SetError(string.Format("Error while writing to DB\r\n\r\n({0}\r\n{1})", ex.Message, ex.StackTrace.ToString()));
                                this.Data.DB = null;
                                return (false);
                            }
                        }
                    }                    
                }

				return (true);
			}
			/// <summary>
			/// Deinstaliert CData und CDevice
			/// </summary>
			public override void Deinstall()
			{
                this.Reset();
                if(this.Data != null)
				    this.Data.Deinstall();
                if(this.Device != null)
				    this.Device.Deinstall();
				return;
			}

			//--------------------------------------------------------------------------------------			
            /// <summary>
            /// Anzahl von Testläufe
            /// </summary>
            public int CountTestRun = 0;
			/// <summary>
			/// Anzahl von den fehlerhaften Testläufen
			/// </summary>
            public int CountTestFail = 0;
			/// <summary>
			/// Anzahl von den gut geprüften Testläufen
			/// </summary>
            public int CountTestGood = 0;

			//--------------------------------------------------------------------------------------
			/// <summary>
			/// Gibt den Namen von Exe-File
			/// </summary>
			/// <param name="FullName">
			/// Name von Exe-File mit Pfad und Erweiterung
			/// </param>
            public virtual void GetInfoExecutingAssembly(out String FullName)
            {                
                FullName = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return;
            }
			/// <summary>
			/// Gibt den Namen von Sys-File
			/// </summary>
			/// <param name="FullName">
			/// Name von Exe-File mit Pfad und Erweiterung
			/// </param>
            public bool GetInfoSystemFile(out String FullName)
            {
                DirectoryInfo DirInfo;

                String strNameFull, strName, strPath, strError, strFFTDirectory, strBuffer, strFullNameSysFile;

                this.GetInfoExecutingAssembly(out strNameFull);
                Honeywell.Data.CFile.GetPath(strNameFull, out strPath, out strError);
                Honeywell.Data.CFile.GetNameShort(strNameFull, out strName, out strError);

                FullName = "";
                strFFTDirectory = "FFTSystem";
                while (true)
                {
                    strBuffer = strPath + "\\" + strFFTDirectory;
                    if (!System.IO.Directory.Exists(strBuffer))
                    {
                        DirInfo = System.IO.Directory.GetParent(strPath);
                        if (DirInfo == null)
                        {
                            strError = String.Format("Directory with FFT-System data was not found ({0})", strFFTDirectory);
                            this.Error=strError;
                            return (false);
                        }
                        strPath = DirInfo.FullName;
                    }
                    else
                        break;
                }


                strFullNameSysFile = strPath + "\\" + strFFTDirectory + "\\" + strName + ".sys";


                if (!File.Exists(strFullNameSysFile))
                {
                    strError = String.Format("Sys-file was not found ({0})", strFullNameSysFile);
                    this.Error=strError;
                    return (false);
                }

                FullName = strFullNameSysFile;
                return (true);
            }

            /// <summary>
            ///Wird vor jeder Testserie ausgeführt in CTest.Initialize()
            /// </summary>
            public void Clear()            
            {
                this.CountTestRun=0;
                this.CountTestFail=0;
                this.CountTestGood=0;
                this.CountOfError.Clear();

                this.Reset();
            }

            /// <summary>
            ///Wird nach jedem Testlauf ausgeführt in CTest.Execute()
            ///Setzt ErrorFirst__ID_TestStep, ErrorFirst__CodeErrorTestStep, FlagDoCancelTest auf default
            /// </summary>
	        public void Reset()            
            {
                int iMax, i;
                CTestStep hCurrentTestStep;

				if(this.FormFFT != null)
				{
					this.FormFFT.DeleteScroll(0);
					this.FormFFT.DeleteDisplay(0);
					this.FormFFT.DeleteStatus();

					this.FormFFT.Update();
				}

				CObjectTest.DelayTotal_Sec = 0;
                this.FlagDoCancelTest = false;//wird immer gesetzt egal was für ein Flag gesetzt ist
                this.Error = "";
                this.ErrorFirst__CodeErrorTestStep = 0;
                this.ErrorFirst__ID_TestStep = 0;
                this.ErrorAttributeLIST.Clear();
				
				this.ErrorReportPrintHeadLineLIST.Clear();
				this.ErrorReportPrintFootLineLIST.Clear();

                this.FlagBreakPointStop = false;
                this.FlagTestCanceledByOperator = false;

                if (this.linkID_TestStepToTestStepLIST != null)
                {
                    iMax = this.linkID_TestStepToTestStepLIST.Count;
                    for (i = 0; i < iMax; i++)
                    {
                        hCurrentTestStep = this.linkID_TestStepToTestStepLIST[i];
                        if (hCurrentTestStep != null)
                            hCurrentTestStep.Reset();
                    }
                }
                if (this.currentTestSequencingLIST != null)
                {
                    iMax = this.currentTestSequencingLIST.Count;
                    for (i = 0; i < iMax; i++)
                    {
                        hCurrentTestStep = this.currentTestSequencingLIST[i];
                        if (hCurrentTestStep != null)
                            hCurrentTestStep.Reset();
                    }
                }                
            }

			/// <summary>
			/// Fügt den TestStep zu m_hCurrentTestSequencingLIST ein
			/// </summary>
            /// <param name="TestStep">
			/// Referenz auf TestStep
			/// </param>
	        public void AddTestStep(CTestStep TestStep)            
            {
            	this.currentTestSequencingLIST.Add(TestStep);
	            return;
            }

            /// <summary>
			/// Schaltet eine Testmode wo es nicht geprüft wird, ob es abgebrochen soll, aber registriert.(Unterschied zu FlagCancelLock)
            /// </summary>
            /// <param name="Unit">
			/// true - wird nicht geprüft, ob es abgebrochen soll, aber registriert.(Unterschied zu FlagCancelLock)
			/// </param>
	        public void SetModeDoNotCheckCancel(bool Mode)            
            {
            	//1-wird nicht geprüft ob es abgebrochen aber registriert Unterschied zu FlagCancelLock
	            this.FlagDoNotCheckCancel=Mode;
	            return;
            }
			

			/// <summary>
			/// Zeigt Fehler Meldungen die in CTest.Initialize entstanden sind.
			/// </summary>
            public void ShowErrorInitialize()
            {
                MessageBox.Show(this.Error, "Error (Test)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            public virtual void PrintFromFFT()
            {
            }

			/// <summary>
			/// Hier werden die TestSchritte erstellt.
			/// Die Funktion sollte in CMyTest überschrieben werden
			/// </summary>
			/// <param name="ID_TestStep">
			/// TestSschritt ID aus Sys-File
            /// </param>
			/// <returns>
			/// Referenz auf den neu entstandenen Testschritt
			/// </returns>
	        public virtual CTestStep CreateTestStep(int ID_TestStep)
            {
                CTestStep hTestStep;
                hTestStep = null;

                switch (ID_TestStep)
                {

                    case CTest.FFT_Template_Start:
                        hTestStep = new CFFT_Template_Start();
                        break;

                    case CTest.FFT_Template_Test1:
                        hTestStep = new CFFT_Template_Test1();
                        break;
                    case CTest.FFT_Template_Test2:
                        hTestStep = new CFFT_Template_Test2();
                        break;

                    case CTest.FFT_Template_Finish:
                        hTestStep = new CFFT_Template_Finish();
                        break;
                } 

                return (hTestStep);
            }

            private class TestStepType
            {
                public string Name { get; set; }
                public int ID { get; set; }
                public string Argument { get; set; }
                public string Validity { get; set; }
                public bool Auto { get; set; }
            }

			/// <summary>
			/// Werden alle Testschritte für CurrentID_Menu erstellt
			/// </summary>
            /// <returns>
			/// Ergebnis
			/// </returns>
	        public bool CreateTestSteps()            
            {
	            String strNameTestSequencing;
	            CTestStep hTestStep;
	            Dictionary<String, bool> hTestSequencingDICTIONARY;
                List<TestStepType> hTestStepSequence;

                strNameTestSequencing = this.Data.LinkID_MenuToSequencingLIST[this.CurrentID_Menu - 1];
	            hTestSequencingDICTIONARY=this.Data.SequencingDICTIONARY[strNameTestSequencing];

                // Testschritte aus der Sys-File parsen und gültigkeit überprüfen ///////////////////////
                hTestStepSequence = new List<TestStepType>();
                foreach (KeyValuePair<String, bool> cTestStepName in hTestSequencingDICTIONARY)
                {
                    int iIdx;
                    TestStepType testStep = new TestStepType();
                    testStep.Name = cTestStepName.Key;
                    testStep.Auto = cTestStepName.Value;

                    iIdx = cTestStepName.Key.IndexOf('[');
                    if (iIdx != -1)
                    {
                        testStep.Validity = cTestStepName.Key.Substring(iIdx + 1);
                        iIdx = testStep.Validity.IndexOf(']');
                        testStep.Validity = testStep.Validity.Remove(iIdx);
                    }

                    iIdx = cTestStepName.Key.IndexOf('(');
                    if (iIdx != -1)
                    {
                        testStep.Argument = cTestStepName.Key.Substring(iIdx + 1);
                        iIdx = testStep.Argument.IndexOf(')');
                        testStep.Argument = testStep.Argument.Remove(iIdx);
                    }

                    if (testStep.Validity != null)
                    {
                        iIdx = testStep.Name.IndexOf('[');
                        testStep.Name = testStep.Name.Remove(iIdx);
                    }
                    else if (testStep.Argument != null)
                    {
                        iIdx = testStep.Name.IndexOf('(');
                        testStep.Name = testStep.Name.Remove(iIdx);
                    }

                    if (!String.IsNullOrWhiteSpace(testStep.Validity))
                    {
                        // Zeile juckt nicht
                        if (testStep.Validity != OSNumber && testStep.Validity != Variant)
                            continue;
                    }

                    testStep.ID = GetID_TestStep(testStep.Name);

                    var compare = from step in hTestStepSequence
                                  where step.Name == testStep.Name
                                  select step;

                    if (compare.Count() > 0)
                    {
                        int testStepLevel = 0;
                        int compTestStepLevel = 0;

                        TestStepType compTestStep = compare.ElementAt(0);

                        if (!String.IsNullOrWhiteSpace(testStep.Validity))
                        {
                            if (testStep.Validity == Variant) testStepLevel = 1;
                            if (testStep.Validity == OSNumber) testStepLevel = 2;
                        }

                        if (!String.IsNullOrWhiteSpace(compTestStep.Validity))
                        {
                            if (compTestStep.Validity == Variant) compTestStepLevel = 1;
                            if (compTestStep.Validity == OSNumber) compTestStepLevel = 2;
                        }

                        if (testStepLevel > compTestStepLevel)
                        {
                            hTestStepSequence.Remove(compTestStep);
                            hTestStepSequence.Add(testStep);
                        }
                    }
                    else
                    {
                        hTestStepSequence.Add(testStep);
                    }
                }

                int iMax = this.Data.ID_TestStepLIST.Count;
                this.linkID_TestStepToTestStepLIST = new List<CTestStep>(iMax);
                for (int i = 0; i < iMax; i++)
                {
                    this.linkID_TestStepToTestStepLIST.Add(null);
                }
                this.currentTestSequencingLIST = new List<CTestStep>();

                // Testschritte überprüfen und anlegen ///////////////////////////////////////////
                foreach (TestStepType testStep in hTestStepSequence)
                {
                    if (testStep.ID == 0)
                    {
                        //4		= "Der Testschritt {1} aus [{0}] ist nicht in [TestSteps] beschrieben" //strNameTestSequencing,strNameTestStep
                        this.Data.Error = String.Format(this.Data.Msg[4], strNameTestSequencing, testStep.Name);
                        this.Error = this.Data.Error;
                        return (false);
                    }

                    hTestStep = this.CreateTestStep(testStep.ID);
                    if (hTestStep == null)
                    {
                        //5		= "Der Testschritt {1} aus [{0}] ist nicht in FFT-SW beschrieben" //strNameTestStep				
                        this.Data.Error = String.Format(this.Data.Msg[5], strNameTestSequencing, testStep.Name);
                        this.Error = this.Data.Error;
                        return (false);
                    }

                    hTestStep.TestStepParameter = testStep.Argument;
                    hTestStep.ID = testStep.ID;
                    hTestStep.FlagDoExecuteFromTest = testStep.Auto;

                    this.linkID_TestStepToTestStepLIST[hTestStep.ID - 1] = hTestStep;
                    if (testStep.Auto)
                        this.currentTestSequencingLIST.Add(hTestStep);
                }

	            return(true);
            }

			/// <summary>
			/// Wird Menü zum Variantenauswählen aufgerufen
			/// </summary>
			public void SetVariant()
			{
				int i;
				Honeywell.Forms.CFormMenuSelect hForm;
				String[] TextARRAY;

				hForm = new Honeywell.Forms.CFormMenuSelect(this);
                this.SetIconToFormMenuSelect(hForm);
				for(i = 0; i < this.Data.ID_MenuLIST.Count; i++)
				{
					TextARRAY = new String[2];
					TextARRAY[0] = this.Data.ID_MenuLIST[i];
					TextARRAY[1] = this.Data.LinkID_MenuToDescriptionSequencingLIST[i];

					hForm.AddItem(TextARRAY);
				}

                if (this.CurrentID_Menu == 0)
                {
                    System.Windows.Forms.Application.Run(hForm);
                }
                else
                {
                    hForm.ShowDialog();
                }

                hForm = null;
			}

            protected virtual void SetIconToFormMenuSelect(Form Formular)
            {
                //System.Resources.ResourceManager rm = new System.Resources.ResourceManager(typeof(Honeywell.Forms.CFormMenuSelect));
                //Formular.Icon = ((System.Drawing.Icon)(rm.GetObject("ScanAndStart")));
               
            }

            public bool ChangeVariant(string Variant)
            {              
                this.Deinstall();
                this.CurrentID_Menu = this.Data.ID_MenuLIST.IndexOf(Variant) + 1;
                //Alle Alias zu current Varian eintragen ////////////////////////////////////////////////////             
                this.CurrentID_MenuWithAllAliasARRAY[0] = this.CurrentID_Menu.ToString();							    //Position in der Menü von 1 angefangen
                this.CurrentID_MenuWithAllAliasARRAY[1] = this.Data.ID_MenuLIST[this.CurrentID_Menu - 1];				//OS-Number
                this.CurrentID_MenuWithAllAliasARRAY[2] = this.Data.LinkID_MenuToVariantLIST[this.CurrentID_Menu - 1];	//Variantenname
                /////////////////////////////////////////////////////////////////////////////
                if (!this.Initialize())
                {
                    this.Test.ShowErrorInitialize();                    
                    return false;
                }

                this.FormFFT.ShowPictureFFTState(Honeywell.Test.CTest.StateFFT.Neutral);
                this.SetTestTimeMax(this.TestTimeMax);

                return true;

            }

            /// <summary>
            /// Setzt max. Testzeit für die Anzeige des Progresses unten rechts (Aufrufen nur wenn Formular aufgebaut ist)
            /// </summary>
            /// <param name="TestTimeMax">
            /// 0 - Testzeit wird nicht angezeigt
            /// </param>
            public void SetTestTimeMax(double TestTimeMax)
            {                
                this.FormFFT.SetTestTimeMax(TestTimeMax);
            }
            /// <summary>
            /// Maximale Testzeit [Sec]
            /// 0 - Testzeit wird unten rechts nicht angezeigt
            /// </summary>
            public int TestTimeMax;

            public int ShowMessageBox(string Text, string Caption, int Buttons, int Icon)
            {
                DialogResult result;
                result = MessageBox.Show(Text, Caption, (MessageBoxButtons)Buttons, (MessageBoxIcon)Icon);

                return (int)result;
            }

			//--------------------------------------------------------------------------------------
            /// <summary>
            /// gerade ausgeführte TestStep. Wird immer in der CTestStep.Execute() gesetzt
            /// </summary>
            public CTestStep CurrentTestStep;            

            /// <summary>
            /// true Test sollte beendet werden nur letzter Testschritt wird ausgeführt
            /// volatile erlaubt Zugriff auf die Variable von mehreren Threads
            /// Wird mit CObjectTest.SetCancelTest() gesetzt
            /// </summary>
            public volatile bool FlagDoCancelTest;
            /// <summary>
            /// Test wurd duch Esc-Taste beendet
            /// </summary>
            public volatile bool FlagTestCanceledByOperator;
            /// <summary>
            /// Zeigt den Teststatus 1-FFT läuft; 0-FFT wartet
            /// </summary>
            public int FlagStateTest;
            /// <summary>
            /// Zeigt den FFT-Buttons (Start und Stop). Wird von Dlg benutzt um mit Tasten auf demm FFT bestätigen zu können(Muss vor dem benutzen auf -1 gesetzt werden)
            /// 1-Starttaste gedrückt; 0-Stoptaste gedrückt; -1-Tasten wurden nicht betätigt
            /// </summary>
            public int? FlagButtonOnFFT = null;
            /// <summary>
            /// Was für ein Bild als Ergebnis gerade gezeigt wird
            /// null - keins
            /// 0 - good
            /// -1 - error
            /// 1 - run
            /// </summary>
            public int? StateOfPictureResult = null;

			public string ArgumentCommandLineCurrentVariant = null;
            public string ArgumentCommandLineFFTMode = null;
            /// <summary>
            ///true - hält an jedem BreakPoint an (F10), false hält nicht aber kann F10-Schlag abfangen  wenn ModeDebug==1
            /// </summary>
            public bool FlagBreakPointStop;            
        	
			/// <summary>
			/// Errorcode vom ersten Fehler
			/// </summary>
	        public int ErrorFirst__CodeErrorTestStep;
			/// <summary>
			/// TestStep ID wo der erste Fehler entstanden ist
			/// </summary>
	        public int ErrorFirst__ID_TestStep;

			/// <summary>
			/// Wird im Konstrukter aufgerufen
			/// </summary>
            public new bool Create()
            {
	            this.CurrentID_Menu=0;	            
	            this.FlagDoNotCheckCancel=false;
                this.FlagDoCancelTest = false;//wird immer gesetzt egal was für ein Flag gesetzt ist
                this.FlagStateTest = 0;
                this.FlagButtonOnFFT = null;
                

	            this.ErrorFirst__CodeErrorTestStep=0;
	            this.ErrorFirst__ID_TestStep=0;

                this.CurrentID_MenuWithAllAliasARRAY=new String[3];            	

	            this.currentTestSequencingLIST=null;

                this.CountOfError = new Dictionary<String, int>();
                this.ErrorAttributeLIST = new List<CTestStep.CErrorTestStep>();
				this.ErrorReportPrintHeadLineLIST = new List<string>();
				this.ErrorReportPrintFootLineLIST = new List<string>();				

                this.TestTimeMax = 0;
	            return(true);
            }

            /// <summary>
            /// Unit=true-wird nicht regestriert und nicht getestet ob der Test abgebrochen werden soll
            /// </summary>
            public void SetCancelLock(bool Mode)           
            {
                this.FlagCancelLock = Mode;
            }

            /// <summary>
            /// true-wird nicht geprüft, ob es abgebrochen soll, aber registriert.(Unterschied zu FlagCancelLock)
            /// </summary>
	        public bool FlagDoNotCheckCancel;            

            /// <summary>
            /// Nur die TestSteps die von Test.Execute ausgeführt werden.
            /// </summary>
            protected List<CTestStep> currentTestSequencingLIST;            

            /// <summary>
            /// Enthält alle in Sys-Datei deklarierte TestSteps. 
            /// Die TestSteps werden in CreateTestSteps angelegt und nur die, 
            /// die zum aktuellen Sequencing gebraucht werden.
            /// In der GetTestStepInstallIfNotThere werden die TestSteps nachinstalliert, wenn sie gebraucht werden.
            /// </summary>
            protected List<CTestStep> linkID_TestStepToTestStepLIST;

            public string GetErrorParameter(int TesStepId, int ErrorCode, out string Description)
            {
                string strError = "";
                Description = "";
                try
                {
                    string strNameTestStep = this.Data.ID_TestStepLIST[TesStepId - 1];
                    Description = this.Data.Msg.GetNameError(strNameTestStep, ErrorCode);
                }
                catch (Exception ex)
                {
                    strError = ex.Message;
                }

                return strError;
            }
            public string GetTestStepParameter(int TesStepId, out string Name, out string Description)
            {
                string strError = "";
                Name = "";
                Description = "";
                try
                {
                    Name = this.Data.ID_TestStepLIST[TesStepId - 1];
                    Description = this.Data.Msg.GetNameTestStep(Name);
                }
                catch (Exception ex)
                {
                    strError = ex.Message;
                }

                return strError;
            }
			
            /// <summary>
            /// Liste von den Erroreigenschaften(Fehlercode), die in einem Testlauf enstanden sind
            /// </summary>
            public List<CTestStep.CErrorTestStep> ErrorAttributeLIST;
			/// <summary>
			/// Wird benutzt beim Drucken vom Fehlerreport als Kopfzeilen
			/// </summary>
			public List<string> ErrorReportPrintHeadLineLIST;
			/// <summary>
			/// Wird benutzt beim Drucken vom Fehlerreport als Fusszeilen 
			/// </summary>
			public List<string> ErrorReportPrintFootLineLIST;
			/// <summary>
			/// Druckt das Fehlerreport bei 1
			/// </summary>
			public int PrinterError = 0;
            /// <summary>
            /// Erlaubt das Drucken aus Menü
            /// </summary>
            public int Print = 1;
			/// <summary>
			/// Druckt Msg auf dem Error Drucker 
			/// </summary>
			/// <param name="Msg">
			/// Message zum Drucken
			/// </param>
			/// <returns>
			/// Ergebnis
			/// </returns>
			public bool DoPrintError(string Msg)
			{
				if(this.PrinterError == 1)
				{
					if(this.Device != null)
					{
						if(this.Device.FFTester != null)
						{
							if(!this.Device.FFTester.DoPrintError(Msg))
								return false;
						}
					}
				}
				return true;
			}
			public bool DoFeedErrorPrinter(int Count = 1)
			{
				if(this.PrinterError == 1)
				{
					if(this.Device != null)
					{
						if(this.Device.FFTester != null)
						{
							if(!this.Device.FFTester.DoFeedErrorPrinter(Count))
								return false;
						}
					}
				}
				return true;
			}
			/// <summary>
			/// Fügt eine Zeile im Kopf vom Fehlerreport zum Ausdrucken
			/// </summary>
			/// <param name="Msg">
			/// Zeile im Kopf
			/// </param>
			public void AddHeadLineErrorReportPrint(string Msg)
			{
				this.ErrorReportPrintHeadLineLIST.Add(Msg);
			}
			/// <summary>
			/// Fügt eine Zeile im Fuß vom Fehlerreport  zum Ausdrucken
			/// </summary>
			/// <param name="Msg">
			/// Zeile im Fuß
			/// </param>
			public void AddFootLineErrorReportPrint(string Msg)
			{
				this.ErrorReportPrintFootLineLIST.Add(Msg);
			}
			/// <summary>
			/// Anzahl von Vorschuben nach dem Drucken vom Fehlerreport
			/// </summary>
			public int CountFeedErrorPrinter = 0;

            /// <summary>
            /// Wird die Erroranzahl im Testserie aufgesammelt (ErrorCodeString,Erroranzahl -> 3:4,6 Fehler 3:4 kommt 6 mal im Test)
            /// </summary>
            public Dictionary<String, int> CountOfError;
            /// <summary>
            /// Array für MenuItems in Extras -> SQL Data Base
            /// </summary>
            public Honeywell.Data.CPartProductionDB.tab_ProductionCategory[] TestSoftwareCategoryArray;

            /// <summary>
            /// Setzt die Lampen/LED der Start- und oder Stoptasten
            /// </summary>
            /// <param name="stateStart">Angabe ob die Startlampe ein (true) oder ausgeschaltet (false) werden soll</param>
            /// <param name="stateStop">Angabe ob die Stoplampe ein (true) oder ausgeschaltet (false) werden soll</param>
            public delegate void SetStartStopButtonLightDelegate(bool stateStart, bool stateStop);

            /// <summary>
            /// Legt den Delegaten für die Behandlung der Start- und oder Stoptasten fest, oder gibt diesen zurück
            /// </summary>
            public SetStartStopButtonLightDelegate SetStartStopButtonLightHandler { get; set; }

            #region // #### Testevents ##################################################
            
            /// <summary>
            /// Wird ausgelöst bevor ein Testlauf gestartet wird
            /// </summary>
            public event EventHandler<BeforeTestStartEventArgs> BeforeTestStart;

            /// <summary>
            /// Funktion zum Auslösen des BeforeTestStart Events
            /// </summary>
            protected void OnBeforeTestStart()
            {
                if (BeforeTestStart != null)
                    BeforeTestStart(this, 
                        new BeforeTestStartEventArgs(this.Test.OSNumber, this.Test.Variant));
            }

            /// <summary>
            /// Wird ausgelöst nachdem ein Testlauf beendet wurde
            /// </summary>
            public event EventHandler<AfterTestEndedEventArgs> AfterTestEnded;

            /// <summary>
            /// Funktion zum Auslösen des AfterTestEnded Events
            /// </summary>
            /// <param name="testResult">Angabe des Testergebnis</param>
            protected void OnAfterTestEnded(int testResult)
            {
                if (AfterTestEnded != null)
                    AfterTestEnded(this, new AfterTestEndedEventArgs(
                        this.Test.OSNumber, this.Test.Variant, testResult));
            }

            /// <summary>
            /// Wird ausgelöst bevor ein Testschritt ausgeführt wird
            /// </summary>
            public event EventHandler<BeforeTestStepEventArgs> BeforeTestStep;

            /// <summary>
            /// Funktion zum Auslösen des BeforeTestStep Events
            /// </summary>
            /// <param name="testStep">Angabe des Testschritts der Ausgeführt werden soll</param>
            protected void OnBeforeTestStep(CTestStep testStep)
            {
                if(BeforeTestStep != null)
                    BeforeTestStep(this, new BeforeTestStepEventArgs(
                        this.Test.OSNumber, this.Test.Variant, testStep));
            }

            /// <summary>
            /// Wird ausgelöst nachdem ein Testschritt ausgeführt wurde
            /// </summary>
            public event EventHandler<AfterTestStepEventArgs> AfterTestStep;

            /// <summary>
            /// Funktion zum Auslösen des AfterTestStep Events
            /// </summary>
            /// <param name="testStep">Angabe des Testschritts der ausgeführt wurde</param>
            /// <param name="testStepResult">Angabe des Ergebnisses des ausgeführten Testschritts</param>
            protected void OnAfterTestStep(CTestStep testStep, int testStepResult)
            {
                if (AfterTestStep != null)
                    AfterTestStep(this, new AfterTestStepEventArgs(
                        this.Test.OSNumber, this.Test.Variant, testStep, testStepResult));
            }

            #endregion

			private void writeTestResultToReport(string TestResultString, string YieldString)
			{
				string strMsgLineSeparate, strMsg;
				DateTime DateNow;


				DateNow = DateTime.Now;
				strMsgLineSeparate = "";
				strMsgLineSeparate = strMsgLineSeparate.PadLeft(this.Data.LengthScroll, '-');
				this.WriteLineToReport(strMsgLineSeparate);
				strMsg = TestResultString.PadLeft(this.Data.LengthScroll / 2, '-');
				strMsg = strMsg.PadRight(this.Data.LengthScroll, '-');
				this.WriteLineToReport(strMsg);
				this.WriteLineToReport(strMsgLineSeparate);
				this.WriteLineToReport(strMsgLineSeparate);
				strMsg = DateNow.ToString();
				strMsg = strMsg.PadLeft(this.Data.LengthScroll / 2);
				strMsg = strMsg.PadRight(this.Data.LengthScroll);
				this.WriteLineToReport(strMsg);
				strMsg = YieldString;
				strMsg = strMsg.PadLeft(this.Data.LengthScroll / 2);
				strMsg = strMsg.PadRight(this.Data.LengthScroll);
				this.WriteLineToReport(strMsg);
				this.WriteLineToReport(strMsgLineSeparate);				

				return;
			}

            public enum ModeFFT : int
            {
                Production  = 0,
                Experten    = 1,
                Engineering = 2
            }

            public enum StateFFT : int
            {
                Neutral     = 0,
                Run         = 1,
                Error       = 2,
                Good        = 3
            }

            public string Path = "";
        }                          

        /// <summary>
        /// Stellt Daten für das BeforeTestStart-Ereignis bereit
        /// </summary>
        public class BeforeTestStartEventArgs : EventArgs
        {
            /// <summary>
            /// Gibt die für den Test ausgewählte OS-Nummer zurück
            /// </summary>
            public string OsNumber { get; private set; }
            /// <summary>
            /// Gibt die für den Test ausgewählte Variante zurück
            /// </summary>
            public string Variant { get; private set; }
            
            /// <summary>
            /// Initialisiert eine neue Instanz der BeforeTestStartEventArgs-Klasse
            /// </summary>
            /// <param name="osNumber">Angabe der für den Test gewählten OS-Nummer</param>
            /// <param name="variant">Angabe der für den Test gewählten Variante</param>
            public BeforeTestStartEventArgs(string osNumber, string variant)
            {
                this.OsNumber = osNumber;
                this.Variant = variant;
            }
        }

        /// <summary>
        /// Stellt Daten für das AfterTestEnded-Ereignis bereit
        /// </summary>
        public class AfterTestEndedEventArgs : EventArgs
        {
            /// <summary>
            /// Gibt die für den Test ausgewählte OS-Nummer zurück
            /// </summary>
            public string OsNumber { get; private set; }
            /// <summary>
            /// Gibt die für den Test ausgewählte Variante zurück
            /// </summary>
            public string Variant { get; private set; }
            /// <summary>
            /// Gibt das Ergebnis des Tests zurück
            /// </summary>
            public int TestResult { get; private set; }

            /// <summary>
            /// Initialisiert eine neue Instanz der AfterTestEndedEventArgs-Klasse
            /// </summary>
            /// <param name="osNumber">Angabe der für den Test gewählten OS-Nummer</param>
            /// <param name="variant">Angabe der für den Test gewählten Variante</param>
            /// <param name="testResult">Angabe des Testergebnis</param>
            public AfterTestEndedEventArgs(string osNumber, string variant, int testResult)
            {
                this.OsNumber = osNumber;
                this.Variant = variant;
                this.TestResult = testResult;
            }
        }

        /// <summary>
        /// Stellt Daten für das BeforeTestStep-Ereignis bereit
        /// </summary>
        public class BeforeTestStepEventArgs : EventArgs
        {
            /// <summary>
            /// Gibt die für den Test ausgewählte OS-Nummer zurück
            /// </summary>
            public string OsNumber { get; private set; }
            /// <summary>
            /// Gibt die für den Test ausgewählte Variante zurück
            /// </summary>
            public string Variant { get; private set; }
            /// <summary>
            /// Gibt das Objekt des zu ausführenden Testschritt zurück
            /// </summary>
            public CTestStep TestStep { get; private set; }

            /// <summary>
            /// Initialisiert eine neue Instanz der BeforeTestStepEventArgs-Klasse
            /// </summary>
            /// <param name="osNumber">Angabe der für den Test gewählten OS-Nummer</param>
            /// <param name="variant">Angabe der für den Test gewählten Variante</param>
            /// <param name="testStep">Angabe des Testschritt-Objekts das ausgeführt werden soll</param>
            public BeforeTestStepEventArgs(string osNumber, string variant, CTestStep testStep)
            {
                /// <summary>
                /// Gibt die für den Test ausgewählte OS-Nummer zurück
                /// </summary>
                this.OsNumber = osNumber;
                this.Variant = variant;
                this.TestStep = testStep;
            }
        }

        /// <summary>
        /// Stellt Daten für das AfterTestStep-Ereignis bereit
        /// </summary>
        public class AfterTestStepEventArgs : EventArgs
        {
            /// <summary>
            /// Gibt die für den Test ausgewählte OS-Nummer zurück
            /// </summary>
            public string OsNumber { get; private set; }
            /// <summary>
            /// Gibt die für den Test ausgewählte Variante zurück
            /// </summary>
            public string Variant { get; private set; }
            /// <summary>
            /// Gibt das Objekt des ausgeführten Testschritt zurück
            /// </summary>
            public CTestStep TestStep { get; private set; }
            /// <summary>
            /// Gibt das Ergebnis des ausgeführten Testschritts zurück
            /// </summary>
            public int TestStepResult { get; private set; }

            /// <summary>
            /// Initialisiert eine neue Instanz der AfterTestStepEventArgs-Klasse
            /// </summary>
            /// <param name="osNumber">Angabe der für den Test gewählten OS-Nummer</param>
            /// <param name="variant">Angabe der für den Test gewählten Variante</param>
            /// <param name="testStep">Angabe des Testschritt-Objekts das ausgeführt wurde</param>
            /// <param name="testStepResult">Angabe des Ergebnisses des ausgeführten Testschritts</param>
            public AfterTestStepEventArgs(string osNumber, string variant, CTestStep testStep, int testStepResult)
            {
                this.OsNumber = osNumber;
                this.Variant = variant;
                this.TestStep = testStep;
                this.TestStepResult = testStepResult;
            }
        }
    }
}
        