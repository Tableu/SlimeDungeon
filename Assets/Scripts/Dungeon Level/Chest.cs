using UnityEngine;

public class Chest : MonoBehaviour, IItem
{
    [SerializeField] private GameObject chestLid;
    [SerializeField] private Vector3 openedRotation;
    [SerializeField] private RandomizedChestLoot _randomChestLoot;
    [SerializeField] private GameObject spellItemPrefab;

    private bool _opened;

    public void PickUp(PlayerController character)
    {
        if (!_opened)
        {
            ResourceManager.Instance.Coins.Add(_randomChestLoot.GetCoins());
            GameObject spellItem = Instantiate(spellItemPrefab);
            spellItem.transform.position = transform.position + new Vector3(0,1,1);
            SpellItem script = spellItem.GetComponent<SpellItem>();
            if (script != null)
            {
                script.Initialize(_randomChestLoot.GetSpell());
            }
            
            chestLid.transform.rotation = Quaternion.Euler(openedRotation.x,openedRotation.y,openedRotation.z);
            _opened = true;
            gameObject.layer = LayerMask.NameToLayer("Obstacles");
        }
    }
}
