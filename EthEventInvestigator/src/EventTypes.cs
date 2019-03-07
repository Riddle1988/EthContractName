using Nethereum.ABI.FunctionEncoding.Attributes;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthEventInvestigator
{
    /// <summary>
    /// Class which is used to cast BidRevealed event into
    /// </summary>
    [Event("BidRevealed")]
    internal class BidRevealedEvent : IEventDTO
    {
        [Parameter("bytes32", "hash", 1, true)]
        public Byte[] Hash { get; set; }

        [Parameter("address", "owner", 2, true)]
        public String Owner { get; set; }

        [Parameter("uint256", "value", 3, false)]
        public BigInteger Value { get; set; }

        [Parameter("uint256", "registrationDate", 4, false)]
        public BigInteger RegistrationDate { get; set; }
    }
}
