using System;
using System.Threading;
using Gast.Domain.Characters;

namespace Gast.Lib.AI
{
    public readonly struct Context<TWorldState> where TWorldState : struct
    {
        readonly Func<TWorldState> stateProvider;

        public TWorldState PlanState { get; }
        public ICharacter Character { get; }
        public CancellationToken CancellationToken { get; }

        public TWorldState CurrentState => stateProvider();

        public Context(
            TWorldState planState,
            Func<TWorldState> stateProvider,
            ICharacter character,
            CancellationToken cancellationToken)
        {
            PlanState = planState;
            this.stateProvider = stateProvider ?? throw new ArgumentNullException(nameof(stateProvider));
            Character = character;
            CancellationToken = cancellationToken;
        }

        public Context<TWorldState> WithCancellationToken(CancellationToken cancellationToken)
        {
            return new Context<TWorldState>(PlanState, stateProvider, Character, cancellationToken);
        }
    }
}