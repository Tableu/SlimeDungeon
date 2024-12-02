using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float radius;
    [SerializeField] private int maxCount = 50;
    [SerializeField] private PlayerController controller;
    private void FixedUpdate()
    {
        RaycastHit[] coinHits = new RaycastHit[maxCount];
        int coinCount = Physics.SphereCastNonAlloc(transform.position, radius, Vector3.down, coinHits, 10f, LayerMask.GetMask("Coins"));
        for (var i = 0; i < coinCount; i++)
        {
            RaycastHit hit = coinHits[i];
            hit.rigidbody.AddForce((transform.position-hit.rigidbody.position).normalized*force, ForceMode.Impulse);
            if ((hit.rigidbody.position - transform.position).magnitude < 0.4)
            {
                ResourceManager.Instance.Coins.Add(1);
                Destroy(hit.rigidbody.gameObject);
            }
        }
        
        RaycastHit[] healthHits = new RaycastHit[maxCount];
        int healthCount = Physics.SphereCastNonAlloc(transform.position, radius, Vector3.down, healthHits, 10f, LayerMask.GetMask("HealthOrbs"));
        for (var i = 0; i < healthCount; i++)
        {
            RaycastHit hit = healthHits[i];
            hit.rigidbody.AddForce((transform.position-hit.rigidbody.position).normalized*force, ForceMode.Impulse);
            if ((hit.rigidbody.position - transform.position).magnitude < 0.4)
            {
                controller.Stats.Heal(5);
                Destroy(hit.rigidbody.gameObject);
            }
        }
    }
}
