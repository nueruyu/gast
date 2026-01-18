using Gast.Lib.AI.Debugging;
using VContainer.Unity;
using System;

namespace Gast.Features.Debugging
{
    public class AIDebugInitializer : IStartable, IDisposable
    {
        readonly IAIDebugger debugger;

        public AIDebugInitializer(IAIDebugger debugger)
        {
            this.debugger = debugger;
        }

        public void Start()
        {
            AIDebuggerBridge.SetInstance(debugger);
        }

        public void Dispose()
        {
            AIDebuggerBridge.ClearInstance();
        }
    }
}
