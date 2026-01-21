using System.Collections.Generic;

namespace Gast.Lib.AI.Debugging
{
    public static class AIDebuggerBridge
    {
        static IAIDebugger instance;

        public static bool IsInitialized => instance != null;
        public static IAIDebugger Debugger => instance;

        public static void SetInstance(IAIDebugger debugger)
        {
            instance = debugger;
        }

        public static void ClearInstance()
        {
            instance = null;
        }

        public static IReadOnlyDictionary<ContextKey, AIDebugInfo> GetAllDebugInfo()
        {
            if (instance == null)
                throw new System.InvalidOperationException("AIDebuggerBridge instance is not initialized.");

            return instance.GetAllDebugInfo();
        }
    }
}