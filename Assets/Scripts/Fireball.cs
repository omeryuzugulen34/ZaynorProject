using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float damage = 20f; // The amount of damage this fireball deals
    public float lifetime = 5f; // Time before the fireball is destroyed if it doesn't hit anything

    private void Start()
    {
        // Automatically destroy the fireball after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the fireball hit the player
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Deal damage to the player
                playerHealth.TakeDamage(damage);
                Debug.Log($"Fireball hit the player and dealt {damage} damage.");
            }

            // Destroy the fireball after it hits the player
            Destroy(gameObject);
        }
        
    }
}
