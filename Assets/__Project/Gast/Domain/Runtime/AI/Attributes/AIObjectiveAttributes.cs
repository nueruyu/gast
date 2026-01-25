using System;

namespace Gast.Domain.AI
{
    /// <summary>
    /// Class level attribute to define an AI Goal/Objective.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AIObjectiveAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public AIObjectiveAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    /// <summary>
    /// Property level attribute to define parameters for the objective.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class AIObjectiveParameterAttribute : Attribute
    {
        public string Description { get; }

        public AIObjectiveParameterAttribute(string description)
        {
            Description = description;
        }
    }
}
