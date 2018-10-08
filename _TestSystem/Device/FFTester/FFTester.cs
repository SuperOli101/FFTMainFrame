using System;
using System.Collections.Generic;
using Honeywell.Test;


namespace Honeywell.Device
{
	/// <summary>
	/// Klasse, die die FFT Funktionen enthält
	/// </summary>
	public class CFFTester : CObjectDevice
	{
		public CFFTester()
			: base("FFTester", 1)
		{					
		}

		public CFFTester(string Name, int Report)
			: base(Name, Report)
		{
		}

		/// <summary>
		/// Druckt Msg auf dem Error Drucker 
		/// </summary>
		/// <param name="Msg">
		/// Message zum Drucken
		/// </param>
		/// <returns>
		/// Ergebnis
		/// </returns>
		public virtual bool DoPrintError(string Msg) { return true; }
		/// <summary>
		/// Macht einen Vorschub beim Errorprinter
		/// </summary>
		/// <param name="Count">
		/// Anzahl von Vorschuben
		/// </param>
		/// <returns>
		/// Ergebnis
		/// </returns>
		public virtual bool DoFeedErrorPrinter(int Count = 1) { return true; }


        public virtual void PrintFromFFT()
        {

        }

        public void CreateListOfFunction()
        {
            
            Type type = this.GetType();
            System.Reflection.MethodInfo[] methods = type.GetMethods();
            List<System.Reflection.MethodInfo> methodsForMenuList = new List<System.Reflection.MethodInfo>(methods.Length);

           
            foreach (System.Reflection.MethodInfo method in methods)
            {
                if (method.Name.IndexOf("__") == 0)
                {
                    methodsForMenuList.Add(method);
                }              
            }

            if (methodsForMenuList.Count != 0)
            {
                this.MethodsForMenu = methodsForMenuList.ToArray();
            }
            return;
        }

        public System.Reflection.MethodInfo[] MethodsForMenu = null;


	}
}
