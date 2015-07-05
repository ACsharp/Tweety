using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweety.Contracts.Entities;
using Tweety.Contracts.Interfaces;

namespace Tweety.Controllers
{
    /// <summary>
    /// This class can format Story objects in a textual human-readable form 
    /// </summary>
    public class StoryFormatter : IEntityFormatter<Story>
    {
        /// <summary>
        /// Renders the provided Story in a textual human-readable form
        /// </summary>
        /// <param name="story"></param>
        /// <returns></returns>
        public string Format(Story story)
        {
            return string.Format("{0} ({1})", story.Message, TimeAgo(story.PostedOn));
        }

        /// <summary>
        /// Converts a past date in a textual form that express the tome difference
        /// </summary>
        /// <example>yesterday, 5 minutes ago, 2 hours ago</example>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string TimeAgo(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;

            if (span.Days > 1)
                return String.Format("{0} {1} {2}", span.Days, LanguageSpecificText.Days, LanguageSpecificText.Ago);
            if (span.Days == 1)
                return LanguageSpecificText.Yesterday;
            if (span.Hours > 0)
                return String.Format("{0} {1} {2}", span.Hours, span.Hours == 1 ? LanguageSpecificText.Hour : LanguageSpecificText.Hours, LanguageSpecificText.Ago);
            if (span.Minutes > 0)
                return String.Format("{0} {1} {2}", span.Minutes, span.Minutes == 1 ? LanguageSpecificText.Minute : LanguageSpecificText.Minutes, LanguageSpecificText.Ago);

            return String.Format("{0} {1} {2}", span.Seconds, span.Seconds == 1 ? LanguageSpecificText.Second : LanguageSpecificText.Seconds, LanguageSpecificText.Ago);
        }
    }
}
