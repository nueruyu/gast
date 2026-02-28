using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cryst.Domain.Combat;
using Cryst.Domain.Characters;
using Cryst.Features.Characters.EventHandlers;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using R3;

namespace Cryst.Infrastructure
{
    public class EventBindingRunner : ILifecycleTask
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly CharacterDecompositionHandler characterDecompositionHandler;
        readonly HitEffectHandler hitEffectHandler;
        readonly HitFeedbackHandler hitFeedbackHandler;

        public EventBindingRunner(
            IDomainEventSubscriber eventSubscriber,
            CharacterDecompositionHandler characterDecompositionHandler,
            HitEffectHandler hitEffectHandler,
            HitFeedbackHandler hitFeedbackHandler)
        {
            this.eventSubscriber = eventSubscriber;
            this.characterDecompositionHandler = characterDecompositionHandler;
            this.hitEffectHandler = hitEffectHandler;
            this.hitFeedbackHandler = hitFeedbackHandler;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            eventSubscriber.Subscribe<CharacterDefeatedEvent>(characterDecompositionHandler.Handle)
                .AddTo(cancellationToken);
            eventSubscriber.Subscribe<CharacterHitEvent<AttackInfo>>(hitEffectHandler.Handle)
                .AddTo(cancellationToken);
            eventSubscriber.Subscribe<CharacterHitEvent<AttackInfo>>(hitFeedbackHandler.Handle)
                .AddTo(cancellationToken);

            await UniTask.WaitUntilCanceled(cancellationToken);
        }
    }
}
