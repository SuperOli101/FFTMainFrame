using System;
using Honeywell.Test;
using System.Collections.Generic;



namespace Honeywell
{
    namespace Data
    {
        /// <summary>
        /// In dieser Klasse werden die wichtigsten Datenklassen erstellt und die Referenzen auf die gespeichert. (CDataMsg, CDataReport, CMyDataIni)
        /// Die Klasse repräsentiert auch die Sys-Datei, wo die Debug und Test- Einstellungen und Testablauf für FFT gespeichert sind.
        /// </summary>
        public class CData : CDataIni
        {
            public static int OFF = 0;
            public static int ON = 1;            

            //[Test]
            public int Print;
            public int Scanner;
            public int FFTMode;
            public int LengthScroll=90;
            public int MenuSorted;
            public string Password; //					= centra2

            //[Directory]
            public string DirectoryOfErrorFile; //	= Error //Ohne Eintrag wird die ReportDatei im Fehlerfall nicht extra im Fehlerverzeichnis geschrieben
            public string DirectoryOfFFTSystem = null;//Ohne Eintrag vom DirectoryOfFFTSystem im Sys File wird in FFTSystem gesucht
            

            //[Debug]
            /// <summary>
            ///0-Debug mode ausgeschaltet 1-Debug mode ON (Mam kan arbeiten mit F10 und F5 Tasten die BreakPoints sind angeschaltet)
            /// </summary>
            public int ModeDebug;//						= 0	//1-An; 0-Aus            
            public int WriteBreakPointToScroll;//		= 0
            /// <summary>
            /// 1-Bei jedem Fehler wird angehalten, 0-nicht angehalten
            /// </summary>
            public int BreakPointAfterEveryError; 
            public bool BreakPointError;
            public String[] BreakPointErrorARRAY;
            public bool BreakPointID;
            public String[] BreakPointIDARRAY;

			//DB
            public bool DB_FlagCreate;//					= 1	//0-Aus 1-An
            public int DB_Report;//				= 1
            public string DB_ServerName;//				= ".\\SQLEXPRESS"
            public string DB_DataBaseName;//			= "PartProductionDB"
            public string DB_Login;//					= "" // "" - Windows Authentication
            public string DB_Password;//				= "" // "" - Windows Authentication           
			
			public string DB_Department;//								= "RS"
			public string DB_SiteCode;//								= "GE51"
            public string DB_SoftwareCategory;
            public string DB_DescriptionSoftwareCategory;
            public string DB_DescriptionFFTSoftware;
            public string DB_OrderName;

            public string COUNTER_FileName = null;

            public string OrderNameCurrent;


            public virtual Dictionary<String, String> GetExternalOSNumber(string  PathExternalOSNumber)
            {
                Dictionary<String, String> externalOSNumberDICTIONARY = new Dictionary<string, string>(1);
                
                return externalOSNumberDICTIONARY;
            }

            protected bool CreateData()
            {
                int iResult = 0;
                String strGroup, strMsg, strBuffer, strPathExternalOSNumber;
              
                if (this.DirectoryOfFFTSystem == null)
                {
                    if (!this.Read(this.NameFull))
                        return (false);
                }
                else
                {
                    if (!this.Read(System.IO.Path.Combine(this.DirectoryOfFFTSystem, this.Name)))
                        return (false);
                }

                if (!this.CreateInfoTestSequence())
                    return (false);

                if (!this.CreateInfoTestStep())
                    return (false);

                if (!this.CreateInfoVariant())
                    return (false);

                string strOSNumber, strSequencing, strVariant;
                Dictionary<String, String> externalOSNumberDICTIONARY;
                for (int i = 0; i < this.ID_MenuLIST.Count; i++)
                {
                    strOSNumber = this.ID_MenuLIST[i];
                    strSequencing = this.LinkID_MenuToSequencingLIST[i];
                    strVariant = this.LinkID_MenuToVariantLIST[i];
                    if (strOSNumber.Length == 0)
                    {
                        strPathExternalOSNumber = this.ID_VariantDICTIONARY[this.LinkID_MenuToVariantLIST[i]];
                        this.ID_MenuLIST.RemoveAt(i);
                        this.LinkID_MenuToSequencingLIST.RemoveAt(i);
                        this.LinkID_MenuToDescriptionSequencingLIST.RemoveAt(i);
                        this.LinkID_MenuToVariantLIST.RemoveAt(i);
                        i--;
                        if (strPathExternalOSNumber.Length != 0)
                        {
                            externalOSNumberDICTIONARY = this.GetExternalOSNumber(strPathExternalOSNumber);
                            if(externalOSNumberDICTIONARY == null)
                            {
                                return (false);
                            }
                            foreach (KeyValuePair<String, String> hOSNumber in externalOSNumberDICTIONARY)
                            {
                                this.ID_MenuLIST.Insert(i + 1, hOSNumber.Key);
                                this.LinkID_MenuToSequencingLIST.Insert(i + 1, strSequencing);
                                this.LinkID_MenuToDescriptionSequencingLIST.Insert(i + 1, hOSNumber.Value);
                                this.LinkID_MenuToVariantLIST.Insert(i + 1, strVariant);
                                i++;
                            }                            
                        }                        
                    }
                }
                
                //[Test]
                strGroup = "Test";
                iResult += this.GetValue(strGroup, "Print", out Print);
                iResult += this.GetValue(strGroup, "Password", out Password);                
                iResult += this.GetValue(strGroup, "LengthScroll", out LengthScroll);
                iResult += this.GetValue(strGroup, "FFTMode", out FFTMode);
                iResult += this.GetValue(strGroup, "Scanner", out Scanner);
                iResult += this.GetValue(strGroup, "MenuSorted", out MenuSorted);
                if (this.Test.ArgumentCommandLineFFTMode != null)
                {
                    int iBuffer;
                    iBuffer = this.Data.FFTMode;
                    try
                    {
                        this.FFTMode = Convert.ToInt16(this.Test.ArgumentCommandLineFFTMode);
                    }
                    catch
                    {
                        this.FFTMode = iBuffer;
                    }
                }
				

                //[DB]
                strGroup = "DB";
                iResult += this.GetValue(strGroup, "WriteToDB", out this.DB_FlagCreate);                    
                iResult += this.GetValue(strGroup, "Report", out this.DB_Report);
                iResult += this.GetValue(strGroup, "ServerName", out this.DB_ServerName);
                iResult += this.GetValue(strGroup, "DataBaseName", out this.DB_DataBaseName);
                iResult += this.GetValue(strGroup, "Login", out this.DB_Login);
                if (this.DB_Login.Length == 0 || this.DB_Login == "")
                    this.DB_Login = null;
                iResult += this.GetValue(strGroup, "Password", out this.DB_Password);
                if (this.DB_Password.Length == 0 || this.DB_Password == "")
                    this.DB_Password = null;

                iResult += this.GetValue(strGroup, "Department", out this.DB_Department);
                iResult += this.GetValue(strGroup, "SiteCode", out this.DB_SiteCode);
                iResult += this.GetValue(strGroup, "SoftwareCategory", out DB_SoftwareCategory);
                iResult += this.GetValue(strGroup, "DescriptionSoftwareCategory", out DB_DescriptionSoftwareCategory);
                iResult += this.GetValue(strGroup, "DescriptionFFTSoftware", out DB_DescriptionFFTSoftware);
                iResult += this.GetValue(strGroup, "OrderName", out DB_OrderName);	

                //[Counter]
                strGroup = "Counter";
                // if this is missing the counter class will not be initializied
                this.GetValue(strGroup, "FileName", out COUNTER_FileName);	

                //[Directory]
                strGroup = "Directory";
                iResult += this.GetValue(strGroup, "DirectoryOfErrorFile", out strBuffer);
                if (strBuffer.Length == 0)
                    this.DirectoryOfErrorFile = "";
                else
                {
                    this.DirectoryOfErrorFile = this.Path + "\\" + strBuffer;
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(this.DirectoryOfErrorFile);
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }
                }
                if (this.GetValue(strGroup, "DirectoryOfFFTSystem", out strBuffer) == 0)
                {
                    if (strBuffer.Length == 0)
                        this.DirectoryOfFFTSystem = this.Test.Path;
                    else
                    {
                        this.DirectoryOfFFTSystem = strBuffer;
                        //if (this.DirectoryOfFFTSystem.IndexOf(":\\") != 1)
                        if (!System.IO.Path.IsPathRooted(this.DirectoryOfFFTSystem))
                            this.DirectoryOfFFTSystem = System.IO.Path.Combine(this.Test.Path, this.DirectoryOfFFTSystem);
                    }
                }
                else
                {
                    this.Error = "";
                    this.DirectoryOfFFTSystem = null;
                }

                
                //[Debug]                
                strGroup = "Debug";
                iResult += this.GetValue(strGroup, "Mode",                      out ModeDebug);
                iResult += this.GetValue(strGroup, "WriteBreakPointToScroll",   out WriteBreakPointToScroll);
                iResult += this.GetValue(strGroup, "BreakPointAfterEveryError", out BreakPointAfterEveryError);
                iResult += this.GetValue(strGroup, "BreakPointError", out BreakPointErrorARRAY);
                if (iResult==0)
                {
                    if(Convert.ToInt32(BreakPointErrorARRAY[0])==0)
                        this.BreakPointError = false;
                    else
                        this.BreakPointError = true;
                }
                iResult += this.GetValue(strGroup, "BreakPointID", out BreakPointIDARRAY);
                if (iResult == 0)
                {
                    if (Convert.ToInt32(BreakPointIDARRAY[0]) == 0)
                        this.BreakPointID = false;
                    else
                        this.BreakPointID = true;
                }    

                if (iResult != 0)
                {
                    strMsg = String.Format("{0} items were not found (Sys-File)", iResult);
                    this.Error=strMsg;
                    return (false);
                }
                else
                    return (true);

            }
          

            public CData(String NameFull)
                : base(NameFull)
            {
                this.Create();
            }

            public virtual bool Initialize(int _Variant)
            {

				

                return (true);
            }

            public override bool Initialize()
            {                
                this.CountResourceForInstalling = 4;

                this.AddRessource("Sys-File");
                if(!this.CreateData())
                    return(false);
                if (this.Data.DirectoryOfFFTSystem != null)
                {
                    if (!this.CreateData())
                        return (false);
                }

                this.AddRessource("Report-File");
                if(!this.CreateReport())
                    return(false);

                this.AddRessource("Msg-File");
                if(!this.CreateMsg())
                    return(false);

                /*
                String strNameFullDBF;
                strNameFullDBF = this.Path + "\\" + this.NameShort + ".dbf";
                if (!this.CreateDBF(strNameFullDBF, ref this.DBF))
                    return (false);
                */

	
				//Variantenauswahlmenu starten
				if(!this.setVariant())
					return false;

                this.AddRessource("Count-File");
                if (!this.CreateCounter())
                    return false;                

                return (true);
            }

			private bool setVariant()
			{
				int i;
				//Befehlszeilenargumente auswerten (OS-Number, oder Position in Menü von 1 angefangen)/////////////////////////////////////////
                if (this.Test.ArgumentCommandLineCurrentVariant != null && this.Test.ArgumentCommandLineCurrentVariant.Length != 0)
				{
					//OS-Number //////////////////
					i=1;
					foreach(string strVariant in this.ID_MenuLIST)
					{						
						if(strVariant==this.Test.ArgumentCommandLineCurrentVariant)
						{
							this.Test.CurrentID_Menu = i;
							break;
						}
						i++;
					}
					////////////////////////////////////					
					if(this.Test.CurrentID_Menu == 0)
					{
						//Position in Menü//////////////////
						try
						{
							i = Convert.ToInt32(this.Test.ArgumentCommandLineCurrentVariant);
						}
						catch
						{
							i = 0;
						}
						if(i < this.Data.ID_MenuLIST.Count + 1)
						{
							this.Test.CurrentID_Menu = i;
						}
						////////////////////////////////////
					}																									 															
				}
				/////////////////////////////////////////////////////////////////////////////

				//Variantenauswahlmenu starten ////////////////////////////////////////				
				if(this.Test.CurrentID_Menu == 0)
				{
					this.HideInstalRessource();
					if(this.Data.ID_MenuLIST.Count == 1)
						this.Test.CurrentID_Menu = 1;
					else
						this.Test.SetVariant();//Hier wird Variantenauswahlmenu gestartet


					if(this.Test.CurrentID_Menu == 0)
					{
						this.ShowInstalRessource();
						return false;
					}
					this.ShowInstalRessource();
				}				
				/////////////////////////////////////////////////////////////////////////////

				//Alle Alias zu current Varian eintragen ////////////////////////////////////////////////////             
				this.Test.CurrentID_MenuWithAllAliasARRAY[0] = this.Test.CurrentID_Menu.ToString();							//Position in der Menü von 1 angefangen
				this.Test.CurrentID_MenuWithAllAliasARRAY[1] = this.ID_MenuLIST[this.Test.CurrentID_Menu - 1];				//OS-Number
				this.Test.CurrentID_MenuWithAllAliasARRAY[2] = this.LinkID_MenuToVariantLIST[this.Test.CurrentID_Menu - 1];	//Variantenname
				/////////////////////////////////////////////////////////////////////////////

				return true;

			}

            public int DBFColumnFirstError;
            public int DBFColumnMeasValue;
            public int DBFColumnLSL;
            public int DBFColumnUSL;
            public int DBFColumnDBFColumnOfMeasValue;

            public CDataMsg Msg;
            public CDataReport Report;
            public CDataDBF DBF;
            public CPartProductionDBforFFT DB = null;
            public CDataCounter Counters = null;
            
            /// <summary>
            ///Testreihen mit allen Testschritten in der Reihe
            /// </summary>
            public Dictionary<String, Dictionary<String, bool>> SequencingDICTIONARY;

            /// <summary>
            ///Zuordnung ID_Menu zu Testreihen
            /// </summary>
            public List<String> LinkID_MenuToSequencingLIST;
			public List<String> LinkID_MenuToDescriptionSequencingLIST;

            /// <summary>
            ///Zuordnung ID_Menu zu ID_VariantString
            /// </summary>
            public List<String> LinkID_MenuToVariantLIST;

            /// <summary>
            ///Zuordnung ID_MenuString zu ID_Menu
            /// </summary>
            public List<String> ID_MenuLIST;

            /// <summary>
            ///Zuordnung ID_VariantString zu ID_Variant
            /// </summary>
            public List<String> ID_VariantLIST;
            /// <summary>
            /// Zuordnung Varianten mit zusätzlicher Datei mit OS-Number(Extern) 
            /// </summary>
            public Dictionary<String, String> ID_VariantDICTIONARY;

            /// <summary>
            ///Zuordnung ID_TestStepString zu ID_TestStep
            /// </summary>
            public List<String> ID_TestStepLIST;

            /// <summary>
            ///Test abrechen bei ErrorID (mit Komma getrennt), False bei jedem Error abbrechen
            ///-1 - Bei jedem Fehler wird der Test abgebrochen
            /// 0 - Test wird nach diesem Testschritt nicht abgebrochen
            ///1,2,3... Fehler die zum Testabbruch führen
            /// </summary>
            public List<int[]> FlagTestStepCancelTestIfErrorID_LIST;

            /// <summary>
            ///true - wird nur dann ausführen, wenn bis jetzt kein Error im Test aufgetretten ist	
            ///false - wird immer ausführen
            /// </summary>
            public List<bool> FlagTestStepRunOnlyIfGoodLIST;


            public new bool Create()
            {
                this.Msg = null;
                this.Report = null;

                this.Data = this;


                this.SequencingDICTIONARY = null;
                this.LinkID_MenuToSequencingLIST = null;
				this.LinkID_MenuToDescriptionSequencingLIST = null;
                this.LinkID_MenuToVariantLIST = null;
                this.ID_MenuLIST = null;
                this.ID_VariantLIST = null;
                this.ID_TestStepLIST = null;
                this.FlagTestStepCancelTestIfErrorID_LIST = null;
                this.FlagTestStepRunOnlyIfGoodLIST = null;

                this.DBFColumnFirstError = 0;
                this.DBFColumnMeasValue = 0;
                this.DBFColumnLSL = 0;
                this.DBFColumnUSL = 0;
                this.DBFColumnDBFColumnOfMeasValue = 0;

                return (true);
            }
 
            

            protected bool CreateMsg()
            {
	            String strNameFull;

                if (this.DirectoryOfFFTSystem == null)
                {
                    strNameFull = this.Path + "\\" + this.NameShort + ".msg";
                }
                else
                {
                    strNameFull = System.IO.Path.Combine(this.DirectoryOfFFTSystem, this.NameShort + ".msg");
                }

	            
	            this.Msg=new CDataMsg(strNameFull);	
	            if(!this.Msg.Initialize())
	            {
		            this.Error=this.Msg.Error;
		            return(false);
	            }	

	            return(true);
            }

            protected bool CreateDBF()
            {
                String strNameFull;
                strNameFull = this.Path + "\\" + this.NameShort + ".dbf";

                this.DBF = new CDataDBF(strNameFull);
                if (this.DBF.GetError()!=0)
                {
                    this.Error=this.DBF.Error;
                    return (false);
                }
                
                return (true);
            }

            protected bool CreateDBF(String _NameFullDBF, ref CDataDBF _RefDBF)
            {                
                _RefDBF = new CDataDBF(_NameFullDBF);
                if (_RefDBF.GetError() != 0)
                {
                    this.Error=_RefDBF.Error;
                    return (false);
                }
                
                return (true);
            }

            protected bool CreateDB()
            {
                String strNameFull;
                strNameFull = this.Path + "\\" + this.NameShort + ".msg";

                this.Msg = new CDataMsg(strNameFull);
                if (!this.Msg.Initialize())
                {
                    this.Error=this.Msg.Error;
                    return (false);
                }

                return (true);
            }

            protected bool CreateReport()
            {
	            String strNameFull;
	            strNameFull=this.Path+"\\"+this.NameShort+".log";
            	
	            this.Report=new CDataReport(strNameFull);
                if (!this.Report.Initialize())
                {
                    this.Error = this.Report.Error;
                    return (false);
                }
                else
                {
                    this.Report.WriteToFileOverwrite(this.Report.NameFull);
                }

	            return(true);
            }

            protected bool CreateCounter()
            {
                if(!String.IsNullOrWhiteSpace(this.COUNTER_FileName))
                {
                    String strNameFullPrototype;
                    strNameFullPrototype = this.Path + "\\" + this.COUNTER_FileName;

                    this.Counters = new CDataCounter(strNameFullPrototype, 
                        this.Test.CurrentID_MenuWithAllAliasARRAY[2], 
                        this.Test.CurrentID_MenuWithAllAliasARRAY[1]);
                }

                return true;
            }

            protected bool CreateInfoTestSequence()
            {
	            int iMax=0, iIndex;
	            String strItem="",  strValue="", strMsg;
	            String strRunLike, strVariante, strID_Menu;
	            String strTestSequence="";
	            Dictionary<String, Dictionary<String, String>> hGroup;
	            Char[] chSignTrim={'"'};
	            bool bResult;
				string[] strTestSequenceItemARRAY;
	            Dictionary<String, bool> hTestStepDICTIONARY;
	            List<String>		strFlagSequenceMarkedLIST=null;

	            bResult=true;
	            try
	            {		           
		            hGroup=this.IniStructuredDICTIONARY["Menu"];
		            iMax=hGroup.Count;
                    this.ID_MenuLIST = new List<String>(iMax);
                    this.LinkID_MenuToSequencingLIST = new List<String>(iMax);
					this.LinkID_MenuToDescriptionSequencingLIST = new List<String>(iMax);
		            this.LinkID_MenuToVariantLIST=new List<String>(iMax);
		            strFlagSequenceMarkedLIST=new List<String>(iMax);


		            foreach(KeyValuePair<String, Dictionary<String, String>> hItem in hGroup)
		            {
			            foreach(KeyValuePair<String, String> hVariant in hItem.Value)
			            {
				            //strGroup=hGroup.Key;
				            //strVariant=hVariant.Key;
				            strItem=hItem.Key;
				            strValue=hVariant.Value;

							strTestSequenceItemARRAY = strValue.Split(new Char[] { '=' });

							strRunLike = strTestSequenceItemARRAY[0];
				            strRunLike = strRunLike.Trim();
							strVariante = strTestSequenceItemARRAY[1];
				            strVariante = strVariante.Trim();
							strID_Menu = strTestSequenceItemARRAY[2];
				            strID_Menu = strID_Menu.Trim();
							strID_Menu = strID_Menu.Trim(new Char[] { '"' });

                            this.ID_MenuLIST.Add(strID_Menu);	
				            this.LinkID_MenuToVariantLIST.Add(strVariante);
				            if(strRunLike.Length==0)
                                this.LinkID_MenuToSequencingLIST.Add(strItem);
				            else
                                this.LinkID_MenuToSequencingLIST.Add(strRunLike);
							if(strTestSequenceItemARRAY.Length > 3)
							{
								strMsg = strTestSequenceItemARRAY[3].Trim();
								this.LinkID_MenuToDescriptionSequencingLIST.Add(strMsg.Trim(new Char[] { '"' }));
							}
							else
								this.LinkID_MenuToDescriptionSequencingLIST.Add("");
            					

			            }
		            }
	            }
	            catch (Exception e)
	            {
		            bResult=false;
		            strMsg=String.Format("Error while processing [Menu] -> {0} -> {1}\r\n{2}",strItem,strValue,e.Message);
		            this.Error=strMsg;
	            }
	            finally
	            {

	            }
	            if(bResult)
	            {
		            try
		            {

			            this.SequencingDICTIONARY = new Dictionary<String, Dictionary<String, bool>>(iMax);

                        iMax = this.LinkID_MenuToSequencingLIST.Count;
                        foreach (String strTestSequenceTemp in this.LinkID_MenuToSequencingLIST)
			            {
                            strTestSequence=strTestSequenceTemp;
				            hGroup=this.IniStructuredDICTIONARY[strTestSequence];				
				            if(strFlagSequenceMarkedLIST.Count!=0)
				            {
					            iIndex=strFlagSequenceMarkedLIST.IndexOf(strTestSequence);
					            if(iIndex!=-1)
						            continue;
				            }
				            strFlagSequenceMarkedLIST.Add(strTestSequence);
				            iMax=hGroup.Count;
				            hTestStepDICTIONARY = new Dictionary<String, bool>(iMax);
				            this.SequencingDICTIONARY.Add(strTestSequence,hTestStepDICTIONARY);
				            foreach(KeyValuePair<String, Dictionary<String, String>> hItem in hGroup)
				            {
					            foreach(KeyValuePair<String, String> hVariant in hItem.Value)
					            {
						            strItem=hItem.Key;
						            strValue=hVariant.Value;
						            if(strValue.Length==0)
							            hTestStepDICTIONARY.Add(strItem,true);
						            else
							            hTestStepDICTIONARY.Add(strItem,false);
					            }
				            }
			            }
            			
		            }
		            catch (Exception e)
		            {
			            bResult=false;
			            strMsg=String.Format("Testsequence {0} is not described\r\n{1}",strTestSequence,e.Message);
			            this.Error=strMsg;
		            }
		            finally
		            {

		            }		
	            }
            	
	            return(bResult);
            }

            protected bool CreateInfoTestStep()
            {
	            int iMax, iIndex, i;
	            String strBuffer, strItem="", strValue="", strCancelTestIfErrorID, strRunTestStepOnlyIfGood, strMsg;
	            Dictionary<String, Dictionary<String, String>> hGroup=null;
	            String[]    strBufferARRAY=null;
	            int[]       intBufferARRAY=null;	
	            Char[]      chSignSplit={','};
	            bool bResult, bBuffer;

	            bResult=true;
	            try
	            {		            
		            hGroup=this.IniStructuredDICTIONARY["TestSteps"];
		            iMax=hGroup.Count;
                    this.ID_TestStepLIST = new List<String>(iMax);
		            this.FlagTestStepCancelTestIfErrorID_LIST=new List<int[]>(iMax);
		            this.FlagTestStepRunOnlyIfGoodLIST=new List<bool>(iMax);

		            foreach(KeyValuePair<String, Dictionary<String, String>> hItem in hGroup)
		            {
			            foreach(KeyValuePair<String, String> hVariant in hItem.Value)
			            {
				            //strGroup=hGroup.Key;
				            //strVariant=hVariant.Key;
				            strItem=hItem.Key;
				            strValue=hVariant.Value;

                            this.ID_TestStepLIST.Add(strItem);							
            				
				            iIndex=strValue.IndexOf("=");
				            if(iIndex==-1)
				            {
					            bBuffer=false;//TestStep wird immer ausgeführt					           
                                intBufferARRAY=new int[1];//Keine Errors, die zum Testabbruch führen
                                intBufferARRAY[0]=0;
				            }
				            else
				            {
					            strCancelTestIfErrorID=strValue.Substring(0,iIndex);
					            strCancelTestIfErrorID.Trim();
					            if(strCancelTestIfErrorID.IndexOf("FALSE")!=-1 || strCancelTestIfErrorID.IndexOf("False")!=-1 || strCancelTestIfErrorID.IndexOf("false")!=-1)
					            {												
						            intBufferARRAY=new int[1];//Jeder Error, führt zum Testabbruch
                                    intBufferARRAY[0]=-1;
					            }
					            else
					            {
						            strBufferARRAY=strCancelTestIfErrorID.Split(chSignSplit);
						            iMax=strBufferARRAY.Length;
						            intBufferARRAY=new int[iMax];
						            for(i=0;i<iMax;i++)
						            {
							            strBuffer=strBufferARRAY[i];
							            if(strBuffer.Length==0)
							            {
								            intBufferARRAY[i]=0;//Keine Errors, die zum Testabbruch führen
							            }
							            else
							            {
								            intBufferARRAY[i]=Convert.ToInt32(strBuffer);//Errors, die zum Testabbruch führen
							            }
						            }
					            }				
            			
					            strRunTestStepOnlyIfGood=strValue.Substring(iIndex+1);
					            strRunTestStepOnlyIfGood.Trim();
					            if(strRunTestStepOnlyIfGood.IndexOf("TRUE")!=-1 || strRunTestStepOnlyIfGood.IndexOf("True")!=-1 || strRunTestStepOnlyIfGood.IndexOf("true")!=-1)
					            {
						            bBuffer=true;//TestStep wird nur ausgeführt, wenn Testergebnis bis jetzt gut war.
					            }
					            else
					            {
						            bBuffer=false;//TestStep wird immer ausgeführt
					            }					
				            }

				            this.FlagTestStepCancelTestIfErrorID_LIST.Add(intBufferARRAY);
				            this.FlagTestStepRunOnlyIfGoodLIST.Add(bBuffer);
			            }
		            }
	            }
	            catch (Exception e)
	            {
		            bResult=false;
		            strMsg=String.Format("Error while processing [TestSteps] -> {0} -> {1}\r\n{2}",strItem,strValue,e.Message);
		            this.Error=strMsg;
	            }
	            finally
	            {

	            }
	            return(bResult);
            }

            protected bool CreateInfoVariant()
            {
	            bool bResult;
	            int iMax;
	            Dictionary<String, Dictionary<String, String>> hGroup;
	            String strItem="",  strValue="", strMsg;

	            bResult=true;
	            try
	            {		            
		            hGroup=this.IniStructuredDICTIONARY["Variants"];
		            iMax=hGroup.Count;

                    this.ID_VariantLIST = new List<String>(iMax);
                    this.ID_VariantDICTIONARY = new Dictionary<string, string>(iMax);
            		

		            foreach(KeyValuePair<String, Dictionary<String, String>> hItem in hGroup)
		            {
			            foreach(KeyValuePair<String, String> hVariant in hItem.Value)
			            {
				            strItem=hItem.Key;
				            strValue=hVariant.Value;

                            this.ID_VariantLIST.Add(strItem);
                            this.ID_VariantDICTIONARY.Add(strItem, strValue);
			            }
		            }
	            }
	            catch (Exception e)
	            {
		            bResult=false;
		            strMsg=String.Format("Error while processing [Variants] -> {0} -> {1}\r\n{2}",strItem,strValue,e.Message);
		            this.Error=strMsg;
	            }
	            finally
	            {

	            }
	            return(bResult);
            }

            public void WriteLineToReport(string Msg, int Mode, int ShiftReport)
            {
                int iShiftTemp = this.ShiftReport;
                this.ShiftReport = ShiftReport;
                this.WriteLineToReport(Msg, Mode);
                this.ShiftReport = iShiftTemp;

                return;
            }

            //
            //	BOOL Initialize();
            //	BOOL Initialize(int Variant);
            //	BOOL Deinstall();
            //	BOOL Deinstall(int Variant);
            //	
            //	
            //
            //
            //
            //	BOOL CreateIni();
            //	BOOL CreateCal();
            //	BOOL CreateTemp();
            //	BOOL CreateLimit();
            //	BOOL CreateMsg();
            //	BOOL CreateReport();	
            //	BOOL CreateDBF();
            //	BOOL CreateDBF(LPCTSTR NameOfDBF, CDBF** DataBase);
            //	//BOOL CreateDBF(LPCTSTR NameOfDBF, LPCTSTR NameOfDBF_2);
            //	//BOOL CreateDBF(LPCTSTR NameOfDBF, CDBF_Data** DBF);
            //	BOOL CreateTelegram();
            //
            //	
            //	//BOOL CreateDBFError();
            //	
            //public:
            //
            //
            //	
            //	CMsgFile*		m_pMsg;
            //	CIniData*		m_pIni;
            //	CCalData*		m_pCal;
            //	CTempData*		m_pTemp;
            //
            //
            //	CDBF*		m_pDBF;
            //	CDBF*		m_pDBF_2;
            //	CDBF*		m_pDBF_3;
            //	CDBF*		m_pDBF_ID;
            //	CDBF*		m_pDBF_LED;
            //	CDBF*		m_pDBF_GUID;
            //
            //	/*
            //	CDBF_Data*		m_pDBF;
            //	CDBF_Data*		m_pDBF_ID;
            //	*/
            //	
            //	
            //	//CDBF_Data		m_DBFError;
            //
            //	CReport*		m_pReport;
            //	CLimitData*		m_pLimit;
            //
            //	//CTelegramm*		m_pTelegram;	
            //	//CGPIBCommand*	m_pGPIB;
        }

    }
}
 
              
