using System;
using Honeywell.Test;


namespace Honeywell
{
    namespace Data
    {
        /// <summary>
        /// Klasse zur Datei-Namen Verarbeitung
        /// </summary>
        public static class CFile
        {            
            /// <summary>
            /// No Path
            /// </summary>
            /// <param name="NameFull"></param>
            /// <param name="Name"></param>
            /// <param name="Error"></param>
            /// <returns></returns>
            public static bool GetName(String NameFull, out String Name, out String Error)
            {
                int iIndex;
                String strMsg, strNameFull;
                Name = "";
                Error = "";

				//System.IO.Path.GetFileName()
                strNameFull = NameFull;
                iIndex = strNameFull.LastIndexOf("\\");
                if (iIndex == -1)
                {
                    strMsg = String.Format("The name of file is not the full name.({0})", strNameFull);
                    Error = strMsg;
             
                    return (false);
                }

                Name = strNameFull.Substring(iIndex + 1);

                return (true);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="NameFull"></param>
            /// <param name="Path"></param>
            /// <param name="Error"></param>
            /// <returns></returns>
            public static bool GetPath(String NameFull, out String Path, out String Error)
            {
                int iIndex;
                String strMsg, strNameFull;
                Path = "";
                Error = "";

				//System.IO.Path.GetDirectoryName()
                strNameFull = NameFull;
                iIndex = strNameFull.LastIndexOf("\\");
                if (iIndex == -1)
                {
                    strMsg = String.Format("The name of file is not the full name.({0})", strNameFull);
                    Error = strMsg;

                    return (false);
                }

                Path = strNameFull.Substring(0, iIndex);

                return (true);
            }

            /// <summary>
            /// No Path, No Extension
            /// </summary>
            /// <param name="NameFull"></param>
            /// <param name="Name"></param>
            /// <param name="Error"></param>
            /// <returns></returns>
            public static bool GetNameShort(String NameFull, out String Name, out String Error)
            {
                int iIndex;
                String strMsg, strName, strNameFull;
                Name = "";                

                if (!CFile.GetName(NameFull, out strName, out Error))
                    return (false);

				//System.IO.Path.GetFileNameWithoutExtension()
                strNameFull = NameFull;
                iIndex = strName.LastIndexOf(".");
                if (iIndex == -1)
                {
                    strMsg = String.Format("The name of file doesn't contain the extension.({0})", strNameFull);
                    Error = strMsg;                    

                    return (false);
                }

                Name = strName.Substring(0, iIndex);
                return (true);
            }


        }

        /// <summary>
        /// Template-Klasse für die Data-Klassen
        /// </summary>
        public class CObjectData : CObjectContainer
        {
            public CObjectData(String NameFull) : base()
            {
                this.SetName(NameFull);
                CObjectTest.PoolIDData++;
                this.Create();                
            }
            ~CObjectData()
            {
                CObjectTest.PoolIDData--;
            }

            /// <summary>
            ///Wird im Konstrukter aufgerufen
            /// </summary>
            public new bool Create()
            {                
                this.IDType = 1;
                this.ID = CObjectTest.PoolIDData;
				//this.Timer.Timer.Name = this.Name;
                return (true);
            }

            /// <summary>
            ///Setzt Name, NameFull, NameShort, Extension und Path von NameFull
            /// <summary>
            public bool SetName(String NameFull)
            {
                this.NameFull = NameFull;

                if (!this.GetPath(this.NameFull, out this.Path))
                    return (false);

                if (!this.GetName(this.NameFull, out this.Name))
                    return (false);

                if (!this.GetNameShort(this.NameFull, out this.NameShort))
                    return (false);

                if (!this.GetExtension(this.NameFull, out this.Extension))
                    return (false);

                return (true);
            }

            /// <summary>
            ///No Path
            /// <summary>
            public bool GetName(String NameFull, out String Name)
            {
                int iIndex;
                String strMsg, strNameFull;
                Name = "";

                strNameFull = NameFull;
                iIndex = strNameFull.LastIndexOf("\\");
                if (iIndex == -1)
                {
                    strMsg = String.Format("The name of file is not the full name.({0})", strNameFull);
                    this.Error=strMsg;

                    return (false);
                }

                Name = strNameFull.Substring(iIndex + 1);

                return (true);

            }

            /// <summary>
            ///No Path, No Extension
            /// <summary>
            public bool GetNameShort(String NameFull, out String Name)
            {
                int iIndex;
                String strMsg, strName, strNameFull;
                Name = "";

                this.GetName(NameFull, out strName);

                strNameFull = NameFull;
                iIndex = strName.LastIndexOf(".");
                if (iIndex == -1)
                {
                    strMsg = String.Format("The name of file doesn't contain the extension.({0})", strNameFull);
                    this.Error=strMsg;

                    return (false);
                }

                Name = strName.Substring(0, iIndex);
                return (true);
            }

            public bool GetPath(String NameFull, out String Path)
            {
                int iIndex;
                String strMsg, strNameFull;
                Path = "";

                strNameFull = NameFull;
                iIndex = strNameFull.LastIndexOf("\\");
                if (iIndex == -1)
                {
                    strMsg = String.Format("The name of file is not the full name.({0})", strNameFull);
                    this.Error=strMsg;

                    return (false);
                }

                Path = strNameFull.Substring(0, iIndex);

                return (true);
            }
            
            public bool GetExtension(String NameFull, out String Extension)
            {
                int iIndex;
                String strMsg, strNameFull;
                Extension = "";

                strNameFull = NameFull;
                iIndex = strNameFull.LastIndexOf(".");
                if (iIndex == -1)
                {
                    strMsg = String.Format("The name of file doesn't contain the extension.({0})", strNameFull);
                    this.Error=strMsg;

                    return (false);
                }

                Extension = strNameFull.Substring(iIndex + 1);
                return (true);
            }


            public String NameFull;
            public String NameShort;
            public String Extension;
            public String Path;

        }
    }
}
