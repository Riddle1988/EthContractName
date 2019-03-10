using Microsoft.CSharp.RuntimeBinder;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EthEventInvestigator
{
    enum SetVerdict
    {
        pass = 0,
        fail = 1,
        inconclusive = 2,
        error = 3,
        comment = 4,
        notset = 5
    }

    /// <summary>
    /// Class that contains all currently available test cases
    /// </summary>
    internal class Tests
    {

        private readonly ContractLoader ContractAbi = null;
        private Web3 Web3 = null;
        private Contract OurContract;
        private List<String> ListOfContractEvents = new List<String>();

        public Tests()
        {
            ContractAbi = ContractLoader.LoadConfiguration(CommandLineParser.AbiJsonFile);
            Web3 = new Web3();
            OurContract = 
                Web3.Eth.GetContract(ContractLoader.ContractString, CommandLineParser.EthAccount);
        }

        /// <summary>
        /// Check eth protocol version
        /// </summary>
        /// <returns></returns>
        [TestCase("ProtocolVersion", "TC000")]
        public async Task ProtocolVersion()
        {
            String protocolVersion = String.Empty;
            protocolVersion = await Web3.Eth.ProtocolVersion.SendRequestAsync();

            Boolean check = protocolVersion != String.Empty;
            Reporting.LogToConsole($"Protocol version:{protocolVersion.ToString()}", check, null);
        }

        /// <summary>
        /// Grab latest block number from RPC client and check is it bigger than 0
        /// </summary>
        /// <returns></returns>
        [TestCase("GetCurrentBlockNumber", "TC001")]
        public async Task GetCurrentBlockNumber()
        {
            HexBigInteger currentBlockNumber = null;
            currentBlockNumber = await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            
            Boolean check = currentBlockNumber.Value > 0;
            Reporting.
                LogToConsole($"Block value:{currentBlockNumber.Value.ToString()}", check, null);

        }

        /// <summary>
        /// Iterate last 200000 blocks
        /// </summary>
        /// <returns></returns>
        [TestCase("BlockIteration", "TC002")]
        public async Task BlockIteration()
        {

            HexBigInteger currentBlockNumber = 
                await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            UInt64 currentBlockNumberInt64 = (UInt64)currentBlockNumber.Value;

            Int64 txTotalCount = 0;
            for 
                ( UInt64 blockNumber = (currentBlockNumberInt64 - 200000)
                ; blockNumber <= currentBlockNumberInt64
                ; blockNumber++ )
            {
                BlockParameter blockParameter = new BlockParameter(blockNumber);
                var block = 
                    await Web3
                    .Eth
                    .Blocks
                    .GetBlockWithTransactionsByNumber
                    .SendRequestAsync(blockParameter);
                if(block != null)
                {
                    Transaction[] trans = block.Transactions;
                    Int32 txCount = trans.Length;
                    txTotalCount += txCount;
                    if (blockNumber % 1000 == 0) Console.Write(".");
                    if (blockNumber % 10000 == 0)
                    {
                        DateTime blockDateTime = Helpers.UnixTimeStampToDateTime
                            ((double)block.Timestamp.Value);
                        Console.WriteLine
                            (blockNumber.ToString() 
                            + " " + txTotalCount.ToString() 
                            + " " + blockDateTime.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Just find the event
        /// </summary>
        /// <returns></returns>
        [TestCase("BidRevealedEvent", "TC003")]
        public async Task BidRevealedEvent()
        {

            Event<BidRevealedEvent> contractEvent = null;
            contractEvent = OurContract.GetEvent<BidRevealedEvent>(CommandLineParser.EthAccount);

            Boolean check = contractEvent != null;

            Reporting.LogToConsole($"Check is Event returned", check, null);

            await Task.Yield();
        }

        /// <summary>
        /// Get all transactions in the last hour registered with BidRevealed event
        /// </summary>
        /// <returns></returns>
        [TestCase("BidRevealedEventHistory", "TC004")]
        public async Task BidRevealedEventHistory()
        {

            DateTime twoDaysBefore = DateTime.Now.AddHours(-1);
            DateTime latestBlockTime = DateTime.Now;


            Event contractEvent = OurContract.GetEvent("BidRevealed");
            HexBigInteger currentBlockNumber = await Web3
                .Eth
                .Blocks
                .GetBlockNumber
                .SendRequestAsync();
            BlockWithTransactions block = null;
            Int32 position = 0;
            while (twoDaysBefore < latestBlockTime)
            {
                block = await Web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync
                    (new HexBigInteger(currentBlockNumber.Value - position));
                latestBlockTime = Helpers.UnixTimeStampToDateTime((double)block.Timestamp.Value);
                position++;
            }

            if (block != null)
            {

                HexBigInteger filterAll2 = await contractEvent.CreateFilterBlockRangeAsync
                    (
                        new BlockParameter(block.Number),
                        new BlockParameter(currentBlockNumber)
                    );
                try
                {
                    var allLogs = await contractEvent.GetAllChanges<BidRevealedEvent>(filterAll2);

                    Int32 number = 0;
                    foreach (var log in allLogs)
                    {
                        number++;
                        Console.WriteLine($"{number}) This is the log class {log.ToString()}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Grab Balance from Ethereum foundation
        /// </summary>
        /// <returns></returns>
        [TestCase("GrabBalance", "TC005")]
        public async Task GrabBalance()
        {
            var web3 = new Web3("https://api.myetherapi.com/eth");

            // Ethereum Foundation account
            var balance = 
                await web3.Eth.GetBalance.SendRequestAsync
                ("0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae");
            Console.WriteLine($"Balance in Wei: {balance.Value}");

            var etherAmount = Web3.Convert.FromWei(balance.Value);
            Console.WriteLine($"Balance in Ether: {etherAmount}");
        }

        /// <summary>
        /// Check Json ABI and print all events
        /// </summary>
        /// <returns></returns>
        [TestCase("PrintAllContractEvents", "TC006")]
        public async Task PrintEvents()
        {
            JArray abi = ContractLoader.ContractJson;
            foreach (dynamic element in abi)
            {
                try
                {
                    if (element.type == "event")
                    {
                        Console.WriteLine($" Event: -> {element.name}");
                        ListOfContractEvents.Add(element.name);
                    }
                }
                catch (RuntimeBinderException ex)
                {
                    /*empty*/
                    // TODO: It wold be faster with reflection
                }
            }

            await Task.Yield();
        }

        /// <summary>
        /// Grab all changes for this contract it could be a large amount
        /// </summary>
        /// <returns></returns>
        [TestCase("AllChanges", "TC007")]
        public async Task AllChanges()
        {
            Event newMessageEvent = OurContract.GetEvent("BidRevealed");
            HexBigInteger filterAllNewMessageEvent = await newMessageEvent.CreateFilterAsync();
            var logNewMessageEvents = await newMessageEvent
                .GetAllChanges<BidRevealedEvent>(filterAllNewMessageEvent);
            foreach (var mea in logNewMessageEvents)
            {
                Console.WriteLine
                    ("newMessageEvent:\t" +
                    mea.Event.Owner
                    + " " + mea.Event.Hash.ToString()
                    + " " + mea.Event.Value.ToString());
            }
        }

        /// <summary>
        /// Value history on last 20000 blocks
        /// </summary>
        /// <returns></returns>
        [TestCase("ContractValuesHistory", "TC008")]
        public async Task ContractValuesHistory()
        {
            HexBigInteger recentBlockNumber = await Web3
                .Eth
                .Blocks
                .GetBlockNumber
                .SendRequestAsync();
            ulong numberBlocks = 20000;
            int offset = 5000;

            String previousValue = "";
            for 
                (ulong blockNumber = (ulong)recentBlockNumber.Value
                ; blockNumber > (ulong)recentBlockNumber.Value - numberBlocks;
                blockNumber--)
            {
                var blockNumberParameter = new BlockParameter(blockNumber);
                var valueAtOffset = await Web3
                    .Eth
                    .GetStorageAt
                    .SendRequestAsync
                        (CommandLineParser.EthAccount
                        , new HexBigInteger(offset)
                        , blockNumberParameter);

                if (valueAtOffset != previousValue)
                {
                    var block = await Web3
                        .Eth
                        .Blocks
                        .GetBlockWithTransactionsByNumber
                        .SendRequestAsync(blockNumberParameter);
                    DateTime blockDateTime = 
                        Helpers.UnixTimeStampToDateTime((double)block.Timestamp.Value);

                    Console.WriteLine("blockDateTime:\t" + blockDateTime.ToString());

                    for (int storageOffset = 0; storageOffset < offset + 2; storageOffset++)
                    {
                        var valueAt = await Web3
                            .Eth
                            .GetStorageAt
                            .SendRequestAsync
                                (CommandLineParser.EthAccount
                                , new HexBigInteger(storageOffset)
                                , blockNumberParameter);

                        Console.WriteLine("value:\t" + blockNumber.ToString() 
                            + " " + storageOffset.ToString() 
                            + " " + valueAt 
                            + " " + Helpers.ConvertHex(valueAt.Substring(2)));
                    }
                    previousValue = valueAtOffset;
                }
            }
        }
    }
}
