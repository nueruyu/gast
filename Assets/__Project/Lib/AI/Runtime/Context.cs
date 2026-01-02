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
        public CancellationToken Token { get; }

        public TWorldState CurrentState => stateProvider != null ? stateProvider() : PlanState;

        public Context(
            TWorldState planState,
            Func<TWorldState> stateProvider,
            ICharacter character,
            CancellationToken token)
        {
            PlanState = planState;
            this.stateProvider = stateProvider;
            Character = character;
            Token = token;
        }

        public Context(TWorldState planState, ICharacter character, CancellationToken token)
            : this(planState, null, character, token) { }

        public Context<TWorldState> WithToken(CancellationToken newToken)
        {
            return new Context<TWorldState>(PlanState, stateProvider, Character, newToken);
        }
    }
}