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
    /// Tests the FollowsCommand implementation in isolation
    /// </summary>
    [TestClass]
    public class FollowsCommandTests
    {
        //Observers
        private StubObserver followerServiceObserver;

        //Fake dependencies
        private IFollowerService followerService;

        //SUT
        private FollowsCommand sut;

        [TestInitialize]
        public void Initialize()
        {
            //Creates a fake IFollowerService with a liked observer in order to
            //assert that the SUT relies on it
            followerServiceObserver = new StubObserver();
            followerService = new Contracts.Interfaces.Fakes.StubIFollowerService
            {
                InstanceObserver = followerServiceObserver
            };

            sut = new FollowsCommand(followerService);
        }

        /// <summary>
        /// Verifies that FollowsCommand calls the FollowerService, passes the arguments to it and gets the result from 
        /// </summary>
        [TestMethod]
        public void Should_CallFollowerService_When_ExecutingFollowsCommand()
        {
            // Act
            sut.Execute(new string[] {"Charlie", "Alice"});

            //Assert
            var serviceCall = GetServiceCall().FirstOrDefault();

            Assert.IsNotNull(serviceCall, "The FollowerService was not called");
            
            var args = serviceCall.GetArguments();

            Assert.AreEqual(2, args.Length, "Unexpected number of arguments sent to FollowerService");

            Assert.AreEqual("Charlie", args[0], "The follower user name was not passed to the service as an argument");
            Assert.AreEqual("Alice", args[1], "The followed user name was not passed to the service as an argument");
        }

        /// <summary>
        /// Gets the calls made to the SetFollowing method of the fake FollowerService
        /// </summary>
        /// <returns></returns>
        private IEnumerable<StubObservedCall> GetServiceCall()
        {
            return followerServiceObserver.GetCalls().Where(c => c.StubbedMethod.Name == "SetFollowing");
        }
    }
}
