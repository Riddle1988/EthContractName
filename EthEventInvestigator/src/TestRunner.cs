using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EthEventInvestigator
{
    /// <summary>
    /// Start the test cases by iterating on test class methods
    /// (show knowledge of the reflection use)
    /// </summary>
    internal class TestRunner
    {
        #region Fields
        private readonly Tests Tests = null;
        #endregion

        #region Constructors
        public TestRunner()
        {
            Tests = new Tests();
        }
        #endregion

        #region Methods
        public void Run()
        {
            Console.WriteLine("\nStarting to perform tests...");

            foreach (KeyValuePair<String, String> testName in MapOf.Tests)
            {
                TestCaseAttribute testCase;

                // Search for the correct test case in class with tests 
                MethodInfo method = Tests.GetType().GetMethods().Where(p =>
                    p.IsDefined(typeof(TestCaseAttribute), false)).SingleOrDefault(p =>
                    {
                        Object tmp = p.GetCustomAttributes(typeof(TestCaseAttribute), false).First();
                        testCase = (TestCaseAttribute)tmp;
                        Boolean tcName = testCase.TcName == testName.Key;
                        Boolean tcNumber = testCase.TcNumber == testName.Value;

                        return (tcName && tcNumber);
                    });

                if (method != null)
                {
                    Console.WriteLine($"\nStarting test case->\t {testName}");
                    Task tcRun = (Task) method.Invoke(Tests, null);
                    tcRun.Wait(); // Run sync. for future use
                    Console.WriteLine($"End of test case->\t {testName}");
                }
            }
        }
        #endregion

    }
}
