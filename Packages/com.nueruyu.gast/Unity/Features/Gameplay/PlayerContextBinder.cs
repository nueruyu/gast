using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Cameras;
using Gast.Domain.Characters;
using Gast.Domain.Players;
using UnityEngine;

namespace Gast.Unity.Features.Gameplay
{
    public class PlayerContextBinder : ILifecycleTask
    {
        readonly IPlayerManager playerManager;
        readonly ICameraService cameraService;

        public PlayerContextBinder(
            IPlayerManager playerManager,
            ICameraService cameraService)
        {
            this.playerManager = playerManager;
            this.cameraService = cameraService;
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
                Debug.Log("PlayerContextBinder: Player unpossessed, cleared camera target");
            }
            else
            {
                cameraService.SetFollowTarget(character.Id);
                Debug.Log($"PlayerContextBinder: Updated camera target to {character.Id}");
            }
        }
    }
}
