using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Application.Economy;
using Gast.Core.Commands;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using Gast.Domain.Players;
using R3;

namespace Gast.Unity.Features.Players
{
    public class PlayerItemUsageService : ILifecycleTask
    {
        readonly IInputProvider inputProvider;
        readonly IPlayerManager playerManager;
        readonly ICommandDispatcher commandDispatcher;

        public PlayerItemUsageService(
            IInputProvider inputProvider,
            IPlayerManager playerManager,
            ICommandDispatcher commandDispatcher)
        {
            this.inputProvider = inputProvider;
            this.playerManager = playerManager;
            this.commandDispatcher = commandDispatcher;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            inputProvider.UseItemSlot
                .Subscribe(slotIndex => UseItem(slotIndex))
                .AddTo(cancellationToken);

            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void UseItem(int slotIndex)
        {
            var character = playerManager.CurrentCharacter.Value;
            if (character == null) return;

            commandDispatcher.Dispatch<UseItemCommand, bool>(new(character.Id, slotIndex));
        }
    }
}
