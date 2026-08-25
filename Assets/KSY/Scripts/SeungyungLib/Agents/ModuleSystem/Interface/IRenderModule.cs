namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IRenderModule : IModule
    {
        void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0);
        void FlipX(bool flip);
    }
}