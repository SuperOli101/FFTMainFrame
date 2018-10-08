using System;
using Honeywell.Test;
using System.Collections.Generic;


namespace Honeywell
{
    namespace Data
    {
        /// <summary>
        /// Für das TestMainFrame optimierte CSerializerIni.
        /// z.B.    Ermöglicht variantenabhängige Variantenbelegung.
        ///         Man kann z.B. gleich double oder int Variablen übergeben ohne vorher an Objekt-Variable zuweisen
        /// </summary>
        public class CDataIni : CSerializerIni
        {
            public CDataIni(String NameFull) : base(NameFull)
            {

            }

            /// <summary>
            ///Return 0 - O.K.; 1 - Error
            /// </summary>
            public int SetValue(String Group, String Item, String Value)      
            {
                object hValue;
                hValue = (object)Value;

                if (this.SetValue(Group, Item, hValue))
                {
                    return (0);
                }
                else
                {
                    return (1);
                }
            }

            /// <summary>
            ///Return 0 - O.K.; 1 - Error
            /// </summary>
            public int SetValue(String Group, String Item, double Value)            
            {
                object hValue;
                hValue = (double)Value;

                if (this.SetValue(Group, Item, hValue))
                {
                    return (0);
                }
                else
                {
                    return (1);
                }
            }

			/// <summary>
			/// zeilenweise eingelese Ini-Datei(eins zu eins)
			/// </summary>
            /// <param name="Group"></param>
            /// <param name="Item"></param>
            /// <param name="Value"></param>
            /// <returns>
			/// Return 0 - O.K.; 1 - Error
			/// </returns>
            public int SetValue(String Group, String Item, int Value)            
            {
                object hValue;
                hValue = (int)Value;

                if (this.SetValue(Group, Item, hValue))
                {
                    return (0);
                }
                else
                {
                    return (1);
                }
            }

            /// <summary>
            ///Return 0 - O.K.; 1 - Error
            /// </summary>
            public int SetValue(String Group, String Item, bool Value)            
            {
                object hValue;
                hValue = (bool)Value;

                if (this.SetValue(Group, Item, hValue))
                {
                    return (0);
                }
                else
                {
                    return (1);
                }
            }

            /// <summary>
            ///Return 0 - O.K.; 1 - Error
            /// </summary>
            public int GetValue(String Group, String Item, out String Value)            
            {
                object hValue;
                Value = "";
                hValue = Value;

                if (this.GetValue(Group, Item, ref hValue))
                {
                    Value = (String)hValue;
                    return (0);
                }
                else
                {
                    return (1);
                }
            }

            /// <summary>
            ///Return 0 - O.K.; 1 - Error
            /// </summary>
            public int GetValue(String Group, String Item, out double Value)            
            {
                object hValue;
                Value = 0;
                hValue = Value;

                if (this.GetValue(Group, Item, ref hValue))
                {
                    Value = (double)hValue;
                    return (0);
                }
                else
                {
                    return (1);
                }
            }

            /// <summary>
            ///Return 0 - O.K.; 1 - Error
            /// </summary>
            public int GetValue(String Group, String Item, out int Value)            
            {
                object hValue;
                Value = 0;
                hValue = Value;

                if (this.GetValue(Group, Item, ref hValue))
                {
                    Value = (int)hValue;
                    return (0);
                }
                else
                {
                    return (1);
                }
            }

            /// <summary>
            ///Return 0 - O.K.; 1 - Error
            /// </summary>
            public int GetValue(String Group, String Item, out uint Value)
            {
                object hValue;
                Value = 0;
                hValue = Value;

                if (this.GetValue(Group, Item, ref hValue))
                {
                    Value = (uint)hValue;
                    return (0);
                }
                else
                {
                    return (1);
                }
            }

			/// <summary>
			///Return 0 - O.K.; 1 - Error
			/// </summary>
			public int GetValue(String Group, String Item, out byte Value)
			{
				object hValue;
				Value = 0;
				hValue = Value;

				if(this.GetValue(Group, Item, ref hValue))
				{
					Value = (byte)hValue;
					return (0);
				}
				else
				{
					return (1);
				}
			}

            /// <summary>
            ///Return 0 - O.K.; 1 - Error
            /// </summary>
            public int GetValue(String Group, String Item, out bool Value)            
            {
                object hValue;
                Value = true;
                hValue = Value;

                if (this.GetValue(Group, Item, ref hValue))
                {
                    Value = (bool)hValue;
                    return (0);
                }
                else
                {
                    return (1);
                }
            }



            /// <summary>
            /// Die Methode überschrieben und liefert in CSerializerIni.GetValue und CSerializerIni.SetValue
            /// die gewählte Variantename statt "default" in CSerializerIni.GetVariantCurrent.
            /// </summary>
            protected override string getVariantCurrent(String Group, String Item)
            {
                Dictionary<String, String> Variants;
                Variants = this.IniStructuredDICTIONARY[Group][Item];
                int iCount = Variants.Count;
                String strVariant;
                if(iCount==1)
                    return base.getVariantCurrent(Group, Item);
                foreach (String strVariantCurrent in this.Test.CurrentID_MenuWithAllAliasARRAY)
                {
                    if (strVariantCurrent==null)
                        return base.getVariantCurrent(Group, Item);
                    foreach (KeyValuePair<String, String> hVariant in Variants)
                    {
                        strVariant = hVariant.Key;
                        if (strVariantCurrent==strVariant)
                            return (strVariant);
                    }                    
                }
                return base.getVariantCurrent(Group, Item);
            }
        }

    }
}