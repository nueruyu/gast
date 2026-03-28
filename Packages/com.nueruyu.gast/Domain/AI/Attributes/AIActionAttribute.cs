using System;

namespace Gast.Domain.AI.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AIActionAttribute : Attribute
    {
        public string Name { get; }

        public AIActionAttribute(string name)
        {
            Name = name;
        }
    }
}
