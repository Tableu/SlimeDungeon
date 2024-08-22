using TMPro;
using UnityEngine;

public class InputHint : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        if (playerController.HighlightedItem != null)
        {
            switch (playerController.HighlightedItem)
            {
                case CharacterItem:
                    text.text = "[Spacebar] - Pick up character";
                    break;
                case Chest:
                    text.text = "[Spacebar] - Open chest";
                    break;
                case LevelExit:
                    text.text = "[Spacebar] - Enter portal";
                    break;
                case SpellItem:
                    text.text = "[Spacebar] - Pick up spell";
                    break;
            }
        }
        else
        {
            text.text = "";
        }
    }
}
