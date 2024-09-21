using System.Collections.Generic;
using System.Linq;
using cakeslice;
using Controller.Player;
using UnityEngine;

public class CharacterItem : MonoBehaviour, IItem
{
    [SerializeField] private GameObject modelRoot;
    [SerializeField] private bool bought;
    
    private List<Outline> _outlineScripts;
    private Chatbox _chatBox;
    private RoomController _roomController;
    private CharacterData _initialCharacterData;
    private Character _oldPartyMember;
    private GameObject _model;
    private bool _isFree;

    public void Initialize(CharacterData data)
    {
        _outlineScripts = new List<Outline>();
        _initialCharacterData = data;
        _model = Instantiate(data.Model, modelRoot.transform);
        _model.layer = LayerMask.NameToLayer("Items");
        List<Renderer> modelRenderers = gameObject.GetComponentsInChildren<Renderer>().ToList();
        foreach(Renderer modelRenderer in modelRenderers)
            _outlineScripts.Add(modelRenderer.gameObject.AddComponent<Outline>());
        Highlight(false);
    }

    private void Start()
    {
        if (!bought)
        {
            _chatBox = ChatBoxManager.Instance.SpawnChatBox(transform);
            _chatBox.SetMessage("<sprite name=\"UI_117\"> " + _initialCharacterData.Cost.ToString());
            _chatBox.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if(_chatBox != null)
            Destroy(_chatBox.gameObject);
    }

    public void PickUp(PlayerController character, InventoryController inventory, PartyController partyController)
    {
        if (!bought)
            ResourceManager.Instance.Coins.Remove(_initialCharacterData.Cost);
        Character partyMember = _oldPartyMember ?? new Character(_initialCharacterData, character, character.transform);
        _oldPartyMember = partyController.AddPartyMember(partyMember);
        if (_oldPartyMember != null)
        {
            SwitchCharacter(_oldPartyMember.Data);
            bought = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanPickup()
    {
        return bought || ResourceManager.Instance.Coins.Value >= _initialCharacterData.Cost;
    }

    public void Highlight(bool enable)
    {
        foreach (Outline script in _outlineScripts)
        {
            if(script != null)
                script.enabled = enable;
        }

        if (_chatBox != null)
            _chatBox.gameObject.SetActive(enable);
    }

    private void SwitchCharacter(CharacterData data)
    {
        if (data != null)
        {
            Destroy(_model);
            _model = Instantiate(data.Model, modelRoot.transform);
            _model.layer = LayerMask.NameToLayer("Items");
            _outlineScripts.Clear();
            List<Renderer> modelRenderers = _model.GetComponentsInChildren<Renderer>().ToList();
            foreach(Renderer modelRenderer in modelRenderers)
                _outlineScripts.Add(modelRenderer.gameObject.AddComponent<Outline>());
            Highlight(false);
        }
        _chatBox.gameObject.SetActive(false);
    }
}