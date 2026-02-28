using System;
using Gast.Unity.Shared.UnityExtensions;
using Gast.Unity.UI.Command;
using R3;
using UnityEngine.UIElements;

namespace Gast.Unity.UI.Menu
{
    public class MenuView : VisualElement
    {
        readonly Button commandButton;

        public MenuView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            focusable = true;
            commandButton = this.Q<Button>("CommandButton");
        }

        public IDisposable Bind(MenuViewModel viewModel, CommandViewModel commandViewModel)
        {
            var disposables = new CompositeDisposable();

            viewModel.Visible
                .Subscribe(visible =>
                {
                    var display = visible ? DisplayStyle.Flex : DisplayStyle.None;
                    style.display = display;

                    if (visible)
                    {
                        Focus();
                    }
                })
                .AddTo(disposables);

            commandButton.SubscribeEvent<ClickEvent>(_ =>
            {
                commandViewModel.IsVisible.Value = true;
            }).AddTo(disposables);

            return disposables;
        }
    }
}