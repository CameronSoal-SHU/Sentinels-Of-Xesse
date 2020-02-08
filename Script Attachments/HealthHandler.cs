using UnityEngine;
using System.Collections;

[System.Serializable]
public class HealthHandler
{
    [SerializeField, Header("Health Settings")] private float m_currentHealth = 0;
    [SerializeField] private float m_maxHealth = 0;
    [SerializeField] private float m_healthRegenAmount = 0;
    [SerializeField] private float m_healthRegenDelay = 0;
    public float healthRegenDelayRemaining { get; set; }
    public bool healthRegenEnabled { get; set; }

    // Hehe English spelling
    [SerializeField, Header("Armour Settings")] private float m_currentArmour = 0;

    [SerializeField, Header("Damage Effect Settings")] private Color m_damageColor = Color.red;
    [SerializeField] private float m_damageEffectDuration = 0.0f;

    public delegate void DieEvent();
    public DieEvent Die;


    // Temporarily change the sprites colour upon taking damage
    public IEnumerator ApplyDamageEffect(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = m_damageColor;
        // Wait for the damage effect duration
        yield return new WaitForSeconds(m_damageEffectDuration);

        // Reset back to original colour
        // Make sure the reference was not destroyed
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1, 1, 1, 1);
    }


    // Generic damage and heal functions
    public void Damage(float damageAmount)
    {
        m_currentHealth = Mathf.Clamp(m_currentHealth - damageAmount, 0, m_maxHealth);
    }
    
    public void Heal(float healAmount) => m_currentHealth = Mathf.Clamp(m_currentHealth + healAmount, 0, m_maxHealth);

    // Public Accessors
    public float GetCurHealth() => m_currentHealth;
    public void SetCurHealth(float newHealthVal) => m_currentHealth = newHealthVal;

    public float GetMaxHealth() => m_maxHealth;
    public void SetMaxHealth(float newMaxHealthVal) => m_maxHealth = newMaxHealthVal;

    public float healthRatio { get => m_currentHealth / m_maxHealth; }

    public float GetHealthRegenAmount() => m_healthRegenAmount;
    public void SetHealthRegenAmount(float newRegenVal) => m_healthRegenAmount = newRegenVal;

    public float GetHealthRegenDelay() => m_healthRegenDelay;
    public void SetHealthRegenDelay(float newRegenDelayVal) => m_healthRegenDelay = newRegenDelayVal;

    public void ResetHealthRegenDelay() => healthRegenDelayRemaining = m_healthRegenDelay;

    public float GetArmourValue() => m_currentArmour;
    public void SetArmourValue(float newArmourVal) => m_currentArmour = newArmourVal;

    public bool isDead { get => m_currentHealth <= 0; }
}
