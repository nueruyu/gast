using System;

namespace Gast.Application.AI.Tools
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AIToolAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public AIToolAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public class AIToolParameterAttribute : Attribute
    {
        public string Description { get; }

        public AIToolParameterAttribute(string description)
        {
            Description = description;
        }
    }
}
