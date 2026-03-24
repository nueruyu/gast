using System;
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

        public VitalsSystem(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);

                foreach (var character in characterRepository.GetAll())
                {
                    if (!character.Is(out BaseCharacter baseCharacter)) continue;

                    var status = baseCharacter.Status;
                    if (!status.IsAlive.Value) continue;

                    var newHunger = status.Hunger.Value - HungerDecreaseRate;
                    status.SetHunger(newHunger);

                    if (status.Hunger.Value <= 0)
                    {
                        status.SetHealth(status.Health.Value - StarvationDamage);
                    }
                }
            }
        }
    }
}
