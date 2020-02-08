using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Skill/Create Melee Range Skill")]
public class MeleeRangeSkill : SkillBase
{
    [Header("Skill Effects")]
    [SerializeField] private float m_meleeRangePerLevel = 0.2f;

    public override void Init(GameObject targetGameObject)
    {
        base.Init(targetGameObject);
        if (m_skillLevel != 0)
            m_player.m_meleeHandler.SetAttackRange(m_player.m_meleeHandler.GetAttackRange() + 
                (m_meleeRangePerLevel * m_skillLevel));
    }

    public override void LevelUpSkill()
    {
        // Check if the skill can be levelled up
        if (m_skillLevel < m_maxSkillLevel)
        {
            ++m_skillLevel;
            DeductPlayerEconomyCount();
            m_player.m_meleeHandler.SetAttackRange(m_player.m_meleeHandler.GetAttackRange() + m_meleeRangePerLevel);
        }
    }

    public override void ResetSkill()
    {
        m_skillLevel = 0;
    }

    public override string GetSkillEffectValue()
    {
        string returnValue = m_skillLevel == 0 ? m_meleeRangePerLevel.ToString() :
            (m_meleeRangePerLevel * m_skillLevel).ToString();


        return returnValue;
    }
}
