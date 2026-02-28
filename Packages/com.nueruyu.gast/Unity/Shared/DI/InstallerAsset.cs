using Gast.Core.DI;
using UnityEngine;

namespace Gast.Unity.Shared.DI
{
    public abstract class InstallerAsset : ScriptableObject, IInstaller
    {
        public abstract void Install(IContainerBuilder builder);
    }
}