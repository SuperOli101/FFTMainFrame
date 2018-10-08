using System;
using Honeywell.Test;
using Honeywell.Data;


namespace Honeywell
{
    namespace Test
    {
        public class CTestStep : CObjectTest
        {
            public class CErrorTestStep
            {
                public int ID_Error;              
                public int ID_TestStep;
                public String ErrorCode
                {
                    get
                    {
                        return (String.Format("{0}:{1}", ID_TestStep, ID_Error));
                    }                    
                }
                public String ErrorMsg;
                public String ErrorMsgPrint;
				public String ErrorMsgMeasurement;
            }

            protected enum EFlagComparer : int 
            {
                Empty           = 0,//Unit noch nicht gesetzt
                                    //                                                  Kein Error wenn:
                RangeLSL_USL    = 1,//Wenn DBF_Index_LSL und DBF_Index_USL != 0 && > 0  (LSL >= Value <=USL)
                RangeLSL        = 2,//Wenn DBF_Index_LSL != 0 && DBF_Index_USL == 0     (LSL >= Value)
                RangeUSL        = 3,//Wenn DBF_Index_USL != 0 && DBF_Index_LSL == 0     (Value<=USL)
                EqualLSL        = 4,//Wenn DBF_Index_USL == DBF_Index_LSL && >0         (Value == LSL) 
                UnequalLSL      = 5,//Wenn DBF_Index_USL == DBF_Index_LSL && <0         (Value != LSL) 
            }

			/// <summary>
			/// Fügt eine Zeile in der Kopfzeile vom Fehlerreport zum Ausdrucken
			/// </summary>
			/// <param name="Msg">
			/// Zeile im Kopf
			/// </param>
			public void AddHeadLineErrorReportPrint(string Msg)
			{
				this.Test.AddHeadLineErrorReportPrint(Msg);
			}
			/// <summary>
			/// Fügt eine Zeile in der Fusszeile vom Fehlerreport zum Ausdrucken
			/// </summary>
			/// <param name="Msg">
			/// Zeile im Fuss
			/// </param>
			public void AddFootLineErrorReportPrint(string Msg)
			{
				this.Test.AddFootLineErrorReportPrint(Msg);
			}


            /// <summary>
            /// Testschritt wird erstellt
            /// </summary>
	        public CTestStep():base()
            {
	            this.Create();
            }

            /// <summary>
            /// Führt einen Testschritt aus 
            /// </summary>
            /// <returns> 
            /// weniger als "-1" - Error mit Testabbruch; 
            /// "0" - Teststep O.K.;	
            /// mehr als "1" - Error, Test darf weitergemacht werden
            /// </returns>
	        public int Execute()		
            {
	            String strMsg, strNameTestStep, strID_TestStep, strNameFunction;
                int iResult, iCountOfErrorBeforeSetTestStep, iLengthScrollTemp;
                Honeywell.Device.CTimer cTimer = new Device.CTimer();

                this.Test.CurrentTestStep = this;
				strNameFunction = "before _SetTestStep()"; 

                iLengthScrollTemp = this.Data.LengthScroll;
	            if(!this.FlagExecutedFromTest)//Testschritt wurde vom anderen Testschritt aufgerufen
	            {
                    this.WriteToScroll("");
                    CTestStep.FlagLayerOfTestStep++;
                    this.Data.LengthScroll = this.Data.LengthScroll - (CTestStep.FlagLayerOfTestStep * 3);
	            }

                try
                {
                    cTimer.SetTimer(1, Honeywell.Device.CTimer.Unit.Sec);
                    //Testschrittname ausgeben //////////////////////////////////////////////////////////////
                    strID_TestStep = this.GetID(this.ID);
                    strNameTestStep = this.GetName(this.ID);
                    //1		= "--- {0} --- [ {1} ] ({2})"
                    strMsg = String.Format(this.Data.Msg[1], strNameTestStep, this.ID.ToString(), strID_TestStep);
                    this.WriteToScroll(strMsg, 3);
                    ////////////////////////////////////////////////////////////////////////////////////

                    //TestSchritt ausführen ///////////////////////////////////////////////////////////////////////
                    iCountOfErrorBeforeSetTestStep = this.GetError();
					strNameFunction = "_SetTestStep()";
                    this._SetTestStep();				// setzt den Tester für diesen Testschritt	
                    this.Test.FormFFT.SetCurrentTestTime(this.Test.Timer.GetTimer(11));
                    if (!this.CancelTest())				//Wenn ein Fehler aufgetretten ist oder abgebrochen dann kein _ExecuteTestStep
                    {
						if(this.GetError() <= iCountOfErrorBeforeSetTestStep)//Sonst, wenn Teststep zweites mal ausgefürt wird und erstes mal fehler hatte, wird es  zweites mal nicht laufen
						{
							strNameFunction = "_ExecuteTestStep()";
							this._ExecuteTestStep();	//führt TestStep aus, nur wenn keine Fehler oder Abbruch in _SetTestStep
						}
                    }
                    ////////////////////////////////////////////////////////////////////////////////////
                }
                catch (Exception ex)
                {                                
					if(this.NameBreakpointLast.Length == 0)
						strMsg = String.Format("Before the first Breakpoint in {0}", strNameFunction);
					else
						strMsg = String.Format("After {0} in {1}", this.NameBreakpointLast, strNameFunction);
                    this.WriteToScroll(strMsg);
                    //0:998	= "System Error"
                    this.SetResultFault(998,false);
                    this.WriteToScroll(ex.Message);
                    this.WriteLineToReport(ex.StackTrace.ToString());
                   
                }
                finally
                {
                    try
                    {
                        this.Test.FormFFT.SetCurrentTestTime(this.Test.Timer.GetTimer(11));
                        //TestSchritt zurücksetzen ///////////////////////////////////////////////////////////////////////
                        this.Test.SetModeDoNotCheckCancel(true);
                        this._ResetTestStep();				//setzt den Tester auf den Zustand vor diesem Testschritt, wird immer ausgeführt.
                        this.Test.SetModeDoNotCheckCancel(false);
                        ////////////////////////////////////////////////////////////////////////////////////
                    }
                    catch (System.Exception ex)
                    {                        
                        strMsg = String.Format("After {0} in _ResetTestStep()", this.NameBreakpointLast);
                        this.WriteToScroll(strMsg);
                        //0:998	= "System Error"
                        this.SetResultFault(998, false);
                        this.WriteToScroll(ex.Message);
                        this.WriteLineToReport(ex.StackTrace.ToString());                        
                    }
                    finally
                    {
                        this.Test.SetModeDoNotCheckCancel(false);
                    }
                    this.WriteToScroll("");
                    //14		= "Testschrittzeit: {0} Sec ({1})"
                    strMsg = String.Format(this.Data.Msg[14], cTimer.GetTimer(1).ToString("0.0"), this.Test.Timer.GetTimer(11).ToString("0.0"));
                    this.WriteToScroll(strMsg);
                    //Ergebnis ausgeben ///////////////////////////////////////////////////////////////////////
                    iResult = this.GetResult();
                    if (iResult == 0)//Alles O.K.
                    {
                        //2		= "--- O.K. ---"
                        this.WriteToScroll(this.Data.Msg[2], 4);
                    }
                    else
                    {                        
                        //3		= "!!! Error: [{0}:{1}] !!!"
                        strMsg = String.Format(this.Data.Msg[3], this.ID, this.ErrorCodeString);
                        this.WriteToScroll(strMsg, 4);
                    }
                    ////////////////////////////////////////////////////////////////////////////////////
                    this.WriteToScroll("");

                    if (!this.FlagExecutedFromTest)
                    {
                        CTestStep.FlagLayerOfTestStep--;
                        this.Data.LengthScroll = iLengthScrollTemp;
                    }                                       
                }

                this.Test.FormFFT.SetCurrentTestTime(this.Test.Timer.GetTimer(11));
                if (this.Test.FlagDoCancelTest == false)
                    return (iResult);
                else
                    return (-1);                                               	            
            }

            /// <summary>
            /// Gibt ein Ergebnis vom Testschritt zurück
            /// </summary>
            /// <returns>
            /// weniger als "-1" - Error mit Testabbruch; 
            /// "0" - Teststep O.K.;	
            /// mehr als "1" - Error, Test darf weitergemacht werden
            /// </returns>
	        public int GetResult()
	        {
	            int iError;
	            iError=this.GetError();
                
                return (iError);
            }

            /// <summary>
            /// Gibt ein Ergebnis vom Test zurück
            /// </summary>
            /// <returns>
            /// weniger als "-1" - Error mit Testabbruch; 
            /// "0" - Teststep O.K.;	
            /// mehr als "1" - Error, Test darf weitergemacht werden
            /// </returns>
	        public int GetResultTest()
            {
	            int iError = 0;
	           
                if (this.Test.ErrorFirst__ID_TestStep != 0)
                    iError = 1;

                if (this.Test.FlagDoCancelTest)
                    iError = -1;
             
	            return(iError);
            }

            /// <summary>
            /// Gibt Errorcode vom Testschritt im Format [2:2,4,56,79] zurück
            /// </summary>
            /// <returns>
            /// z.B. [2:2,4,56,79]
            /// Wenn Kein Error -> ""
            /// </returns>
	        public String GetErrorCode()
            {
	            String strErrorCode, strErrorList;
	            strErrorCode="";
	            strErrorList="";

	            if(this.GetResult()!=0)
	            {
		            strErrorCode=String.Format("[{0}:{1}]",this.ID,strErrorList);
	            }
	            return(strErrorCode);
            }

            /// <summary>
            /// Gibt den Testschrittname zurück
            /// Wenn es keinen Namen für ID gibt, dann ""
            /// </summary>
            /// <param name="ID">ID als Zahl</param>
            /// <returns>Testschrittname</returns>
	        public String GetName(int ID)
            {
	            String strNameTestStep, strID_TestStep;

	            strID_TestStep=this.GetID(this.ID);
	            strNameTestStep=this.Data.Msg.GetNameTestStep(strID_TestStep);	
            	
	            return(strNameTestStep);
            }

            /// <summary>
            /// Gibt ID im String Format zurück, wenn es keinen ID im String Format für ID gibt, dann ""
            /// </summary>
            /// <param name="ID">ID als Zahl</param>
            /// <returns>ID im String Format</returns>
	        public String GetID(int ID)	
		    {
	            String strID_TestStep;

	            try
	            {
                    strID_TestStep = this.Data.ID_TestStepLIST[this.ID - 1];		
	            }
	            catch (Exception e)
	            {
		            strID_TestStep=e.Message;
		            strID_TestStep="";
	            }
                
	            return(strID_TestStep);
            }

            /// <summary>
            /// Wenn Debug Modus gesetzt hält es von einem SetBreakPoint zu dem anderen 
            /// Mit F10 springt man vom einem SetBreakPoint zu dem anderen
            /// Mit F5 hält nich mehr an, mit F10 hält bei den nächsten SetBreakPoint
            /// Wenn Msg==null, dann wird der letzten Name aus der List this.Test.ErrorAttributeLIST genommen 
            /// </summary>
            /// <param name="Msg">Message was erschein wenn das Programm an den BreakPoint angekommen ist</param>
            /// <returns>true - Programm wird abgebrochen, false weiter machen</returns>
            protected bool SetBreakPoint(String Msg)            
            {
                String strName, strID_BreakPoint = "", strErrorCode="";
                bool bResult=false;                

                if(this.CancelTest())
                    return (true);

                if (this.Data.ModeDebug == 1)
                {
                    while (true)
                    {
                        //Name für den BreakPoint wird ermittelt//////////////////////////////////////////////////////////////////////////////////////////////////////
                        if (Msg != null)
                        {
                            this.ID_BeakPoint++;

                            strID_BreakPoint = String.Format("{0}:{1}", this.ID, this.ID_BeakPoint);
                            strName = String.Format("Breakpoint: {0} - {1}", strID_BreakPoint, Msg);
                            this.NameBreakpointLast = strName;
                            if (this.Data.WriteBreakPointToScroll == 1)
                            {
                                this.WriteToScroll("### " + strName + " ###");
                            }
                        }
                        else
                        {
                            if (this.Test.ErrorAttributeLIST.Count != 0)
                            {
                                strName = this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsg;
                                strErrorCode = this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorCode;
                                //7		= "Fehler: [{0}] - {1}"  //strErrorCode, strMsgError
                                strName = String.Format(this.Data.Msg[7], strErrorCode, strName);
                                this.NameBreakpointLast = strName;
                            }
                            else
                            {
                                strName = "";
                                this.NameBreakpointLast = strName;
                            }
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //Finden ob das Program am Breakpoint anhalten soll, wegen der Sys-Einstellungen //////////////////////////
                        if (Msg != null)
                        {
                            if (!this.Test.FlagBreakPointStop && this.Data.BreakPointID)
                            {
                                foreach (String strID in this.Data.BreakPointIDARRAY)
                                {
                                    if (strID_BreakPoint.CompareTo(strID) == 0)
                                    {
                                        this.Test.FlagBreakPointStop = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!this.Test.FlagBreakPointStop && strErrorCode.Length != 0)
                        {
                            if(this.Data.BreakPointAfterEveryError == 1)
                            {
                                this.Test.FlagBreakPointStop = true;
                            }
                            else
                            {
                                if(this.Data.BreakPointError)
                                {                                
                                    foreach (String strError in this.Data.BreakPointErrorARRAY)
                                    {
                                        if (strErrorCode.CompareTo(strError) == 0)
                                        {
                                            this.Test.FlagBreakPointStop = true;
                                            break;
                                        }
                                    }
                                }
                            }                            
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //Halteschleife ////////////////////////////////////////////////////////////////////////////////////////////////
                        if (this.Test.FlagBreakPointStop)
                        {
                            //8		= "(Weiter mit F10. Beenden mit F5 oder Esc)"
                            this.Test.FormFFT.WriteToStatus(strName + " " + this.Data.Msg[8]);
                            this.Test.FormFFT.KeyHitLast_F10_F5 = System.Windows.Forms.Keys.Escape;
                            while (true)
                            {
                                switch (this.Test.FormFFT.KeyHitLast_F10_F5)
                                {
                                    case System.Windows.Forms.Keys.F10:
                                        this.Test.FlagBreakPointStop = true;
                                        this.WriteToStatus("");
                                        return false;
                                    case System.Windows.Forms.Keys.F5:
                                        this.Test.FlagBreakPointStop = false;
                                        this.WriteToStatus("");
                                        return false;
                                }

                                if (this.Sleep(100))
                                {
                                    bResult = true;
                                    break;                                    
                                }                                    
                            }
                        }
                        break;
                        //////////////////////////////////////////////////////////////////////////////////////////////////
                    }
                    this.WriteToStatus("");
                    return (bResult);                   
                }      
                else
                {
                    if (Msg != null)
                    {
                        this.ID_BeakPoint++;
                        strID_BreakPoint = String.Format("{0}:{1}", this.ID, this.ID_BeakPoint);
                        strName = String.Format("Breakpoint: {0} - {1}", strID_BreakPoint, Msg);
                        this.NameBreakpointLast = strName;
                    }
                }
                return(false);
            }

            /// <summary>
            /// Schreibt zeilenweise Messages auf dem Display 
            /// </summary>
            /// <param name="Msg">Message zum Schreiben auf dem Display</param>
            /// <param name="Row">Zeile, wo Message geschrieben wird. max. 4</param>
            public void WriteToDisplay(String Msg, int Row)
            {
                this.Test.FormFFT.WriteToDisplay(Msg, Row);
            }

            /// <summary>
            /// Name den zu letzt ausgeführten Breakpoint
            /// </summary>
            string NameBreakpointLast;
            /// <summary>
            /// Der Test wird abgebrochen, wenn der aktuelle Testschritt zu Ende ist.
            /// </summary>
            protected void DoBreakFaultTest()
            {
                this.Test.FlagDoCancelTest = true;
                //this.Test.SetCancelTest();                
            }

            /// <summary>
            /// Löscht Zeilen im Displayfenster
            /// </summary>
            /// <param name="Row">
            /// Zeile zum Löschen im Displayfenster ( Row max. 10)
            /// Row=0-Alle; 
            /// Row=1-1 Zeile; 
            /// </param>
            void DeleteDisplay(int Row)
            {
                this.Test.FormFFT.DeleteDisplay(Row);                
            }

            /// <summary>
            /// Markiert den TestStep als fehlerhaft
            /// </summary>
            /// <param name="Error">
            /// Errorcode
            /// </param>
            /// <returns>
            /// Zustand des FFT vor dem Error setzen
            /// true  - First Error
            /// false - nächste Fehler
            /// </returns>
            public bool SetResultFault(int Error)
            {
                return (this.SetResultFault(Error, true));
            }


            /// <summary>
            /// Markiert den TestStep als fehlerhaft 
            /// </summary>
            /// <param name="Error">
            /// Errorcode
            /// </param>            
            /// <param name="TestContinue">
            /// Ist TestContinue auf false gesetzt, wird der ganzer Test, nachdem der TestStep ausgeführt wurde, abgebrochen
            /// </param>
            /// <returns>
            /// Zustand des FFT vor dem Error setzen
            /// true  - First Error
            /// false - nächste Fehler
            /// </returns>
            public bool SetResultFault(int Error, bool TestContinue)
            {
                System.Drawing.Color hBackColorOfListBoxItemTEMP;
                String strError, strErrorFirst;
                int iCount;
                int[] iErrorCodeToCancelARRAY;
                CErrorTestStep hError;
                bool bFirstError;

                bFirstError = false;
				if(Error != 0)
				{					
					if(!this.flagCallViaIsResultFault)
					{
                        CPartProductionDBforFFT.CMeasurment measurment = new CPartProductionDBforFFT.CMeasurment(this.ID, Error, string.Format("{0}:{1}", this.ID, Error));
						measurment.Result = false;
						measurment.ErrorDescription = this.Data.Msg.GetNameError(this.ID_String, Error);
						measurment.TestStepCodeString = this.ID_String;
                        measurment.TestStepDescription = this.GetName(this.ID);
						if(this.Data.DB != null)
							this.Data.DB.MeasurmentList.Add(measurment);
					}
					//Firsterror registrieren /////////////////////////////////////////
					if(this.Test.ErrorFirst__ID_TestStep == 0)
					{
                        this.Test.FormFFT.SetBarTestTime(false);
						this.Test.ErrorFirst__ID_TestStep = this.ID;
						this.Test.ErrorFirst__CodeErrorTestStep = Error;
						if(this.Data.DB != null)
							this.Data.DB.FirstError = string.Format("{0}:{1}", this.ID, Error);
						if(this.Data.DBFColumnFirstError != 0)
						{
							strErrorFirst = string.Format("{0}:{1}", this.ID, Error);
                            if (this.Data.DBF != null)
							    this.Data.DBF.SetValue(this.Data.DBFColumnFirstError, strErrorFirst);
							bFirstError = true;
						}
					}
					/////////////////////////////////////////
					//Testausführen  auf abbrechen setzen /////////////////////////////////////////
					if(TestContinue == false)
					{
						this.DoBreakFaultTest();
					}
					else
					{
						//Prüfen, ob Testschritt mit Sys-Datei abgebrochn wird
						iErrorCodeToCancelARRAY = this.Test.Data.FlagTestStepCancelTestIfErrorID_LIST[this.ID - 1];
						foreach(int iErrorCode in iErrorCodeToCancelARRAY)
						{
							if(iErrorCode == -1 || iErrorCode == Error)
							{
								this.DoBreakFaultTest();
								break;
							}
						}
					}
					/////////////////////////////////////////

					hError = new CErrorTestStep();
					hError.ID_Error = Error;
					hError.ID_TestStep = this.ID;
					hError.ErrorMsg = this.Data.Msg.GetNameError(this.ID_String, Error);
					hError.ErrorMsgPrint = this.Data.Msg.GetNameErrorPrint(this.ID_String, Error);
					hError.ErrorMsgMeasurement = "";

					//7		= "Fehler: [{0}] - {1}" 
					strError = String.Format(this.Data.Msg[7], hError.ErrorCode, hError.ErrorMsg);

					hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
					this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;

					this.WriteToScroll(strError);

					this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;

					this.Test.ErrorAttributeLIST.Add(hError);
					this.Error += hError.ErrorMsg;
					this.ErrorCode = hError.ID_Error;
					this.SetBreakPoint(null);

					//Top 10 Error //////////////////////////////////////////////////////////
					if(this.Test.CountOfError.ContainsKey(hError.ErrorCode) == true)
					{
						iCount = this.Test.CountOfError[hError.ErrorCode];
						iCount++;
						this.Test.CountOfError[hError.ErrorCode] = iCount;
					}
					else
					{
						iCount = 1;
						this.Test.CountOfError.Add(hError.ErrorCode, iCount);
					}
					//////////////////////////////////////////////////////////

				}
				else
				{
					this.Error = "";
					this.ErrorCode = 0;
				}

                return (bFirstError);
            }
			/// <summary>
			/// Um fest zustellen, ob die SetResultFault über IsResultFault oder direkt aufgerufen wurde 
			/// </summary>
			private bool flagCallViaIsResultFault = false;
            /// <summary>
            /// Mit dieser Funktion wird der Wert "Value" mit Parametern "LSL" und "USL" überprüft, im Scrollfenster angezeigt und in die Datenbankgeschrieben.
            /// Liegt der Wert ausserhalb der Limits, setzt die Funktion den Testschritt als fehlerhaft mit dem Errrorcode "Error"
            /// </summary>
            /// <param name="Error">Errorcode</param>
            /// <param name="Value">Wert, was geprüft wird</param>
            /// <param name="LSL">Untere Grenze</param>
            /// <param name="USL">Obere Grenze</param>
            /// <param name="FormatString_NameValueUnitLimit">
            /// Mit Hilfe dieses Strings wird der Wert "Value" im Scrollfenster angezeigt
            /// Z.B. "R_FAN_0|0.000|KOhm|0.00" 
            /// 1-Name von "Value"; 
            /// 2-Nachkommastellen in Value; 
            /// 3-Messeinheit von Value; 
            /// 4-Nachkommastellen für Limits (optional)
            /// </param>
            /// <param name="DBF_Index_Value">
            /// Spalte in der Datenbank, wo Value geschrieben wird
            /// 0 - Daten werden nicht in die datenbank geschrieben
            /// </param>
            /// <param name="DBF_Index_LSL">    
            /// Spalte in der Datenbank, wo LSL geschrieben wird
            /// Mit DBF_Index_LSL und DBF_Index_USL kann man die Art der Prüfung bestimmen
            ///                                                    Kein Error wenn:
            /// Wenn DBF_Index_LSL und DBF_Index_USL != 0 und mehr als 0  (LSL mehr oder = Value weniger oder = USL)      EFlagComparer.RangeLSL_USL    = 1,
            /// Wenn DBF_Index_LSL != 0 und DBF_Index_USL == 0            (LSL mehr oder = Value)                         EFlagComparer.RangeLSL        = 2,
            /// Wenn DBF_Index_USL != 0 und DBF_Index_LSL == 0            (Value weniger oder = USL)                      EFlagComparer.RangeUSL        = 3,
            /// Wenn DBF_Index_USL == DBF_Index_LSL und mehr als 0        (Value == LSL)                                  EFlagComparer.EqualLSL_USL    = 4,
            /// Wenn DBF_Index_USL == DBF_Index_LSL und weniger als 0     (Value != LSL)                                  EFlagComparer.UnequalLSL_USL  = 5,
            /// Unit noch nicht gesetzt                                                                                   EFlagComparer.Empty           = 0,
            /// </param>
            /// <param name="DBF_Index_USL">
            /// Spalte in der Datenbank, wo USL geschrieben wird
            /// </param>
            /// <returns>
            /// true wenn Value liegt innerhalb der Grenzen
            /// false wenn Value liegt asserhalb der grenzen 
            /// </returns>
            protected bool IsResultFault(int Error, double Value, double LSL, double USL, String FormatString_NameValueUnitLimit, int DBF_Index_Value, int DBF_Index_LSL, int DBF_Index_USL)   
            {
                EFlagComparer eFlagToTest;
                String[] strFormatARRAY;
                String[] strBufferARRAY;
                String strValueFormat, strSLFormat;
                String strName, strValue, strUnit, strLSL, strUSL, strBuffer, strValueUnit;
                int iCount, iDBF_Index_Value, iDBF_Index_LSL, iDBF_Index_USL;
                System.Drawing.Color hBackColorOfListBoxItemTEMP;
                bool bResult = false;


                iDBF_Index_Value=DBF_Index_Value;
                if(iDBF_Index_Value<0)
                    iDBF_Index_Value*=(-1);
                iDBF_Index_LSL=DBF_Index_LSL;
                if(iDBF_Index_LSL<0)
                    iDBF_Index_LSL*=(-1);
                iDBF_Index_USL=DBF_Index_USL;
                if(iDBF_Index_USL<0)
                    iDBF_Index_USL*=(-1);

                //FormatString ausgwertet ///////////////////////////////////////////
                strFormatARRAY = FormatString_NameValueUnitLimit.Split('|');
                iCount = strFormatARRAY.Length;                
                strValueFormat = "";                
                strSLFormat = "";
                strUnit = "";
                strName = "";
                switch (iCount)
                {
                    case 4:                        
                        strSLFormat = strFormatARRAY[3];
                        goto case 3;
                    case 3:                        
                        strUnit = strFormatARRAY[2];
                        goto case 2;
                    case 2:                        
                        strValueFormat = strFormatARRAY[1];
                        goto case 1;
                    case 1:
                        strName = strFormatARRAY[0];
                        break;                                                  
                }
                //////////////////////////////////////////////////////

                //Testart setzen //////////////////////////////////////////////////////
                eFlagToTest=EFlagComparer.Empty;
                if (DBF_Index_LSL != DBF_Index_USL)
                {
                    if (DBF_Index_LSL == 0)
                        eFlagToTest = EFlagComparer.RangeUSL;
                    else if (DBF_Index_USL == 0)
                        eFlagToTest = EFlagComparer.RangeLSL;
                    else 
                        eFlagToTest = EFlagComparer.RangeLSL_USL;
                }
                else
                {
                    if (DBF_Index_LSL != 0)
                    {
                        if(DBF_Index_LSL<0)
                            eFlagToTest = EFlagComparer.UnequalLSL;
                        else
                            eFlagToTest = EFlagComparer.EqualLSL;
                    }
                    else//Wenn DBF_Index_LSL == DBF_Index_USL == 0
                    {
                        if(LSL==USL)
                            eFlagToTest = EFlagComparer.EqualLSL;
                        else
                            eFlagToTest = EFlagComparer.RangeLSL_USL;
                    }

                }
                //////////////////////////////////////////////////////

                //Umwandeln Value, LSL und USL in String Format umwandeln ////////////////////////////                
                if (strValueFormat.Length != 0)
                {
                    strBuffer=String.Format("{0}",strValueFormat);
                    strValue = Value.ToString(strBuffer);
                }
                else
                    strValue = "";
                if (strSLFormat.Length != 0)
                {
                    strBuffer = String.Format("{0}", strSLFormat);
                    strLSL = LSL.ToString(strBuffer);
                }
                else
                {
                    if (strValueFormat.Length != 0)
                    {
                        strBuffer = String.Format("{0}", strValueFormat);
                        strLSL = LSL.ToString(strBuffer);
                    }
                    else
                        strLSL = "";
                }
                if (strSLFormat.Length != 0)
                {
                    strBuffer = String.Format("{0}", strSLFormat);
                    strUSL = USL.ToString(strBuffer);
                }
                else
                {
                    if (strValueFormat.Length != 0)
                    {
                        strBuffer = String.Format("{0}", strValueFormat);
                        strUSL = USL.ToString(strBuffer);
                    }
                    else
                        strUSL = "";
                }                   
                //////////////////////////////////////////////////////


                //Auswertung //////////////////////////////////////////////////////
                if (strUnit.Length != 0)
                    strValueUnit = String.Format("{0}[{1}]", strValue, strUnit);
                else
                    strValueUnit = strValue;
                switch (eFlagToTest)
                {                    
                    case EFlagComparer.RangeLSL_USL:
                        if (iDBF_Index_Value != 0)
                        {
                            this.Data.DBF.SetValue(iDBF_Index_Value, strValue);
                            if (iDBF_Index_LSL != 0)
                                this.Data.DBF.SetValue(iDBF_Index_LSL, strLSL);
                            if (iDBF_Index_USL != 0)
                                this.Data.DBF.SetValue(iDBF_Index_USL, strUSL);
                        }
                        if (Value < LSL || Value > USL)//Error
                        {
                            if(this.SetResultFault(Error))
                            {
                                //First Error ///////////////////////                               
                                if (this.Data.DBFColumnMeasValue != 0 && this.Data.DBFColumnLSL != 0 && this.Data.DBFColumnUSL != 0)
                                {
                                    this.Data.DBF.SetValue(this.Data.DBFColumnMeasValue, strValue);
                                    this.Data.DBF.SetValue(this.Data.DBFColumnLSL, strLSL);
                                    this.Data.DBF.SetValue(this.Data.DBFColumnUSL, strUSL);
                                    if (this.Data.DBFColumnDBFColumnOfMeasValue != 0 && iDBF_Index_Value != 0)
                                    {
                                        strBuffer = string.Format("{0}", iDBF_Index_Value);
                                        this.Data.DBF.SetValue(this.Data.DBFColumnDBFColumnOfMeasValue, strBuffer);
                                    }
                                }                                
                            }
							this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} ({2} - {3})", strName, strValueUnit, strLSL, strUSL);
                            hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                            this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                            if (strName.Length != 0 || strValueUnit.Length != 0 || strLSL.Length != 0 || strUSL.Length != 0)
                            {
                                if (strName.Length == 0)
                                {
                                    strBufferARRAY=this.Test.FormFFT.GetRowFromListBox(0);
                                    this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, strLSL, strUSL);                                    
                                }
                                else
                                    this.WriteToScroll(strName, strValueUnit, strLSL, strUSL);								
                            }
                            this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                            bResult = true;
	                    }
                        else
                        {
                            this.WriteToScroll(strName, strValueUnit, strLSL, strUSL);                             
                        }                        
                	    break;
                    case EFlagComparer.RangeLSL:
                        if (iDBF_Index_Value != 0)
                        {
                            this.Data.DBF.SetValue(iDBF_Index_Value, strValue);
                            if (iDBF_Index_LSL != 0)
                                this.Data.DBF.SetValue(iDBF_Index_LSL, strLSL);
                        }
                        if (Value < LSL)//Error
                        {
                            if(this.SetResultFault(Error))
                            {
                                //First Error ///////////////////////
                                if (this.Data.DBFColumnMeasValue != 0 && this.Data.DBFColumnLSL != 0)
                                {
                                    this.Data.DBF.SetValue(this.Data.DBFColumnMeasValue, strValue);
                                    this.Data.DBF.SetValue(this.Data.DBFColumnLSL, strLSL);
                                    if (this.Data.DBFColumnDBFColumnOfMeasValue != 0 && iDBF_Index_Value != 0)
                                    {
                                        strBuffer = string.Format("{0}", iDBF_Index_Value);
                                        this.Data.DBF.SetValue(this.Data.DBFColumnDBFColumnOfMeasValue, strBuffer);
                                    }
                                }
                            }
							this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} (< {2})", strName, strValueUnit, strLSL);
                            hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                            this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                            if (strName.Length != 0 || strValueUnit.Length != 0 || strLSL.Length != 0)
                            {
                                if (strName.Length == 0)
                                {
                                    strBufferARRAY=this.Test.FormFFT.GetRowFromListBox(0);
                                    this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, strLSL, "");                                    
                                }
                                else
                                    this.WriteToScroll(strName, strValueUnit, strLSL, "");
                            }
                            this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                            bResult = true;                                                       
                        }
                        else
                        {
                            this.WriteToScroll(strName, strValueUnit, strLSL, "");                             
                        }
                	    break;
                    case EFlagComparer.RangeUSL:
                        if (iDBF_Index_Value != 0)
                        {
                            this.Data.DBF.SetValue(iDBF_Index_Value, strValue);
                            if (iDBF_Index_USL != 0)
                                this.Data.DBF.SetValue(iDBF_Index_USL, strUSL);
                        }
                        if (Value > USL)//Error
                        {
                            if(this.SetResultFault(Error))
                            {
                                //First Error ///////////////////////
                                if (this.Data.DBFColumnMeasValue != 0 && this.Data.DBFColumnUSL != 0)
                                {
                                    this.Data.DBF.SetValue(this.Data.DBFColumnMeasValue, strValue);                                    
                                    this.Data.DBF.SetValue(this.Data.DBFColumnUSL, strUSL);
                                    if (this.Data.DBFColumnDBFColumnOfMeasValue != 0 && iDBF_Index_Value != 0)
                                    {
                                        strBuffer = string.Format("{0}", iDBF_Index_Value);
                                        this.Data.DBF.SetValue(this.Data.DBFColumnDBFColumnOfMeasValue, strBuffer);
                                    }
                                }
                            }
							this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} (> {2})", strName, strValueUnit, strUSL);
                            hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                            this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                            if (strName.Length != 0 || strValueUnit.Length != 0 || strUSL.Length != 0)
                            {
                                if (strName.Length == 0)
                                {
                                    strBufferARRAY = this.Test.FormFFT.GetRowFromListBox(0);
                                    this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, "", strUSL);
                                }
                                else
                                    this.WriteToScroll(strName, strValueUnit, "", strUSL);
                            }
                            this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                            bResult = true;
                        }
                        else
                        {
                            this.WriteToScroll(strName, strValueUnit, "", strUSL);
                        }
                        break;
                    case EFlagComparer.EqualLSL:
                        if (iDBF_Index_Value != 0)
                        {
                            this.Data.DBF.SetValue(iDBF_Index_Value, strValue);
                            if (iDBF_Index_LSL != 0)
                                this.Data.DBF.SetValue(iDBF_Index_LSL, strLSL);
                            if (iDBF_Index_USL != 0)
                                this.Data.DBF.SetValue(iDBF_Index_USL, strUSL);
                        }
                        if (Value != LSL)//Error
                        {
                            if(this.SetResultFault(Error))
                            {
                                //First Error ///////////////////////
                                if (this.Data.DBFColumnMeasValue != 0 && this.Data.DBFColumnLSL != 0 && this.Data.DBFColumnUSL != 0)
                                {
                                    this.Data.DBF.SetValue(this.Data.DBFColumnMeasValue, strValue);
                                    this.Data.DBF.SetValue(this.Data.DBFColumnLSL, strLSL);
                                    this.Data.DBF.SetValue(this.Data.DBFColumnUSL, strLSL);
                                    if (this.Data.DBFColumnDBFColumnOfMeasValue != 0 && iDBF_Index_Value != 0)
                                    {
                                        strBuffer = string.Format("{0}", iDBF_Index_Value);
                                        this.Data.DBF.SetValue(this.Data.DBFColumnDBFColumnOfMeasValue, strBuffer);
                                    }
                                }
                            }
							this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} (> {2})", strName, strValueUnit, strLSL);
                            hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                            this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                            if (strName.Length != 0 || strValueUnit.Length != 0 || strLSL.Length != 0)
                            {
                                if (strName.Length == 0)
                                {
                                    strBufferARRAY = this.Test.FormFFT.GetRowFromListBox(0);
                                    this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, "==", strLSL);
                                }
                                else
                                    this.WriteToScroll(strName, strValueUnit, "==", strLSL);
                            }
                            this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                            bResult = true;
                        }
                        else
                        {
                            this.WriteToScroll(strName, strValueUnit, "==", strLSL);
                        }
                        break;
                    case EFlagComparer.UnequalLSL:
                        if (iDBF_Index_Value != 0)
                        {
                            this.Data.DBF.SetValue(iDBF_Index_Value, strValue);
                            if (iDBF_Index_LSL != 0)
                                this.Data.DBF.SetValue(iDBF_Index_LSL, strLSL);
                            if (iDBF_Index_USL != 0)
                                this.Data.DBF.SetValue(iDBF_Index_USL, strUSL);
                        }
                        if (Value == LSL)//Error
                        {
                            if(this.SetResultFault(Error))
                            {
                                //First Error ///////////////////////
                                if (this.Data.DBFColumnMeasValue != 0 && this.Data.DBFColumnLSL != 0 && this.Data.DBFColumnUSL != 0)
                                {
                                    this.Data.DBF.SetValue(this.Data.DBFColumnMeasValue, strValue);
                                    this.Data.DBF.SetValue(this.Data.DBFColumnLSL, strLSL);
                                    this.Data.DBF.SetValue(this.Data.DBFColumnUSL, strLSL);
                                    if (this.Data.DBFColumnDBFColumnOfMeasValue != 0 && iDBF_Index_Value!=0)
                                    {
                                        strBuffer = string.Format("{0}", iDBF_Index_Value);
                                        this.Data.DBF.SetValue(this.Data.DBFColumnDBFColumnOfMeasValue, strBuffer);
                                    }
                                }
                            }
							this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} (== {2})", strName, strValueUnit, strLSL);
                            hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                            this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                            if (strName.Length != 0 || strValueUnit.Length != 0 || strLSL.Length != 0)
                            {
                                if (strName.Length == 0)
                                {
                                    strBufferARRAY = this.Test.FormFFT.GetRowFromListBox(0);
                                    this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, "!=", strLSL);
                                }
                                else
                                    this.WriteToScroll(strName, strValueUnit, "!=", strLSL);
                            }
                            this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                            bResult = true;
                        }
                        else
                        {
                            this.WriteToScroll(strName, strValueUnit, "!=", strLSL);
                        }
                        break;                        
                }
                //////////////////////////////////////////////////////

                return (bResult);
            }

            protected bool IsResultFault(int Error, object Value, object LSL, object USL, string FormatString_NameValueUnitLimit, string DB_ValueName, EFlagComparer Mode)
            {
                String[] strFormatARRAY;
                String[] strBufferARRAY;
                String strValueFormat, strSLFormat;
                String strName, strValue, strUnit, strLSL = "", strUSL = "", strBuffer, strValueUnit;
                string strType;
                int iCount;
                System.Drawing.Color hBackColorOfListBoxItemTEMP;
                bool bResult = false;
                double dValue, dLSL, dUSL;


                CPartProductionDBforFFT.CMeasurment measurment = new CPartProductionDBforFFT.CMeasurment(this.ID, Error, DB_ValueName);

                //////////////////////////////////////////////////////
                #region//FormatString ausgwertet
                strFormatARRAY = FormatString_NameValueUnitLimit.Split('|');
                iCount = strFormatARRAY.Length;
                strValueFormat = "";
                strSLFormat = "";
                strUnit = "";
                strName = "";
                switch (iCount)
                {
                    case 4:
                        strSLFormat = strFormatARRAY[3];
                        goto case 3;
                    case 3:
                        strUnit = strFormatARRAY[2];
                        measurment.MeasUnitName = strUnit;
                        goto case 2;
                    case 2:
                        strValueFormat = strFormatARRAY[1];
                        goto case 1;
                    case 1:
                        strName = strFormatARRAY[0];
                        break;
                }
                #endregion
                //////////////////////////////////////////////////////


                //////////////////////////////////////////////////////
                #region//Umwandeln Value, LSL und USL in String Format umwandeln
                if (Value.GetType() == typeof(double))
                {
                    double dBuffer;
                    int iPlace;
                    dBuffer = (double)Value;
                    if (strValueFormat.Length != 0)
                    {
                        strBuffer = String.Format("{0}", strValueFormat);
                        strValue = dBuffer.ToString(strBuffer);
                        iPlace = strBuffer.IndexOf(".");
                        if (strValue.Length > CPartProductionDB.tab_Measurment.LengthValueField)
                        {
                            strValue = dBuffer.ToString(string.Format("E{0}", CPartProductionDB.tab_Measurment.LengthValueField - 7));
                            strType = "double";
                        }
                        else
                        {
                            if (iPlace >= 0)
                            {
                                if (strBuffer.Length - 1 - iPlace != 0)
                                    strType = string.Format("double{0}", strBuffer.Length - 1 - iPlace);
                                else
                                    strType = "double";
                            }
                            else
                                strType = "double";
                        }
                    }
                    else
                    {
                        strValue = dBuffer.ToString();
                        if (strValue.Length > CPartProductionDB.tab_Measurment.LengthValueField)
                        {
                            strValue = dBuffer.ToString(string.Format("E{0}", CPartProductionDB.tab_Measurment.LengthValueField - 7));
                        }
                        strType = "double";
                    }
                    measurment.Value = strValue;

                    if (LSL != null)
                    {
                        dBuffer = (double)LSL;
                        if (strSLFormat.Length != 0)
                        {
                            strBuffer = String.Format("{0}", strSLFormat);
                            strLSL = dBuffer.ToString(strBuffer);
                        }
                        else
                        {
                            if (strValueFormat.Length != 0)
                            {
                                strBuffer = String.Format("{0}", strValueFormat);
                                strLSL = dBuffer.ToString(strBuffer);
                            }
                            else
                                strLSL = dBuffer.ToString();
                        }
                        measurment.LSL = strLSL;
                    }

                    if (USL != null)
                    {
                        dBuffer = (double)USL;
                        if (strSLFormat.Length != 0)
                        {
                            strBuffer = String.Format("{0}", strSLFormat);
                            strUSL = dBuffer.ToString(strBuffer);
                        }
                        else
                        {
                            if (strValueFormat.Length != 0)
                            {
                                strBuffer = String.Format("{0}", strValueFormat);
                                strUSL = dBuffer.ToString(strBuffer);
                            }
                            else
                                strUSL = dBuffer.ToString();
                        }
                        measurment.USL = strUSL;
                    }
                }
                else if (Value.GetType() == typeof(int))
                {
                    int iBuffer;
                    strType = "int";
                    iBuffer = (int)Value;
                    strValue = iBuffer.ToString();
                    measurment.Value = strValue;
                    iBuffer = (int)LSL;
                    strLSL = iBuffer.ToString();
                    measurment.LSL = strLSL;
                    if (USL != null)
                    {
                        iBuffer = (int)USL;
                        strUSL = iBuffer.ToString();
                        measurment.USL = strUSL;
                    }
                }
                else
                {
                    strType = "string";
                    strValue = Value.ToString();
                    measurment.Value = strValue;
                    strLSL = LSL.ToString();
                    measurment.LSL = strLSL;
                    if (USL != null)
                    {
                        strUSL = USL.ToString();
                        measurment.USL = strUSL;
                    }
                }

                #endregion
                //////////////////////////////////////////////////////			

                //////////////////////////////////////////////////////
                #region//Auswertung
                if (strUnit.Length != 0)
                    strValueUnit = String.Format("{0}[{1}]", strValue, strUnit);
                else
                    strValueUnit = strValue;
                switch (Mode)
                {
                    case EFlagComparer.RangeLSL_USL:
                        #region
                        dValue = (double)Value;
                        dLSL = (double)LSL;
                        dUSL = (double)USL;
                        if (dValue < dLSL || dValue > dUSL)//Error
                        {                            
                            hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                            this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                            if (strName.Length != 0 || strValueUnit.Length != 0 || strLSL.Length != 0 || strUSL.Length != 0)
                            {
                                if (strName.Length == 0)
                                {
                                    strBufferARRAY = this.Test.FormFFT.GetRowFromListBox(0);
                                    this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, strLSL, strUSL);
                                }
                                else
                                    this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, strLSL, strUSL);
                            }
                            this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                            this.flagCallViaIsResultFault = true;
                            this.SetResultFault(Error);
                            this.flagCallViaIsResultFault = false;
                            this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} ({2} - {3})", strName, strValueUnit, strLSL, strUSL);
                            bResult = true;
                        }
                        else
                        {
                            this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, strLSL, strUSL);
                        }
                        #endregion
                        break;
                    case EFlagComparer.RangeLSL:
                        #region
                        dValue = (double)Value;
                        dLSL = (double)LSL;
                        measurment.USL = null;
                        if (dValue < dLSL)//Error
                        {
                            hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                            this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                            if (strName.Length != 0 || strValueUnit.Length != 0 || strLSL.Length != 0)
                            {
                                if (strName.Length == 0)
                                {
                                    strBufferARRAY = this.Test.FormFFT.GetRowFromListBox(0);
                                    this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, strLSL, "");
                                }
                                else
                                    this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, strLSL, "");
                            }
                            this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                            this.flagCallViaIsResultFault = true;
                            this.SetResultFault(Error);
                            this.flagCallViaIsResultFault = false;
                            this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} (< {2})", strName, strValueUnit, strLSL);
                            bResult = true;
                        }
                        else
                        {
                            this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, strLSL, "");
                        }
                        #endregion
                        break;
                    case EFlagComparer.RangeUSL:
                        #region
                        dValue = (double)Value;
                        dUSL = (double)USL;
                        measurment.LSL = null;
                        if (dValue > dUSL)//Error
                        {                            
                            hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                            this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                            if (strName.Length != 0 || strValueUnit.Length != 0 || strUSL.Length != 0)
                            {
                                if (strName.Length == 0)
                                {
                                    strBufferARRAY = this.Test.FormFFT.GetRowFromListBox(0);
                                    this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, "", strUSL);
                                }
                                else
                                    this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, "", strUSL);
                            }
                            this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                            this.flagCallViaIsResultFault = true;
                            this.SetResultFault(Error);
                            this.flagCallViaIsResultFault = false;
                            this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} (> {2})", strName, strValueUnit, strUSL);
                            bResult = true;
                        }
                        else
                        {
                            this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, "", strUSL);
                        }
                        #endregion
                        break;
                    case EFlagComparer.EqualLSL:
                        #region
                        measurment.USL = strLSL;
                        if (strType == "string")
                        {
                            if (strValue != strLSL)//Error
                            {                                
                                hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                                this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                                if (strName.Length != 0 || strValueUnit.Length != 0 || strLSL.Length != 0)
                                {
                                    if (strName.Length == 0)
                                    {
                                        strBufferARRAY = this.Test.FormFFT.GetRowFromListBox(0);
                                        this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, "==", strLSL);
                                    }
                                    else
                                        this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, "==", strLSL);
                                }
                                this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                                this.flagCallViaIsResultFault = true;
                                this.SetResultFault(Error);
                                this.flagCallViaIsResultFault = false;
                                this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} (> {2})", strName, strValueUnit, strLSL);
                                bResult = true;
                            }
                            else
                            {
                                this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, "==", strLSL);
                            }
                        }
                        else
                        {
                            dValue = (double)Value;
                            dLSL = (double)LSL;
                            if (dValue != dLSL)//Error
                            {                                
                                hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                                this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                                if (strName.Length != 0 || strValueUnit.Length != 0 || strLSL.Length != 0)
                                {
                                    if (strName.Length == 0)
                                    {
                                        strBufferARRAY = this.Test.FormFFT.GetRowFromListBox(0);
                                        this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, "==", strLSL);
                                    }
                                    else
                                        this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, "==", strLSL);
                                }
                                this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                                this.flagCallViaIsResultFault = true;
                                this.SetResultFault(Error);
                                this.flagCallViaIsResultFault = false;
                                this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} (> {2})", strName, strValueUnit, strLSL);
                                bResult = true;
                            }
                            else
                            {
                                this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, "==", strLSL);
                            }
                        }
                        #endregion
                        break;
                    case EFlagComparer.UnequalLSL:
                        #region
                        //measurment.Multiplier = -1;
                        measurment.USL = strLSL;
                        if (strType == "string")
                        {
                            if (strValue == strLSL)//Error
                            {                                
                                hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                                this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                                if (strName.Length != 0 || strValueUnit.Length != 0 || strLSL.Length != 0)
                                {
                                    if (strName.Length == 0)
                                    {
                                        strBufferARRAY = this.Test.FormFFT.GetRowFromListBox(0);
                                        this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, "!=", strLSL);
                                    }
                                    else
                                        this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, "!=", strLSL);
                                }
                                this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                                this.flagCallViaIsResultFault = true;
                                this.SetResultFault(Error);
                                this.flagCallViaIsResultFault = false;
                                this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} (== {2})", strName, strValueUnit, strLSL);
                                bResult = true;
                            }
                            else
                            {
                                this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, "!=", strLSL);
                            }
                        }
                        else
                        {
                            dValue = (double)Value;
                            dLSL = (double)LSL;
                            if (dValue == dLSL)//Error
                            {                                
                                hBackColorOfListBoxItemTEMP = this.Test.FormFFT.BackColorOfListBoxItem;
                                this.Test.FormFFT.BackColorOfListBoxItem = System.Drawing.Color.Red;
                                if (strName.Length != 0 || strValueUnit.Length != 0 || strLSL.Length != 0)
                                {
                                    if (strName.Length == 0)
                                    {
                                        strBufferARRAY = this.Test.FormFFT.GetRowFromListBox(0);
                                        this.ReplaceScrollLast(strBufferARRAY[0], strValueUnit, "!=", strLSL);
                                    }
                                    else
                                        this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, "!=", strLSL);
                                }
                                this.Test.FormFFT.BackColorOfListBoxItem = hBackColorOfListBoxItemTEMP;
                                this.flagCallViaIsResultFault = true;
                                this.SetResultFault(Error);
                                this.flagCallViaIsResultFault = false;
                                this.Test.ErrorAttributeLIST[this.Test.ErrorAttributeLIST.Count - 1].ErrorMsgMeasurement = string.Format("{0}: {1} (== {2})", strName, strValueUnit, strLSL);
                                bResult = true;
                            }
                            else
                            {
                                this.WriteToScroll(strName + string.Format(" [{0}]", DB_ValueName), strValueUnit, "!=", strLSL);
                            }
                        }
                        #endregion
                        break;
                }
                #endregion
                //////////////////////////////////////////////////////

                //////////////////////////////////////////////////////
                #region//Vorbereitung für DB-Beschreibung
                measurment.Result = !bResult;
                measurment.MeasUnitType = strType;
                measurment.ErrorDescription = this.Data.Msg.GetNameError(this.ID_String, Error);
                measurment.TestStepCodeString = this.ID_String;
                measurment.TestStepDescription = this.Name;
                if (this.Data.Msg.DescriptionMeasurementDICTIONARY != null)
                {
                    if (this.Data.Msg.DescriptionMeasurementDICTIONARY.ContainsKey(DB_ValueName))
                    {
                        measurment.ErrorDescriptionOfMeasurement = this.Data.Msg.DescriptionMeasurementDICTIONARY[DB_ValueName];
                    }
                }
                if (this.Data.DB != null)
                    this.Data.DB.MeasurmentList.Add(measurment);
                #endregion
                //////////////////////////////////////////////////////

                return (bResult);
            }

			protected void WriteToDbNote(string Name, string Value)
			{
				if(this.Data.DB != null)
				{
                    CPartProductionDBforFFT.CNote note = new CPartProductionDBforFFT.CNote(this.ID, Name, Value);
					note.TestStepIdString = this.ID_String;
					note.TestStepDescription = this.Name;
					this.Data.DB.NoteList.Add(note);
				}
				return;
			}
            protected void WriteToDbDutNumber(string Number, string NumberType)
			{
				if(this.Data.DB != null)
				{
                    CPartProductionDBforFFT.CDutNumber dutNumber = new CPartProductionDBforFFT.CDutNumber(Number, NumberType);
					this.Data.DB.DutNumberList.Add(dutNumber);
				}
				return;
			}

            /// <summary>
            /// Inkrementiert die DUT-Nummber (wird benutzt um besodere Regeln zum Inkrementieren zu benutzen)
            /// </summary>
            /// <param name="Number">
            /// Nummer zum Inkrementieren
            /// </param>
            /// <returns>
            /// null - Fehler
            /// sonst die Nummer nach der Inkremenrierung
            /// </returns>
            public Honeywell.Data.CPartProductionDB.tab_DutNumberForCreate.IncrementDutNumberHandle IncrementDutNumber = null;
            /// <summary>
            /// Erstelt die Nummer in der tab_DutNumberForCreate mit der NumberType -> NumberType)
            /// Beim besonderen Inkrementiern wird delegate IncrementDutNumber benutzt
            /// </summary>
            /// <param name="NumberType">
            /// NumberType der Nummer, um die Nummer in der Tabelle auseinander zu halten
            /// </param>
            /// <param name="Number_LSL">
            /// Wenn in der Datenbank noch bis jetzt keine Nummer angelegt war, wird diese Number_LSL benutzt um alle erste Nummer anzulegen
            /// bei null - die Nummer wird mit IncrementDutNumber erstellt
            /// </param>
            /// <param name="Number_USL">
            /// Die Max.Nummer die erstellt werden darf
            /// </param>
            /// <param name="FromBase">
            /// Die Nummer ist eine
            /// 10 - dezimale Nummer
            /// 16 - hexadezimale Nummer
            /// </param>
            /// <returns>
            /// null - Fehler
            /// sonst neuerstellte Nummer
            /// </returns>
            protected string CreateDutNumberInTab_DutNumberForCreateDB(string NumberType, string Number_LSL = null, string Number_USL = null, int FromBase = 16)
            {
                string strNumberDUT = null;
                if (this.Data.DB != null)
                {
                    this.Data.DB.Tab_DutNumberForCreate.IncrementDutNumber = this.IncrementDutNumber;
                    strNumberDUT = this.Data.DB.Tab_DutNumberForCreate.CreateDutNumber(NumberType, Number_LSL, Number_USL, FromBase);
                    if (strNumberDUT == null)
                    {
                        this.WriteToScroll(this.Data.DB.Error);
                    }
                }
                return strNumberDUT;
            }
            /// <summary>
            /// Sucht die Nummer in tab_DutNumberForCreate
            /// (Die NumberType wird erstellt in tab_NumberType, wenn bis jetzt noch keine angelegt war)
            /// </summary>
            /// <param name="NumberType"></param>
            /// <param name="Number"></param>
            /// <returns>
            /// -1 - Error
            ///  0 - Seriennummer ist nicht in der DB
            ///  1 - Seriennummer ist in der DB
            /// </returns>
            protected int SearchDutNumberInTab_DutNumberForCreateDB(string NumberType, string Number)
            {
                int iResult = 0;
                if (this.Data.DB != null)
                {
                    iResult = this.Data.DB.Tab_DutNumberForCreate.SearchDutNumber(NumberType, Number);
                }
                return iResult;
            }
            /// <summary>
            /// nur zum Kompatibilität (sonst sollte CreateDutNumberInTab_DutNumberForCreateDB benutzt werden)
            /// </summary>
            /// <param name="NumberType"></param>
            /// <param name="Number_LSL"></param>
            /// <param name="Number_USL"></param>
            /// <param name="FromBase"></param>
            /// <returns></returns>
            protected string CreateDutNumberFromDB(string NumberType, string Number_LSL = null, string Number_USL = null, int FromBase = 16)
            {                
                return this.CreateDutNumberInTab_DutNumberForCreateDB(NumberType, Number_LSL, Number_USL, FromBase);
            }
            
            
            //}
            /// <summary>
            /// Löscht Zeilen im Scrollfenster
            /// </summary>
            /// <param name="Nr">Nr=0-Alle; Nr=1-Letzte; Nr=2-Letzte zwei usw.</param>
            public void DeleteScroll(int Nr)
            {
                this.Test.FormFFT.DeleteScroll(Nr);
            }

            /// <summary>
            /// Ersetzt letzte Zeile im Scrollfenster
            /// </summary>
            /// <param name="Msg">Message-Spalte</param>
            /// <param name="Value">Value-Spalte</param>
            /// <param name="LSL">LSL-Spalte</param>
            /// <param name="USL">USL-Spalte</param>
            public void ReplaceScrollLast(String Msg, String Value, String LSL, String USL)
            {
                String[] strBufferARRAY;
                String strMsg, strValue, strLSL, strUSL;
                strBufferARRAY=this.Test.FormFFT.GetRowFromListBox(0);
                if(Msg.Length==0)
                    strMsg=strBufferARRAY[0];
                else
                    strMsg=Msg;
                if(Value.Length==0)
                    strValue=strBufferARRAY[1];
                else
                    strValue=Value;
                if (LSL.Length == 0)
                    strLSL = strBufferARRAY[2];
                else
                    strLSL = LSL;
                if (USL.Length == 0)
                    strUSL = strBufferARRAY[3];
                else
                    strUSL = USL;
                this.DeleteScroll(1);
                this.WriteToScroll(strMsg, strValue, strLSL, strUSL);
                return;
            }

            /// <summary>
            /// Fügt eine Zeile am Ende im Scrollfenster
            /// </summary>
            /// <param name="Msg">Message-Spalte</param>
	        public int WriteToScroll(String Msg)
            {

                return this.WriteToScroll(Msg, "", "", "");
            }

            /// <summary>
            /// Fügt eine Zeile am Ende im Scrollfenster
            /// </summary>
            /// <param name="Msg">Message-Spalte</param>
            /// <param name="Value">Value-Spalte</param>
            /// <param name="LSL">LSL-Spalte</param>
            /// <param name="USL">USL-Spalte</param>
            public int WriteToScroll(String Msg, String Value, String LSL, String USL)           
            {
                String strMsg, strLine = "", strText, strLineLast, strShift;
                int iLength, iModulo, iMax, i, iStart, iStartSearch, iIndex;
                bool bLoop;


                int iCountListViewFFT = this.Test.FormFFT.GetCountItemListViewFFT();
                
                strShift = "";
				if(CTestStep.FlagLayerOfTestStep > 0)
				{
					strShift = strShift.PadLeft(CTestStep.FlagLayerOfTestStep * 3);
				}
				else
				{
					if(CTestStep.FlagLayerOfTestStep < 0)
						CTestStep.FlagLayerOfTestStep = 0;
				}
                    
                strText = Msg;
                iStartSearch = 0;
                bLoop = true;
                while (bLoop)
                {
                    iIndex = strText.IndexOf("\r\n", iStartSearch);
                    if (iIndex != -1)
                    {
                        strMsg = strText.Substring(iStartSearch, iIndex - iStartSearch);
                    }
                    else
                    {
                        strMsg = strText.Substring(iStartSearch);
                        bLoop = false;
                    }

                    iLength = strMsg.Length;
                    if (iLength > this.Data.LengthScroll)
                    {
                        iModulo = iLength / this.Data.LengthScroll;
                        iMax = (iLength - iModulo) / this.Data.LengthScroll;
                        for (i = 0; i < iMax; i++)
                        {
                            iStart = i * (this.Data.LengthScroll);
                            strLine = strMsg.Substring(iStart, this.Data.LengthScroll);
                            this.WriteLineToReport(strShift+strLine);
                            this.Test.FormFFT.AddRowToListViewFFT(strShift+strLine);
                        }
                        if (iModulo != 0)
                        {
                            iStart = i * (this.Data.LengthScroll);
                            strLine = strMsg.Substring(iStart);
                        }
                    }
                    else
                        strLine = strMsg;

                    if (Value.Length != 0 || LSL.Length != 0 || USL.Length != 0)
                    {
                        strLineLast = strLine.PadRight(this.Data.LengthScroll, ' ');
                        strLineLast = strLineLast + " | " + Value + " | " + LSL + " | " + USL;
                    }
                    else
                        strLineLast = strLine;

                    this.WriteLineToReport(strShift+strLineLast);
                    this.Test.FormFFT.AddRowToListViewFFT(strShift+strLine, Value, LSL, USL);

                    iStartSearch = iIndex + 2;
                }

                return this.Test.FormFFT.GetCountItemListViewFFT() - iCountListViewFFT;
            }

            /// <summary>
            /// Fügt eine Zeile am Ende im Scrollfenster
            /// </summary>
            /// <param name="Msg">Message-Spalte</param>
            /// <param name="Mode">
            /// Unit-0 einfach zu scroll fenster schreiben
            /// Unit-1 Msg wird mit Stärnchen umgeben.
            /// Unit-2 Msg wird mit Minus umgeben.
            /// Unit-3 Msg wird mit Minus nachgefüllt.
            /// Unit-4 dem Msg werden Minus vorangestellt
            /// </param>
	        public void WriteToScroll(String Msg, int Mode)
	        {
	            String strMsg, strBuffer;
	            int iLengthRight, iLength;                

	            strMsg=Msg;
	            switch (Mode)
	            {
	            case 0:				
		            break;
	            case 1:	
		            if(strMsg.Length-2<this.Data.LengthScroll)
		            {
			            strMsg=" "+strMsg+" ";	
			            iLength=strMsg.Length;
			            iLengthRight=(this.Data.LengthScroll + iLength)/2;
			            strMsg=strMsg.PadRight(iLengthRight,'*');
			            strMsg=strMsg.PadLeft(this.Data.LengthScroll,'*');
		            }
		            break;			
	            case 2:		
		            if(strMsg.Length-2<this.Data.LengthScroll)
		            {
			            strMsg=" "+strMsg+" ";
			            iLength=strMsg.Length;
			            iLengthRight=(this.Data.LengthScroll + iLength)/2;
			            strBuffer=strMsg.PadRight(iLengthRight,'-');
			            strMsg=strBuffer.PadLeft(this.Data.LengthScroll,'-');
		            }
		            break;		
	            case 3:	
		            if(strMsg.Length-1<this.Data.LengthScroll)
		            {
			            strMsg=strMsg+" ";
			            strMsg=strMsg.PadRight(this.Data.LengthScroll,'-');
		            }
		            break;		
	            case 4:	
		            if(strMsg.Length-1<this.Data.LengthScroll)
		            {
			            strMsg=" "+strMsg;
			            strMsg=strMsg.PadLeft(this.Data.LengthScroll,'-');
		            }
		            break;		
	            }	
	            this.WriteToScroll(strMsg);

	            return;
            }



            public void DeleteStatus()
            {
                this.WriteToStatus("");
            }

            public void WriteToStatus(String Msg)
            {
                this.Test.FormFFT.WriteToStatus(Msg);
            }

	        public override bool Initialize()
            {
	            return(true);
            }

        
            //Wieviele Testschritte wurden von den anderen aufgerufen 
	        public static int FlagLayerOfTestStep=0;

            //Default - false; Wenn TestStep von CTest-Klasse (int CTest::Execute()) aufgerufen wird, dann true.
	        public bool FlagExecutedFromTest;

            //Default - true; TestStep wird von CTest-Klasse aufgerufen.
	        public bool FlagDoExecuteFromTest;

            // Argumente die von Sys-File übergeben werden können
            public string TestStepParameter { get; set; }

	        //public int ID;
            //TestStep ID 

            public int ID_BeakPoint;
            //BeakPoint ID 

            public String ID_String
            {
		        get
		        {
			        return(this.GetID(this.ID));
		        }
	        }

            /// <summary>
            /// Gibt via Indexer, die Msg für den Testschritt
            /// </summary>
            public CDataMsgTestStep Msg;

	        protected virtual void _SetTestStep(){return;}
	        protected virtual void _ExecuteTestStep(){return;}
	        protected virtual void _ResetTestStep(){return;}

            /// <summary>
            ///Wird im Konstrukter aufgerufen
            /// </summary>
            public new bool Create()
            {
                this.NameBreakpointLast = "";
	            this.FlagExecutedFromTest=false;
	            this.FlagDoExecuteFromTest=true;

                this.Msg = new CDataMsgTestStep(this, this.Data.Msg);                
            	
	            this.RemoveError();

	            return(true);
            }

            /// <summary>
            /// Wird nach jedem Testlauf ausgeführt in CTest.Execute()            
            /// </summary>
            public void Reset()
            {
                this.Error = "";
                this.ErrorCode = 0;
                this.NameBreakpointLast = "";
                this.ID_BeakPoint = 0;
            }
        }
        
    }
}

namespace Honeywell
{
    namespace Data
    {
        public class CDataMsgTestStep
        {
            public CDataMsgTestStep(CTestStep _TestStep, CDataMsg _Msg)
            {
                this.Msg = _Msg;
                this.TestStep = _TestStep;
            }
            private CTestStep TestStep = null;
            private CDataMsg Msg = null;

            public String this[int Index]
            {
                get
                {
                    return (this.Msg.GetNameMsg(this.TestStep.ID_String, Index));
                }
            }
        }
    }
}