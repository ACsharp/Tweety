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
    /// Tests the AggregatedStoryFormatter implementation in isolation
    /// </summary>
    [TestClass]
    public class AggregatedStoryFormatterTests
    {
        //Dependencies
        private IEntityFormatter<Story> fakeStoryFormatter;

        //SUT
        private AggregatedStoryFormatter sut;

        [TestInitialize]
        public void Initialize()
        {
            //Creates a fake IEntityFormatter<Story> object that formats in an arbitrary test-recognizable way
            //just to assert that the SUT relies on it
            fakeStoryFormatter = new Contracts.Interfaces.Fakes.StubIEntityFormatter<Story>
            {
                FormatT0 = story => string.Format("FakeFormattedStory:{0}", story.Message)
            };

            sut = new AggregatedStoryFormatter(fakeStoryFormatter);
        }

        /// <summary>
        /// Verifies that AggregatedStoryFormatter formats an AggregatedStory object as expected
        /// </summary>
        [TestMethod]
        public void Should_FormatAggregatedStory()
        {
            // Arrange
            var aggregatedStory = new AggregatedStory
            {
                Story = new Story { Message = "Fake story", PostedOn = DateTime.Now },
                BelongingTo = new User { Name = "Alice" }
            };

            // Act
            var actual = sut.Format(aggregatedStory);

            //Assert
            Assert.AreEqual("Alice - FakeFormattedStory:Fake story", actual, "Formatted aggregated story different than expected");
        }

    }
}
