using System;

namespace EthEventInvestigator
{
    [Flags]
    public enum ExitCode : int
    {
        // Application has encountered an general error
        SystemError = 4,

        // Address could not be parsed or contacted
        BadEthAddress = 5,

        // Could not load Eth ABI from json
        BadAbiLoad = 6,

        // Command line arguments could not be parsed
        BadCommandLineArguments = 7,
    }
}
