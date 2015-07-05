using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using System.Linq;
using System.Collections.Generic;
using Tweety.Contracts.Interfaces;
using Tweety.Controllers;
using Tweety.Contracts.Entities;
using Tweety.Logic.Repository;

namespace Tweety.UnitTests.Repository
{
    /// <summary>
    /// Tests the UserRepository implementation in isolation
    /// </summary>
    [TestClass]
    public class UserRepositoryTests
    {
        //SUT
        private UserRepository sut;

        [TestInitialize]
        public void Initialize()
        {
            sut = new UserRepository();
        }

        [TestMethod]
        public void Should_GetNewUser_When_AskingForNonExistentUserName()
        {
            // Act
            var actual = sut.GetOrCreate("Alice");

            //Assert
            Assert.IsNotNull(actual, "The new user was not returned by the repository");
            Assert.AreEqual("Alice", actual.Name, "The newly created user has an unexpected name");
        }

        [TestMethod]
        public void Should_InitializeCollections_When_CreatingNewUser()
        {
            // Act
            var actual = sut.GetOrCreate("Alice");

            //Assert
            if (actual == null) Assert.Inconclusive();
            Assert.IsNotNull(actual.Timeline, "Timeline property was not initialized");
            Assert.IsNotNull(actual.Following, "Following property was not initialized");
        }

        [TestMethod]
        public void Should_RetrieveUser_When_UserExistsInRepository()
        {
            // Arrange
            var arrangedUser = sut.GetOrCreate("Alice");

            // Act
            var actual = sut.GetOrCreate("Alice");

            //Assert
            if (actual == null) Assert.Inconclusive();
            Assert.AreSame(arrangedUser, actual, "The repository returned a different instance of a user than the previously created one");
        }

        /// <summary>
        /// Verifies that the same object is returned when querying by "Alice" and "alice"
        /// </summary>
        [TestMethod]
        public void Should_IgnoreUserNameCase_When_RetrievingUser()
        {
            // Arrange
            var arrangedUser = sut.GetOrCreate("Alice");

            // Act
            var actual = sut.GetOrCreate("alice");

            //Assert
            if (actual == null) Assert.Inconclusive();
            Assert.AreSame(arrangedUser, actual, "The repository returned a different instance of a user than the previously created one");
        }
    }
}
