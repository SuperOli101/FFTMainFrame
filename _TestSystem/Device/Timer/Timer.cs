using System;
using System.Collections.Generic;
using Honeywell.Test;


//System.Diagnostics.Stopwatch
//System.Diagnostics.Stopwatch Timer = new System.Diagnostics.Stopwatch();
namespace Honeywell.Device
{
	/// <summary>
	/// Klasse zum Zeitmessen
	/// </summary>
	public class CTimer : CObjectDevice
	{
		public CTimer()
			: base("Timer", 1)
		{
			this.timerDictionary = new Dictionary<int, long[]>(100);
			this.countdownDictionary = new Dictionary<int, long[]>(100);			
		}


		public enum Unit : int
		{
					
			mSec=0,
			Sec
		}
       
		//-----------------------------------------------------------------------------
		/// <summary>
        /// Setz Timer auf 0
		/// </summary>
		/// <param name="Number">
        /// Timernummer
        /// </param>
		/// <param name="Mode">
        /// CTimer.Unit.Sec
        /// </param>
		public void SetTimer(int Number, CTimer.Unit Mode=CTimer.Unit.mSec)
		{					
			try
			{
				
				long[] longArray;
				longArray = new long[2];
				longArray[0] = DateTime.Now.Ticks;
				longArray[1] = (long)Mode;

				this.timerDictionary.Add(Number, longArray);				
			}
			catch
			{
				this.timerDictionary[Number][0] = DateTime.Now.Ticks;
				this.timerDictionary[Number][1] = (long)Mode;				
			}			
		}
		/// <summary>
		/// Liefert wie viel Zeit ist vergangen seit dem der gesetzt wurde
		/// </summary>
		/// <param name="Number">
		/// Timernummer
		/// </param>
		/// <returns>
		/// -1 wenn ein Fehler augetretten
		/// Sonst die Zeit in mSec oder in Sec (je nach Unit in SetTimer) seit dem letzten Aufruf von SetTimer
		/// </returns>
		public double GetTimer(int Number)
		{
			double dValue=-1;
			
			try
			{
				dValue = this.GetDurationTimer(Number);				
			}
			catch
			{
			}

			return dValue;
		}

		//-----------------------------------------------------------------------------
		/// <summary>
		/// Setzt Countdown auf den Wert gleich Value
		/// mit GetCountdown wird gewartet bis Coundown abgelaufen ist.
		/// Wird benutz um die bestimmte Delays abzuhalten und zwischen SetCountdown und WaitCountdownUp was zu machen
		/// </summary>
		/// <param name="Number">
		/// Countdown Nummer
		/// </param>		
		/// <param name="Value">
		/// Wert für Countdown
		/// </param>
		/// <param name="Unit">
		/// 0 - Milisek; 1 - Sek
		/// </param>
		public void SetCountdown(int Number, double Value, CTimer.Unit Mode = CTimer.Unit.mSec)
		{
			try
			{
				long[] longArray;
				longArray = new long[3];
				longArray[0] = DateTime.Now.Ticks;
				longArray[1] = (long)Mode;
				longArray[2] = (long)Value;

				this.countdownDictionary.Add(Number, longArray);
			}
			catch
			{
				this.countdownDictionary[Number][0] = DateTime.Now.Ticks;
				this.countdownDictionary[Number][1] = (long)Mode;
				this.countdownDictionary[Number][2] = (long)Value;
			}		

			return;
		}

		/// <summary>
		/// Warten bis Countdown abgelaufen ist
		/// </summary>
		/// <param name="Number">
		/// Countdown Nummer
		/// </param>
		/// <returns>
		/// true Test soll abgebrochen werden
		/// false Test weiter machen
		/// </returns>
		public bool WaitCountdownUp(int Number)
		{
			bool bFlagTestCancel = false;
			double dTime;
			double dTimeOutOffset;
			CTimer timer;
			long lDelay;


			try
			{
				dTimeOutOffset = this.countdownDictionary[Number][2];

				timer = new CTimer();
				timer.SetTimer(1);
				dTime = this.GetDurationCountdown(Number);
				while(dTime < dTimeOutOffset)
				{
					dTime = this.GetDurationCountdown(Number);
					if(this.Test.CancelTest() == true)
					{
						bFlagTestCancel = true;
						break;
					}
                    System.Windows.Forms.Application.DoEvents();
					System.Threading.Thread.Sleep(10);
				}

				lDelay = (long)timer.GetTimer(1);
				dTime = this.GetDurationCountdown(Number);

				CObjectTest.DelayTotal_Sec += (lDelay * 1000);
				if(this.countdownDictionary[Number][1] != (long)CTimer.Unit.mSec)
					dTimeOutOffset *= 1000D;
				this.WriteLineToReport(string.Format("Delay from WaitCountdownUp ({0} mSec) = {1} mSec", (long)dTimeOutOffset, lDelay));
			}
			catch
			{
			}

			return bFlagTestCancel;			
		}

		//-----------------------------------------------------------------------------
		/// <summary>
		/// Führt ein Delay_mSec von Delay_mSec aus
		/// </summary>
		/// <param name="Delay_mSec">
		/// Delay in mSec
		/// </param>
		/// <returns>
		/// true Test soll abgebrochen werden
		/// false Test weiter machen
		/// </returns>
		public bool SleepAsDelay(long Delay_mSec)
		{
			bool bFlagTestCancel = false;
			long lDelay;
			double dDelay;
			CTimer timer = new CTimer();


			timer.SetTimer(1);

			dDelay=timer.GetTimer(1);
			while(dDelay < Delay_mSec)
			{
				dDelay = timer.GetTimer(1);
				if(test.CancelTest() == true)
				{
					bFlagTestCancel = true;
					break;
				}
				System.Windows.Forms.Application.DoEvents();				
			}


			lDelay = (long)timer.GetTimer(1);
			CObjectTest.DelayTotal_Sec += (lDelay*1000);
			this.WriteLineToReport(string.Format("::Sleep from {0}.Timer = {1} mSec", this.Name, lDelay));
			
			return bFlagTestCancel;
		}

		//-----------------------------------------------------------------------------
		/// <summary>
		/// Gibt wie lange es dauerte zwischen einem Zeitpunkt in der Vergangenheit und jetzt in mSec und Sec
		/// </summary>
		/// <param name="TimeInThePast">
		/// Startzeitpunkt
		/// </param>
		/// <param name="Unit">
		/// mSec = 0
		///	Sec  = 1
		/// </param>
		/// <returns>
		/// Zeitdauer (abhänig von Unit)
		/// </returns>
		public double GetDuration(DateTime TimeInThePast, CTimer.Unit Mode = CTimer.Unit.mSec)
		{
			TimeSpan timeSpan;
			double dValue;

			timeSpan = DateTime.Now - TimeInThePast;
			if(Mode == CTimer.Unit.mSec)
			{
				dValue = timeSpan.TotalMilliseconds;
			}
			else
			{
				dValue = timeSpan.TotalSeconds;
			}

			return dValue;
		}
		public double GetDurationTimer(int Number)
		{			
			return this.GetDuration(new DateTime(this.timerDictionary[Number][0]), (CTimer.Unit)this.timerDictionary[Number][1]);
		}
		public double GetDurationCountdown(int Number)
		{
			return this.GetDuration(new DateTime(this.countdownDictionary[Number][0]), (CTimer.Unit)this.countdownDictionary[Number][1]);
		}

		//-----------------------------------------------------------------------------
		/// <summary>
		/// Timernummer mit der Zeit wann er gesetzt wurde (Erstes Element im array) und der Unit (Zweites Element im array) in was es gerechnet wird; 
		/// Unit 0 - Milisek; 1 - Sek
		/// Wird benutzt um auszurechnet wie viel ist Zeit vergangen nach dem Aufruf von SetTimer()
		/// </summary>
		protected Dictionary<int, long[]> timerDictionary;
		/// <summary>
		/// Timernummer mit der Zeit (Erstes Element im array) wie lange in der Schleife drehen soll, 
		/// bis die gleichzeitg gespeichert Zeit (drittes Ellement im array) abgelauf ist.
		/// Unit (Zweites Element im array) 0 - Milisek; 1 - Sek
		/// Countdowndauer ist das dritte Ellement im array
		/// </summary>
		protected Dictionary<int, long[]> countdownDictionary;		

	}
}
