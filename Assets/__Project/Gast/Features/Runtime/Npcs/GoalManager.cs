using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Api.AI;
using Gast.Api.AI.Goals;
using Gast.Core.Events;
using Gast.Domain.Characters;
using Gast.Domain.Events;
using R3;

namespace Gast.Features.Npcs
{
    public class GoalManager : IDisposable
    {
        readonly CompositeDisposable subscriptions = new();
        readonly List<IGoal> currentGoals = new();
        CharacterId characterId;

        public IReadOnlyList<IGoal> CurrentGoals => currentGoals;

        public GoalManager(IDomainEventSubscriber eventSubscriber)
        {
            eventSubscriber.Subscribe<CharacterDefeatedEvent>(OnCharacterDefeated)
                .AddTo(subscriptions);
            eventSubscriber.Subscribe<ItemAcquiredEvent>(OnItemAcquired)
                .AddTo(subscriptions);
        }

        public void Update(CharacterId characterId, List<IGoal> goals)
        {
            this.characterId = characterId;

            currentGoals.Clear();
            currentGoals.AddRange(goals);
        }

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

        public void Dispose()
        {
            subscriptions.Dispose();
        }
    }
}