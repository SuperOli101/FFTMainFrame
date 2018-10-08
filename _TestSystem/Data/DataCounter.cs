using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace Honeywell.Data
{
    public class CDataCounter : CObjectData
    {
        /// <summary>
        /// Gets or sets filename with path and extension and placeholder
        /// </summary>
        public string FullNamePrototype { get; set; }

        /// <summary>
        /// Gets or sets the VariantName, needed for %v placeholder
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// Gets or sets the OsNumberName, needed for %o placeholder
        /// </summary>
        public string OsNumberName { get; set; }

        /// <summary>
        /// Initializes new instance of a CDataCounter Class
        /// </summary>
        /// <param name="fullNamePrototype"></param>
        /// <param name="variantName"></param>
        /// <param name="osNumberName"></param>
        public CDataCounter(string fullNamePrototype, string variantName, string osNumberName)
            : base(fullNamePrototype)
        {
            this.FullNamePrototype = fullNamePrototype;
            this.VariantName = variantName;
            this.OsNumberName = osNumberName;
        }

        /// <summary>
        /// Increments the given Counter, and creates it when it's not exists
        /// </summary>
        /// <param name="counterName">Name of counter to increment</param>
        public void Count(string counterName)
        {
            string fileName;
            string fileContent;
            string[] fileContentRows;
            bool fileCorrupt = false;
            Dictionary<string, int> Items = new Dictionary<string, int>();
            FileInfo counterFile; 

            if (String.IsNullOrWhiteSpace(counterName))
                throw new ArgumentException("Der counterName Parameter darf nicht null oder leer sein");

            fileName = CreateFileName();
            counterFile = new FileInfo(fileName);

            #region // If File doesn't exists, create

            try
            {
                if (!counterFile.Exists)
                {
                    if (!counterFile.Directory.Exists)
                    {
                        counterFile.Directory.Create();
                    }

                    using (counterFile.CreateText()) { }
                }
            }
            catch { }

            #endregion

            #region // Read out Data, and Parse

            try
            {
                using(StreamReader counterReader = counterFile.OpenText())
                {
                    fileContent = counterReader.ReadToEnd();
                    counterReader.Close();
                }

                fileContentRows = 
                    fileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                for(int index = 0; index < (fileContentRows.Length / 2); index++)
                {
                    int itemCount;

                    if (!Int32.TryParse(fileContentRows[index * 2 + 1], out itemCount))
                    {
                        fileCorrupt = true;
                        break;
                    }

                    Items.Add(fileContentRows[index * 2], itemCount);
                }

                if (fileCorrupt)
                {
                    counterFile.MoveTo(counterFile.FullName + 
                        DateTime.Now.ToString(".HHmmss-ddMMYYYY") + ".fail");
                    this.Count(counterName);
                    return;
                }
            }
            catch
            { }

            #endregion

            #region // Count up, Write back

            if (Items.ContainsKey(counterName))
                Items[counterName]++;
            else
                Items.Add(counterName, 1);

            try
            {
                using (StreamWriter counterWrite = counterFile.CreateText())
                {
                    foreach (KeyValuePair<string, int> item in Items)
                    {
                        counterWrite.WriteLine(item.Key);
                        counterWrite.WriteLine(item.Value.ToString());
                    }

                    counterWrite.Close();
                }
            }
            catch { }

            #endregion
        }

        private string CreateFileName()
        {
            string actualFileName = this.FullNamePrototype;
            
            Dictionary<string, string> replacments = new Dictionary<string, string>()
            {
                {   "%m", DateTime.Now.ToString("mm")                           },
                {   "%h", DateTime.Now.ToString("HH")                           },
                {   "%d", DateTime.Now.ToString("dd")                           },
                {   "%c", GetIso8601WeekOfYear(DateTime.Now).ToString("00")     },
                {   "%M", DateTime.Now.ToString("MM")                           },
                {   "%y", DateTime.Now.ToString("yyyy")                         },
                {   "%v", this.VariantName                                      },
                {   "%o", this.OsNumberName                                     }
            };

            foreach(KeyValuePair<string, string> item in replacments)
            {
                actualFileName = actualFileName.Replace(item.Key, item.Value);
            }

            return actualFileName;
        }

        private static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
