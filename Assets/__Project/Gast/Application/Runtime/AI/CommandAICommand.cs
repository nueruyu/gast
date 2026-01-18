using System;
using Gast.Core.Commands;

namespace Gast.Application.AI
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
