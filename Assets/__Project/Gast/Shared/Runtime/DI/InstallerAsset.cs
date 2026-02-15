using Gast.Core.DI;
using UnityEngine;

namespace Gast.Shared.DI
{
    public abstract class InstallerAsset : ScriptableObject, IInstaller
    {
        public abstract void Install(IContainerBuilder builder);
    }
}