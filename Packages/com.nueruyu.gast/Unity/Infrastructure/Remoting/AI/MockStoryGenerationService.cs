using System.Threading;
using System.Threading.Tasks;
using Gast.Application.AIPlanning;
using Gast.Lib.Gaia;
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
            var storyDef =
                JsonConvert.DeserializeObject<StoryDefinitionDto>(settings.MockStoryJson, definitionSettings);
            return Task.FromResult(mapper.Map(storyDef));
        }
    }
}