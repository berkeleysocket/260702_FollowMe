using UnityEngine;

namespace FollowMe.KDS
{
    public static class PlayerTriggerUtility
    {
        public static bool IsPlayer(Collider2D other)
        {
            if (other == null) return false;
            return other.CompareTag("Player")
                || other.GetComponentInParent<PhotoProbePlayer>() != null;
        }

        public static PhotoProbePlayer GetPlayer(Collider2D other)
        {
            if (other == null) return null;
            return other.GetComponentInParent<PhotoProbePlayer>();
        }
    }
}
