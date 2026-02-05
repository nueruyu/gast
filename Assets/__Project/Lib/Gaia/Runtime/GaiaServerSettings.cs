using UnityEngine;

namespace Gast.Lib.Gaia
{
    [CreateAssetMenu(fileName = "GaiaServerSettings", menuName = "Gast/Gaia/Server Settings")]
    public class GaiaServerSettings : ScriptableObject
    {
        [SerializeField]
        string baseUrl = "http://localhost:8000";

        public string BaseUrl => baseUrl;
    }
}
