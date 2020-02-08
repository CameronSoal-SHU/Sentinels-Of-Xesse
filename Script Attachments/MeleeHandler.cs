using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeleeHandler
{
    [Header("Melee Settings")]
    [SerializeField] private float m_meleeDamage = 10f;
    // How long between each melee attack (must be above 0 for obvious reasons)
    [SerializeField] private float m_meleeCooldown = 0.5f;  // How long before you can melee again
    [SerializeField, Range(0f, 5f)] private float m_meleeSpeedMult = 1f;
    [SerializeField] private float m_attackRange = 1.0f;

    private AnimatorController m_animatorController;
    [Header("Melee Hitbox Settings")]
    [SerializeField] private Vector2 m_hitboxOffset = default;
    [SerializeField] private LayerMask m_targetCollisionMask = default;

    public float meleeCooldownRemaining { get; set; }
    public bool canMelee { get => meleeCooldownRemaining <= 0f; }


    public void Init(AnimatorController animatorController)
    {
        m_animatorController = animatorController;
		if (m_targetCollisionMask == LayerMask.GetMask("Nothing"))
			Debug.Log("Warning! Target layer mask set to Nothing");
    }


	// Retrieves all gameobjects with the target collision mask within the melee hitbox
    public Collider2D[] GetEnemiesInAttack(BaseCharacter baseCharacter)
    {
        BoxCollider2D boxCollider2D = baseCharacter.GetCharController2D().GetBoxCollider2D();

        Vector2 hitboxSize = (Vector2)boxCollider2D.bounds.size + new Vector2(m_attackRange, 0);
		Vector2 hitBoxOrigin = boxCollider2D.bounds.center;

		// Check which direction the character is facing at the time to apply the offset correctly
		if (baseCharacter.GetCharacterDirection() == BaseCharacter.char_direction.RIGHT)
			hitBoxOrigin += m_hitboxOffset;
		else
			hitBoxOrigin -= m_hitboxOffset;

        Collider2D[] collidedEnemies = Physics2D.OverlapBoxAll(hitBoxOrigin, hitboxSize, 0f, m_targetCollisionMask);

        return collidedEnemies;
    }


    // Public Accessors
    public float GetMeleeDamage() => m_meleeDamage;
    public void SetMeleeDamage(float newMeleeDamage) => m_meleeDamage = newMeleeDamage;

    public float GetMeleeSpeedMult() => m_meleeSpeedMult;
    public void SetMeleeSpeedMult(float newSpeedMult)
    {
        m_meleeSpeedMult = newSpeedMult;
        m_animatorController.SetFloat("meleeSpeedMult", m_meleeSpeedMult);
    }

    public float GetAttackRange() => m_attackRange;
    public void SetAttackRange(float newAttackRange) => m_attackRange = newAttackRange;

    public Vector2 GetHitboxOffset() => m_hitboxOffset;
    public void GetHitboxOffset(Vector2 newOffset) => m_hitboxOffset = newOffset;

    public float GetMeleeCooldown() => m_meleeCooldown;
    public void SetMeleeCooldown(float newCooldown) => m_meleeCooldown = newCooldown;

    public LayerMask GetEnemyLayerMask() => m_targetCollisionMask;
    // Is the melee ready to be used?
    public void ResetCooldown() => meleeCooldownRemaining = m_meleeCooldown * (1 / m_meleeSpeedMult);
}
