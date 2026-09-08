using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.Template.EventChannels;
using UnityEngine;
using Unity.Cinemachine; // Cinemachine 3.x 기준 (2.x는 Cinemachine)

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    
    [SerializeField] private EventChannelSO playerEventChannel;

    private void Awake()
    {
        if (impulseSource == null)
            impulseSource = GetComponent<CinemachineImpulseSource>();
        
        playerEventChannel.AddListener<PlayerHitEvent>(Shake);
    }

    public void Shake(PlayerHitEvent evt)
    {
        DebugLogger.Log("Camera Shake!");
        if (impulseSource != null)
        {
            // 인스펙터에 설정된 기본 파형으로 충격 발사
            impulseSource.GenerateImpulse();
            
            // 힘을 직접 지정하고 싶다면 아래 메서드 사용
            // impulseSource.GenerateImpulseWithForce(1.5f);
        }
    }
}