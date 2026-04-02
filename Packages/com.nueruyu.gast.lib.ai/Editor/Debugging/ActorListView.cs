using System;
using System.Collections.Generic;
using R3;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    [UxmlElement]
    partial class ActorListView : VisualElement, IDisposable
    {
        public static readonly string UssClassName = "actor-list-view";
        static readonly string HeaderUssClassName = UssClassName + "__header";
        static readonly string ListUssClassName = UssClassName + "__list";
        static readonly string ItemUssClassName = UssClassName + "__item";

        readonly ListView actorList;
        readonly CompositeDisposable disposables = new();

        public ActorListView()
        {
            AddToClassList(UssClassName);

            var header = new Label("Actors");
            header.AddToClassList(HeaderUssClassName);
            Add(header);

            actorList = new ListView();
            actorList.AddToClassList(ListUssClassName);
            actorList.makeItem = () =>
            {
                var label = new Label();
                label.AddToClassList(ItemUssClassName);
                return label;
            };
            Add(actorList);
        }

        public void Bind(AIDebuggerViewModel vm)
        {
            actorList.bindItem = (element, i) =>
            {
                var actor = vm.Actors.CurrentValue[i];
                ((Label)element).text = $"{actor.Name} ({actor.Id})";
            };

            vm.Actors.Subscribe(actors =>
            {
                actorList.itemsSource = actors;
                actorList.Rebuild();

                if (vm.SelectedActorId.Value != null)
                {
                    for (var i = 0; i < actors.Length; i++)
                    {
                        if (Equals(actors[i].Id, vm.SelectedActorId.Value))
                        {
                            actorList.SetSelectionWithoutNotify(new[] { i });
                            return;
                        }
                    }
                }
                actorList.ClearSelection();
            }).AddTo(disposables);

            Action<IEnumerable<object>> onSelectionChanged = _ =>
            {
                if (actorList.selectedIndex >= 0 && actorList.selectedIndex < vm.Actors.CurrentValue.Length)
                {
                    vm.SelectedActorId.Value = vm.Actors.CurrentValue[actorList.selectedIndex].Id;
                }
                else
                {
                    vm.SelectedActorId.Value = null;
                }
            };
            actorList.selectionChanged += onSelectionChanged;
            disposables.Add(Disposable.Create(() => actorList.selectionChanged -= onSelectionChanged));
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
