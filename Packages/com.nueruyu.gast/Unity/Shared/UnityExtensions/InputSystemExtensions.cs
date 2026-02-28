using System;
using R3;
using UnityEngine.InputSystem;

namespace Gast.Unity.Shared.UnityExtensions
{
    public static class InputSystemExtensions
    {
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
    }
}