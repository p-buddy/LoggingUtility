using UnityEngine;

using static pbuddy.LoggingUtility.RuntimeScripts.ContextProvider;

namespace pbuddy.LoggingUtility.EditModeTests.TestObjects
{
    public struct TestStructThatLogsOnNonDefaultConstruction
    {
        public TestStructThatLogsOnNonDefaultConstruction(object dummy)
        {
            Debug.Log(Context().WithMessage($"Logging from {nameof(TestClass.NestedTestStruct)}'s constructor"));
        }
        
        public void TestFunction()
        {
            Debug.Log($"{Context()} Logging from Test function");
        }
    }
}