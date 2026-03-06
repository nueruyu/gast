using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System.Threading;

namespace Cryst.Features.CharacterAI.Strategic.Actions
{
    public class IdleAction : IAction<ActorContext<StrategicState>, StrategicState>
    {
        public bool CanExecute(StrategicState worldState)
        {
            return true;
        }

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(ActorContext<StrategicState> context, CancellationToken cancellationToken)
        {
            return UniTask.WaitUntilCanceled(cancellationToken);
        }
    }
}
