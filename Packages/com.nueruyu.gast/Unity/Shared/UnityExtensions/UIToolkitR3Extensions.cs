using System;
using System.Collections.Generic;
using R3;
using UnityEngine.UIElements;

namespace Gast.Unity.Shared.UnityExtensions
{
    public static class UIToolkitR3Extensions
    {
        public static IDisposable BindText(this Label label, ReadOnlyReactiveProperty<string> property)
        {
            return property
                .Subscribe(x => label.text = x);
        }

        public static IDisposable BindVisibility(this VisualElement element, ReadOnlyReactiveProperty<bool> property)
        {
            return property.Subscribe(isVisible => element.style.display = isVisible ?
                DisplayStyle.Flex :
                DisplayStyle.None);
        }

        public static IDisposable Bind(
            this DropdownField dropdown,
            ReactiveProperty<int> selectedIndex,
            ReadOnlyReactiveProperty<IReadOnlyList<string>> choices)
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

            var callback = new EventCallback<ChangeEvent<string>>(evt =>
            {
                selectedIndex.Value = dropdown.index;
            });
            dropdown.RegisterValueChangedCallback(callback);
            disposables.Add(Disposable.Create(() => dropdown.UnregisterValueChangedCallback(callback)));

            return disposables;
        }

        public static IDisposable BindTo<T>(
            this ListView listView,
            ReadOnlyReactiveProperty<IReadOnlyList<T>> source,
            Action<VisualElement, T> bindItem)
        {
            listView.bindItem = (element, index) =>
            {
                if (source.CurrentValue != null && index >= 0 && index < source.CurrentValue.Count)
                {
                    bindItem(element, source.CurrentValue[index]);
                }
            };

            return source.Subscribe(items =>
            {
                listView.itemsSource = items as System.Collections.IList ?? new List<T>(items);
                listView.Rebuild();
            });
        }
    }
}