using System;
using System.Collections.Generic;
using System.Reflection;

namespace EthEventInvestigator
{
    /// <summary>
    /// Attribute used to find test case method (used instead xUnit or similar framework)
    /// </summary>
    internal class TestCaseAttribute : Attribute
    {
        public String TcNumber { get; set; } = null;
        public String TcName { get; set; } = null;

        public TestCaseAttribute(String tcName, String tcNumber)
        {
            TcNumber = tcNumber;
            TcName = tcName;
        }
    }
}
