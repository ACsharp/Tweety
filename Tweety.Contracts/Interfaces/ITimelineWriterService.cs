using System;
namespace Tweety.Contracts.Interfaces
{
    /// <summary>
    /// Contract of service classes that can write to user timelines
    /// </summary>
    public interface ITimelineWriterService
    {
        /// <summary>
        /// Post a story to a user timeline
        /// </summary>
        /// <param name="recipientUserName">Name of the user that owns the timeline in which you want to post the message</param>
        /// <param name="message">Massege you want to post to the timeline</param>
        void Post(string recipientUserName, string message);
    }
}
