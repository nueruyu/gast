using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UIElements;

namespace Gast.Shared.UnityExtensions
{
    public static class UIToolkitR3Extensions
    {
        public static IDisposable BindText(this Label label, ReadOnlyReactiveProperty<string> property, CancellationToken cancellationToken)
        {
            var disposables = new CompositeDisposable();
            property.Subscribe(x => label.text = x).AddTo(disposables);
            cancellationToken.Register(() => disposables.Dispose());
            return disposables;
        }

        public static IDisposable BindVisibility(this VisualElement element, ReadOnlyReactiveProperty<bool> property, CancellationToken cancellationToken)
        {
            var disposables = new CompositeDisposable();
            property.Subscribe(isVisible => element.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None).AddTo(disposables);
            cancellationToken.Register(() => disposables.Dispose());
            return disposables;
        }

        public static IDisposable Bind(
            this DropdownField dropdown,
            ReactiveProperty<int> selectedIndex,
            ReadOnlyReactiveProperty<IReadOnlyList<string>> choices,
            CancellationToken cancellationToken)
        {
            var disposables = new CompositeDisposable();

            choices.Subscribe(c =>
            {
                dropdown.choices = c is List<string> list ? list : new List<string>(c);
                if (selectedIndex.Value >= c.Count && c.Count > 0)
                {
                    selectedIndex.Value = c.Count - 1;
                }
                else if (c.Count == 0)
                {
                    selectedIndex.Value = -1;
                }
            }).AddTo(disposables);

            selectedIndex.Subscribe(index =>
            {
                if (dropdown.index != index)
                {
                    dropdown.index = index;
                }
            }).AddTo(disposables);

            dropdown.RegisterValueChangedCallback(evt =>
            {
                selectedIndex.Value = dropdown.index;
            });

            cancellationToken.Register(() => disposables.Dispose());

            return disposables;
        }

        public static IDisposable BindTo<T>(
            this ListView listView,
            ReadOnlyReactiveProperty<IReadOnlyList<T>> source,
            Action<VisualElement, T> bindItem,
            CancellationToken cancellationToken)
        {
            var disposables = new CompositeDisposable();

            listView.bindItem = (element, index) =>
            {
                if (source.CurrentValue != null && index >= 0 && index < source.CurrentValue.Count)
                {
                    bindItem(element, source.CurrentValue[index]);
                }
            };

            source.Subscribe(items =>
            {
                listView.itemsSource = items as System.Collections.IList ?? new List<T>(items);
                listView.Rebuild();
            }).AddTo(disposables);

            cancellationToken.Register(() => disposables.Dispose());

            return disposables;
        }
    }
}
