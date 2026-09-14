using SeungyungLib.CollectSystem.Core;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Md.Body.Core;
using SeungyungLib.ModuleSystem.Core;

using UnityEngine;

namespace KSY.Item
{
    public class Item : AbstractCollectable
    {
        [SerializeField] private ItemSO itemData;
        [SerializeField] private SpriteRenderer spRenderer;

        private void OnValidate()
        { 
            this.spRenderer.sprite = itemData.DefaultSprite;
        }

        protected override void OnCollect(IModuleOwner collector)
        {
            IBodyModule body = collector.GetModule<IBodyModule>();
            if (body == null)
            {
                DebugLogger.LogError($"{gameObject.name} : body is null");
                return;
            }
            
            if (body.CurrentHealth == body.MaxHealth)
                DebugLogger.Log($"점수 업! : {itemData.Score}");
            else
                body.Recovery(itemData.RecoveryValue);

            Delete();
        }
    }
}

