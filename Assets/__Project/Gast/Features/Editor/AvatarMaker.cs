using UnityEngine;
using UnityEditor;
using System.IO;

namespace Gast.Features.Editor
{
    public class AvatarMaskMaker : MonoBehaviour
    {
        [MenuItem("Gast/Tools/Create Avatar Mask")]
        static void CreateUpperBodyMask()
        {
            var rootObj = Selection.activeGameObject;

            if (rootObj == null)
            {
                Debug.LogError("Select a GameObject");
                return;
            }

            var mask = new AvatarMask();
            mask.AddTransformPath(rootObj.transform, true);

            var path = $"Assets/{rootObj.name}_Mask.mask";

            if (File.Exists(path))
                AssetDatabase.DeleteAsset(path);

            AssetDatabase.CreateAsset(mask, path);
            AssetDatabase.SaveAssets();

            Selection.activeObject = mask;
        }
    }
}