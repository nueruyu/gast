using DescrioGames.Composition.Installers;
using DescrioGames.Core.Exceptions;
using DescrioGames.Features.Cameras;
using DescrioGames.Features.Economy;
using DescrioGames.Features.Gameplay;
using DescrioGames.Features.Gathering;
using DescrioGames.Features.Inputs;
using DescrioGames.Features.Interactions;
using DescrioGames.Features.SpawnSites;
using DescrioGames.Infrastructure.Settings;
using DescrioGames.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace DescrioGames.Composition
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Settings")]
        [SerializeField]
        GameInitializationSettings gameInitializationSettings;

        [SerializeField]
        InputSettings inputSettings;

        [SerializeField]
        CharacterDatabaseSettings characterDatabaseSettings;

        [SerializeField]
        InteractionSystemSettings interactionSettings;

        [SerializeField]
        PickupSystemSettings pickupSystemSettings;

        [SerializeField]
        ItemDatabaseSettings itemDatabase;

        [SerializeField]
        CharacterBrainFactorySettings characterBrainFactorySettings;

        [Header("Camera")]
        [SerializeField]
        CameraRegistry cameraRegistry;

        [Header("UI")]
        [SerializeField]
        UIDocument mainUIDocument;

        [SerializeField]
        UIAssetSettings uiAssetSettings;

        [Header("Economy")]
        [SerializeField]
        GatheringSpotRegistry gatheringSpotRegistry;

        [SerializeField]
        SpawnSiteRegistry spawnSiteRegistry;

        [SerializeField]
        ShopRegistry shopRegistry;

        [Header("Gameplay")]
        [SerializeField]
        PlayerSpawnPoint playerSpawnPoint;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterSettings(builder);
            RegisterSceneComponents(builder);

            Install(builder, new CoreInstaller());
            Install(builder, new UseCaseInstaller());
            Install(builder, new CharacterInstaller());
            Install(builder, new EconomyInstaller());
            Install(builder, new GameplaySystemInstaller());
            Install(builder, new UIInstaller());
        }

        void RegisterSettings(IContainerBuilder builder)
        {
            RegisterSetting(builder, gameInitializationSettings, nameof(gameInitializationSettings));
            RegisterSetting(builder, inputSettings, nameof(inputSettings));
            RegisterSetting(builder, characterDatabaseSettings, nameof(characterDatabaseSettings));
            RegisterSetting(builder, interactionSettings, nameof(interactionSettings));
            RegisterSetting(builder, pickupSystemSettings, nameof(pickupSystemSettings));
            RegisterSetting(builder, itemDatabase, nameof(itemDatabase));
            RegisterSetting(builder, characterBrainFactorySettings, nameof(characterBrainFactorySettings));
            RegisterSetting(builder, uiAssetSettings, nameof(uiAssetSettings));
        }

        void RegisterSceneComponents(IContainerBuilder builder)
        {
            RegisterComponent(builder, cameraRegistry, nameof(cameraRegistry));
            RegisterComponent(builder, mainUIDocument, nameof(mainUIDocument));
            RegisterComponent(builder, shopRegistry, nameof(shopRegistry));
            RegisterComponent(builder, gatheringSpotRegistry, nameof(gatheringSpotRegistry));
            RegisterComponent(builder, spawnSiteRegistry, nameof(spawnSiteRegistry));
            RegisterComponent(builder, playerSpawnPoint, nameof(playerSpawnPoint));
        }

        void Install(IContainerBuilder builder, IInstaller installer)
        {
            installer.Install(builder);
        }

        void RegisterSetting<T>(IContainerBuilder builder, T setting, string fieldName) where T : class
        {
            ThrowIfMissing(setting, fieldName);
            builder.RegisterInstance(setting);
        }

        void RegisterComponent<T>(IContainerBuilder builder, T component, string fieldName) where T : Component
        {
            ThrowIfMissing(component, fieldName);
            builder.RegisterComponent(component);
        }

        void ThrowIfMissing<T>(T fieldValue, string fieldName) where T : class
        {
            if (fieldValue == null)
                throw new MissingDependencyException($"{fieldName} is not assigned in GameLifetimeScope");
        }
    }
}