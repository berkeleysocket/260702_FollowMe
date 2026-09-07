#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FollowMe.KDS.Editor
{
    [CustomEditor(typeof(PhotoPointRewardSO))]
    public class PhotoPointRewardSOEditor : UnityEditor.Editor
    {
        private SerializedProperty _displayName;
        private SerializedProperty _photoSprite;
        private SerializedProperty _likeBonus;
        private SerializedProperty _followBonus;
        private SerializedProperty _holdSeconds;
        private SerializedProperty _oneShot;
        private SerializedProperty _hashtags;

        private string _newTag = "";
        private bool _previewFoldout = true;

        private void OnEnable()
        {
            _displayName = serializedObject.FindProperty("_displayName");
            _photoSprite = serializedObject.FindProperty("_photoSprite");
            _likeBonus = serializedObject.FindProperty("_likeBonus");
            _followBonus = serializedObject.FindProperty("_followBonus");
            _holdSeconds = serializedObject.FindProperty("_holdSeconds");
            _oneShot = serializedObject.FindProperty("_oneShot");
            _hashtags = serializedObject.FindProperty("_hashtags");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawIdentity();
            EditorGUILayout.Space(6);
            DrawRewards();
            EditorGUILayout.Space(6);
            DrawHashtags();
            EditorGUILayout.Space(10);
            DrawPolaroidPreview();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawIdentity()
        {
            EditorGUILayout.LabelField("포토존 정보", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(_displayName, new GUIContent("표시 이름"));
                EditorGUILayout.PropertyField(_photoSprite, new GUIContent("사진 스프라이트", "인스타/폴라로이드 카드에 넣을 일러스트"));
                EditorGUILayout.PropertyField(_oneShot, new GUIContent("한 번만 촬영"));
            }
        }

        private void DrawRewards()
        {
            EditorGUILayout.LabelField("보상", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(_likeBonus, new GUIContent("좋아요 (+)"));
                EditorGUILayout.PropertyField(_followBonus, new GUIContent("팔로우 (+)"));
                EditorGUILayout.PropertyField(_holdSeconds, new GUIContent("홀드 시간(초)", "탭 방식으로 바꾸면 무시될 수 있음"));

                long likes = _likeBonus.longValue;
                long follows = _followBonus.longValue;
                EditorGUILayout.HelpBox($"촬영 시  +♡ {likes:N0}   +Follow {follows:N0}", MessageType.None);
            }
        }

        private void DrawHashtags()
        {
            EditorGUILayout.LabelField("해시태그", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (_hashtags != null)
                {
                    for (int i = 0; i < _hashtags.arraySize; i++)
                    {
                        var elem = _hashtags.GetArrayElementAtIndex(i);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUI.BeginChangeCheck();
                            string raw = EditorGUILayout.TextField(elem.stringValue);
                            if (EditorGUI.EndChangeCheck())
                                elem.stringValue = PhotoPointRewardSO.NormalizeHashtag(raw);

                            if (GUILayout.Button("✕", GUILayout.Width(24)))
                            {
                                _hashtags.DeleteArrayElementAtIndex(i);
                                break;
                            }
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    _newTag = EditorGUILayout.TextField(_newTag);
                    using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(_newTag)))
                    {
                        if (GUILayout.Button("추가", GUILayout.Width(52)))
                        {
                            int idx = _hashtags.arraySize;
                            _hashtags.InsertArrayElementAtIndex(idx);
                            _hashtags.GetArrayElementAtIndex(idx).stringValue =
                                PhotoPointRewardSO.NormalizeHashtag(_newTag);
                            _newTag = "";
                            GUI.FocusControl(null);
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("# 정규화"))
                        NormalizeAllHashtags();

                    if (GUILayout.Button("기본 태그 채우기") && _hashtags.arraySize == 0)
                    {
                        _hashtags.arraySize = 2;
                        _hashtags.GetArrayElementAtIndex(0).stringValue = "#FollowMe";
                        _hashtags.GetArrayElementAtIndex(1).stringValue = "#인증샷";
                    }
                }

                var so = (PhotoPointRewardSO)target;
                string line = so.HashtagLine;
                if (!string.IsNullOrEmpty(line))
                {
                    var style = new GUIStyle(EditorStyles.miniLabel)
                    {
                        normal = { textColor = new Color(0.35f, 0.55f, 0.95f) },
                        wordWrap = true
                    };
                    EditorGUILayout.LabelField(line, style);
                }
            }
        }

        private void NormalizeAllHashtags()
        {
            for (int i = 0; i < _hashtags.arraySize; i++)
            {
                var elem = _hashtags.GetArrayElementAtIndex(i);
                elem.stringValue = PhotoPointRewardSO.NormalizeHashtag(elem.stringValue);
            }
        }

        private void DrawPolaroidPreview()
        {
            _previewFoldout = EditorGUILayout.Foldout(_previewFoldout, "카드 미리보기", true);
            if (!_previewFoldout) return;

            var so = (PhotoPointRewardSO)target;
            const float cardW = 220f;
            const float cardH = 280f;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                var cardRect = GUILayoutUtility.GetRect(cardW, cardH, GUILayout.ExpandWidth(false));
                DrawCard(cardRect, so);
                GUILayout.FlexibleSpace();
            }
        }

        private static void DrawCard(Rect rect, PhotoPointRewardSO so)
        {
            EditorGUI.DrawRect(rect, new Color(0.95f, 0.95f, 0.96f));
            EditorGUI.DrawRect(new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - 2), Color.white);

            var photoRect = new Rect(rect.x + 14, rect.y + 14, rect.width - 28, rect.height * 0.58f);
            EditorGUI.DrawRect(photoRect, new Color(0.12f, 0.14f, 0.2f));

            if (so.PhotoSprite != null)
            {
                var tex = AssetPreview.GetAssetPreview(so.PhotoSprite);
                if (tex == null)
                    tex = so.PhotoSprite.texture;
                if (tex != null)
                    GUI.DrawTexture(photoRect, tex, ScaleMode.ScaleAndCrop);
            }
            else
            {
                var placeholder = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleCenter };
                GUI.Label(photoRect, "사진 스프라이트\n미지정", placeholder);
            }

            float ty = photoRect.yMax + 10f;
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12
            };
            GUI.Label(new Rect(rect.x + 8, ty, rect.width - 16, 20), so.DisplayName, titleStyle);

            ty += 22f;
            if (!string.IsNullOrEmpty(so.HashtagLine))
            {
                var tagStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    normal = { textColor = new Color(0.25f, 0.45f, 0.9f) }
                };
                GUI.Label(new Rect(rect.x + 8, ty, rect.width - 16, 32), so.HashtagLine, tagStyle);
                ty += 30f;
            }

            var rewardStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            GUI.Label(
                new Rect(rect.x + 8, ty, rect.width - 16, 18),
                $"+♡ {so.LikeBonus:N0}   +Follow {so.FollowBonus:N0}",
                rewardStyle);
        }

        public override bool HasPreviewGUI() => true;

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (target is PhotoPointRewardSO so)
                DrawCard(r, so);
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            var so = target as PhotoPointRewardSO;
            if (so == null || so.PhotoSprite == null || so.PhotoSprite.texture == null)
                return base.RenderStaticPreview(assetPath, subAssets, width, height);

            var src = so.PhotoSprite.texture;
            var preview = new Texture2D(width, height);
            // Project sprite into preview; fallback soft tint if not readable
            try
            {
                var rt = RenderTexture.GetTemporary(width, height);
                Graphics.Blit(src, rt);
                RenderTexture.active = rt;
                preview.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                preview.Apply();
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(rt);
                return preview;
            }
            catch
            {
                DestroyImmediate(preview);
                return base.RenderStaticPreview(assetPath, subAssets, width, height);
            }
        }
    }

    [CustomEditor(typeof(PhotoPoint))]
    public class PhotoPointEditor : UnityEditor.Editor
    {
        private SerializedProperty _pointId;
        private SerializedProperty _reward;
        private SerializedProperty _fallbackLikeBonus;
        private SerializedProperty _fallbackFollowBonus;
        private SerializedProperty _fallbackHoldSeconds;
        private SerializedProperty _oneShot;
        private SerializedProperty _availableVisual;
        private SerializedProperty _usedVisual;
        private SerializedProperty _promptVisual;

        private void OnEnable()
        {
            _pointId = serializedObject.FindProperty("_pointId");
            _reward = serializedObject.FindProperty("_reward");
            _fallbackLikeBonus = serializedObject.FindProperty("_fallbackLikeBonus");
            _fallbackFollowBonus = serializedObject.FindProperty("_fallbackFollowBonus");
            _fallbackHoldSeconds = serializedObject.FindProperty("_fallbackHoldSeconds");
            _oneShot = serializedObject.FindProperty("_oneShot");
            _availableVisual = serializedObject.FindProperty("_availableVisual");
            _usedVisual = serializedObject.FindProperty("_usedVisual");
            _promptVisual = serializedObject.FindProperty("_promptVisual");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("포토존", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(_pointId, new GUIContent("Point Id"));
                EditorGUILayout.PropertyField(_reward, new GUIContent("보상 SO"));

                var reward = _reward.objectReferenceValue as PhotoPointRewardSO;
                if (reward == null)
                {
                    EditorGUILayout.HelpBox("보상 SO를 지정하세요. (Create → FollowMe/KDS/Photo Point Reward)", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        $"{reward.DisplayName}\n{reward.HashtagLine}\n+♡ {reward.LikeBonus:N0}  +Follow {reward.FollowBonus:N0}",
                        MessageType.Info);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("SO 선택"))
                            EditorGUIUtility.PingObject(reward);
                        if (GUILayout.Button("SO 열기"))
                            AssetDatabase.OpenAsset(reward);
                    }
                }
            }

            EditorGUILayout.Space(4);
            _fallbackLikeBonus.isExpanded = EditorGUILayout.Foldout(_fallbackLikeBonus.isExpanded, "Fallback / Visuals", true);
            if (_fallbackLikeBonus.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(_fallbackLikeBonus);
                    EditorGUILayout.PropertyField(_fallbackFollowBonus);
                    EditorGUILayout.PropertyField(_fallbackHoldSeconds);
                    EditorGUILayout.PropertyField(_oneShot);
                    EditorGUILayout.PropertyField(_availableVisual);
                    EditorGUILayout.PropertyField(_usedVisual);
                    EditorGUILayout.PropertyField(_promptVisual);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
