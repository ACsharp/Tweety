using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweety.Contracts.Interfaces
{
    /// <summary>
    /// Contract of an object that can handle text requests with a specific signature
    /// (usually by calling the appropriate business logic class)
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Returns the signature of the text request that the command can handle
        /// </summary>
        /// <remarks>
        /// The signature is a regular expression that is used by the command handler
        /// to match the command and to parse the argument array from the request 
        /// </remarks>
        string Signature { get; }

        /// <summary>
        /// Performs the command (usually by calling the appropriate business logic class)
        /// </summary>
        /// <param name="arguments">
        /// Zero or more command arguments 
        /// (usually parsed by the command handler using the regular expression contained
        /// by the <see cref="Signature"/> property)
        /// </param>
        /// <returns></returns>
        string Execute(string[] arguments);
    }
}
