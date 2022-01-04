using UnityEngine;

using static pbuddy.LoggingUtility.RuntimeScripts.ContextProvider;

namespace pbuddy.LoggingUtility.EditModeTests.TestObjects
{
    public class TestClass
    {
        public struct NestedTestStruct
        {
            public void TestFunction()
            {
                Debug.Log($"{Context()} Logging from Test function");
            }
        }
        
        private readonly bool logOnFinalize;

        public TestClass()
        {
            logOnFinalize = false;
        }
        
        public TestClass(bool logOnFinalize)
        {
            this.logOnFinalize = logOnFinalize;
        }
            
        public void TestFunction()
        {
            Debug.Log(Context().WithMessage($"Logging from {nameof(TestFunction)}"));
        }
            
        ~TestClass()
        {
            if (logOnFinalize)
            {
                Debug.Log($"{Context()} Logging from {nameof(TestClass)}'s finalizer");
            }
        }
    }
}