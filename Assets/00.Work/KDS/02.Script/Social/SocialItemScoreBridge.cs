using UnityEngine;
using YHW.Items;
using YHW.Stats;

namespace FollowMe.KDS
{
    /// <summary>
    /// YHW ItemPickup → SocialScoreService / StressMeter 훅.
    /// Heart = 팔로우(+스트레스↓), Emoji = 좋아요(+스트레스↓),
    /// 화난/악마 이모지 = 스트레스만 감소.
    /// YHW 폴더는 수정하지 않는다.
    /// </summary>
    public class SocialItemScoreBridge : MonoBehaviour
    {
        [SerializeField] private long _followPerHeartValue = 50;
        [SerializeField] private long _likePerEmojiValue = 100;
        [SerializeField] private bool _scaleByItemValue = true;
        [SerializeField] private float _stressReducePerAngry = 8f;
        [SerializeField] private StressMeter _stressMeter;

        private PlayerItemCollector _collector;

        private void OnEnable()
        {
            Bind();
        }

        private void Start()
        {
            MapTriggerLayer.ApplyAllItemPickupsInScene();
            Bind();
        }

        private void OnDisable()
        {
            if (_collector != null)
                _collector.ItemCollected -= OnItemCollected;
            _collector = null;
        }

        private void Bind()
        {
            var player = ResolvePlayer();
            if (player == null) return;

            var next = player.GetComponent<PlayerItemCollector>();
            if (next == null)
                next = player.gameObject.AddComponent<PlayerItemCollector>();

            if (_collector == next) return;

            if (_collector != null)
                _collector.ItemCollected -= OnItemCollected;

            _collector = next;
            _collector.ItemCollected += OnItemCollected;
        }

        private static Transform ResolvePlayer()
        {
            var tagged = GameObject.FindGameObjectWithTag("Player");
            if (tagged != null) return tagged.transform;

            var respawn = Object.FindFirstObjectByType<PlayerRespawn>();
            if (respawn != null) return respawn.transform;

            var probe = Object.FindFirstObjectByType<PhotoProbePlayer>();
            return probe != null ? probe.transform : null;
        }

        private void OnItemCollected(ItemData item)
        {
            if (item == null)
                return;

            if (IsAngryEmoji(item))
            {
                ApplyAngryStress(item);
                return;
            }

            if (SocialScoreService.Instance == null)
                return;

            long amount = _scaleByItemValue ? Mathf.Max(1, item.Value) : 1;

            switch (item.ItemType)
            {
                case ItemType.Heart:
                    if (SocialScoreService.Instance.CollectFollow(amount * _followPerHeartValue))
                        StageRunStats.Instance?.RegisterFollowPickup();
                    break;
                case ItemType.Emoji:
                    if (SocialScoreService.Instance.CollectLike(amount * _likePerEmojiValue))
                        StageRunStats.Instance?.RegisterLikePickup();
                    break;
            }
        }

        private void ApplyAngryStress(ItemData item)
        {
            if (!StageStressPolicy.UsesStressInActiveScene())
                return;

            float amount = _scaleByItemValue
                ? Mathf.Max(1, item.Value) * _stressReducePerAngry
                : _stressReducePerAngry;

            if (SocialScoreService.Instance != null)
            {
                SocialScoreService.Instance.RelieveStress(amount);
                return;
            }

            var meter = ResolveStressMeter();
            if (meter != null)
                meter.ReduceStress(amount);
        }

        private StressMeter ResolveStressMeter()
        {
            if (_stressMeter != null)
                return _stressMeter;

            _stressMeter = FindFirstObjectByType<StressMeter>();
            return _stressMeter;
        }

        private static bool IsAngryEmoji(ItemData item)
        {
            if (item.ItemType != ItemType.Emoji)
                return false;

            string id = item.ItemId;
            if (string.IsNullOrEmpty(id))
                return false;

            return id == "emoji_angry"
                   || id == "emoji_devil"
                   || id.IndexOf("angry", System.StringComparison.OrdinalIgnoreCase) >= 0
                   || id.IndexOf("devil", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
