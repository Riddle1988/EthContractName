using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Nethereum.Util;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace EthEventInvestigator
{
    internal static class CommandLineParser
    {
        #region Nested classes/structs
        /// <summary>
        /// Internal argument class each parsed argument has one object of this element
        /// </summary>
        internal class Argument
        {
            /// <summary>
            /// Description that is printed when is help used on this argument
            /// </summary>
            public String Description { get; }

            /// <summary>
            /// Current value of the argument before parsing
            /// </summary>
            public String Value { get; set; }

            /// <summary>
            /// True if is mandatory argument
            /// </summary>
            public Boolean IsMandatory { get; set; }

            /// <summary>
            /// Which method is used to parse argument
            /// </summary>
            public Action<String> Method { get; }

            /// <summary>
            /// Constructor of the argument class which parses the argument
            /// </summary>
            /// <param name="description">Help text</param>
            /// <param name="method">Method used for parsing</param>
            public Argument(String description, Action<String> method, Boolean isMandatory = false)
            {
                Description = description;
                Method = method;
                IsMandatory = isMandatory;
            }
        }
        #endregion

        #region Properties Parsed values
        public static String EthAccount { get; private set; }
        public static String AbiJsonFile { get; private set; } = "contract.json";
        #endregion


        private static Dictionary<String, Argument> options;

        #region Methods
        /// <summary>
        /// Parse all supported arguments,
        /// if possible otherwise print help, can set exit code to BadCommandLineArguments
        /// </summary>
        /// <returns>Parse arguments </returns>
        public static void Parse()
        {
            try
            {
                String[] args = Environment.GetCommandLineArgs();
                args = args.Skip(1).ToArray();
                Options();
                List<String> mandatoryKeys = new List<String>();
                foreach(var option in options)
                {
                    if (option.Value.IsMandatory)
                    {
                        mandatoryKeys.Add(option.Key);
                    }
                }
                for (Int32 index = 0; index < args.Length; index++)
                {
                    Char[] delimiter = { '=' };
                    String[] separate = args[index].Split
                        (delimiter, 2, StringSplitOptions.RemoveEmptyEntries);

                    String parsedKey = separate[0].ToLower();
                    String parsedValue = separate[1].ToLower();

                    if (options.ContainsKey(parsedKey))
                    {
                        options[parsedKey].Value = parsedValue;
                        options[parsedKey].Method(parsedValue);
                        if (mandatoryKeys.Contains(parsedKey))
                        {
                            mandatoryKeys.Remove(parsedKey);
                        }
                    }
                }
                if (mandatoryKeys.Count != 0)
                {
                    ParseExceptions argumentMissing = 
                        new ParseExceptions("Not all arguments are present");
                    throw argumentMissing;
                }
            }
            catch (Exception ex)
            {
                ParseExceptions parseException = ex as ParseExceptions;
                PrintHelp();
                throw parseException ?? ex;
            }
        }

        /// <summary>
        /// To add a new argument just add element to the list and implement delegate below
        /// </summary>
        private static void Options()
        {
            options = new Dictionary<String, Argument>
            {
                {
                    "address",
new Argument(
    @"Ethereum contract address (usage: address=<validAddress>) <default: N/A> <mandatory=true>"
    , ParseEthAddress
    , true)
                },
                {
                    "abi",
 new Argument(
    @"Ethereum abi JSON file (usage: abi=<validAbiJSON>.json>) <default: contract.json> <mandatory=false>"
    , ParseAbi)
                }
            };
        }

        /// <summary>
        /// Prints help for each argument
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine("\nOptions:");
            foreach (var argument in options)
            {
                Console.WriteLine
                    (String.Format("{0,-8} -> {1}", argument.Key, argument.Value.Description));
            }
        }

        /// <summary>
        /// Parse Eth address throw exception if fails
        /// </summary>
        /// <param name="argument">argument value / Eth string address</param>
        private static void ParseEthAddress(String argument)
        {
            AddressUtil parseAddress = new AddressUtil();
            if(parseAddress.IsValidEthereumAddressHexFormat(argument))
            {
                EthAccount = argument;
            }
            else
            {
                ParseExceptions invalidAddress = new ParseExceptions("Not a valid address");
                throw invalidAddress;
            }
        }

        /// <summary>
        /// Find given json file inside run directory, throw exception if not found
        /// </summary>
        /// <param name="argument">argument value / json file name</param>
        private static void ParseAbi(String argument)
        {
            String filePath = 
                    Path.Combine
                    (
                          Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
                        , argument
                    );

            if (File.Exists(filePath))
            {
                AbiJsonFile = filePath;
            }
            else
            {
                ParseExceptions fileNotFound =
                    new ParseExceptions($"File {argument} not found in run directory");
                throw fileNotFound;
            }
        }
        #endregion
    }

    /// <summary>
    /// Exceptions only for CommandLineParser class which are caught in main method
    /// </summary>
    internal class ParseExceptions : Exception
    {
        public static String printMessage { get; private set; } =
            "\nCommand line argument not valid:\n";
        public ParseExceptions(String message)
            : base(message)
        {
            /*empty*/
        }
        public ParseExceptions(string message, Exception inner)
            : base(message, inner)
        {
            /*empty*/
        }
    }
}
