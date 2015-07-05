using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweety.Contracts.Entities;
using Tweety.Contracts.Interfaces;

namespace Tweety.Controllers
{
    /// <summary>
    /// This class can format AggregatedStory objects in a textual human-readable form 
    /// </summary>
    public class AggregatedStoryFormatter : IEntityFormatter<AggregatedStory>
    {
        private IEntityFormatter<Story> storyFormatter;

        /// <summary>
        /// </summary>
        /// <param name="storyFormatter">
        /// Formatter to rely on when sub-formatting the Story objects
        /// contained by AggregatedStory objects</param>
        public AggregatedStoryFormatter(IEntityFormatter<Story> storyFormatter)
        {
            this.storyFormatter = storyFormatter;
        }

        /// <summary>
        /// Renders the provided AggregatedStory in a textual human-readable form
        /// </summary>
        /// </summary>
        /// <param name="aggregatedStory"></param>
        /// <returns></returns>
        public string Format(AggregatedStory aggregatedStory)
        {
            //Uses the storyFormatter to format the Story object contained by the AggregatedStory object
            string formattedStory = storyFormatter.Format(aggregatedStory.Story);

            //Formats the aggregated story including the formatted story 
            string formattedAggregatedStory = string.Format(
                "{0} - {1}", aggregatedStory.BelongingTo.Name, formattedStory);

            return formattedAggregatedStory;
        }

    }
}
