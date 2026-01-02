using Cysharp.Threading.Tasks;
using DescrioGames.Lib.AI;
using System;

namespace DescrioGames.Features.Npcs.Actions
{
    [Serializable]
    public class IdleAction : PrimitiveTask<CombatWorldState>
    {
        public IdleAction() : base("Idle")
        {
        }

        protected override bool CheckCondition(CombatWorldState state)
        {
            return !state.HasTarget;
        }

        protected override void ApplyEffect(ref CombatWorldState state, ISimulationContext context)
        {
            // No state change
        }

        protected override async UniTask ExecuteAsync(Context<CombatWorldState> ctx)
        {
            ctx.Character.NavigationProvider.Stop();
            await UniTask.Delay(1000, cancellationToken: ctx.Token);
        }
    }
}