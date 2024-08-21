using Journadex.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;

namespace Journadex.Tests
{
    
    /// <summary>
    /// ChatGPT: Write unit tests for the RelativeDateRegex class.
    /// </summary>
    [TestClass]    
    public class RelativeDateRegexTests
    {
        [TestMethod]
        public void TestYesterday()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("Yesterday");
            var relativeTo = new DateTime(2022, 01, 01);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2021, 12, 31));
        }

        [TestMethod]
        public void TestTomorrow()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("Tomorrow");
            var relativeTo = new DateTime(2022, 01, 01);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2022, 01, 02));
        }

        [TestMethod]
        public void TestNextWeek()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("Next week");
            var relativeTo = new DateTime(2022, 01, 01);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2022, 01, 08));
        }

        [TestMethod]
        public void TestLastMonth()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("Last month");
            var relativeTo = new DateTime(2022, 01, 01);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2021, 12, 01));
        }

        [TestMethod]
        public void TestTwoWeeksAgo()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("2 weeks ago");
            var relativeTo = new DateTime(2022, 01, 01);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2021, 12, 18));
        }

        [TestMethod]
        public void LastSundayOnSundayShouldReturnPreviousSunday()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("Last Sunday");
            var relativeTo = new DateTime(2023, 02, 05);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2023, 1, 29));
        }
        [TestMethod]
        public void LastSundayOnMondayShouldReturnPreviousDay()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("Last Sunday");
            var relativeTo = new DateTime(2023, 02, 06);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2023, 2, 05));
        }

        [TestMethod]
        public void LastSundayOnSaturdayShouldReturnPreviousSunday()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("Last Sunday");
            var relativeTo = new DateTime(2023, 02, 03);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2023, 1, 29));
        }

        [TestMethod]
        public void NextSundayOnSundayShouldReturnNextSunday()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("Next Sunday");
            var relativeTo = new DateTime(2023, 02, 05);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2023, 2, 12));
        }

        [TestMethod]
        public void NextSundayOnMondayShouldReturnNextSunday()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("Next Sunday");
            var relativeTo = new DateTime(2023, 02, 06);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2023, 2, 12));
        }

        [TestMethod]
        public void NextSundayOnSaturdayShouldReturnNextDay()
        {
            // Arrange
            var regex = new RelativeDateRegex();
            var match = regex.Match("Next Sunday");
            var relativeTo = new DateTime(2023, 02, 04);

            // Act
            var result = RelativeDateRegex.CalculateDate(match, relativeTo);

            // Assert
            Assert.AreEqual(result, new DateTime(2023, 02, 05));
        }
    }
}
