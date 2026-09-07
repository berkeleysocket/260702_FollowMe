using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 포토존 월드 연출: idle 펄스, 홀드 흔들림, 촬영 플래시, Used 페이드.
    /// PhotoPoint와 같은 오브젝트에 붙인다.
    /// </summary>
    [RequireComponent(typeof(PhotoPoint))]
    public class PhotoPointFx : MonoBehaviour
    {
        [SerializeField] private PhotoPoint _photo;
        [SerializeField] private Transform _available;
        [SerializeField] private Transform _prompt;
        [SerializeField] private Transform _used;
        [SerializeField] private SpriteRenderer _flashBurst;

        [Header("Idle")]
        [SerializeField] private float _idlePulseSpeed = 2.4f;
        [SerializeField] private float _idlePulseAmount = 0.08f;
        [SerializeField] private float _idleBobAmount = 0.12f;

        [Header("Hold")]
        [SerializeField] private float _holdShake = 0.08f;
        [SerializeField] private float _holdScalePunch = 0.18f;

        [Header("Flash")]
        [SerializeField] private float _flashDuration = 0.22f;
        [SerializeField] private float _flashMaxScale = 4.5f;

        private Vector3 _availableBaseScale = Vector3.one;
        private Vector3 _promptBaseScale = Vector3.one;
        private Vector3 _promptBasePos;
        private Vector3 _usedBaseScale = Vector3.one;
        private bool _wasUsed;
        private float _flashT = -1f;
        private float _usedPopT = -1f;

        private void Awake()
        {
            if (_photo == null)
                _photo = GetComponent<PhotoPoint>();

            AutoWire();
            CacheBases();
            EnsureFlashBurst();
        }

        private void OnEnable()
        {
            AutoWire();
            CacheBases();
            _wasUsed = _photo != null && _photo.IsUsed;
            if (_flashBurst != null)
                _flashBurst.enabled = false;
        }

        private void AutoWire()
        {
            if (_available == null)
            {
                var t = transform.Find("Available");
                if (t != null) _available = t;
            }

            if (_prompt == null)
            {
                var t = transform.Find("Prompt");
                if (t != null) _prompt = t;
            }

            if (_used == null)
            {
                var t = transform.Find("Used");
                if (t != null) _used = t;
            }

            if (_flashBurst == null)
            {
                var t = transform.Find("FlashBurst");
                if (t != null) _flashBurst = t.GetComponent<SpriteRenderer>();
            }
        }

        private void CacheBases()
        {
            if (_available != null)
                _availableBaseScale = _available.localScale;
            if (_prompt != null)
            {
                _promptBaseScale = _prompt.localScale;
                _promptBasePos = _prompt.localPosition;
            }

            if (_used != null)
                _usedBaseScale = _used.localScale;
        }

        private void EnsureFlashBurst()
        {
            if (_flashBurst != null) return;

            var go = new GameObject("FlashBurst");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            _flashBurst = go.AddComponent<SpriteRenderer>();
            _flashBurst.sprite = Texture2D.whiteTexture != null
                ? Sprite.Create(Texture2D.whiteTexture,
                    new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
                    new Vector2(0.5f, 0.5f), 4f)
                : null;
            _flashBurst.color = new Color(1f, 1f, 1f, 0f);
            _flashBurst.sortingOrder = 20;
            _flashBurst.enabled = false;
        }

        private void LateUpdate()
        {
            if (_photo == null) return;

            float t = Time.unscaledTime;
            bool used = _photo.IsUsed;
            bool holding = _photo.IsHolding;
            bool nearby = _photo.IsPlayerInside && !used;

            if (!_wasUsed && used)
            {
                _flashT = 0f;
                _usedPopT = 0f;
            }

            _wasUsed = used;

            AnimateAvailable(t, used, holding);
            AnimatePrompt(t, nearby, holding);
            AnimateUsed(used);
            AnimateFlash();
        }

        private void AnimateAvailable(float t, bool used, bool holding)
        {
            if (_available == null || !_available.gameObject.activeInHierarchy) return;

            float pulse = 1f + Mathf.Sin(t * _idlePulseSpeed) * _idlePulseAmount;
            float bob = Mathf.Sin(t * (_idlePulseSpeed * 0.85f)) * _idleBobAmount;
            Vector3 pos = _available.localPosition;
            pos.y = bob;

            if (holding)
            {
                float p = _photo.HoldProgress;
                float shake = Mathf.Sin(t * 48f) * _holdShake * p;
                pos.x = shake;
                pos.y += Mathf.Cos(t * 37f) * _holdShake * 0.5f * p;
                pulse = 1f + p * _holdScalePunch + Mathf.Sin(t * 20f) * 0.03f * p;
            }
            else
            {
                pos.x = 0f;
            }

            _available.localPosition = pos;
            _available.localScale = _availableBaseScale * pulse;

            var sr = _available.GetComponent<SpriteRenderer>();
            if (sr != null && !used)
            {
                Color c = sr.color;
                c.a = holding
                    ? Mathf.Lerp(0.55f, 0.95f, _photo.HoldProgress)
                    : 0.45f + 0.2f * (0.5f + 0.5f * Mathf.Sin(t * _idlePulseSpeed));
                sr.color = c;
            }
        }

        private void AnimatePrompt(float t, bool nearby, bool holding)
        {
            if (_prompt == null || !_prompt.gameObject.activeInHierarchy) return;

            float bounce = 1f + Mathf.Sin(t * 6f) * 0.12f;
            Vector3 pos = _promptBasePos;
            pos.y += Mathf.Abs(Mathf.Sin(t * 4.5f)) * 0.15f;
            if (holding)
                bounce = 1f + _photo.HoldProgress * 0.35f;

            _prompt.localPosition = pos;
            _prompt.localScale = _promptBaseScale * bounce;
        }

        private void AnimateUsed(bool used)
        {
            if (_used == null || !_used.gameObject.activeInHierarchy) return;

            if (_usedPopT >= 0f)
            {
                _usedPopT += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(_usedPopT / 0.35f);
                float ease = 1f - Mathf.Pow(1f - u, 3f);
                float overshoot = 1f + Mathf.Sin(ease * Mathf.PI) * 0.2f;
                _used.localScale = _usedBaseScale * Mathf.Lerp(0.2f, 1f, ease) * overshoot;
                if (u >= 1f) _usedPopT = -1f;
            }
            else if (used)
            {
                _used.localScale = _usedBaseScale;
            }
        }

        private void AnimateFlash()
        {
            if (_flashBurst == null || _flashT < 0f) return;

            _flashT += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(_flashT / Mathf.Max(0.01f, _flashDuration));
            float fade = 1f - u;
            float scale = Mathf.Lerp(0.6f, _flashMaxScale, EaseOutCubic(u));

            _flashBurst.enabled = fade > 0.02f;
            _flashBurst.transform.localScale = Vector3.one * scale;
            _flashBurst.color = new Color(1f, 0.95f, 1f, fade * 0.85f);

            if (u >= 1f)
            {
                _flashT = -1f;
                _flashBurst.enabled = false;
            }
        }

        private static float EaseOutCubic(float x)
        {
            float inv = 1f - x;
            return 1f - inv * inv * inv;
        }
    }
}
