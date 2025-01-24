using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 5f; // Radius within which the enemy stops and attacks
    public float followRange = 10f; // Radius within which the enemy follows the player
    public float attackDamage = 10f; // Damage dealt per attack
    public float attackCooldown = 1.5f; // Time between attacks

    [Header("Fireball Settings")]
    public GameObject fireballPrefab; // Fireball prefab to spawn
    public Transform fireballSpawnPoint; // Child transform where the fireball will spawn
    public float fireballSpeed = 10f; // Speed of the fireball

    private Transform player; // Reference to the player's transform
    private bool canAttack = true;
    private Renderer enemyRenderer;
    private Color originalColor;
    private bool isFlashing = false;

    private Rigidbody rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRenderer = GetComponent<Renderer>();
        originalColor = enemyRenderer.material.color;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Within attack range, stop and attack
            StopAndAttack();
        }
        else if (distanceToPlayer <= followRange)
        {
            // Within follow range, chase the player
            ChasePlayer();
        }
        else
        {
            // Out of follow range, stop chasing
            StopChasing();
        }
    }

    private void StopAndAttack()
    {
        // Stop moving
        rb.linearVelocity = Vector3.zero;

        // Face the player
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // Attack the player
        if (canAttack)
        {
            StartCoroutine(ShootFireball());
        }
    }

    private IEnumerator ShootFireball()
    {
        canAttack = false;

    // Spawn the fireball
    if (fireballPrefab != null && fireballSpawnPoint != null)
    {
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);

        // Add velocity to the fireball
        Rigidbody fireballRb = fireball.GetComponent<Rigidbody>();
        if (fireballRb != null)
        {
            fireballRb.linearVelocity = fireballSpawnPoint.forward * fireballSpeed;
        }
    }

    // Cooldown before the next attack
    yield return new WaitForSeconds(attackCooldown);
    canAttack = true;
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * Time.deltaTime);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    private void StopChasing()
    {
        rb.linearVelocity = Vector3.zero; // Stop the enemy
    }

    public void TakeDamage(float damage, Vector3 knockbackDirection)
    {
        Debug.Log($"{name} is taking {damage} damage!");
        if (!isFlashing)
        {
            StartCoroutine(FlashRed());
        }

        // Apply knockback
        rb.AddForce(knockbackDirection * 5f, ForceMode.Impulse);

        // Reduce health (assuming Health script is attached)
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }

    private IEnumerator FlashRed()
    {
        isFlashing = true;

        // Change color to red
        enemyRenderer.material.color = Color.red;

        // Wait for a short duration
        yield return new WaitForSeconds(0.3f);

        // Revert to original color
        enemyRenderer.material.color = originalColor;
        isFlashing = false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw the follow range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}
