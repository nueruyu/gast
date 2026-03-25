using Cryst.Features.CharacterAI.Humanoid.Objective;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Pickups;

namespace Cryst.Features.CharacterAI
{
    public record AIBrainServices(
        ICharacterRepository CharacterRepository,
        IPickupRepository PickupRepository,
        ICommandDispatcher CommandDispatcher,
        IObjectiveQueries ObjectiveQueries
    );
}
