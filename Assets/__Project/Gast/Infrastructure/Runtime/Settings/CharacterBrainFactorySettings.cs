using Gast.Features.Characters;
using Gast.Features.Npcs;
using System;
using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    [CreateAssetMenu(menuName = "DescrioGames/Characters/Brain Factory Settings")]
    public class CharacterBrainFactorySettings : ScriptableObject
    {
        [SerializeField]
        CharacterTypeReference[] soldierBrainTypes = { };

        [SerializeField]
        SoldierBrainSettings soldierBrainSettings = new();

        public CharacterTypeReference[] SoldierBrainTypes => soldierBrainTypes;
        public SoldierBrainSettings SoldierBrainSettings => soldierBrainSettings;
    }
}