using System;
using Honeywell.Data;
using System.Collections.Generic;
using System.Data.Odbc;

using System.Data.OleDb;

namespace Honeywell
{
    namespace Data
    {        

        /// <summary>
        /// Interface zur DBF-Tabelle
        /// </summary>       
        public class CDataDBF : CObjectData
        {
            /// <summary>
            /// Konstruktor zum Erstellen des Datenbankobjekts
            /// </summary>            
            /// <param name="_NameDBF">Name von der Dbf-tabelle mit Pfad und Erweiterung</param>
            public CDataDBF(String _NameFull)
                : base(_NameFull)
            {
                this.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;User ID=Admin;Password=;";               
                                                        
                this.Error = "";
                this.dbf_Extension = null;
                this.dbf_Previous = null;
                this.CoundFieldForHeaderTemplate = 0;                                                              

                this.Create();
            }

            /// <summary>
            /// Wird von Konstrukter aufgerufen
            /// Baut die Verbindung zur DBF
            /// Holt die Strukture von der DBF und speichert in structureDBFTableARRAY
            /// </summary>
            /// <returns>Ergebnis</returns>
            public new bool Create()
            {
                int i;
                String strCommand;
                this.Error = "";

                this.connection = new OleDbConnection();              
                this.connection.ConnectionString = string.Format(this.ConnectionString, this.Path);
                this.command = new OleDbCommand();
                this.command.Connection = this.connection;                    

                if (!this.Open())
                    return (false);

                strCommand = String.Format("Select * from {0} WHERE FALSE",this.NameShort);            
                if (this.SetCommand(strCommand, true, 0) < 0)
                {
                    if (!this.Close())
                        return (false);
                    return (false);
                }
               
                this.CountField = this.reader.FieldCount;
                //Speichert DBF-Struktur in this.structureDBFTableARRAY /////////
                this.tableTempDICTIONARY = new Dictionary<int, String>(this.CountField);
                this.structureDBFTableARRAY = new String[this.CountField];
                for(i=0;i<this.CountField;i++)
                {
                    this.structureDBFTableARRAY[i] = this.reader.GetName(i);
                }
                /////////////////////////////////////////////////////////////////////////
                
                
                if (!this.Close())
                    return (false);
                

                return (true);
            }

            /// <summary>
            /// Führt SQL Befehl aus
            /// Vorher muss die Verbindung zur Datebbank mit Open() geöffnet werden
            /// und mit Close geschlossen werden (siehe Komentar zu KindOfExecution).
            /// </summary>
            /// <param name="Command">SQL-Befehl</param>
            /// <param name="ModeTableBack">
            /// true - nach dem Ausführen bes SQL-Befehls wird eine Tabelle geliefert
            /// false - keine Tabelle nach dem Ausführen bes SQL-Befehls
            /// </param>
            /// <param name="KindOfExecution">
            /// 0-nur Befehl ausführen
            /// 1-Verbindung vor der Befehlausführung öffnen
            /// 2-Verbindung vor der Befehlausführung öffnen und nach der Ausführung schließen
            /// -1-Befehl ausführen und die Verbindung schließen
            /// </param>
            /// <returns>
            /// Anzahl von den beeinflüssten Zeilen
            /// weniger als 0 Fehlerhaft
            /// </returns>            
            public int SetCommand(String _Command, bool _ModeTableBack, int _KindOfExecution)
            {
                this.command.CommandText = _Command;
                int iCountRecords=-1;
                if (_KindOfExecution == 1 || _KindOfExecution == 2)
                {
                    if (!this.Open())
                        return (-1);
                }
                try
                {
                    if (_ModeTableBack)
                    {
                        this.reader = this.command.ExecuteReader();
                        iCountRecords = this.reader.RecordsAffected;
                        iCountRecords = 0;
                    }
                    else
                        iCountRecords = this.command.ExecuteNonQuery();                    
                }
                catch (Exception ex)
                {
                    iCountRecords = -1;
                    this.Error = String.Format("Error while execute command {1}\r\n({0})\r\n{2}", ex.Message, _Command, this.NameFull);
                    return (iCountRecords);
                }
                finally
                {
                    if (_KindOfExecution == -1 || _KindOfExecution == 2)
                    {
                        if (!this.Close())
                        {
                            iCountRecords = -1;
                        }
                    }
                }
                return (iCountRecords);                
            }

            /// <summary>
            /// Öffnet eine Verbindung zur Datenbank
            /// </summary>
            /// <returns>Ergebnis</returns>
            public bool Open()
            {               
                try
                {
                    this.connection.Open();                    
                }
                catch (Exception ex)
                {
                    this.Error = String.Format("Error while open conection\r\n({0})", ex.Message);
                    return (false);
                }

                return (true);
            }


            /// <summary>
            /// Schließt die Verbindung zur Datenbank
            /// </summary>
            /// <returns>Ergebnis</returns>
            public bool Close()
            {
                try
                {
                    this.connection.Close();
                }
                catch (Exception ex)
                {
                    this.Error = String.Format("Error while close conection\r\n({0})", ex.Message);
                    return (false);
                }

                return (true);
            }


            /// <summary>
            /// Schreibt _Value ins temporäre tableTempDICTIONARY.
            /// Mit der Funktion WriteToFile werden die Inthalte vom tableTempDICTIONARY in die Datenbank geschrieben
            /// </summary>
            /// <param name="_Column">Spalte in der Datenbank. Fängt mit 1 an</param>
            /// <param name="_Value">Wert, der zur Datenbank geschrieben wird</param>           
            public void SetValue(int _Column, String _Value)
            {
                this.SetValue(_Column, _Value, 0);
                
                return;
            }

            /// <summary>
            /// Schreibt _Value ins temporäre tableTempDICTIONARY.
            /// Mit der Funktion WriteToFile werden die Inthalte vom tableTempDICTIONARY in die Datenbank geschrieben
            /// </summary>
            /// <param name="_Column">Spalte in der Datenbank. Fängt mit 1 an</param>
            /// <param name="_Value">Wert, der zur Datenbank geschrieben wird</param>
            /// <param name="Mode">
            /// 0 - _Value wird zur Datenbank geschrieben so wie es ist
            /// 1 - Ein Punkt in _Value wird durch ein Komma ersetzt und dann zur Datenbank geschrieben
            /// </param>
            public void SetValue(int _Column, String _Value, int _Mode)// Unit 1 . ersetzt durch , - Setzen Value in das m_strDBFArray 
            {
                String strValue;

                strValue = _Value;
                switch (_Mode)
                {                
                case 1:
                    strValue = _Value.Replace(".",",");
                    break;
                }

                try
                {
                    this.tableTempDICTIONARY.Add(_Column, strValue);
                }
                catch (ArgumentException)
                {
                    this.tableTempDICTIONARY[_Column]= strValue;                                        
                }

                return;
            }

            /// <summary>
            /// Liest aus dem tableTempDICTIONARY aus
            /// </summary>
            /// <param name="_Column">Spalte in der Datenbank. Fängt mit 1 an</param>
            /// <param name="_Value">Wert, der aus dem Array ausgelesen wird</param>
            public void GetValue(int _Column, out String _Value)
            {
                String strMsg;
                _Value = "";
                try
                {
                    _Value = this.tableTempDICTIONARY[_Column];
                }
                catch (Exception e)
                {
                    strMsg = e.Message;                    
                }

                return;
            }

            /// <summary>
            /// Schreibt tableTempDICTIONARY in die Datenbank (Datensatz wird angehängt)
            /// </summary>
            /// <returns>Ergebnis</returns>
            public bool WriteToFile()
            {
                String strNameColumns, strValues, strBuffer, strCommand;
                int iCountOfPreviousFields, iFieldCurrent;
                bool bResult;


                bResult = false;
                while (true)
                {

                    strNameColumns = "";
                    strValues = "";
                    iCountOfPreviousFields = this.GetCountOfPreviousFields();

                    //Wenn es Vorlagefelder existieren, die aus der Vorlagedatenbank stammen ////////
                    if (this.CoundFieldForHeaderTemplate != 0 && this.DBF_HeaderTemplate.tableTempDICTIONARY != null)
                    {
                        foreach (KeyValuePair<int, String> hStructure in this.DBF_HeaderTemplate.tableTempDICTIONARY)
                        {
                            if (hStructure.Key - 1 < this.CoundFieldForHeaderTemplate && hStructure.Key - 1 < this.CountField)
                            {
                                strBuffer = string.Format("[{0}], ", this.structureDBFTableARRAY[hStructure.Key - 1]);
                                strNameColumns += strBuffer;
                                strBuffer = string.Format("'{0}', ", hStructure.Value);
                                strValues += strBuffer;
                            }
                        }
                    }
                    ////////////////////////////////////////////////////////

                    //Felder für die eigene Datei (Ohne Vorlagenfelder) ///////////////////////////////////
                    foreach (KeyValuePair<int, String> hStructure in this.tableTempDICTIONARY)
                    {
                        if (hStructure.Key > iCountOfPreviousFields)
                        {
                            if (hStructure.Key < iCountOfPreviousFields + this.CountField - this.CoundFieldForHeaderTemplate + 1)
                            {
                                iFieldCurrent = hStructure.Key - iCountOfPreviousFields + this.CoundFieldForHeaderTemplate;

                                strBuffer = string.Format("[{0}], ", this.structureDBFTableARRAY[iFieldCurrent - 1]);
                                strNameColumns += strBuffer;
                                strBuffer = string.Format("'{0}', ", hStructure.Value);
                                strValues += strBuffer;
                            }
                        }
                    }
                    ////////////////////////////////////////////////////////

                    strBuffer = strNameColumns.Substring(0, strNameColumns.Length - 2);
                    strNameColumns = strBuffer;
                    strBuffer = strValues.Substring(0, strValues.Length - 2);
                    strValues = strBuffer;

                    if (!this.Open())
                        break;

                    strCommand = string.Format("INSERT INTO {0} ({1}) VALUES ({2});", this.NameShort, strNameColumns, strValues);
                    if (this.SetCommand(strCommand, false, 0) < 0)
                    {
                        this.Close();                            
                        break; 
                    }

                    if (!this.Close())
                        break; 

                    if (this.DBF_Extension != null)
                    {
                        this.DBF_Extension.tableTempDICTIONARY = this.tableTempDICTIONARY;
                        if (!this.DBF_Extension.WriteToFile())
                            break;
                    }
                    bResult = true;
                    break;
                }

                if (this.dbf_Previous == null)
                    this.tableTempDICTIONARY.Clear();

                return (bResult);
            }

			
            /// <summary>
            /// Gibt Anzahl von Datenfelder die in vorherigen Datenbanken benutzt sind (ohne Anzahl von Header Felder bei den Extentiondatenbank)
            /// </summary>
            /// <returns>
            /// Anzahl von Datenfelder
            /// </returns>
            private int GetCountOfPreviousFields()
            {
                CDataDBF DBF_PreviousTemp;
                int iCountFieldPrevious;
                iCountFieldPrevious = 0;
                if (this.dbf_Previous == null)
                    return iCountFieldPrevious;
                DBF_PreviousTemp = this.dbf_Previous;                
                while (true)
                {                                       
                    if (DBF_PreviousTemp != null)
                    {
                        iCountFieldPrevious += DBF_PreviousTemp.CountField;
                        iCountFieldPrevious -= DBF_PreviousTemp.CoundFieldForHeaderTemplate;
                        DBF_PreviousTemp = DBF_PreviousTemp.dbf_Previous;
                    }
                    else
                    {
                        break;
                    }                    
                }                
                return iCountFieldPrevious;
            }

            /// <summary>
            /// ConectionString in Format "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;User ID=Admin;DB_Password=;";
            /// Mit der Platzhalterung für den Datenbankpfad
            /// </summary>
            public String ConnectionString;
            private OleDbConnection connection;
            private OleDbCommand command;
            private OleDbDataReader reader;   

            /// <summary>
            /// Temporäre Array zur Zwischenspeicherung der Datenbankzeile
            /// Wird mit der Funktion WriteToFile zur Datenbank geschrieben
            /// </summary>
            private Dictionary<int, String> tableTempDICTIONARY;
            /// <summary>
            /// Feldnamen von der Datenbank
            /// 0-Erstes Datenbankfeld
            /// </summary>
            private String[] structureDBFTableARRAY;
            /// <summary>
            /// Anzahl der Spalten in DBF
            /// </summary>
            public int CountField; 
                          
            /// <summary>
            /// Wird von der Eigenschaft DBF_Extension ausgefüllt.
            /// </summary>
            private CDataDBF dbf_Previous;
            /// <summary>
            /// Siehe Eigenschaft feld DBF_Extension
            /// </summary>
            /// <seealso>DBF_Extension</seealso>
            private CDataDBF dbf_Extension;
            /// <summary>
            /// Wenn physikalische Datenbank, soll auf zwei Dateien erweitert werden.
            /// Hier wird Referenz auf die zweite datei abgelegt
            /// </summary>
            public CDataDBF DBF_Extension
            {
                get
                {
                    return this.dbf_Extension;
                }
                set
                {
                    this.dbf_Extension = value;
                    if(value != null)
                        value.dbf_Previous = this;
                }
            }           
            /// <summary>
            /// Referen auf die Datenbank von wo die Vorlagenfelder für die Extentionsdatenbank genommen werden
            /// </summary>
            public CDataDBF DBF_HeaderTemplate;
            /// <summary>
            /// Erste CoundFieldForHeaderTemplate-Spalten von DBF_HeaderTemplate sind Vorlage für die neue Datenbank
            /// </summary>
            public int CoundFieldForHeaderTemplate;                 
        }
    }
}