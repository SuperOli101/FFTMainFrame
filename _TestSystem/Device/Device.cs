using System;
using Honeywell.Test;
using System.Collections.Generic;
using Honeywell.Data;


namespace Honeywell
{
    namespace Device
    {
        /// <summary>
        /// In dieser Klasse werden die im FFT verwendete geräte initialisiert.
        /// Die Klasse enthält auch die Referenze auf die instalierten Geräte.
        /// </summary>
        public class CDevice : CObjectDevice
        {
            public const int OFF = 0;
            public const int ON = 1;

			//----------------------------------------------------------
            /// <summary>
            /// Konstruktor zum Initialisieren von CDevice Klasse
            /// </summary>
            public CDevice(string Name, int ModeReport)
				: base(Name, ModeReport)
            {
                
            }

            
			//----------------------------------------------------------
			/// <summary>
			/// In dieser Funktion werden die variantenunabhängigen Geräte initialisiert
			/// </summary>
			public override bool Initialize()
			{
				this.FFTester = new CFFTester();
                if (this.Test.FormFFT != null)
                {
                    if (this.Data.FFTMode == 0)
                    {
                        this.Test.FormFFT.TextBoxFFT.BackColor = System.Drawing.SystemColors.Control;
                    }
                    else
                    {
                        this.Test.FormFFT.TextBoxFFT.BackColor = System.Drawing.Color.Yellow;
                    }
                }

				return (true);
			}

            /// <summary>
            /// In dieser Funktion werden die variantenabhängigen Geräte initialisiert
            /// </summary>
            /// <param name="_Variant">
            /// this.Test.CurrentID_Menu wird von MainFrame über _Variant übergeben.
            /// </param>
            /// <returns>
            /// Ergebnis von der Initialisierung
            /// </returns>
            public virtual bool Initialize(int _Variant)
            {
				
                return (true);
            }


			private CFFTester ffTester;

			public virtual CFFTester FFTester
			{
				get { return ffTester; }
				set { ffTester = value; }
			}
        }

    }
}
 
              
