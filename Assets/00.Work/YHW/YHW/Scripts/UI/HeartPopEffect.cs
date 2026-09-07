using DG.Tweening;
using UnityEngine;

namespace YHW.UI
{
    /// <summary>
    /// Instagram-like "double tap heart" pop: scales in with overshoot,
    /// drifts upward, fades out, then destroys itself. Works on a
    /// SpriteRenderer (world space) or a CanvasGroup (UI space) - whichever
    /// is present on this object.
    /// </summary>
    [AddComponentMenu("UI/Effects/Heart Pop Effect")]
    public class HeartPopEffect : MonoBehaviour
    {
        [SerializeField] private float popScale = 1.2f;
        [SerializeField] private float popDuration = 0.2f;
        [SerializeField] private float settleDuration = 0.12f;
        [SerializeField] private float holdDuration = 0.15f;
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float floatUpDistance = 0.6f;
        [SerializeField] private Vector2 randomSidewaysDrift = new Vector2(-0.15f, 0.15f);

        private SpriteRenderer _spriteRenderer;
        
        private Vector3 _baseScale;
private CanvasGroup _canvasGroup;

private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _baseScale = transform.localScale;
        }

        private void OnEnable()
        {
            Play();
        }

private void Play()
        {
            transform.localScale = Vector3.zero;
            SetAlpha(1f);

            Vector3 startPos = transform.position;
            float drift = Random.Range(randomSidewaysDrift.x, randomSidewaysDrift.y);
            Vector3 endPos = startPos + new Vector3(drift, floatUpDistance, 0f);
            float totalDuration = popDuration + settleDuration + holdDuration + fadeDuration;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(_baseScale * popScale, popDuration).SetEase(Ease.OutBack));
            sequence.Join(transform.DOMove(endPos, totalDuration).SetEase(Ease.OutQuad));
            sequence.Append(transform.DOScale(_baseScale, settleDuration).SetEase(Ease.InOutQuad));
            sequence.AppendInterval(holdDuration);
            sequence.Append(DOTween.To(GetAlpha, SetAlpha, 0f, fadeDuration));
            sequence.OnComplete(() => Destroy(gameObject));
        }

        private float GetAlpha()
        {
            if (_canvasGroup != null) return _canvasGroup.alpha;
            if (_spriteRenderer != null) return _spriteRenderer.color.a;
            return 1f;
        }

        private void SetAlpha(float alpha)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = alpha;
                return;
            }

            if (_spriteRenderer == null) return;

            Color color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }
    }
}
