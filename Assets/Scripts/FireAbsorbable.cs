using System.Collections;
using System.Collections.Generic;
using Controller.Form;
using UnityEngine;

public class FireAbsorbable : MonoBehaviour, IAbsorbable
{
    [SerializeField] private FormData data;

    public void Absorb(PlayerController character)
    {
        character.EquipForm(data);
        Destroy(gameObject);
    }
}
