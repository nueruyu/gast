using System;

namespace Gast.Domain.AI.Attributes
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
}