using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthEventInvestigator
{
    /// <summary>
    /// Should contain all elements of the reporting in this case only console logging
    /// </summary>
    internal static class Reporting
    {
        public static void LogToConsole
            (String verificationPointText
            , Boolean check
            , Exception caughtException)
        {
            /* 
            Report cases:
            1. check == true && caughtException == false --> pass
            2. check == false && caughtException == false --> fail
            3. check == true && caughtException == true --> error
            4. check == false && caughtException == true --> fail with exception
            5. other options --> inconclusive
            */
            SetVerdict vp = SetVerdict.notset;

            if (caughtException == null)
            {
                vp = check ? SetVerdict.pass : SetVerdict.fail;
            }
            else if ((check) && (caughtException != null))
            {
                vp = SetVerdict.error;
            }
            else if ((!check) && (caughtException != null))
            {
                vp = SetVerdict.fail;
            }
            else
            {
                vp = SetVerdict.inconclusive;
            }

            Console.Write($"status of {verificationPointText} test:  ");
            Console.ForegroundColor = check ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"{ vp.ToString()}");
            Console.ResetColor();
        }
    }
}
