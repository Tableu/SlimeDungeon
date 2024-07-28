using System;
using System.Collections;
using Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellBarIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private Image inputIcon;
    [SerializeField] private IconDictionary iconDictionary;
    private int _index;
    private Attack _attack;

    public int Index => _index;
    private void Awake()
    {
        icon.enabled = false;
        background.enabled = false;
    }
    
    public void Initialize(int index, string input)
    {
        _index = index;
        inputIcon.sprite = iconDictionary.Dictionary[input];
    }

    private void OnCooldown(float duration)
    {
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
