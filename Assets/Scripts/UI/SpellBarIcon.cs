using System.Collections;
using Controller;
using UnityEngine;
using UnityEngine.UI;

public class SpellBarIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private Image inputIcon;
    [SerializeField] private GameObject inputIconParent;
    [SerializeField] private IconDictionary iconDictionary;
    private Attack _attack;
    private void Awake()
    {
        icon.enabled = false;
        background.enabled = false;
    }
    
    public void Initialize(string input, bool raycastTarget = false)
    {
        if (iconDictionary.Dictionary.ContainsKey(input))
        {
            inputIcon.sprite = iconDictionary.Dictionary[input];
        }
        else
        {
            inputIconParent.SetActive(false);
        }

        icon.raycastTarget = raycastTarget;
    }

    private void OnCooldown(float duration)
    {
        if(isActiveAndEnabled)
            StartCoroutine(Cooldown(duration));
    }

    private IEnumerator Cooldown(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            icon.fillAmount = time / duration;
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
    }

    public void EquipAttack(Attack attack)
    {
        icon.enabled = true;
        icon.sprite = attack.Data.Icon;
        background.enabled = true;
        background.sprite = attack.Data.Icon;
        attack.OnCooldownEvent += OnCooldown;
        _attack = attack;
    }

    public void UnEquipAttack()
    {
        icon.enabled = false;
        background.enabled = false;
        if (_attack != null)
        {
            _attack.OnCooldownEvent -= OnCooldown;
            _attack = null;
        }
    }


    private void OnDestroy()
    {
        StopAllCoroutines();
        UnEquipAttack();
    }
}
