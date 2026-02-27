using R3;
using System;
using UnityEngine.UIElements;

namespace Gast.Shared.UnityExtensions
{
    public static class UIToolkitExtensions
    {
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