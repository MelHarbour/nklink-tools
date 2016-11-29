using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace LiNKTools.Model
{
    public class Session
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public List<Interval> Intervals = new List<Interval>();
        public int TotalElapsedTime { get; set; }
        public int TotalDistance { get; set; }
        public double AverageStrokeRate { get; set; }
        public int AverageHeartRate { get; set; }
        public double AverageSpeed { get; set; }
        public SpeedInput SpeedInput { get; set; }

        public Session() { }

        public Session(int id, string name, DateTime startTime, int totalElapsedTime, int totalDistance, double averageStrokeRate, int averageHeartRate, double averageSpeed, SpeedInput speedInput)
        {
            Id = id;
            Name = name;
            StartTime = startTime;
            TotalElapsedTime = totalElapsedTime;
            TotalDistance = totalDistance;
            AverageStrokeRate = averageStrokeRate;
            AverageHeartRate = averageHeartRate;
            AverageSpeed = AverageSpeed;
            SpeedInput = speedInput;
        }
    }

    public enum SpeedInput
    {
        Gps,
        Impeller
    }
}
