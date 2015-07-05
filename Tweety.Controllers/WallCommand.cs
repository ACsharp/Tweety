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
    /// Command that can handle text requests to read user's wall
    /// </summary>
    /// <example>Alice wall</example>
    public class WallCommand : Contracts.Interfaces.ICommand
    {
        //command signature: {UserName} wall
        private const string signature = @"^([^\s][A-Za-z0-9-_]+) wall$";

        private IAggregatorService aggregatorService;
        private IEntityFormatter<AggregatedStory> storyFormatter;

        /// <summary>
        /// </summary>
        /// <param name="aggregatorService">Service class to use to perform the command</param>
        /// <param name="storyFormatter">Object that this class uses to text-render the aggregated story items</param>
        public WallCommand(IAggregatorService aggregatorService, IEntityFormatter<AggregatedStory> storyFormatter)
        {
            this.aggregatorService = aggregatorService;
            this.storyFormatter = storyFormatter;
        }

        /// <summary>
        /// Returns the signature of the text request that this command can handle
        /// </summary>
        /// <remarks>
        /// <para>Command signature: {UserName} wall</para>
        /// <para>Regular expression: ^([^\s][A-Za-z0-9-_]+) wall$</para>
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
        /// <returns>This command returns zero or more lines. Each one represents an aggregated story</returns>
        public string Execute(string[] arguments)
        {
            if (arguments.Length < 1) throw new ArgumentException("arguments");
            var userName = arguments[0];

            var wall = aggregatorService.GetAggregatedStories(userName);

            var formattedTimeline = wall.Select(s => storyFormatter.Format(s));

            var result = string.Join(Environment.NewLine, formattedTimeline);

            return result;
        }
    }
}
