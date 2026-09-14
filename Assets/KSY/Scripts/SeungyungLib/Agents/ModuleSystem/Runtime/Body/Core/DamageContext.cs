using UnityEngine;

namespace SeungyungLib.Md.Body.Core
{
    public readonly struct DamageContext
    {
        public DamageContext(int value, GameObject Giver)
        {
            this.Value = value;
            this.Giver = Giver;
        }
        
        public readonly int Value;
        public readonly GameObject Giver;
    }
}
