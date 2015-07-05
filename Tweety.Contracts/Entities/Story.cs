using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweety.Contracts.Entities
{
    /// <summary>
    /// A Story entity (usually part of a user's timeline)
    /// </summary>
    public class Story
    {
        /// <summary>
        /// Text message representing the content of the story
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Date and time when the story was posted
        /// </summary>
        public DateTime PostedOn { get; set; }
    }
}
