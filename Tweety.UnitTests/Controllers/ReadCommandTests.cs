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
    /// Tests the ReadCommand implementation in isolation
    /// </summary>
    [TestClass]
    public class ReadCommandTests
    {
        //Observers
        private StubObserver timelineReaderServiceObserver;
        private StubObserver storyFormatterObserver;

        //Fake dependencies
        private ITimelineReaderService timelineReaderService;
        private IEntityFormatter<Story> storyFormatter;

        //Fake entities
        private Story[] fakeStories;


        //SUT
        private ReadCommand sut;

        [TestInitialize]
        public void Initialize()
        {
            fakeStories = new Story[]
            {
                new Story { Message = "First story", PostedOn = new DateTime(2000, 1, 1, 15, 1, 0) },
                new Story { Message = "Second story", PostedOn = new DateTime(2000, 1, 1, 15, 5, 0) }
            };

            //Creates a fake ITimelineReaderService with a liked observer in order to
            //assert that the SUT relies on it
            timelineReaderServiceObserver = new StubObserver();
            timelineReaderService = new Contracts.Interfaces.Fakes.StubITimelineReaderService
            {
                InstanceObserver = timelineReaderServiceObserver,
                GetTimelineString = userName => fakeStories
            };

            storyFormatterObserver = new StubObserver();
            storyFormatter = new Contracts.Interfaces.Fakes.StubIEntityFormatter<Story>
            {
                InstanceObserver = storyFormatterObserver,
                FormatT0 = story => story.Message
            };

            sut = new ReadCommand(timelineReaderService, storyFormatter);
        }

        /// <summary>
        /// Verifies that ReadCommand calls the TimelineReaderService, passes the arguments to it and gets the result from 
        /// </summary>
        [TestMethod]
        public void Should_CallTimelineReaderService_When_ExecutingReadCommand()
        {
            // Act
            sut.Execute(new string[] {"Alice"});

            //Assert
            var serviceCall = GetServiceCall().FirstOrDefault();
            Assert.IsNotNull(serviceCall, "The TimelineReaderService was not called");
            var args = serviceCall.GetArguments();
            Assert.AreEqual(1, args.Length, "Unexpected number of arguments sent to TimelineReaderService");
            Assert.AreEqual("Alice", args[0], "The user name was not passed to the service as an argument");
        }

        /// <summary>
        /// Verifies that ReadCommand calls the StoryFormatter, passes the arguments to it and gets the result from 
        /// </summary>
        [TestMethod]
        public void Should_CallStoryFormatter_When_ExecutingReadCommand()
        {
            // Act
            sut.Execute(new string[] { "Alice" });

            //Assert
            var formatterCalls = GetFormatterCalls().ToArray();
            Assert.AreEqual(fakeStories.Length, formatterCalls.Length, "Number of calls to StoryFormatter different than expected");

            for (int i = 0; i < fakeStories.Length; i++)
            {
                var arguments = formatterCalls[i].GetArguments();
                Assert.AreEqual(1, arguments.Length, "Unexpected number of arguments sent to StoryFormatter");
                Assert.AreEqual(fakeStories[i], arguments[0], "The story object was not passed to the StoryFormatter as an argument");
            }
        }

        /// <summary>
        /// Verifies that ReadCommand relies on StoryFormatter to get the formatted story 
        /// </summary>
        [TestMethod]
        public void Should_GetOutputOfStoryFormatter_When_ExecutingReadCommand()
        {
            // Act
            var actual = sut.Execute(new string[] { "Alice" });

            //Assert
            Assert.IsNotNull(actual, "Read Command returned null result");

            CollectionAssert.AreEquivalent(
                fakeStories.Select(s => s.Message).ToArray(), SplitLines(actual),
                "The text returned by the Read Command is different than the one returned by the story formatter");
        }

        /// <summary>
        /// Gets the calls made to the GetTimeline method of the fake TimelineReaderService
        /// </summary>
        /// <returns></returns>
        private IEnumerable<StubObservedCall> GetServiceCall()
        {
            return timelineReaderServiceObserver.GetCalls().Where(c => c.StubbedMethod.Name == "GetTimeline");
        }

        /// <summary>
        /// Gets the calls made to the Format method of the fake StoryFormatter
        /// </summary>
        /// <returns></returns>
        private IEnumerable<StubObservedCall> GetFormatterCalls()
        {
            return storyFormatterObserver.GetCalls().Where(c => c.StubbedMethod.Name == "Format");
        }

        private string[] SplitLines(string multilineText)
        {
            return multilineText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }
    }
}
