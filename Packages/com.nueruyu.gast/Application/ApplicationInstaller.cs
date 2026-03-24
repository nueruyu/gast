using Gast.Application.AIPlanning;
using Gast.Application.Characters;
using Gast.Application.Economy;
using Gast.Application.Interactions;
using Gast.Application.Items;
using Gast.Core.DI;

namespace Gast.Application
{
    public class ApplicationInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Characters
            builder.Register<SpawnCharacterUseCase>();
            builder.Register<CreatePlayerUseCase>().AsImplementedInterfaces();
            builder.Register<CreateNpcUseCase>().AsImplementedInterfaces();

            // Economy
            builder.Register<BuyItemUseCase>().AsImplementedInterfaces();
            builder.Register<PickUpItemUseCase>().AsImplementedInterfaces();
            builder.Register<UseItemUseCase>().AsImplementedInterfaces();

            // Items
            builder.Register<SpawnItemUseCase>().AsImplementedInterfaces();

            // Interactions
            builder.Register<InteractUseCase>().AsImplementedInterfaces();

            // AI
            builder.Register<CommandAIUseCase>().AsImplementedInterfaces();
        }
    }
}