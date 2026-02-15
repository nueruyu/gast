using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Modules.CharacterAI.Strategic.Actions
{
    public class IdleAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        public bool CanExecute(StrategicState worldState)
        {
            return true;
        }

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicState> ctx)
        {
            return UniTask.WaitUntilCanceled(ctx.CancellationToken);
        }
    }
}