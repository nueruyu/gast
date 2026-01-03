using System;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Interactions
{
    public class MenuView : VisualElement
    {
        public MenuView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            //focusable = true;
        }

        public IDisposable Bind(MenuViewModel viewModel)
        {
            var disposables = new CompositeDisposable();

            viewModel.Visible
                .Subscribe(visible =>
                {
                    var display = visible ? DisplayStyle.Flex : DisplayStyle.None;
                    style.display = display;

                    //if (visible)
                    //{
                    //    Focus();
                    //}
                })
                .AddTo(disposables);

            return disposables;
        }
    }
}