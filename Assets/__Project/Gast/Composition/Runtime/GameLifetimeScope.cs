using System.Collections.Generic;
using Gast.Application;
using Gast.Core.Exceptions;
using Gast.Features;
using Gast.Features.Cameras;
using Gast.Features.Economy;
using Gast.Features.Gameplay;
using Gast.Features.Gathering;
using Gast.Features.Inputs;
using Gast.Features.SpawnSites;
using Gast.Infrastructure;
using Gast.Lib.Gaia;
using Gast.Infrastructure.Services;
using Gast.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;
using IContainerBuilder = VContainer.IContainerBuilder;
using Gast.Shared.DI;
using Gast.Infrastructure.Characters;
using Gast.Infrastructure.Pickups;
using Gast.Infrastructure.Items;
using Gast.Infrastructure.Remoting.AI;

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
        GaiaServerSettings gaiaServerSettings;

        [SerializeField]
        MockAIPlanningSettings mockAIPlanningSettings;

        [SerializeField]
        UIAssetSettings uiAssetSettings;

        [Header("Scene Components")]
        [SerializeField]
        CameraRegistry cameraRegistry;

        [SerializeField]
        UIDocument mainUIDocument;

        [SerializeField]
        ShopRegistry shopRegistry;

        [SerializeField]
        GatheringSpotRegistry gatheringSpotRegistry;

        [SerializeField]
        SpawnSiteRegistry spawnSiteRegistry;

        [SerializeField]
        PlayerSpawnPoint playerSpawnPoint;

        [Header("Extensions")]
        [SerializeField]
        List<InstallerAsset> additionalInstallers = new();

        protected override void Configure(IContainerBuilder builder)
        {
            var builderAdapter = new VContainerBuilder(builder);

            RegisterSettings(builder);
            RegisterSceneComponents(builder);

            // Install registrations from each assembly
            new ApplicationInstaller().Install(builderAdapter);
            new FeaturesInstaller().Install(builderAdapter);
            new InfrastructureInstaller(mockAIPlanningSettings).Install(builderAdapter);
            new UIInstaller().Install(builderAdapter);

            // Install additional/override registrations from external assemblies
            foreach (var installer in additionalInstallers)
            {
                if (installer != null)
                    installer.Install(builderAdapter);
            }

            // Register EntryPoints (VContainer specific)
            builder.RegisterEntryPoint<LifecycleTaskRunner>();
        }

        void RegisterSettings(IContainerBuilder builder)
        {
            RegisterInstance(builder, gameInitializationSettings, nameof(gameInitializationSettings));
            RegisterInstance(builder, inputSettings, nameof(inputSettings));
            RegisterInstance(builder, characterDatabaseSettings, nameof(characterDatabaseSettings));
            RegisterInstance(builder, pickupSystemSettings, nameof(pickupSystemSettings));
            RegisterInstance(builder, itemDatabase, nameof(itemDatabase));
            RegisterInstance(builder, gaiaServerSettings, nameof(gaiaServerSettings));
            RegisterInstance(builder, mockAIPlanningSettings, nameof(mockAIPlanningSettings));
            RegisterInstance(builder, uiAssetSettings, nameof(uiAssetSettings));
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

        void RegisterInstance<T>(IContainerBuilder builder, T instance, string fieldName) where T : class
        {
            ThrowIfMissing(instance, fieldName);
            builder.RegisterInstance(instance);
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