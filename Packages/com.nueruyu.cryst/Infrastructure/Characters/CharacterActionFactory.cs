using System;
using Cryst.Features.CharacterActions;
using Cryst.Features.CharacterActions.Actions.Attack;
using Cryst.Features.CharacterActions.Actions.Dash;
using Cryst.Features.CharacterActions.Actions.Default;
using Cryst.Features.CharacterActions.Actions.Die;
using Cryst.Features.CharacterActions.Actions.Guard;
using Cryst.Features.CharacterActions.Actions.Hit;
using Cryst.Features.CharacterActions.Actions.Jump;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
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
                builder.RegisterInstance(context.CharacaterId);

                foreach (var (type, module) in context.GetModules())
                {
                    builder.RegisterInstance(module, type);
                }

                var typeDefinition = context.Resolve<ICharacterTypeDefinition>();
                var movementSettings = typeDefinition.GetSettings<CharacterMovementSettings>();
                builder.RegisterInstance(movementSettings);

                builder.Register<CharacterMovement>(Lifetime.Transient);
                builder.Register<CharacterActionEffectDispatcher>(Lifetime.Transient);
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