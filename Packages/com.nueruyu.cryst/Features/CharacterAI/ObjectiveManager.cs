using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Domain.AI;
using Cryst.Domain.AI.Objectives;
using Gast.Core.Events;
using Gast.Domain.Characters;
using R3;
using Gast.Domain.Economy;
using Cryst.Domain.Characters;

namespace Cryst.Features.CharacterAI
{
    public class ObjectiveManager
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly List<IAIObjective> currentObjectives = new();

        public IReadOnlyList<IAIObjective> CurrentObjectives => currentObjectives;

        public ObjectiveManager(IDomainEventSubscriber eventSubscriber)
        {
            this.eventSubscriber = eventSubscriber;
        }

        public void UpdateObjectives(IEnumerable<IAIObjective> objectives)
        {
            currentObjectives.Clear();
            currentObjectives.AddRange(objectives);
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
                foreach (var goal in currentObjectives.OfType<DefeatCharacterObjective>())
                {
                    if (e.AttackerId == characterId &&
                        goal.TargetTypeId == e.DefeatedCharacter.As<BaseCharacter>().TypeId)
                    {
                        goal.IncrementCount();
                    }
                }
            }

            void OnItemAcquired(ItemAcquiredEvent e)
            {
                foreach (var goal in currentObjectives.OfType<AcquireItemObjective>())
                {
                    if (e.AcquirerId == characterId &&
                        goal.TargetItemId == e.AcquiredItemId)
                    {
                        goal.AddQuantity(e.AcquiredQuantity);
                    }
                }
            }
        }
    }
}