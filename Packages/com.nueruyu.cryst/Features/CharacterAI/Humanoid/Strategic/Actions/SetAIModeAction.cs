using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic.Actions
{
    public class SetAIModeAction : IAction<ActorContext<StrategicState>, StrategicState>
    {
        readonly AIMode mode;

        public SetAIModeAction(AIMode mode)
        {
            this.mode = mode;
        }

        public bool IsAvailable(StrategicState worldState)
        {
            return true;
        }

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(ActorContext<StrategicState> context, CancellationToken cancellationToken)
        {
            var memory = context.GetModule<AIModeMemory>();
            memory.SetMode(mode);
            return UniTask.CompletedTask;
        }

        public override string ToString()
        {
            return $"{nameof(SetAIModeAction)}: {mode}";
        }
    }
}
