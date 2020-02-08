using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Skill/Create Double Jump Skill")]
public class DoubleJumpSkill : SkillBase
{
    public override void Init(GameObject targetGameObject)
    {
        base.Init(targetGameObject);

        if (m_skillLevel != 0)
        {
            m_player.m_movementHandler.SetDoubleJumpEnabled(true);
        }

    }

    public override void LevelUpSkill()
    {
        // Check if the skill can be levelled up
        if (m_skillLevel < m_maxSkillLevel)
        {
            ++m_skillLevel;
            DeductPlayerEconomyCount();

            DebugAssignedSpeed(true);
            m_player.m_movementHandler.SetDoubleJumpEnabled(true);
        }
    }

    public override void ResetSkill()
    {
        // Rrrrrrrrrrrreset!
        m_skillLevel = 0;

        DebugAssignedSpeed(false);
        m_player.m_movementHandler.SetDoubleJumpEnabled(false);
    }

    public override string GetSkillEffectValue()
    {
        return "This does nothing";
    }

    /// <summary>
    /// Display the skill level and what it's doing in the console
    /// </summary>
    /// <param name="newJumpHeight">What the jump height is being assigned to</param>
    private void DebugAssignedSpeed(bool newValue)
    {
        Debug.Log("Skill level: " +
            m_skillLevel + "/" + m_maxSkillLevel +
            "; Double jump enabled: " + newValue);
    }
}
