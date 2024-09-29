using UnityEngine;

public class EnemyExperience : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    private PartyController _partyController;
    private void Start()
    {
        _partyController = FindObjectOfType<PartyController>();
    }

    private void OnDestroy()
    {
        if(_partyController != null)
            _partyController.AddExperience(data.Experience);
    }
}
