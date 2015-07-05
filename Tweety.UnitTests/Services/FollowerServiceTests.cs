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
    /// Tests the FollowerService implementation in isolation
    /// </summary>
    [TestClass]
    public class FollowerServiceTests
    {
        //Fake commands
        private IRepository<User> userRepository;

        //Fake entities
        private User fakeUserCharlie;
        private User fakeUserAlice;

        //SUT
        private FollowerService sut;

        [TestInitialize]
        public void Initialize()
        {
            // Initialize fake users
            fakeUserCharlie = new User
            {
                Name = "Charlie",
                Timeline = new List<Story>(),
                Following = new List<User>()
            };
            fakeUserAlice = new User
            {
                Name = "Alice",
                Timeline = new List<Story>()
            };

            // Initializes a fake repository that returns the fake users
            userRepository = new Contracts.Interfaces.Fakes.StubIRepository<User>
            {
                GetOrCreateString = (userName) => userName == "Charlie" ? fakeUserCharlie : fakeUserAlice
            };

            sut = new FollowerService(userRepository);
        }

        [TestMethod]
        public void Should_AddFollowedUsers()
        {
            // Act
            sut.SetFollowing("Charlie", "Alice");

            //Assert
            Assert.AreEqual(1, fakeUserCharlie.Following.Count, "Unexpected followed user count in repository");
            Assert.AreSame(fakeUserAlice, fakeUserCharlie.Following.First(), "Unexpected followed user in repository");
        }

        /// <summary>
        /// Verifies that the followers are not duplicated in the repository if a user request to follow the same user twice
        /// </summary>
        [TestMethod]
        public void Should_NotDuplicateFollowingRecord_When_UserTriesToFollowAUserTheyAlreadyFollow()
        {
            // Arrange
            sut.SetFollowing("Charlie", "Alice");

            // Act
            sut.SetFollowing("Charlie", "Alice");

            //Assert
            Assert.AreEqual(1, fakeUserCharlie.Following.Count, "Unexpected followed user count in repository");
            Assert.AreSame(fakeUserAlice, fakeUserCharlie.Following.First(), "Unexpected followed user in repository");
        }
    }
}
