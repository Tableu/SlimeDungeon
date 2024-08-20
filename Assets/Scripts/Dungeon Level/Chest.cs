using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class Chest : MonoBehaviour, IItem
{
    [SerializeField] private GameObject chestLid;
    [SerializeField] private Vector3 openedRotation;
    [SerializeField] private RandomizedChestLoot _randomChestLoot;
    [SerializeField] private GameObject spellItemPrefab;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private List<Outline> outlineScripts;

    private bool _opened;

    public void PickUp(PlayerController character)
    {
        if (!_opened)
        {
            ResourceManager.Instance.Coins.Add(_randomChestLoot.GetCoins());
            GameObject spellItem = Instantiate(spellItemPrefab, transform);
            spellItem.transform.localPosition += new Vector3(0,1,0);
            SpellItem script = spellItem.GetComponent<SpellItem>();
            if (script != null)
            {
                script.Initialize(_randomChestLoot.GetSpell());
            }
            particleSystem.Play();
            chestLid.transform.localRotation = Quaternion.Euler(openedRotation.x,openedRotation.y,openedRotation.z);
            _opened = true;
            gameObject.layer = LayerMask.NameToLayer("Obstacles");
        }
    }

    public void Highlight(bool enable)
    {
        foreach (Outline script in outlineScripts)
        {
            if(script != null)
                script.enabled = enable;
        }
    }

    public bool CanPickup()
    {
        return !_opened;
    }
}
