using Elements;
using UnityEngine;

namespace Controller.Form
{
    public abstract class FormAnimator : MonoBehaviour
    {
        protected Form form;
        protected Animator animator;
        public abstract void Initialize(Form form);
        public abstract void Attack();
    }

    public enum Forms
    {
        FireForm
    }
}