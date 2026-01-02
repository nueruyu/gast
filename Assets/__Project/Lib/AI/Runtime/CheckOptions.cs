using UnityEngine;

namespace Gast.Lib.AI
{
    public struct CheckOptions
    {
        /// <summary>
        /// Remaining search depth.
        /// -1: Unlimited (Deep) - Perform simulation
        ///  0: Shallow - Check current state only
        /// >0: Limited depth
        /// </summary>
        public int MaxDepth;

        public static CheckOptions Deep => new CheckOptions { MaxDepth = -1 };
        public static CheckOptions Shallow => new CheckOptions { MaxDepth = 0 };

        public CheckOptions StepDown()
        {
            if (MaxDepth == -1) return this;
            return new CheckOptions { MaxDepth = Mathf.Max(0, MaxDepth - 1) };
        }

        public static CheckOptions Resolve(CheckOptions parentOptions, int localLimit)
        {
            if (parentOptions.MaxDepth == 0) return parentOptions;
            if (localLimit != -1)
            {
                if (parentOptions.MaxDepth == -1) return new CheckOptions { MaxDepth = localLimit };
                return new CheckOptions { MaxDepth = Mathf.Min(parentOptions.MaxDepth, localLimit) };
            }
            return parentOptions;
        }
    }
}