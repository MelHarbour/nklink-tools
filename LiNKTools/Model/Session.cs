﻿using System;
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

        public Session(int id, string name, long startTime, int totalElapsedTime, int totalDistance, double averageStrokeRate, int averageHeartRate, double averageSpeed, SpeedInput speedInput)
        {
            Id = id;
            Name = name;
            SetStartTime(startTime);
            TotalElapsedTime = totalElapsedTime;
            TotalDistance = totalDistance;
            AverageStrokeRate = averageStrokeRate;
            AverageHeartRate = averageHeartRate;
            AverageSpeed = AverageSpeed;
            SpeedInput = speedInput;
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

    public enum SpeedInput
    {
        Gps,
        Impeller
    }
}
