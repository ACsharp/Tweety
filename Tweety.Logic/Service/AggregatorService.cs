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
    /// Service class that aggregates stories from multiple user timelines
    /// </summary>
    public class AggregatorService : IAggregatorService
    {
        private IRepository<User> userRepository;

        /// <summary>
        /// </summary>
        /// <param name="userRepository">repository object to rely on for data access</param>
        public AggregatorService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Gets a set of aggregated stories from the timeline of the provided user
        /// AND from the timelines of the users that he or she follows
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Set of aggregated stories including a reference to the users they belong to</returns>
        public IEnumerable<AggregatedStory> GetAggregatedStories(string userName)
        {
            //Gets the follower user
            var FollowerUser = userRepository.GetOrCreate(userName);

            //Gets the users that contain the timelines to merge
            var UsersToMergeStoriesFrom = GetUsersToMergeStoriesFrom(FollowerUser);

            //Merges the timelines and returns
            var result = MergeUserStories(UsersToMergeStoriesFrom);
            return result;
        }

        /// <summary>
        /// Gets a list of the followed users AND the follower user
        /// </summary>
        /// <param name="followerUser"></param>
        /// <returns></returns>
        private IEnumerable<User> GetUsersToMergeStoriesFrom(User followerUser)
        {
            var result = followerUser.Following.ToList();
            result.Add(followerUser);
            return result;
        }

        /// <summary>
        /// Gets a set of aggregated stories obtained by merging the timelines of the provided users 
        /// </summary>
        /// <remarks>
        /// The set is ordered by post time descending
        /// </remarks>
        /// <param name="users"></param>
        /// <returns></returns>
        private IEnumerable<AggregatedStory> MergeUserStories(IEnumerable<User> users)
        {
            var result = from user in users
                         from story in user.Timeline
                         orderby story.PostedOn descending
                         select new AggregatedStory
                         {
                             Story = story,
                             BelongingTo = user
                         };
            return result;
                         
        }
    }
}
