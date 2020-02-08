using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtilities;

public class Enemy : BaseCharacter
{
    private enum targetting_mode { PATROL, TRACK };
    private enum attack_type { MELEE, PROJECTILE };

    [Header("AI Settings")]
    [SerializeField] targetting_mode m_targetingMode = targetting_mode.PATROL;
    [SerializeField] attack_type m_attackType = attack_type.MELEE;
    [SerializeField] private float m_destinationLenience = 0.05f;

    [Header("Patrol Mode Settings")]
    [SerializeField] List<Transform> m_patrolPoints = new List<Transform>();
    [SerializeField] private float m_playerDetectionRadius = 0.5f;  // How far the player needs to be before the enemy just tracks him
    private int m_patrolIndex = 0;
    private float m_distanceFromTarget;

    [Header("Attack Settings")]
    [SerializeField] private float m_initialAttackDelay = 0.0f; // How long before the enemy attacks on initial contact with player
    [SerializeField] private MeleeHandler m_meleeHandler = new MeleeHandler();
    [SerializeField] private ProjectileHandler m_projectileHandler = new ProjectileHandler();
    [SerializeField] private Transform m_player = null;     // Keep track of the players position

    private const string m_deathAnimationName = "Skeleton Death";   // Used for getting the duration of the animation

    [Header("XP Settings")]
    [SerializeField] private GameObject m_expBall;
    [SerializeField] private int m_numberOfExpBalls = 5;
    private bool m_expSpawnTriggered = false;


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        if (m_attackType == attack_type.MELEE)
            m_meleeHandler.Init(m_animatorController);
        else
            m_projectileHandler.Init(m_animatorController);

        if (m_player == null)
            GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if (m_healthHandler.Die == null)
        {
            m_healthHandler.Die += DieEffect;
        }
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Let enemies walk through other enemies
		if (collision.gameObject.CompareTag("Enemy"))
			Physics2D.IgnoreCollision(m_charController2D.GetBoxCollider2D(), collision.collider);
	}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // If flying enemy start shooting projectiles
        if (m_attackType == attack_type.PROJECTILE && !m_healthHandler.isDead)
            AerialAttack();
    }
    protected override void LateUpdate()
    {
        switch (m_targetingMode)
        {
            case targetting_mode.PATROL:
                MoveTowards(m_patrolPoints[m_patrolIndex]);
                break;
            case targetting_mode.TRACK:
                MoveTowardsPlayer();
                break;
        }

        if (m_healthHandler.isDead)
        {
            m_healthHandler.Die();
        }
    }

    private void MoveTowardsPlayer()
    {
        m_distanceFromTarget = (transform.position - m_player.position).magnitude;  // Get distance from target
        float moveDirection = -Mathf.Sign(transform.position.x - m_player.position.x);  // Determine which direction the sprite should be moving

        EvaluateDirection(moveDirection);
    }

    private void MoveTowards(Transform patrolTransform)
    {
        m_distanceFromTarget = (transform.position - patrolTransform.position).magnitude;
        float distanceFromPlayer = (transform.position - m_player.position).magnitude;
        bool shouldTargetPlayer = distanceFromPlayer <= m_playerDetectionRadius;

        float moveDirection;

        // Is the enemy close enough to the player to chase her?
        if (shouldTargetPlayer)
        {
            m_distanceFromTarget = distanceFromPlayer;  // Retarget to player
            moveDirection = -Mathf.Sign(transform.position.x - m_player.position.x);
        }
        else
        {
            moveDirection = -Mathf.Sign(transform.position.x - patrolTransform.position.x);
        }

        EvaluateDirection(moveDirection);
    }

    private void EvaluateDirection(float moveDirection)
    {
        if (m_distanceFromTarget > m_destinationLenience)
        {
            // Unique delay for initial contact with player
            m_meleeHandler.meleeCooldownRemaining = m_initialAttackDelay * (1 / m_meleeHandler.GetMeleeSpeedMult());


            switch (moveDirection)
            {
                // Needs to move right
                case -1:
                    m_movementHandler.velocity = new Vector2(-m_movementHandler.GetMovementSpeed(),
						m_rigidbody2D.velocity.y);
                    break;
                // Needs to move left
                case 1:
                    m_movementHandler.velocity = new Vector2(m_movementHandler.GetMovementSpeed(),
						m_rigidbody2D.velocity.y);
                    break;
                default:    // Nothing happens, what did you expect?
                    break;
            }
            m_animatorController.ToggleTrigger(m_animTriggers.m_attackTriggerNames[0], false);

        }
        else    // Target reached
        {
            if (m_attackType == attack_type.MELEE)
                Attack();
            StopMoving();
            NewDestination();
		}
        
    }

    // Super advanced pathfinding AI
    private void NewDestination()
    {// succcccccccccccccccc
        if (m_patrolIndex + 1 < m_patrolPoints.Count)
            ++m_patrolIndex;
        else
            m_patrolIndex = 0;
    }

    // STOP.
    private void StopMoving()
    {
        m_movementHandler.velocity = Vector2.zero;
        m_animatorController.SetBool(m_animTriggers.m_movingBoolName, false);
		m_rigidbody2D.velocity = Vector2.zero;
    }

    private void Attack()
    {
        // Is the character able to attack?
        if (m_meleeHandler.canMelee)
        {
            Collider2D[] m_collidedEnemies = m_meleeHandler.GetEnemiesInAttack(this);
            m_animatorController.ToggleTrigger(m_animTriggers.m_attackTriggerNames[0], true);

            if (m_collidedEnemies.Length != 0)
            {
                Player player = m_collidedEnemies[0].GetComponent<Player>();

                

                if (player != null)
                {
                    object[] args = new object[3] { 0.7f, player, m_meleeHandler };
                    //Waits to deal the damage until the swinging frame of the animation and then deals the damage.
                    StartCoroutine("DelayedDamage", args);
                }

                m_meleeHandler.ResetCooldown();
            }
        }
        else
        {
            m_meleeHandler.meleeCooldownRemaining -= Time.deltaTime;
        }
    }

    private void AerialAttack()
    {
        m_projectileHandler.fireRateDelayRemaining -= Time.deltaTime;

        // Is the enemy ready to fire?
        if (m_projectileHandler.fireRateDelayRemaining <= 0f)
        {
            m_projectileHandler.CreateProjectile(transform.position, m_player.position);
            m_projectileHandler.fireRateDelayRemaining = m_projectileHandler.GetFireRate();  // Reset delay
        }
    }

    public IEnumerator DelayedDamage(object[] args)
    {
        float delayTime = (float)args[0];
        yield return new WaitForSeconds(delayTime); // lmao this will be fine

        Player player = (Player)args[1];
        MeleeHandler m_meleeHandler = (MeleeHandler)args[2];

        float distanceFromPlayer = (player.transform.position - transform.position).magnitude;

        if (distanceFromPlayer - m_meleeHandler.GetAttackRange() <= 0f) // Check if in range BEFORE appling damage
        {
            player.m_healthHandler.Damage(m_meleeHandler.GetMeleeDamage());
            StartCoroutine(player.m_audioHandler.PlayDamageSound());
            StartCoroutine(player.m_healthHandler.ApplyDamageEffect(player.GetComponent<SpriteRenderer>()));
            player.m_movementHandler.Knockback(1000f, (player.transform.position - transform.position), player.GetComponent<Rigidbody2D>());
        }
    }

    public IEnumerator DelayXPSpawn(float delay)
    {
        m_expSpawnTriggered = true;

        yield return new WaitForSeconds(delay);

        for (int i = 0; i < m_numberOfExpBalls; ++i)
        {
            Instantiate(m_expBall, transform.position + Vector3.up, transform.rotation);
        }
    }


    private void DieEffect()
    {
        float deathAnimationLength = m_animatorController.GetAnimationLength(m_deathAnimationName);

        m_animatorController.ToggleTrigger(m_animTriggers.m_attackTriggerNames[0], false);
        m_animatorController.ToggleTrigger(m_animTriggers.m_deathTriggerName, true);
		// So the corpse doesn't get in the way
		GetComponent<BoxCollider2D>().enabled = false;

        // Prevent the enemy from falling through the floor
        GetComponent<Rigidbody2D>().gravityScale = 0f;
		// Prevent any and all movement
		StopMoving();

        //Spawns orbs that give experience
        if (!m_expSpawnTriggered)
            StartCoroutine("DelayXPSpawn", deathAnimationLength - 0.2f);

        // Remove the game object after the death animation is played
        Destroy(gameObject, deathAnimationLength - 0.15f);
    }
}
