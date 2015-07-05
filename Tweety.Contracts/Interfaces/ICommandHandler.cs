using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweety.Contracts.Interfaces
{
    /// <summary>
    /// Contract of an object that can handle text requests
    /// (usually routing them to the appropriate command) 
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Handles the text request (usually routing it to the appropriate command)
        /// and returns the related result
        /// </summary>
        /// <param name="commandText">Request content</param>
        /// <returns>If the request generates output then returns the text output, otherwise returns null</returns>
        string HandleUserCommand(string commandText);
    }
}
