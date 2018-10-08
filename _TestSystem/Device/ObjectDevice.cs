using System;
using Honeywell.Test;


namespace Honeywell
{
    namespace Device
    {       
        /// <summary>
        /// Template-Klasse f�r die Device-Klassen
        /// </summary>
        public class CObjectDevice : CObjectContainer
        {
            public CObjectDevice(string Name, int ModeReport) : base(Name,  ModeReport)
            {                
                CObjectTest.PoolIDDevice++;
                this.Create();                
            }
            ~CObjectDevice()
            {
                CObjectTest.PoolIDDevice--;
            }
            

            /// <summary>
            ///Wird im Konstrukter aufgerufen
            /// </summary>
            public new bool Create()
            {
                this.IDType = 2;
                this.ID = CObjectTest.PoolIDDevice;
				//this.Timer.Timer.Name = this.Name;	
                return (true);
            }

			/// <summary>
			/// �berpr�ft ob das Ger�t "NameDevice" in der Liste "ListOfAllowedDevice" vorhanden ist.
			/// Ist die Liste leer sind alle Ger�te zugelassen
			/// </summary>
			/// <param name="NameDevice">
			/// Name des zu �berpr�fenden Ger�tes
			/// </param>
			/// <param name="ListOfAllowedDevice">
			/// Liste von Ger�ten die zugelassen sind
			/// </param>
			/// <returns>
			/// true - wenn "NameDevice" in der Liste "ListOfAllowedDevice" vorhanden ist
			/// false - wenn "NameDevice" nicht in der Liste "ListOfAllowedDevice" vorhanden ist
			/// </returns>
			public bool IsDeviceAllowed(string NameDevice, string[] ListOfAllowedDevice)
			{
				if(ListOfAllowedDevice.Length == 0)
					return true;

				foreach(string strName in ListOfAllowedDevice)
				{
					if(NameDevice.IndexOf(strName) != -1)
						return true;
				}

				this.Error = string.Format("Device `{0}` is not allowed to use", NameDevice);
				return false;				
			}
        }
    }
}
