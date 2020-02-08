using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(Animator))]
public abstract class BaseCharacter : MonoBehaviour
{
    public enum char_direction { LEFT, RIGHT };

    [Header("Character Stats")]
    // Handler classes for public use
    public HealthHandler m_healthHandler = new HealthHandler();
    public MovementHandler m_movementHandler = new MovementHandler();
    public LevelHandler m_levelHandler = new LevelHandler();

    protected Controller2D m_charController2D;
	protected Rigidbody2D m_rigidbody2D;
	protected AnimatorController m_animatorController = null;
	protected SpriteRenderer m_spriteRenderer = null;

    
    [Header("Animation Settings"), SerializeField] protected AnimationNames m_animTriggers = new AnimationNames();

    protected bool characterJumped { get; set; } = false;
    protected char_direction m_charDirection { get; set; } = char_direction.RIGHT;

    // Start is called before the first frame update
    protected virtual void Awake()
	{
		m_charController2D = GetComponent<Controller2D>();
		m_rigidbody2D = GetComponent<Rigidbody2D>();
		m_animatorController = new AnimatorController(GetComponent<Animator>());
		m_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	protected virtual void Update()
	{
        // Health Regeneration
        if (m_healthHandler.healthRegenEnabled)
        {
            if (m_healthHandler.healthRegenDelayRemaining <= 0f)
            {
                m_healthHandler.Heal(m_healthHandler.GetHealthRegenAmount());
                m_healthHandler.ResetHealthRegenDelay();
            }
            else
                m_healthHandler.healthRegenDelayRemaining -= Time.deltaTime;
        }

		// Used for walk/jump cycle animation
		m_animatorController.SetBool(m_animTriggers.m_movingBoolName, m_movementHandler.velocity.x != 0);    
        // Hey alex, I made you a nice lil gift for animations xx
		m_animatorController.SetBool(m_animTriggers.m_groundedBoolName, m_charController2D.isGrounded);

        // Should the grounded acceleration or in-air acceleration be applied?
        float accelerationToApply = characterJumped ?
            m_movementHandler.GetJumpAccelerationDur() : m_movementHandler.GetAccelerationDur();

        // Final move commit
        m_charController2D.Move(m_movementHandler.velocity, ref m_movementHandler.m_velocitySmoothingX, accelerationToApply);

        UpdateSpriteDirection();
    }

    protected abstract void LateUpdate();

    private void UpdateSpriteDirection()
	{
		// Check which way the character is moving to change the sprite direction
		switch (Mathf.Sign(m_rigidbody2D.velocity.x)) 
		{
			case -1:    // Moving left
				m_spriteRenderer.flipX = true;
                m_charDirection = char_direction.LEFT;
				break;
			case 1:     // Moving right
				m_spriteRenderer.flipX = false;
                m_charDirection = char_direction.RIGHT;
                break;
			default:    // Stationary
				break;
		}
	}

	public Vector3 GetScale() => gameObject.transform.localScale;
	public void SetScale(Vector3 newScale) => gameObject.transform.localScale = newScale;

    public char_direction GetCharacterDirection() => m_charDirection;
    public Controller2D GetCharController2D() => m_charController2D;

    [System.Serializable]
    protected struct AnimationNames
    {
        public string m_movingBoolName;
        public string m_groundedBoolName;
        public string[] m_attackTriggerNames;
        public string m_deathTriggerName;
    }
}
