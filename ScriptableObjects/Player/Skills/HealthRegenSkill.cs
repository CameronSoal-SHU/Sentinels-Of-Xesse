using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Skill/Create Health Regeneration Skill")]
public class HealthRegenSkill : SkillBase
{
    [Header("Skill Effects")]
    [SerializeField] private float m_healthRegenPerLevel = 0.2f;

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
            m_player.m_healthHandler.SetHealthRegenAmount(m_healthRegenPerLevel * m_skillLevel);
            m_player.m_healthHandler.healthRegenEnabled = true;
        }
        else ResetSkill();
    }

    public override void ResetSkill()
    {
        m_skillLevel = 0;
        m_player.m_healthHandler.SetHealthRegenAmount(0);
        m_player.m_healthHandler.healthRegenEnabled = false;
    }

    public override string GetSkillEffectValue()
    {
        string returnValue = m_skillLevel == 0 ? m_healthRegenPerLevel.ToString() :
            (m_healthRegenPerLevel * m_skillLevel).ToString();

        return returnValue;
    }
}
