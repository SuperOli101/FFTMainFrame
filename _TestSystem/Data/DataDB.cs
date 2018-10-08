using System;
using Honeywell.Data;
using System.Collections.Generic;
using System.Data.SqlClient;



namespace Honeywell
{
    namespace Data
    {        

        /// <summary>
        /// Interface zur Datenbank
        /// </summary>
        public class CDataDB : CObjectData
        {
            
            /// <summary>
            /// AuthenticationMethod =  "SSPI"-Windows Authentication
            ///                         "true"-Windows Authentication
            ///                         "false"
            /// </summary>
            public CDataDB(String ServerName, String DBName, String TableName, String AuthenticationMethod, String UserID, String Password)
                : base(DBName)
            {
                

                if (DBName == null)
                    this.NameDB = "";
                else
                    this.NameDB = DBName;
                if (ServerName == null)
                    this.NameServer = "";
                else
                    this.NameServer = ServerName;

                if (TableName == null)
                     this.NameTable = "General";
                else
                     this.NameTable = TableName;
                
                if (AuthenticationMethod == null)
                    this.AuthenticationMethod = "";
                else
                    this.AuthenticationMethod = AuthenticationMethod;
                if (UserID == null)
                    this.UserID = "";
                else
                    this.UserID = UserID;
                if (Password == null)
                    this.Password = "";
                else
                    this.Password = Password;

                this.ConnectionString = String.Format("Data Source={0};Integrated Security={1};Initial Catalog={2};UserID={3};Password={4};",
                        this.NameServer, this.AuthenticationMethod, this.NameDB, this.UserID, this.Password);

                this.Create();

            }

            /// <summary>
            ///Wird im Konstrukter aufgerufen
            /// </summary>
            public new bool Create()
            {
                String strCommand;
                this.ConnectionSQL = new SqlConnection(this.ConnectionString);

                strCommand = String.Format("SELECT * FROM [{0}] WHERE 1=0",this.NameTable);//Wird die leere Tabelle geholt, um die Sructur zu bekommen
                this.CommandSQL = new SqlCommand(strCommand, this.ConnectionSQL);
                return (true);
            }

            /// <summary>
            ///virtueller Inhalt des Tabellendatensatzes, wird beim Append  benutzt.
            /// </summary>
            public Dictionary<String, Object> TableRecordDICTIONARY;
            public String ConnectionString;

            private SqlConnection ConnectionSQL;
            private SqlCommand CommandSQL;

            private String NameServer;
            private String NameDB;
            private String NameTable;
            private String AuthenticationMethod;
            private String UserID;
            private String Password;
            
        }
    }
}
