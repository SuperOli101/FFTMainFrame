using System;
using System.IO;
using Honeywell.Test;
using System.Collections.Generic;



namespace Honeywell
{
    namespace Data
    {
        /// <summary>
        /// Mit der Klasse wird eine Msg-Datei eingelesen.
        /// </summary>
        public class CDataMsg : CDataIni
        {			   
	        public CDataMsg(String NameFull):base(NameFull)
	        {
		        this.Create();
	        }

            /// <summary>
            /// Holen die globalen Messages
            /// </summary>
	        public String this[int Index]
	        {
		        get
		        {
			        return(this.GetNameMsg("General",Index));
		        }
	        }


            /// <summary>
            /// Liest die Msg-Datei in die Variablen MsgDICTIONARY, ErrorDICTIONARY, NameTestStepDICTIONARY ein
            /// </summary>
            public override bool Initialize()           
			{
	            bool bResult;
	            String strCode="", strVariant, strName="", strMsg, strCodeTestStepString="", strBuffer;
	            int iCountGroup, iCountItem, iIndex, iCode;
                Dictionary<int, String> hNameCodeError, hNameCodeMsg, hNameCodeErrorPrint;
	            Char[] chSignTrim = {'\"'};
            			
	            bResult=true;
	            try
	            {
		            if(!this.Read(this.NameFull))
		            {
			            throw new Exception(this.Error);
		            }

		            iCountGroup=this.IniStructuredDICTIONARY.Count;
		            this.NameTestStepDICTIONARY	= new Dictionary<String, String>(iCountGroup-1);//ohne [General]
		            this.MsgDICTIONARY			= new Dictionary<String, Dictionary<int, String>>(iCountGroup);
		            this.ErrorDICTIONARY		= new Dictionary<String, Dictionary<int, String>>(iCountGroup);
                    this.ErrorPrintDICTIONARY   = new Dictionary<String, Dictionary<int, String>>(iCountGroup);

		            foreach(KeyValuePair<String, Dictionary<String, Dictionary<String, String>>> cGroup in this.IniStructuredDICTIONARY)
		            {
			            strCodeTestStepString=cGroup.Key;	
			            iCountItem=cGroup.Value.Count;
            			
			            hNameCodeMsg = new Dictionary<int, String>(iCountItem);
			            this.MsgDICTIONARY.Add(strCodeTestStepString,hNameCodeMsg);
			            hNameCodeError = new Dictionary<int, String>(iCountItem);
			            this.ErrorDICTIONARY.Add(strCodeTestStepString,hNameCodeError);
                        hNameCodeErrorPrint = new Dictionary<int, String>(iCountItem);
                        this.ErrorPrintDICTIONARY.Add(strCodeTestStepString, hNameCodeErrorPrint);
			            foreach(KeyValuePair<String, Dictionary<String, String>> cItem in cGroup.Value)
			            {
                            if (strCodeTestStepString == "DescriptionMeasurement")
                            {
                                if (this.DescriptionMeasurementDICTIONARY == null)
                                {
                                    this.DescriptionMeasurementDICTIONARY = new Dictionary<string, string>(500);
                                }
                                foreach (KeyValuePair<String, String> cVariant in cItem.Value)
                                {
                                    strCode = cItem.Key;
                                    strName = cVariant.Value.Trim(chSignTrim);
                                    this.DescriptionMeasurementDICTIONARY.Add(strCode, strName);
                                }  
                            }
                            else
                            {
                                strCode = cItem.Key;
                                foreach (KeyValuePair<String, String> cVariant in cItem.Value)
                                {
                                    strName = cVariant.Value.Trim(chSignTrim);
                                    if (strCode == "Name")
                                    {
                                        this.NameTestStepDICTIONARY.Add(strCodeTestStepString, strName);
                                    }
                                    else
                                    {
                                        iIndex = strCode.IndexOf(":");//Error Code
                                        if (iIndex != -1)
                                        {
                                            strBuffer = strCode.Substring(iIndex + 1);
                                            iCode = Convert.ToInt32(strBuffer);
                                            hNameCodeError.Add(iCode, strName);
                                        }
                                        else
                                        {
                                            iIndex = strCode.IndexOf("#");//Error Codes for Printer
                                            if (iIndex != -1)
                                            {
                                                strBuffer = strCode.Substring(iIndex + 1);
                                                iCode = Convert.ToInt32(strBuffer);
                                                hNameCodeErrorPrint.Add(iCode, strName);
                                            }
                                            else//Messages
                                            {
                                                iCode = Convert.ToInt32(strCode);
                                                hNameCodeMsg.Add(iCode, strName);
                                            }
                                        }
                                    }

                                    strVariant = cVariant.Key;
                                }
                            }
			            }
		            }		
	            }
	            catch(Exception e)
	            {
		            bResult=false;
		            strMsg=e.Message;
		            if(strMsg.Length!=0)
		            {
			            strMsg=String.Format("Error while reading Msg-File ({4}). ([{0}] {1} {2})\r\n{3}",strCodeTestStepString,strCode,strName,e.Message, this.Name);
			            this.Error=strMsg;
		            }
	            }
	            finally
	            {

	            }

               
	            return(bResult);
            }



    
            /// <summary>
            /// Gibt das Error Message vom Error-Code zurück
            /// </summary>
            /// <param name="ID_TestStepString">TestStep-ID in String Format.</param>
            /// <param name="ID_Error">Error ID.</param>            
            /// <returns>Error Message.</returns>
            public String GetNameError(String ID_TestStepString, int ID_Error)
            {
	            String strName;                
	            try
	            {
		            strName=this.ErrorDICTIONARY["General"][ID_Error];		
	            }
	            catch (Exception e)
	            {	
		            strName=e.Message;
		            try
		            {			
			            strName=this.ErrorDICTIONARY[ID_TestStepString][ID_Error];								
		            }
		            catch (Exception e2)
		            {
			            strName=e2.Message;
			            strName=String.Format("Not defined error code [{0}] {1}",ID_TestStepString,ID_Error);
		            }
	            }
            	
	            return(strName);
            }


            /// <summary>
            /// Gibt den Namen vom Error-Code für Den Drucker zurück. Gibt es keinen ErrorNamen für Drucker, wird den Errornamen für Display ausgegeben
            /// </summary>
            public String GetNameErrorPrint(String ID_TestStepString, int ID_Error)
            {
                String strName;

                try
                {
                    strName = this.ErrorPrintDICTIONARY["General"][ID_Error];
                }
                catch (Exception e)
                {
                    strName = e.Message;
                    try
                    {
						strName = this.ErrorPrintDICTIONARY[ID_TestStepString][ID_Error];
                    }
                    catch (Exception e2)
                    {
                        strName = e2.Message;
                        strName = this.GetNameError(ID_TestStepString, ID_Error);
                    }
                }

                return (strName);
            }

            /// <summary>
            /// Gibt den Namen vom Message-Code zurück
            /// </summary>
            public String GetNameMsg(String ID_TestStepString, int ID_Msg)
            {
	            String strName;

	            try
	            {
		            strName=this.MsgDICTIONARY[ID_TestStepString][ID_Msg];		
	            }
	            catch (Exception e)
	            {
		            strName=e.Message;
		            strName=String.Format("Not defined message [{0}] {1}",ID_TestStepString,ID_Msg);				
	            }

	            return(strName);
            }

            /// <summary>
            /// Gibt den Namen vom TestStep zurück
            /// </summary>
            public String GetNameTestStep(String ID_TestStepString)
            {
	            String strName;

	            try
	            {
		            strName=this.NameTestStepDICTIONARY[ID_TestStepString];		
	            }
	            catch (Exception e)
	            {
		            strName=e.Message;
		            strName=String.Format("{0}",ID_TestStepString);				
	            }

	            return(strName);
            }


            /// <summary>
            /// Wird im Konstrukter aufgerufen
            /// </summary>
            public new bool Create()
            {
                this.NameTestStepDICTIONARY = null;
                this.MsgDICTIONARY = null;
                this.ErrorDICTIONARY = null;
                this.ErrorPrintDICTIONARY = null;

                return (true);
            }								


            /// <summary>
            /// Ermöglicht den Zugriff auf die globale und locale Messages
            /// </summary>
	        public Dictionary<String, Dictionary<int, String>>	MsgDICTIONARY;

            /// <summary>
            /// Ermöglicht den Zugriff auf die globale und locale Errors
            /// </summary>											                    
	        public Dictionary<String, Dictionary<int, String>>	ErrorDICTIONARY;

            /// <summary>
            /// Ermöglicht den Zugriff auf die globale und locale Errors für den Drucker
            /// </summary>
            public Dictionary<String, Dictionary<int, String>> ErrorPrintDICTIONARY;
 
            /// <summary>
            /// Ermöglicht den Zugriff auf die Namen von TestSteps
            /// </summary>								                    
	        public Dictionary<String, String>					NameTestStepDICTIONARY;

            /// <summary>
            /// Beschreibt  die DB-Felder von CMyData.DBFFields
            /// </summary>
            public Dictionary<String, String> DescriptionMeasurementDICTIONARY = null;    																                   
        }
    }
}                    
  
              
