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
    /// Tests the PostCommand implementation in isolation
    /// </summary>
    [TestClass]
    public class PostCommandTests
    {
        //Observers
        private StubObserver timelineWriterServiceObserver;

        //Fake dependencies
        private ITimelineWriterService timelineWriterService;


        //SUT
        private PostCommand sut;

        [TestInitialize]
        public void Initialize()
        {
            //Creates a fake ITimelineWriterService with a liked observer in order to
            //assert that the SUT relies on it
            timelineWriterServiceObserver = new StubObserver();
            timelineWriterService = new Contracts.Interfaces.Fakes.StubITimelineWriterService
            {
                InstanceObserver = timelineWriterServiceObserver
            };

            sut = new PostCommand(timelineWriterService);
        }

        /// <summary>
        /// Verifies that PostCommand calls the TimelineWriterService, passes the arguments to it and gets the result from 
        /// </summary>
        [TestMethod]
        public void Should_CallTimelineWriterService_When_ExecutingPostCommand()
        {
            // Act
            sut.Execute(new string[] {"Alice", "I love the weather today!"});

            //Assert
            var serviceCall = GetServiceCall().FirstOrDefault();

            Assert.IsNotNull(serviceCall, "The TimelineWriterService was not called");
            
            var args = serviceCall.GetArguments();

            Assert.AreEqual(2, args.Length, "Unexpected number of arguments sent to TimelineWriterService");

            Assert.AreEqual("Alice", args[0], "The user name was not passed to the service as an argument");
            Assert.AreEqual("I love the weather today!", args[1], "The message was not passed to the service as an argument");
        }

        /// <summary>
        /// Gets the calls made to the Post method of the fake TimelineWriterService
        /// </summary>
        /// <returns></returns>
        private IEnumerable<StubObservedCall> GetServiceCall()
        {
            return timelineWriterServiceObserver.GetCalls().Where(c => c.StubbedMethod.Name == "Post");
        }
    }
}
