using System.Collections.Generic;
using Controller.Form;
using UnityEngine;

public class FormBar : MonoBehaviour
{
    [SerializeField] private GameObject formIconPrefab;
    [SerializeField] private PlayerController playerController;
    private List<FormBarIcon> formIcons;

    private void Start()
    {
        formIcons = new List<FormBarIcon>();
        for(int i = 0; i < playerController.FormManager.MaxFormCount; i++)
        {
            GameObject icon = Instantiate(formIconPrefab, transform);
            var script = icon.GetComponent<FormBarIcon>();
            if (script != null)
            {
                formIcons.Add(script);
                if(playerController.FormManager.Forms.Count > i)
                    script.SetIcon(playerController.FormManager.Forms[i]);
            }
        }

        playerController.FormManager.OnFormAdd += OnFormChange;
    }

    private void OnFormChange(Form formInstance, int index)
    {
        formIcons[index].SetIcon(formInstance);
    }

    private void OnDestroy()
    {
        playerController.FormManager.OnFormAdd -= OnFormChange;
    }
}
