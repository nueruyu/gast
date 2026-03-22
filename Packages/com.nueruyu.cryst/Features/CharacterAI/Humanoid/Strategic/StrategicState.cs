using System.Linq;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicState : IWorldState<StrategicState>
    {
        public AIMode CurrentMode { get; private set; }
        public bool HasGatheringObjective { get; private set; }
        public bool HasObjectiveCombatTarget { get; private set; }
        public bool IsThreatened { get; private set; }
        public bool IsOutOfTerritory { get; private set; }
        public bool IsOutOfTerritoryCore { get; private set; }

        public void WriteTo(ref StrategicState dest)
        {
            dest ??= new();
            dest.CurrentMode = CurrentMode;
            dest.HasGatheringObjective = HasGatheringObjective;
            dest.HasObjectiveCombatTarget = HasObjectiveCombatTarget;
            dest.IsThreatened = IsThreatened;
            dest.IsOutOfTerritory = IsOutOfTerritory;
            dest.IsOutOfTerritoryCore = IsOutOfTerritoryCore;
        }

        public void Update(ActorContext<StrategicState> context)
        {
            var character = context.Character;
            var actor = context.Actor;
            var memory = context.GetModule<HumanoidMemory>();

            var threats = actor.VisionSensor.VisibleCharacters
                .Select(c => c.As<BaseCharacter>())
                .Where(a => a.IsThreatTo(actor))
                .ToList();
            var closestThreat = threats
                .OrderBy(a => Vector3.Distance(actor.VisionSensor.EyePosition, a.Body.Position))
                .FirstOrDefault();
            memory.SetThreatTarget(closestThreat);

            IsThreatened = closestThreat != null && closestThreat.Status.IsAlive.Value;

            if (memory.CurrentObjective != null && memory.CurrentObjective.IsCompleted.Value)
                memory.ClearObjective();

            CurrentMode = memory.CurrentMode;
            HasGatheringObjective = memory.CurrentObjective is AcquireItemObjective;
            HasObjectiveCombatTarget = memory.HasObjectiveCombatTarget;

            if (character.Is(out TerritorialCharacter territorial))
            {
                IsOutOfTerritory = territorial.IsOutOfTerritory();
                IsOutOfTerritoryCore = !territorial.HasReturnedToTerritory();
            }
            else
            {
                IsOutOfTerritory = false;
                IsOutOfTerritoryCore = false;
            }
        }
    }
}