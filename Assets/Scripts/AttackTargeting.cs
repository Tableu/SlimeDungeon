using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//static class for auto aiming spells. Uses raycasts to locate enemies and returns them to the caller.
public static class AttackTargeting
{
    public static Collider SphereScan(Transform transform, float radius, LayerMask mask)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward*radius/2, radius, mask);
        List<Collider> sortedColliders = hitColliders.OrderBy(o => o.transform.position - transform.position).ToList();
        sortedColliders.RemoveAll(collider =>  collider.GetComponent<IDamageable>() == null);
        return sortedColliders.Count <= 0 ? null : sortedColliders[0];
    }

    public static List<Collider> SphereScanAll(Transform transform, float radius, LayerMask mask)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward*radius/2, radius, mask);
        List<Collider> sortedColliders = hitColliders.OrderBy(o => o.transform.position - transform.position).ToList();
        sortedColliders.RemoveAll(collider =>  collider.GetComponent<IDamageable>() == null);
        return sortedColliders;
    }

    public static void RotateTowards(Transform transform, Transform target)
    {
        var diff = target.position - transform.position;
        var targetVector = new Vector3(diff.x, target.position.y, diff.z);
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetVector, 
            Mathf.Infinity, 0.0f));
    }
}