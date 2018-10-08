using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Honeywell.Test;

namespace MainFrame
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            //Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CTest hTest;
            bool bResult;
            //String strMsg, strBuffer;

            hTest = new CTest();
            hTest.Name = "FFT-Test";            
            if (args.Length != 0)
            {
                hTest.ArgumentCommandLineCurrentVariant = args[0];//Current Variant als argument übergeben
                if (args.Length > 1)
                {
                    hTest.ArgumentCommandLineFFTMode = args[1];//FFTMode als argument übergeben
                }
            }

            bResult = hTest.Initialize();

            if (!bResult || hTest.CurrentID_Menu == 0)
            {
                if (hTest.Error.Length != 0)
                {

                    if (hTest.Data != null && hTest.Data.Report != null)
                    {
                        if (hTest.Data.Report.Error.Length == 0)
                        {
                            hTest.WriteLineToReport(hTest.Error);
                            hTest.Data.Report.WriteToFileAppend(hTest.Data.Report.NameFull);
                        }
                    }
                    hTest.ShowErrorInitialize();                   
                }
                hTest.Deinstall();
                Application.Exit();
                return (1);
            }


            // Hauptfenster erstellen und ausführen
            Application.Run(new Honeywell.Forms.CFormMainFrame(hTest));

            return (0);
        }
    }
}