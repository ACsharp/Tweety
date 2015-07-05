using System;
using System.Collections.Generic;
using Tweety.Contracts.Entities;

namespace Tweety.Contracts.Interfaces
{
    /// <summary>
    /// Contract of service classes that can aggregate stories from multiple user timelines
    /// </summary>
    public interface IAggregatorService
    {
        /// <summary>
        /// Gets a set of aggregated stories from the timeline of the provided user
        /// AND from the timelines of the users that he or she follows
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Set of aggregated stories including a reference to the users they belong to</returns>
        IEnumerable<AggregatedStory> GetAggregatedStories(string userName);
    }
}
