using System;
using UnityEngine;

namespace Controller.Form
{
    [Serializable]
    public abstract class FormData : ScriptableObject
    {
        [HeaderAttribute("Stats")]
        [SerializeField] private Elements.Type elementType;
        [SerializeField] private float health;
        [SerializeField] private float speed;
        [SerializeField] private float captureMultiplier;
        
        [HeaderAttribute("References")]
        [SerializeField] private GameObject model;
        [SerializeField] private GameObject item;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector3 spellOffset;
        [SerializeField] private AttackData basicAttack;

        public Elements.Type ElementType => elementType;
        public float Health => health;
        public float Speed => speed;
        public float CaptureMultiplier => captureMultiplier;
        public GameObject Model => model;
        public Vector3 SpellOffset => spellOffset;
        public GameObject Item => item;
        public Sprite Icon => icon;
        public AttackData BasicAttack => basicAttack;

        public abstract FormAnimator AttachScript(GameObject gameObject);
    }
}