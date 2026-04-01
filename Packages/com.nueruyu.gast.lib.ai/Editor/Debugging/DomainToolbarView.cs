using System;
using R3;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    [UxmlElement]
    partial class DomainToolbarView : VisualElement, IDisposable
    {
        public static readonly string UssClassName = "domain-toolbar-view";

        readonly CompositeDisposable disposables = new();

        public DomainToolbarView()
        {
            AddToClassList(UssClassName);
        }

        public void Bind(AIDebuggerViewModel vm)
        {
            CompositeDisposable toggleDisposables = null;

            vm.DomainNameChoices.Subscribe(domains =>
            {
                toggleDisposables?.Dispose();
                toggleDisposables = new CompositeDisposable();

                Clear();

                if (domains.Length > 0)
                {
                    if (vm.SelectedDomainIndex.Value < 0 || vm.SelectedDomainIndex.Value >= domains.Length)
                        vm.SelectedDomainIndex.Value = 0;
                }
                else
                {
                    vm.SelectedDomainIndex.Value = -1;
                }

                for (var i = 0; i < domains.Length; i++)
                {
                    var index = i;
                    var toggle = new ToolbarToggle { text = domains[i] };
                    toggle.SetValueWithoutNotify(index == vm.SelectedDomainIndex.Value);

                    var callback = new EventCallback<ChangeEvent<bool>>(evt =>
                    {
                        if (evt.newValue) vm.SelectedDomainIndex.Value = index;
                    });
                    toggle.RegisterValueChangedCallback(callback);
                    toggleDisposables.Add(Disposable.Create(() => toggle.UnregisterValueChangedCallback(callback)));
                    Add(toggle);
                }
            }).AddTo(disposables);

            disposables.Add(Disposable.Create(() => toggleDisposables?.Dispose()));

            vm.SelectedDomainIndex.Subscribe(index =>
            {
                var toggles = this.Query<ToolbarToggle>().ToList();
                for (var i = 0; i < toggles.Count; i++) toggles[i].SetValueWithoutNotify(i == index);
            }).AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
