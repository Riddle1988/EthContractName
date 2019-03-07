# EthContractName
CLI tool that can visualize contract events on the ETH network for checking the logs

### Preconditions for running:
- .NET Framework 4.5.2

### Preconditions for building:
- Developed with VS2015 so either that or MSBuild Xbuild or something that can build targeted framework .NET 4.5.2
- NuGet and Internet connection required to restore dependencies

### Building:
- There is postbuild command that copies default contract.json file from contract folder in solution root to the build directory
- build directory is solutionDir/Build/EthEventInvestigator for both debug and release versions

### How is code organized:
CommandLineParser.cs -> CLA handling  
ContractLoader.cs -> Singleton for loading json ABI  
EventTypes.cs -> contains contract event classes ("TODO see can we use dynamic casting")  
ExitCode.cs -> current list of exit codes  
Helpers.cs -> from example: https://hyperonomy.com/2017/12/19/ethereum-net-examples-using-the-nethereum-libraries/  
ListOfTests.cs -> Contains current list of tests  
Reporting.cs -> Currently used for console printing handling  
TestAttribute.cs -> attributes used to describe and call test methods  
TestRunner.cs -> Used for staring test methods (TODO: currently it is not asnyc)  
Tests.cs -> class that contains all test methods  

### Current usage (running the program):
    Command line options:
    address  -> Ethereum contract address (usage: address=<validAddress>) <default: N/A> <mandatory=true>
    abi      -> Ethereum abi JSON file (usage: abi=<validAbiJSON>.json>) <default: contract.json> <mandatory=false>

Provided .json file will be searched form the run directory, by default 

### Status and TODOs
Current status: not fully working needs more work


##### 1. Iterate and grab all events from the contract
Ideas: Iterate the ABI JArray and find all JObject.type=event. 
It should be possible to make a generic method and using the reflection make an "TEvent" object of all Event object and than call them.
Or easier approach but with less "power" is just to grab names and list them.

##### 2. BlockIteration test FIX
Ideas: For some reason it seems block cannot be assigned and RPC call either returns null or has some timeouts/exceptions.
Read documentation of nethereum more :)

##### 3. BidRevealedEventHistory needs a proper way to work
Ideas: Well at this point no idea what is wrong with multiple approaches, could be that something with test setup is wrong
since used parity was only synced with --light at first, but after the normal sync different problems are there.
Tryesd so far:

###### Algoritham one:
1. Get current block
2. iterate on the blocks until different between loopBlock and currentBlock time stamp is 2days
3. Create a filter with this block difference
4. grab all event logs from the past two days
Issue: no event log returned

###### Algoritham two:
Similar to one but use eventhandler instead

###### Algoritham three:
Couple of brute force attempts also tried but no luck :)

