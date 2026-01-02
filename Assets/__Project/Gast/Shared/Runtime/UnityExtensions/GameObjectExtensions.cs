using Gast.Shared.Exceptions;
using UnityEngine;

namespace Gast.Shared.UnityExtensions
{
    public static class GameObjectExtensions
    {
        public static T RequireComponent<T>(this GameObject gameObject) where T : class
        {
            if (gameObject.TryGetComponent<T>(out var component))
            {
                return component;
            }
            throw new ComponentNotFoundException(typeof(T), gameObject.GetType());
        }

        public static T RequireComponent<T>(this Component component) where T : class
        {
            if (component.TryGetComponent<T>(out var requiredComponent))
            {
                return requiredComponent;
            }
            throw new ComponentNotFoundException(typeof(T), component.GetType());
        }

        public static T RequireComponentInChildren<T>(this GameObject gameObject) where T : class
        {
            var component = gameObject.GetComponentInChildren<T>();
            if (component != null)
            {
                return component;
            }
            throw new ComponentNotFoundException(typeof(T), gameObject.GetType());
        }

        public static T RequireComponentInChildren<T>(this Component component) where T : class
        {
            var requiredComponent = component.GetComponentInChildren<T>();
            if (requiredComponent != null)
            {
                return requiredComponent;
            }
            throw new ComponentNotFoundException(typeof(T), component.GetType());
        }

        public static T RequireComponentInParent<T>(this GameObject gameObject) where T : class
        {
            var component = gameObject.GetComponentInParent<T>();
            if (component != null)
            {
                return component;
            }
            throw new ComponentNotFoundException(typeof(T), gameObject.GetType());
        }

        public static T RequireComponentInParent<T>(this Component component) where T : class
        {
            var requiredComponent = component.GetComponentInParent<T>();
            if (requiredComponent != null)
            {
                return requiredComponent;
            }
            throw new ComponentNotFoundException(typeof(T), component.GetType());
        }
    }
}