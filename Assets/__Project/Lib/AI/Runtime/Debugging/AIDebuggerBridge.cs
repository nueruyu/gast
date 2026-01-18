namespace Gast.Lib.AI.Debugging
{
    public static class AIDebuggerBridge
    {
        public static IAIDebugger Instance { get; private set; }
        public static bool IsInitialized => Instance != null;

        public static void SetInstance(IAIDebugger instance)
        {
            Instance = instance;
        }

        public static void ClearInstance()
        {
            Instance = null;
        }
    }
}
