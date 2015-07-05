using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweety.Contracts.Interfaces;
using Tweety.Contracts.Entities;

namespace Tweety.Controllers
{
    /// <summary>
    /// Command that can handle text requests from a user to follow another user
    /// </summary>
    /// <example>Alice follows Bob</example>
    public class FollowsCommand : Contracts.Interfaces.ICommand
    {
        //command signature: {UserName} follows {UserName}
        private const string signature = @"^([^\s][A-Za-z0-9-_]+) follows ([^\s][A-Za-z0-9-_]+)$";

        private IFollowerService followerService;

        /// <summary>
        /// </summary>
        /// <param name="followerService">Service class to use to perform the command</param>
        public FollowsCommand(IFollowerService followerService)
        {
            this.followerService = followerService;
        }

        /// <summary>
        /// Returns the signature of the text request that this command can handle
        /// </summary>
        /// <remarks>
        /// <para>Command signature: {UserName} follows {Message}</para>
        /// <para>Regular expression: ^([^\s][A-Za-z0-9-_]+) follows ([^\s][A-Za-z0-9-_]+)$</para>
        /// </remarks>
        public string Signature
        {
            get { return signature; }
        }

        /// <summary>
        /// Performs the command by calling the appropriate service class
        /// </summary>
        /// <param name="arguments">Array of arguments where
        /// the first element is the user name of the follower and
        /// the second element is the user name of the followed</param>
        /// <returns>This command always returns null (or an exception in case of failure)</returns>
        public string Execute(string[] arguments)
        {
            if (arguments.Length < 2) throw new ArgumentException("arguments");
            var followerUserName = arguments[0];
            var followedUserName = arguments[1];

            followerService.SetFollowing(followerUserName, followedUserName);

            return null;
        }
    }
}
