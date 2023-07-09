using UnityEngine;

namespace Controller
{
    public abstract class Form : MonoBehaviour
    {
        protected FormData _data;
        protected Character _character;
        public abstract void Equip(Character character, FormData data);
        public abstract void Drop();
    }
}