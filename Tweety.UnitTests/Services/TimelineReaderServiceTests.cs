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
    /// Tests the TimelineReaderService implementation in isolation
    /// </summary
    [TestClass]
    public class TimelineReaderServiceTests
    {
        //Fake commands
        private IRepository<User> userRepository;

        //Fake entity
        private User fakeUserAlice;
        private User fakeUserBob;

        //SUT
        private TimelineReaderService sut;

        [TestInitialize]
        public void Initialize()
        {
            // Initialize fake users
            fakeUserAlice = new User
            {
                Name = "Alice",
                Timeline = new List<Story>
                {
                    new Story { Message = "I love the weather today", PostedOn = DateTime.Now }
                }
            };
            fakeUserBob = new User
            {
                Name = "Bob",
                Timeline = new List<Story>
                {
                    new Story { Message = "Damn! We lost!", PostedOn = DateTime.Now.AddMinutes(-5) },
                    new Story { Message = "Good game though.", PostedOn = DateTime.Now }
                }
            };

            // Initializes a fake repository that returns the fake users
            userRepository = new Contracts.Interfaces.Fakes.StubIRepository<User>
            {
                GetOrCreateString = (userName) => userName == "Alice" ? fakeUserAlice : fakeUserBob,
            };

            sut = new TimelineReaderService(userRepository);
        }

        [TestMethod]
        public void Should_ReadAliceTimeline_When_ThereIsOneStoryInRepository()
        {
            // Act
            var actual = sut.GetTimeline("Alice");

            //Assert
            Assert.IsNotNull(actual, "GetTimeline returned null");
            Assert.AreEqual(1, actual.Count(), "Unexpected story count in timeline");
            Assert.AreSame(fakeUserAlice.Timeline[0], actual.First(), "Unexpected story returned as first item of the timeline");
        }

        [TestMethod]
        public void Should_ReadBobTimeline_When_ThereAreTwoStoriesInRepository()
        {
            // Act
            var actual = sut.GetTimeline("Bob");

            //Assert
            Assert.IsNotNull(actual, "GetTimeline returned null");
            CollectionAssert.AreEquivalent(fakeUserBob.Timeline.ToList(), actual.ToList(), "Unexpected stories in the returned timeline");
        }

        [TestMethod]
        public void Should_SortStoriesByTimeDescending_When_ReadingBobTimeline()
        {
            // Act
            var actual = sut.GetTimeline("Bob");

            //Assert
            if (actual == null || actual.Count() != 2)
                Assert.Inconclusive();
            Assert.AreSame(fakeUserBob.Timeline[1], actual.First(), "Unexpected story returned as first item of the timeline");
            Assert.AreSame(fakeUserBob.Timeline[0], actual.Last(), "Unexpected story returned as second item of the timeline");
        }
    }
}
