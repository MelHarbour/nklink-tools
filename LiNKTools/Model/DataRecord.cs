using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcxTools;
using System.Xml;

namespace LiNKTools.Model
{
    public class DataRecord
    {
        XmlDocument doc = new XmlDocument();

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

        public Trackpoint ToTrackpoint(DateTime startTime)
        {
            Trackpoint trackpoint = new Trackpoint();
            trackpoint.Position = new Position() { LatitudeDegrees = Latitude, LongitudeDegrees = Longitude };
            trackpoint.Cadence = (byte)StrokeRate;
            trackpoint.Time = startTime.AddMilliseconds(ElapsedTime).ToUniversalTime();
            trackpoint.HeartRateBpm = new HeartRateInBeatsPerMinute() { Value = (byte)HeartRate };

            XmlElement speed = doc.CreateElement("Speed");
            speed.InnerText = SpeedGps.ToString();
            trackpoint.Extensions.Any.Add(speed);
            return trackpoint;
        }
    }

    public enum DataRecordType
    {
        Stroke
    }
}
