using System.Threading.Tasks;

namespace Gast.Application.AI
{
    public interface ITool
    {
        ToolDefinition Definition { get; }
        Task<string> ExecuteAsync(string argumentsJson);
    }
}
