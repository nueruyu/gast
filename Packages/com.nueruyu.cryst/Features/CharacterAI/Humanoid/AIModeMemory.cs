namespace Cryst.Features.CharacterAI.Humanoid
{
    public class AIModeMemory
    {
        public AIMode CurrentMode { get; private set; } = AIMode.Idle;

        public void SetMode(AIMode mode)
        {
            CurrentMode = mode;
        }
    }
}
