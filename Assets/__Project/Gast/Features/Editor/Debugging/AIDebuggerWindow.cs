using Gast.Lib.AI.Editor.Debugging;
using UnityEditor;

namespace Gast.Features.Editor.Debugging
{
    public class AIDebuggerWindow : BaseAIDebuggerWindow
    {
        [MenuItem("Gast/Tools/AI Debugger")]
        public static void ShowWindow()
        {
            GetWindow<AIDebuggerWindow>("AI Debugger");
        }
    }
}
