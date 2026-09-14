using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KSY.StressSystem
{
    public class StressPostProcessController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Volume globalVolume;
        [SerializeField] private StressManagement stressManagement;

        [Header("Smooth Transition")]
        [SerializeField] private float lerpSpeed = 5f; // 효과가 부드럽게 변하는 속도

        // Volume Profile Overrides (URP)
        private Vignette _vignette;
        private ChromaticAberration _chromaticAberration;
        private FilmGrain _filmGrain;
        private ColorAdjustments _colorAdjustments;

        private float _targetRatio;

        private void Awake()
        {
            if (globalVolume == null) globalVolume = GetComponent<Volume>();

            // Volume Profile 내 효과 가져오기
            if (globalVolume != null && globalVolume.profile != null)
            {
                var profile = globalVolume.profile;
                profile.TryGet(out _vignette);
                profile.TryGet(out _chromaticAberration);
                profile.TryGet(out _filmGrain);
                profile.TryGet(out _colorAdjustments);
            }
        }

        private void Update()
        {
            if (stressManagement == null) return;

            UpdatePostProcessingEffects(_targetRatio);
        }

        public void SetStressRatio(int ratio)
        {
            _targetRatio = Mathf.Clamp01(ratio);
        }

        private void UpdatePostProcessingEffects(float ratio)
        {
            // 1. Vignette: 외곽이 점점 어두워지며 화면을 조여옴 (0.2 -> 0.65)
            if (_vignette != null)
            {
                float targetIntensity = Mathf.Lerp(0.2f, 0.65f, ratio);
                _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, targetIntensity, Time.deltaTime * lerpSpeed);
            }

            // 2. Chromatic Aberration: 색수차가 심해지며 어지러움 표현 (0.0 -> 1.0)
            if (_chromaticAberration != null)
            {
                float targetIntensity = Mathf.Lerp(0.0f, 1.0f, ratio);
                _chromaticAberration.intensity.value = Mathf.Lerp(_chromaticAberration.intensity.value, targetIntensity, Time.deltaTime * lerpSpeed);
            }

            // 3. Film Grain: 노이즈가 생겨 거칠고 불안정한 느낌 제공 (0.0 -> 1.0)
            if (_filmGrain != null)
            {
                float targetIntensity = Mathf.Lerp(0.0f, 1.0f, ratio);
                _filmGrain.intensity.value = Mathf.Lerp(_filmGrain.intensity.value, targetIntensity, Time.deltaTime * lerpSpeed);
            }

            // 4. Color Adjustments: 화면 전체가 어두워지고 흑백에 가까워짐 (채도 -80, 노출 -1.5)
            if (_colorAdjustments != null)
            {
                float targetExposure = Mathf.Lerp(0.0f, -1.5f, ratio);      // 화면 어두워짐
                float targetSaturation = Mathf.Lerp(0.0f, -80.0f, ratio);   // 채도 감소 (창백해짐)
                float targetContrast = Mathf.Lerp(0.0f, 40.0f, ratio);      // 대비 증가 (기괴한 음영)

                _colorAdjustments.postExposure.value = Mathf.Lerp(_colorAdjustments.postExposure.value, targetExposure, Time.deltaTime * lerpSpeed);
                _colorAdjustments.saturation.value = Mathf.Lerp(_colorAdjustments.saturation.value, targetSaturation, Time.deltaTime * lerpSpeed);
                _colorAdjustments.contrast.value = Mathf.Lerp(_colorAdjustments.contrast.value, targetContrast, Time.deltaTime * lerpSpeed);
            }
        }
    }
}