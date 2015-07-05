using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using System.Linq;
using System.Collections.Generic;
using Tweety.Contracts.Interfaces;
using Tweety.Controllers;
using Tweety.Contracts.Entities;
using Microsoft.QualityTools.Testing.Fakes;

namespace Tweety.UnitTests.Controllers
{
    /// <summary>
    /// Tests the StoryFormatter implementation in isolation
    /// </summary>
    [TestClass]
    public class StoryFormatterTests
    {
        //Fake contitions
        private DateTime fakeTime;

        //SUT
        private StoryFormatter sut;

        [TestInitialize]
        public void Initialize()
        {
            //Sets a fake fixed time in order to better check the elapsed string returned by the formatter
            fakeTime = new DateTime(2000, 1, 1, 15, 5, 10);
            
            sut = new StoryFormatter();
        }

        [TestMethod]
        public void Should_FormatStory_When_PostedASecondAgo()
        {
            var story = new Story { Message = "A second ago story", PostedOn = fakeTime.AddSeconds(-1) };
            CoreTestLogic(story, "A second ago story (1 second ago)");
        }

        [TestMethod]
        public void Should_FormatStory_When_PostedSecondsAgo()
        {
            var story = new Story { Message = "Seconds ago story", PostedOn = fakeTime.AddSeconds(-10) };
            CoreTestLogic(story, "Seconds ago story (10 seconds ago)");
        }

        [TestMethod]
        public void Should_FormatStory_When_PostedAMinuteAgo()
        {
            var story = new Story { Message = "A minute ago story", PostedOn = fakeTime.AddSeconds(-70) };
            CoreTestLogic(story, "A minute ago story (1 minute ago)");
        }

        [TestMethod]
        public void Should_FormatStory_When_PostedMinutesAgo()
        {
            var story = new Story { Message = "Minutes ago story", PostedOn = fakeTime.AddMinutes(-7) };
            CoreTestLogic(story, "Minutes ago story (7 minutes ago)");
        }

        [TestMethod]
        public void Should_FormatStory_When_PostedAnHourAgo()
        {
            var story = new Story { Message = "An hour ago story", PostedOn = fakeTime.AddMinutes(-70) };
            CoreTestLogic(story, "An hour ago story (1 hour ago)");
        }

        [TestMethod]
        public void Should_FormatStory_When_PostedHoursAgo()
        {
            var story = new Story { Message = "Hours ago story", PostedOn = fakeTime.AddHours(-4) };
            CoreTestLogic(story, "Hours ago story (4 hours ago)");
        }

        [TestMethod]
        public void Should_FormatStory_When_PostedYesterday()
        {
            var story = new Story { Message = "Yesterday's story", PostedOn = fakeTime.Date.AddDays(-1) };
            CoreTestLogic(story, "Yesterday's story (yesterday)");
        }

        [TestMethod]
        public void Should_FormatStory_When_PostedDaysAgo()
        {
            var story = new Story { Message = "Days ago story", PostedOn = fakeTime.AddDays(-10) };
            CoreTestLogic(story, "Days ago story (10 days ago)");
        }

        /// <summary>
        /// Common test logic called by all the test cases in this class
        /// </summary>
        /// <param name="arrangedStory">Input story to be formatted by the SUT</param>
        /// <param name="expectedResult">Output expected from the SUT</param>
        private void CoreTestLogic(Story arrangedStory, string expectedResult)
        {
            string actual;

            // Arrange
            using (var shimsContext = ShimsContext.Create())
            {
                //Sets a fake fixed time in order to better check the elapsed string returned by the formatter
                System.Fakes.ShimDateTime.NowGet = () => fakeTime;

                // Act
                actual = sut.Format(arrangedStory);
            }

            //Assert
            Assert.AreEqual(expectedResult, actual, "Formatted story different than expected");
        }
    }
}
