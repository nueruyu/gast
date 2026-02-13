using Gast.Core.Events;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Domain.Sensors;
using Gast.Domain.Stats;
using Gast.Features.Combat;
using System;
using System.Collections.Generic;

namespace Gast.Features.Characters
{
    public record CharacterContext(
        CharacterId Id,
        CharacterTypeId TypeId,
        ICharacterTypeDefinition TypeDefinition,
        Faction Faction,
        IStatSchema StatSchema,
        CharacterBody Body,
        CharacterAnimationReceiver AnimationReceiver,
        CharacterAudio Audio,
        IVisionSensor VisionSensor,
        IInteractionSensor InteractionSensor,
        INavigationProvider NavigationProvider,
        CombatFeedbackService FeedbackService,
        IDomainEventPublisher EventPublisher)
    {
        readonly Dictionary<Type, object> services = new();

        public void Register<T>(T service) where T : class
        {
            services[typeof(T)] = service;
        }

        public T Resolve<T>() where T : class
        {
            return services.TryGetValue(typeof(T), out var service) ? (T)service : null;
        }
    }
}
