using System.Threading;
using System.Threading.Tasks;
using Gast.Application.AIPlanning;
using Gast.Lib.Gaia;
using Gast.Unity.Infrastructure.Stories;
using Newtonsoft.Json;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class MockStoryGenerationService : IStoryGenerationService
    {
        readonly JsonSerializerSettings definitionSettings;
        readonly StoryBlueprintMapper mapper;
        readonly MockStoryGenerationSettings settings;

        public MockStoryGenerationService(MockStoryGenerationSettings settings, StoryBlueprintMapper mapper)
        {
            this.settings = settings;
            this.mapper = mapper;
            definitionSettings = GaiaJsonSettings.Create();
        }

        public Task<StoryBlueprint> GenerateStoryAsync(string instruction, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.MockStoryJson))
                return Task.FromResult<StoryBlueprint>(null);

            var storyDef =
                JsonConvert.DeserializeObject<StoryDefinitionDto>(settings.MockStoryJson, definitionSettings);
            return Task.FromResult(mapper.Map(storyDef));
        }
    }
}