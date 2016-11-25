using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiNKTools.Model
{
    public class Interval
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public int TotalElapsedTime { get; set; }
        public int TotalDistance { get; set; }
        public double AverageStrokeRate { get; set; }
        public int AverageHeartRate { get; set; }
        public double AverageSpeed { get; set; }
        public int TotalStrokeCount { get; set; }

        public List<DataRecord> DataRecords = new List<DataRecord>();

        public Interval(int id, long startTime, int totalElapsedTime, int totalDistance, double averageStrokeRate, int averageHeartRate, double averageSpeed, int totalStrokeCount)
        {
            Id = id;
            SetStartTime(startTime);
            TotalElapsedTime = totalElapsedTime;
            TotalDistance = totalDistance;
            AverageStrokeRate = averageStrokeRate;
            AverageHeartRate = averageHeartRate;
            AverageSpeed = averageSpeed;
            TotalStrokeCount = totalStrokeCount;
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
