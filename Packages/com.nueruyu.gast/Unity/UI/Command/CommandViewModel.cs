using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Application.AIPlanning;
using Gast.Core.Commands;
using R3;
using UnityEngine;

namespace Gast.Unity.UI.Command
{
    public class CommandViewModel : IDisposable
    {
        readonly ICommandDispatcher commandDispatcher;
        readonly CancellationTokenSource cts = new();
        readonly CompositeDisposable disposables = new();

        public CommandViewModel(ICommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher;
        }

        public ReactiveProperty<bool> IsVisible { get; } = new(false);

        public ReactiveProperty<string> InstructionText { get; } =
            new("Explore the current area, eliminate any enemies, and collect healing items.");

        public ReactiveProperty<bool> IsLoading { get; } = new(false);
        public ReactiveProperty<string> StatusMessage { get; } = new("");
        public ReactiveProperty<bool> HasError { get; } = new(false);

        public void Dispose()
        {
            disposables.Dispose();
            cts.Cancel();
        }

        public void SendInstruction()
        {
            if (string.IsNullOrWhiteSpace(InstructionText.Value))
                return;

            if (IsLoading.Value)
                return;

            SendInstructionAsync().Forget();
        }

        async UniTaskVoid SendInstructionAsync()
        {
            var instruction = InstructionText.Value;

            StatusMessage.Value = "";
            HasError.Value = false;
            IsLoading.Value = true;

            Debug.Log($"[CommandViewModel] Sending instruction: {instruction}");

            try
            {
                var result = await commandDispatcher
                    .DispatchAsync<CommandAICommand, CommandAIResult>(new(instruction), cts.Token);

                if (result.IsSuccess)
                {
                    StatusMessage.Value = result.Message ?? $"Assigned {result.GoalCount} goal(s) to NPC";
                    HasError.Value = false;
                    IsVisible.Value = false;
                }
                else
                {
                    StatusMessage.Value = result.Message;
                    HasError.Value = true;
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                StatusMessage.Value = "An unexpected error occurred";
                HasError.Value = true;
            }
            finally
            {
                IsLoading.Value = false;
            }
        }
    }
}