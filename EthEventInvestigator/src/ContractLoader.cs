using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthEventInvestigator
{
    /// <summary>
    /// Singleton (non-multithread safe version) for loading contract from json file
    /// </summary>
    internal class ContractLoader
    {
        #region Members Properties
        /// <summary>
        /// Our contract is json with a lot of elements Jarray should work
        /// </summary>
        public static JArray ContractJson { get; private set; } = null;
        /// <summary>
        /// String representation of contract (just for easier use)
        /// </summary>
        public static String ContractString { get; private set; } = null;

        // Singelton instance
        private static ContractLoader instance = null;
        private readonly String jsonFile;
        #endregion

        #region Constructors
        /// <summary>
        /// Private constructor because of the singleton instance
        /// </summary>
        private ContractLoader()
        {
            /*empty*/
        }
        /// <summary>
        /// Private constructor which is used to create singleton instance
        /// </summary>
        /// <param name="fileName"></param>
        private ContractLoader(String fileName)
        {
            jsonFile = fileName;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method should create singleton instance only if json was successfully created
        /// </summary>
        /// <param name="filePath">full path to a file</param>
        /// <returns>singleton instance</returns>
        public static ContractLoader LoadConfiguration(String filePath)
        {
            ContractString = File.ReadAllText(filePath);
            // Load configuration .json file to the memory (to a JObject)
            ContractJson = JArray.Parse(ContractString);

            if (instance == null)
            {
                instance = new ContractLoader(filePath);
            }
            return instance;
        }
        #endregion
    }
}
