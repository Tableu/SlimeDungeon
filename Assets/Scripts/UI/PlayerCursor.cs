using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCursor : MonoBehaviour
{
    private static PlayerCursor _instance;
    public static PlayerCursor Instance => _instance;

    [SerializeField] private PartyController controller;
    [SerializeField] private Image reloadIcon;
    [SerializeField] private Image crossHairIcon;
    [SerializeField] private Image cursorIcon;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    private void Start()
    {
        UnityEngine.Cursor.visible = false;
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
