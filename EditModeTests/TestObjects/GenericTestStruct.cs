using UnityEngine;

using static pbuddy.LoggingUtility.RuntimeScripts.ContextProvider;

namespace pbuddy.LoggingUtility.EditModeTests.TestObjects
{
    public struct GenericTestStruct<T>
    {
        public void TestFunction()
        {
            Debug.Log(Context().WithMessage($"Logging from {nameof(TestFunction)}"));
        }
    }
}