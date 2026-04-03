using System;
using ObservableCollections;
using R3;

namespace Gast.Lib.AI.Debugging
{
    public static class AIDebuggerBridge
    {
        static readonly ReactiveProperty<bool> isInitialized = new(false);

        public static ReadOnlyReactiveProperty<bool> IsInitialized => isInitialized;
        public static IAIDebugger Debugger { get; private set; }

        public static IReadOnlyObservableDictionary<ContextKey, AIDebugInfo> AllDebugInfo =>
            Debugger?.AllDebugInfo ?? throw new InvalidOperationException();

        public static void SetInstance(IAIDebugger debugger)
        {
            Debugger = debugger;
            isInitialized.Value = true;
        }

        public static void ClearInstance()
        {
            Debugger = null;
            isInitialized.Value = false;
        }
    }
}