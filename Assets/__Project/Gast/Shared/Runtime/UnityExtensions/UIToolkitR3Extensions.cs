using R3;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Gast.Shared.UnityExtensions
{
    public static class UIToolkitR3Extensions
    {
        public static IDisposable BindText(this Label label, ReadOnlyReactiveProperty<string> property, CancellationToken cancellationToken)
        {
            return property.Subscribe(x => label.text = x, cancellationToken);
        }

        public static IDisposable BindVisibility(this VisualElement element, ReadOnlyReactiveProperty<bool> property, CancellationToken cancellationToken)
        {
            return property.Subscribe(
                isVisible => element.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None,
                cancellationToken);
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
            }, cancellationToken).AddTo(disposables);

            selectedIndex.Subscribe(index =>
            {
                if (dropdown.index != index)
                {
                    dropdown.index = index;
                }
            }, cancellationToken).AddTo(disposables);

            dropdown.RegisterValueChangedCallback(evt =>
            {
                selectedIndex.Value = dropdown.index;
            });

            cancellationToken.Register(() => dropdown.UnregisterValueChangedCallback(evt => { }));

            return disposables;
        }

        public static IDisposable BindTo<T>(
            this ListView listView,
            ReadOnlyReactiveProperty<IReadOnlyList<T>> source,
            Action<VisualElement, T> bindItem,
            CancellationToken cancellationToken)
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
            }, cancellationToken);
        }

        public static void SetEnabled(
            this InputActionMap inputActionMap,
            bool enabled)
        {
            if (enabled)
                inputActionMap.Enable();
            else
                inputActionMap.Disable();
        }

        public static IDisposable SubscribePerformed(
            this InputAction inputAction,
            Action<InputAction.CallbackContext> callback)
        {
            inputAction.performed += callback;

            return Disposable.Create(() => inputAction.performed -= callback);
        }

        public static IDisposable SubscribeCanceled(
            this InputAction inputAction,
            Action<InputAction.CallbackContext> callback)
        {
            inputAction.canceled += callback;

            return Disposable.Create(() => inputAction.canceled -= callback);
        }

        public static IDisposable SubscribeEvent<TEventType>(
            this VisualElement visualElement,
            EventCallback<TEventType> callback,
            TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEventType : EventBase<TEventType>, new()
        {
            visualElement.RegisterCallback(callback, useTrickleDown);

            return Disposable.Create(() => visualElement.UnregisterCallback(callback, useTrickleDown));
        }
    }
}
