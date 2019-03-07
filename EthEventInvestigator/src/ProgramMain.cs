using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthEventInvestigator
{
    internal static class Program
    {
        [MTAThread] // Hopefully no STA COM objects
        static void Main(String[] args)
        {
            try
            {
                CommandLineParser.Parse();
                TestRunner testRunner = new TestRunner();
                testRunner.Run();
            }
            catch (ParseExceptions ex)
            {
                Console.WriteLine($"{ParseExceptions.printMessage} ---> {ex.Message}");
                Environment.ExitCode = (Int32)ExitCode.BadCommandLineArguments;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nGeneral exception caught:\n ---> {ex.Message}");
                Environment.ExitCode = (Int32)ExitCode.SystemError;
            }
            finally
            {
                Environment.Exit(Environment.ExitCode);
            }
        }
    }
}
