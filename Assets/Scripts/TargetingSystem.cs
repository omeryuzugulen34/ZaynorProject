using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    public Transform currentTarget;
    public float targetingRange = 10f;
    public LayerMask enemyLayer;

    private void Update()
    {
        UpdateClosestTarget();

        if (currentTarget != null)
        {
           // HighlightTarget(currentTarget, true);
        }
    }

    private void UpdateClosestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, targetingRange, enemyLayer);

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = collider.transform;
            }
        }

        if (currentTarget != closestEnemy)
        {
            if (currentTarget != null)
            {
               // HighlightTarget(currentTarget, false);
            }

            currentTarget = closestEnemy;
            if (currentTarget != null)
            {
                Debug.Log($"New target acquired: {currentTarget.name}");
            }
        }
    }

    // private void HighlightTarget(Transform target, bool highlight)
    // {
    //     Renderer renderer = target.GetComponent<Renderer>();
    //     if (renderer != null)
    //     {
    //         renderer.material.color = highlight ? Color.yellow : Color.white;
    //     }
    // }
    
}