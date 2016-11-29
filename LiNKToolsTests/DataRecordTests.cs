using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiNKTools.Model;
using TcxTools;

namespace LiNKToolsTests
{
    [TestClass]
    public class DataRecordTests
    {
        [TestMethod]
        public void ToTrackpoint_ShouldMapToPosition()
        {
            // Arrange
            DataRecord record = new DataRecord();
            record.Latitude = 1;
            record.Longitude = 2;

            // Act
            Trackpoint trackpoint = record.ToTrackpoint(DateTime.Now);

            // Assert
            Assert.AreEqual(1, trackpoint.Position.LatitudeDegrees);
            Assert.AreEqual(2, trackpoint.Position.LongitudeDegrees);
        }

        [TestMethod]
        public void ToTrackpoint_GivenStartTime_ShouldAddToElapsedTime()
        {
            // Arrange
            DataRecord record = new DataRecord();
            record.ElapsedTime = 100;
            DateTime startTime = DateTime.Now;

            // Act
            Trackpoint trackpoint = record.ToTrackpoint(startTime);

            // Assert
            Assert.AreEqual(startTime.AddMilliseconds(record.ElapsedTime), trackpoint.Time);
        }
    }
}
