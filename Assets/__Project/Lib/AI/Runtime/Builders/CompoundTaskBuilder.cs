using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class CompoundTaskBuilder<TWorldState> where TWorldState : struct
    {
        readonly DomainBuilder<TWorldState> domainBuilder;
        readonly string taskName;
        readonly bool isRoot;
        readonly List<Method<TWorldState>> methods = new();

        int localDepthLimit = -1;
        bool runOnBackground = false;
        IMethodSelector<TWorldState> selector;

        internal CompoundTaskBuilder(
            DomainBuilder<TWorldState> domainBuilder,
            string taskName,
            bool isRoot = false)
        {
            this.domainBuilder = domainBuilder;
            this.taskName = taskName;
            this.isRoot = isRoot;
        }

        public CompoundTaskBuilder<TWorldState> CheckDepth(int depth)
        {
            localDepthLimit = depth;
            return this;
        }

        public CompoundTaskBuilder<TWorldState> RunOnBackground(bool enable = true)
        {
            runOnBackground = enable;
            return this;
        }

        public CompoundTaskBuilder<TWorldState> UseSelector(IMethodSelector<TWorldState> selector)
        {
            this.selector = selector;
            return this;
        }

        public MethodBuilder<TWorldState> AddMethod(string methodName)
        {
            return new MethodBuilder<TWorldState>(this, methodName);
        }

        internal CompoundTaskBuilder<TWorldState> CompleteMethod(Method<TWorldState> method)
        {
            methods.Add(method);
            return this;
        }

        public DomainBuilder<TWorldState> End()
        {
            var compoundTask = new CompoundTask<TWorldState>(taskName)
            {
                LocalDepthLimit = localDepthLimit,
                RunPlanningOnBackground = runOnBackground
            };

            if (selector != null)
            {
                compoundTask.Selector = selector;
            }

            foreach (var method in methods)
            {
                compoundTask.Methods.Add(method);
            }

            return domainBuilder.CompleteCompound(taskName, compoundTask, isRoot);
        }

        internal DomainBuilder<TWorldState> DomainBuilder => domainBuilder;
    }
}