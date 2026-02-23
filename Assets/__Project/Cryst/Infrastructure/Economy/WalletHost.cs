using Gast.Domain.Economy;

namespace Cryst.Infrastructure.Economy
{
    class WalletHost : IWalletHost
    {
        public Wallet Wallet { get; }

        public WalletHost(Wallet wallet)
        {
            Wallet = wallet;
        }
    }
}