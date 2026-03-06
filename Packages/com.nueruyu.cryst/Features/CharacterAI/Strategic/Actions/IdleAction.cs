using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System.Threading;
using ActorContext_ = Cryst.Features.CharacterAI.ActorContext<Cryst.Features.CharacterAI.Strategic.StrategicState>;

namespace Cryst.Features.CharacterAI.Strategic.Actions
{
    public class IdleAction : IAction<ActorContext_, StrategicState>
    {
        public bool CanExecute(StrategicState worldState)
        {
            return true;
        }

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(ActorContext_ context, CancellationToken cancellationToken)
        {
            return UniTask.WaitUntilCanceled(cancellationToken);
        }
    }
}
