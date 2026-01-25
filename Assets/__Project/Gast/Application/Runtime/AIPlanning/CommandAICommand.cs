using System;
using Gast.Core.Commands;

namespace Gast.Application.AIPlanning
{
    [Serializable]
    public readonly struct CommandAICommand : IAsyncCommand<CommandAIResult>
    {
        public string Instruction { get; }

        public CommandAICommand(string instruction)
        {
            Instruction = instruction;
        }
    }
}