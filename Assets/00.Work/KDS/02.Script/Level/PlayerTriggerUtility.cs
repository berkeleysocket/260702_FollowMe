using UnityEngine;

namespace FollowMe.KDS
{
    public static class PlayerTriggerUtility
    {
        public static bool IsPlayer(Collider2D other)
        {
            if (other == null) return false;
            return other.CompareTag("Player")
                || other.GetComponentInParent<PlayerRespawn>() != null
                || other.GetComponentInParent<PhotoProbePlayer>() != null;
        }

        public static Transform GetPlayerTransform(Collider2D other)
        {
            if (other == null) return null;

            var respawn = other.GetComponentInParent<PlayerRespawn>();
            if (respawn != null)
                return respawn.transform;

            var probe = other.GetComponentInParent<PhotoProbePlayer>();
            return probe != null ? probe.transform : null;
        }
    }
}
