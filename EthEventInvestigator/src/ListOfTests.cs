using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthEventInvestigator
{
    /// <summary>
    /// Class that contains list of currently available test cases
    /// </summary>
    internal static class MapOf
    {
        public static Dictionary<String, String> Tests = new Dictionary<string, string>()
        {
            { "ProtocolVersion", "TC000" },
            { "GetCurrentBlockNumber", "TC001" },
            { "BlockIteration", "TC002" },
            { "BidRevealedEvent", "TC003" },
            { "BidRevealedEventHistory", "TC004" },
            { "GrabBalance", "TC005" },
            { "PrintAllContractEvents", "TC006" },
            { "AllChanges", "TC007" },
            { "ContractValuesHistory", "TC008" },
        };
    }
}
