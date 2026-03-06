using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Core.Commands;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using Gast.Lib.AI.Debugging;
using Cryst.Domain.Characters;
using R3;

namespace Cryst.Features.CharacterAI
{
    public interface IDomainRegistrar
    {
        void Register<TWorldState, TContext>(
            string domainName,
            TWorldState worldState,
            AIRunner<TWorldState, TContext> runner,
            Action stateUpdater)
            where TWorldState : class, IWorldState<TWorldState>, new()
            where TContext : struct, IContext<TContext, TWorldState>;
    }

    public abstract class BaseAIBrain : ICharacterAIBrain
    {
        readonly IContextRegistry contextRegistry;
        readonly ICharacterRepository characterRepository;
        readonly IPickupRepository pickupRepository;
        readonly ICommandDispatcher commandDispatcher;
        readonly ObjectiveManager objectiveManager;

        DomainRunner domainRunner;
        CancellationTokenSource characterCts;
        protected BaseCharacter actor;
        protected ICharacter character;
        protected AIMemory memory;

        public IReadOnlyList<IAIObjective> CurrentObjectives => objectiveManager.CurrentObjectives;

        protected BaseAIBrain(
            IContextRegistry contextRegistry,
            ICharacterRepository characterRepository,
            IPickupRepository pickupRepository,
            ICommandDispatcher commandDispatcher,
            ObjectiveManager objectiveManager)
        {
            this.contextRegistry = contextRegistry;
            this.characterRepository = characterRepository;
            this.pickupRepository = pickupRepository;
            this.commandDispatcher = commandDispatcher;
            this.objectiveManager = objectiveManager;
        }

        public void OnAttached(ICharacter character)
        {
            this.character = character;
            actor = character.As<BaseCharacter>();
            memory = new AIMemory();
            characterCts = CancellationTokenSource.CreateLinkedTokenSource(character.CancellationToken);

            domainRunner = new DomainRunner();
            RegisterDomains(new DomainRegistrar(this, domainRunner));

            domainRunner.RunAsync(characterCts.Token).Forget();

            objectiveManager.BindCharacter(actor.Id)
                .AddTo(characterCts.Token);
        }

        public void OnDetached()
        {
            characterCts?.Cancel();
            characterCts?.Dispose();
            characterCts = null;

            domainRunner = null;
            actor = null;
            character = null;
            memory = null;
        }

        public void SetObjectives(IEnumerable<IAIObjective> objectives)
        {
            objectiveManager.UpdateObjectives(objectives);
        }

        protected abstract void RegisterDomains(IDomainRegistrar registrar);

        class DomainRegistrar : IDomainRegistrar
        {
            readonly BaseAIBrain brain;
            readonly DomainRunner domainRunner;
            readonly Dictionary<string, ContextKey> contextKeys = new();

            public DomainRegistrar(BaseAIBrain brain, DomainRunner domainRunner)
            {
                this.brain = brain;
                this.domainRunner = domainRunner;

                brain.characterCts.Token.Register(() =>
                {
                    foreach (var key in contextKeys.Values)
                    {
                        DebugLogger.ClearContext(key);
                        brain.contextRegistry.Unregister(key);
                    }
                });
            }

            public void Register<TWorldState, TContext>(
                string domainName,
                TWorldState worldState,
                AIRunner<TWorldState, TContext> runner,
                Action stateUpdater)
                where TWorldState : class, IWorldState<TWorldState>, new()
                where TContext : struct, IContext<TContext, TWorldState>
            {
                var contextKey = new ContextKey(brain.actor.Id, domainName);
                contextKeys[domainName] = contextKey;
                brain.contextRegistry.Register(contextKey, worldState);

                var context = (TContext)Activator.CreateInstance(typeof(TContext),
                    contextKey, brain.actor, brain.character, brain.memory,
                    brain.characterRepository, brain.pickupRepository, brain.commandDispatcher,
                    worldState, stateUpdater, CancellationToken.None);

                var process = new DomainProcess<TWorldState, TContext>(runner, context);
                domainRunner.Register(process);
            }
        }
    }
}
