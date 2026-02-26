using System.Collections.Generic;

namespace Gast.Application.AI
{
    public interface IToolRegistry
    {
        List<ToolDefinition> GetToolDefinitions();
        ITool GetTool(string toolName);
    }
}
