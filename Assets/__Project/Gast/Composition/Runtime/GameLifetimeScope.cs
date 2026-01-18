using Gast.Composition.Installers;
using Gast.Core.Exceptions;
using Gast.Features.Cameras;
using Gast.Features.Economy;
using Gast.Features.Gameplay;
using Gast.Features.Gathering;
using Gast.Features.Inputs;
using Gast.Features.Interactions;
using Gast.Features.SpawnSites;
using Gast.Infrastructure.Remoting.AI;
using Gast.Infrastructure.Settings;
using Gast.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace Gast.Composition
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
        PickupSystemSettings pickupSystemSettings;

        [SerializeField]
        ItemDatabaseSettings itemDatabase;

        [SerializeField]
        AIServerSettings aiServerSettings;

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
            Install(builder, new AIInstaller());
            Install(builder, new EconomyInstaller());
            Install(builder, new GameplaySystemInstaller());
            Install(builder, new UIInstaller());
        }

        void RegisterSettings(IContainerBuilder builder)
        {
            RegisterSetting(builder, gameInitializationSettings, nameof(gameInitializationSettings));
            RegisterSetting(builder, inputSettings, nameof(inputSettings));
            RegisterSetting(builder, characterDatabaseSettings, nameof(characterDatabaseSettings));
            RegisterSetting(builder, pickupSystemSettings, nameof(pickupSystemSettings));
            RegisterSetting(builder, itemDatabase, nameof(itemDatabase));
            RegisterSetting(builder, aiServerSettings, nameof(aiServerSettings));
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