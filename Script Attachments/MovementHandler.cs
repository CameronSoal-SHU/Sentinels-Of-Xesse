using UnityEngine;

[System.Serializable]
public class MovementHandler
{
    [Header("Movement Settings")]
    [SerializeField] private float m_movementSpeed = 0;
    [SerializeField] private float m_jumpMovementSpeed = 0;
    [SerializeField] private float m_accelerationDuration = 0;
    [SerializeField] private float m_jumpAccelerationDuration = 0;
    [SerializeField, Header("Jump Settings")] protected float m_jumpHeight = 5;

    public Vector2 velocity { get; set; } = Vector2.zero;
    [HideInInspector] public float m_velocitySmoothingX;

    [Header("Dash Settings")]
    [SerializeField] private float m_dashMultiplier;
    [SerializeField] private float m_dashCooldown = 10f;
    public float dashCooldownRemaining { get; set; }
    public bool canDash { get => dashCooldownRemaining <= 0f; }
    public void ResetCooldown() => dashCooldownRemaining = m_dashCooldown;

    public bool dashEnabled { get; set; } = false;

    public bool isDashing { get; set; } = false;

    [SerializeField] private bool m_doubleJumpEnabled = false;
	public bool canDoubleJump { get; set; } = false;

    public void Knockback(float vel, Vector2 KnockbackDirection, Rigidbody2D ObjectRB)
    {
        //I need to knock back some drinks after seeing this code
        KnockbackDirection.y = (KnockbackDirection.x * 10);
        KnockbackDirection.x = (KnockbackDirection.x * 100);
        ObjectRB.AddForce(KnockbackDirection.normalized * vel);
        //velocity = new Vector2(ObjectRB.velocity.x, ObjectRB.velocity.y); -- Remnent of a time when I tried to do this a classy way.
    }


    // Public Accessors
    public float GetMovementSpeed() => m_movementSpeed;
    public float SetMovementSpeed(float newMoveSpeed) => m_movementSpeed = newMoveSpeed;

    public float GetJumpMovementSpeed() => m_jumpMovementSpeed;
    public float SetJumpMovementSpeed(float newMoveSpeed) => m_jumpMovementSpeed = newMoveSpeed;

    public float GetAccelerationDur() => m_accelerationDuration;
    public void SetAccelerationDur(float newAccelDur) => m_accelerationDuration = newAccelDur;

    public float GetJumpAccelerationDur() => m_jumpAccelerationDuration;
    public void SetJumpAccelerationDur(float newAccelDur) => m_jumpAccelerationDuration = newAccelDur;

    public float GetJumpHeight() => m_jumpHeight;
    public float SetJumpHeight(float newJumpHeight) => m_jumpHeight = newJumpHeight;

	public bool GetDoubleJumpEnabled() => m_doubleJumpEnabled;
	public void SetDoubleJumpEnabled(bool newValue) => m_doubleJumpEnabled = newValue;

    public float GetDashMultiplier() => m_dashMultiplier;

    public void SetDashMultiplier(float newDashMultipler) => m_dashMultiplier = newDashMultipler;

    public float GetDashCooldown() => m_dashCooldown;
    public void SetDashCooldown(float newCooldown) => m_dashCooldown = newCooldown;

}
