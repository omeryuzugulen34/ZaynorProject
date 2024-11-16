using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettingSO", menuName = "Scriptable Objects/CharacterSettingSO")]
public class CharacterSettingSO : ScriptableObject
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public float rollSpeed = 12f;
    public float rollDuration = 0.5f;
    public float rollCooldown = 1f;

    public float lightAttackDamage = 10f;
    public float heavyAttackDamage = 25f;
    public float attackCooldown = 1f;
}
