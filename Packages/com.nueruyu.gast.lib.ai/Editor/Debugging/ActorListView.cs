using System;
using System.Collections.Generic;
using R3;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    [UxmlElement]
    partial class ActorListView : VisualElement, IDisposable
    {
        readonly CompositeDisposable disposables = new();

        public void Bind(AIDebuggerViewModel vm)
        {
            var actorList = this.Q<ListView>();

            actorList.makeItem = () => new Label();
            actorList.bindItem = (element, i) =>
            {
                var actor = vm.Actors.CurrentValue[i];
                ((Label)element).text = $"{actor.Name} ({actor.Id})";
            };

            vm.Actors.Subscribe(actors =>
            {
                actorList.itemsSource = actors;
                actorList.Rebuild();
            }).AddTo(disposables);

            Action<IEnumerable<object>> onSelectionChanged = _ => vm.SelectedActorIndex.Value = actorList.selectedIndex;
            actorList.selectionChanged += onSelectionChanged;
            disposables.Add(Disposable.Create(() => actorList.selectionChanged -= onSelectionChanged));

            vm.SelectedActorIndex.Subscribe(index => actorList.selectedIndex = index).AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
