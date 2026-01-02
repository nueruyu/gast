using System;
using UnityEngine;
using Gast.Core.Observables;

namespace Gast.Domain.Economy
{
    /// <summary>
    /// Currency (money) management.
    /// </summary>
    public class Wallet
    {
        readonly Live<int> amount;
        readonly int maxAmount;

        /// <summary>
        /// Current money amount. Observable for changes.
        /// </summary>
        public ILive<int> Amount => amount;

        public Wallet(int initialAmount = 0, int maxAmount = 999999)
        {
            this.amount = new Live<int>(Mathf.Clamp(initialAmount, 0, maxAmount));
            this.maxAmount = maxAmount;
        }

        /// <summary>
        /// Add money to the wallet.
        /// </summary>
        public void Add(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be positive.");
            amount.Value = Mathf.Min(amount.Value + value, maxAmount);
        }

        /// <summary>
        /// Check if the wallet can afford the specified cost.
        /// </summary>
        public bool CanAfford(int cost)
        {
            return amount.Value >= cost;
        }

        /// <summary>
        /// Try to spend money from the wallet.
        /// Returns false without spending if insufficient funds.
        /// </summary>
        public bool TrySpend(int cost)
        {
            if (cost < 0)
                throw new ArgumentOutOfRangeException(nameof(cost), "Cost must be positive.");

            if (amount.Value >= cost)
            {
                amount.Value -= cost;
                return true;
            }
            return false;
        }
    }
}