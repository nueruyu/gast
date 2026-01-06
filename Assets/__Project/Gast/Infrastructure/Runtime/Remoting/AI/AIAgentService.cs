using Cysharp.Threading.Tasks;
using Gast.Api.AI;
using Gast.Api.AI.Goals;
using Gast.Application.Services;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Infrastructure.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Gast.Infrastructure.Remoting.AI
{
    public class AIAgentService : IAIAgentService
    {
        readonly AIServerSettings settings;
        readonly CharacterTypeRepository characterTypeRepository;
        readonly ItemRepository itemRepository;
        readonly HttpClient httpClient = new();

        public AIAgentService(AIServerSettings settings, CharacterTypeRepository characterTypeRepository, ItemRepository itemRepository)
        {
            this.settings = settings;
            this.characterTypeRepository = characterTypeRepository;
            this.itemRepository = itemRepository;
        }

        public async Task<List<IGoal>> GetGoalsAsync(string instruction, CancellationToken cancellationToken = default)
        {
            var requestDto = CreateRequestDto(instruction);
            var requestJson = JsonConvert.SerializeObject(new { input = requestDto });

            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            try
            {
                var httpResponse = await httpClient.PostAsync(settings.ServerUrl, content, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();

                var responseJson = await httpResponse.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<AIResponse>(responseJson);

                return ParseGoals(responseDto);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AiServerClient] Failed to get goals: {ex.Message}");
                Debug.LogException(ex);
                return new List<IGoal>();
            }
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
            if (response?.Output?.Goals == null)
                return goals;

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
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[AiServerClient] Failed to parse a goal: {ex.Message}");
                    Debug.LogException(ex);
                }
            }
            return goals;
        }
    }
}