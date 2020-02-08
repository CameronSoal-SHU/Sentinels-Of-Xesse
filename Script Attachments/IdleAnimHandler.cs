using UnityEngine;

[System.Serializable]
public class IdleAnimHandler
{
    private float m_idleAnimMultiplier = 1f;
    [SerializeField] private float m_idleRampUpDelay = 5f;  // 20 second patience
    private float m_delayRemaining = 0f;

    private AnimatorController m_animController = null;
    private SpriteRenderer m_spriteRenderer = null;


    public void Init(AnimatorController animController, SpriteRenderer spriteRenderer)
    {
        m_animController = animController;
        m_spriteRenderer = spriteRenderer;

        Reset();
    }

    public void CountDownDelay(float deltaTime)
    {
        m_delayRemaining -= deltaTime;

        if (m_delayRemaining <= 0f)
        {
            LetTheFunBegin(deltaTime);
        }
    }

    // You should probably move now...
    private void LetTheFunBegin(float deltaTime)
    {
        // Speed up the idle animation
        m_idleAnimMultiplier = Mathf.Clamp(m_idleAnimMultiplier + (deltaTime / 4f), 1f, float.MaxValue);
        float twentiethDeltaTime = deltaTime / 20f;

        // More angery
        m_spriteRenderer.color -= new Color(0, twentiethDeltaTime, twentiethDeltaTime, 0);
        m_animController.SetFloat("idleSpeedMult", m_idleAnimMultiplier);
    }

    public void Reset()
    {
        m_delayRemaining = m_idleRampUpDelay;
        m_idleAnimMultiplier = 1f;

        m_animController.SetFloat("idleSpeedMult", m_idleAnimMultiplier);

        m_spriteRenderer.color = new Color(1,1,1,1);
    }
}
