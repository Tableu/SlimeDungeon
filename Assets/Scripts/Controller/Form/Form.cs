using UnityEngine;

namespace Controller.Form
{
    public abstract class Form : MonoBehaviour
    {
        internal FormData data;
        protected PlayerController playerController;
        internal float damageMultiplier = 1;
        internal float sizeMultiplier = 1;
        internal float speedMultiplier = 1;
        public abstract void Equip(PlayerController playerController);
        public abstract void Drop();
    }

    public enum Forms
    {
        FireForm
    }
}