using System;
using System.Collections.Generic;
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
    }
}
