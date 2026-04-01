using System;
using System.Linq;
using System.Reflection;
using Gast.Lib.AI.Debugging;
using R3;
using UnityEditor;
using UnityEngine;
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

        readonly VisualElement container;
        readonly CompositeDisposable disposables = new();

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

        public void Bind(AIDebuggerViewModel vm)
        {
            AIDebugInfo currentInfo = null;

            vm.SelectedDebugInfo.Subscribe(info =>
            {
                currentInfo = info;
                if (info == null) container.Clear();
            }).AddTo(disposables);

            void OnUpdate()
            {
                if (!EditorApplication.isPlaying || currentInfo == null) return;
                Render(currentInfo.WorldState);
            }

            EditorApplication.update += OnUpdate;
            disposables.Add(Disposable.Create(() => EditorApplication.update -= OnUpdate));
        }

        void Render(object state)
        {
            container.Clear();

            if (state == null) return;

            var type = state.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .OrderBy(p => p.Name);

            foreach (var property in properties)
            {
                var row = new VisualElement();
                row.AddToClassList(RowUssClassName);

                var nameLabel = new Label(property.Name);
                nameLabel.AddToClassList(NameUssClassName);

                var value = property.GetValue(state);
                var valueLabel = new Label(FormatValue(value));
                valueLabel.AddToClassList(ValueUssClassName);

                if (value is bool boolValue)
                    valueLabel.AddToClassList(boolValue ? ValueTrueUssClassName : ValueFalseUssClassName);

                row.Add(nameLabel);
                row.Add(valueLabel);
                container.Add(row);
            }
        }

        string FormatValue(object value)
        {
            if (value == null) return "null";
            if (value is bool boolValue) return boolValue ? "True" : "False";
            if (value is float floatValue) return floatValue.ToString("F2");
            if (value is double doubleValue) return doubleValue.ToString("F2");
            if (value is Vector3 vec3) return $"({vec3.x:F2}, {vec3.y:F2}, {vec3.z:F2})";
            if (value is Vector2 vec2) return $"({vec2.x:F2}, {vec2.y:F2})";
            return value.ToString();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
