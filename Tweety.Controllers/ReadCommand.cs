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
    /// Command that can handle text requests to read user's timelines
    /// </summary>
    /// <example>Alice</example>
    public class ReadCommand : Contracts.Interfaces.ICommand
    {
        //command signature: {UserName}
        private const string signature = @"^([^\s][A-Za-z0-9-_]+)$";

        private ITimelineReaderService timelineReaderService;
        private IEntityFormatter<Story> storyFormatter;

        /// <summary>
        /// </summary>
        /// <param name="timelineReaderService">Service class to use to perform the command</param>
        /// <param name="storyFormatter">Object that this class uses to text-render the story items</param>
        public ReadCommand(ITimelineReaderService timelineReaderService, IEntityFormatter<Story> storyFormatter)
        {
            this.timelineReaderService = timelineReaderService;
            this.storyFormatter = storyFormatter;
        }

        /// <summary>
        /// Returns the signature of the text request that this command can handle
        /// </summary>
        /// <remarks>
        /// <para>Command signature: {UserName}</para>
        /// <para>Regular expression: ^([^\s][A-Za-z0-9-_]+)$</para>
        /// </remarks>
        public string Signature
        {
            get { return signature; }
        }

        /// <summary>
        /// Performs the command by calling the appropriate service class
        /// </summary>
        /// <param name="arguments">Array of arguments where
        /// the only element is the user name</param>
        /// <returns>This command returns zero or more lines. Each one represents a story</returns>
        public string Execute(string[] arguments)
        {
            if (arguments.Length < 1) throw new ArgumentException("arguments");
            var userName = arguments[0];

            var timeline = timelineReaderService.GetTimeline(userName);

            var formattedTimeline = timeline.Select(s => storyFormatter.Format(s));

            var result = string.Join(Environment.NewLine, formattedTimeline);

            return result;
        }
    }
}
