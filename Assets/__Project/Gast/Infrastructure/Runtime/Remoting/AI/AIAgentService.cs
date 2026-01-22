using Gast.Application.AI;
using Gast.Domain.AI;
using Gast.Domain.AI.Goals;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Infrastructure.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Gast.Infrastructure.Remoting.AI
{
    public class AIAgentService : IAIAgentService
    {
        static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        readonly AIServerSettings settings;
        readonly CharacterTypeRepository characterTypeRepository;
        readonly ItemRepository itemRepository;
        readonly HttpClient httpClient;
        readonly JsonSerializerSettings jsonSettings;

        public AIAgentService(AIServerSettings settings, CharacterTypeRepository characterTypeRepository, ItemRepository itemRepository)
        {
            this.settings = settings;
            this.characterTypeRepository = characterTypeRepository;
            this.itemRepository = itemRepository;

            httpClient = new HttpClient
            {
                Timeout = DefaultTimeout
            };

            jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Formatting = Formatting.None
            };
        }

        public async Task<AIAgentResult> GetGoalsAsync(string instruction, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(instruction))
            {
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.InvalidResponse,
                    "Instruction cannot be empty"));
            }

            if (string.IsNullOrWhiteSpace(settings.ServerUrl))
            {
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.ServiceUnavailable,
                    "AI server URL is not configured"));
            }

            var requestDto = CreatePlanRequest(instruction);
            string requestJson;

            try
            {
                requestJson = JsonConvert.SerializeObject(requestDto, jsonSettings);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"[AIAgentService] Failed to serialize request: {ex.Message}");
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.InvalidResponse,
                    "Failed to create request",
                    ex.Message));
            }

            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            try
            {
                using var response = await httpClient.PostAsync(settings.ServerUrl, content, cancellationToken);
                return await HandleResponseAsync(response);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("[AIAgentService] Request was cancelled");
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.Cancelled,
                    "Request was cancelled"));
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                Debug.LogWarning($"[AIAgentService] Request timed out: {ex.Message}");
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.Timeout,
                    "Request timed out",
                    $"Timeout after {DefaultTimeout.TotalSeconds} seconds"));
            }
            catch (HttpRequestException ex) when (IsConnectionError(ex))
            {
                Debug.LogWarning($"[AIAgentService] Connection error: {ex.Message}");
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.NetworkError,
                    "Failed to connect to AI server",
                    ex.Message));
            }
            catch (HttpRequestException ex)
            {
                Debug.LogError($"[AIAgentService] HTTP error: {ex.Message}");
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.NetworkError,
                    "Network error occurred",
                    ex.Message));
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AIAgentService] Unexpected error: {ex.Message}");
                Debug.LogException(ex);
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.ServerError,
                    "An unexpected error occurred",
                    ex.Message));
            }
        }

        async Task<AIAgentResult> HandleResponseAsync(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorCode = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => AIAgentErrorCode.InvalidResponse,
                    HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => AIAgentErrorCode.ServiceUnavailable,
                    HttpStatusCode.NotFound => AIAgentErrorCode.ServiceUnavailable,
                    HttpStatusCode.RequestTimeout or HttpStatusCode.GatewayTimeout => AIAgentErrorCode.Timeout,
                    HttpStatusCode.ServiceUnavailable => AIAgentErrorCode.ServiceUnavailable,
                    >= HttpStatusCode.InternalServerError => AIAgentErrorCode.ServerError,
                    _ => AIAgentErrorCode.NetworkError
                };

                Debug.LogWarning($"[AIAgentService] Server returned {(int)response.StatusCode}: {responseBody}");
                return AIAgentResult.Failure(new AIAgentError(
                    errorCode,
                    $"Server returned {(int)response.StatusCode} {response.ReasonPhrase}",
                    responseBody));
            }

            PlanResponseDto plan;
            try
            {
                plan = JsonConvert.DeserializeObject<PlanResponseDto>(responseBody, jsonSettings);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"[AIAgentService] Failed to parse response: {ex.Message}");
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.InvalidResponse,
                    "Failed to parse server response",
                    ex.Message));
            }

            if (plan?.Objectives == null)
            {
                Debug.LogWarning("[AIAgentService] Server returned empty or invalid response");
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.InvalidResponse,
                    "Plan contained no objectives"));
            }

            Debug.Log($"[AIAgentService] AI Thought: {plan.Thought}");
            var goals = ConvertObjectivesToGoals(plan.Objectives);
            Debug.Log($"[AIAgentService] Successfully parsed {goals.Count} goals");
            return AIAgentResult.Success(goals);
        }

        static bool IsConnectionError(HttpRequestException ex)
        {
            return ex.InnerException is SocketException ||
                   ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
                   ex.Message.Contains("refused", StringComparison.OrdinalIgnoreCase);
        }

        PlanRequestDto CreatePlanRequest(string instruction)
        {
            var context = new AgentContextDto
            {
                AgentCharacterType = "soldier", // TODO: Should be dynamic based on the actual agent
                MissionObjective = instruction
            };

            var characterTypes = characterTypeRepository.GetAllDefinitions()
                .Select(def => new CharacterTypeDto
                {
                    TypeId = def.TypeId.ToString(),
                    DisplayName = def.DisplayName,
                    ThreatLevel = 5 // Default value, ideally added to ScriptableObject
                })
                .ToList();

            var items = itemRepository.GetAllDefinitions()
                .Select(def => new ItemDto
                {
                    ItemId = def.Id.ToString(),
                    Name = def.Name,
                    Utility = 5 // Default value
                })
                .ToList();

            var definitions = new StaticDefinitionsDto
            {
                CharacterTypes = characterTypes,
                ItemTypes = items
            };

            var availableGoals = new List<GoalDefinitionDto>
            {
                new GoalDefinitionDto
                {
                    Name = "DefeatCharacter",
                    Description = "Eliminate a specific number of enemies of a certain type.",
                    Parameters = new Dictionary<string, object>
                    {
                        { "character_type_id", "string" },
                        { "quantity", "integer" }
                    }
                },
                new GoalDefinitionDto
                {
                    Name = "AcquireItem",
                    Description = "Collect a specific number of items.",
                    Parameters = new Dictionary<string, object>
                    {
                        { "item_id", "string" },
                        { "quantity", "integer" }
                    }
                }
            };

            return new PlanRequestDto
            {
                Context = context,
                Definitions = definitions,
                AvailableGoals = availableGoals
            };
        }

        List<IGoal> ConvertObjectivesToGoals(List<ObjectiveDto> objectives)
        {
            var goals = new List<IGoal>();

            foreach (var objective in objectives)
            {
                try
                {
                    switch (objective)
                    {
                        case DefeatCharacterObjectiveDto defeatObj:
                            var charTypeId = CharacterTypeId.FromString(defeatObj.Parameters.CharacterTypeId);
                            goals.Add(new DefeatCharacterGoal(charTypeId, defeatObj.Parameters.Quantity));
                            break;

                        case AcquireItemObjectiveDto itemObj:
                            if (Guid.TryParse(itemObj.Parameters.ItemId, out var itemGuid))
                            {
                                goals.Add(new AcquireItemGoal(ItemId.FromGuid(itemGuid), itemObj.Parameters.Quantity));
                            }
                            else
                            {
                                Debug.LogWarning($"[AIAgentService] Invalid Item GUID: {itemObj.Parameters.ItemId}");
                            }
                            break;

                        case UnknownObjectiveDto unknown:
                            Debug.LogWarning($"[AIAgentService] Skipping unknown objective type: {unknown.Type}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[AIAgentService] Failed to convert objective '{objective.Type}': {ex.Message}");
                }
            }

            return goals;
        }
    }
}
