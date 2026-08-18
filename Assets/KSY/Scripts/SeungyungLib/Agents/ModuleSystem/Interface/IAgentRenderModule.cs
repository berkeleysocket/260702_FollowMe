namespace SeungyungLib.Agents.ModuleSystem.Interface
{
    public interface IAgentRenderModule : IModule
    {
        void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0);
        void FlipX(bool flip);
    }
}