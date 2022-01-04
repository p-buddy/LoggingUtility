using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine.Assertions;

namespace pbuddy.LoggingUtility.RuntimeScripts
{
    public static class ContextProvider
    {
        private static StrategyType Strategy;
        private static Dictionary<StrategyType, ContextReadoutMechanism> ContextReadoutByStrategy;
        static ContextProvider()
        {
            Strategy = StrategyType.SlowWithMethodNameAndEnclosingTypeName;
            ContextReadoutByStrategy = new Dictionary<StrategyType, ContextReadoutMechanism>(StrategyCount);
            
            ContextReadoutByStrategy[StrategyType.SlowWithMethodNameAndEnclosingTypeName] = new ContextReadoutMechanism
            {
                ConstructorContext = typeName => $"[{typeName} Constructor]",
                FinalizerContext = typeName => $"[{typeName} Finalizer]",
                MethodContext = (typeName, methodName) => $"[Type: {typeName}, Method: {methodName}]"
            };
            
            ContextReadoutByStrategy[StrategyType.FastButWithFileNameInsteadOfTypeName] = new ContextReadoutMechanism
            {
                ConstructorContext = fileName => $"[Constructor in File '{fileName}']",
                FinalizerContext = fileName => $"[Finalizer in File '{fileName}']",
                MethodContext = (fileName, methodName) => $"[Method '{methodName}' in File '{fileName}']"
            };
        }
        
        #region Internal Types
        delegate string ConstructorContext(string typeOrFileName);
        delegate string FinalizerContext(string typeOrFileName);
        delegate string MethodContext(string typeOrFileName, string methodName);
        enum StrategyType
        {
            SlowWithMethodNameAndEnclosingTypeName,
            FastButWithFileNameInsteadOfTypeName,
        }
        private static readonly int StrategyCount = Enum.GetValues(typeof(StrategyType)).Length;

        private struct ContextReadoutMechanism
        {
            public ConstructorContext ConstructorContext { get; set; }
            public FinalizerContext FinalizerContext { get; set; }
            public MethodContext MethodContext { get; set; }

            public string GetContextReadout(ContextInformation info)
            {
                if (info.IsConstructor)
                {
                    return ConstructorContext.Invoke(info.TypeOrFileName); 
                }

                if (info.IsFinalizer)
                {
                    return FinalizerContext.Invoke(info.TypeOrFileName); 
                }
            
                return MethodContext.Invoke(info.TypeOrFileName, info.MethodName);
            }
        }

        private struct ContextInformation
        {
            public bool IsConstructor { get; set; }
            public bool IsFinalizer { get; set; }
            public string TypeOrFileName { get; set; }
            public string MethodName { get; set; }
        }
        #endregion Internal Types
        
        private const string ObjectConstructor = ".ctor";
        private const string StaticConstructor = ".cctor";
        private const string Finalizer = "Finalize";

        public static string Context([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            switch (Strategy)
            {
                case StrategyType.SlowWithMethodNameAndEnclosingTypeName:
                    StackTrace stackTrace = new StackTrace();
                    StackFrame frame = stackTrace.GetFrames()?[1];
                    Assert.IsNotNull(frame);
                    MethodBase method = frame.GetMethod();
                    Assert.IsNotNull(method.DeclaringType);
                    return ContextReadoutByStrategy[Strategy].GetContextReadout(new ContextInformation
                    {
                        TypeOrFileName = method.DeclaringType.GetReadableTypeName(),
                        MethodName = method.Name,
                        IsConstructor = method.IsConstructor,
                        IsFinalizer = !method.IsConstructor && method.Name == Finalizer,
                    });
                case StrategyType.FastButWithFileNameInsteadOfTypeName:
                    return ContextReadoutByStrategy[Strategy].GetContextReadout(new ContextInformation
                    {
                        TypeOrFileName = Path.GetFileName(sourceFilePath),
                        MethodName = memberName,
                        IsConstructor = memberName == ObjectConstructor || memberName == StaticConstructor,
                        IsFinalizer = memberName == Finalizer,
                    });
                default:
                    return "Unsupported Context Provider Strategy";
            }
        }

        public static string WithMessage(this string context, string message)
        {
            return $"{context} {message}";
        }

        private static string GetReadableTypeName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.FullName;
            }
            
            StringBuilder sb = new StringBuilder();
            
            string AppendGenericTypeArgument(string aggregate, Type genericTypeArgument)
            {
                return aggregate + (aggregate == "<" ? "" : ",") + GetReadableTypeName(genericTypeArgument);
            }

            sb.Append(type.FullName.Substring(0, type.FullName.LastIndexOf("`", StringComparison.Ordinal)));
            sb.Append(type.GetGenericArguments().Aggregate("<", AppendGenericTypeArgument));
            sb.Append(">");

            return sb.ToString();
        }
        
        #region Method(s) for testing
        private static StrategyType? StrategyToReset;
        public static string GetStrategyName => Strategy.ToString();
        public static bool NextStrategy()
        {
            if (StrategyToReset == Strategy)
            {
                StrategyToReset = null;
                return false;
            }
            
            StrategyToReset ??= Strategy;

            StrategyType[] strategyTypes = Enum.GetValues(typeof(StrategyType)) as StrategyType[];
            int index = Array.IndexOf(strategyTypes, Strategy);
            int nextIndex = index == strategyTypes.Length - 1 ? 0 : index + 1;
            Strategy = strategyTypes[nextIndex];
            return true;
        }
        #endregion
    }
}