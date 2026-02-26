using Gast.Domain.Characters;

namespace Gast.Domain.Economy
{
    public interface IWalletHost : ICharacterFacet
    {
        Wallet Wallet { get; }
    }
}