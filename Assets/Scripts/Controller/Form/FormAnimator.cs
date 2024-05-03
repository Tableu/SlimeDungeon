using Elements;
using UnityEngine;

namespace Controller.Form
{
    public abstract class FormAnimator : MonoBehaviour
    {
        protected Form form;
        protected Animator animator;
        protected PlayerInputActions inputActions;
        public abstract void Initialize(Form form, PlayerInputActions inputActions);
    }

    public enum Forms
    {
        FireForm
    }
}