using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Players;
using System;
using System.Threading;

namespace Gast.Features.Players
{
    public class PlayerBrain : ICharacterBrain
    {
        readonly IPlayerCharacterController playerCharacterController;
        CancellationTokenSource cts;

        public PlayerBrain(
            IPlayerCharacterController playerCharacterController)
        {
            this.playerCharacterController = playerCharacterController;
        }

        public void OnAttached(ICharacter character)
        {
            if (character is null)
                throw new ArgumentNullException(nameof(character));

            if (cts != null)
                throw new InvalidOperationException();

            cts = new();
            RunAsync(character, cts.Token).Forget();
        }

        public void OnDetached()
        {
            cts?.Cancel();
            cts = null;
        }

        async UniTask RunAsync(ICharacter actor, CancellationToken cancellationToken)
        {
            while (true)
            {
                await UniTask.NextFrame(cancellationToken);
                playerCharacterController.HandleInput(actor);
            }
        }
    }
}