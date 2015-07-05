using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using System.Linq;
using System.Collections.Generic;
using Tweety.Contracts.Interfaces;
using Tweety.Controllers;
using Tweety.Contracts.Entities;

namespace Tweety.UnitTests.Controllers
{
    /// <summary>
    /// Tests the WallCommand implementation in isolation
    /// </summary>
    [TestClass]
    public class WallCommandTests
    {
        //Observers
        private StubObserver aggregatorServiceObserver;
        private StubObserver aggregatedStoryFormatterObserver;

        //Fake dependencies
        private IAggregatorService aggregatorService;
        private IEntityFormatter<AggregatedStory> aggregatedStoryFormatter;

        //Fake entities
        private AggregatedStory[] fakeAggregatedStories;

        //SUT
        private WallCommand sut;

        [TestInitialize]
        public void Initialize()
        {
            fakeAggregatedStories = new AggregatedStory[]
            {
                new AggregatedStory { Story = new Story { Message = "First story", PostedOn = new DateTime(2000, 1, 1, 15, 1, 0) }},
                new AggregatedStory { Story = new Story { Message = "Second story", PostedOn = new DateTime(2000, 1, 1, 15, 5, 0) }}
            };

            //Creates a fake IAggregatorService with a liked observer in order to
            //assert that the SUT relies on it
            aggregatorServiceObserver = new StubObserver();
            aggregatorService = new Contracts.Interfaces.Fakes.StubIAggregatorService
            {
                InstanceObserver = aggregatorServiceObserver,
                GetAggregatedStoriesString = userName => fakeAggregatedStories
            };

            aggregatedStoryFormatterObserver = new StubObserver();
            aggregatedStoryFormatter = new Contracts.Interfaces.Fakes.StubIEntityFormatter<AggregatedStory>
            {
                InstanceObserver = aggregatedStoryFormatterObserver,
                FormatT0 = aggregatedStory => aggregatedStory.Story.Message
            };

            sut = new WallCommand(aggregatorService, aggregatedStoryFormatter);
        }

        /// <summary>
        /// Verifies that WallCommand calls the AggregatorService, passes the arguments to it and gets the result from 
        /// </summary>
        [TestMethod]
        public void Should_CallAggregatorService_When_ExecutingWallCommand()
        {
            // Act
            sut.Execute(new string[] {"Alice"});

            //Assert
            var serviceCall = GetServiceCall().FirstOrDefault();
            Assert.IsNotNull(serviceCall, "The AggregatorService was not called");
            var args = serviceCall.GetArguments();
            Assert.AreEqual(1, args.Length, "Unexpected number of arguments sent to AggregatorService");
            Assert.AreEqual("Alice", args[0], "The user name was not passed to the service as an argument");
        }

        /// <summary>
        /// Verifies that WallCommand calls the StoryFormatter and passes the arguments to it
        /// </summary>
        [TestMethod]
        public void Should_CallStoryFormatter_When_ExecutingWallCommand()
        {
            // Act
            sut.Execute(new string[] { "Alice" });

            //Assert
            var formatterCalls = GetFormatterCalls().ToArray();
            Assert.AreEqual(fakeAggregatedStories.Length, formatterCalls.Length, "Number of calls to StoryFormatter different than expected");

            for (int i = 0; i < fakeAggregatedStories.Length; i++)
            {
                var arguments = formatterCalls[i].GetArguments();
                Assert.AreEqual(1, arguments.Length, "Unexpected number of arguments sent to StoryFormatter");
                Assert.AreEqual(fakeAggregatedStories[i], arguments[0], "The aggregated story object was not passed to the StoryFormatter as an argument");
            }
        }

        /// <summary>
        /// Verifies that WallCommand relies on AggragatedStoryFormatter to get the formatted aggregated story 
        /// </summary>
        [TestMethod]
        public void Should_GetOutputOfStoryFormatter_When_ExecutingWallCommand()
        {
            // Act
            var actual = sut.Execute(new string[] { "Alice" });

            //Assert
            Assert.IsNotNull(actual, "Wall Command returned null result");

            CollectionAssert.AreEquivalent(
                fakeAggregatedStories.Select(s => s.Story.Message).ToArray(), SplitLines(actual),
                "The text returned by the Wall Command is different than the one returned by the story formatter");
        }

        /// <summary>
        /// Gets the calls made to the GetAggregatedStories method of the fake AggregatorService
        /// </summary>
        /// <returns></returns>
        private IEnumerable<StubObservedCall> GetServiceCall()
        {
            return aggregatorServiceObserver.GetCalls().Where(c => c.StubbedMethod.Name == "GetAggregatedStories");
        }

        /// <summary>
        /// Gets the calls made to the Format method of the fake AggregatedStoryFormatter
        /// </summary>
        /// <returns></returns>
        private IEnumerable<StubObservedCall> GetFormatterCalls()
        {
            return aggregatedStoryFormatterObserver.GetCalls().Where(c => c.StubbedMethod.Name == "Format");
        }

        private string[] SplitLines(string multilineText)
        {
            return multilineText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }
    }
}
