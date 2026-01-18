using Gast.Features.Npcs.Actions;
using System;
using UnityEngine;

namespace Gast.Features.Npcs
{
    [Serializable]
    public class SoldierBrainSettings
    {
        [Header("Tactical Actions")]
        [SerializeField]
        ChaseTargetAction chaseTargetAction = new();
        public ChaseTargetAction ChaseTargetAction => chaseTargetAction;

        [SerializeField]
        MeleeAttackAction meleeAttackAction = new();
        public MeleeAttackAction MeleeAttackAction => meleeAttackAction;

        [SerializeField]
        BackOffAction backOffAction = new();
        public BackOffAction BackOffAction => backOffAction;

        [SerializeField]
        StrafeAction strafeAction = new();
        public StrafeAction StrafeAction => strafeAction;
    }
}
