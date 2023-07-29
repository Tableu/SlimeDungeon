using Elements;
using UnityEngine;

namespace Controller.Form
{
    public abstract class Form : MonoBehaviour
    {
        internal FormData data;
        //todo move multiplier logic to new feature
        internal float damageMultiplier = 1;
        internal float sizeMultiplier = 1;
        internal float speedMultiplier = 1;
        internal float health;
        internal float speed;
        internal Type elementType;
        protected Animator animator;
        public abstract void Equip(PlayerController playerController);
        public abstract void Drop();
        public abstract void Attack();
    }

    public class SavedForm
    {
        private FormData _data;
        public FormData Data => _data;
        public float Health;

        public SavedForm(FormData data)
        {
            _data = data;
            Health = data.Health;
        }
    }

    public enum Forms
    {
        FireForm
    }
}