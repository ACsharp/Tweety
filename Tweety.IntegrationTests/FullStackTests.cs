using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Tweety.Contracts.Interfaces;
using System.Text.RegularExpressions;


namespace Tweety.SystemTests
{
    /// <summary>
    /// System tests that use the full application stack (excluding the ConsoleUI) without any isolation or mocking
    /// </summary>
    [TestClass]
    public class FullStackTests
    {
        /// <summary>
        /// Resolution root of the SUT
        /// </summary>
        private ICommandHandler commandHandler;

        //Test case constants
        private const string alicePostMessage = "I love the weather today!";
        private const string bobFirstPostMessage = "Damn! We lost!";
        private const string bobSecondPostMessage = "Good game though.";
        private const string charliePostMessage = "I’m in New York today! Anyone wants to have a coffee?";

        /// <summary>
        /// Initializes the IoC Container (Convention over Configuration)
        /// and resolve the resolution root (commandHandler)
        /// </summary>
        [TestInitialize]
        public void InitializeDependencies()
        {
            var IoCcontainer = new UnityContainer();

            IoCcontainer.RegisterTypes(
                  AllClasses.FromAssembliesInBasePath(),
                  WithMappings.FromMatchingInterface,
                  WithName.Default,
                  WithLifetime.ContainerControlled);

            IoCcontainer.LoadConfiguration();

            //Gets the resolution root
            commandHandler = IoCcontainer.Resolve<ICommandHandler>();
        }

        /// <summary>
        /// Alice and Bob post to their timelines
        /// </summary>
        /// <remarks>
        /// <para>Scenario:
        /// Alice posts a message to her timeline.
        /// Bob posts two messages to his timeline.
        /// </para>
        /// </remarks>
        [TestMethod]
        public void SystemTest_AliceAndBobPostSenario()
        {
            // Arrange
            var cases = new string[]
            {
                "Alice -> " + alicePostMessage,
                "Bob -> " + bobFirstPostMessage,
                "Bob -> " + bobSecondPostMessage
            };

            foreach (var inputCase in cases)
            {
                // Act
                var actual = commandHandler.HandleUserCommand(inputCase);

                // Assert
                Assert.IsNull(actual, "Post command should return null");
                
                // 1 millisecond break between post commands to allow checking sorting by post time later
                System.Threading.Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Charlie posts to his timeline
        /// </summary>
        /// <remarks>
        /// Scenario: Charlie posts a message to his timeline.
        /// </remarks>
        [TestMethod]
        public void SystemTest_CharliePostSenario()
        {
            // Act
            var actual = commandHandler.HandleUserCommand("Charlie -> " + charliePostMessage);

            // Assert
            Assert.IsNull(actual, "Post command should return null");
        }

        /// <summary>
        /// Alice reads her timeline
        /// </summary>
        /// <remarks>
        /// <para>Preconditions: AliceAndBobPostSenario</para>
        /// <para>Scenario:
        /// Alice reads the single story previously posted to her timeline.
        /// </para>
        /// </remarks>
        [TestMethod]
        public void SystemTest_AliceReadSenario()
        {
            // Arrange
            RunTestPreconditions(() => SystemTest_AliceAndBobPostSenario());

            // Act
            var actual = commandHandler.HandleUserCommand("Alice");

            // Assert
            Assert.IsNotNull(actual, "Null result returned");
            //Assert that the command result matches the expected pattern 
            StringAssert.Matches(actual, GetStoryMatchingRegex(alicePostMessage), "Unexpected result returned");
        }

        /// <summary>
        /// Bob reads her timeline
        /// </summary>
        /// <remarks>
        /// <para>Preconditions: AliceAndBobPostSenario</para>
        /// <para>Scenario:
        /// Bob reads the two stories previously posted to his timeline.
        /// </para>
        /// </remarks>
        [TestMethod]
        public void SystemTest_BobReadSenario()
        {
            // Arrange
            RunTestPreconditions(() => SystemTest_AliceAndBobPostSenario());

            // Act
            var actual = commandHandler.HandleUserCommand("Bob");

            // Assert
            Assert.IsNotNull(actual, "Null result returned");
            var returnedLines = SplitLines(actual);
            Assert.AreEqual(2, returnedLines.Length, "Number of lines returned different than expected");

            //Assert that the command result matches the expected pattern 
            StringAssert.Matches(returnedLines[0], GetStoryMatchingRegex(bobSecondPostMessage), "Unexpected result returned as a first line");
            StringAssert.Matches(returnedLines[1], GetStoryMatchingRegex(bobFirstPostMessage), "Unexpected result returned as a first line");
        }

        /// <summary>
        /// Charlie reads his timeline
        /// </summary>
        /// <remarks>
        /// <para>Preconditions: AliceAndBobPostSenario</para>
        /// <para>Scenario:
        /// Charlie reads his empty timeline.
        /// </para>
        /// </remarks>
        [TestMethod]
        public void SystemTest_CharlieReadSenario()
        {
            // Arrange
            RunTestPreconditions(() => SystemTest_AliceAndBobPostSenario());

            // Act
            var actual = commandHandler.HandleUserCommand("Charlie");

            // Assert
            Assert.IsNotNull(actual, "Null result returned");
            Assert.AreEqual(string.Empty, actual, "An empty result was expected");
        }

        /// <summary>
        /// Charlie follows Alice
        /// </summary>
        /// <remarks>
        /// Scenario: Charlie becomes a follower of Alice
        /// </remarks>
        [TestMethod]
        public void SystemTest_CharlieFollowsAliceSenario()
        {
            // Act
            var actual = commandHandler.HandleUserCommand("Charlie follows Alice");

            // Assert
            Assert.IsNull(actual, "Null result expected");
        }

        /// <summary>
        /// Charlie follows Bob
        /// </summary>
        /// <remarks>
        /// Scenario: Charlie becomes a follower of Bob
        /// </remarks>
        [TestMethod]
        public void SystemTest_CharlieFollowsBobSenario()
        {
            // Act
            var actual = commandHandler.HandleUserCommand("Charlie follows Bob");

            // Assert
            Assert.IsNull(actual, "Null result expected");
        }

        /// <summary>
        /// Charlie reads his wall after he became a follower of Alice
        /// </summary>
        /// <remarks>
        /// <para>Preconditions:
        /// AliceAndBobPostSenario,
        /// CharliePostSenario,
        /// CharlieFollowsAliceSenario
        /// </para>
        /// <para>Scenario:
        /// Charlie reads his wall which contains one story by himself and one story by Alice
        /// </para>
        /// </remarks>
        [TestMethod]
        public void SystemTest_CharlieWall_When_FollowingAliceSenario()
        {
            // Arrange
            RunTestPreconditions(() => {
                SystemTest_AliceAndBobPostSenario();

                // 1 millisecond break between post commands to allow checking sorting by post time
                System.Threading.Thread.Sleep(1);

                SystemTest_CharliePostSenario();
                SystemTest_CharlieFollowsAliceSenario();
            });

            // Act
            var actual = commandHandler.HandleUserCommand("Charlie wall");

            // Assert
            Assert.IsNotNull(actual, "Wall command returned null result");
            var returnedLines = SplitLines(actual);
            Assert.AreEqual(2, returnedLines.Length, "Number of lines returned different than expected");

            //Assert that the command result matches the expected pattern 
            StringAssert.Matches(returnedLines[0], GetAggregatedStoryMatchingRegex("Charlie", charliePostMessage), "Unexpected result returned as a first line");
            StringAssert.Matches(returnedLines[1], GetAggregatedStoryMatchingRegex("Alice", alicePostMessage), "Unexpected result returned as a first line");

        }

        /// <summary>
        /// Charlie reads his wall after he became a follower of Alice AND Bob
        /// </summary>
        /// <remarks>
        /// <para>Preconditions:
        /// AliceAndBobPostSenario,
        /// CharliePostSenario,
        /// CharlieFollowsAliceSenario,
        /// SystemTest_CharlieFollowsBobSenario
        /// </para>
        /// <para>Scenario:
        /// Charlie reads his wall which contains one story by himself, one story by Alice and two stories from Bob
        /// </para>
        /// </remarks>
        [TestMethod]
        public void SystemTest_CharlieWall_When_FollowingAliceAndBobSenario()
        {
            // Arrange
            RunTestPreconditions(() =>
            {
                SystemTest_AliceAndBobPostSenario();
                System.Threading.Thread.Sleep(1); // 1 millisecond break between post commands to allow checking sorting by post time
                SystemTest_CharliePostSenario();
                SystemTest_CharlieFollowsAliceSenario();
                SystemTest_CharlieFollowsBobSenario();
            });

            // Act
            var actual = commandHandler.HandleUserCommand("Charlie wall");

            // Assert
            Assert.IsNotNull(actual, "Wall command returned null result");
            var returnedLines = SplitLines(actual);
            Assert.AreEqual(4, returnedLines.Length, "Number of lines returned different than expected");

            //Assert that the command result matches the expected pattern 
            StringAssert.Matches(returnedLines[0], GetAggregatedStoryMatchingRegex("Charlie", charliePostMessage), "Unexpected result returned as a first line");
            StringAssert.Matches(returnedLines[1], GetAggregatedStoryMatchingRegex("Bob", bobSecondPostMessage), "Unexpected result returned as a first line");
            StringAssert.Matches(returnedLines[2], GetAggregatedStoryMatchingRegex("Bob", bobFirstPostMessage), "Unexpected result returned as a first line");
            StringAssert.Matches(returnedLines[3], GetAggregatedStoryMatchingRegex("Alice", alicePostMessage), "Unexpected result returned as a first line");

        }

        /// <summary>
        /// Runs the provided action and set the test as inconclusive in case of exception during its execution
        /// </summary>
        /// <param name="preconditionPredicate"></param>
        private void RunTestPreconditions(Action preconditionPredicate)
        {
            try
            {
                preconditionPredicate();
            }
            catch (Exception ex)
            {
                Assert.Inconclusive(ex.ToString());
            }
        }

        /// <summary>
        /// Gets a regular expression to be used during the assertion stage to
        /// parse the result text and check whether it matches the expected Story
        /// </summary>
        /// <param name="storyMessage">Content of the expected message</param>
        /// <returns></returns>
        private Regex GetStoryMatchingRegex(string storyMessage)
        {
            var pattern = string.Concat("^", Regex.Escape(storyMessage), @" \([0-9]{1,2} seconds{0,1} ago\)$");
            return new Regex(pattern);
        }

        /// <summary>
        /// Gets a regular expression to be used during the assertion stage to
        /// parse the result text and check whether it matches the expected Aggregated Story
        /// </summary>
        /// <param name="storyMessage">Content of the expected message</param>
        /// <param name="userName">User name the aggregated story is expected to be posted by</param>
        /// <returns></returns>
        private Regex GetAggregatedStoryMatchingRegex(string userName, string storyMessage)
        {
            var pattern = string.Concat("^", Regex.Escape(userName), @" \- ", Regex.Escape(storyMessage), @" \([0-9]{1,2} seconds{0,1} ago\)$");
            return new Regex(pattern);
        }

        private string[] SplitLines(string multilineText)
        {
            return multilineText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }
    }
}
