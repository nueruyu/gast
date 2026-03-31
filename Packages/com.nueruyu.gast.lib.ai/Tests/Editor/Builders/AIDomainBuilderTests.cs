using Gast.Lib.AI.Builders;
using Gast.Lib.AI.Tasks;
using NUnit.Framework;

namespace Gast.Lib.AI.Tests.Editor.Builders
{
    [TestFixture]
    public class AIDomainBuilderTests
    {
        [Test]
        public void Build_WithCircularDependency_ShouldNotThrow()
        {
            var builder = new AIDomainBuilder<TestActorContext, TestWorldState>();
            var taskA = builder.DefineCompound("TaskA");
            var taskB = builder.DefineCompound("TaskB");

            taskA.AddMethod("GoToB").Do(taskB);
            taskB.AddMethod("GoToA").Do(taskA);

            AIDomain<TestActorContext, TestWorldState> domain = null;
            Assert.DoesNotThrow(() =>
            {
                domain = builder.Build("TaskA");
            });

            Assert.IsNotNull(domain);
            var rootTask = domain.RootTask as CompoundTask<TestActorContext, TestWorldState>;
            Assert.IsNotNull(rootTask);
        }
    }
}
