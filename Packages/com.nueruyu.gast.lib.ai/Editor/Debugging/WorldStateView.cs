using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Gast.Lib.AI.Debugging;
using R3;
using UnityEditor;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    [UxmlElement]
    partial class WorldStateView : VisualElement, IDisposable
    {
        public static readonly string UssClassName = "world-state-view";
        static readonly string HeaderUssClassName = UssClassName + "__header";
        static readonly string ContainerUssClassName = UssClassName + "__container";
        static readonly string RowUssClassName = UssClassName + "__row";
        static readonly string NameUssClassName = UssClassName + "__name";
        static readonly string ValueUssClassName = UssClassName + "__value";
        static readonly string ValueTrueUssClassName = UssClassName + "__value--true";
        static readonly string ValueFalseUssClassName = UssClassName + "__value--false";

        static readonly Dictionary<Type, PropertyInfo[]> propertyCache = new();

        readonly VisualElement container;
        readonly CompositeDisposable disposables = new();
        PropertyInfo[] currentProperties;

        Type currentStateType;
        Label[] valueLabels;

        public WorldStateView()
        {
            AddToClassList(UssClassName);

            var header = new Label("World State");
            header.AddToClassList(HeaderUssClassName);
            Add(header);

            container = new VisualElement();
            container.AddToClassList(ContainerUssClassName);
            Add(container);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        public void Bind(AIDebuggerViewModel vm)
        {
            AIDebugInfo currentInfo = null;

            vm.SelectedDebugInfo.Subscribe(info =>
            {
                currentInfo = info;
                if (info == null)
                    container.Clear();
            }).AddTo(disposables);

            void OnUpdate()
            {
                if (!EditorApplication.isPlaying ||
                    currentInfo == null)
                    return;

                Render(currentInfo.WorldState);
            }

            EditorApplication.update += OnUpdate;
            disposables.Add(Disposable.Create(() => EditorApplication.update -= OnUpdate));
        }

        void Render(object state)
        {
            if (state == null)
            {
                if (currentStateType != null)
                {
                    container.Clear();
                    currentStateType = null;
                    currentProperties = null;
                    valueLabels = null;
                }

                return;
            }

            var type = state.GetType();
            if (currentStateType != type)
                RebuildUI(type);

            UpdateValues(state);
        }

        void RebuildUI(Type type)
        {
            container.Clear();
            currentStateType = type;
            currentProperties = GetProperties(type);
            valueLabels = new Label[currentProperties.Length];

            for (var i = 0; i < currentProperties.Length; i++)
            {
                var property = currentProperties[i];
                var row = new VisualElement();
                row.AddToClassList(RowUssClassName);

                var nameLabel = new Label(property.Name);
                nameLabel.AddToClassList(NameUssClassName);

                var valueLabel = new Label();
                valueLabel.AddToClassList(ValueUssClassName);
                valueLabels[i] = valueLabel;

                row.Add(nameLabel);
                row.Add(valueLabel);
                container.Add(row);
            }
        }

        PropertyInfo[] GetProperties(Type type)
        {
            if (!propertyCache.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead)
                    .OrderBy(p => p.Name)
                    .ToArray();

                propertyCache[type] = properties;
            }

            return properties;
        }

        void UpdateValues(object state)
        {
            for (var i = 0; i < currentProperties.Length; i++)
            {
                var property = currentProperties[i];
                var value = property.GetValue(state);
                var valueLabel = valueLabels[i];

                valueLabel.text = FormatValue(value);

                if (value is bool boolValue)
                {
                    valueLabel.EnableInClassList(ValueTrueUssClassName, boolValue);
                    valueLabel.EnableInClassList(ValueFalseUssClassName, !boolValue);
                }
                else
                {
                    valueLabel.EnableInClassList(ValueTrueUssClassName, false);
                    valueLabel.EnableInClassList(ValueFalseUssClassName, false);
                }
            }
        }

        string FormatValue(object value)
        {
            if (value == null)
                return "null";
            if (value is bool boolValue)
                return boolValue ? "True" : "False";
            if (value is float floatValue)
                return floatValue.ToString("F2");
            if (value is double doubleValue)
                return doubleValue.ToString("F2");
            if (value is Vector3 vec3Num)
                return $"({vec3Num.X:F2}, {vec3Num.Y:F2}, {vec3Num.Z:F2})";
            if (value is Vector2 vec2Num)
                return $"({vec2Num.X:F2}, {vec2Num.Y:F2})";
            if (value is UnityEngine.Vector3 vec3)
                return $"({vec3.x:F2}, {vec3.y:F2}, {vec3.z:F2})";
            if (value is UnityEngine.Vector2 vec2)
                return $"({vec2.x:F2}, {vec2.y:F2})";

            return value.ToString();
        }
    }
}