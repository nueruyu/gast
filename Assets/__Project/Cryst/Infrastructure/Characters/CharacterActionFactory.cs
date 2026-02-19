using System;
using Cryst.Modules.CharacterActions;
using Cryst.Modules.CharacterActions.Default;
using Cysharp.Threading.Tasks;
using Gast.Features.Characters;
using VContainer;

namespace Cryst.Infrastructure.Characters
{
    public class CharacterActionFactory : ICharacterActionFactory
    {
        readonly IObjectResolver resolver;

        public CharacterActionFactory(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }

        public ICharacterAction Create(CharacterActionSettings settings, CharacterContext context)
        {
            var scope = resolver.CreateScope(builder =>
            {
                builder.RegisterInstance(context);
                builder.RegisterInstance(settings, settings.GetType());

                var animator = context.Resolve<CharacterAnimator>();
                if (animator != null)
                    builder.RegisterInstance(animator);

                var movement = context.Resolve<CharacterMovement>();
                if (movement != null)
                    builder.RegisterInstance(movement);

                var stateStore = context.Resolve<CharacterActionStateStore>();
                if (stateStore != null)
                    builder.RegisterInstance(stateStore);
            });

            scope.AddTo(context.CancellationToken);

            return settings switch
            {
                AttackActionSettings => scope.Resolve<AttackAction>(),
                DashActionSettings => scope.Resolve<DashAction>(),
                DefaultActionSettings => scope.Resolve<DefaultAction>(),
                DieActionSettings => scope.Resolve<DieAction>(),
                GuardActionSettings => scope.Resolve<GuardAction>(),
                HitActionSettings => scope.Resolve<HitAction>(),
                JumpActionSettings => scope.Resolve<JumpAction>(),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(settings),
                    $"Unknown action settings type: {settings.GetType().Name}")
            };
        }
    }
}