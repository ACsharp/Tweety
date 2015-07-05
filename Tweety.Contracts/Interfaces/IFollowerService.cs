using System;
namespace Tweety.Contracts.Interfaces
{
    /// <summary>
    /// Contract of service classes that can manage the follower-followed relationship of users
    /// </summary>
    public interface IFollowerService
    {
        /// <summary>
        /// Registers a user (the follower) to follow another user (the followed)
        /// </summary>
        /// <param name="followerUserName"></param>
        /// <param name="followedUserName"></param>
        void SetFollowing(string followerUserName, string followedUserName);
    }
}
