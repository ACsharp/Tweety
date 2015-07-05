using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweety.Contracts.Entities;
using Tweety.Contracts.Interfaces;

namespace Tweety.Logic.Service
{
    /// <summary>
    /// Service class that writes to user timelines
    /// </summary>
    public class TimelineWriterService : ITimelineWriterService
    {
        private IRepository<User> userRepository;

        /// <summary>
        /// </summary>
        /// <param name="userRepository">repository object to rely on for data access</param>
        public TimelineWriterService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Post a story to a user timeline
        /// </summary>
        /// <param name="recipientUserName">Name of the user that owns the timeline in which you want to post the message</param>
        /// <param name="message">Massege you want to post to the timeline</param>
        public void Post(string recipientUserName, string message)
        {
            //Creates the story entity to post
            var story = new Story
            {
                Message = message,
                PostedOn = DateTime.Now
            };

            //Gets the user from the repository
            var user = userRepository.GetOrCreate(recipientUserName);

            //Adds the story to the user's timeline
            user.Timeline.Add(story);
        }
    }
}
