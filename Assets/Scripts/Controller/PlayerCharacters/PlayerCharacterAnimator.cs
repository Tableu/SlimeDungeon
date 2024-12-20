using UnityEngine;

namespace Controller.Player
{
    public abstract class PlayerCharacterAnimator : MonoBehaviour
    {
        [SerializeField] private Transform hatRoot;
        protected PlayerCharacter PlayerCharacter;
        protected Animator animator;
        protected PlayerInputActions inputActions;
        private GameObject _hat;
        public abstract void Initialize(PlayerCharacter playerCharacter, PlayerInputActions inputActions);

        public virtual void RefreshHat(EquipmentData equipmentData)
        {
            if(_hat != null)
                Destroy(_hat);
            if(equipmentData != null)
                _hat = Instantiate(equipmentData.Model, hatRoot);
        }
    }
}