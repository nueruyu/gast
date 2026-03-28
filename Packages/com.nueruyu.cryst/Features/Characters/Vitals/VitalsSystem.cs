using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cryst.Application.Characters;
using Cryst.Domain.Characters;
using Cryst.Infrastructure.Characters;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Characters;

namespace Cryst.Features.Characters.Vitals
{
    public class VitalsSystem : ILifecycleTask
    {
        readonly ICharacterRepository characterRepository;
        readonly ICharacterDeathService characterDeathService;
        readonly VitalsSettings settings;
        readonly List<ICharacter> characterBuffer = new();

        public VitalsSystem(
            ICharacterRepository characterRepository,
            ICharacterDeathService characterDeathService,
            VitalsSettings settings)
        {
            this.characterRepository = characterRepository;
            this.characterDeathService = characterDeathService;
            this.settings = settings;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);

                characterBuffer.Clear();
                characterBuffer.AddRange(characterRepository.GetAll());

                foreach (var character in characterBuffer)
                {
                    if (!character.Is(out BaseCharacter baseCharacter)) continue;

                    var status = baseCharacter.Status;
                    if (!status.IsAlive.Value) continue;

                    status.SetHunger(status.Hunger.Value - settings.HungerDecreaseRate);

                    if (status.Hunger.Value <= 0)
                    {
                        if (baseCharacter.ApplyPassiveDamage(settings.StarvationDamage))
                        {
                            characterDeathService.Kill(character, null);
                        }
                    }
                }
            }
        }
    }
}
