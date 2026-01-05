using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    [CreateAssetMenu(fileName = "AIServerSettings", menuName = "Gast/AI/Server Settings")]
    public class AIServerSettings : ScriptableObject
    {
        [SerializeField]
        string serverUrl = "http://localhost:8000/invoke";

        public string ServerUrl => serverUrl;
    }
}