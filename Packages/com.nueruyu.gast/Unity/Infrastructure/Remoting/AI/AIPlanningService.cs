using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gast.Application.AI;
using Gast.Application.AIPlanning;
using Gast.Lib.Gaia;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class AIPlanningService : IAIPlanningService
    {
        readonly IGaiaPlanningClient gaiaClient;
        readonly IToolRegistry toolRegistry;
        readonly IObjectiveRegistry objectiveRegistry;
        readonly PlanConverter planConverter;

        public AIPlanningService(
            IGaiaPlanningClient gaiaClient,
            IToolRegistry toolRegistry,
            IObjectiveRegistry objectiveRegistry,
            PlanConverter planConverter)
        {
            this.gaiaClient = gaiaClient;
            this.toolRegistry = toolRegistry;
            this.objectiveRegistry = objectiveRegistry;
            this.planConverter = planConverter;
        }

        public async Task<AIPlanningResult> GetObjectivesAsync(string instruction, CancellationToken cancellationToken)
        {
            try
            {
                var request = new CreateSessionRequest
                {
                    Instruction = instruction,
                    ToolDefinitions = toolRegistry.GetToolDefinitions().Select(ModelToDto).ToList(),
                    ObjectiveDefinitions = objectiveRegistry.GetObjectiveDefinitions().Select(ModelToDto).ToList()
                };
                var sessionDto = await gaiaClient.CreateSessionAsync(request, cancellationToken);

                while (sessionDto.Status == SessionStatus.WaitingForTool && sessionDto.ToolCalls != null && sessionDto.ToolCalls.Any())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var toolOutputs = await ExecuteToolsAsync(sessionDto.ToolCalls);
                    var submitRequest = new SubmitToolOutputsRequest { ToolOutputs = toolOutputs };
                    sessionDto = await gaiaClient.SubmitToolOutputsAsync(sessionDto.SessionId, submitRequest, cancellationToken);
                }

                if (sessionDto.Status == SessionStatus.Completed && sessionDto.Plan != null)
                {
                    var goals = planConverter.ToGoals(sessionDto.Plan);
                    return new AIPlanningResult(goals);
                }

                var errorMessage = !string.IsNullOrEmpty(sessionDto.ErrorMessage)
                    ? sessionDto.ErrorMessage
                    : "AI failed to generate a plan.";
                throw new AIPlanningException(errorMessage);
            }
            catch (GaiaTimeoutException ex)
            {
                Debug.LogError($"[AIAgentService] Gaia Timeout Error: {ex.Message}");
                throw new AIPlanningException("Request to AI server timed out.", ex);
            }
            catch (GaiaConnectionException ex)
            {
                Debug.LogError($"[AIAgentService] Gaia Connection Error: {ex.Message}");
                throw new AIPlanningException("Failed to connect to AI server.", ex);
            }
            catch (GaiaServerException ex)
            {
                Debug.LogError($"[AIAgentService] Gaia Server Error: {ex.StatusCode} - {ex.Message}");
                throw new AIPlanningException("AI server returned an error.", ex);
            }
        }

        async Task<List<ToolOutputDto>> ExecuteToolsAsync(List<ToolCallDto> toolCalls)
        {
            var tasks = toolCalls.Select(async call =>
            {
                var tool = toolRegistry.GetTool(call.FunctionName);
                var output = await tool.ExecuteAsync(call.Arguments);
                return new ToolOutputDto { ToolCallId = call.Id, Output = output };
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        ToolDefinitionDto ModelToDto(ToolDefinition model)
        {
            return new ToolDefinitionDto
            {
                Name = model.Name,
                Description = model.Description,
                Parameters = model.Parameters
            };
        }

        ObjectiveDefinitionDto ModelToDto(ObjectiveDefinition model)
        {
            return new ObjectiveDefinitionDto
            {
                Name = model.Name,
                Description = model.Description,
                Parameters = new(model.Parameters),
            };
        }
    }
}