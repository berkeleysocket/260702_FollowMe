using System; // Action 네임스페이스 추가
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace KSY
{
    public class HorrorPostProcessSetter : MonoBehaviour
    {
        [Header("Volume Reference")]
        [SerializeField] private Volume horrorVolume;

        [Header("Transition Settings")]
        [SerializeField] private bool disableGameObjectOnDefault = false;

        private Coroutine _transitionCoroutine;

        private void Awake()
        {
            if (horrorVolume == null)
            {
                horrorVolume = GetComponent<Volume>();
            }

            ResetToDefaultImmediate();
        }

        public void ApplyHorrorAtmosphere(float duration)
        {
            if (horrorVolume == null) return;
            if (horrorVolume.profile == null && horrorVolume.sharedProfile == null) return;

            if (!horrorVolume.gameObject.activeSelf)
                horrorVolume.gameObject.SetActive(true);

            horrorVolume.enabled = true;

            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }

            _transitionCoroutine = StartCoroutine(Co_AnimateWeight(1f, duration, null));
        }

        public void ResetToDefaultAtmosphere(float duration)
        {
            if (horrorVolume == null) return;

            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }

            _transitionCoroutine = StartCoroutine(Co_AnimateWeight(0f, duration, () =>
            {
                horrorVolume.weight = 0f;
                horrorVolume.enabled = false;

                if (disableGameObjectOnDefault)
                {
                    horrorVolume.gameObject.SetActive(false);
                }
            }));
        }

        public void ResetToDefaultImmediate()
        {
            if (horrorVolume == null) return;

            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }

            horrorVolume.weight = 0f;
            horrorVolume.enabled = false;

            if (disableGameObjectOnDefault)
            {
                horrorVolume.gameObject.SetActive(false);
            }
        }

        private IEnumerator Co_AnimateWeight(float targetWeight, float duration, Action onComplete)
        {
            float startWeight = horrorVolume.weight;

            if (duration <= 0f)
            {
                horrorVolume.weight = targetWeight;
                onComplete?.Invoke();
                _transitionCoroutine = null;
                yield break;
            }

            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                horrorVolume.weight = Mathf.Lerp(startWeight, targetWeight, timer / duration);
                yield return null;
            }

            horrorVolume.weight = targetWeight;
            onComplete?.Invoke();
            _transitionCoroutine = null;
        }

        private void OnDisable()
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }
        }
    }
}