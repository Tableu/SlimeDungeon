using System.Collections.Generic;
using System.Linq;
using cakeslice;
using Controller.Player;
using UnityEngine;

public class CapturedSlimeItem : MonoBehaviour, IItem
{
    [SerializeField] private GameObject cage;
    [SerializeField] private GameObject modelRoot;
    
    private Collider _characterCollider;
    private Animator _animator;
    private List<Outline> _outlineScripts;
    private Chatbox _chatBox;
    private RoomController _roomController;
    private GameObject _model;
    private bool _isFree;
    private bool _bought;

    public void Initialize(RandomCharacterData randomCharacterData)
    {
        CharacterData data = randomCharacterData.GetRandomElement();
        _outlineScripts = new List<Outline>();
        _model = Instantiate(data.Model, modelRoot.transform);
        _model.layer = LayerMask.NameToLayer("Items");
        List<Renderer> modelRenderers = gameObject.GetComponentsInChildren<Renderer>().ToList();
        foreach(Renderer modelRenderer in modelRenderers)
            _outlineScripts.Add(modelRenderer.gameObject.AddComponent<Outline>());
        Highlight(false);
        _characterCollider = _model.GetComponent<Collider>();
        _characterCollider.enabled = false;
        _animator = _model.GetComponent<Animator>();
        if(cage != null)
            cage.SetActive(true);
    }
    
    private void Start()
    {
        _chatBox = ChatBoxManager.Instance.SpawnChatBox(transform);
        _chatBox.SetMessage("Help!");
    }
    
    private void Update()
    {
        if ((_isFree) && GlobalReferences.Instance.Player != null)
        {
            AttackTargeting.RotateTowards(transform, GlobalReferences.Instance.Player.transform);
        }
    }
    
    private void FreeCharacter()
    {
        if (_isFree)
            return;
        PlayerPrefs.SetInt("SlimesSaved", PlayerPrefs.GetInt("SlimesSaved") + 1);
        if(cage != null)
            cage.SetActive(false);
        _characterCollider.enabled = true;
        _isFree = true;
        _animator.Play("Jump");
        _chatBox.SetMessage("Thank You!");
    }

    public void PickUp(PlayerController character, InventoryController inventory)
    {
        FreeCharacter();
    }

    public void Highlight(bool enable)
    {
        foreach (Outline script in _outlineScripts)
        {
            if(script != null)
                script.enabled = enable;
        }
    }
    
    public bool CanPickup()
    {
        return !_isFree;
    }
}
