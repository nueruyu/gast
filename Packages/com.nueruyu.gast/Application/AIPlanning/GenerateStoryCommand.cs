using Gast.Core.Commands;

namespace Gast.Application.AIPlanning
{
    public readonly struct GenerateStoryCommand : IAsyncCommand<bool>
    {
        public string Instruction { get; }

        public GenerateStoryCommand(string instruction)
        {
            Instruction = instruction;
        }
    }
}
