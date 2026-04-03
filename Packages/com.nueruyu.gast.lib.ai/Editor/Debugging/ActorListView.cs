using System;
using System.Collections.Generic;
using ObservableCollections;
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

        public void Dispose()
        {
            disposables.Dispose();
        }

        public void Bind(AIDebuggerViewModel vm)
        {
            var actorListSource = new List<ActorInfo>();
            actorList.itemsSource = actorListSource;

            vm.Actors.ObserveAdd().Subscribe(e =>
            {
                actorListSource.Insert(e.Index, e.Value);
                actorList.RefreshItems();

                UpdateSelection();
            }).AddTo(disposables);

            vm.Actors.ObserveRemove().Subscribe(e =>
            {
                actorListSource.RemoveAt(e.Index);
                actorList.RefreshItems();

                UpdateSelection();
            }).AddTo(disposables);

            vm.Actors.ObserveReset().Subscribe(_ =>
            {
                actorListSource.Clear();
                actorList.RefreshItems();

                UpdateSelection();
            }).AddTo(disposables);

            actorList.bindItem = (element, i) =>
            {
                var actor = actorListSource[i];
                ((Label)element).text = $"{actor.Name} ({actor.Id})";
            };

            vm.SelectedActorId
                .Subscribe(_ => UpdateSelection())
                .AddTo(disposables);

            Action<IEnumerable<object>> onSelectionChanged = _ =>
            {
                if (actorList.selectedIndex >= 0 &&
                    actorList.selectedIndex < actorListSource.Count)
                    vm.SelectedActorId.Value = actorListSource[actorList.selectedIndex].Id;
                else
                    vm.SelectedActorId.Value = null;
            };
            actorList.selectionChanged += onSelectionChanged;
            disposables.Add(Disposable.Create(() => { actorList.selectionChanged -= onSelectionChanged; }));
            return;

            void UpdateSelection()
            {
                var id = vm.SelectedActorId.CurrentValue;

                if (id != null)
                    for (var i = 0; i < actorListSource.Count; i++)
                        if (Equals(actorListSource[i].Id, id))
                        {
                            actorList.SetSelectionWithoutNotify(new[] { i });
                            return;
                        }

                actorList.ClearSelection();
            }
        }
    }
}