using System;

namespace Gast.Application.AI.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public class ToolParameterAttribute : Attribute
    {
        public string Description { get; }

        public ToolParameterAttribute(string description)
        {
            Description = description;
        }
    }
}