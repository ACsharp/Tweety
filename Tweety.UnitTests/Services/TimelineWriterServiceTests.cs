using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using System.Linq;
using System.Collections.Generic;
using Tweety.Contracts.Interfaces;
using Tweety.Controllers;
using Tweety.Contracts.Entities;
using Tweety.Logic.Service;
using Microsoft.QualityTools.Testing.Fakes;

namespace Tweety.UnitTests.Services
{
    /// <summary>
    /// Tests the TimelineWriterService implementation in isolation
    /// </summary
    [TestClass]
    public class TimelineWriterServiceTests
    {
        //Observers
        private StubObserver userRepositoryObserver;

        //Fake repository
        private IRepository<User> userRepository;

        //Fake entity
        private User fakeUser;

        //SUT
        private TimelineWriterService sut;

        [TestInitialize]
        public void Initialize()
        {
            // Initialize fake user
            fakeUser = new User
            {
                Name = "Alice",
                Timeline = new List<Story>()
            };

            // Initializes a fake repository that returns the fake user
            // and attaches an observer to it in order to check if the repository
            // is correctly called by the TimelineWriterService
            userRepositoryObserver = new StubObserver();
            userRepository = new Contracts.Interfaces.Fakes.StubIRepository<User>
            {
                GetOrCreateString = (userName) => fakeUser,
                InstanceObserver = userRepositoryObserver
            };

            sut = new TimelineWriterService(userRepository);
        }

        /// <summary>
        /// Verifies that the repository is correctly written by the TimelineWriterService when a user posts a story
        /// </summary>
        [TestMethod]
        public void Should_PostToTimeline()
        {
            // Act
            sut.Post("Alice", "I love the weather today!");

            //Assert
            var repositoryCall = GetRepositoryCall().FirstOrDefault();

            Assert.AreEqual(1, fakeUser.Timeline.Count, "Unexpected story count in timeline");

            Assert.AreEqual("I love the weather today!", fakeUser.Timeline.First().Message, "Unexpected message posted to timeline");

            var arg = repositoryCall.GetArguments().FirstOrDefault();
            Assert.AreEqual("Alice", arg, "The user name was not passed to the repository as an argument");
        }

        /// <summary>
        /// Verifies that TimelineWriterService set the story date and time when a user posts to a timeline 
        /// </summary>
        [TestMethod]
        public void Should_SetPostedOn_When_PostingToTimeline()
        {
            // Arrange
            var fakeTime = new DateTime(2000, 1, 1, 13, 22, 45); //Fake fixed time
            using (var shimsContext = ShimsContext.Create())
            {
                // Setting the framework DateTime.Now to a fake fixed time
                System.Fakes.ShimDateTime.NowGet = () => fakeTime;

                // Act
                sut.Post("Alice", "I love the weather today!");
            }

            //Assert
            var story = fakeUser.Timeline.FirstOrDefault();
            if (story == null) Assert.Inconclusive();

            Assert.AreEqual(fakeTime, story.PostedOn, "PostedOn different than expected");
        }

        /// <summary>
        /// Verifies that the TimelineWriterService gets the user from the repository when a user posts to a timeline
        /// </summary>
        [TestMethod]
        public void Should_RequestAliceUserFromRepository_When_PostingToAliceTimeline()
        {
            // Act
            sut.Post("Alice", "I love the weather today!");

            //Assert
            var repositoryCall = GetRepositoryCall().FirstOrDefault();

            Assert.IsNotNull(repositoryCall, "The user repository was not called");

            var arg = repositoryCall.GetArguments().FirstOrDefault();
            Assert.AreEqual("Alice", arg, "The user name was not passed to the repository as an argument");
        }

        /// <summary>
        /// Gets the calls made to the GetOrCreateUser method of the fake UserRepository
        /// </summary>
        /// <returns></returns>
        private IEnumerable<StubObservedCall> GetRepositoryCall()
        {
            return userRepositoryObserver.GetCalls().Where(c => c.StubbedMethod.Name == "GetOrCreate");
        }
    }
}
