using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweety.Contracts.Interfaces;
using Microsoft.Practices.Unity.Configuration;

namespace Tweety.ConsoleUI
{
    class Program
    {
        private static IUnityContainer IoCcontainer;
        private static ICommandHandler commandHandler; //DI Resolution Root
        
        static void Main(string[] args)
        {
            //Initializes dependencies using the IoC Container
            InitializeDependencies();

            while (true)
            {
                Console.Write("> "); //Writes command prompt symbol to the console

                //Uses commandHandler to handle the user entered text
                var result = commandHandler.HandleUserCommand(Console.ReadLine());

                //If the command handeler returns any result than writes it to the console
                if (result != null)
                    Console.WriteLine(result);
            }
        }

        /// <summary>
        /// Initializes the IoC Container (using Convention over Configuration)
        /// and resolve the resolution root (commandHandler)
        /// </summary>
        private static void InitializeDependencies()
        {
            var IoCcontainer = new UnityContainer();

            IoCcontainer.RegisterTypes(
                  AllClasses.FromAssembliesInBasePath(),
                  WithMappings.FromMatchingInterface,
                  WithName.Default,
                  WithLifetime.ContainerControlled);

            IoCcontainer.LoadConfiguration();

            //Gets the resolution root
            commandHandler = IoCcontainer.Resolve<ICommandHandler>();
        }
    }
}
