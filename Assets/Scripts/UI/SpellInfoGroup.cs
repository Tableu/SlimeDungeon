using System.Collections.Generic;
using Controller.Form;
using UnityEngine;

public class SpellInfoGroup : MonoBehaviour
{
    [SerializeField] private GameObject verticalGroup;
    [SerializeField] private GameObject spellInfo;
    private List<GameObject> _spellInfos = new List<GameObject>();
    public void SetCharacter(FormData data)
    {
        if (_spellInfos.Count > 0)
        {
            foreach (GameObject spellInfo in _spellInfos)
            {
                Destroy(spellInfo);
            }
            _spellInfos.Clear();
        }
        
        GameObject widget = Instantiate(spellInfo, verticalGroup.transform);
        _spellInfos.Add(widget);
        SpellInfo script = widget.GetComponent<SpellInfo>();
        if (script != null)
        {
            script.Initialize(data.BasicAttack);
        }

        foreach (AttackData attackData in data.Spells)
        {
            widget = Instantiate(spellInfo, verticalGroup.transform);
            _spellInfos.Add(widget);
            script = widget.GetComponent<SpellInfo>();
            if (script != null)
            {
                script.Initialize(attackData);
            }
        }
    }
}
