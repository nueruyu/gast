using System;
using System.Collections.Generic;
using System.Threading;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using Gast.Lib.AI.Debugging;

namespace Cryst.Features.CharacterAI
{
    public abstract class BaseAIBrain : ICharacterAIBrain
    {
        readonly IContextRegistry contextRegistry;
        readonly AIBrainServices services;
        BaseCharacter actor;
        ICharacter character;
        CancellationTokenSource characterCts;

        DomainRunner domainRunner;
        AIMemory memory;

        protected BaseAIBrain(
            IContextRegistry contextRegistry,
            AIBrainServices services)
        {
            this.contextRegistry = contextRegistry;
            this.services = services;
        }

        public IReadOnlyList<IAIObjective> CurrentObjectives => services.ObjectiveManager.CurrentObjectives;

        public void OnAttached(ICharacter character)
        {
            this.character = character;
            actor = character.As<BaseCharacter>();
            memory = new AIMemory();
            characterCts = CancellationTokenSource.CreateLinkedTokenSource(character.CancellationToken);

            domainRunner = new DomainRunner();
            RegisterDomains(new DomainRegistrar(this, domainRunner));

            domainRunner.RunAsync(characterCts.Token).Forget();

            services.ObjectiveManager.BindCharacter(actor.Id)
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
            services.ObjectiveManager.UpdateObjectives(objectives);
        }

        protected abstract void RegisterDomains(IDomainRegistrar registrar);

        class DomainRegistrar : IDomainRegistrar
        {
            readonly BaseAIBrain brain;
            readonly Dictionary<string, ContextKey> contextKeys = new();
            readonly DomainRunner domainRunner;

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

            public void Register<TWorldState>(
                string domainName,
                AIRunner<ActorContext<TWorldState>, TWorldState> runner,
                TWorldState worldState,
                Action<ActorContext<TWorldState>> worldStateUpdater)
                where TWorldState : class, IWorldState<TWorldState>
            {
                var contextKey = new ContextKey(brain.actor.Id, domainName);
                contextKeys[contextKey.DomainName] = contextKey;
                brain.contextRegistry.Register(contextKey, worldState);

                var actorContext = new ActorContext<TWorldState>(
                    brain.services,
                    brain.actor,
                    brain.character,
                    brain.memory,
                    worldState,
                    worldStateUpdater);

                var aiContext = new AIContext<ActorContext<TWorldState>>(contextKey, actorContext);
                var process = new DomainProcess<ActorContext<TWorldState>, TWorldState>(runner, aiContext);
                domainRunner.Register(process);
            }
        }
    }
}