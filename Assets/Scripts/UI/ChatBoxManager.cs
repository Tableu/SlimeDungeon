using UnityEngine;

public class ChatBoxManager : MonoBehaviour
{
    private static ChatBoxManager _instance;

    public static ChatBoxManager Instance => _instance;

    [SerializeField] private GameObject chatBoxPrefab;
    [SerializeField] private Vector3 offset;
    
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
        GameObject chatBox =
            Instantiate(chatBoxPrefab, character.position + offset, chatBoxPrefab.transform.rotation, transform);
        Chatbox script = chatBox.GetComponent<Chatbox>();
        script.Initialize();
        return script;
    }
}
