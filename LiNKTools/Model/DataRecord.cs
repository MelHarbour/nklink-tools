using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiNKTools.Model
{
    public class DataRecord
    {
        public int Id { get; set; }
        public DataRecordType Type { get; set; }
        public int ElapsedTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double SpeedGps { get; set; }
        public double SpeedImpeller { get; set; }
        public int DistanceGps { get; set; }
        public int DistanceImpeller { get; set; }
        public double StrokeRate { get; set; }
        public int HeartRate { get; set; }
    }

    public enum DataRecordType
    {
        Stroke
    }
}
