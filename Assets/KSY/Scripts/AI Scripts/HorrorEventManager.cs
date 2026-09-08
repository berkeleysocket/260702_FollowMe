using SeungyungLib.Core.ReadOnlyAttribute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSY
{
    public class HorrorEventManager : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField, ReadOnly] private Transform playerTransform;
        [SerializeField] private HorrorPostProcessSetter ppSetter;
        [SerializeField] private GhostEnemy ghostEnemyPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private int poolSize = 15;
        [SerializeField] private int initialSpawnCount = 5;
        [SerializeField] private int maxActiveEnemyCount = 10;
        [SerializeField] private float minSpawnRadius = 5f;
        [SerializeField] private float maxSpawnRadius = 8f;

        [Header("Spawn Timing & Delay Settings")]
        [SerializeField] private float spawnMoveDelay = 1.5f;
        [SerializeField] private float minSpawnInterval = 3f;
        [SerializeField] private float maxSpawnInterval = 7f;
        [SerializeField] private int spawnCountPerInterval = 2;

        [Header("Event Timing Settings")]
        [SerializeField] private float eventTransitionDuration = 2f;

        // Object Pool & Active Trackers
        private readonly List<GhostEnemy> _enemyPool = new List<GhostEnemy>();
        private readonly List<GhostEnemy> _activeEnemies = new List<GhostEnemy>();

        private bool _isEventActive = false;
        private Coroutine _periodicSpawnCoroutine;

        private void Awake()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
                
            InitializePool();
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) playerTransform = playerObj.transform;
            }
        }

        private void InitializePool()
        {
            if (ghostEnemyPrefab == null) return;

            for (int i = 0; i < poolSize; i++)
            {
                GhostEnemy enemy = Instantiate(ghostEnemyPrefab, transform);
                enemy.gameObject.SetActive(false);
                _enemyPool.Add(enemy);
            }
        }

        #region Public API & ContextMenu

        [ContextMenu("Start Horror Event")]
        public void StartHorrorEvent()
        {
            if (_isEventActive) return;
            _isEventActive = true;

            if (ppSetter != null)
            {
                ppSetter.ApplyHorrorAtmosphere(eventTransitionDuration);
            }

            SpawnEnemies(initialSpawnCount);

            if (_periodicSpawnCoroutine != null) StopCoroutine(_periodicSpawnCoroutine);
            _periodicSpawnCoroutine = StartCoroutine(Co_PeriodicSpawnRoutine());
        }

        [ContextMenu("Stop Horror Event")]
        public void StopHorrorEvent()
        {
            if (!_isEventActive) return;
            _isEventActive = false;

            if (_periodicSpawnCoroutine != null)
            {
                StopCoroutine(_periodicSpawnCoroutine);
                _periodicSpawnCoroutine = null;
            }

            if (ppSetter != null)
            {
                ppSetter.ResetToDefaultAtmosphere(eventTransitionDuration);
            }

            StartCoroutine(Co_StopEventSequence());
        }

        #endregion

        #region Event Sequences & Routines

        private void SpawnEnemies(int requestedCount)
        {
            int spawnableSlots = maxActiveEnemyCount - _activeEnemies.Count;
            if (spawnableSlots <= 0) return;

            int actualSpawnCount = Mathf.Min(requestedCount, spawnableSlots);

            for (int i = 0; i < actualSpawnCount; i++)
            {
                GhostEnemy enemy = GetPooledEnemy();
                if (enemy == null) break;

                Vector3 spawnPosition = GetRandomSpawnPosition();
                enemy.transform.position = spawnPosition;

                enemy.gameObject.SetActive(true);
                _activeEnemies.Add(enemy);

                enemy.OnSpawnSequence(playerTransform, eventTransitionDuration, spawnMoveDelay);
            }
        }

        private IEnumerator Co_PeriodicSpawnRoutine()
        {
            while (_isEventActive)
            {
                float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
                yield return new WaitForSeconds(randomInterval);

                if (_isEventActive && _activeEnemies.Count < maxActiveEnemyCount)
                {
                    SpawnEnemies(spawnCountPerInterval);
                }
            }
        }

        private IEnumerator Co_StopEventSequence()
        {
            // 메모리 할당(new List) 없이 인덱스로 접근하여 GC 최소화
            for (int i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                GhostEnemy enemy = _activeEnemies[i];
                if (enemy != null && enemy.gameObject.activeSelf)
                {
                    enemy.OnDespawnSequence(eventTransitionDuration, () =>
                    {
                        ReturnToPool(enemy);
                    });
                }
            }

            yield return null;
        }

        #endregion

        #region Pool & Helper Methods

        private GhostEnemy GetPooledEnemy()
        {
            int poolCount = _enemyPool.Count;
            for (int i = 0; i < poolCount; i++)
            {
                if (!_enemyPool[i].gameObject.activeSelf)
                {
                    return _enemyPool[i];
                }
            }

            if (ghostEnemyPrefab != null)
            {
                GhostEnemy newEnemy = Instantiate(ghostEnemyPrefab, transform);
                newEnemy.gameObject.SetActive(false);
                _enemyPool.Add(newEnemy);
                return newEnemy;
            }

            return null;
        }

        private void ReturnToPool(GhostEnemy enemy)
        {
            enemy.gameObject.SetActive(false);
            _activeEnemies.Remove(enemy);
        }

        private Vector3 GetRandomSpawnPosition()
        {
            Vector3 center = (playerTransform != null) ? playerTransform.position : Vector3.zero;
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnRadius, maxSpawnRadius);
            
            return center + new Vector3(randomCircle.x, randomCircle.y, 0f);
        }

        #endregion
    }
}