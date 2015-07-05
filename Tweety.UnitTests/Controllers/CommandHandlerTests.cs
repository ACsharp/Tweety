using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using Tweety.Contracts.Interfaces;
using Tweety.Controllers;
using System.Linq;
using System.Collections.Generic;

namespace Tweety.UnitTests.Controllers
{
    /// <summary>
    /// Tests the CommandHandler implementation in isolation
    /// </summary>
    [TestClass]
    public class CommandHandlerTests
    {
        //Observers
        private StubObserver readCommandObserver;
        private StubObserver postCommandObserver;
        private StubObserver wallCommandObserver;

        //Fake commands
        private ICommand readCommand;
        private ICommand postCommand;
        private ICommand wallCommand;


        [TestInitialize]
        public void Initialize()
        {
            //Initializing observers
            readCommandObserver = new StubObserver();
            postCommandObserver = new StubObserver();
            wallCommandObserver = new StubObserver();

            //Initializing fake commands with arbitrary signatures.
            //Each one is linked to its observer in order to check 
            //whether the requests are properly routed
            readCommand = new Contracts.Interfaces.Fakes.StubICommand
            {
                // command signature: {UserName}
                SignatureGet = () => @"^([^\s][A-Za-z0-9-_]+)$",
                InstanceObserver = readCommandObserver
            };
            postCommand = new Contracts.Interfaces.Fakes.StubICommand
            {
                // command signature: {UserName} -> {Message}
                SignatureGet = () => @"^([^\s][A-Za-z0-9-_]+) -> (.+)$",
                InstanceObserver = postCommandObserver
            };
            wallCommand = new Contracts.Interfaces.Fakes.StubICommand
            {
                // command signature: {UserName} Wall
                SignatureGet = () => @"^([^\s][A-Za-z0-9-_]+) Wall$",
                InstanceObserver = wallCommandObserver
            };
        }

        /// <summary>
        /// Verifies that the request "Alice -> I love the weather today!" is routed to the PostCommand
        /// </summary>
        [TestMethod]
        public void Should_RoutePostRequestToAppropriateCommand()
        {
            // Arrange
            var SUT = new CommandHandler(readCommand, postCommand, wallCommand);

            // Act
            SUT.HandleUserCommand("Alice -> I love the weather today!");

            //Assert
            Assert.IsFalse(GetExecuteCall(readCommandObserver).Any(), "The request was incorrectly routed to ReadCommand");
            Assert.IsTrue(GetExecuteCall(postCommandObserver).Any(), "The request was not routed to PostCommand");
            Assert.IsFalse(GetExecuteCall(wallCommandObserver).Any(), "The request was incorrectly routed to WallCommand");
        }

        /// <summary>
        /// Verifies that the request "Alice WALL" is routed to the WallCommand regardless its case (WALL or Wall or wall must all work)
        /// </summary>
        [TestMethod]
        public void Should_IgnoreCaseInWallCommandSignature()
        {
            // Arrange
            var SUT = new CommandHandler(readCommand, postCommand, wallCommand);

            // Act
            SUT.HandleUserCommand("Alice WALL");

            //Assert
            Assert.IsFalse(GetExecuteCall(readCommandObserver).Any(), "The request was incorrectly routed to ReadCommand");
            Assert.IsFalse(GetExecuteCall(postCommandObserver).Any(), "The request was incorrectly routed to PostCommand");
            Assert.IsTrue(GetExecuteCall(wallCommandObserver).Any(), "The request was not routed to WallCommand");
        }

        /// <summary>
        /// Verifies that the request "Alice -> I love the weather today!" is routed to the PostCommand
        /// </summary>
        [TestMethod]
        public void Should_RoutePostArgumentsToAppropriateCommand()
        {
            // Arrange
            var SUT = new CommandHandler(readCommand, postCommand, wallCommand);

            // Act
            SUT.HandleUserCommand("Alice -> I love the weather today!");

            //Assert
            var routedCall = GetExecuteCall(postCommandObserver).FirstOrDefault();
            if (routedCall == null) Assert.Inconclusive("The request was not routed to the command so this test cannot assert the appropriate routing of arguments");

            var routedArguments = routedCall.GetArguments().FirstOrDefault();
            Assert.IsInstanceOfType(routedArguments, typeof(string[]), "The routed arguments are of wrong type");
            
            var convertedArguments = (string[])routedArguments;
            Assert.AreEqual(2, convertedArguments.Length, "Wrong number of arguments");
            Assert.AreEqual("Alice", convertedArguments[0], "The User Name argument was not properly routed to the command");
            Assert.AreEqual("I love the weather today!", convertedArguments[1], "The message argument was not properly routed to the command");
        }

        /// <summary>
        /// Verifies that the request "Alice_80 Wall" is routed and its argument Alice_80 (which contains an underscore) is correctly routed
        /// </summary>
        [TestMethod]
        public void Should_AllowUnderscoreInUserName_When_RoutingCommands()
        {
            // Arrange
            var SUT = new CommandHandler(readCommand, postCommand, wallCommand);

            // Act
            SUT.HandleUserCommand("Alice_80 Wall");

            //Assert
            var routedCall = GetExecuteCall(wallCommandObserver).FirstOrDefault();
            if (routedCall == null) Assert.Inconclusive("The request was not routed to the command so this test cannot assert the appropriate routing of arguments");

            var routedArguments = routedCall.GetArguments().FirstOrDefault();
            Assert.IsInstanceOfType(routedArguments, typeof(string[]), "The routed arguments are of wrong type");

            var convertedArguments = (string[])routedArguments;
            Assert.AreEqual("Alice_80", convertedArguments.FirstOrDefault(), "The User Name argument was not properly routed to the command");
        }

        /// <summary>
        /// Verifies that the request "Alice-80 Wall" is routed and its argument Alice-80 (which contains a dash) is correctly routed
        /// </summary>
        [TestMethod]
        public void Should_AllowDashInUserName_When_RoutingCommands()
        {
            // Arrange
            var SUT = new CommandHandler(readCommand, postCommand, wallCommand);

            // Act
            SUT.HandleUserCommand("Alice-80 Wall");

            //Assert
            var routedCall = GetExecuteCall(wallCommandObserver).FirstOrDefault();
            if (routedCall == null) Assert.Inconclusive("The request was not routed to the command so this test cannot assert the appropriate routing of arguments");

            var routedArguments = routedCall.GetArguments().FirstOrDefault();
            Assert.IsInstanceOfType(routedArguments, typeof(string[]), "The routed arguments are of wrong type");

            var convertedArguments = (string[])routedArguments;
            Assert.AreEqual("Alice-80", convertedArguments.FirstOrDefault(), "The User Name argument was not properly routed to the command");
        }

        /// <summary>
        /// Verifies that the request "Alice" is routed to the ReadCommand
        /// </summary>
        [TestMethod]
        public void Should_RouteEmptySignatureCommandTextToReadCommand()
        {
            // Arrange
            var SUT = new CommandHandler(readCommand, postCommand, wallCommand);

            // Act
            SUT.HandleUserCommand("Alice");

            //Assert
            Assert.IsTrue(GetExecuteCall(readCommandObserver).Any(), "The request was not routed to ReadCommand");
            Assert.IsFalse(GetExecuteCall(postCommandObserver).Any(), "The request was incorrectly routed to PostCommand");
            Assert.IsFalse(GetExecuteCall(wallCommandObserver).Any(), "The request was incorrectly routed to WallCommand");
        }

        /// <summary>
        /// Verifies that the request "Alice NotACommand" returns "Unknown command"
        /// </summary>
        [TestMethod]
        public void Should_ReturnErrorMessage_WhenCommandDoesNotExist()
        {
            // Arrange
            var SUT = new CommandHandler(readCommand, postCommand, wallCommand);

            // Act
            var actual = SUT.HandleUserCommand("Alice NotACommand");

            //Assert
            Assert.AreEqual("Unknown command", actual);
        }

        /// <summary>
        /// Gets the calls made to the Execute method of the provided observer
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        private IEnumerable<StubObservedCall> GetExecuteCall(StubObserver observer)
        {
            return observer.GetCalls().Where(c => c.StubbedMethod.Name == "Execute");
        }
    }
}
