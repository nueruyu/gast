using UnityEngine;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    [CreateAssetMenu(fileName = "MockStoryGenerationSettings", menuName = "Gast/AI/Mock Story Generation Settings")]
    public class MockStoryGenerationSettings : ScriptableObject
    {
        [SerializeField]
        bool isEnabled = true;

        [SerializeField, TextArea(15, 30)]
        string mockStoryJson;

        public bool IsEnabled => isEnabled;
        public string MockStoryJson => mockStoryJson;
    }
}
