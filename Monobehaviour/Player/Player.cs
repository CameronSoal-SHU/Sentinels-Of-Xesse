using UnityEngine;
using Controls;
using System.Collections;


public class Player : BaseCharacter		// Inherits from base character class
{
	private Vector2 m_playerInput;
    [SerializeField] private IdleAnimHandler m_idleAnimController = new IdleAnimHandler();
    public MeleeHandler m_meleeHandler = new MeleeHandler();
    public AudioHandler m_audioHandler = new AudioHandler();

    [Header("UI References")]
    [SerializeField] private GameObject m_skillMenu = null;
    [SerializeField] private GameObject m_skillBackground = null;
    [SerializeField] private GameObject m_pauseMenu = null;

	private int m_meleeAnimation = 0;

    protected override void Awake()
    {
        base.Awake();
        m_idleAnimController.Init(m_animatorController, GetComponent<SpriteRenderer>());
        m_meleeHandler.Init(m_animatorController);
    }

    private void Start()
	{
        // UI Linking
        if (m_skillMenu == null)    // Assign the skill menu if it isn't already
            m_skillMenu = GameObject.FindGameObjectWithTag("SkillMenu");

        if (m_pauseMenu == null)    // Assign the skill menu if it isn't already
            m_pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
    }

	// Update is called once per frame
	protected override void Update()
    {
        if (!m_healthHandler.isDead)
        {

            HandleMovementInput(m_charController2D.isGrounded);
            HandleCombatInput();
            if (m_movementHandler.dashEnabled) HandleDashing();
            HandleUIInput();
        }
        else
        {
            StartCoroutine(m_audioHandler.PlayDeathSound());
            m_animatorController.ToggleTrigger("isDead", true);
            // Load the dead game over scene
        }
        base.Update();
    }

    protected override void LateUpdate()
    { }

    protected void OnCollisionEnter2D(Collision2D other)
    {

        //// Let player walk through enemies
        //if (other.gameObject.CompareTag("Enemy"))
        //{
        //    Physics2D.IgnoreCollision(m_charController2D.GetBoxCollider2D(), other.collider, true);
        //}
    }

    private void HandleMovementInput(bool isGrounded)
    {
        // Get player input
        m_playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Apply a seperate jumping speed to the player if they are in the air due to jumps
        // In-air speed is NOT applied if the player fell from a ledge and will keep full momentum
        float movementSpeedToApply = (characterJumped && !m_charController2D.isGrounded) ?
            m_movementHandler.GetJumpMovementSpeed() : m_movementHandler.GetMovementSpeed();

        // Set x and y velocity
        m_movementHandler.velocity = new Vector2(m_playerInput.x * movementSpeedToApply,
			m_rigidbody2D.velocity.y);    // Gravity

        // Secret idle effect
        if (m_movementHandler.velocity == Vector2.zero)
            m_idleAnimController.CountDownDelay(Time.deltaTime);
        else
            m_idleAnimController.Reset();

        // Tried to get footstep audio working but couldn't fully due to time constraints ///
        if (m_movementHandler.velocity.x != 0 && (isGrounded))                             //
        {                                                                                  //
            StartCoroutine(m_audioHandler.LoopFootstepSound());                            //
        }                                                                                  //
        else                                                                               //
        {                                                                                  //
            m_audioHandler.SetLoop(false);                                                 //
        }                                                                                  //
        /////////////////////////////////////////////////////////////////////////////////////



        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) &&
            (isGrounded || (m_movementHandler.GetDoubleJumpEnabled() && m_movementHandler.canDoubleJump)))
        {
            // Reset double jump when required
            if (!isGrounded && m_movementHandler.canDoubleJump)
                m_movementHandler.canDoubleJump = false;

            /* Quickly reset velocity to balance multi-jumping
			Also lets jumping be 100% effective when falling */
            m_movementHandler.velocity = new Vector2(m_movementHandler.velocity.x, 0);
            m_movementHandler.velocity = new Vector2(m_movementHandler.velocity.x,
                m_movementHandler.velocity.y + m_movementHandler.GetJumpHeight());

            characterJumped = true;
        }

        if (isGrounded)
        {
            if (!m_movementHandler.canDoubleJump)
                m_movementHandler.canDoubleJump = true;

            if (Mathf.Sign(m_movementHandler.velocity.y) < 0)
                characterJumped = false;
        }

    }

    private void HandleDashing()
    {
        if (m_movementHandler.canDash)
        {
            if (Input.GetKeyDown(ControlScheme.m_dashAbility))
            {
                m_animatorController.ToggleTrigger("Dashing", true);
                m_movementHandler.isDashing = true;
                GetComponent<Rigidbody2D>().AddForce(new Vector2((m_playerInput.x * m_movementHandler.GetDashMultiplier()), 0), ForceMode2D.Force);
                StartCoroutine("DelayTimeDash", 0.1f);
                m_movementHandler.ResetCooldown();
            }
            
        }
        else
        {
            m_movementHandler.dashCooldownRemaining -= Time.deltaTime;
        }
    }

    private void HandleCombatInput()
    {
        if (Input.GetKeyDown(ControlScheme.m_meleeKey))
        {
            // Is the player character able to attack?
            if (m_meleeHandler.canMelee)
            {
                m_animatorController.ToggleTrigger(m_animTriggers.m_attackTriggerNames[m_meleeAnimation], true);
				// Alternate between the 2 animations
				if (m_meleeAnimation == 0)
					++m_meleeAnimation;
				else
					--m_meleeAnimation;

                StartCoroutine(m_audioHandler.PlayAttackSound());
                //Makes an array for all the colliders to be hit by the attack
                Collider2D[] m_collidedEnemies = m_meleeHandler.GetEnemiesInAttack(this);

                for (int i = 0; i < m_collidedEnemies.Length; i++)
                {
                    BaseCharacter enemy = m_collidedEnemies[i].GetComponent<BaseCharacter>();

                    
                    //causes damage to every character that has been hit whilst applying a red affect and knockback.
                    enemy.m_healthHandler.Damage(m_meleeHandler.GetMeleeDamage());
                    StartCoroutine(enemy.m_healthHandler.ApplyDamageEffect(enemy.GetComponent<SpriteRenderer>()));
                    enemy.m_movementHandler.Knockback(1000f, (enemy.transform.position - transform.position), enemy.GetComponent<Rigidbody2D>());
                }

                m_meleeHandler.ResetCooldown();
            }
        }
        else // Cooldown is in effect
        {
            m_meleeHandler.meleeCooldownRemaining -= Time.deltaTime;
        }
    }

    public IEnumerator DelayTimeDash(float delay)
    {

        while (delay > 0)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2((m_playerInput.x * m_movementHandler.GetDashMultiplier()), 0), ForceMode2D.Force);
            delay -= Time.deltaTime;
            yield return null;
            m_animatorController.ToggleTrigger("Dashing", false);
        }

        m_movementHandler.isDashing = false;
    }

    //So we can see the Attack Range - Can be deleted later
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        // If the application is playing, THEN draw the damn cube. NullReferenceException THIS you stingy twat 
        if (Application.isPlaying)
            Gizmos.DrawWireCube((Vector2)m_charController2D.GetBoxCollider2D().bounds.center + 
                (m_charDirection == char_direction.RIGHT ? m_meleeHandler.GetHitboxOffset() : -m_meleeHandler.GetHitboxOffset()), 
                (Vector2)m_charController2D.GetBoxCollider2D().bounds.size + new Vector2(m_meleeHandler.GetAttackRange(), 0));
    }

    private void HandleUIInput()
    {
        // Prevent the skill menu being accessed whilst the game is paused
        if (!m_pauseMenu.activeSelf)
        {
            if (Input.GetKeyDown(ControlScheme.m_toggleSkillTreeMenu))
            {
                m_skillMenu.SetActive(!m_skillMenu.activeSelf);
                m_skillBackground.SetActive(!m_skillBackground.activeSelf);
                GameObject.FindGameObjectWithTag("MoreSkillInfo")?.SetActive(m_skillMenu.activeSelf);

                if (m_skillMenu.activeSelf)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;

                Debug.Log(m_skillMenu.activeSelf ? "Skill Menu Active" : "Skill Menu Inactive");
            }
        }
    }

	public Vector2 GetPlayerInput() => m_playerInput;
}