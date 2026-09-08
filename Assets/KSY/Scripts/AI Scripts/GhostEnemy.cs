using System;
using System.Collections;
using SeungyungLib.ModuleSystem.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace KSY
{
    public class GhostEnemy : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private bool autoFindPlayerWithTag = true;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float stopDistance = 0.5f;

        [Header("Floating / Wiggle Settings")]
        [SerializeField] private float waveFrequency = 3f;
        [SerializeField] private float waveAmplitude = 0.5f;

        [Header("Idle Floating Settings (제자리 둥실둥실)")]
        [SerializeField] private float idleFloatFrequency = 2f;
        [SerializeField] private float idleFloatAmplitude = 0.3f;

        [Header("Separation Settings (뭉침 방지 최적화)")]
        [SerializeField] private float separationRadius = 1.5f;
        [SerializeField] private float separationWeight = 1.5f;
        [SerializeField] private float separationInterval = 0.1f; // 0.1초마다 분리 연산
        [SerializeField] private LayerMask enemyLayer;

        [Header("Fade & Despawn Settings")]
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float minDespawnDelay = 0.0f;
        [SerializeField] private float maxDespawnDelay = 0.5f;

        [Header("Animation Names")]
        [SerializeField] private string idleAnimationName = "IDLE";
        [SerializeField] private string runAnimationName = "RUN";
        [SerializeField] private string deathAnimationName = "DEAD";

        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;

        private float _sineOffset;
        private Coroutine _fadeCoroutine;
        private Coroutine _sequenceCoroutine;
        private static readonly Collider2D[] OverlapResults = new Collider2D[10];

        private int _idleAnimHash;
        private int _runAnimHash;
        private int _deathAnimHash;
        private int _currentAnimHash;

        private bool _isDying = false;
        private Vector3 _cachedSeparation = Vector3.zero;
        private float _nextSeparationTime;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponent<Animator>();

            if (rb != null)
            {
                rb.gravityScale = 0f;
            }

            _sineOffset = UnityEngine.Random.Range(0f, 100f);

            if (!string.IsNullOrEmpty(idleAnimationName)) _idleAnimHash = Animator.StringToHash(idleAnimationName);
            if (!string.IsNullOrEmpty(runAnimationName)) _runAnimHash = Animator.StringToHash(runAnimationName);
            if (!string.IsNullOrEmpty(deathAnimationName)) _deathAnimHash = Animator.StringToHash(deathAnimationName);
        }

        private void Start()
        {
            if (autoFindPlayerWithTag && target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) target = playerObj.transform;
            }

            SetAnimation(_idleAnimHash);
        }

        private void Update()
        {
            if (_isDying || target == null)
            {
                if (!_isDying) SetAnimation(_idleAnimHash);
                return;
            }

            FlipSprite();

            if (rb == null || rb.bodyType == RigidbodyType2D.Kinematic)
            {
                MoveTransform();
            }
        }

        private void FixedUpdate()
        {
            if (_isDying) return;

            if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                if (target != null)
                {
                    MoveRigidbody();
                }
                else
                {
                    float idleSine = Mathf.Cos((Time.time + _sineOffset) * idleFloatFrequency) * idleFloatAmplitude;
                    rb.linearVelocity = new Vector2(0f, idleSine);
                    SetAnimation(_idleAnimHash);
                }
            }
        }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (other.gameObject.TryGetComponent(out IBodyModule body))
        //         body.Damage(1);
        // }

        #region Event Sequence Methods

        public void OnSpawnSequence(Transform playerTarget, float fadeDuration, float moveDelay = 0f)
        {
            _isDying = false;
            target = null;

            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                spriteRenderer.color = new Color(c.r, c.g, c.b, 0f);
            }

            if (_sequenceCoroutine != null) StopCoroutine(_sequenceCoroutine);
            _sequenceCoroutine = StartCoroutine(Co_SpawnSequence(playerTarget, fadeDuration, moveDelay));
        }

        private IEnumerator Co_SpawnSequence(Transform playerTarget, float fadeDuration, float moveDelay)
        {
            yield return StartCoroutine(CoFade(1f, fadeDuration));

            if (moveDelay > 0f)
            {
                yield return new WaitForSeconds(moveDelay);
            }

            target = playerTarget;
            _sequenceCoroutine = null;
        }

        public void OnDespawnSequence(float duration, Action onComplete)
        {
            _isDying = true;
            target = null;

            if (rb != null) rb.linearVelocity = Vector2.zero;

            if (_sequenceCoroutine != null) StopCoroutine(_sequenceCoroutine);
            _sequenceCoroutine = StartCoroutine(Co_DespawnSequence(duration, onComplete));
        }

        private IEnumerator Co_DespawnSequence(float duration, Action onComplete)
        {
            float randomDelay = UnityEngine.Random.Range(minDespawnDelay, maxDespawnDelay);
            if (randomDelay > 0f)
            {
                yield return new WaitForSeconds(randomDelay);
            }

            SetAnimation(_deathAnimHash);

            yield return StartCoroutine(CoFade(0f, duration));

            _isDying = false;
            _sequenceCoroutine = null;
            onComplete?.Invoke();
        }

        #endregion

        #region Animation Controller

        private void SetAnimation(int targetAnimHash)
        {
            if (animator == null || targetAnimHash == 0) return;

            if (_currentAnimHash != targetAnimHash)
            {
                _currentAnimHash = targetAnimHash;
                animator.Play(_currentAnimHash);
            }
        }

        #endregion

        #region Fade Methods

        private IEnumerator CoFade(float targetAlpha, float duration)
        {
            if (spriteRenderer == null) yield break;

            Color initialColor = spriteRenderer.color;
            float startAlpha = initialColor.a;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
                
                spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, newAlpha);
                yield return null;
            }

            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
            _fadeCoroutine = null;
        }

        #endregion

        #region Movement & Separation Logic (Optimized)

        // 매 프레임 연산하지 않고 separationInterval 마다만 연산하여 물리 렉 제거
        private Vector3 GetSeparation()
        {
            if (Time.time < _nextSeparationTime) return _cachedSeparation;

            _nextSeparationTime = Time.time + separationInterval;
            Vector3 separationSteer = Vector3.zero;
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, separationRadius, OverlapResults, enemyLayer);

            Vector3 toTargetDir = (target != null) ? (target.position - transform.position).normalized : Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                Collider2D other = OverlapResults[i];
                if (other.gameObject == gameObject) continue;

                Vector3 diff = transform.position - other.transform.position;
                float distance = diff.magnitude;

                if (distance > 0f)
                {
                    Vector3 pushDir = diff.normalized / distance;

                    if (toTargetDir != Vector3.zero)
                    {
                        float forwardComponent = Vector3.Dot(pushDir, toTargetDir);
                        pushDir -= toTargetDir * forwardComponent;
                    }

                    separationSteer += pushDir;
                }
            }

            _cachedSeparation = separationSteer.normalized * separationWeight;
            return _cachedSeparation;
        }

        private void MoveTransform()
        {
            Vector3 currentPos = transform.position;
            Vector3 targetPos = target.position;

            float distance = Vector3.Distance(currentPos, targetPos);

            if (distance > stopDistance)
            {
                SetAnimation(_runAnimHash);

                Vector3 moveDirection = (targetPos - currentPos).normalized;
                Vector3 separation = GetSeparation();

                float sineValue = Mathf.Sin((Time.time + _sineOffset) * waveFrequency) * waveAmplitude;
                Vector3 waveDirection = Vector3.Cross(moveDirection, Vector3.forward).normalized;
                Vector3 waveOffset = waveDirection * (sineValue * Time.deltaTime);

                Vector3 finalDirection = (moveDirection + separation).normalized;

                transform.position += (finalDirection * (moveSpeed * Time.deltaTime)) + waveOffset;
            }
            else
            {
                SetAnimation(_idleAnimHash);

                float idleSine = Mathf.Cos((Time.time + _sineOffset) * idleFloatFrequency) * idleFloatAmplitude;
                transform.position += new Vector3(0f, idleSine * Time.deltaTime, 0f);
            }
        }

        private void MoveRigidbody()
        {
            Vector2 currentPos = rb.position;
            Vector2 targetPos = target.position;

            float distance = Vector2.Distance(currentPos, targetPos);

            if (distance > stopDistance)
            {
                SetAnimation(_runAnimHash);

                Vector2 moveDirection = (targetPos - currentPos).normalized;
                Vector2 separation = GetSeparation();

                float sineValue = Mathf.Sin((Time.time + _sineOffset) * waveFrequency) * waveAmplitude;
                Vector2 perpendicularDir = new Vector2(-moveDirection.y, moveDirection.x);

                Vector2 finalDir = (moveDirection + separation).normalized;
                rb.linearVelocity = (finalDir * moveSpeed) + (perpendicularDir * sineValue);
            }
            else
            {
                SetAnimation(_idleAnimHash);

                float idleSine = Mathf.Cos((Time.time + _sineOffset) * idleFloatFrequency) * idleFloatAmplitude;
                rb.linearVelocity = new Vector2(0f, idleSine);
            }
        }

        private void FlipSprite()
        {
            if (spriteRenderer == null) return;

            if (target.position.x < transform.position.x)
                spriteRenderer.flipX = true;
            else if (target.position.x > transform.position.x)
                spriteRenderer.flipX = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, separationRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stopDistance);
        }

        #endregion
    }
}