using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Npcs;
using Gast.Domain.Npcs.Goals;
using Gast.Infrastructure.Remoting;
using Gast.Infrastructure.Repositories;
using Gast.Infrastructure.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Gast.Infrastructure.Services
{
    public class AIAgentService : IAIAgentService
    {
        readonly AIServerSettings settings;
        readonly CharacterTypeRepository characterTypeRepository;
        readonly ItemRepository itemRepository;
        static readonly HttpClient HttpClient = new HttpClient();

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
                var httpResponse = await HttpClient.PostAsync(settings.ServerUrl, content, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();

                var responseJson = await httpResponse.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<AiResponse>(responseJson);

                return ParseGoals(responseDto);
            }
            catch (Exception e)
            {
                Debug.LogError($"[AiServerClient] Failed to get goals: {e.Message}");
                return new List<IGoal>();
            }
        }

        AiRequest CreateRequestDto(string instruction)
        {
            var characterTypes = characterTypeRepository.GetAllDefinitions()
                .Select(def => new CharacterTypeDto { Id = def.TypeId.ToString(), Name = def.DisplayName })
                .ToList();

            var items = itemRepository.GetAllDefinitions()
                .Select(def => new ItemDto { Id = def.Id.ToString(), Name = def.Name })
                .ToList();

            return new AiRequest
            {
                Instruction = instruction,
                CharacterTypes = characterTypes,
                Items = items
            };
        }

        List<IGoal> ParseGoals(AiResponse response)
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
                catch (Exception e)
                {
                    Debug.LogWarning($"[AiServerClient] Failed to parse a goal: {e.Message}");
                }
            }
            return goals;
        }
    }
}