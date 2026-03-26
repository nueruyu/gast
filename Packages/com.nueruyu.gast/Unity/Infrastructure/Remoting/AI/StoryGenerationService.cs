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
        readonly IGaiaPlanningClient gaiaClient;
        readonly ICharacterRepository characterRepository;

        public StoryGenerationService(IGaiaPlanningClient gaiaClient, ICharacterRepository characterRepository)
        {
            this.gaiaClient = gaiaClient;
            this.characterRepository = characterRepository;
        }

        public async Task<string> GenerateStoryAsync(string instruction, CancellationToken cancellationToken)
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
            return response?.StoryJson;
        }
    }
}
