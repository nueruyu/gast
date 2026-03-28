using Cryst.Domain.Characters;
using Gast.Core.Events;
using Gast.Domain.Characters;
using Gast.Domain.Loot;

namespace Cryst.Application.Characters
{
    public class CharacterDeathService : ICharacterDeathService
    {
        readonly IDomainEventPublisher eventPublisher;
        readonly ICharacterBrainManager brainManager;
        readonly ICharacterTypeRepository typeRepository;

        public CharacterDeathService(
            IDomainEventPublisher eventPublisher,
            ICharacterBrainManager brainManager,
            ICharacterTypeRepository typeRepository)
        {
            this.eventPublisher = eventPublisher;
            this.brainManager = brainManager;
            this.typeRepository = typeRepository;
        }

        public void Kill(ICharacter character, CharacterId? attackerId)
        {
            var baseCharacter = character.As<BaseCharacter>();

            baseCharacter.Status.SetHealth(0);
            baseCharacter.Die();

            brainManager.DetachBrain(character.Id);

            eventPublisher.Publish(new CharacterDefeatedEvent(character, attackerId));

            var typeDefinition = typeRepository.Get(baseCharacter.TypeId);
            if (typeDefinition.TryGetSettings<ILootTable>(out var lootTable))
            {
                eventPublisher.Publish(new LootPotentialDropEvent(lootTable, baseCharacter.Body.Position));
            }
        }
    }
}
