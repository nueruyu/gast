using UnityEngine;

namespace Gast.Core.DI
{
    public abstract class InstallerAsset : ScriptableObject, IInstaller
    {
        public abstract void Install(IContainerBuilder builder);
    }
}
