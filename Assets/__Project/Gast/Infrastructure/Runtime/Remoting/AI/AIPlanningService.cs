using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gast.Application.AI;
using Gast.Application.AI.Tools;
using Gast.Application.AIPlanning;
using Gast.Lib.Gaia;
using Gast.Lib.Gaia.Dto;
using UnityEngine;

namespace Gast.Infrastructure.Remoting.AI
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
            GameInfoTools gameInfoTools,
            PlanConverter planConverter)
        {
            this.gaiaClient = gaiaClient;
            this.toolRegistry = toolRegistry;
            this.objectiveRegistry = objectiveRegistry;
            this.planConverter = planConverter;

            this.toolRegistry.RegisterToolSet(gameInfoTools);
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
                    return AIPlanningResult.Success(goals);
                }

                var errorMessage = !string.IsNullOrEmpty(sessionDto.ErrorMessage)
                    ? sessionDto.ErrorMessage
                    : "AI failed to generate a plan.";
                return AIPlanningResult.Failure(new AIPlanningError(AIPlanningErrorCode.ServerError, errorMessage));
            }
            catch (GaiaTimeoutException ex)
            {
                Debug.LogError($"[AIAgentService] Gaia Timeout Error: {ex.Message}");
                return AIPlanningResult.Failure(new AIPlanningError(AIPlanningErrorCode.Timeout, "Request to AI server timed out."));
            }
            catch (GaiaConnectionException ex)
            {
                Debug.LogError($"[AIAgentService] Gaia Connection Error: {ex.Message}");
                return AIPlanningResult.Failure(new AIPlanningError(AIPlanningErrorCode.NetworkError, "Failed to connect to AI server."));
            }
            catch (GaiaServerException ex)
            {
                Debug.LogError($"[AIAgentService] Gaia Server Error: {ex.StatusCode} - {ex.Message}");
                return AIPlanningResult.Failure(new AIPlanningError(AIPlanningErrorCode.ServerError, "AI server returned an error.", ex.ResponseContent));
            }
            catch (System.OperationCanceledException)
            {
                return AIPlanningResult.Failure(new AIPlanningError(AIPlanningErrorCode.Cancelled, "Operation was cancelled."));
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return AIPlanningResult.Failure(new AIPlanningError(AIPlanningErrorCode.InvalidResponse, "An unexpected error occurred.", ex.Message));
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