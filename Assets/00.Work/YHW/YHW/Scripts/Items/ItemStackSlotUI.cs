using UnityEngine;
using UnityEngine.UI;

namespace YHW.Items
{
    public class ItemStackSlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Text countText;

        public void Setup(Sprite icon)
        {
            iconImage.sprite = icon;
        }

        public void SetCount(int count)
        {
            countText.text = "x" + count;
        }
    }
}
