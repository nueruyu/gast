using System;
using System.Collections.Generic;
using System.Threading;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI
{
    public abstract class AIBrainBase : ICharacterAIBrain
    {
        readonly IContextRegistry contextRegistry;
        readonly AIBrainServices services;

        protected ObjectiveManager ObjectiveManager { get; }
        protected BaseCharacter Actor => actor;
        BaseCharacter actor;
        ICharacter character;
        CancellationTokenSource characterCts;

        DomainRunner domainRunner;

        protected AIBrainBase(
            IContextRegistry contextRegistry,
            AIBrainServices services,
            ObjectiveManager objectiveManager)
        {
            this.contextRegistry = contextRegistry;
            this.services = services;
            ObjectiveManager = objectiveManager;
        }

        public IReadOnlyList<IAIObjective> CurrentObjectives => ObjectiveManager.CurrentObjectives;

        public void OnAttached(ICharacter character)
        {
            this.character = character;
            actor = character.As<BaseCharacter>();
            characterCts = CancellationTokenSource.CreateLinkedTokenSource(character.CancellationToken);

            OnBrainInitialize();

            domainRunner = new DomainRunner();
            RegisterDomains(new DomainRegistrar(this, domainRunner));

            domainRunner.RunAsync(characterCts.Token).Forget();

            ObjectiveManager.BindCharacter(actor.Id)
                .AddTo(characterCts.Token);
        }

        public void OnDetached()
        {
            OnBrainCleanup();

            characterCts?.Cancel();
            characterCts?.Dispose();
            characterCts = null;

            domainRunner = null;
            actor = null;
            character = null;
        }

        public void SetObjectives(IEnumerable<IAIObjective> objectives)
        {
            ObjectiveManager.UpdateObjectives(objectives);
        }

        protected abstract void RegisterDomains(IDomainRegistrar registrar);

        protected abstract void RegisterModules<TWorldState>(ActorContext<TWorldState> context)
            where TWorldState : class, IWorldState<TWorldState>;

        protected virtual void OnBrainInitialize()
        {
        }

        protected virtual void OnBrainCleanup()
        {
        }

        class DomainRegistrar : IDomainRegistrar
        {
            readonly AIBrainBase brain;
            readonly Dictionary<string, ContextKey> contextKeys = new();
            readonly DomainRunner domainRunner;

            public DomainRegistrar(AIBrainBase brain, DomainRunner domainRunner)
            {
                this.brain = brain;
                this.domainRunner = domainRunner;

                brain.characterCts.Token.Register(() =>
                {
                    foreach (var key in contextKeys.Values)
                        brain.contextRegistry.Unregister(key);
                });
            }

            public void RegisterPreUpdate(Action action)
            {
                domainRunner.RegisterPreUpdate(action);
            }

            public void Register<TWorldState>(
                string domainName,
                IDomainProvider<ActorContext<TWorldState>, TWorldState> domainProvider,
                TWorldState worldState,
                Action<ActorContext<TWorldState>> worldStateUpdater)
                where TWorldState : class, IWorldState<TWorldState>
            {
                var contextKey = new ContextKey(brain.actor.Id, domainName);
                contextKeys[contextKey.DomainName] = contextKey;
                brain.contextRegistry.Register(contextKey, worldState, brain.actor.DisplayName);

                var actorContext = new ActorContext<TWorldState>(
                    brain.services,
                    brain.ObjectiveManager,
                    brain.actor,
                    brain.character,
                    worldState,
                    worldStateUpdater);

                brain.RegisterModules(actorContext);

                var process =
                    new DomainProcess<ActorContext<TWorldState>, TWorldState>(domainProvider, actorContext, contextKey);
                domainRunner.Register(process);
            }
        }
    }
}