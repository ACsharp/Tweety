using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweety.Contracts.Entities
{
    /// <summary>
    /// Entity containing a story and additional information related to its aggregation
    /// </summary>
    /// <remarks>
    /// This entity is generally used to represent story items aggregated from different timelines
    /// </remarks>
    public class AggregatedStory
    {
        /// <summary>
        /// The story entity (usually part of a user's timeline)
        /// </summary>
        public Story Story { get; set; }

        /// <summary>
        /// The user that owns the timeline containing the story
        /// </summary>
        public User BelongingTo { get; set; }
    }
}
