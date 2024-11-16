using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
   public Transform currentTarget;
    public float lockOnRange = 10f; 

    public LayerMask enemyLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            LockOnTarget();
        }

        if (currentTarget != null)
        {
            
            RotateTowardTarget();
        }
    }

    void LockOnTarget()
    {
     
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, lockOnRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            Transform nearestEnemy = null;
            float closestDistance = Mathf.Infinity;

          
            foreach (Collider enemy in enemiesInRange)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    nearestEnemy = enemy.transform;
                }
            }

     
            currentTarget = nearestEnemy;
            Debug.Log($"Locked onto: {currentTarget.name}");
        }
        else
        {
            // No enemies in range
            currentTarget = null;
            Debug.Log("No target found.");
        }
    }

    void RotateTowardTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected()
    {
     
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lockOnRange);
    }
}
