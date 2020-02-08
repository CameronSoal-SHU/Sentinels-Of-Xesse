using UnityEngine;

public class CameraController : MonoBehaviour // This script works but is still rough. Lots of publics that I'll sort later - no need! Ily so I cleaned it up for you xx
{
	private Player m_player = null;
	private BoxCollider2D m_playerBoxCollider = null;

	[SerializeField] private GameObject m_playerGameObject = null;

	private FocusArea m_focusArea; //Uh oh stinky I fucked up the syntax - Fixed! xx
	[SerializeField] private Vector2 m_focusAreaSize = Vector2.zero;

	[Header("Offset Info")]
	[SerializeField] private float m_verticalOffset = 0.0f;
	[SerializeField] private float m_horizontalOffset = 0.0f;
    [SerializeField] private float m_distanceFromScene = 5.0f;

	[Header("Camera Smoothing")]
	[SerializeField] private float m_horizontalSmoothTime = 0.0f;
	[SerializeField] private float m_verticalSmoothTime = 0.0f;

	private float m_currentHorizontalLookAhead;
	private float m_targetHorizontalLookAhead;

	private float m_horizontalSmoothVelocity;
	private float m_verticalSmoothVelocity;

	private bool m_lookAheadStopped;


    private void Start()
    {
		// Get the necessary components
		m_player = m_playerGameObject.GetComponent<Player>();
		m_playerBoxCollider = m_playerGameObject.GetComponent<BoxCollider2D>();

        m_focusArea = new FocusArea(m_playerBoxCollider.bounds, m_focusAreaSize); // apparently ".collider" isn't allowed anymore so they have to use these GetComponent functions. If it fucks up, I'm looking here first
																			// .collider is too general and never used, reference BoxCollider2D from the playerGO and use that - also fixed xx
	}

    private void LateUpdate()                                                                         
    {                                                                                                 
        m_focusArea.Update(m_playerBoxCollider.bounds);                               

        Vector2 focusPosition = m_focusArea.centre + Vector2.up * m_verticalOffset;

        if (m_focusArea.velocity.x != 0)
        {
			float m_horizontalLookAheadDirection = Mathf.Sign(m_focusArea.velocity.x);

			// Allow the camera to stop panning with the player so it doesn't wildly swing back and forth
			if (Mathf.Sign(m_player.GetPlayerInput().x) == Mathf.Sign(m_focusArea.velocity.x) && m_player.GetPlayerInput().x != 0)
			{
				m_lookAheadStopped = false;
				m_targetHorizontalLookAhead = m_horizontalLookAheadDirection * m_horizontalOffset;
			}
			else
			{
				if (!m_lookAheadStopped)
				{
					m_lookAheadStopped = true;
					m_targetHorizontalLookAhead = m_currentHorizontalLookAhead + (m_horizontalLookAheadDirection * m_horizontalOffset - m_currentHorizontalLookAhead) / 4.0f;
				}
			}
        }

        m_currentHorizontalLookAhead = Mathf.SmoothDamp(m_currentHorizontalLookAhead, m_targetHorizontalLookAhead, ref m_horizontalSmoothVelocity, m_horizontalSmoothTime);
		focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref m_verticalSmoothVelocity, m_verticalSmoothTime);

        focusPosition += Vector2.right * m_currentHorizontalLookAhead;
        // Position the camnera
        transform.position = (Vector3)focusPosition + Vector3.forward * -m_distanceFromScene;
    }

    // Public Accessors
    public Vector2 GetCameraOffset() => new Vector2(m_horizontalOffset, m_verticalOffset);

    // DEBUGGING
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, .2f, .5f);
        Gizmos.DrawCube(m_focusArea.centre, m_focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 centre;  // Centre of the target bounds
        public Vector2 velocity; // Speed and direction of the focus area
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size) // Bounds is a datatype that is basically what it sounds like. This constructor-lite just assigns values to each of the sides so we can refer to them later.
        {
			top = targetBounds.min.y + size.y;
			bottom = targetBounds.min.y;
			left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds) // Y'all ever heard of the cha cha slide?
        {
            // How far the camera should move
            float shiftX = 0;
			float shiftY = 0;

			if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
			else if (targetBounds.max.x > right) 
            {
                shiftX = targetBounds.max.x - right;
            }

            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            top += shiftY;
            bottom += shiftY;
			left += shiftX;
			right += shiftX;

			centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}
