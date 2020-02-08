using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Base script for all abilities/skills. 
/// Must be inherited to create a new skill for the skill tree.
/// </summary>

public abstract class SkillBase : ScriptableObject
{
    // Inherited variables for skills
    [Header("General Skill Details")]
    [SerializeField] protected string m_skillName = null;
    [SerializeField] protected string m_skillQuote = null;
    [SerializeField] protected string m_skillDesc = null;
    [SerializeField] protected Sprite m_skillImage = null;
    [SerializeField] protected skill_type m_skillType = default;

    [Header("Skill Level Settings")]
    [SerializeField] protected int m_skillLevel = 0;
    [SerializeField] protected int m_maxSkillLevel = 5;     // Setting to 1 essentially makes the skill toggleable
    [Header("Skill Unlock Settings")]
                                                            // Which skill needs to be unlocked before skill can be used
    [SerializeField] protected List<SkillBase> m_prevUnlockRequired = new List<SkillBase>();
    [SerializeField] protected int m_levelRequired = 0;     // Skill level requirement
    [SerializeField] protected bool m_skillUnlocked = false;

    [SerializeField] protected int m_darkEssenceCost = 0;   // Only referenced if the skill type is DARK

    protected Player m_player = null;                       // Will be pointing at a reference of the player in-game
    
    // Must be defined within the child class
    // targetGameObject is generally the player Game Object
    public virtual void Init(GameObject targetGameObject)
    {
        m_player = targetGameObject.GetComponent<Player>();
        Debug.Log("Init called, player: " + m_player);
    }

    // Must be defined within the child class, essentially lets the skill do whatever it wants
    public abstract void LevelUpSkill();
    public abstract void ResetSkill();

    protected void DeductPlayerEconomyCount()
    {
        switch (m_skillType)
        {
            case skill_type.LIGHT:
                m_player.m_levelHandler.SetSkillPoints(m_player.m_levelHandler.GetSkillPoints() - 1);
                break;
            case skill_type.DARK:
                m_player.m_levelHandler.SetCorruptedEssence(m_player.m_levelHandler.GetCorruptedEssence() - 1);
                break;
        }
    }

    /// <summary>
    /// Checks if the skill is eligible to be invested in
    /// </summary>
    public void CheckSkillAvailability()
    {
        bool allSkillsRequiredUnlocked = true;
        int skillIndex = 0;

        bool initialCheck;

        /* No point checking if previous skills 
        are unlocked if player isn't even a high enough level */
        switch (m_skillType)
        {
            case skill_type.LIGHT:
                initialCheck = m_player.m_levelHandler.GetLevel() >= m_levelRequired;
                break;
            case skill_type.DARK:
                initialCheck = m_player.m_levelHandler.GetCorruptedEssence() >= m_darkEssenceCost;
                break;
            default:    // Should never really be called, just to prevent errors
                initialCheck = true;
                break;
        }

        if (initialCheck)
        {
            // Check all skills that are required to unlock this skill
            while (allSkillsRequiredUnlocked && skillIndex < m_prevUnlockRequired.Count)
            {
                // Check if a point has been invested into previous skills
                allSkillsRequiredUnlocked = m_prevUnlockRequired[skillIndex].GetSkillLevel() > 0;
                ++skillIndex;
            }
        }
        else
        {
            allSkillsRequiredUnlocked = false;
        }

        m_skillUnlocked = allSkillsRequiredUnlocked;
    }

    // Public accessors
    public string GetSkillName() => m_skillName;
    public string GetSkillQuote() => m_skillQuote;
    public string GetSkillDesc() => m_skillDesc;
    public Sprite GetSkillImage() => m_skillImage;
    public int GetSkillLevel() => m_skillLevel;
    public int GetSkillLevelRequired() => m_levelRequired;
    public int GetMaxSkillLevel() => m_maxSkillLevel;
    public abstract string GetSkillEffectValue();
    public List<SkillBase> GetPrevUnlockRequired() => m_prevUnlockRequired;
    public bool GetSkillUnlocked() => m_skillUnlocked;

    public skill_type GetSkillType() => m_skillType;

    public enum skill_type
    {
        LIGHT,
        DARK,
    }
}
