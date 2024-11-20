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
                case CapturedSlimeItem:
                    text.text = "[F] - Free slime";
                    break;
                case CharacterItem:
                    text.text = "[F] - Pick up character";
                    break;
                case Chest:
                    text.text = "[F] - Open chest";
                    break;
                case LevelExit:
                case HubExit:
                    text.text = "[F] - Enter portal";
                    break;
                case SpellItem:
                    text.text = "[F] - Pick up spell";
                    break;
                case HatItem:
                    text.text = "[F] - Pick up hat";
                    break;
            }
        }
        else
        {
            text.text = "";
        }
    }
}
