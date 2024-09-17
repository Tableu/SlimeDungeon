using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCursor : MonoBehaviour
{
    [SerializeField] private PartyController controller;
    [SerializeField] private Image reloadIcon;
    [SerializeField] private Image crossHairIcon;
    [SerializeField] private Image cursorIcon;
    
    private void Start()
    {
        Cursor.visible = false;
        if (controller != null)
        {
            controller.CurrentCharacter.BasicAttack.OnCooldownEvent += OnCooldown;
            controller.OnCharacterChanged += delegate
            {
                //We don't need to unsubscribe since the basic attack will be destroyed before we are able to do so
                StopCoroutine(Cooldown(0));
                controller.CurrentCharacter.BasicAttack.OnCooldownEvent += OnCooldown;
            };
        }
    }
    private void Update()
    {
        transform.position = Mouse.current.position.ReadValue();
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StopCoroutine(ClickAnimation());
            StartCoroutine(ClickAnimation());
        }
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
        reloadIcon.fillAmount = 1;
    }

    private IEnumerator ClickAnimation()
    {
        cursorIcon.transform.localScale = Vector3.one * 0.9f;
        yield return new WaitForSeconds(0.2f);
        cursorIcon.transform.localScale = Vector3.one;
    }

    public void SwitchToCrossHair()
    {
        crossHairIcon.enabled = true;
        reloadIcon.enabled = true;
        cursorIcon.enabled = false;
    }

    public void SwitchToCursor()
    {
        cursorIcon.enabled = true;
        crossHairIcon.enabled = false;
        reloadIcon.enabled = false;
    }
}
