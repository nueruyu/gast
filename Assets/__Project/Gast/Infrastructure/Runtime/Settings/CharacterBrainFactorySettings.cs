using Gast.Features.Characters;
using Gast.Features.Npcs;
using System;
using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    [CreateAssetMenu(menuName = "Gast/Characters/Brain Factory Settings")]
    public class CharacterBrainFactorySettings : ScriptableObject
    {
        [SerializeField]
        CharacterTypeReference[] soldierBrainTypes = { };

        public CharacterTypeReference[] SoldierBrainTypes => soldierBrainTypes;
    }
}