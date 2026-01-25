using System.Collections.Generic;

namespace Gast.Lib.Gaia.Dto
{
    // --- Enums ---
    public enum SessionStatus
    {
        Thinking,
        WaitingForTool,
        Completed,
        Error
    }

    // --- Requests ---
    public class CreateSessionRequest
    {
        public string Instruction { get; set; }
        public List<ToolDefinitionDto> ToolDefinitions { get; set; }
        public List<ObjectiveDefinitionDto> ObjectiveDefinitions { get; set; }
    }

    public class SubmitToolOutputsRequest
    {
        public List<ToolOutputDto> ToolOutputs { get; set; }
    }

    // --- Responses ---
    public class PlanningSessionDto
    {
        public string SessionId { get; set; }
        public SessionStatus Status { get; set; }
        public List<ToolCallDto> ToolCalls { get; set; }
        public PlanDto Plan { get; set; }
        public string ErrorMessage { get; set; }
    }

    // --- Shared Definitions ---
    public class ToolDefinitionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    public class ObjectiveDefinitionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    public class ToolCallDto
    {
        public string Id { get; set; }
        public string FunctionName { get; set; }
        public string Arguments { get; set; } // JSON String
    }

    public class ToolOutputDto
    {
        public string ToolCallId { get; set; }
        public string Output { get; set; } // JSON String
    }

    public class PlanDto
    {
        public string OverallObjective { get; set; }
        public List<ObjectiveDto> Objectives { get; set; }
        public StrategyDto Strategy { get; set; }
        public string Thought { get; set; }
    }

    public class ObjectiveDto
    {
        public string Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public int Priority { get; set; }
    }

    public class StrategyDto
    {
        public string Priority { get; set; }
        public string Engagement { get; set; }
        public Dictionary<string, object> RetreatCondition { get; set; }
    }
}
