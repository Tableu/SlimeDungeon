using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float radius;
    [SerializeField] private int maxCount = 50;
    private void FixedUpdate()
    {
        RaycastHit[] hits = new RaycastHit[maxCount];
        int count = Physics.SphereCastNonAlloc(transform.position, radius, Vector3.down, hits, 10f, LayerMask.GetMask("Coins"));
        for (var i = 0; i < count; i++)
        {
            RaycastHit hit = hits[i];
            hit.rigidbody.AddForce((transform.position-hit.rigidbody.position).normalized*force, ForceMode.Impulse);
            if ((hit.rigidbody.position - transform.position).magnitude < 0.4)
            {
                ResourceManager.Instance.Coins.Add(1);
                Destroy(hit.rigidbody.gameObject);
            }
        }
    }
}
