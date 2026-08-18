using System;
using System.Collections.Generic;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 캐릭터/표정 키를 스프라이트로 매핑한다.
    /// </summary>
    public class CharacterPortraitLibrary : MonoBehaviour
    {
        [Serializable]
        private class PortraitEntry
        {
            public string CharacterId = "Haru";
            public string ExpressionId = "Neutral";
            public Sprite Portrait;
        }

        [SerializeField] private List<PortraitEntry> _entries = new List<PortraitEntry>();

        public bool TryGetPortrait(string characterId, string expressionId, out Sprite portrait)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (!string.Equals(e.CharacterId, characterId, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!string.Equals(e.ExpressionId, expressionId, StringComparison.OrdinalIgnoreCase))
                    continue;

                portrait = e.Portrait;
                return portrait != null;
            }

            portrait = null;
            return false;
        }
    }
}
