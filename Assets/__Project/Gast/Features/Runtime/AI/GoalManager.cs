using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Api.AI;
using Gast.Api.AI.Goals;
using Gast.Core.Events;
using Gast.Domain.Characters;
using Gast.Domain.Events;
using R3;

namespace Gast.Features.AI
{
    public class GoalManager
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly List<IGoal> currentGoals = new();

        public IReadOnlyList<IGoal> CurrentGoals => currentGoals;

        public GoalManager(IDomainEventSubscriber eventSubscriber)
        {
            this.eventSubscriber = eventSubscriber;
        }

        public void UpdateGoals(IEnumerable<IGoal> goals)
        {
            currentGoals.Clear();
            currentGoals.AddRange(goals);
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
                foreach (var goal in currentGoals.OfType<DefeatCharacterGoal>())
                {
                    if (e.AttackerId == characterId &&
                        goal.TargetTypeId == e.DefeatedCharacter.TypeId)
                    {
                        goal.IncrementCount();
                    }
                }
            }

            void OnItemAcquired(ItemAcquiredEvent e)
            {
                foreach (var goal in currentGoals.OfType<AcquireItemGoal>())
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