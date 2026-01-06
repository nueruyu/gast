using Gast.Api.Economy;
using Gast.Application.UseCases.Economy;

namespace Gast.Application.CommandHandlers
{
    public class EconomyCommandHandler :
        ICommandHandler<BuyItemCommand, bool>
    {
        readonly BuyItemUseCase buyItemUseCase;

        public EconomyCommandHandler(BuyItemUseCase buyItemUseCase)
        {
            this.buyItemUseCase = buyItemUseCase;
        }

        public bool Execute(in BuyItemCommand command)
        {
            return buyItemUseCase.Execute(command.BuyerId, command.ItemId);
        }
    }
}