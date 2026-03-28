using System;
using System.Collections.Generic;
using System.Linq;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using Gast.Core.Events;
using Gast.Domain.AI;
using Gast.Domain.Economy;
using R3;

namespace Cryst.Features.CharacterAI
{
    public class ObjectiveManager
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly ReactiveProperty<IReadOnlyList<IAIObjective>> currentObjectives = new(Array.Empty<IAIObjective>());

        public ObjectiveManager(IDomainEventSubscriber eventSubscriber)
        {
            this.eventSubscriber = eventSubscriber;
        }

        public IReadOnlyList<IAIObjective> CurrentObjectives => currentObjectives.Value;
        public ReadOnlyReactiveProperty<IReadOnlyList<IAIObjective>> CurrentObjectivesObservable => currentObjectives;

        public void UpdateObjectives(IEnumerable<IAIObjective> objectives)
        {
            currentObjectives.Value = objectives.ToList();
        }

        public IDisposable BindCharacter(CharacterId characterId)
        {
            var subscriptions = new CompositeDisposable();
            eventSubscriber.Subscribe<CharacterDefeatedEvent>(OnCharacterDefeated)
                .AddTo(subscriptions);
            eventSubscriber.Subscribe<ItemAcquiredEvent>(OnItemAcquired)
                .AddTo(subscriptions);

            return subscriptions;

            void OnCharacterDefeated(CharacterDefeatedEvent e)
            {
                foreach (var goal in currentObjectives.Value.OfType<DefeatCharacterObjective>())
                    if (e.AttackerId == characterId &&
                        goal.TargetTypeId == e.DefeatedCharacter.As<BaseCharacter>().TypeId)
                        goal.IncrementCount();
            }

            void OnItemAcquired(ItemAcquiredEvent e)
            {
                foreach (var goal in currentObjectives.Value.OfType<AcquireItemObjective>())
                    if (e.AcquirerId == characterId &&
                        goal.TargetItemId == e.AcquiredItemId)
                        goal.AddQuantity(e.AcquiredQuantity);
            }
        }
    }
}
