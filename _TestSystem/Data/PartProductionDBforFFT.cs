using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Honeywell
{
    namespace Data
    {
        public class CPartProductionDBforFFT : CPartProductionDB
        {
            	/// <summary>
			/// Erstellt Connection zur SQL-Datenbank
			/// </summary>
			/// <param name="ServerName">
			/// Name des Servers auf dem die SQL-Instanz läuft z.B. ".\\SQLEXPRESS"
			/// </param>
			/// <param name="DataBaseName">
			/// Name der Datenbank, zu der die Verbindung aufgebaut werden soll
			/// </param>
			/// <param name="Login">
			/// Username, null wenn mit "Windows Authentication" angemeldet werden soll
			/// </param>
			/// <param name="Password">
			/// Passwort, null wenn mit "Windows Authentication" angemeldet werden soll
			/// </param>
            /// <param name="WriteLineToReportFile">
            /// Delegate auf eine Funktion, die Ausgabe zur Report-Datei ermöglicht. Null - keine Ausgabe zur Reportdatei
            /// </param>
            public CPartProductionDBforFFT(string ServerName, string DataBaseName, string Login = null, string Password = null, CDbSql.WriteLineToReportFileHandle WriteLineToReportFile = null)
                : base(ServerName, DataBaseName, Login, Password, WriteLineToReportFile)
			{
                this.MeasurmentList = new List<CMeasurment>(1000);
                this.NoteList = new List<CNote>(1000);
                this.DutNumberList = new List<CDutNumber>(100);               
			}

            //-------------------------------------------------------------------------------------------
            /// <summary>
            /// Wird vor dem die TestSteps-Initialisation aufgerufen
            /// </summary>
            /// <param name="ConfigurationParameterSW"></param>
            /// <param name="ConfigurationParameterBatch">
            /// In der ConfigurationParameterBatch.OrderNameCurrent wird der Aufragsname des aktuellen Batches zurückgegeben
            /// </param>
            /// <returns>Ergebnis</returns>
            public bool SetConfigurationFFT(CConfigurationParameterFFTSW ConfigurationParameterSW, CConfigurationParameterFFTBatch ConfigurationParameterBatch)
            {
                this.GetErrorParameter = ConfigurationParameterSW.GetErrorParameter;
                this.GetTestStepParameter = ConfigurationParameterSW.GetTestStepParameter;

                if (!this.SetPreConfigurationSW(ConfigurationParameterSW))
                    return false;

                if (!this.SetPreCofigurationBatch(ConfigurationParameterBatch.OSNumber, ConfigurationParameterBatch.DescriptionOSNumber, ConfigurationParameterBatch.OrderName, ref ConfigurationParameterBatch.OrderNameCurrent))
                    return (false);

                return true;
            }

            public bool WriteToDBBeforeFFT()
            {
                long iProductionStepId;

                //Löschen vorherige ergebnisse //////////
                this.Clear();
                /////////////////////////////////////////

                if (!this.DbSql.SetTransaction())
                {
                    this.Error = this.DbSql.Error;
                    return false;
                }

                CPartProductionDB.tab_ProductionStep Tab_ProductionStep = new CPartProductionDB.tab_ProductionStep();
                Tab_ProductionStep.PartNumber = this.Tab_Part.PartNumber;
                Tab_ProductionStep.ProductionSystemId = this.Tab_ProductionSystem.ProductionSystemId;
                Tab_ProductionStep.SoftwareVersionId = this.Tab_SoftwareVersion.SoftwareVersionId;
                this.Tab_ProductionStep.Date = System.DateTime.Now;
                Tab_ProductionStep.Date = this.Tab_ProductionStep.Date;
                Tab_ProductionStep.Result = "run";
                iProductionStepId = this.Tab_ProductionStep.SetProductionStep(Tab_ProductionStep);
                this.Tab_ProductionStep.ProductionStepId = iProductionStepId;

                if (iProductionStepId < 1)
                {
                    if (!this.DbSql.Rollback())
                    {
                        this.Error = this.DbSql.Error;
                        this.DbSql.Close();
                        return false;
                    }
                    this.DbSql.Close();
                    return false;
                }
                if (!this.DbSql.Commit())
                {
                    this.Error = this.DbSql.Error;
                    this.DbSql.Close();
                    return false;
                }

                if (!this.DbSql.Close())
                {
                    this.Error = this.DbSql.Error;
                    return false;
                }

                return true;
            }
            public bool WriteToDBAfterFFT(bool TestResult, int FftMode)
            {
                string[] strErrorDescriptionArray, strStepDescriptionArray, strMeasUnitDescriptionArray;
                string strBuffer;

                bool bResultLoop = true;
                bool bResult = true;

                this.Tab_ProductionStep.Mode = FftMode;

                if (!this.DbSql.SetTransaction())
                {
                    this.Error = this.DbSql.Error;
                    return false;
                }

                while (true)
                {

                    ////////////////////////////////////////////////////////////////////////////////////////
                    #region//update tab_Measurment
                    if (this.TestStepDictionary != null && this.ErrorDictionary != null && this.MeasUnitDictionary != null)
                    {
                        string strMeasUnitKey;
                        tab_Measurment tab_MeasurmentTemp;                        

                        bResultLoop = true;
                        foreach (CMeasurment measurment in this.MeasurmentList)
                        {
                            tab_MeasurmentTemp = new tab_Measurment();

                            tab_MeasurmentTemp.ProductionStepId = this.Tab_ProductionStep.ProductionStepId;
                            tab_MeasurmentTemp.ProductionSystemId = this.Tab_ProductionSystem.ProductionSystemId;
                            tab_MeasurmentTemp.PartNumber = this.Tab_Part.PartNumber;
                            tab_MeasurmentTemp.OrderId = this.Tab_ProductionStep.OrderId;
                            tab_MeasurmentTemp.Mode = this.Tab_ProductionStep.Mode;
                            if (!this.ErrorDictionary.ContainsKey(measurment.ErrorCodeString))
                            {
                                tab_Error tab_ErrorTemp = new tab_Error();
                                if (!this.TestStepDictionary.ContainsKey(measurment.TestStepCode))
                                {
                                    tab_TestStep tab_TestStepTemp = new tab_TestStep();
                                    tab_TestStepTemp.SoftwareId = this.Tab_Software.ParentSoftwareId;
                                    tab_TestStepTemp.Code = measurment.TestStepCode;
                                    tab_TestStepTemp.Name = measurment.TestStepCodeString;
                                    tab_TestStepTemp.Description = measurment.TestStepDescription;
                                    if (this.Tab_TestStep.SetTestStep(tab_TestStepTemp) < 1)
                                    {
                                        bResultLoop = false;
                                        break;
                                    }
                                    strStepDescriptionArray = new string[3];
                                    strStepDescriptionArray[0] = string.Format("{0}", tab_TestStepTemp.TestStepId ?? 0);
                                    strStepDescriptionArray[1] = (string)(tab_TestStepTemp.Name);
                                    strStepDescriptionArray[2] = (string)(tab_TestStepTemp.Description);

                                    this.TestStepDictionary.Add(tab_TestStepTemp.Code ?? 0, strStepDescriptionArray);
                                }
                                tab_ErrorTemp.TestStepId = Convert.ToInt32(this.TestStepDictionary[measurment.TestStepCode][0]);
                                tab_ErrorTemp.Code = measurment.ErrorCode;
                                tab_ErrorTemp.Name = measurment.ErrorDescription;
                                tab_ErrorTemp.DescriptionOfMeasurement = measurment.ErrorDescriptionOfMeasurement;
                                if (this.Tab_Error.SetError(tab_ErrorTemp) < 1)
                                {
                                    bResultLoop = false;
                                    break;
                                }
                                
                                strErrorDescriptionArray = new string[2];
                                strErrorDescriptionArray[0] = string.Format("{0}", tab_ErrorTemp.ErrorId ?? 0);
                                strErrorDescriptionArray[1] = tab_ErrorTemp.Name;
                                strBuffer = string.Format("{0}:{1}", measurment.TestStepCode, tab_ErrorTemp.Code);
                                this.ErrorDictionary.Add(strBuffer, strErrorDescriptionArray);
                            }
                            tab_MeasurmentTemp.ErrorId = Convert.ToInt32(this.ErrorDictionary[measurment.ErrorCodeString][0]);
                            tab_MeasurmentTemp.Date = measurment.Date;
                            tab_MeasurmentTemp.Name = measurment.Name;
                            tab_MeasurmentTemp.Value = measurment.Value;
                            tab_MeasurmentTemp.ValueLSL = measurment.LSL;
                            tab_MeasurmentTemp.ValueUSL = measurment.USL;
                            tab_MeasurmentTemp.Multiplier = measurment.Multiplier;
                            if (measurment.MeasUnitName == null && measurment.MeasUnitType != null)
                                measurment.MeasUnitName = "_";//Sonst keine Auswertung in CEvaluationManager.ExportToExcel
                            if (measurment.MeasUnitName != null && measurment.MeasUnitType != null)
                            {
                                if (measurment.MeasUnitName.Trim() == string.Empty)
                                    measurment.MeasUnitName = "_";
                                strMeasUnitKey = string.Format("{0}:{1}", measurment.MeasUnitName, measurment.MeasUnitType);
                                if (!this.MeasUnitDictionary.ContainsKey(strMeasUnitKey.ToLower()))
                                {
                                    tab_MeasUnit tab_MeasUnitTemp = new tab_MeasUnit();
                                    tab_MeasUnitTemp.Name = measurment.MeasUnitName;
                                    tab_MeasUnitTemp.Type = measurment.MeasUnitType;
                                    if (this.Tab_MeasUnit.SetMeasUnit(tab_MeasUnitTemp) < 1)
                                    {
                                        bResultLoop = false;
                                        break;
                                    }

                                    strMeasUnitDescriptionArray = new string[2];
                                    strMeasUnitDescriptionArray[0] = string.Format("{0}", tab_MeasUnitTemp.MeasUnitId);
                                    strMeasUnitDescriptionArray[1] = tab_MeasUnitTemp.Description;

                                    strBuffer = string.Format("{0}:{1}", tab_MeasUnitTemp.Name, tab_MeasUnitTemp.Type);
                                    this.MeasUnitDictionary.Add(strBuffer.ToLower(), strMeasUnitDescriptionArray);
                                }
                                tab_MeasurmentTemp.MeasUnitId = Convert.ToInt32(this.MeasUnitDictionary[strMeasUnitKey.ToLower()][0]);
                            }
                            tab_MeasurmentTemp.Result = measurment.Result.ToString().ToLower();

                            if (!this.Tab_Measurment.SetMeasurment(tab_MeasurmentTemp))
                            {
                                bResultLoop = false;
                                break;
                            }
                        }
                        if (!bResultLoop)
                        {
                            bResult = false;
                            break;
                        }
                    }
                    #endregion
                    ////////////////////////////////////////////////////////////////////////////////////////

                    ////////////////////////////////////////////////////////////////////////////////////////
                    #region//update tab_Note
                    if (this.TestStepDictionary != null && this.MeasUnitDictionary != null)
                    {
                        bResultLoop = true;
                        tab_Note tab_NoteTemp;
                        foreach (CNote note in this.NoteList)
                        {
                            tab_NoteTemp = new tab_Note();
                            tab_NoteTemp.ProductionStepId = this.Tab_ProductionStep.ProductionStepId;
                            if (!this.TestStepDictionary.ContainsKey(note.TestStepId))
                            {
                                tab_TestStep tab_TestStepTemp = new tab_TestStep();
                                tab_TestStepTemp.SoftwareId = this.Tab_Software.ParentSoftwareId;
                                tab_TestStepTemp.Code = note.TestStepId;
                                tab_TestStepTemp.Name = note.TestStepIdString;
                                tab_TestStepTemp.Description = note.TestStepDescription;
                                if (this.Tab_TestStep.SetTestStep(tab_TestStepTemp) < 1)
                                {
                                    bResultLoop = false;
                                    break;
                                }
                                strStepDescriptionArray = new string[3];
                                strStepDescriptionArray[0] = string.Format("{0}", tab_TestStepTemp.TestStepId ?? 0);
                                strStepDescriptionArray[1] = (string)(tab_TestStepTemp.Name);
                                strStepDescriptionArray[2] = (string)(tab_TestStepTemp.Description);

                                this.TestStepDictionary.Add(tab_TestStepTemp.Code ?? 0, strStepDescriptionArray);
                            }
                            tab_NoteTemp.TestStepId = Convert.ToInt32(this.TestStepDictionary[note.TestStepId][0]);
                            tab_NoteTemp.Date = note.Date;
                            tab_NoteTemp.Name = note.Name;
                            tab_NoteTemp.Value = note.Value;
                            if (!this.Tab_Note.SetNote(tab_NoteTemp))
                            {
                                bResultLoop = false;
                                break;
                            }
                        }
                        if (!bResultLoop)
                        {
                            bResult = false;
                            break;
                        }
                    }
                    #endregion
                    ////////////////////////////////////////////////////////////////////////////////////////

                    ////////////////////////////////////////////////////////////////////////////////////////
                    #region//update tab_DutNumber
                    if (this.DutNumberList.Count != 0)
                    {
                        bResultLoop = true;
                        foreach (CDutNumber dutNumber in this.DutNumberList)
                        {
                            if (!this.Tab_DutNumber.SetDutNumber(this.Tab_ProductionStep.ProductionStepId ?? 0, dutNumber.Number, dutNumber.NumberType))
                            {
                                bResultLoop = false;
                                break;
                            }
                        }
                        if (!bResultLoop)
                        {
                            bResult = false;
                            break;
                        }
                    }
                    #endregion
                    ////////////////////////////////////////////////////////////////////////////////////////

                    ////////////////////////////////////////////////////////////////////////////////////////
                    #region//update tab_ProduktionStep
                    CPartProductionDB.tab_ProductionStep Tab_ProductionStep = new CPartProductionDB.tab_ProductionStep();

                    Tab_ProductionStep.OrderId = this.Tab_ProductionStep.OrderId;
                    Tab_ProductionStep.Mode = this.Tab_ProductionStep.Mode;
                    Tab_ProductionStep.ProductionStepId = this.Tab_ProductionStep.ProductionStepId;
                    Tab_ProductionStep.Result = TestResult.ToString().ToLower();
                    TimeSpan duration = System.DateTime.Now - (this.Tab_ProductionStep.Date ?? System.DateTime.Now);
                    Tab_ProductionStep.Duration = Math.Round(duration.TotalSeconds, 1);
                    if (this.FirstError != null)
                        Tab_ProductionStep.FirstErrorId = Convert.ToInt32(this.ErrorDictionary[this.FirstError][0]);
                    if (!this.Tab_ProductionStep.UpdateProductionStep(Tab_ProductionStep))
                    {
                        bResult = false;
                        break;
                    }
                    #endregion
                    ////////////////////////////////////////////////////////////////////////////////////////

                    break;
                }

                if (bResult)
                {
                    if (!this.DbSql.Commit())
                    {
                        this.Error = this.DbSql.Error;
                        this.DbSql.Close();
                        return false;
                    }
                }
                else
                {
                    if (!this.DbSql.Rollback())
                    {
                        this.Error = this.DbSql.Error;
                        this.DbSql.Close();
                        return false;
                    }
                }
                if (!this.DbSql.Close())
                {
                    this.Error = this.DbSql.Error;
                    return false;
                }

                return bResult;
            }

            /// <summary>
            /// Gibt Descrioption von einem Error, das zu einem Testschritt mit TesStepId und ErrorCode
            /// </summary>
            public GetErrorParameterHandle GetErrorParameter = null;
            /// <summary>
            /// Gibt Name und Description vom TestStep mit der bestimmten TesStepId
            /// </summary>
            public GetTestStepParameterHandle GetTestStepParameter = null;

            //-------------------------------------------------------------------------------------------           
            /// <summary>
            /// Liste von den allen Messungen, die in einem Testlauf enstanden sind. (für DB)
            /// </summary>
            public List<CMeasurment> MeasurmentList;
            /// <summary>
            /// Liste von den allen Notes, die in einem Testlauf enstanden sind. (für DB)
            /// </summary>
            public List<CNote> NoteList;
            /// <summary>
            /// Liste von den allen Dut-Numbers, die in einem Testlauf enstanden sind. (für DB)
            /// </summary>
            public List<CDutNumber> DutNumberList;
            /// <summary>
            /// FirstError während einem fehlerhaften Testlauf. (für DB)
            /// </summary>
            public string FirstError;

            /// <summary>
            /// string - ErrorCodeString z.B. 1:2
            /// [0] - ErrorId
            /// [1] - Name
            /// </summary>
            public Dictionary<string, string[]> ErrorDictionary = null;
            /// <summary>
            /// int - TestStepCode
            /// [0] - TestStepId
            /// [1] - Name
            /// [2] - Description
            /// </summary>
            public Dictionary<int, string[]> TestStepDictionary = null;
            /// <summary>
            /// string - Name:Type z.B mA:double2
            /// [0] - MeasUnitId
            /// [1] - Description
            /// </summary>
            public Dictionary<string, string[]> MeasUnitDictionary = null;

            //-------------------------------------------------------------------------------------------
            public bool CreateErrorDictionaryFromPartProductionDB(int SoftwareId)
            {
                string strCommand, strTableError, strVariableListError, strValueListError;
                string strTableTStep, strVariableListTStep, strValueListTStep;
                int iRecordsAffected;

                tab_Error tab_ErrorTemp = new tab_Error();
                tab_ErrorTemp.ErrorId = 0;//Soll ausgelesen werden
                tab_ErrorTemp.TestStepId = 0;//Soll ausgelesen werden
                tab_ErrorTemp.Code = 0;//Soll ausgelesen werden
                tab_ErrorTemp.Name = "";//Soll ausgelesen werden
                this.DbSql.GetParameterDBfromObject(tab_ErrorTemp, out strTableError, out strVariableListError, out strValueListError, CDbSql.KindSQLCommand.Select);

                tab_TestStep tab_TestStepTemp = new tab_TestStep();
                tab_TestStepTemp.SoftwareId = 0;//Soll ausgelesen werden
                tab_TestStepTemp.Code = 0;//Soll ausgelesen werden
                this.DbSql.GetParameterDBfromObject(tab_TestStepTemp, out strTableTStep, out strVariableListTStep, out strValueListTStep, CDbSql.KindSQLCommand.Select);
                strCommand = string.Format(
                        "SELECT {0}, " +
                        "{1}, " +
                        "[{3}].[Code] AS TStepCode " +
                        "FROM {2} " +
                        "LEFT OUTER JOIN {3} " +
                        "ON [{2}].[TestStepId] = [{3}].[TestStepId] " +
                        "WHERE [{3}].[SoftwareId] = '{4}';",
                        strVariableListError, strVariableListTStep, strTableError, strTableTStep, SoftwareId);
                iRecordsAffected = this.DbSql.SetCommand(strCommand, true, false);

                if (iRecordsAffected < 0)
                {
                    this.Error = this.DbSql.Error;
                    return false;
                }
                else
                {
                    this.ErrorDictionary = new Dictionary<string, string[]>(1000);
                    int iTStepCode, iErrorCode, iErrorId;
                    string strName, strBuffer;
                    if (this.DbSql.Reader.HasRows)
                    {
                        string[] strErrorDescriptionArray;
                        while (this.DbSql.Reader.Read())
                        {
                            strErrorDescriptionArray = new string[2];
                            if (!this.DbSql.Reader.IsDBNull(this.DbSql.Reader.GetOrdinal("Name")))
                                strName = (string)(this.DbSql.Reader["Name"]);
                            else
                                strName = "";

                            iErrorId = (int)(this.DbSql.Reader["ErrorId"]);

                            if (!this.DbSql.Reader.IsDBNull(this.DbSql.Reader.GetOrdinal("TStepCode")))
                                iTStepCode = (int)(this.DbSql.Reader["TStepCode"]);
                            else
                                iTStepCode = 0;
                            if (!this.DbSql.Reader.IsDBNull(this.DbSql.Reader.GetOrdinal("Code")))
                                iErrorCode = (int)(this.DbSql.Reader["Code"]);
                            else
                                iErrorCode = 0;
                            strBuffer = string.Format("{0}:{1}", iTStepCode, iErrorCode);

                            strErrorDescriptionArray[0] = string.Format("{0}", iErrorId);
                            strErrorDescriptionArray[1] = strName;
                            this.ErrorDictionary.Add(strBuffer, strErrorDescriptionArray);
                        }
                    }
                }

                return true;
            }
            /// <summary>
            /// Erstellt ein Dictionary für alle Testschritte in der aktuellen SW
            /// </summary>
            /// <param name="SoftwareId">
            /// SoftwareId
            /// </param>
            /// <returns>
            /// Ergebnis
            /// </returns>
            public bool CreateTestStepDictionaryFromPartProductionDB(int SoftwareId)
            {
                string strCommand, strTable, strVariableList, strValueList;
                int iRecordsAffected;

                tab_TestStep tab_TestStepTemp = new tab_TestStep();
                tab_TestStepTemp.TestStepId = 0;
                tab_TestStepTemp.Code = 0;//Soll ausgelesen werden
                tab_TestStepTemp.Name = "";//Soll ausgelesen werden
                tab_TestStepTemp.Description = "";//Soll ausgelesen werden
                this.DbSql.GetParameterDBfromObject(tab_TestStepTemp, out strTable, out strVariableList, out strValueList, CDbSql.KindSQLCommand.Select);
                strCommand = string.Format(
                        "SELECT {0} " +
                        "FROM {1} " +
                        "WHERE [{1}].[SoftwareId] = '{2}';",
                        strVariableList, strTable, SoftwareId);
                iRecordsAffected = this.DbSql.SetCommand(strCommand, true, false);

                if (iRecordsAffected < 0)
                {
                    this.Error = this.DbSql.Error;
                    return false;
                }
                else
                {
                    this.TestStepDictionary = new Dictionary<int, string[]>(100);
                    if (this.DbSql.Reader.HasRows)
                    {
                        string[] strStepDescriptionArray;
                        int iTestStepCode;
                        while (this.DbSql.Reader.Read())
                        {
                            if (!this.DbSql.Reader.IsDBNull(this.DbSql.Reader.GetOrdinal("Code")))
                                iTestStepCode = (int)(this.DbSql.Reader["Code"]);
                            else
                                iTestStepCode = 0;
                            strStepDescriptionArray = new string[3];
                            strStepDescriptionArray[0] = string.Format("{0}", (int)(this.DbSql.Reader["TestStepId"]));
                            if (!this.DbSql.Reader.IsDBNull(this.DbSql.Reader.GetOrdinal("Name")))
                                strStepDescriptionArray[1] = (string)(this.DbSql.Reader["Name"]);
                            if (!this.DbSql.Reader.IsDBNull(this.DbSql.Reader.GetOrdinal("Description")))
                                strStepDescriptionArray[2] = (string)(this.DbSql.Reader["Description"]);

                            this.TestStepDictionary.Add(iTestStepCode, strStepDescriptionArray);
                        }
                    }
                }

                return true;
            }
            /// <summary>
            /// Erstellt ein MeasUnitDictionary wo, alle in PartProductionDB verwendete Units abgespeichert sind
            /// </summary>
            /// <returns>
            /// Ergebnis
            /// </returns>
            public bool CreateMeasUnitDictionaryOnPartProductionDB()
            {
                string strCommand, strTable, strVariableList, strValueList;
                int iRecordsAffected;

                tab_MeasUnit tab_MeasUnitTemp = new tab_MeasUnit();
                tab_MeasUnitTemp.MeasUnitId = 0;//Soll ausgelesen werden
                tab_MeasUnitTemp.Name = "";//Soll ausgelesen werden
                tab_MeasUnitTemp.Description = "";//Soll ausgelesen werden
                tab_MeasUnitTemp.Type = "";//Soll ausgelesen werden
                this.DbSql.GetParameterDBfromObject(tab_MeasUnitTemp, out strTable, out strVariableList, out strValueList, CDbSql.KindSQLCommand.Select);

                strCommand = string.Format(
                        "SELECT {0} " +
                        "FROM {1};",
                        strVariableList, strTable);
                iRecordsAffected = this.DbSql.SetCommand(strCommand, true, false);

                if (iRecordsAffected < 0)
                {
                    this.Error = this.DbSql.Error;
                    return false;
                }
                else
                {
                    this.MeasUnitDictionary = new Dictionary<string, string[]>(1000);
                    string strName, strDescription, strType, strBuffer;
                    int iMeasUnitId;
                    if (this.DbSql.Reader.HasRows)
                    {
                        string[] strMeasUnitDescriptionArray;
                        while (this.DbSql.Reader.Read())
                        {
                            strMeasUnitDescriptionArray = new string[2];
                            if (!this.DbSql.Reader.IsDBNull(this.DbSql.Reader.GetOrdinal("Name")))
                                strName = (string)(this.DbSql.Reader["Name"]);
                            else
                                strName = "";
                            if (!this.DbSql.Reader.IsDBNull(this.DbSql.Reader.GetOrdinal("Description")))
                                strDescription = (string)(this.DbSql.Reader["Description"]);
                            else
                                strDescription = "";
                            if (!this.DbSql.Reader.IsDBNull(this.DbSql.Reader.GetOrdinal("Type")))
                                strType = (string)(this.DbSql.Reader["Type"]);
                            else
                                strType = "";
                            iMeasUnitId = (int)(this.DbSql.Reader["MeasUnitId"]);

                            strBuffer = string.Format("{0}:{1}", strName, strType);

                            strMeasUnitDescriptionArray[0] = string.Format("{0}", iMeasUnitId);
                            strMeasUnitDescriptionArray[1] = strDescription;
                            this.MeasUnitDictionary.Add(strBuffer.ToLower(), strMeasUnitDescriptionArray);
                        }
                    }
                }

                return true;
            }
            
            //-------------------------------------------------------------------------------------------
            public class CConfigurationParameterFFTSW
            {
                private CConfigurationParameterFFTSW() { }
                public CConfigurationParameterFFTSW(string NameSW, string VersionSW, string DescriptionFFTSoftware, string SoftwareCategory, string DescriptionSoftwareCategory, string Department, string SiteCode, int FFTMode, GetErrorParameterHandle GetErrorParameter = null, GetTestStepParameterHandle GetTestStepParameter = null)
                {
                    this.NameSW = NameSW;
                    this.VersionSW = VersionSW;
                    this.DescriptionFFTSoftware = DescriptionFFTSoftware;
                    this.SoftwareCategory = SoftwareCategory;
                    this.DescriptionSoftwareCategory = DescriptionSoftwareCategory;
                    this.Department = Department;
                    this.SiteCode = SiteCode;
                    this.FFTMode = FFTMode;

                    this.GetErrorParameter = GetErrorParameter;
                    this.GetTestStepParameter = GetTestStepParameter;
                }
                public string NameSW;
                public string VersionSW;
                public string DescriptionFFTSoftware;
                public string SoftwareCategory;
                public string DescriptionSoftwareCategory;
                public string Department;
                public string SiteCode;
                public int FFTMode;
                public GetErrorParameterHandle GetErrorParameter = null;
                public GetTestStepParameterHandle GetTestStepParameter = null;
            }
            public class CConfigurationParameterFFTBatch
            {
                private CConfigurationParameterFFTBatch() { }
                public CConfigurationParameterFFTBatch(string OSNumber, string DescriptionOSNumber, string OrderName)
                {
                    this.OSNumber = OSNumber;
                    this.DescriptionOSNumber = DescriptionOSNumber;
                    this.OrderName = OrderName;
                }
                public string OSNumber;
                public string DescriptionOSNumber;
                public string OrderName;
                public string OrderNameCurrent = null;
            }
            public class CMeasurment
            {
                private CMeasurment()
                {
                }
                public CMeasurment(int TestStepCode, int ErrorCode, string Name)
                {
                    this.TestStepCode = TestStepCode;
                    this.TestStepCodeString = "";
                    this.TestStepDescription = "";
                    this.ErrorCode = ErrorCode;
                    this.ErrorCodeString = string.Format("{0}:{1}", TestStepCode, ErrorCode);
                    this.ErrorDescription = null;
                    this.Date = DateTime.Now;
                    this.Name = Name;
                    this.Value = null;
                    this.LSL = null;
                    this.USL = null;
                    this.Multiplier = null;
                    this.MeasUnitName = null;
                    this.MeasUnitType = null;
                    this.ErrorDescriptionOfMeasurement = null;
                    this.Result = true;

                }

                public int TestStepCode;
                public string TestStepCodeString;
                public string TestStepDescription;
                public int ErrorCode;
                public string ErrorDescription;
                public string ErrorCodeString;
                public DateTime Date;
                public string Name;
                public string Value;
                public string LSL;
                public string USL;
                public double? Multiplier;
                public string MeasUnitName;
                public string MeasUnitType;
                public string ErrorDescriptionOfMeasurement;
                public bool Result;
            }
            public class CNote
            {
                private CNote()
                {
                }
                public CNote(int TestStepId, string Name, string Value)
                {
                    this.TestStepId = TestStepId;
                    this.TestStepIdString = "";
                    this.TestStepDescription = "";

                    this.Date = DateTime.Now;
                    this.Name = Name;
                    this.Value = Value;
                }

                public int TestStepId;
                public string TestStepIdString;
                public string TestStepDescription;

                public DateTime Date;
                public string Name;
                public string Value;
            }
            public class CDutNumber
            {
                private CDutNumber()
                {
                }
                public CDutNumber(string Number, string NumberType)
                {
                    this.Date = DateTime.Now;
                    this.Number = Number;
                    this.NumberType = NumberType;
                }

                public DateTime Date;
                public string Number;
                public string NumberType;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="TesStepId"></param>
            /// <param name="Name"></param>
            /// <param name="Description"></param>
            /// <returns>
            /// "" - Keine Error
            /// sonst Fehlermeldung
            /// </returns>
            public delegate string GetTestStepParameterHandle(int TesStepId, out string Name, out string Description);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="TesStepId"></param>
            /// <param name="ErrorCode"></param>
            /// <param name="Description"></param>
            /// <returns>
            /// "" - Keine Error
            /// sonst Fehlermeldung
            /// </returns>
            public delegate string GetErrorParameterHandle(int TesStepId, int ErrorCode, out string Description);


            //-------------------------------------------------------------------------------------------
            /// <summary>
            /// Wird von SetConfiguration aufgerufen
            /// </summary>
            /// <param name="PreConfigurationParameter"></param>
            /// <returns>Ergebnis</returns>
            protected bool SetPreConfigurationSW(CConfigurationParameterFFTSW PreConfigurationParameter)
            {

                string strError = "";
                while (true)
                {
                    if (!this.DbSql.SetTransaction())
                    {
                        strError = this.DbSql.Error;
                        break;
                    }

                    //Prüfen, ob die benötigte Abteilung in der DB ist ///////////////////////////////
                    CPartProductionDB.tab_Department PropertyDepartment;
                    PropertyDepartment = new CPartProductionDB.tab_Department();
                    PropertyDepartment.Name = PreConfigurationParameter.Department;
                    PropertyDepartment.SiteCode = PreConfigurationParameter.SiteCode;
                    this.Tab_Department.DepartmentId = this.Tab_Department.CheckDepartment(PropertyDepartment);
                    if (this.Tab_Department.DepartmentId <= 0)
                    {
                        strError = this.Error;
                        break;
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////

                    //Prüfen, ob die ProductionCategory der SW ist in der Datenkbank vorhanden, wenn nicht wird die ProductionCategory automatisch neu angelegt				
                    this.Tab_ProductionCategory.ProductionCategoryId = this.Tab_ProductionCategory.CheckProductionCategory(PreConfigurationParameter.SoftwareCategory, PreConfigurationParameter.DescriptionSoftwareCategory);
                    if (this.Tab_ProductionCategory.ProductionCategoryId <= 0)
                    {
                        strError = this.Error;
                        break;
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////

                    //Prüft ob die Nummer zur Verwahltung von allen Nummern, die zur Produktionssystem gehören, angelegt ist
                    this.Tab_ProductionSystemNumber.NumberTypeId = this.Tab_NumberType.CheckNumberType("ProductionSystemNumber", "manage the listing of all production system number");
                    if (this.Tab_ProductionSystemNumber.ProductionSystemNumberId <= 0)
                    {
                        strError = this.Error;
                        break;
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////

                    //Prüfen, ob die SW und ihre Versionen in der DB angelegt sind. /////////////////////////							
                    CPartProductionDB.tab_Software propertySW;
                    propertySW = new CPartProductionDB.tab_Software();
                    propertySW.ParentSoftwareId = null;
                    propertySW.ProductionCategoryId = this.Tab_ProductionCategory.ProductionCategoryId;
                    propertySW.Name = System.IO.Path.GetFileName(PreConfigurationParameter.NameSW).ToLower();
                    propertySW.Path = System.IO.Path.GetDirectoryName(PreConfigurationParameter.NameSW).ToLower();
                    propertySW.Description = PreConfigurationParameter.DescriptionFFTSoftware;
                    this.Tab_SoftwareVersion.SoftwareVersionId = this.Tab_Software.CheckProductionSW(propertySW, PreConfigurationParameter.VersionSW);
                    if (this.Tab_SoftwareVersion.SoftwareVersionId <= 0)
                    {
                        strError = this.Error;
                        break;
                    }
                    this.Tab_Software.SoftwareId = propertySW.SoftwareId ?? 0;
                    if (propertySW.ParentSoftwareId == null)
                        this.Tab_Software.ParentSoftwareId = this.Tab_Software.SoftwareId;
                    else
                        this.Tab_Software.ParentSoftwareId = propertySW.ParentSoftwareId ?? 0;
                    //////////////////////////////////////////////////////////////////////////////////////

                    //Prüfen, ob das Production System vorhanden ist ////////////////////////////////////
                    int iProductionSystemId;
                    CPartProductionDB.tab_ProductionSystem propertyProductionSystem = new CPartProductionDB.tab_ProductionSystem();
                    propertyProductionSystem.SoftwareId = this.Tab_Software.SoftwareId;
                    propertyProductionSystem.DepartmentId = this.Tab_Department.DepartmentId;
                    iProductionSystemId = this.Tab_ProductionSystem.GetProductionSystemId(null, this.Tab_Software.SoftwareId, null, this.Tab_Department.DepartmentId);
                    if (iProductionSystemId < 0)
                    {
                        strError = this.Error;
                        break;
                    }
                    if (iProductionSystemId == 0)
                    {
                        iProductionSystemId = this.Tab_ProductionSystem.SetProductionSystem(propertyProductionSystem);
                        if (iProductionSystemId < 1)
                        {
                            strError = this.Error;
                            break;
                        }
                    }
                    this.Tab_ProductionSystem.ProductionSystemId = iProductionSystemId;
                    //////////////////////////////////////////////////////////////////////////////////////

                                       

                    this.Tab_ProductionStep.Mode = PreConfigurationParameter.FFTMode;

                    break;
                }

                if (strError.Length == 0)
                {
                    if (!this.DbSql.Commit())
                        strError = this.DbSql.Error;
                }
                else
                {
                    if (!this.DbSql.Rollback())
                        strError = this.DbSql.Error;
                }
                this.DbSql.Close();

                if (strError.Length != 0)
                {
                    this.Error = strError;
                    return false;
                }
                return true;
            }
            /// <summary>
            /// Wird von SetConfiguration aufgerufen
            /// </summary>
            /// <param name="PreConfigurationParameter"></param>
            /// <returns>Ergebnis</returns>
            protected bool SetPreCofigurationBatch(string OSNumber, string DescriptionOSNumber, string OrderName, ref string OrderNameCurrent)
            {
                int iResult;
                int id, iCode;
                string strNameTestStep, strDescription, strError;
                bool bFlagUpdate, bResultLoop;


                bool bResult = true;
                while (true)
                {

                    //////////////////////////////////////////////////////////////////
                    #region//Create production order
                    OrderNameCurrent = null;
                    this.Tab_ProductionStep.OrderId = null;
                    if (OrderName.Length != 0)
                    {
                        System.Int64 iOrderId;
                        string strLotSize, strTitle = "";
                        CPartProductionDB.tab_Order tabOrder;

                        #region //Feststellen, ob der Auftrag vorhanden ist und was mit ihm passieren soll (neuanlegen mit fortlaufender Nummer / ändern)
                        if (!this.Tab_Order.GetOrderLastLike(OrderName + "_§§", out tabOrder))
                        {
                            bResult = false;
                            break;
                        }
                        if (tabOrder != null)//Auftrag schon vorhanden
                        {
                            string[] strArray;
                            if (this.Tab_ProductionStep.Mode != 0)//Nicht Produktions Modes
                            {
                                System.Windows.Forms.DialogResult result;
                                //TODO GO MessageBox.Show
                                result = System.Windows.Forms.MessageBox.Show(
                                            string.Format("Der Auftrag '{0}' ist schon vorhanden.\r\nWollen Sie den neuen Auftrag anlegen?\r\n\r\nGeben Sie die Auftraggröße ein", OrderName),
                                            "Auftrag vorhanden", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Asterisk);
                                if (result == System.Windows.Forms.DialogResult.Yes)//Auftrag neu anlegen
                                {
                                    tabOrder.OrderId = null;
                                    strArray = tabOrder.Name.Split(new string[] { "_§§_" }, System.StringSplitOptions.RemoveEmptyEntries);
                                    if (strArray.Length == 1)
                                    {
                                        tabOrder.Name += "_§§_1";
                                    }
                                    else
                                    {
                                        tabOrder.Name = strArray[0] + string.Format("_§§_{0}", System.Convert.ToInt32(strArray[1]) + 1);
                                    }
                                    strTitle = "Auftrag '{0}' anlegen";
                                }
                                else if (result == System.Windows.Forms.DialogResult.No)//Auftrag updaten
                                {
                                    this.Tab_ProductionStep.OrderId = tabOrder.OrderId;
                                    strTitle = "Auftrag '{0}' ändern";
                                }
                                else
                                {
                                    tabOrder = null;//Die Testläufe werden ohne Auftragnummer verwaltet
                                }
                            }
                            else//Fertigungsmode
                            {
                                tabOrder.OrderId = null;
                                strArray = tabOrder.Name.Split(new string[] { "_§§_" }, System.StringSplitOptions.RemoveEmptyEntries);
                                if (strArray.Length == 1)
                                {
                                    tabOrder.Name += "_§§_1";
                                }
                                else
                                {
                                    tabOrder.Name = strArray[0] + string.Format("_§§_{0}", System.Convert.ToInt32(strArray[1]) + 1);
                                }
                                tabOrder.LotSize = 0;
                            }
                        }
                        else //Auftrag nicht vorhanden
                        {
                            tabOrder = new CPartProductionDB.tab_Order();
                            tabOrder.Name = OrderName;
                            if (this.Tab_ProductionStep.Mode != 0)//Nicht ProduktionsModes
                            {
                                strTitle = "Auftrag '{0}' anlegen";
                            }
                            else//Fertigungsmode
                            {
                                tabOrder.LotSize = 0;
                            }
                        }
                        #endregion

                        #region//Auftrag neu anlegen bzw. ändern (+ LotSize festlegen)
                        if (tabOrder != null)
                        {
                            OrderNameCurrent = tabOrder.Name;
                            if (this.Tab_ProductionStep.Mode != 0)//Nicht Fertigungsmode
                            {
                                int iLotSize = 0;
                                iLotSize = tabOrder.LotSize ?? 0;
                                strLotSize = Microsoft.VisualBasic.Interaction.InputBox("Geben Sie die Auftraggröße ein",
                                                                    string.Format(strTitle, tabOrder.Name), string.Format("{0}", iLotSize));
                                if (strLotSize.Length != 0)
                                {
                                    tabOrder.LotSize = System.Convert.ToInt32(strLotSize);
                                }
                                else
                                {
                                    this.Tab_ProductionStep.OrderId = null;
                                    tabOrder = null;//Ohne Auftrag wird getestet
                                }
                            }
                            if (tabOrder != null)
                            {
                                if (this.Tab_ProductionStep.OrderId == null)//Auftrag muss angelegt werden
                                {
                                    iOrderId = this.Tab_Order.SetOrder(tabOrder);
                                    if (iOrderId < 1)
                                    {
                                        bResult = false;
                                        break;
                                    }
                                    this.Tab_ProductionStep.OrderId = iOrderId;
                                }
                                else
                                {
                                    if (!this.Tab_Order.UpdateOrder(tabOrder))
                                    {
                                        bResult = false;
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion
                    //////////////////////////////////////////////////////////////////

                    if (!this.DbSql.SetTransaction())
                    {
                        this.Error = this.DbSql.Error;
                        bResult = false;
                        break;
                    }

                    if (!this.CreateTestStepDictionaryFromPartProductionDB(this.Tab_Software.ParentSoftwareId ?? 0))
                    {
                        bResult = false;
                        break;
                    }
                    #region//Überprüfen ob in DB aktuelle TestStep-Beschreibung ist (Wenn nicht update)
                    if (this.GetTestStepParameter != null)
                    {
                        bResultLoop = true;
                        foreach (System.Collections.Generic.KeyValuePair<int, string[]> hItem in this.TestStepDictionary)
                        {
                            id = hItem.Key;
                            strError = this.GetTestStepParameter(id, out strNameTestStep, out strDescription);
                            if (strError.Length != 0)
                            {
                                bResultLoop = false;
                                this.Error = strError;
                                break;
                            }                      
                            bFlagUpdate = false;
                            CPartProductionDB.tab_TestStep tabTestStep = new CPartProductionDB.tab_TestStep();
                            tabTestStep.TestStepId = System.Convert.ToInt32(hItem.Value[0]);
                            if (strNameTestStep != hItem.Value[1])
                            {
                                tabTestStep.Name = strNameTestStep;
                                bFlagUpdate = true;
                            }
                            if (strDescription != hItem.Value[2])
                            {
                                tabTestStep.Description = strDescription;
                                bFlagUpdate = true;
                            }
                            if (bFlagUpdate)
                            {
                                if (!this.Tab_TestStep.UpdateTestStep(tabTestStep))
                                {
                                    bResultLoop = false;
                                    break;
                                }
                            }
                        }
                        if (!bResultLoop)
                        {
                            bResult = false;
                            break;
                        }
                    }
                    #endregion

                    if (!this.CreateErrorDictionaryFromPartProductionDB(this.Tab_Software.ParentSoftwareId ?? 0))
                    {
                        bResult = false;
                        break;
                    }
                    #region//Überprüfen ob in DB aktuelle Error-Beschreibung ist (Wenn nicht update)
                    if (this.GetErrorParameter != null)
                    {
                        bResultLoop = true;
                        foreach (System.Collections.Generic.KeyValuePair<string, string[]> hItem in this.ErrorDictionary)
                        {
                            string[] strErrorPropertyArray;
                            strErrorPropertyArray = hItem.Key.Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
                            id = System.Convert.ToInt32(strErrorPropertyArray[0]);
                            iCode = System.Convert.ToInt32(strErrorPropertyArray[1]);
                            strError = this.GetErrorParameter(id, iCode, out strDescription);
                            if (strError.Length != 0)
                            {
                                bResultLoop = false;
                                this.Error = strError;
                                break;
                            }                           
                            bFlagUpdate = false;
                            CPartProductionDB.tab_Error tabError = new CPartProductionDB.tab_Error();
                            tabError.ErrorId = System.Convert.ToInt32(hItem.Value[0]);
                            if (strDescription != hItem.Value[1])
                            {
                                tabError.Name = strDescription;
                                bFlagUpdate = true;
                            }
                            if (bFlagUpdate)
                            {
                                if (!this.Tab_Error.UpdateError(tabError))
                                {
                                    bResultLoop = false;
                                    break;
                                }
                            }
                        }
                        if (!bResultLoop)
                        {
                            bResult = false;
                            break;
                        }
                    }
                    #endregion

                    if (!this.CreateMeasUnitDictionaryOnPartProductionDB())
                    {
                        bResult = false;
                        break;
                    }

                    this.Tab_Part.PartNumber = OSNumber;
                    iResult = this.Tab_Part.IsPartInDB(this.Tab_Part.PartNumber, out strDescription);
                    if (iResult == -1)
                    {
                        bResult = false;
                        break;
                    }
                    else
                    {
                        if (iResult == 0)
                        {
                            if (!this.Tab_Part.SetPartNumber(this.Tab_Part.PartNumber, DescriptionOSNumber))
                            {
                                bResult = false;
                                break;
                            }
                        }
                    }


                    //Prüfen, ob das Production System mit OS-Number verknüpft ist ///////////////////////
                    int iProductionSystemId = this.Tab_ProductionSystem.ProductionSystemId??0;
                    iResult = this.Tab_PartProduction.IsPartToProductionSystemAllocated(OSNumber, iProductionSystemId);
                    if (iResult < 0)
                    {
                        bResult = false;
                        break;
                    }
                    if (iResult == 0)
                    {
                        if (!this.Tab_PartProduction.DoAllocatePartToProductionSystem(OSNumber, iProductionSystemId))
                        {
                            bResult = false;
                            break;
                        }
                    }
                    //////////////////////////////////////////////////////////////////////////////////////
                    break;
                }
                if (bResult)
                {
                    if (!this.DbSql.Commit())
                        this.Error = this.DbSql.Error;

                }
                else
                {
                    if (!this.DbSql.Rollback())
                        this.Error = this.DbSql.Error;
                }
                this.DbSql.Close();

                return (bResult);
            }
            /// <summary>
            /// Leert alle List z.B. vor dem neuen test
            /// </summary>
            protected void Clear()
            {
                this.MeasurmentList.Clear();
                this.NoteList.Clear();
                this.DutNumberList.Clear();
                this.FirstError = null;
            }

        }
    }
}
