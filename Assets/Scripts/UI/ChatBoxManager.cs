using UnityEngine;

public class ChatBoxManager : MonoBehaviour
{
    private static ChatBoxManager _instance;

    public static ChatBoxManager Instance => _instance;

    [SerializeField] private GameObject chatBoxPrefab;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Camera canvasCamera;
    
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

    public Chatbox SpawnChatBox(Transform character)
    {
        Vector3 pos = canvasCamera.WorldToScreenPoint(character.position);
        GameObject chatBox =
            Instantiate(chatBoxPrefab, pos, Quaternion.identity, transform);
        Chatbox script = chatBox.GetComponent<Chatbox>();
        script.Initialize(character, offset, canvasCamera);
        return script;
    }
}
