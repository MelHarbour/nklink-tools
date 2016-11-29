using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiNKTools.Model;
using TcxTools;

namespace LiNKToolsTests
{
    [TestClass]
    public class IntervalTests
    {
        [TestMethod]
        public void ToLap_ShouldSetIntensityToActive()
        {
            // Arrange
            Interval interval = new Interval();

            // Act
            ActivityLap lap = interval.ToLap();

            // Assert
            Assert.AreEqual(Intensity.Active, lap.Intensity);
        }

        [TestMethod]
        public void ToLap_ShouldSetTriggerMethodToManual()
        {
            // Arrange
            Interval interval = new Interval();

            // Act
            ActivityLap lap = interval.ToLap();

            // Assert
            Assert.AreEqual(TriggerMethod.Manual, lap.TriggerMethod);
        }
    }
}
