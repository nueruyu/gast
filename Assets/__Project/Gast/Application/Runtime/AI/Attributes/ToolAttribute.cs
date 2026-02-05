using System;

namespace Gast.Application.AI.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ToolAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public ToolAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}