using System.Collections.Generic;
using System.Threading.Tasks;
using Gast.Application.AI.Models;

namespace Gast.Application.AI.Tools
{
    public interface IToolRegistry
    {
        void Register(object target);
        void Unregister(object target);

        List<ToolDefinition> GetToolDefinitions();

        Task<string> ExecuteAsync(string functionName, string argumentsJson);
    }
}
