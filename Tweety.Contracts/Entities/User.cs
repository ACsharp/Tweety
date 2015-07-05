using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweety.Contracts.Entities
{
    /// <summary>
    /// Entity representing a User of the system
    /// </summary>
    public class User
    {
        /// <summary>
        /// User name used for identity and display purposes 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Set of stories posted by the user
        /// </summary>
        public virtual IList<Story> Timeline { get; set; }

        /// <summary>
        /// List of other users that this user follows
        /// </summary>
        public virtual IList<User> Following { get; set; }
    }
}
