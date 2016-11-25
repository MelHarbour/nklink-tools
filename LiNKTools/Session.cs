using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace LiNKTools
{
    public class Session
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }

        public Session() { }

        public Session(string name, long startTime)
        {
            Name = name;
            SetStartTime(startTime);
        }

        private void SetStartTime(long value)
        {
            string hexValue = value.ToString("X").PadLeft(14, '0');

            int year = int.Parse(hexValue.Substring(0, 4), NumberStyles.HexNumber);
            int month = int.Parse(hexValue.Substring(4, 2), NumberStyles.HexNumber);
            int day = int.Parse(hexValue.Substring(6, 2), NumberStyles.HexNumber);
            int hour = int.Parse(hexValue.Substring(8, 2), NumberStyles.HexNumber);
            int minute = int.Parse(hexValue.Substring(10, 2), NumberStyles.HexNumber);
            int second = int.Parse(hexValue.Substring(12, 2), NumberStyles.HexNumber);
            
            StartTime = new DateTime(year, month, day, hour, minute, second);
        }
    }
}
