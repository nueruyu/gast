using System;
using Gast.Lib.AI.Debugging;
using UnityEngine;
using VContainer.Unity;

namespace Gast.Unity.Infrastructure.Services
{
    public class AIDebugInitializer : IStartable, IDisposable
    {
        readonly IAIDebugger debugger;

        public AIDebugInitializer(IAIDebugger debugger)
        {
            this.debugger = debugger;
            Debug.Log($"[AIDebugInitializer] Constructed with debugger: {debugger}");
        }

        public void Start()
        {
            AIDebuggerBridge.SetInstance(debugger);
            Debug.Log($"[AIDebugInitializer] Start called, Bridge initialized: {AIDebuggerBridge.IsInitialized.CurrentValue}");
        }

        public void Dispose()
        {
            AIDebuggerBridge.ClearInstance();
            Debug.Log("[AIDebugInitializer] Disposed");
        }
    }
}