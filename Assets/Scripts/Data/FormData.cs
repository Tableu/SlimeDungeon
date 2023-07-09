using System.Collections.Generic;
using Controller;
using UnityEngine;

public abstract class FormData : ScriptableObject
{
    [SerializeField] private List<Attack> attacks;

    public List<Attack> Attacks => attacks;
    
    public abstract Form AttachScript(GameObject gameObject);
}
