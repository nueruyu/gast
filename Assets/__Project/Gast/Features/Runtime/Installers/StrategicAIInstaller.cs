using Gast.Core.DI;
using Gast.Features.AI.Strategic;
using Gast.Features.AI.Strategic.Actions;

namespace Gast.Features.Installers
{
    class StrategicAIInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<StrategicDomain>(Lifetime.Transient);
            builder.Register<ClearTargetAction>(Lifetime.Transient);
            builder.Register<FindTargetForGoalAction>(Lifetime.Transient);
            builder.Register<SelectThreatAction>(Lifetime.Transient);
        }
    }
}
