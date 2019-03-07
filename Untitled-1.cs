using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
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

        public Tests()
        {
            ContractAbi = ContractLoader.LoadConfiguration(CommandLineParser.AbiJsonFile);
            Web3 = new Web3();
            OurContract = Web3.Eth.GetContract(ContractLoader.ContractString, CommandLineParser.EthAccount);
            //Event<BidRevealedEvent> contractEvent = OurContract.GetEvent<BidRevealedEvent>(CommandLineParser.EthAccount);

            //var filterAll = OurContract.CreateFilterAsync();
            //filterAll.Wait();

            //var resultFilter = filterAll.Result;
            //var logs = contractEvent.GetAllChanges(filterAll.Result);
            //logs.Wait();

            //Console.WriteLine($"This is the resultFilter {resultFilter.ToString()}\n");

            //Int32 number = 0;
            //foreach (var log in logs.Result)
            //{
            //    number++;
            //    Console.WriteLine($"{number}) This is the log class {log.ToString()}");
            //}
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
            Reporting.LogToConsole($"Block value:{currentBlockNumber.Value.ToString()}", check, null);

        }

        /// <summary>
        /// Iterate last 200000 blocks
        /// </summary>
        /// <returns></returns>
        [TestCase("BlockIteration", "TC002")]
        public async Task BlockIteration()
        {

            HexBigInteger currentBlockNumber = await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            UInt64 currentBlockNumberInt64 = (UInt64)currentBlockNumber.Value;

            Int64 txTotalCount = 0;
            for (UInt64 blockNumber = (currentBlockNumberInt64 - 200000); blockNumber <= currentBlockNumberInt64; blockNumber++)
            {
                BlockParameter blockParameter = new BlockParameter(/*blockNumber*/);
                blockParameter.SetValue(BlockParameter.BlockParameterType.pending);
                var block = await Web3.Eth.Blocks.GetBlockTransactionCountByNumber.SendRequestAsync(blockParameter);
                if(block != null)
                {
                    //Transaction[] trans = block.Transactions;
                    //Int32 txCount = trans.Length;
                    //txTotalCount += txCount;
                    //if (blockNumber % 1000 == 0) Console.Write(".");
                    //if (blockNumber % 10000 == 0)
                    //{
                    //    DateTime blockDateTime = Helpers.UnixTimeStampToDateTime((double)block.Timestamp.Value);
                    //    Console.WriteLine(blockNumber.ToString() + " " + txTotalCount.ToString() + " " + blockDateTime.ToString());
                    //}
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
        /// Get all transactions in the last hour
        /// </summary>
        /// <returns></returns>
        [TestCase("BidRevealedEventHistory", "TC004")]
        public async Task BidRevealedEventHistory()
        {


            HexBigInteger currentBlockNumber = await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            await GetContractValuesHistoryUniqueOffsetValueExample(Web3, CommandLineParser.EthAccount, currentBlockNumber, 10000, 1000);

            //// Create transfer event handler using our Event definition class
            //var transferEventHandler = Web3.Eth.GetEvent<BidRevealedEvent>(CommandLineParser.EthAccount);

            //// Create a filter for logs in this case from block 5000 to the latest
            //HexBigInteger currentBlockNumber = await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            //HexBigInteger startBlock = new HexBigInteger(currentBlockNumber.Value - 10000);
            //var filterAllTransferEventsForContract = transferEventHandler.CreateFilterInput
            //    (
            //        //new BlockParameter(startBlock),
            //        //new BlockParameter(currentBlockNumber)
            //    );

            //// Retrieve logs
            //var allTransferEventsForContract = await transferEventHandler.GetAllChanges(filterAllTransferEventsForContract);

            //foreach(var element in allTransferEventsForContract)
            //{
            //    Console.WriteLine(element.Log.Address);
            //}

            //var newMessageEvent = OurContract.GetEvent("BidRevealed");
            //var filterAllNewMessageEvent = await newMessageEvent.CreateFilterAsync(/*fromAddress*/);
            //var logNewMessageEvents = await newMessageEvent.GetAllChanges<BidRevealedEvent>(filterAllNewMessageEvent);
            //foreach (var mea in logNewMessageEvents)
            //{
            //    Console.WriteLine("newMessageEvent:\t" +
            //        mea.Event.Owner + " " + mea.Event.Hash.ToString() + " " + mea.Event.Value.ToString());
            //}




            //DateTime twoDaysBefore = DateTime.Now.AddHours(-2);
            //DateTime latestBlockTime = DateTime.Now;


            //Event contractEvent = OurContract.GetEvent("BidRevealed");
            ////Event<BidRevealedEvent> contractEvent = OurContract.GetEvent<BidRevealedEvent>(CommandLineParser.EthAccount);
            //HexBigInteger currentBlockNumber = await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            //BlockWithTransactions block = null;
            //Int32 position = 0;
            //while (twoDaysBefore < latestBlockTime)
            //{
            //    block = await Web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync
            //        (new HexBigInteger(currentBlockNumber.Value - position));
            //    latestBlockTime = Helpers.UnixTimeStampToDateTime((double)block.Timestamp.Value);
            //    position++;
            //}

            //if (block != null)
            //{

            //    HexBigInteger filterAll2 = await contractEvent.CreateFilterBlockRangeAsync
            //        (
            //            new BlockParameter(block.Number),
            //            new BlockParameter(currentBlockNumber)
            //        );


            //var filterAll =
            //    OurContract.GetDefaultFilterInput
            //        (
            //          new BlockParameter(currentBlockNumber)
            //        , new BlockParameter(block.Number)
            //        );
            //filterAll.Wait();
            //try
            //{
            //    var logs = await contractEvent.GetAllChanges<BidRevealedEvent>(filterAll2);

            //    Int32 number = 0;
            //    foreach (var log in logs)
            //    {
            //        number++;
            //        Console.WriteLine($"{number}) This is the log class {log.ToString()}");
            //    }
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
            //var logs = contractEvent.GetAllChanges(filterAll);
            //logs.Wait();
            //}


            ////for (int i = 0; i <= 10; i++)
            ////{
            ////    var block = await Web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(currentBlockNumber.Value - i));
            ////    searchBlocks.Add(block);
            ////}
        }

        static public async Task GetContractValuesHistoryUniqueOffsetValueExample(Web3 web3, string contractAddress, HexBigInteger recentBlockNumber, ulong numberBlocks, int offset)
        {
            Console.WriteLine("GetContractValuesHistoryUniqueOffsetValueExample:");

            string previousValue = "";
            for (ulong blockNumber = (ulong)recentBlockNumber.Value; blockNumber > (ulong)recentBlockNumber.Value - numberBlocks; blockNumber--)
            {
                var blockNumberParameter = new BlockParameter(blockNumber);
                var valueAtOffset = await web3.Eth.GetStorageAt.SendRequestAsync(contractAddress, new HexBigInteger(offset), blockNumberParameter);
                if (valueAtOffset != previousValue)
                {
                    var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumberParameter);
                    DateTime blockDateTime = Helpers.UnixTimeStampToDateTime((double)block.Timestamp.Value);
                    Console.WriteLine("blockDateTime:\t" + blockDateTime.ToString());

                    for (int storageOffset = 0; storageOffset < offset + 2; storageOffset++)
                    {
                        var valueAt = await web3.Eth.GetStorageAt.SendRequestAsync(contractAddress, new HexBigInteger(storageOffset), blockNumberParameter);
                        Console.WriteLine("value:\t" + blockNumber.ToString() + " " + storageOffset.ToString() + " " + valueAt + " " + Helpers.ConvertHex(valueAt.Substring(2)));
                    }
                    previousValue = valueAtOffset;
                }
            }
        }
    }
}