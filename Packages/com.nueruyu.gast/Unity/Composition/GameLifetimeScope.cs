using System.Collections.Generic;
using Gast.Application;
using Gast.Core.Exceptions;
using Gast.Lib.Gaia;
using Gast.Unity.UI;
using Gast.Unity.Features;
using Gast.Unity.Features.Cameras;
using Gast.Unity.Features.Economy;
using Gast.Unity.Features.Gameplay;
using Gast.Unity.Features.Gathering;
using Gast.Unity.Features.Inputs;
using Gast.Unity.Features.Placement;
using Gast.Unity.Features.SpawnSites;
using Gast.Unity.Infrastructure;
using Gast.Unity.Infrastructure.Characters;
using Gast.Unity.Infrastructure.HitDetection;
using Gast.Unity.Infrastructure.Items;
using Gast.Unity.Infrastructure.Pickups;
using Gast.Unity.Infrastructure.Remoting.AI;
using Gast.Unity.Infrastructure.Services;
using Gast.Unity.Shared.DI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;
using IContainerBuilder = VContainer.IContainerBuilder;

namespace Gast.Unity.Composition
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
        MockStoryGenerationSettings mockStoryGenerationSettings;

        [SerializeField]
        HitAreaSettings damageAreaSettings;

        [SerializeField]
        UIAssetSettings uiAssetSettings;

        [SerializeField]
        PlacementSettings placementSettings;

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
            new InfrastructureInstaller(mockAIPlanningSettings, mockStoryGenerationSettings).Install(builderAdapter);
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
            RegisterInstance(builder, mockStoryGenerationSettings, nameof(mockStoryGenerationSettings));
            RegisterInstance(builder, damageAreaSettings, nameof(damageAreaSettings));
            RegisterInstance(builder, uiAssetSettings, nameof(uiAssetSettings));
            RegisterInstance(builder, placementSettings, nameof(placementSettings));
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