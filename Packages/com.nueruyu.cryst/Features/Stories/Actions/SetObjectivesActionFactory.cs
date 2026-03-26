using System.Collections.Generic;
using Gast.Domain.Characters;

namespace Cryst.Features.Stories.Actions
{
    public class SetObjectivesActionFactory : StoryActionFactory<SetObjectivesActionFactory.Params>
    {
        public override string ActionName => "SetObjectives";

        protected override SetObjectivesAction Create(Params parameters)
            => new SetObjectivesAction(parameters.Assignments);

        public class Params
        {
            public List<SetObjectivesAction.Assignment> Assignments { get; set; }
        }
    }
}
