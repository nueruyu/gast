using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Core.Events;
using Gast.Domain.Events;
using Gast.Domain.Npcs.Goals;
using R3;

namespace Gast.Features.Npcs
{
    public class GoalManager : IDisposable
    {
        readonly List<IGoal> currentGoals = new();
        readonly CompositeDisposable subscriptions = new();

        public IReadOnlyList<IGoal> CurrentGoals => currentGoals;

        public GoalManager(IDomainEventSubscriber eventSubscriber)
        {
            eventSubscriber.Subscribe<CharacterDefeatedEvent>(OnCharacterDefeated)
                .AddTo(subscriptions);
            eventSubscriber.Subscribe<ItemAcquiredEvent>(OnItemAcquired)
                .AddTo(subscriptions);
        }

        public void SetGoals(List<IGoal> newGoals)
        {
            currentGoals.Clear();
            currentGoals.AddRange(newGoals);
        }

        void OnCharacterDefeated(CharacterDefeatedEvent e)
        {
            foreach (var goal in currentGoals.OfType<DefeatCharacterGoal>())
            {
                if (goal.TargetTypeId == e.DefeatedCharacter.TypeId)
                {
                    goal.IncrementCount();
                }
            }
        }

        void OnItemAcquired(ItemAcquiredEvent e)
        {
            foreach (var goal in currentGoals.OfType<AcquireItemGoal>())
            {
                if (goal.TargetItemId == e.AcquiredItemId)
                {
                    goal.AddQuantity(e.AcquiredQuantity);
                }
            }
        }

        public void Dispose()
        {
            subscriptions.Dispose();
        }
    }
}