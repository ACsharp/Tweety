using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweety.Contracts.Entities;
using Tweety.Contracts.Interfaces;

namespace Tweety.Logic.Repository
{
    /// <summary>
    /// Memory-based implementation of the user repository
    /// <remarks>It is a repository of User entities including their navigation properties</remarks>
    /// </summary>
    public class UserRepository : IRepository<User>
    {
        /// <summary>
        /// Internal storage based on a dictionary that uses the user name as a key
        /// </summary>
        private Dictionary<string, User> users = new Dictionary<string, Contracts.Entities.User>();

        /// <summary>
        /// Gets the user object matching the provided name.
        /// If no user exists with such name, a new user object is added to the repository and returned.
        /// </summary>
        /// <param name="name">Name of the user</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public User GetOrCreate(string name)
        {
            //Argument validation
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("key");

            //The user name matching must be case insensitive
            var loweredKey = name.ToLower();

            //If the internal repository contains the user then returns it
            if (users.ContainsKey(loweredKey))
                return users[loweredKey];
            else
            {
                //if the user does not exist then it creates it, adds it to the internal storage and returns it
                var user = CreateUser(name);
                users.Add(loweredKey, user);
                return user;
            }
        }

        /// <summary>
        /// Creates a new user and initializes its properties with the default values
        /// </summary>
        /// <param name="name">Name of the new user</param>
        /// <returns></returns>
        private User CreateUser(string name)
        {
            var user = new User
            {
                Name = name,
                Timeline = new List<Story>(),
                Following = new List<User>()
            };
            return user;
        }


    }
}
