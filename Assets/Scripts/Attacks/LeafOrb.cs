using UnityEngine;
using Type = Elements.Type;

public class LeafOrb : MonoBehaviour, BasicProjectile
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private AttackData leafAttackData;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;
    private int _mask;

    public void Initialize(float damage, float knockback, float hitStun, Vector3 force, Type type)
    {
        _hitStun = hitStun;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        rigidbody.AddForce(force, ForceMode.Impulse);
        if (gameObject.layer == LayerMask.NameToLayer("PlayerAttacks"))
        {
            _mask = LayerMask.GetMask("Walls", "Obstacles", "Enemy");
        }
        else if (gameObject.layer == LayerMask.NameToLayer("EnemyAttacks"))
        {
            _mask = LayerMask.GetMask("Walls", "Obstacles", "Player");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_mask == (_mask | (1 << other.gameObject.layer)))
        {
            int angle = 0;
            while (angle < 360)
            {
                Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.up);
                GameObject leaf = Instantiate(leafAttackData.Prefab, transform.position+angleAxis*Vector3.right*0.25f, Quaternion.Euler(0,angle,0));
                leaf.layer = gameObject.layer;
                Leaf script = leaf.GetComponent<Leaf>();
                if(script != null)
                    script.Initialize(leafAttackData.Damage, leafAttackData.Knockback, leafAttackData.HitStun,
                        angleAxis*Vector3.right*leafAttackData.Speed, leafAttackData.ElementType);
                angle += 20;
            }
        }
        Destroy(gameObject);
    }
}
