using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using Gast.Lib.AI.Debugging;
using Cryst.Domain.Characters;
using R3;

namespace Cryst.Features.CharacterAI
{
    public interface IDomainRegistrar
    {
        void Register<TActorContext, TWorldState>(
            ContextKey contextKey,
            AIRunner<TActorContext, TWorldState> runner,
            AIContext<TActorContext> context)
            where TWorldState : class, IWorldState<TWorldState>, new()
            where TActorContext : class, IActorContext<TWorldState>;
    }

    public abstract class BaseAIBrain : ICharacterAIBrain
    {
        readonly IContextRegistry contextRegistry;
        protected readonly AIBrainServices services;
        readonly ObjectiveManager objectiveManager;

        DomainRunner domainRunner;
        CancellationTokenSource characterCts;
        protected BaseCharacter actor;
        protected ICharacter character;
        protected AIMemory memory;

        public IReadOnlyList<IAIObjective> CurrentObjectives => objectiveManager.CurrentObjectives;

        protected BaseAIBrain(
            IContextRegistry contextRegistry,
            AIBrainServices services,
            ObjectiveManager objectiveManager)
        {
            this.contextRegistry = contextRegistry;
            this.services = services;
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

            public void Register<TActorContext, TWorldState>(
                ContextKey contextKey,
                AIRunner<TActorContext, TWorldState> runner,
                AIContext<TActorContext> context)
                where TWorldState : class, IWorldState<TWorldState>, new()
                where TActorContext : class, IActorContext<TWorldState>
            {
                contextKeys[contextKey.DomainName] = contextKey;
                brain.contextRegistry.Register(contextKey, context.ActorContext.WorldState);

                var process = new DomainProcess<TActorContext, TWorldState>(runner, context);
                domainRunner.Register(process);
            }
        }
    }
}
