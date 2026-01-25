using System;

namespace Gast.Domain.AI.Attributes
{
    /// <summary>
    /// Property level attribute to define parameters for the objective.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class AIObjectiveParameterAttribute : Attribute
    {
        public string Description { get; }
        public string TypeName { get; }

        public AIObjectiveParameterAttribute(string description, string typeName = null)
        {
            Description = description;
            TypeName = typeName;
        }
    }
}