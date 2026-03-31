using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gast.Application.AIPlanning;
using Gast.Domain.Characters;
using Gast.Lib.Gaia;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class StoryGenerationService : IStoryGenerationService
    {
        readonly ICharacterRepository characterRepository;
        readonly IGaiaPlanningClient gaiaClient;
        readonly StoryBlueprintMapper mapper;

        public StoryGenerationService(
            IGaiaPlanningClient gaiaClient,
            ICharacterRepository characterRepository,
            StoryBlueprintMapper mapper)
        {
            this.gaiaClient = gaiaClient;
            this.characterRepository = characterRepository;
            this.mapper = mapper;
        }

        public async Task<StoryBlueprint> GenerateStoryAsync(string instruction, CancellationToken cancellationToken)
        {
            var characters = characterRepository.GetAll()
                .Select(c => new CharacterContextDto { Id = c.Id.ToString() })
                .ToList();

            var request = new CreateStoryRequest
            {
                Instruction = instruction,
                AvailableCharacters = characters
            };

            var response = await gaiaClient.CreateStoryAsync(request, cancellationToken);

            return mapper.Map(response.Story);
        }
    }
}