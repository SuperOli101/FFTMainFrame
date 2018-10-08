using System;
using Honeywell.Test;

using System.Data.OleDb;
using Honeywell.Data;
using Honeywell.Device;



namespace Honeywell
{
    namespace Test
    {
        /// <summary>
        /// Um einen neuen Testschritt anzulegen, folgen Sie folgenden Anweisungen:
        /// 1.  Legen Sie die cs-Datei für die neuen FFT-Testschritte Klassen (z.B. FFT_XL12.sc)
        /// 2.  Kopieren Sie die CFFT_Template Klasse aus der FFT_Template.cs in die neue angelegte cs-Datei
        /// 3.  Umbennen Sie die neuangelegte Klasse  (z.B. CFFT18_XL12_CheckRelay)
        /// 4.  Legen Sie die neue Konstante in CTest aus Test.sc für die neuangelegte Klasse (z.B. public const int FFT_Template_Test1 = 2;)
        /// 5.  Instanzieren Sie die neuangelegte Klasse in public override CTestStep CTest.CreateTestStep(int ID_TestStep) aus Test.sc
        ///     case CTest.FFT_Template_Test1:
        ///         hTestStep = new CFFT_Template_Test1();
		///		    break;
        /// 6.  Legen Sie die neue Klasse in der Sys-Datei an(z.B. FFT_Template_Test1					=                   =			//2)
        /// 7.  Zum Verwenden der neuen Klasse fügen Sie die in die entsprechende Sequenz ein (z.B [SEQUENCING_XFC3D06001] FFT_Template_Test1)
        /// 8.  Legen Sie die neue Klasse in der Msg-Datei an 
        ///     [FFT_Template_Test1]
        ///     Name	= "Relay-Test"
        ///     1		= ""
        ///     :10		= ""
        /// 9. Jetzt wird die neue Klasse vom FFT-SW benutzt
        /// </summary>
        public class CFFT_Template : CTestStep
        {
	        protected override void _SetTestStep()
            {

	            return;
            }
	        protected override void _ExecuteTestStep()
            {

	            return;
            }
	        protected override void _ResetTestStep() 
            {

	            return;
            }
        }                         
               

        public class CFFT_Template_Start : CFFT_Template
        {
	        protected override void _SetTestStep()
            {
                /*
				this.WriteToScroll("im Start " + System.DateTime.Now.ToString());
				this.WriteToScroll(((DateTime)this.Data.DB.Tab_ProductionStep.Date).ToString("dd.MM.yyyy HH:mm:ss.fff"));
				this.WriteToScroll(this.Data.DB.Tab_ProductionStep.ProductionStepId.ToString());
                 * */
	            return;
            }
	        protected override void _ExecuteTestStep()
            {				
	            return;
            }
	        protected override void _ResetTestStep() 
            {

	            return;
            }
        }                            

        public class CFFT_Template_Finish : CFFT_Template
        {
	        protected override void _SetTestStep()
            {                

	            return;
            }
	        protected override void _ExecuteTestStep()
            {				

	            return;
            }
	        protected override void _ResetTestStep() 
            {				
                
	            return;
            }
        }    

        public class CFFT_Template_Test1 : CFFT_Template
        {
	        protected override void _SetTestStep()
            {
	            return;
            }
	        protected override void _ExecuteTestStep()
            {				
	            return;
            }
	        protected override void _ResetTestStep() 
            {
                
	            return;
            }
        }

        public class CFFT_Template_Test2 : CFFT_Template
        {
            protected override void _SetTestStep()
            {
                
                return;
            }
            protected override void _ExecuteTestStep()
            {
                return;
            }
            protected override void _ResetTestStep()
            {
                
                return;
            }
        }
                               
    }
}
