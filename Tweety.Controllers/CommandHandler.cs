using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using Tweety.Contracts.Interfaces;

namespace Tweety.Controllers
{
    /// <summary>
    /// Handle text requests, route them to the appropriate command and obtain the related result 
    /// </summary>
    public class CommandHandler : ICommandHandler
    {
        private Dictionary<Regex, ICommand> commandDictionary;

        /// <summary>
        /// </summary>
        /// <param name="supportedCommands">
        /// Set of ICommand objects that can parse text requests and get them done.
        /// CommandHandler relyes on the Signature property of these ICommand objects to parse the
        /// text requests and to route them to the appropriate command
        /// </param>
        public CommandHandler(params ICommand[] supportedCommands)
        {
            //Builds a dictionary having:
            //    key = the Regex obtained from the command signatures
            //    value = the command instances

            commandDictionary = new Dictionary<Regex, ICommand>(supportedCommands.Length);

            foreach (var cmd in supportedCommands)
                commandDictionary.Add(new Regex(cmd.Signature, RegexOptions.IgnoreCase), cmd);
        }

        /// <summary>
        /// Handles the text request by routing it to the command with the matching signature
        /// and returns the related result
        /// </summary>
        /// <param name="commandText">Request content</param>
        /// <returns>
        /// If the request generates output, return the text output, otherwise return null.
        /// If the request does not match any supported ICommand object, return an error text
        /// </returns>
        public string HandleUserCommand(string commandText)
        {
            foreach (var commandSignature in commandDictionary.Keys)
            {
                //Try evaluating the request text against the Regex object linked to each supportedCommand
                var match = commandSignature.Match(commandText);
                if (match.Success) //The request matches the command signature
                {
                    //Get the command argument excluding the first Regex match as it contains the whole request
                    var arguments = match.Groups.OfType<Group>().Skip(1).Select(g => g.Value);
                    
                    //Get the command instance and perform the command passing the parsed arguments
                    var command = commandDictionary[commandSignature];
                    return command.Execute(arguments.ToArray());
                }
            }
            //The request does not match any supported ICommand object
            return LanguageSpecificText.UnknownCommand;
        }
    }
}
