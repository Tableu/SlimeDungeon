using System;
using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "Form Dictionary", menuName = "Form Dictionary")]
[Serializable]
public class FormDataDictionary : ScriptableObject
{
    [SerializeField] private SerializableDictionary<string, FormData> dataObjects;

    public SerializableDictionary<string, FormData> Dictionary => dataObjects;
}