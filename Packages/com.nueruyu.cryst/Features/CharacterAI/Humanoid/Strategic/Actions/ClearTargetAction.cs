using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic.Actions
{
    public class ClearTargetAction : IAction<ActorContext<StrategicState>, StrategicState>
    {
        public bool IsAvailable(StrategicState worldState)
        {
            return true;
        }

        public void Simulate(StrategicState worldState)
        {
            // This is a cognitive action that clears memory,
            // it does not change the predictable future state of the world for planning.
        }

        public UniTask ExecuteAsync(ActorContext<StrategicState> context, CancellationToken cancellationToken)
        {
            var memory = context.GetModule<HumanoidMemory>();
            memory.CombatTarget = null;
            memory.CurrentObjective = null;
            memory.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}