using ObservableCollections;

namespace Gast.Lib.AI.Debugging
{
    public static class AIDebuggerBridge
    {
        public static bool IsInitialized => Debugger != null;
        public static IAIDebugger Debugger { get; private set; }

        public static IReadOnlyObservableDictionary<ContextKey, AIDebugInfo> AllDebugInfo => Debugger?.AllDebugInfo;

        public static void SetInstance(IAIDebugger debugger)
        {
            Debugger = debugger;
        }

        public static void ClearInstance()
        {
            Debugger = null;
        }
    }
}