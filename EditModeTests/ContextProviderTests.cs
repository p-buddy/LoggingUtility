using NUnit.Framework;

using pbuddy.LoggingUtility.EditModeTests.TestObjects;
using static pbuddy.LoggingUtility.RuntimeScripts.ContextProvider;

namespace pbuddy.StringUtility.EditModeTests
{
    public class ContextProviderTests 
    {
        [Test]
        public void LogFromMemberFunctionTest()
        {
            TestStructThatLogsOnNonDefaultConstruction testStruct = new TestStructThatLogsOnNonDefaultConstruction();
            TestClass testClass = new TestClass();
            TestClass.NestedTestStruct nestedTestStruct = new TestClass.NestedTestStruct();
            GenericTestStruct<int> genericTestClass = new GenericTestStruct<int>();

            while (NextStrategy())
            {
                testStruct.TestFunction();
                testClass.TestFunction();
                nestedTestStruct.TestFunction();
                genericTestClass.TestFunction();
            }
        }

        [Test]
        public void LogOnConstructionTest()
        {
            new TestStructThatLogsOnNonDefaultConstruction(default);
        }

        [Test]
        public void LogOnFinalizeTest()
        {
            {
                new TestClass(true);
            }
        }
    }
}