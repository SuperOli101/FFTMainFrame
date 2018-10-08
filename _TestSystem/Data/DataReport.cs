using System;
using System.IO;
using Honeywell.Test;
using System.Text;
using System.Diagnostics;

namespace Honeywell
{
    namespace Data
    {
        /// <summary>
        /// Mit der Klasse wird eine Log-Datei erstellt (Nach der Datei-Erstellung die Verbindung zur Datei wird geschlossen)
        /// </summary>
        public class CDataReport : CObjectData
        {						        
	        public CDataReport(String NameFull):base(NameFull)
	        {
		        this.Create();
	        }
        	            
            /// <summary>
            ///Schreibt Msg in den Buffer "Buffer" kein \r\n nur angehängt, nur wenn FlagReportOn=1
            /// </summary>
	        public void Write(String Msg)
            {
                this.Write(Msg, 0);
                return;
            }

            /// <summary>
            ///Schreibt Msg in den Buffer "m_strBuffer" kein \r\n nur angehängt,	nur wenn FlagReportOn=1
	        ///Unit-0 einfach anhängen 
            ///Unit-1 . wird durch , ersetzt	und einfach anhängen	
            /// </summary>
	        public void Write(String Msg, int Mode)
            {
	            String strMsg;

	            if(this.FlagReportOn==1)
	            {
		            strMsg=Msg;
		            switch (Mode)
		            {
                        case 0:
                            break;	
		                case 1:
			                strMsg=strMsg.Replace(".",",");
                            break;                    	            		
		            }	
		            this.Buffer.Append(strMsg);
					Debug.Write(Msg);
	            }
	            return;
            }

            /// <summary>
            ///Schreibt Msg in den Buffer "Buffer" und hängt \r\n an, nur wenn FlagReportOn=1
            /// </summary>
            public void WriteLine(String Msg)
           {

               if (this.FlagReportOn == 1)
               {
                   this.Buffer.AppendFormat("{0}\r\n", Msg);
				   Debug.WriteLine(Msg);
               }
               return;
           }




            
            /// <summary>
            ///Schreibt Msg in den Buffer "Buffer" und hängt \r\n an, nur wenn FlagReportOn=1
            ///Mode-0 wie void WriteLine(String Msg);
            ///Mode-1 . wird durch , ersetzt und dann wie void WriteLine(String Msg);		
            /// </summary>           
            /// <param name="Msg"></param>
            /// <param name="Mode"></param>
            public void WriteLine(String Msg, int Mode)
            {
                String strMsg;

                if(this.FlagReportOn==1)
                {
                    strMsg=Msg;
                    switch (Mode)
                    {
                        case 0:
                            break;
                        case 1:
                            strMsg=strMsg.Replace(".",",");
                            break;                   
                    }	
                    this.WriteLine(strMsg);
                }
                return;
            }

            /// <summary>
            ///Löscht den Inhalt von Buffer
            /// </summary>
            public void ClearBuffer()							
            {
                this.Buffer = new StringBuilder(this.Capacity);
                //this.Buffer.Clear();
                return;
            }

            /// <summary>
            ///Schreibt von Buffer in die Datei (anhängend)
            /// </summary>
            public bool WriteToFileAppend(String NameFull)	
           {
	            bool  bResult;
	            String strMsg, strName;
            	
	            if(!this.GetName(NameFull,out strName))
		            return(false);

	            try
	            {
		            bResult=true;
                    //Öffnet eine Datei, fügt die angegebene Zeichenfolge an die Datei an und schließt dann die Datei.
                    File.AppendAllText(NameFull, this.Buffer.ToString(), System.Text.Encoding.UTF8);

	            }
	            catch(Exception e)
	            {
		            bResult=false;
		            strMsg=String.Format("Error while writing the file {0}.\r\n{1}",strName,e.Message);
		            this.Error=strMsg;
	            }
	            finally
	            {
                    this.ClearBuffer();
	            }
            	
	            return(bResult);
            }

            /// <summary>
            ///Schreibt von Buffer in die Datei (überschreibend)
            /// </summary>
            public bool WriteToFileOverwrite(String NameFull)
           {
	            bool  bResult;
	            String strMsg, strName;
	            FileStream hFileStream=null;
	            StreamWriter hStreamWriter=null;

	            if(!this.GetName(NameFull,out strName))
		            return(false);

	            try
	            {
		            bResult=true;
		            //Write m_strIniTextFileARRAY in die Ini Datei //////////////////////////////////////////////
		            hFileStream=new FileStream(NameFull,FileMode.Create);
                    hStreamWriter = new StreamWriter(hFileStream, System.Text.Encoding.UTF8);
                    
		            hStreamWriter.Write(this.Buffer.ToString());
		            //////////////////////////////////////////////

	            }
	            catch(Exception e)
	            {
		            bResult=false;
		            strMsg=String.Format("Error while writing the file {0}.\r\n{1}",strName,e.Message);
		            this.Error=strMsg;
	            }
	            finally
	            {
                    this.ClearBuffer();
		            if(hStreamWriter!=null)
			            hStreamWriter.Close();
	            }

	            return(bResult);
            }

            /// <summary>
            ///Löscht die Datei mit dem Namen -> NameFull
            /// </summary>
            public bool Remove(String NameFull)				
           {
	            bool  bResult;
	            String strMsg, strName;

	            if(!this.GetName(NameFull,out strName))
		            return(true);

	            try
	            {
		            bResult=true;
		            // Delete the file if it exists.
		            if(File.Exists(NameFull))
		            {
			            File.Delete(NameFull);
		            }
	            }
	            catch(Exception e)
	            {
		            bResult=false;
		            strMsg=String.Format("Error while removing the file {0}.\r\n{1}",strName,e.Message);
		            this.Error=strMsg;
	            }
            	
            	
	            return(bResult);
            }

            /// <summary>
            ///Alles wird zuerst in die variable Buffer geschrieben und danach in die datei
            /// </summary>
            public StringBuilder Buffer;
            //private String Buffer;
            /// <summary>
            /// kapazität von Buffer in Zeichenstück default 10000
            /// </summary>
            public int Capacity;
            /// <summary>
            ///Wird im Konstrukter aufgerufen
            /// </summary>
            public new bool Create()
            {
                this.Capacity = 10000;
                this.Buffer = new StringBuilder(this.Capacity);
                this.FlagReportOn = 1;

                return (true);
            }
        }
        
    }
}           
  
            