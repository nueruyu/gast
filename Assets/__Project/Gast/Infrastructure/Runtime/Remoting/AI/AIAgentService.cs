using Gast.Application.Services;
using Gast.Domain.AI;
using Gast.Domain.AI.Goals;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Infrastructure.Repositories;
using Newtonsoft.Json;
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

        public AIAgentService(AIServerSettings settings, CharacterTypeRepository characterTypeRepository, ItemRepository itemRepository)
        {
            this.settings = settings;
            this.characterTypeRepository = characterTypeRepository;
            this.itemRepository = itemRepository;

            httpClient = new HttpClient
            {
                Timeout = DefaultTimeout
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

            var requestDto = CreateRequestDto(instruction);
            string requestJson;

            try
            {
                requestJson = JsonConvert.SerializeObject(new { input = requestDto });
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

            AIResponse responseDto;
            try
            {
                responseDto = JsonConvert.DeserializeObject<AIResponse>(responseBody);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"[AIAgentService] Failed to parse response: {ex.Message}");
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.InvalidResponse,
                    "Failed to parse server response",
                    ex.Message));
            }

            if (responseDto?.Output?.Goals == null)
            {
                Debug.LogWarning("[AIAgentService] Server returned empty or invalid response");
                return AIAgentResult.Failure(new AIAgentError(
                    AIAgentErrorCode.InvalidResponse,
                    "Server returned empty or invalid response"));
            }

            var goals = ParseGoals(responseDto);
            Debug.Log($"[AIAgentService] Successfully parsed {goals.Count} goals");
            return AIAgentResult.Success(goals);
        }

        static bool IsConnectionError(HttpRequestException ex)
        {
            return ex.InnerException is SocketException ||
                   ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
                   ex.Message.Contains("refused", StringComparison.OrdinalIgnoreCase);
        }

        AIRequest CreateRequestDto(string instruction)
        {
            var characterTypes = characterTypeRepository.GetAllDefinitions()
                .Select(def => new CharacterTypeDto { Id = def.TypeId.ToString(), Name = def.DisplayName })
                .ToList();

            var items = itemRepository.GetAllDefinitions()
                .Select(def => new ItemDto { Id = def.Id.ToString(), Name = def.Name })
                .ToList();

            return new AIRequest
            {
                Instruction = instruction,
                CharacterTypes = characterTypes,
                Items = items
            };
        }

        List<IGoal> ParseGoals(AIResponse response)
        {
            var goals = new List<IGoal>();

            foreach (var goalDto in response.Output.Goals)
            {
                try
                {
                    switch (goalDto.Type)
                    {
                        case "DefeatCharacter":
                            var typeId = CharacterTypeId.FromString(goalDto.CharacterTypeId);
                            goals.Add(new DefeatCharacterGoal(typeId, goalDto.Quantity));
                            break;

                        case "AcquireItem":
                            var itemId = ItemId.FromGuid(Guid.Parse(goalDto.ItemId));
                            goals.Add(new AcquireItemGoal(itemId, goalDto.Quantity));
                            break;

                        default:
                            Debug.LogWarning($"[AIAgentService] Unknown goal type: {goalDto.Type}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[AIAgentService] Failed to parse goal '{goalDto.Type}': {ex.Message}");
                }
            }

            return goals;
        }
    }
}