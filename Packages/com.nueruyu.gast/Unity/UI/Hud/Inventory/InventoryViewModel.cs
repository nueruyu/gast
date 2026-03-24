using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Domain.Economy;
using Gast.Domain.Players;
using Gast.Unity.Shared.Observables;
using R3;

namespace Gast.Unity.UI.Hud.Inventory
{
    public class InventoryViewModel : IDisposable
    {
        private readonly CompositeDisposable disposables = new();

        public ReadOnlyReactiveProperty<IReadOnlyList<ItemStackViewModel>> InventoryItems { get; }

        public InventoryViewModel(IPlayerManager playerManager, ItemStackViewModelFactory itemStackViewModelFactory)
        {
            InventoryItems = playerManager.CurrentCharacter.ToObservable()
                .Select(character =>
                {
                    if (character == null || !character.Is(out IInventoryHost inventoryHost))
                    {
                        return Observable.Return((IReadOnlyList<ItemStackViewModel>)Array.Empty<ItemStackViewModel>());
                    }

                    return inventoryHost.Inventory.InventoryChanged.ToObservable()
                        .Prepend(Unit.Default)
                        .Select(_ => (IReadOnlyList<ItemStackViewModel>)inventoryHost.Inventory.Items
                            .Select(itemStackViewModelFactory.Create)
                            .Take(10)
                            .ToArray());
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}