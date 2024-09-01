using UnityEngine;

namespace Controller.Player
{
    public abstract class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private Transform hatRoot;
        protected Character Character;
        protected Animator animator;
        protected PlayerInputActions inputActions;
        private GameObject _hat;
        public abstract void Initialize(Character character, PlayerInputActions inputActions);

        public virtual void RefreshEquipment(EquipmentData equipmentData)
        {
            if(_hat != null)
                Destroy(_hat);
            if(equipmentData != null)
                _hat = Instantiate(equipmentData.Model, hatRoot);
        }
    }
}