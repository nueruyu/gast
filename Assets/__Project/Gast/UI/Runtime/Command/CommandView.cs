using System;
using Gast.Shared.UnityExtensions;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Command
{
    public class CommandView : VisualElement
    {
        readonly TextField instructionInput;
        readonly Button sendButton;

        public CommandView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            instructionInput = this.Q<TextField>("InstructionInput");
            sendButton = this.Q<Button>("SendButton");
        }

        public IDisposable Bind(CommandViewModel viewModel)
        {
            var d = new CompositeDisposable();

            viewModel.IsVisible.Subscribe(visible =>
            {
                style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
                if (visible)
                {
                    instructionInput.Focus();
                }
            }).AddTo(d);

            // Two-way binding for TextField
            instructionInput.RegisterValueChangedCallback(evt =>
            {
                viewModel.InstructionText.Value = evt.newValue;
            });

            viewModel.InstructionText.Subscribe(text =>
            {
                if (instructionInput.value != text)
                {
                    instructionInput.value = text;
                }
            }).AddTo(d);

            sendButton.SubscribeEvent<ClickEvent>(_ => viewModel.SendInstruction()).AddTo(d);

            // Handle Enter key to send
            instructionInput.SubscribeEvent<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == UnityEngine.KeyCode.Return || evt.keyCode == UnityEngine.KeyCode.KeypadEnter)
                {
                    viewModel.SendInstruction();
                    evt.StopPropagation();
                }
                else if (evt.keyCode == UnityEngine.KeyCode.Escape)
                {
                    viewModel.IsVisible.Value = false;
                    evt.StopPropagation();
                }
            }).AddTo(d);

            return d;
        }
    }
}