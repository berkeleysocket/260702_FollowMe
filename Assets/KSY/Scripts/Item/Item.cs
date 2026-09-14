using SeungyungLib.CollectSystem.Core;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Md.Body.Core;
using SeungyungLib.ModuleSystem.Core;

using System.Collections;
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
            body.Recovery(itemData.RecoveryValue);

            FadeIn();
            PlayParticle();
            Delete();
        }

        private void PlayParticle()
        {
            if (Instantiate(itemData.CollectParticle).TryGetComponent(out ParticleSystem ps))
            {
                ps.transform.position = transform.position;
                ps.Play();
            }
        }

        private void FadeIn()
        {
            StartCoroutine(DoFadeIn());
        }
        private IEnumerator DoFadeIn()
        {
            Color color = spRenderer.color;
            while (color.a > 0)
            {
                color.a -= 0.01f;
                spRenderer.color = color;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}

