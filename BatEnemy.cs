using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : BaseCharacter
{
    [Header("AI Settings")]
    [SerializeField] private float m_destinationLenience = 0.05f;

    [Header("Patrol Mode Settings")]
    [SerializeField] List<Transform> m_patrolPoints = new List<Transform>();
    [SerializeField] private float m_playerDetectionRadius = 0.5f;  // How far the player needs to be before the enemy just tracks him
    private int m_patrolIndex = 0;
    private float m_distanceFromTarget;

    [Header("Attack Settings")]
    [SerializeField] private ProjectileHandler m_projectileHandler = new ProjectileHandler();
    [SerializeField] private Transform m_player = null;     // Keep track of the players position

    private const string m_deathAnimationName = "Bat Dying";   // Used for getting the duration of the animation

    [Header("XP Settings")]
    [SerializeField] private GameObject m_expBall;
    [SerializeField] private int m_numberOfExpBalls = 5;
    private bool m_expSpawnTriggered = false;
    protected override void Awake()
    {
        base.Awake();

        m_projectileHandler.Init(m_animatorController);

        if (m_player == null)
            GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if (m_healthHandler.Die == null)
            m_healthHandler.Die += DieEffect;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    {
        MoveTowards(m_patrolPoints[m_patrolIndex]);
        if (m_healthHandler.isDead)
            m_healthHandler.Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            Physics2D.IgnoreCollision(m_charController2D.GetBoxCollider2D(), collision.collider);
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
            if (!m_healthHandler.isDead)
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

    private void Attack()
    {
        m_projectileHandler.fireRateDelayRemaining -= Time.deltaTime;

        if (m_projectileHandler.fireRateDelayRemaining <= 0f)
        {
            m_projectileHandler.CreateProjectile(transform.position, m_player.position);
            m_projectileHandler.fireRateDelayRemaining = m_projectileHandler.GetFireRate();
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

    private void StopMoving()
    {
        m_movementHandler.velocity = Vector2.zero;
        m_animatorController.SetBool(m_animTriggers.m_movingBoolName, false);
		m_rigidbody2D.velocity = Vector2.zero;
    }

    private void DieEffect()
    {
		m_rigidbody2D.constraints = RigidbodyConstraints2D.None;

        float deathAnimationLength = m_animatorController.GetAnimationLength(m_deathAnimationName) * 19;
        Debug.Log(deathAnimationLength);

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
