using UnityEngine;

namespace Gast.Infrastructure.Remoting.AI
{
    [CreateAssetMenu(fileName = "AIServerSettings", menuName = "Gast/AI/Server Settings")]
    public class AIServerSettings : ScriptableObject
    {
        // Updated endpoint to match Gaia's router
        [SerializeField]
        string serverUrl = "http://localhost:8000/request_plan";

        public string ServerUrl => serverUrl;
    }
}