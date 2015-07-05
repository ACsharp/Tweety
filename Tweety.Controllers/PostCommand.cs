using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweety.Contracts.Interfaces;
using Tweety.Contracts.Entities;

namespace Tweety.Controllers
{
    /// <summary>
    /// Command that can handle text requests from a user to post a message to their timeline
    /// </summary>
    /// <example>Alice -> What a beautiful weather!</example>
    public class PostCommand : Contracts.Interfaces.ICommand
    {
        //command signature: {UserName} -> {Message}
        private const string signature = @"^([^\s][A-Za-z0-9-_]+) -> (.+)$";

        private ITimelineWriterService timelineWriterService;

        /// <summary>
        /// </summary>
        /// <param name="timelineWriterService">Service class to use to perform the command</param>
        public PostCommand(ITimelineWriterService timelineWriterService)
        {
            this.timelineWriterService = timelineWriterService;
        }

        /// <summary>
        /// Returns the signature of the text request that this command can handle
        /// </summary>
        /// <remarks>
        /// <para>Command signature: {UserName} -> {Message}</para>
        /// <para>Regular expression: ^([^\s][A-Za-z0-9-_]+) -> (.+)$</para>
        /// </remarks>
        public string Signature
        {
            get { return signature; }
        }

        /// <summary>
        /// Performs the command by calling the appropriate service class
        /// </summary>
        /// <param name="arguments">Array of arguments where
        /// the first element is the user name of the poster and
        /// the second element is the message to post</param>
        /// <returns>This command always returns null (or an exception in case of failure)</returns>
        public string Execute(string[] arguments)
        {
            if (arguments.Length < 2) throw new ArgumentException("arguments");
            var userName = arguments[0];
            var message = arguments[1];

            timelineWriterService.Post(userName, message);

            return null;
        }
    }
}
