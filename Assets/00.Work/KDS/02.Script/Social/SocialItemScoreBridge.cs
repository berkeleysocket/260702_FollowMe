using UnityEngine;
using YHW.Items;

namespace FollowMe.KDS
{
    /// <summary>
    /// YHW ItemPickup → SocialScoreService 훅.
    /// Heart = 팔로우, Emoji = 좋아요. YHW 폴더는 수정하지 않는다.
    /// </summary>
    public class SocialItemScoreBridge : MonoBehaviour
    {
        [SerializeField] private long _followPerHeartValue = 50;
        [SerializeField] private long _likePerEmojiValue = 100;
        [SerializeField] private bool _scaleByItemValue = true;

        private PlayerItemCollector _collector;

        private void OnEnable()
        {
            Bind();
        }

        private void Start()
        {
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
            if (item == null || SocialScoreService.Instance == null)
                return;

            long amount = _scaleByItemValue ? Mathf.Max(1, item.Value) : 1;

            switch (item.ItemType)
            {
                case ItemType.Heart:
                    SocialScoreService.Instance.AddFollows(amount * _followPerHeartValue);
                    StageRunStats.Instance?.RegisterLikePickup();
                    break;
                case ItemType.Emoji:
                    SocialScoreService.Instance.AddLikes(amount * _likePerEmojiValue);
                    StageRunStats.Instance?.RegisterLikePickup();
                    break;
            }
        }
    }
}
