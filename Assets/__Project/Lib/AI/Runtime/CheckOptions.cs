using UnityEngine;

namespace Gast.Lib.AI
{
    public readonly struct CheckOptions
    {
        /// <summary>
        /// Remaining search depth.
        /// -1: Unlimited (Deep) - Perform simulation
        ///  0: Shallow - Check current state only
        /// >0: Limited depth
        /// </summary>
        public int MaxDepth { get; }

        public static CheckOptions Deep => new(-1);
        public static CheckOptions Shallow => new(0);

        public CheckOptions(int maxDepth)
        {
            MaxDepth = maxDepth;
        }

        public readonly CheckOptions StepDown()
        {
            if (MaxDepth < 0)
                return this;

            return new CheckOptions(Mathf.Max(0, MaxDepth - 1));
        }

        public static CheckOptions Resolve(CheckOptions parentOptions, int localLimit)
        {
            if (parentOptions.MaxDepth == 0)
                return parentOptions;

            if (localLimit >= 0)
            {
                if (parentOptions.MaxDepth < 0)
                {
                    return new CheckOptions(localLimit);
                }

                return new CheckOptions(Mathf.Min(parentOptions.MaxDepth, localLimit));
            }

            return parentOptions;
        }
    }
}