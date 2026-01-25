using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gast.Application.AI;
using Gast.Application.AI.Tools;
using Gast.Application.AIPlanning;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Gast.Infrastructure.Remoting.AI
{
    public class AIPlanningService : IAIPlanningService
    {
        readonly AIServerSettings settings;
        readonly IToolRegistry toolRegistry;
        readonly IObjectiveRegistry objectiveRegistry;
        readonly PlanConverter planConverter;
        readonly HttpClient httpClient;
        readonly JsonSerializerSettings serializerSettings;

        public AIPlanningService(
            AIServerSettings settings,
            IToolRegistry toolRegistry,
            IObjectiveRegistry objectiveRegistry,
            GameInfoTools gameInfoTools,
            PlanConverter planConverter)
        {
            this.settings = settings;
            this.toolRegistry = toolRegistry;
            this.objectiveRegistry = objectiveRegistry;
            this.planConverter = planConverter;

            httpClient = new HttpClient { Timeout = System.TimeSpan.FromSeconds(60) };
            serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            // Register the tools
            this.toolRegistry.RegisterToolSet(gameInfoTools);
        }

        public async Task<AIPlanningResult> GetObjectivesAsync(string instruction, CancellationToken cancellationToken)
        {
            try
            {
                var sessionDto = await CreateSessionAsync(instruction, cancellationToken);

                while (sessionDto.Status == SessionStatus.WaitingForTool && sessionDto.ToolCalls != null && sessionDto.ToolCalls.Any())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var toolOutputs = await ExecuteToolsAsync(sessionDto.ToolCalls);
                    sessionDto = await SubmitToolOutputsAsync(sessionDto.SessionId, toolOutputs, cancellationToken);
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
            catch (HttpRequestException ex)
            {
                Debug.LogError($"[AIAgentService] HTTP Request Error: {ex.Message}");
                return AIPlanningResult.Failure(new AIPlanningError(AIPlanningErrorCode.NetworkError, "Failed to connect to AI server."));
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

        async Task<PlanningSessionDto> CreateSessionAsync(string instruction, CancellationToken cancellationToken)
        {
            var request = new CreateSessionRequest
            {
                Instruction = instruction,
                ToolDefinitions = toolRegistry.GetToolDefinitions().Select(ModelToDto).ToList(),
                ObjectiveDefinitions = objectiveRegistry.GetObjectiveDefinitions().Select(ModelToDto).ToList()
            };

            var requestJson = JsonConvert.SerializeObject(request, serializerSettings);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{settings.ServerUrl}/planning/request", content, cancellationToken);
            return await ProcessResponseAsync(response);
        }

        async Task<PlanningSessionDto> SubmitToolOutputsAsync(string sessionId, List<ToolOutputDto> outputs, CancellationToken cancellationToken)
        {
            var request = new SubmitToolOutputsRequest { ToolOutputs = outputs };
            var requestJson = JsonConvert.SerializeObject(request, serializerSettings);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var url = $"{settings.ServerUrl}/planning/respond/{sessionId}";
            var response = await httpClient.PostAsync(url, content, cancellationToken);
            return await ProcessResponseAsync(response);
        }

        async Task<List<ToolOutputDto>> ExecuteToolsAsync(List<ToolCallDto> toolCalls)
        {
            var tasks = toolCalls.Select(async call =>
            {
                var output = await toolRegistry.ExecuteAsync(call.FunctionName, call.Arguments);
                return new ToolOutputDto { ToolCallId = call.Id, Output = output };
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        async Task<PlanningSessionDto> ProcessResponseAsync(HttpResponseMessage response)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<PlanningSessionDto>(responseJson, serializerSettings);
        }

        // --- Mappers ---
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
                Parameters = model.Parameters
            };
        }
    }
}