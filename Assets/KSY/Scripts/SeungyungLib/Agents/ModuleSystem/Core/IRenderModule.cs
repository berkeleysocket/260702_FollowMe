namespace SeungyungLib.ModuleSystem.Core
{
    public interface IRenderModule : IModule
    {
        void PlayClip(int stateHashName, float fixedTransitionDuration, float fixedTimeOffset,
            float normalizedTransitionTime, int layer = -1); 
        void FlipX(bool flip);
        
        //대충 쓴 코드
        void PlayInvincibilityEffect();
        void PlayHitShakeEffect();
    }
}