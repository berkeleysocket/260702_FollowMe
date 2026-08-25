using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace YHW.UI
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Effects/UI Button Punch")]
    public class UIButtonPunch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float hoverScale = 1.06f;
        [SerializeField] private float pressScale = 0.94f;
        [SerializeField] private float duration = 0.15f;

        private Vector3 _baseScale;
        private Tween _tween;
        private bool _pointerInside;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        private void OnDisable()
        {
            _tween?.Kill();
            transform.localScale = _baseScale;
            _pointerInside = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _pointerInside = true;
            Animate(_baseScale * hoverScale);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pointerInside = false;
            Animate(_baseScale);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Animate(_baseScale * pressScale);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Animate(_pointerInside ? _baseScale * hoverScale : _baseScale);
        }

        private void Animate(Vector3 target)
        {
            _tween?.Kill();
            _tween = transform.DOScale(target, duration).SetEase(Ease.OutQuad);
        }
    }
}
