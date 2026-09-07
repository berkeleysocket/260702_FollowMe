using UnityEngine;

namespace KSY.Enemy
{
    public class GhostEnemy : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target; // 추적할 플레이어 Transform
        [SerializeField] private bool autoFindPlayerWithTag = true;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 3f; // 기본 추적 속도
        [SerializeField] private float stopDistance = 0.5f; // 목표 근처 정지 거리

        [Header("Floating / Wiggle Settings")]
        [SerializeField] private float waveFrequency = 3f; // 흔들리는 속도 (주파수)
        [SerializeField] private float waveAmplitude = 0.5f; // 흔들리는 크기 (진폭)
        [SerializeField] private bool useLocalUpForWave = true; // 이동 방향의 수직 방향으로 흔들릴지 여부

        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D rb;

        private float _sineOffset;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (rb == null) rb = GetComponent<Rigidbody2D>();

            // 유령마다 비동기적으로 흔들리도록 무작위 오프셋 부여
            _sineOffset = Random.Range(0f, 100f);
        }

        private void Start()
        {
            if (autoFindPlayerWithTag && target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    target = playerObj.transform;
                }
            }
        }

        private void Update()
        {
            if (target == null) return;

            // 바라보는 방향에 따른 Sprite Flip 처리
            FlipSprite();

            // Rigidbody2D가 없거나 Kinematic 상태인 경우 Transform으로 이동
            if (rb == null || rb.bodyType == RigidbodyType2D.Kinematic)
            {
                MoveTransform();
            }
        }

        private void FixedUpdate()
        {
            // Dynamic Rigidbody2D를 사용하는 물리 기반 이동
            if (target != null && rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                MoveRigidbody();
            }
        }

        /// <summary>
        /// Transform 기반 이동 (위아래/수직 파동 계산 포함)
        /// </summary>
        private void MoveTransform()
        {
            Vector3 currentPos = transform.position;
            Vector3 targetPos = target.position;

            // 플레이어 방향 단위 벡터 계산
            Vector3 moveDirection = (targetPos - currentPos).normalized;
            float distance = Vector3.Distance(currentPos, targetPos);

            if (distance > stopDistance)
            {
                // 기본 추적 위치 이동
                Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);

                // 사인파(Sine Wave)를 이용한 파동 오프셋 계산
                float sineValue = Mathf.Sin((Time.time + _sineOffset) * waveFrequency) * waveAmplitude;

                // 이동 방향에 직교하는 수직 벡터 계산 (Local Up 또는 Global Up)
                Vector3 waveDirection = useLocalUpForWave 
                    ? Vector3.Cross(moveDirection, Vector3.forward).normalized 
                    : Vector3.up;

                // 최종 위치 적용
                transform.position = newPos + (waveDirection * (sineValue * Time.deltaTime));
            }
        }

        /// <summary>
        /// Rigidbody2D 기반 속도(Velocity) 설정 이동
        /// </summary>
        private void MoveRigidbody()
        {
            Vector2 currentPos = rb.position;
            Vector2 targetPos = target.position;
            Vector2 moveDirection = (targetPos - currentPos).normalized;

            float distance = Vector2.Distance(currentPos, targetPos);

            if (distance > stopDistance)
            {
                // 사인파 오프셋 계산
                float sineValue = Mathf.Sin((Time.time + _sineOffset) * waveFrequency) * waveAmplitude;
                
                // 이동 방향의 수직 벡터 (Perpendicular)
                Vector2 perpendicularDir = new Vector2(-moveDirection.y, moveDirection.x);

                // 직진 속도 + 수직 흔들림 속도 합산
                Vector2 finalVelocity = (moveDirection * moveSpeed) + (perpendicularDir * sineValue);
                rb.velocity = finalVelocity;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }

        /// <summary>
        /// 타겟 위치에 따라 좌우 반전
        /// </summary>
        private void FlipSprite()
        {
            if (spriteRenderer == null) return;

            if (target.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true; // 왼쪽 바라보기 (기본 이미지 방향에 따라 조정)
            }
            else if (target.position.x > transform.position.x)
            {
                spriteRenderer.flipX = false; // 오른쪽 바라보기
            }
        }
    }
}