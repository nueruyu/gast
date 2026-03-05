namespace Gast.Lib.AI.Builders
{
    // A base token to represent a task in the AI domain.
    public abstract class TaskToken
    {
        public string Name { get; }
        protected TaskToken(string name) { Name = name; }
    }

    // Represents a primitive action without parameters.
    public sealed class PrimitiveTaskToken : TaskToken
    {
        public PrimitiveTaskToken(string name) : base(name) { }
    }

    // Represents a primitive action with parameters.
    public sealed class ParametricTaskToken<TParam> : TaskToken
    {
        public ParametricTaskToken(string name) : base(name) { }
    }

    // Represents a compound task.
    public sealed class CompoundTaskToken : TaskToken
    {
        public CompoundTaskToken(string name) : base(name) { }
    }
}
