using UnityEngine;
using UnityEngine.SceneManagement;

namespace FollowMe.KDS
{
    /// <summary>
    /// 클리어 시 별점 결과 오버레이 + 스테이지 선택으로 돌아가기.
    /// </summary>
    public class StageClearStarsHud : MonoBehaviour
    {
        private bool _visible;
        private int _stage;
        private int _stars;
        private int _followsCollected;
        private int _followsTotal;
        private float _showTime;

        public void Show(int stage, int stars, int followsCollected, int followsTotal)
        {
            _stage = stage;
            _stars = Mathf.Clamp(stars, 1, 3);
            _followsCollected = followsCollected;
            _followsTotal = followsTotal;
            _visible = true;
            _showTime = Time.unscaledTime;
        }

        private void OnGUI()
        {
            if (!_visible) return;

            float t = Mathf.Clamp01((Time.unscaledTime - _showTime) / 0.35f);
            float alpha = Mathf.SmoothStep(0f, 1f, t);

            var prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.55f * alpha);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);

            float w = 420f;
            float h = 280f;
            float x = (Screen.width - w) * 0.5f;
            float y = (Screen.height - h) * 0.38f;

            GUI.color = new Color(0.08f, 0.09f, 0.12f, 0.92f * alpha);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);

            GUI.color = new Color(1f, 1f, 1f, alpha);
            var title = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 28,
                fontStyle = FontStyle.Bold
            };
            GUI.Label(new Rect(x, y + 24f, w, 36f), $"Stage {_stage} Clear", title);

            var starStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 42,
                fontStyle = FontStyle.Bold
            };
            starStyle.normal.textColor = new Color(1f, 0.85f, 0.25f, alpha);
            string stars = new string('★', _stars) + new string('☆', 3 - _stars);
            GUI.Label(new Rect(x, y + 72f, w, 50f), stars, starStyle);

            float ratio = _followsTotal <= 0 ? 0f : _followsCollected / (float)_followsTotal;
            var sub = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16
            };
            sub.normal.textColor = new Color(0.85f, 0.88f, 0.95f, alpha);
            GUI.Label(
                new Rect(x, y + 140f, w, 28f),
                $"Follow {_followsCollected} / {_followsTotal}  ({ratio * 100f:0.#}%)",
                sub);

            var btnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            float bw = 260f;
            float bh = 44f;
            if (GUI.Button(new Rect(x + (w - bw) * 0.5f, y + 200f, bw, bh), "스테이지 선택으로", btnStyle))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(StageSceneCatalog.StageSelectSceneName);
            }

            GUI.color = prev;
        }
    }
}
