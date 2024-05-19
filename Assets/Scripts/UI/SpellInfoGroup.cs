using System.Collections;
using System.Collections.Generic;
using Controller.Form;
using UnityEngine;

public class SpellInfoGroup : MonoBehaviour
{
    [SerializeField] private GameObject verticalGroup;
    [SerializeField] private GameObject spellInfo;
    
    public void Initialize(FormData data)
    {
        GameObject widget = Instantiate(spellInfo, verticalGroup.transform);
        SpellInfo script = widget.GetComponent<SpellInfo>();
        if (script != null)
        {
            script.Initialize(data.BasicAttack);
        }

        foreach (AttackData attackData in data.Spells)
        {
            widget = Instantiate(spellInfo, verticalGroup.transform);
            
            script = widget.GetComponent<SpellInfo>();
            if (script != null)
            {
                script.Initialize(attackData);
            }
        }
    }
}
