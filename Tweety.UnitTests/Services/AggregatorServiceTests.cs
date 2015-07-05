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
    /// Tests the AggregatorService implementation in isolation
    /// </summary>
    [TestClass]
    public class AggregatorServiceTests
    {
        //Fake commands
        private IRepository<User> userRepository;

        //Fake entity
        private User fakeUserAlice;
        private User fakeUserBob;
        private User fakeUserCharlie;

        //SUT
        private AggregatorService sut;

        [TestInitialize]
        public void Initialize()
        {
            InitializeUsers();
            InitializeRepository();

            sut = new AggregatorService(userRepository);
        }

        /// <summary>
        /// Initialize fake users
        /// </summary>
        private void InitializeUsers()
        {
            var time = DateTime.Now;
            fakeUserAlice = new User
            {
                Name = "Alice",
                Timeline = new List<Story>
                {
                    new Story { Message = "I love the weather today", PostedOn = time.AddMinutes(-1) }
                }
            };

            fakeUserBob = new User
            {
                Name = "Bob",
                Timeline = new List<Story>
                {
                    new Story { Message = "Damn! We lost!", PostedOn = time.AddMinutes(-3) },
                    new Story { Message = "Good game though.", PostedOn = time }
                }
            };

            fakeUserCharlie = new User
            {
                Name = "Charlie",
                Timeline = new List<Story>
                {
                    new Story { Message = "I’m in New York today! Anyone wants to have a coffee?", PostedOn = time.AddMinutes(-4) },
                }
            };
        }

        /// <summary>
        /// Initializes a fake repository that returns the fake users
        /// </summary>
        private void InitializeRepository()
        {
            userRepository = new Contracts.Interfaces.Fakes.StubIRepository<User>
            {
                GetOrCreateString = (userName) =>
                {
                    switch (userName)
                    {
                        case "Alice": return fakeUserAlice;
                        case "Bob": return fakeUserBob;
                        case "Charlie": return fakeUserCharlie;
                        default: throw new NotSupportedException();
                    }
                },
            };
        }

        /// <summary>
        /// Verifies that AggregatorService returns aggregated stories from the user's timeline
        /// when the user does not follow any other user
        /// </summary>
        [TestMethod]
        public void Should_ReturnCharlieTimeline_When_HeDoesNotFollowOtherUsers()
        {
            // Arrange
            fakeUserCharlie.Following = new User[0]; 

            // Act
            var actual = sut.GetAggregatedStories("Charlie");

            //Assert
            Assert.IsNotNull(actual, "GetAggregatedStories returned null");
            Assert.AreEqual(1, actual.Count(), "Unexpected story count in wall");
            Assert.AreSame(fakeUserCharlie.Timeline[0], actual.First().Story, "Unexpected aggregated story returned as first item of the wall");
            Assert.AreSame(fakeUserCharlie, actual.First().BelongingTo, "The BelongingTo property of the returned aggregated story references a wrong user");
        }

        /// <summary>
        /// Verifies that AggregatorService returns aggregated stories from the user's timeline
        /// AND from the users he or she follows. Also checks whether tehy are correctly sorted. 
        /// </summary>
        [TestMethod]
        public void Should_ReturnMergedCharlieAliceAndBobStoriesSortedByTime_When_CharlieFollowsAliceAndBob()
        {
            // Arrange
            fakeUserCharlie.Following = new User[] { fakeUserAlice, fakeUserBob };

            // Act
            var actual = sut.GetAggregatedStories("Charlie");

            //Assert
            Assert.IsNotNull(actual, "GetAggregatedStories returned null");
            Assert.AreEqual(4, actual.Count(), "Unexpected story count in wall");

            var wallStories = actual.ToArray();

            VerifyWallStory(wallStories, 0, fakeUserBob.Timeline[1], fakeUserBob);
            VerifyWallStory(wallStories, 1, fakeUserAlice.Timeline[0], fakeUserAlice);
            VerifyWallStory(wallStories, 2, fakeUserBob.Timeline[0], fakeUserBob);
            VerifyWallStory(wallStories, 3, fakeUserCharlie.Timeline[0], fakeUserCharlie);
        }

        /// <summary>
        /// Common assertion logic to verify a specific wall story
        /// </summary>
        /// <param name="wallStories">Array of stories returned by the SUT</param>
        /// <param name="wallStoryIndex">Index of the story to verify</param>
        /// <param name="expectedStory">Story that the aggregated story is expected to contain</param>
        /// <param name="expectedBelogingToUser">User that the aggregated story is expected to belong to</param>
        private void VerifyWallStory(AggregatedStory[] wallStories, int wallStoryIndex, Story expectedStory, User expectedBelogingToUser)
        {
            Assert.AreSame(expectedStory, wallStories[wallStoryIndex].Story, "Unexpected aggregated story item at position {0} in the wall", wallStoryIndex);
            Assert.AreSame(expectedBelogingToUser, wallStories[wallStoryIndex].BelongingTo, "The BelongingTo property of the returned aggregated story references a wrong user");
        }

    }
}
