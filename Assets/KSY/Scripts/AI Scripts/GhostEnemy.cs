using System.Collections;
using UnityEngine;

namespace SeungyungLib.Enemy
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
        [SerializeField] private float idleFloatFrequency = 2f;  // 정지 시 둥실거리는 속도
        [SerializeField] private float idleFloatAmplitude = 0.3f; // 정지 시 둥실거리는 높이(크기)

        [Header("Separation Settings (뭉침 방지)")]
        [SerializeField] private float separationRadius = 1.5f;
        [SerializeField] private float separationWeight = 1.5f;
        [SerializeField] private LayerMask enemyLayer;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 1f;

        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D rb;

        private float _sineOffset;
        private Coroutine _fadeCoroutine;
        private static readonly Collider2D[] OverlapResults = new Collider2D[10];

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (rb == null) rb = GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.gravityScale = 0f;
            }

            _sineOffset = Random.Range(0f, 100f);
        }

        private void Start()
        {
            if (autoFindPlayerWithTag && target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) target = playerObj.transform;
            }
        }

        private void Update()
        {
            if (target == null) return;

            FlipSprite();

            if (rb == null || rb.bodyType == RigidbodyType2D.Kinematic)
            {
                MoveTransform();
            }
        }

        private void FixedUpdate()
        {
            if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                if (target != null)
                {
                    MoveRigidbody();
                }
                else
                {
                    // 타겟이 없을 때 (Stop 상태) 반발력 없이 pure Y축 둥실거림만 적용
                    float idleSine = Mathf.Cos((Time.time + _sineOffset) * idleFloatFrequency) * idleFloatAmplitude;
                    rb.linearVelocity = new Vector2(0f, idleSine);
                }
            }
        }

        #region Fade Methods & ContextMenu

        [ContextMenu("Fade In (나타나기)")]
        public void FadeIn() => StartFade(1f);

        [ContextMenu("Fade Out (사라지기)")]
        public void FadeOut() => StartFade(0f);

        public void FadeIn(float duration) => StartFade(1f, duration);

        public void FadeOut(float duration) => StartFade(0f, duration);

        private void StartFade(float targetAlpha, float customDuration = -1f)
        {
            if (spriteRenderer == null) return;

            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

            float duration = customDuration > 0f ? customDuration : fadeDuration;
            _fadeCoroutine = StartCoroutine(CoFade(targetAlpha, duration));
        }

        private IEnumerator CoFade(float targetAlpha, float duration)
        {
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

        private Vector3 CalculateSeparation()
        {
            Vector3 separationSteer = Vector3.zero;
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, separationRadius, OverlapResults, enemyLayer);

            for (int i = 0; i < count; i++)
            {
                Collider2D other = OverlapResults[i];
                if (other.gameObject == gameObject) continue;

                Vector3 diff = transform.position - other.transform.position;
                float distance = diff.magnitude;

                if (distance > 0)
                {
                    separationSteer += (diff.normalized / distance);
                }
            }

            return separationSteer.normalized * separationWeight;
        }

        private void MoveTransform()
        {
            Vector3 currentPos = transform.position;
            Vector3 targetPos = target.position;

            float distance = Vector3.Distance(currentPos, targetPos);

            if (distance > stopDistance)
            {
                // 이동 중에만 반발력(Separation) 계산 및 적용
                Vector3 moveDirection = (targetPos - currentPos).normalized;
                Vector3 separation = CalculateSeparation();

                float sineValue = Mathf.Sin((Time.time + _sineOffset) * waveFrequency) * waveAmplitude;
                Vector3 waveDirection = Vector3.Cross(moveDirection, Vector3.forward).normalized;
                Vector3 waveOffset = waveDirection * (sineValue * Time.deltaTime);

                Vector3 finalDirection = (moveDirection + separation).normalized;

                transform.position += (finalDirection * (moveSpeed * Time.deltaTime)) + waveOffset;
            }
            else
            {
                // Stop 상태: 반발력 없이 pure Y축 둥실거림만 처리
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
                // 이동 중에만 반발력(Separation) 계산 및 적용
                Vector2 moveDirection = (targetPos - currentPos).normalized;
                Vector2 separation = CalculateSeparation();

                float sineValue = Mathf.Sin((Time.time + _sineOffset) * waveFrequency) * waveAmplitude;
                Vector2 perpendicularDir = new Vector2(-moveDirection.y, moveDirection.x);

                Vector2 finalDir = (moveDirection + separation).normalized;
                rb.linearVelocity = (finalDir * moveSpeed) + (perpendicularDir * sineValue);
            }
            else
            {
                // Stop 상태: X축 이동 0, Y축 둥실거리기만 적용
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
    }
}