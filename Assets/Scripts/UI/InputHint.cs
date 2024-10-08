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
                    text.text = "[Spacebar] - Free slime";
                    break;
                case CharacterItem:
                    text.text = "[Spacebar] - Pick up character";
                    break;
                case Chest:
                    text.text = "[Spacebar] - Open chest";
                    break;
                case LevelExit:
                case HubExit:
                    text.text = "[Spacebar] - Enter portal";
                    break;
                case SpellItem:
                    text.text = "[Spacebar] - Pick up spell";
                    break;
                case HatItem:
                    text.text = "[Spacebar] - Pick up hat";
                    break;
            }
        }
        else
        {
            text.text = "";
        }
    }
}
