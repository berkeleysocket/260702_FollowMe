using UnityEngine;

namespace YHW.Items
{
    [RequireComponent(typeof(Collider2D))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemData item;
        [SerializeField] private GameObject pickupEffectPrefab;

        private void Reset()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var collector = other.GetComponentInParent<PlayerItemCollector>();
            if (collector == null) return;

            collector.Collect(item);

            if (pickupEffectPrefab != null)
            {
                Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
