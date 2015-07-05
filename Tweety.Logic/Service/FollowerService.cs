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
    /// Manages the follower-followed relationship of users
    /// </summary>
    public class FollowerService : IFollowerService
    {
        private IRepository<User> userRepository;

        /// <summary>
        /// </summary>
        /// <param name="userRepository">repository object to rely on for data access</param>
        public FollowerService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Registers a user (the follower) to follow another user (the followed)
        /// </summary>
        /// <remarks>
        /// Both the follower and the followed users are automatically created if they don't exist
        /// </remarks>
        /// <param name="followerUserName"></param>
        /// <param name="followedUserName"></param>
        public void SetFollowing(string followerUserName, string followedUserName)
        {
            //Gets follower and followed users from the repository
            var followerUser = userRepository.GetOrCreate(followerUserName);
            var followedUser = userRepository.GetOrCreate(followedUserName);

            //if the followed user is not contained in the Following collection, performs an add
            if (!followerUser.Following.Contains(followedUser))
                followerUser.Following.Add(followedUser);
        }
    }
}
