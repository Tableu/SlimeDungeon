using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private PartyController controller;
    [SerializeField] private Image reloadIcon;
    private void Start()
    {
        controller.CurrentCharacter.BasicAttack.OnCooldownEvent += OnCooldown;
        controller.OnCharacterChanged += delegate
        {
            //We don't need to unsubscribe since the basic attack will be destroyed before we are able to do so
            StopCoroutine(Cooldown(0));
            controller.CurrentCharacter.BasicAttack.OnCooldownEvent += OnCooldown;
        };
    }
    private void Update()
    {
        transform.position = Mouse.current.position.ReadValue();
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
            reloadIcon.fillAmount = time / duration;
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
    }
}
