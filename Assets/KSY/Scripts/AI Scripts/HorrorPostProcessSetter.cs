using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace KSY
{
    public class HorrorPostProcessSetter : MonoBehaviour
    {
        [Header("Volume Reference")]
        [SerializeField] private Volume horrorVolume;

        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            if (horrorVolume == null)
                horrorVolume = GetComponent<Volume>();

            // 초기 상태는 Volume을 끄고 weight 0으로 설정
            if (horrorVolume != null)
            {
                horrorVolume.weight = 0f;
                horrorVolume.enabled = false;
            }
        }

        /// <summary>
        /// 호러 연출 시작 (Volume Weight 0 -> 1)
        /// </summary>
        public void ApplyHorrorAtmosphere(float duration)
        {
            if (horrorVolume == null) return;

            horrorVolume.enabled = true;

            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(Co_AnimateWeight(1.0f, duration, null));
        }

        /// <summary>
        /// 호러 연출 종료 (Volume Weight 1 -> 0)
        /// </summary>
        public void ResetToDefaultAtmosphere(float duration)
        {
            if (horrorVolume == null) return;

            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(Co_AnimateWeight(0.0f, duration, () =>
            {
                horrorVolume.enabled = false;
            }));
        }

        private IEnumerator Co_AnimateWeight(float targetWeight, float duration, System.Action onComplete)
        {
            float startWeight = horrorVolume.weight;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                horrorVolume.weight = Mathf.Lerp(startWeight, targetWeight, timer / duration);
                yield return null;
            }

            horrorVolume.weight = targetWeight;
            onComplete?.Invoke();
            _fadeCoroutine = null;
        }
    }
}