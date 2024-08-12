using UnityEngine;

public class Egg : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;
    private GameObject _enemyPrefab;
    private RoomController _roomController;
    private int _collisionCount = 0;

    public void Initialize(float damage, float knockback,float hitStun, Vector3 force, Elements.Type type, GameObject enemyPrefab,
        RoomController roomController)
    {
        _hitStun = hitStun;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        rigidbody.AddForce(force, ForceMode.Impulse);
        _enemyPrefab = enemyPrefab;
        _roomController = roomController;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            IDamageable damage = other.attachedRigidbody.gameObject.GetComponent<IDamageable>();
            if (damage != null)
            {
                damage.TakeDamage(_damage, _knockback * _force.normalized, _hitStun, _type);
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Floor") && _collisionCount <= 0)
        {
            GameObject enemy = Instantiate(_enemyPrefab, transform.position, transform.rotation,
                _roomController.transform);
            EnemyController controller = enemy.GetComponent<EnemyController>();
            _roomController.AddEnemy(controller);
            Destroy(gameObject);
            _collisionCount++;
        }
    }
}