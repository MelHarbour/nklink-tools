using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcxTools;
using System.Xml;
using System.Xml.Serialization;

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
        public double Power { get; set; }
        public int CatchAngle { get; set; }
        public int FinishAngle { get; set; }
        public int MaxForceAngle { get; set; }
        public double AvgHandleForce { get; set; }
        public double MaxHandleForce { get; set; }
        public int SlipAngle { get; set; }
        public int WashAngle { get; set; }
        public double WorkPerStroke { get; set; }

        public Trackpoint ToTrackpoint(DateTime startTime)
        {
            Trackpoint trackpoint = new Trackpoint();
            trackpoint.Position = new Position() { LatitudeDegrees = Latitude, LongitudeDegrees = Longitude };
            trackpoint.Cadence = (byte)StrokeRate;
            trackpoint.Time = startTime.AddMilliseconds(ElapsedTime).ToUniversalTime();
            trackpoint.HeartRateBpm = new HeartRateInBeatsPerMinute() { Value = (byte)HeartRate };

            NKEmpowerTrackpointExtension_t extensionData;
            if (Power != 0)
            {
                extensionData = new NKEmpowerTrackpointExtension_t(SpeedGps, Power, CatchAngle, FinishAngle, MaxForceAngle, AvgHandleForce, MaxHandleForce, SlipAngle, WashAngle, WorkPerStroke);
            }
            else
            {
                extensionData = new NKEmpowerTrackpointExtension_t(SpeedGps);
            }

            XmlDocument doc = new XmlDocument();

            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                new XmlSerializer(typeof(NKEmpowerTrackpointExtension_t)).Serialize(writer, extensionData);
            }

            trackpoint.Extensions.Any.Add(doc.DocumentElement);

            return trackpoint;
        }
    }

    public enum DataRecordType
    {
        Stroke
    }
}
