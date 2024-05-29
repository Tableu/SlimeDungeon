using Controller;
using Controller.Character;
using UnityEngine;

public class CapturedCharacter : MonoBehaviour
{
    [SerializeField] private GameObject cage;
    [SerializeField] private GameObject character;
    [SerializeField] private Collider characterCollider;
    private RoomController _roomController;
    private Character _character;

    public Character Character => _character;
    
    public void Initialize(RoomController roomController, CharacterData characterData)
    {
        _roomController = roomController;
        _roomController.OnAllEnemiesDead += FreeCharacter;
        _character = new Character(characterData);
        characterCollider.enabled = false;
    }

    public void SwitchCharacter(Character newCharacter)
    {
        if (newCharacter != null)
        {
            _character = newCharacter;
            Destroy(character.gameObject);
            character = Instantiate(_character.Data.Model, transform);
            character.layer = LayerMask.NameToLayer("Items");
        }
    }

    private void FreeCharacter()
    {
        cage.SetActive(false);
        characterCollider.enabled = true;
    }

    private void OnDestroy()
    {
        _roomController.OnAllEnemiesDead -= FreeCharacter;
    }
}
