using UnityEngine;
using UnityEngine.UI;

namespace YHW.UI
{
    [AddComponentMenu("UI/Effects/UI Gradient")]
    public class UIGradient : BaseMeshEffect
    {
        [SerializeField] private Color topColor = new Color(0.16f, 0.20f, 0.42f);
        [SerializeField] private Color bottomColor = new Color(0.03f, 0.04f, 0.10f);

        public Color TopColor { get => topColor; set { topColor = value; if (graphic != null) graphic.SetVerticesDirty(); } }
        public Color BottomColor { get => bottomColor; set { bottomColor = value; if (graphic != null) graphic.SetVerticesDirty(); } }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0) return;

            float minY = float.MaxValue;
            float maxY = float.MinValue;
            var vertex = new UIVertex();

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                float y = vertex.position.y;
                if (y > maxY) maxY = y;
                if (y < minY) minY = y;
            }

            float height = Mathf.Max(0.0001f, maxY - minY);
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                float t = (vertex.position.y - minY) / height;
                vertex.color = Color32.Lerp(bottomColor, topColor, t);
                vh.SetUIVertex(vertex, i);
            }
        }
    }
}
