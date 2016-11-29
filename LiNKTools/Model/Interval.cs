using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcxTools;

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

        public Interval(int id, DateTime startTime, int totalElapsedTime, int totalDistance, double averageStrokeRate, int averageHeartRate, double averageSpeed, int totalStrokeCount)
        {
            Id = id;
            StartTime = startTime;
            TotalElapsedTime = totalElapsedTime;
            TotalDistance = totalDistance;
            AverageStrokeRate = averageStrokeRate;
            AverageHeartRate = averageHeartRate;
            AverageSpeed = averageSpeed;
            TotalStrokeCount = totalStrokeCount;
        }

        public Interval() { }

        public ActivityLap ToLap()
        {
            ActivityLap lap = new ActivityLap();
            lap.StartTime = StartTime;
            lap.TotalTimeSeconds = TotalElapsedTime;
            lap.DistanceMeters = TotalDistance;
            lap.AverageHeartRateBpm = new HeartRateInBeatsPerMinute() { Value = (byte)AverageHeartRate };
            lap.Intensity = Intensity.Active;
            lap.TriggerMethod = TriggerMethod.Manual;
            return lap;
        }
    }
}
