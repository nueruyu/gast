using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI
{
    public class DomainRunner
    {
        readonly List<IDomainProcess> processes = new();

        public void Register(IDomainProcess process)
        {
            processes.Add(process);
        }

        public UniTask RunAsync(CancellationToken cancellationToken)
        {
            var tasks = new List<UniTask>
            {
                StateUpdateLoop(cancellationToken)
            };
            tasks.AddRange(processes.Select(d => d.RunAsync(cancellationToken)));
            return UniTask.WhenAll(tasks);
        }

        async UniTask StateUpdateLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var domain in processes)
                    domain.UpdateState();
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }
    }
}