using Gast.Lib.AI;

namespace Cryst.Features.Stories
{
    /// <summary>
    /// Provides a fixed <see cref="AIDomain{TActorContext,TWorldState}"/> to a
    /// <see cref="DomainProcess{TActorContext,TWorldState}"/>. Created per-story by <see cref="StorySystem"/>.
    /// </summary>
    public class StorytellerBrain : IDomainProvider<StoryActorContext, StoryWorldState>
    {
        readonly AIDomain<StoryActorContext, StoryWorldState> domain;

        public StorytellerBrain(AIDomain<StoryActorContext, StoryWorldState> domain)
        {
            this.domain = domain;
        }

        public AIDomain<StoryActorContext, StoryWorldState> GetDomain() => domain;
    }
}
