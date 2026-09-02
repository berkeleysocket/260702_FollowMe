namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IRenderModule : IModule
    {
        void PlayClip(int stateHashName, float fixedTransitionDuration, float fixedTimeOffset,
            float normalizedTransitionTime, int layer = -1); 
        void FlipX(bool flip);
    }
}