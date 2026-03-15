using Gast.Domain.Characters;

namespace Cryst.Domain.Characters.Facets
{
    public class TerritorialCharacter : ICharacterFacet
    {
        public Territory Territory { get; }
        readonly ICharacterBody body;

        public TerritorialCharacter(Territory territory, ICharacterBody body)
        {
            Territory = territory;
            this.body = body;
        }

        public bool IsOutOfTerritory()
        {
            return Territory.IsOutOfTerritory(body.Position);
        }

        public bool HasReturnedToTerritory()
        {
            return Territory.IsInsideTerritoryWithMargin(body.Position, margin: 2f);
        }
    }
}
