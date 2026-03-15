using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic.Actions
{
    /// <summary>
    /// An action that sets the AI's operational mode in memory.
    /// </summary>
    public class SetAIModeAction : IAction<ActorContext<StrategicState>, StrategicState>
    {
        readonly AIMode mode;

        public SetAIModeAction(AIMode mode)
        {
            this.mode = mode;
        }

        public bool IsAvailable(StrategicState worldState) => true;

        public void Simulate(StrategicState worldState)
        {
            // This is a cognitive action that doesn't change the predictable world state for planning.
        }

        public UniTask ExecuteAsync(ActorContext<StrategicState> context, CancellationToken cancellationToken)
        {
            var memory = context.GetModule<HumanoidMemory>();
            if (memory.CurrentMode != mode)
            {
                memory.CurrentMode = mode;
            }
            return UniTask.CompletedTask;
        }

        public override string ToString()
        {
            return $"{nameof(SetAIModeAction)}: {mode}";
        }
    }
}
