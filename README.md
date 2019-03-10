# EthContractName
CLI tool that access etherium network and with a given abi and address searches for specific event and runs some specific test cases

### Preconditions for running:
- .NET Framework 4.5.2
- synced etherium client (e.g. parity) running a node

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

Also needs additional verification with properly synced eth node

