using Cysharp.Threading.Tasks;
using Gast.Core.Observables;
using Gast.Core.Tasks;
using Gast.Domain.Cameras;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Domain.Players;
using Gast.Features.Players;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Gast.Features.Gameplay
{
    /// <summary>
    /// Binds player character changes to dependent systems (camera, interactions).
    /// Automatically updates camera follow target and interaction detector origin
    /// when the player possesses a new character.
    /// </summary>
    public class PlayerContextBinder : ILifecycleTask
    {
        readonly IPlayerManager playerManager;
        readonly ICameraService cameraService;
        readonly IInteractionSystem interactionSystem;

        public PlayerContextBinder(
            IPlayerManager playerManager,
            ICameraService cameraService,
            IInteractionSystem interactionSystem)
        {
            this.playerManager = playerManager;
            this.cameraService = cameraService;
            this.interactionSystem = interactionSystem;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            playerManager.CurrentCharacter
                .SubscribeWithCurrent(OnPlayerCharacterChanged)
                .AddTo(cancellationToken);

            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void OnPlayerCharacterChanged(ICharacter character)
        {
            if (character == null)
            {
                cameraService.UnsetFollowTarget();
                interactionSystem.UnsetInteractor();

                Debug.Log("PlayerContextBinder: Player unpossessed, cleared camera and interaction targets");
            }
            else
            {
                cameraService.SetFollowTarget(character.Id);
                interactionSystem.SetInteractor(character.Id);

                Debug.Log($"PlayerContextBinder: Updated camera and interaction targets to {character.Id}");
            }
        }
    }
}