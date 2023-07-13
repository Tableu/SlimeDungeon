using UnityEngine;

namespace Controller.Form
{
    public abstract class Form : MonoBehaviour
    {
        internal FormData data;
        protected Character character;
        internal float damageMultiplier = 1;
        internal float sizeMultiplier = 1;
        internal float speedMultiplier = 1;
        public abstract void Equip(Character character);
        public abstract void Drop();
    }

    public enum Forms
    {
        FireForm
    }
}