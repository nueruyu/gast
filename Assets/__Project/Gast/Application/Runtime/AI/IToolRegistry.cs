using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gast.Application.AI
{
    public interface IToolRegistry
    {
        void RegisterToolSet(object target);

        void UnregisterToolSet(object target);

        List<ToolDefinition> GetToolDefinitions();

        Task<string> ExecuteAsync(string functionName, string argumentsJson);
    }
}