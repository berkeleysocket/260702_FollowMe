using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // URP 사용 시 필요

public class HorrorPostProcessController : MonoBehaviour
{
    [Header("Volume Reference")]
    [SerializeField] private Volume globalVolume;

    [Header("Horror Effect Settings")]
    [SerializeField] private float hitDuration = 0.4f;

    // Post Processing Overrides
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    private LensDistortion _lensDistortion;
    private ColorAdjustments _colorAdjustments;

    private Coroutine _hitEffectCoroutine;

    private void Awake()
    {
        if (globalVolume == null)
            globalVolume = GetComponent<Volume>();

        if (globalVolume != null && globalVolume.profile != null)
        {
            // Volume Profile에서 개별 효과 가져오기
            globalVolume.profile.TryGet(out _vignette);
            globalVolume.profile.TryGet(out _chromaticAberration);
            globalVolume.profile.TryGet(out _lensDistortion);
            globalVolume.profile.TryGet(out _colorAdjustments);
        }
    }

    /// <summary>
    /// 플레이어 피격 시 공포 효과(화면 붉어짐 + 색수차 + 렌즈 왜곡)를 발생시킵니다.
    /// </summary>
    public void TriggerHitEffect()
    {
        if (_hitEffectCoroutine != null)
            StopCoroutine(_hitEffectCoroutine);

        _hitEffectCoroutine = StartCoroutine(CoHitEffect());
    }

    private IEnumerator CoHitEffect()
    {
        float elapsed = 0f;

        // 효과 강도 기본값 백업 및 설정
        if (_vignette != null)
        {
            _vignette.active = true;
            _vignette.color.value = Color.red; // 비네팅 색상을 붉은색으로
        }
        if (_chromaticAberration != null) _chromaticAberration.active = true;
        if (_lensDistortion != null) _lensDistortion.active = true;

        while (elapsed < hitDuration)
        {
            float t = elapsed / hitDuration;
            // 피크 점을 향해 빠르게 올라갔다가 부드럽게 감소 (Sine 곡선)
            float intensity = Mathf.Sin(t * Mathf.PI);

            // 1. Vignette (화면 테두리가 붉어짐)
            if (_vignette != null)
                _vignette.intensity.value = Mathf.Lerp(0.3f, 0.75f, intensity);

            // 2. Chromatic Aberration (색수차 - 화면 색상 분리 번짐)
            if (_chromaticAberration != null)
                _chromaticAberration.intensity.value = Mathf.Lerp(0.1f, 1.0f, intensity);

            // 3. Lens Distortion (화면이 기괴하게 왜곡됨)
            if (_lensDistortion != null)
                _lensDistortion.intensity.value = Mathf.Lerp(0f, -0.6f, intensity);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 효과 원복
        ResetEffects();
        _hitEffectCoroutine = null;
    }

    private void ResetEffects()
    {
        if (_vignette != null)
        {
            _vignette.color.value = Color.black;
            _vignette.intensity.value = 0.3f; // 평소 기본 어두운 분위기 유지
        }
        if (_chromaticAberration != null) _chromaticAberration.intensity.value = 0.1f;
        if (_lensDistortion != null) _lensDistortion.intensity.value = 0f;
    }
}