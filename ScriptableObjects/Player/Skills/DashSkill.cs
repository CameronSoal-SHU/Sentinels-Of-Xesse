using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Skill/Create Dash Skill")]
public class DashSkill : SkillBase
{
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
            m_player.m_movementHandler.dashEnabled = true;
        }
    }

    public override void ResetSkill()
    {
        m_skillLevel = 0;
        m_player.m_movementHandler.dashEnabled = false;
    }

    public override string GetSkillEffectValue()
    {
        return "This does nothing";
    }
}
