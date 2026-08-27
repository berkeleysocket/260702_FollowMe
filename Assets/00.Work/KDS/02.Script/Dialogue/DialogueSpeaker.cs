using System;
using System.Collections.Generic;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 씬의 화자. JSON characterId와 같은 값을 넣으면 말풍선이 이 오브젝트 위에 뜬다.
    /// </summary>
    public class DialogueSpeaker : MonoBehaviour
    {
        private static readonly List<DialogueSpeaker> Speakers = new List<DialogueSpeaker>();

        [SerializeField] private string _characterId = "Haru";
        [SerializeField] private Transform _bubbleAnchor;
        [SerializeField] private SpriteRenderer _expressionRenderer;
        [SerializeField] private CharacterPortraitLibrary _portraits;

        public string CharacterId => _characterId;
        public Transform BubbleAnchor => _bubbleAnchor != null ? _bubbleAnchor : transform;

        private void OnEnable()
        {
            Speakers.Add(this);
        }

        private void OnDisable()
        {
            Speakers.Remove(this);
        }

        public void ApplyExpression(string expressionId)
        {
            if (_expressionRenderer == null || _portraits == null)
                return;

            if (_portraits.TryGetPortrait(_characterId, expressionId, out var sprite))
                _expressionRenderer.sprite = sprite;
        }

        public static DialogueSpeaker FindById(string characterId)
        {
            if (string.IsNullOrEmpty(characterId))
                return null;

            for (int i = 0; i < Speakers.Count; i++)
            {
                var speaker = Speakers[i];
                if (speaker != null &&
                    string.Equals(speaker._characterId, characterId, StringComparison.OrdinalIgnoreCase))
                    return speaker;
            }

            return null;
        }
    }
}
