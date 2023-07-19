using System;
using System.Collections.Generic;
using Elements;
using UnityEngine;

[CreateAssetMenu(fileName = "Element Type Manager", menuName = "Data/Element Type Manager")]
[Serializable]
public class ElementTypeManager : ScriptableObject
{
    [SerializeField] private List<ElementTypeData> elementTypeData;

    public float GetTypeMultiplier(Elements.Type characterType, Elements.Type attackType)
    {
        float multiplier = 1;
        foreach (ElementTypeData data in elementTypeData)
        {
            if ((data.ElementType & attackType) != Elements.Type.None)
            {
                if ((data.Advantage & characterType) != Elements.Type.None)
                {
                    multiplier *= 2f;
                }
                if ((data.Disadvantage & characterType) != Elements.Type.None)
                {
                    multiplier /= 2f;
                }
            }
        }
        return multiplier;
    }
}
