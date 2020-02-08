using UnityEngine;

/// <summary>
/// Handling Levelling and Experience for unlocking Skills
/// </summary>
[System.Serializable]
public class LevelHandler
{
    [SerializeField] private int m_playerLevel = 1;
    [SerializeField] private int m_skillPoints = 0;
    [SerializeField] private int m_corruptedEssence = 0;
    private MeleeHandler m_meleeHandler;
    public float m_currentExperience { get; set; } = 0;
    public float m_experienceForNextLevel { get; set; } = 100;

    public void GainXP(float expGained)
    {
        m_currentExperience = Mathf.Clamp(m_currentExperience + expGained, 0, m_experienceForNextLevel);

        if (m_currentExperience >= m_experienceForNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        ++m_skillPoints;
        ++m_playerLevel;

        m_currentExperience = 0;
        // Experience scaling (this wont get out of hand)
        m_experienceForNextLevel = (int)((60 * Mathf.Pow(m_playerLevel, 1.82f)) - 60);
    }


    /// <summary>
    /// Increase characters level, Update the experience for next level, 
    /// Adjust any stats if needed
    /// </summary>

    // Public Accessors
    public int GetLevel() => m_playerLevel;
    public int SetLevel(int newLevel) => m_playerLevel = newLevel;

    public int GetSkillPoints() => m_skillPoints;
    public int SetSkillPoints(int newSkillPoints) => m_skillPoints = newSkillPoints;

    public int GetCorruptedEssence() => m_corruptedEssence;
    public void SetCorruptedEssence(int newValue) => m_corruptedEssence = newValue;

    public float expRatio { get => m_currentExperience / m_experienceForNextLevel; }
}
