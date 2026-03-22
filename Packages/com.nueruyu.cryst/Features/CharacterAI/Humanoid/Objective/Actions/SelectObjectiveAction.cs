using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Objective.Actions
{
    public abstract class SelectObjectiveAction<TObjective> : IAction<ActorContext<ObjectiveState>, ObjectiveState>
        where TObjective : IAIObjective
    {
        protected readonly TObjective objective;

        protected SelectObjectiveAction(TObjective objective)
        {
            this.objective = objective;
        }

        public bool IsAvailable(ObjectiveState worldState) => true;

        public void Simulate(ObjectiveState worldState) { }

        public abstract UniTask ExecuteAsync(ActorContext<ObjectiveState> context, CancellationToken cancellationToken);

        public override string ToString()
        {
            return $"{GetType().Name}({objective})";
        }
    }
}
