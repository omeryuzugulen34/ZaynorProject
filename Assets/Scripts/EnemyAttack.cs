using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 1f;  // Range within which the enemy can attack
    public float attackDamage = 10f;  // Damage dealt per attack
    public float attackCooldown = 1.5f;  // Time between attacks
    public float knockbackForce = 5f;  // Force to push the enemy back when hit

    private Transform player;  // Reference to the player's transform
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

        // Check if within attack range
        if (distanceToPlayer <= attackRange)
        {
            if (canAttack)
            {
                StartCoroutine(AttackPlayer());
            }
        }
        else
        {
            // Move toward the player
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * Time.deltaTime;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;

        // Simulate an attack (add an animation trigger here if needed)
        Debug.Log("Enemy attacks the player!");

        // Check if the player has a Health component and apply damage
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        // Wait for the cooldown before allowing another attack
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(float damage, Vector3 knockbackDirection)
    {
        Debug.Log($"{name} is taking {damage} damage!");
        // Flash red to indicate damage
        if (!isFlashing)
        {
            StartCoroutine(FlashRed());
        }

        // Apply knockback
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }  
}
