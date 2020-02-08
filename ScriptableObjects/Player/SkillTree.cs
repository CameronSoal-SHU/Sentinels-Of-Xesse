using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A skill tree object will hold multiple skills for the player 
/// to interact with through the UI
/// </summary>
/// 
[CreateAssetMenu(menuName ="Skill Tree/Create New Skill Tree")]
public class SkillTree : ScriptableObject
{
    [SerializeField] private string m_skillTreeName = null;
    [SerializeField] private List<SkillBase> m_skills = new List<SkillBase>();

    /// <summary>
    /// Initialise the skills inside the skill tree with a reference to the player object
    /// </summary>
    /// <param name="targetGameObject">Pointer to the player in-game</param>
    public void Init(GameObject targetGameObject)
    {
        foreach(SkillBase skill in m_skills)
        {
            skill.Init(targetGameObject);
        }
    }

    public void ResetAllSkills()
    {
        foreach(SkillBase skill in m_skills)
        {
            skill.ResetSkill();
        }
    }

    // Public Accessors
    public string GetSkillTreeName() => m_skillTreeName;
    // Public Accessors for Skills
    public List<SkillBase> GetSkillList() => m_skills;
    public SkillBase GetSkill(string skillNameQuery)
    {
        SkillBase foundSkill = null;
        int index = 0;

        // Search through every skill for a match
        do
        {
            SkillBase temp = m_skills[index];

            foundSkill = temp.GetSkillName() == skillNameQuery ? temp : null;
            ++index;
        } while (foundSkill == null && index < m_skills.Count - 1);

        return foundSkill;
    }
}
