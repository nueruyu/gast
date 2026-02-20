using Cryst.Modules.CharacterActions;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Features.Characters;
using Gast.Features.Combat;
using Gast.Features.Interactions;
using Gast.Features.Navigations;
using Gast.Features.Sensors;
using Gast.Infrastructure.Characters;
using Gast.Shared.UnityExtensions;

namespace Cryst.Infrastructure.Characters
{
    public class CharacterContextInitializer : ICharacterContextInitializer
    {
        readonly IHitAreaFactory hitAreaFactory;

        public CharacterContextInitializer(IHitAreaFactory hitAreaFactory)
        {
            this.hitAreaFactory = hitAreaFactory;
        }

        public void Initialize(CharacterContext context)
        {
            // Core Components
            var animator = context.Body.GetComponentInChildren<CharacterAnimator>();
            var audio = context.Body.RequireComponentInChildren<CharacterAudio>();
            var stateStore = new CharacterActionStateStore();
            var movement = new CharacterMovement(
                animator,
                context.Body,
                context.TypeDefinition);

            context.Register(animator);
            context.Register(audio);
            context.Register(stateStore);
            context.Register(movement);
            context.Register(hitAreaFactory);

            // Sensors & Navigation
            var visionSensor = context.Body.RequireComponentInChildren<ConeVisionSensor>();
            var interactionSensor = context.Body.RequireComponentInChildren<InteractionSensor>();
            var navigationProvider = context.Body.RequireComponentInChildren<NavMeshNavigator>();

            var visionSettings = context.Resolve<ConeVisionSensorSettings>();
            if (visionSettings != null)
            {
                visionSensor.ViewAngle = visionSettings.ViewAngle;
                visionSensor.ViewRadius = visionSettings.ViewRadius;
                visionSensor.EyeOffset = visionSettings.EyeOffset;
            }

            var navSettings = context.Resolve<NavMeshNavigatorSettings>();
            if (navSettings != null)
            {
                navigationProvider.StoppingDistance = navSettings.StoppingDistance;
            }

            context.Register<IVisionSensor>(visionSensor);
            context.Register<IInteractionSensor>(interactionSensor);
            context.Register<INavigationProvider>(navigationProvider);

            // Handlers
            var footstepSettings = context.Resolve<CharacterFootstepSettings>();
            if (footstepSettings != null)
            {
                new CharacterFootstepHandler(animator, audio, footstepSettings);
            }
        }
    }
}