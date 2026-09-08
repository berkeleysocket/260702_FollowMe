using UnityEngine;
using YHW.Items;

namespace FollowMe.KDS
{
    /// <summary>
    /// KSY Player(Layer6)는 Default와 물리 충돌하지 않는다.
    /// 수집·골·체크포인트 등 맵 트리거는 Pickup 레이어로 올려 Player와 맞춘다.
    /// </summary>
    public static class MapTriggerLayer
    {
        public const string PickupLayerName = "Pickup";

        public static int PickupLayer
        {
            get
            {
                int layer = LayerMask.NameToLayer(PickupLayerName);
                return layer >= 0 ? layer : 0;
            }
        }

        public static void Apply(GameObject go)
        {
            if (go == null) return;
            int layer = PickupLayer;
            if (layer < 0) return;
            SetLayerRecursive(go.transform, layer);
        }

        public static void ApplyAllItemPickupsInScene()
        {
            int layer = PickupLayer;
            if (layer < 0) return;

            var pickups = Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < pickups.Length; i++)
            {
                var pickup = pickups[i];
                if (pickup == null) continue;

                SetLayerRecursive(pickup.transform, layer);

                // 스프라이트 스케일 대비 콜라이더가 작아 스치기 쉬움 → 최소 반경 보정
                var circle = pickup.GetComponent<CircleCollider2D>();
                if (circle != null && circle.radius < 0.28f)
                    circle.radius = 0.28f;
            }
        }

        private static void SetLayerRecursive(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            for (int i = 0; i < root.childCount; i++)
                SetLayerRecursive(root.GetChild(i), layer);
        }
    }
}
