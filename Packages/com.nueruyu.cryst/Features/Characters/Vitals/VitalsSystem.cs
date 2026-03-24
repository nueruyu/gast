using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Characters;

namespace Cryst.Features.Characters.Vitals
{
    public class VitalsSystem : ILifecycleTask
    {
        const float HungerDecreaseRate = 0.5f; // per second
        const float StarvationDamage = 2.0f;   // per second

        readonly ICharacterRepository characterRepository;
        readonly List<ICharacter> characterBuffer = new();

        public VitalsSystem(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
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

                    status.SetHunger(status.Hunger.Value - HungerDecreaseRate);

                    if (status.Hunger.Value <= 0)
                    {
                        baseCharacter.ApplyPassiveDamage(StarvationDamage);
                    }
                }
            }
        }
    }
}
