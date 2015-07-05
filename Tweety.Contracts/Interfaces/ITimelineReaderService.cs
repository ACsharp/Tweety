using System;
using System.Collections.Generic;
using Tweety.Contracts.Entities;

namespace Tweety.Contracts.Interfaces
{
    /// <summary>
    /// Contract of service classes that can read user timelines
    /// </summary>
    public interface ITimelineReaderService
    {
        /// <summary>
        /// Read the timeline of the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Set of Story items contained by the user's timeline</returns>
        IEnumerable<Story> GetTimeline(string userName);
    }
}
