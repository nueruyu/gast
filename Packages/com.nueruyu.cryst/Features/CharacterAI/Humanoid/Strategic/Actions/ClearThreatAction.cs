using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic.Actions
{
    public class ClearThreatAction : IAction<ActorContext<StrategicState>, StrategicState>
    {
        public bool IsAvailable(StrategicState worldState) => true;

        public void Simulate(StrategicState worldState) { }

        public UniTask ExecuteAsync(ActorContext<StrategicState> context, CancellationToken cancellationToken)
        {
            context.GetModule<HumanoidMemory>().ClearThreat();
            return UniTask.CompletedTask;
        }
    }
}
