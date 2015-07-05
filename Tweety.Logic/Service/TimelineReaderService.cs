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
    /// Service class that reads user timelines
    /// </summary>
    public class TimelineReaderService : ITimelineReaderService
    {
        private IRepository<User> userRepository;

        /// <summary>
        /// </summary>
        /// <param name="userRepository">repository object to rely on for data access</param>
        public TimelineReaderService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Read the timeline of the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Set of Story items contained by the user's timeline</returns>
        public IEnumerable<Contracts.Entities.Story> GetTimeline(string userName)
        {
            var user = userRepository.GetOrCreate(userName);
            return user.Timeline.OrderByDescending(s => s.PostedOn);
        }
    }
}
