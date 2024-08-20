using System.Collections.Generic;
using System.Linq;
using cakeslice;
using Controller.Player;
using UnityEngine;

public class CharacterItem : MonoBehaviour, IItem
{
    [SerializeField] private GameObject cage;
    [SerializeField] private GameObject modelRoot;
    [SerializeField] private bool captured;
    [SerializeField] private bool isStoreItem; 
    
    private Collider _characterCollider;
    private Animator _animator;
    private List<Outline> _outlineScripts;
    private Chatbox _chatBox;
    private RoomController _roomController;
    private Character _character;
    private GameObject _model;
    private bool _isFree;

    public Character Character => _character;
    
    public void Initialize(RandomCharacterData randomCharacterData, RoomController roomController)
    {
        _outlineScripts = new List<Outline>();
        CharacterData data = randomCharacterData.GetRandomElement();
        _character = new Character(data);
        _model = Instantiate(data.Model, modelRoot.transform);
        _model.layer = LayerMask.NameToLayer("Items");
        List<Renderer> modelRenderers = _model.GetComponentsInChildren<Renderer>().ToList();
        foreach(Renderer modelRenderer in modelRenderers)
            _outlineScripts.Add(modelRenderer.gameObject.AddComponent<Outline>());
        Highlight(false);
        _characterCollider = _model.GetComponent<Collider>();
        _characterCollider.enabled = !captured;
        _animator = _model.GetComponent<Animator>();
        if(captured)
            SetCaptured(roomController);
    }

    private void Start()
    {
        if (captured)
        {
            _chatBox = ChatBoxManager.Instance.SpawnChatBox(transform);
            _chatBox.SetMessage("Help!");
        }
        else
        {
            _chatBox = ChatBoxManager.Instance.SpawnChatBox(transform);
            _chatBox.SetMessage("<sprite name=\"UI_117\"> "+_character.Data.Cost.ToString());
        }
    }

    private void Update()
    {
        if ((_isFree || !captured) && GlobalReferences.Instance.Player != null)
        {
            AttackTargeting.RotateTowards(transform, GlobalReferences.Instance.Player.transform);
        }
    }

    private void OnDestroy()
    {
        if(_roomController != null)
            _roomController.OnAllEnemiesDead -= FreeCharacter;
        if(_chatBox != null)
            Destroy(_chatBox.gameObject);
    }
    
    private void SetCaptured(RoomController roomController)
    {
        if(cage != null)
            cage.SetActive(true);
        _roomController = roomController;
        _roomController.OnAllEnemiesDead += FreeCharacter;
    }

    public void PickUp(PlayerController character)
    {
        //slightly hacky solution here
        PartyController partyController = character.GetComponent<PartyController>();
        if (partyController == null)
            return;
        Character oldCharacter = partyController.AddPartyMember(Character);
        if (oldCharacter != null)
        {
            SwitchCharacter(oldCharacter);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanPickup()
    {
        if (isStoreItem)
        {
            return ResourceManager.Instance.Coins.Value >= _character.Data.Cost;
        }
        return _isFree;
    }

    public void Highlight(bool enable)
    {
        foreach (Outline script in _outlineScripts)
        {
            if(script != null)
                script.enabled = enable;
        }
    }

    private void SwitchCharacter(Character newCharacter)
    {
        if (newCharacter != null)
        {
            _character = newCharacter;
            Destroy(_model);
            _model = Instantiate(_character.Data.Model, modelRoot.transform);
            _model.layer = LayerMask.NameToLayer("Items");
            _characterCollider = _model.GetComponent<Collider>();
            _animator = _model.GetComponent<Animator>();
            _outlineScripts.Clear();
            List<Renderer> modelRenderers = _model.GetComponentsInChildren<Renderer>().ToList();
            foreach(Renderer modelRenderer in modelRenderers)
                _outlineScripts.Add(modelRenderer.gameObject.AddComponent<Outline>());
            Highlight(false);
        }
        _chatBox.gameObject.SetActive(false);
    }

    private void FreeCharacter()
    {
        if(cage != null)
            cage.SetActive(false);
        _characterCollider.enabled = true;
        _isFree = true;
        _animator.Play("Jump");
        _chatBox.SetMessage("Thank You!");
    }
}