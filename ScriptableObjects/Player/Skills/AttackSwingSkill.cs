using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Skill/Create Swing Speed Skill")]
public class AttackSwingSkill : SkillBase
{
    [Header("Skill Effects")]
    [SerializeField] private float m_swingSpeedPerLevel = 0.1f;

    public override void Init(GameObject targetGameObject)
    {
        base.Init(targetGameObject);
    }

    public override void LevelUpSkill()
    {
        // Check if the skill can be levelled up
        if (m_skillLevel < m_maxSkillLevel)
        {
            ++m_skillLevel;
            DeductPlayerEconomyCount();
            m_player.m_meleeHandler.SetMeleeSpeedMult(m_player.m_meleeHandler.GetMeleeSpeedMult() + m_swingSpeedPerLevel);
        }
    }

    public override void ResetSkill()
    {
        m_skillLevel = 0;
        m_player.m_meleeHandler.SetMeleeSpeedMult(1);
    }

    public override string GetSkillEffectValue()
    {
        string returnValue = m_skillLevel == 0 ? m_swingSpeedPerLevel.ToString() :
            (m_swingSpeedPerLevel * m_skillLevel).ToString();

        return returnValue;
    }
    
}
