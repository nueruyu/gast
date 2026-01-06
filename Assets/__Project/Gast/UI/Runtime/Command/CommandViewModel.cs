using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Application.UseCases.Npcs;
using R3;
using UnityEngine;

namespace Gast.UI.Command
{
    public class CommandViewModel : IDisposable
    {
        readonly CommandAIUseCase commandAiUseCase;
        readonly CompositeDisposable disposables = new();
        readonly CancellationTokenSource cts = new();

        public ReactiveProperty<bool> IsVisible { get; } = new(false);
        public ReactiveProperty<string> InstructionText { get; } = new("");

        public CommandViewModel(CommandAIUseCase commandAiUseCase)
        {
            this.commandAiUseCase = commandAiUseCase;
        }

        public void SendInstruction()
        {
            if (string.IsNullOrWhiteSpace(InstructionText.Value))
                return;

            Debug.Log($"Sending instruction: {InstructionText.Value}");
            commandAiUseCase.Execute(InstructionText.Value, cts.Token).Forget();

            InstructionText.Value = "";
            IsVisible.Value = false; // Hide after sending
        }

        public void Dispose()
        {
            disposables.Dispose();
            cts.Cancel();
        }
    }
}