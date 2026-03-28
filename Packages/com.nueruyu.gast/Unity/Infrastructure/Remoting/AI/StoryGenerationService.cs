using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gast.Application.AIPlanning;
using Gast.Domain.Characters;
using Gast.Lib.Gaia;
using Gast.Unity.Infrastructure.Stories;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class StoryGenerationService : IStoryGenerationService
    {
        readonly IGaiaPlanningClient gaiaClient;
        readonly ICharacterRepository characterRepository;
        readonly StoryBlueprintParser parser;

        public StoryGenerationService(
            IGaiaPlanningClient gaiaClient,
            ICharacterRepository characterRepository,
            StoryBlueprintParser parser)
        {
            this.gaiaClient = gaiaClient;
            this.characterRepository = characterRepository;
            this.parser = parser;
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
            if (string.IsNullOrWhiteSpace(response?.StoryJson))
                return null;

            return parser.Parse(response.StoryJson);
        }
    }
}
