using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
	private Rigidbody2D m_rigidbody2D = null;
	private BoxCollider2D m_boxCollider2D = null;

	[Header("Grounded Info")]
	[SerializeField] private float m_groundedBoxSize = 0.2f;
	[SerializeField] private LayerMask m_collidableLayerMask = default;
    public bool isGrounded { get; set; }


    private Vector2 m_finalVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
		m_rigidbody2D = GetComponent<Rigidbody2D>();
		m_boxCollider2D = GetComponent<BoxCollider2D>();
	}

    void Update()
    {
        CheckIfGrounded();
    }

	public void Move(Vector2 targetVelocity, ref float velocitySmoothingX, float accelerationDuration)
	{
		// Final velocity to be applied to player
		m_finalVelocity.x = Mathf.SmoothDamp(m_finalVelocity.x, targetVelocity.x, ref velocitySmoothingX, accelerationDuration);
		m_finalVelocity.y = targetVelocity.y; 

		// Apply final velocity
		m_rigidbody2D.velocity = m_finalVelocity;
	}

	/// <summary>
	/// Checks if the player is touching the ground, or any collidable surface
	/// </summary>
	/// <returns>Returns true if the boxcast intersects with ground</returns>
	private void CheckIfGrounded()
	{
		// Create a box raycast that checks for collisions slightly below the player
		RaycastHit2D raycastHit = Physics2D.BoxCast(m_boxCollider2D.bounds.center, m_boxCollider2D.bounds.size, 0.0f, Vector2.down, m_groundedBoxSize, m_collidableLayerMask);

		/// DEBUGGING ///							// Green when grounded, Red when in air
		Color rayColour = raycastHit.collider != null ? Color.green : Color.red;

		Debug.DrawRay(m_boxCollider2D.bounds.center + new Vector3(m_boxCollider2D.bounds.extents.x, 0), Vector2.down * (m_boxCollider2D.bounds.extents.y + m_groundedBoxSize), rayColour);
		Debug.DrawRay(m_boxCollider2D.bounds.center - new Vector3(m_boxCollider2D.bounds.extents.x, 0), Vector2.down * (m_boxCollider2D.bounds.extents.y + m_groundedBoxSize), rayColour);
		Debug.DrawRay(m_boxCollider2D.bounds.center - new Vector3(m_boxCollider2D.bounds.extents.x, m_boxCollider2D.bounds.extents.y + m_groundedBoxSize), Vector2.right * m_boxCollider2D.bounds.extents.x, rayColour);
        /////////////////

        isGrounded = raycastHit.collider != null;
	}
	public BoxCollider2D GetBoxCollider2D() => m_boxCollider2D;
}
