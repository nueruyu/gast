using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Domain.Players;
using Gast.Shared.Observables;
using R3;

namespace Gast.UI.Hud
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
                    if (character == null)
                    {
                        return Observable.Return((IReadOnlyList<ItemStackViewModel>)Array.Empty<ItemStackViewModel>());
                    }

                    return character.Inventory.InventoryChanged.ToObservable()
                        .Prepend(Unit.Default)
                        .Select(_ => (IReadOnlyList<ItemStackViewModel>)character.Inventory.Items
                            .Select(itemStackViewModelFactory.Create)
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