using UnityEngine;

namespace Controller
{
    public abstract class Form : MonoBehaviour
    {
        protected FormData data;
        protected Character character;
        internal float damageMultiplier = 1;
        internal float sizeMultiplier = 1;
        internal float speedMultiplier = 1;
        public abstract void Equip(Character character, FormData data);
        public abstract void Drop();
    }
}