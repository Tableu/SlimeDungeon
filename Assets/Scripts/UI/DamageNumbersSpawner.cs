using UnityEngine;

public class DamageNumbersSpawner : MonoBehaviour
{
    [SerializeField] private GameObject damageNumbersPrefab;

    private void Start()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        EnemyController enemyController = GetComponent<EnemyController>();
        if (playerController != null)
        {
            playerController.OnDamage += SpawnDamageNumber;
        }
        else if (enemyController != null)
        {
            enemyController.OnDamage += SpawnDamageNumber;
        }
    }

    private void SpawnDamageNumber(int damage)
    {
        Vector3 offset = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(0.9f, 1.0f), Random.Range(-0.25f, 0.25f));
        GameObject text = Instantiate(damageNumbersPrefab, transform.position + offset, Quaternion.identity);
        DamageNumberText script = text.GetComponent<DamageNumberText>();
        script.Initialize(damage, Color.white);
    }
}
