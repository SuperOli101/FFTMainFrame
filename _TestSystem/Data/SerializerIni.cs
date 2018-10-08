using System;
using System.Collections.Generic;


namespace Honeywell
{
    namespace Data
    {
        /// <summary>
        /// Mit dieser Klasse werden die Datein im Ini-Format in die interne Variable CSerializerIni::IniStructuredDICTIONARY eingelesen und 
        /// mit ihre Hilfe werden die Änderungen in der Datei vorgenommen. (Alle Parameter nur als String und nicht variantenabhänig)
        /// </summary>
        public class CSerializerIni : CObjectData
        {
            public CSerializerIni(String NameFull)
                : base(NameFull)
            {
                this.Create();
            }


           
            /// <summary>
            /// Liest die Ini-Datei(NameFull) in die internen Variablen IniStructuredDICTIONARY,
            /// IniTextFileARRAY, IniTextFileNoCommentLIST, IniTextFile
            /// </summary>
            /// <param name="NameFull">
            /// Name mit Pfad und Erweiterung 
            /// </param>
            /// <returns></returns>
            public bool Read(String NameFull)           
            {
                int iLineFile, iFlagCommentTextOn, iIndex, iIndexBracketOpen, iIndexBracketClose, iCountGroup, iIndexLineComment, iIndexAssign;
				int iMax, iIndexBracketCurly, iCount;
                bool bResult;
                String strLine, strLineNoComment, strLineNoCommentText, strMsg, strItem, strGroup, strValue, strItemVariant, strVariant, strGroupNoBracket;
                String strLineFile, strName, strValueInBracket;
                String[] strSignSplitARRAY ={ "\r\n" };
                Dictionary<String, Dictionary<String, String>> hGroup = null, hGroupLine = null;
                Dictionary<String, String> hItem = null, hItemLine = null;
                System.IO.FileStream hFileStream = null;
                System.IO.StreamReader hStreamReader = null;

                strGroupNoBracket = "";
                iLineFile = 0;
                bResult = true;

                strName = "";
                if (!this.GetName(NameFull, out strName))
                    return (false);

                try
                {

                    hFileStream = new System.IO.FileStream(NameFull, System.IO.FileMode.Open);
                    hStreamReader = new System.IO.StreamReader(hFileStream, System.Text.Encoding.UTF8);
                    this.IniTextFile = hStreamReader.ReadToEnd();
                    hStreamReader.Close();
                    hStreamReader = null;

                    this.IniTextFileARRAY = this.IniTextFile.Split(strSignSplitARRAY, StringSplitOptions.None);
                    iMax = this.IniTextFileARRAY.Length;

                    this.IniTextFileNoCommentLIST = new System.Collections.Generic.List<String>(iMax);
                    this.IniStructuredDICTIONARY = new Dictionary<String, Dictionary<String, Dictionary<String, String>>>(iMax);
                    this.IniLineStructuredDICTIONARY = new Dictionary<String, Dictionary<String, Dictionary<String, String>>>(iMax);

                    this.GroupWithDoubleItemLIST = new List<String>(iMax);

                    iLineFile = 1;
                    iFlagCommentTextOn = 0;
                    iCountGroup = 0;
                    strGroup = "";
                    foreach (String strLineTemp in this.IniTextFileARRAY)
                    {
                        strLine = strLineTemp;
                        //Textkommentar (/* */) entfernen ////////////////////////////////////////
                        if (iFlagCommentTextOn == 1)
                        {
                            iIndex = strLine.IndexOf("*/");
                            if (iIndex != -1)
                            {
                                iFlagCommentTextOn = 0;
                                strLineNoCommentText = strLine.Substring(iIndex + 2);
                            }
                            else
                                strLineNoCommentText = null;
                        }
                        else
                        {
                            iIndex = strLine.IndexOf("/*");
                            iIndexLineComment = strLine.IndexOf("//");
                            if (iIndex < iIndexLineComment || iIndexLineComment == -1)//In der Zeile sind // und /* vorhanden. (was zuerst kommt, das wird angeschaltet)
                            {
                                if (iIndex != -1)
                                {
                                    iFlagCommentTextOn = 1;
                                    if (iIndex != 0)
                                    {
                                        strLineNoCommentText = strLine.Substring(0, iIndex);
                                    }
                                    else
                                        strLineNoCommentText = null;
                                }
                                else
                                    strLineNoCommentText = strLine;
                            }
                            else
                                strLineNoCommentText = strLine;
                        }
                        ////////////////////////////////////////

                        if (strLineNoCommentText != null)
                        {
                            //Linekommentar (//) entfernen ////////////////////////////////////////
                            iIndex = strLineNoCommentText.IndexOf("//");
							if(iIndex != -1)							
								strLineNoComment = strLineNoCommentText.Substring(0, iIndex);							
							else
								strLineNoComment = strLineNoCommentText;
                            ////////////////////////////////////////

                            //Entfernen von Tab und Leerstellen am Anfang und Ende//////////////////////////////////////
                            strLineNoComment = strLineNoComment.Replace("\t", "");
                            strLineNoComment = strLineNoComment.Trim();
                            ////////////////////////////////////////


                            //Die leere Zeilen werden nicht bearbeitet //////////////////////////////////////
                            iIndex = strLineNoComment.Length;
                            //////////////////////////////////////
                            if (iIndex != 0)
                            {
                                this.IniTextFileNoCommentLIST.Add(strLineNoComment);
                                iIndexBracketOpen = strLineNoComment.IndexOf("[");
                                iIndexBracketClose = strLineNoComment.IndexOf("]");
								if(iIndexBracketOpen >= 0 && iIndexBracketClose > iIndexBracketOpen)
								{
									strValueInBracket = strLineNoComment.Substring(iIndexBracketOpen+1, iIndexBracketClose - iIndexBracketOpen-1);
									strValueInBracket.Trim();
								}
								else
									strValueInBracket = "";								
                                //group item value					
                                strItem = "";
                                strValue = "";
                                //[group] //////////////////////////////////////
								if(iIndexBracketOpen == 0 && iIndexBracketClose > 0)
                                {
                                    iCountGroup++;
                                    iIndexAssign = strLineNoComment.IndexOf("=");
                                    strGroup = strLineNoComment.Substring(iIndexBracketOpen, iIndexBracketClose + 1);
                                    hGroup = new Dictionary<String, Dictionary<String, String>>();
                                    hGroupLine = new Dictionary<String, Dictionary<String, String>>();
                                    strGroupNoBracket = strGroup.Substring(1, strGroup.Length - 2);
                                    this.IniStructuredDICTIONARY.Add(strGroupNoBracket, hGroup);
                                    this.IniLineStructuredDICTIONARY.Add(strGroupNoBracket, hGroupLine);
                                    //Überpüfen, ob die aktuelle Gruppe die doppelte Items haben darf ([group]{} bzw. [group]{}=value)/////////////////////
                                    iIndexBracketCurly = strLineNoComment.IndexOf("{}");
                                    if ((iIndexBracketCurly > iIndexBracketClose && iIndexBracketCurly < iIndexAssign) || (iIndexBracketCurly > iIndexBracketClose && iIndexAssign == -1))
                                    {
                                        this.GroupWithDoubleItemLIST.Add(strGroupNoBracket);
                                    }
                                    //////////////////////////////////////
                                }
                                //////////////////////////////////////
                                else
                                {
                                    iIndex = strLineNoComment.IndexOf("=");
									//item[variant]=value aber nicht item[0]=value das ist auch default value//////////////////////////////////////
									if(iIndexBracketOpen > 0 && iIndexBracketClose > iIndexBracketOpen && iIndexBracketClose < iIndex && strValueInBracket != "0")
                                    {
                                        strItem = strLineNoComment.Substring(0, iIndexBracketOpen);//Bis [
                                        strVariant = strLineNoComment.Substring(iIndexBracketOpen + 1, iIndexBracketClose - iIndexBracketOpen - 1);
                                        strVariant = strVariant.Replace("\t", "");
                                        strVariant = strVariant.Trim();
                                        if (!hGroup.ContainsKey(strItem))
                                        {
                                            strMsg = String.Format("Default item is not available for {0}[{1}]", strItem, strVariant);
                                            throw new Exception(strMsg);
                                        }
                                        if (iIndex < 0)
                                        {
                                            strItem = strLineNoComment;
                                            strValue = "";
                                        }
                                        else
                                        {
                                            strItemVariant = strLineNoComment.Substring(0, iIndex);// mit [
                                            strItemVariant = strItemVariant.Trim();
                                            strValue = strLineNoComment.Substring(iIndex + 1);
                                            strValue = strValue.Trim();
                                        }

                                        hItem.Add(strVariant, strValue);
                                        strLineFile = String.Format("{0}", iLineFile);
                                        hItemLine.Add(strVariant, strLineFile);
                                    }
                                    //////////////////////////////////////
                                    //item=value //////////////////////////////////////
                                    else
                                    {
                                        hItem = new Dictionary<String, String>();
                                        hItemLine = new Dictionary<String, String>();
                                        if (iIndex < 0)
                                        {
                                            strItem = strLineNoComment;
                                            strValue = "";
                                        }
                                        else
                                        {
                                            strItem = strLineNoComment.Substring(0, iIndex);
                                            strItem = strItem.Trim();
											if(strValueInBracket == "0")//item[0]=value das ist auch default value									
												strItem = strItem.Substring(0, iIndexBracketOpen);											
                                            strValue = strLineNoComment.Substring(iIndex + 1);
                                            strValue = strValue.Trim();
                                            strValue = strValue.Replace("\\n", "\n");
                                            strValue = strValue.Replace("\\r", "\r");

                                            //Überpüfen ob die aktuelle Items sind die nächte Gruppe, die doppelte Items enthalen dürfen (in Ini mit ={} markiert /////////////////////
                                            //Item, Item{1}, Item{2} .... ////////////////////////////////////////////////////////////
                                            iIndexBracketCurly = strValue.IndexOf("{}");
                                            if (iIndexBracketCurly == 0)
                                            {
                                                strValue = strValue.Substring(iIndexBracketCurly + 2);
                                                strValue = strValue.Trim();
                                                this.GroupWithDoubleItemLIST.Add(strItem);
                                            }
                                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        }

                                        if (iCountGroup != 0)
                                        {
                                            if (hGroup.ContainsKey(strItem))
                                            {
                                                iMax = this.GroupWithDoubleItemLIST.Count;
                                                //Überpüfen, ob die aktuelle Gruppe die doppelte Items haben darf/////////////////////
                                                if (iMax != 0)
                                                {
                                                    if (this.GroupWithDoubleItemLIST.IndexOf(strGroupNoBracket) != -1)
                                                    {
                                                        iCount = this.GetCountOfItemsWithPattern(hGroup, strItem + "{");
                                                        iCount++;
                                                        strItem = strItem + "{" + iCount.ToString() + "}";
                                                    }
                                                    else
                                                    {
                                                        strMsg = String.Format("Item {0} is double", strItem);
                                                        throw new Exception(strMsg);
                                                    }
                                                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                                }
                                                else
                                                {
                                                    strMsg = String.Format("Item {0} is double", strItem);
                                                    throw new Exception(strMsg);
                                                }
                                            }

                                            hItem.Add("Default", strValue);
                                            hGroup.Add(strItem, hItem);
                                            strLineFile = String.Format("{0}", iLineFile);
                                            hItemLine.Add("Default", strLineFile);
                                            hGroupLine.Add(strItem, hItemLine);
                                        }
                                    }
                                    //////////////////////////////////////
                                }
                            }
                        }

                        iLineFile++;
                    }
                }
                catch (Exception e)
                {
                    if (iLineFile != 0)
                    {
                        strMsg = String.Format("Error while reading the line {0} in {2}\r\n{1}", iLineFile, e.Message, strName);
                        this.Error=strMsg;
                    }
                    else
                    {
                        strMsg = String.Format("Error while opening the file {0}\r\n{1}", strName, e.Message);
                        this.Error=strMsg;
                    }
                    bResult = false;
                }
                finally
                {
                    if (hStreamReader != null)
                        hStreamReader.Close();
                }

                return (bResult);

            }
            /// <summary>
            ///Liest die Ini-Datei(NameFull) in die internen Variablen IniStructuredDICTIONARY,
            ///IniTextFileARRAY, IniTextFileNoCommentLIST, IniTextFile
            /// </summary>
            public bool Read()
            {
                return (this.Read(this.NameFull));
            }

            /// <summary>
            ///Ersetzen IniTextFileARRAY mit den neuen Daten aus IniLineStructuredDICTIONARY
            ///und Write IniTextFileARRAY in die Ini Datei
            /// </summary>
            /// <param name="NameFull">
            /// Name mit Pfad und Erweiterung 
            /// </param>
            /// <returns></returns>
            public bool Write(String NameFull)           
            {
                System.IO.FileStream hFileStream = null;
                System.IO.StreamWriter hStreamWriter = null;
                bool bResult;
                String strMsg, strGroup = "", strItem = "", strVariant = "", strValue = "", strNumberLine = "", strLine, strLineNew, strName = "";


                if (!this.GetName(NameFull, out strName))
                    return (false);

                bResult = true;
                try
                {
                    //Ersetzen IniTextFileARRAY mit den neuen Daten aus IniLineStructuredDICTIONARY //////////////////////////////////////////////
                    foreach (KeyValuePair<String, Dictionary<String, Dictionary<String, String>>> hGroup in this.IniStructuredDICTIONARY)
                    {
                        foreach (KeyValuePair<String, Dictionary<String, String>> hItem in hGroup.Value)
                        {
                            foreach (KeyValuePair<String, String> hVariant in hItem.Value)
                            {
                                strGroup = hGroup.Key;
                                strItem = hItem.Key;
                                strVariant = hVariant.Key;
                                strValue = hVariant.Value;
                                strNumberLine = this.IniLineStructuredDICTIONARY[strGroup][strItem][strVariant];
                                strLine = this.IniTextFileARRAY[Convert.ToInt32(strNumberLine) - 1];

                                strLineNew = this.ReplaceValue(strLine, strValue);
                                this.IniTextFileARRAY[Convert.ToInt32(strNumberLine) - 1] = strLineNew;
                            }
                        }
                    }
                    //////////////////////////////////////////////

                    //Write IniTextFileARRAY in die Ini Datei //////////////////////////////////////////////
                    hFileStream = new System.IO.FileStream(NameFull, System.IO.FileMode.Create);
                    hStreamWriter = new System.IO.StreamWriter(hFileStream, System.Text.Encoding.UTF8);

                    foreach (String strLineWrite in this.IniTextFileARRAY)
                    {
                        hStreamWriter.WriteLine(strLineWrite);
                    }
                    //////////////////////////////////////////////

                }
                catch (Exception e)
                {
                    bResult = false;
                    if (hFileStream != null)
                    {
                        strMsg = String.Format("Error while writing the file {0}.\r\n{1}", strName, e.Message);
                        this.Error=strMsg;
                    }
                    else
                    {
                        if (strVariant == "Default")
                        {
                            strMsg = String.Format("Error while setting [{0}]{1}={2} (Line {3})", strGroup, strItem, strValue, strNumberLine);
                        }
                        else
                        {
                            strMsg = String.Format("Error while setting [{0}]{1}[{4}]={2} (Line {3})", strGroup, strItem, strValue, strNumberLine, strVariant);
                        }
                        this.Error=strMsg;
                    }
                }
                finally
                {
                    if (hStreamWriter != null)
                        hStreamWriter.Close();
                }

                return (bResult);
            }
            /// <summary>
            ///Ersetzen IniTextFileARRAY mit den neuen Daten aus IniLineStructuredDICTIONARY
            ///und Write IniTextFileARRAY in die Ini Datei
            /// </summary>
            public bool Write()
            {               
                return (this.Write(this.NameFull));
            }

            /// <summary>
            ///Schreibt Value in IniStructuredDICTIONARY
            /// </summary>
            /// <param name="Group"></param>
            /// <param name="Item"></param>
            /// <param name="Value"></param>
            /// <returns></returns>
            public bool SetValue(String Group, String Item, object Value)            
            {
                String strMsg, strValue, strValueNew, strFormat, strVariant;
                int iPosOfPoint, iSignAfterPoint;
                bool bResult;
                double dValue=0;
                int iValue = 0;
                bool bValue = true;


                bResult = true;
                try
                {
                    strVariant = this.getVariantCurrent(Group, Item);
                    //strVariant = "Default";

                    if (Value.GetType() == typeof(double))
                    {    
                        strValue = this.IniStructuredDICTIONARY[Group][Item][strVariant];
                        iPosOfPoint=strValue.IndexOf(".");
                        iSignAfterPoint = strValue.Length - iPosOfPoint-1;
                        strFormat = "0.";
                        strFormat = strFormat.PadRight(strFormat.Length+iSignAfterPoint, '0');
                        dValue = (double)Value;
                        strValueNew = dValue.ToString(strFormat);
                        iPosOfPoint=strValueNew.IndexOf(",");
                        if (iPosOfPoint != -1)
                            strValueNew=strValueNew.Replace(",", ".");
                        this.IniStructuredDICTIONARY[Group][Item][strVariant] = strValueNew;
                    }
                    else if (Value.GetType() == typeof(int))
                    {
                        iValue = (int)Value;
                        strValueNew = iValue.ToString();
                        this.IniStructuredDICTIONARY[Group][Item][strVariant] = strValueNew;
                    }
                    else if (Value.GetType() == typeof(String))
                    {
                        strValueNew = (String)Value;
                        this.IniStructuredDICTIONARY[Group][Item][strVariant] = strValueNew;
                    }
                    else if (Value.GetType() == typeof(bool))
                    {
                        bValue = (bool)Value;
                        strValueNew = bValue.ToString();
                        this.IniStructuredDICTIONARY[Group][Item][strVariant] = strValueNew;
                    }
                    else
                    {
                        bResult = false;
                        strMsg = String.Format("Unbekannter Datentyp {0}", (Value.GetType()).ToString());
                        this.Error=strMsg;
                    }
                }
                catch (Exception e)
                {
                    bResult = false;
                    strMsg = String.Format("No Group or Item exist ([{0}] {1})\r\n{2}", Group, Item, e.Message);
                    this.Error=strMsg;
                }


                return (bResult);
            }

			/// <summary>
			///Holt Value aus IniStructuredDICTIONARY 
			/// Beispiel zum Aufruf:
			/// object hValue;
			/// String strValue;
			/// strValue="";
			/// hValue = strValue;
			/// this.GetValue(Group, Item, ref hValue)
			/// </summary>
			public bool GetValue(String Group, String Item, ref object Value)
			{
				String strValue = "", strMsg, strVariant = "";
				bool bResult = true;
				int iBuffer;

				try
				{
					strVariant = this.getVariantCurrent(Group, Item);
					//strVariant = "Default";
					strValue = this.IniStructuredDICTIONARY[Group][Item][strVariant];


					if(Value.GetType() == typeof(double))
					{                        
						Value = Convert.ToDouble(strValue, System.Globalization.CultureInfo.InvariantCulture);
					}
					else if(Value.GetType() == typeof(int))
					{
						if(strValue.IndexOf("0x") == 0 || strValue.IndexOf("0X") == 0)
						{
							Value = Convert.ToInt32(strValue, 16);
						}
						else
						{
							Value = Convert.ToInt32(strValue);
						}
					}
                    else if (Value.GetType() == typeof(uint))
                    {
                        if (strValue.IndexOf("0x") == 0 || strValue.IndexOf("0X") == 0)
                        {
                            Value = Convert.ToUInt32(strValue, 16);
                        }
                        else
                        {
                            Value = Convert.ToUInt32(strValue);
                        }
                    }
					else if(Value.GetType() == typeof(String))
					{
						Value = strValue;
					}
					else if(Value.GetType() == typeof(bool))
					{
						try
						{
							Value = Convert.ToBoolean(strValue);
						}
						catch(FormatException)
						{
							iBuffer = Convert.ToInt32(strValue);
							if(iBuffer == 0)
								Value = false;
							else
								Value = true;
						}

					}
					else if(Value.GetType() == typeof(byte))
					{
						Value = Convert.ToByte(strValue);
					}
					else
					{
						bResult = false;
						strMsg = String.Format("Unbekannter Datentyp {0}", (Value.GetType()).ToString());
						this.Error = strMsg;
					}


				}
				catch(Exception e)
				{
					bResult = false;
					strMsg = String.Format("No Group or Item exist ([{0}] {1})\r\n{2}", Group, Item, e.Message);
					this.Error = strMsg;
				}

				return (bResult);

			}

			/// <summary>
			///Holt Array aus IniStructuredDICTIONARY 
			///In Ini so gekennzeichnet:
			/// VersionFirmware 				= 1.0.0.A 
			/// VersionFirmware[2]				= 1.0.0.B
			/// VersionFirmware[1]				= 1.0.0.C
			/// VersionFirmware[4]				= 1.0.0.E
			/// In Array so:
			/// Array[0]     	    			= "1.0.0.A"
			/// Array[1]	    	    		= "1.0.0.B"
			/// Array[2]				        = "1.0.0.C"
			/// Array[3]				        = ""
			/// Array[4]				        = "1.0.0.E"
			/// </summary>
			public int GetValue(String Group, String Item, out String[] Array)
			{
				bool bResult = true;
				String strArrayName, strValue, strMsg;
				Dictionary<String, String> hArrayDICTIONARY;
				List<string> strBufferList = new List<string>(100);
				string[] strBufferArray;
				//strBufferList.AddRange();
				int iIndex;
				Array = null;

				try
				{
					hArrayDICTIONARY = this.IniStructuredDICTIONARY[Group][Item];
					Array = new String[hArrayDICTIONARY.Count];

					foreach(KeyValuePair<String, String> hBreakPointError in hArrayDICTIONARY)
					{
						strArrayName = hBreakPointError.Key;
						strValue = hBreakPointError.Value;

						if(strArrayName == "Default")
							iIndex = 0;
						else
							iIndex = Convert.ToInt32(strArrayName);
						if(iIndex < strBufferList.Count)
						{
							strBufferList[iIndex] = strValue;
						}
						else
						{
							if(iIndex != strBufferList.Count)
							{
								strBufferArray = new string[iIndex - strBufferList.Count];
								for(int i = 0; i < strBufferArray.Length; i++)
								{
									strBufferArray[i] = "";
								}
								strBufferList.AddRange(strBufferArray);

							}
							strBufferList.Add(strValue);
						}
					}
					Array = strBufferList.ToArray();
				}
				catch(Exception e)
				{
					bResult = false;
					strMsg = String.Format("No Group or Item exist ([{0}] {1})\r\n{2}", Group, Item, e.Message);
					this.Error = strMsg;
				}





				if(bResult)
					return (0);
				else
					return (1);
			}
			/// <summary>
			/// siehe Beschreibung von  public int GetValue(String Group, String Item, out String[] Array)
			/// wenn String ist "" dann 0
			/// </summary>
			/// <param name="Group"></param>
			/// <param name="Item"></param>
			/// <param name="Array"></param>
			/// <param name="ValueIfEmpty">
			/// Wenn String Value leer ist, dann zu dem ValueIfEmpty konvertieren
			/// </param>
			/// <returns></returns>
			public int GetValue(String Group, String Item, out double[] Array, double ValueIfEmpty = 0)
			{
				string[] strBufferArray;
				Array = new double[0];
				int iResult;
				iResult = this.GetValue(Group, Item, out strBufferArray);
				if(iResult == 0)
				{
					try
					{
						Array = new double[strBufferArray.Length];
						for(int i = 0; i < strBufferArray.Length; i++)
						{
                            if (strBufferArray[i].Length != 0)
                            {
                                Array[i] = Convert.ToDouble(strBufferArray[i], System.Globalization.CultureInfo.InvariantCulture);
                            }
                            else
                                Array[i] = ValueIfEmpty;
						}
					}
					catch
					{
						iResult++;
					}
				}
				return iResult;
			}
			/// <summary>
			/// siehe Beschreibung von  public int GetValue(String Group, String Item, out String[] Array)
			/// wenn String ist "" dann 0
			/// </summary>
			/// <param name="Group"></param>
			/// <param name="Item"></param>
			/// <param name="Array"></param>
			/// <param name="ValueIfEmpty">
			/// Wenn String Value leer ist, dann zu dem ValueIfEmpty konvertieren
			/// </param>
			/// <returns></returns>
			public int GetValue(String Group, String Item, out int[] Array, int ValueIfEmpty = 0)
			{
				string[] strBufferArray;
				Array = new int[0];
				int iResult;
				iResult = this.GetValue(Group, Item, out strBufferArray);
				if(iResult == 0)
				{
					try
					{
						Array = new int[strBufferArray.Length];
						for(int i = 0; i < strBufferArray.Length; i++)
						{
							if(strBufferArray[i].Length != 0)
							{
								if(strBufferArray[i].IndexOf("0x") != -1 || strBufferArray[i].IndexOf("0X") != -1)
									Array[i] = Convert.ToInt32(strBufferArray[i], 16);
								else
									Array[i] = Convert.ToInt32(strBufferArray[i], 10);
							}
							else
								Array[i] = ValueIfEmpty;
						}
					}
					catch
					{
						iResult++;
					}
				}
				return iResult;
			}
			/// <summary>
			/// siehe Beschreibung von  public int GetValue(String Group, String Item, out String[] Array)
			/// wenn String ist "" dann 0
			/// </summary>
			/// <param name="Group"></param>
			/// <param name="Item"></param>
			/// <param name="Array"></param>
			/// <param name="ValueIfEmpty">
			/// Wenn String Value leer ist, dann zu dem ValueIfEmpty konvertieren
			/// </param>
			/// <returns></returns>
			public int GetValue(String Group, String Item, out byte[] Array, byte ValueIfEmpty = 0)
			{
				string[] strBufferArray;
				Array = new byte[0];
				int iResult;
				iResult = this.GetValue(Group, Item, out strBufferArray);
				if(iResult == 0)
				{
					try
					{
						Array = new byte[strBufferArray.Length];
						for(int i = 0; i < strBufferArray.Length; i++)
						{
							if(strBufferArray[i].Length != 0)
							{
								if(strBufferArray[i].IndexOf("0x") != -1 || strBufferArray[i].IndexOf("0X") != -1)
									Array[i] = Convert.ToByte(strBufferArray[i], 16);
								else
									Array[i] = Convert.ToByte(strBufferArray[i], 10);
							}
							else
								Array[i] = ValueIfEmpty;
						}
					}
					catch
					{
						iResult++;
					}
				}
				return iResult;
			}           


            /// <summary>
            ///Ersetzt im String aus StringIn the Value und gibt den neuen String zurück.
            /// </summary>
            private String ReplaceValue(String StringIn, String Value)            
            {
                int iIndex;
                String strLine, strLineNew, strLineNoComment, strValueCurrentString, strValueCurrent, strValueNewString, strLineNoCommentNew;
                String strComment;
                strLine = StringIn;
                iIndex = strLine.IndexOf("//");
                if (iIndex != -1)
                {
                    strLineNoComment = strLine.Substring(0, iIndex);
                    strComment = strLine.Substring(iIndex);
                }
                else
                {
                    strLineNoComment = strLine;
                    strComment = "";
                }

                iIndex = strLineNoComment.IndexOf("=");
                if (iIndex != -1)
                {
                    strValueCurrentString = strLineNoComment.Substring(iIndex + 1);//Alles was auf der rechten Seite vom Zeichen "=" steht (nur Ohne Kommentar)
                    strValueCurrent = strValueCurrentString.Replace("\t", "");//nur Value
                    strValueCurrent = strValueCurrent.Trim();//nur Value
                    if (strValueCurrent.Length == 0)
                        strValueNewString = " " + Value + " ";
                    else
                        strValueNewString = strValueCurrentString.Replace(strValueCurrent, Value);
                    strLineNoCommentNew = strLineNoComment.Replace("=" + strValueCurrentString, "=" + strValueNewString);

                    strLineNew = strLineNoCommentNew + strComment;
                }
                else
                {
                    strLineNew = strLine;
                }

                return (strLineNew);
            }

            private int GetCountOfItemsWithPattern(Dictionary<String, Dictionary<String, String>> Group, String Pattern)
            {
                String strItem, strValue, strVariant;
                int iCount;

                iCount = 0;
                foreach (KeyValuePair<String, Dictionary<String, String>> hItem in Group)
                {
                    foreach (KeyValuePair<String, String> hVariant in hItem.Value)
                    {
                        //strGroup=hGroup.Key;
                        strItem = hItem.Key;
                        if (strItem.IndexOf(Pattern) == 0)
                            iCount++;

                        strVariant = hVariant.Key;
                        strValue = hVariant.Value;

                    }
                }
                return (iCount);
            }

            public new bool Create()
            {
                this.IniTextFile = null;
                this.IniTextFileARRAY = null;
                this.GroupWithDoubleItemLIST = null;

                this.IniTextFileNoCommentLIST = null;
                this.IniStructuredDICTIONARY = null;
                this.IniLineStructuredDICTIONARY = null;


                return (true);
            }

            protected virtual String getVariantCurrent(String Group, String Item)
            {
                return("Default");
            }

            /// <summary>
            ///Im Textformat Ini-Datei
            /// </summary>
            public String IniTextFile;

            /// <summary>
            ///zeilenweise eingelese Ini-Datei(eins zu eins)
            /// </summary>
            public String[] IniTextFileARRAY;            

            /// <summary>
            ///zeilenweise eingelese Ini-Datei ohne Kommentare
            /// </summary>
            public List<String> IniTextFileNoCommentLIST;
            
            /// <summary>
            ///Ini-Datei sortiert nach Gruppen, Items und Varianten
            /// </summary>
            public Dictionary<String, Dictionary<String, Dictionary<String, String>>> IniStructuredDICTIONARY;
            
            /// <summary>
            ///Ini-Lines sortiert nach Gruppen, Items und Varianten
            /// </summary>
            public Dictionary<String, Dictionary<String, Dictionary<String, String>>> IniLineStructuredDICTIONARY;
            
            
            protected List<String> GroupWithDoubleItemLIST;
        }
    }

}