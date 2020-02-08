using System.Collections.Generic;
using UnityEngine;

public class SkillTreeHandler : MonoBehaviour
{
    [SerializeField] private List<SkillTree> m_skillTrees = new List<SkillTree>();

    // Start is called before the first frame update
    void Start()
    {
        // Initialise each skill tree with a reference to the player object
        foreach(SkillTree skillTree in m_skillTrees)
        {
            skillTree.Init(gameObject);
            skillTree.ResetAllSkills();
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(SkillTree skillTree in m_skillTrees)
        {
            foreach(SkillBase skill in skillTree.GetSkillList())
            {
                skill.CheckSkillAvailability();
            }
        }
    }

    public SkillBase GetSkill(string skillNameQuery)
    {
        SkillBase foundSkill = null;
        int index = 0;
        // Check every skill tree for the skill
        do
        {
            // Check every skill in currently focused skill tree
            foundSkill = m_skillTrees[index].GetSkill(skillNameQuery);
            ++index;
        } while (foundSkill == null && index < m_skillTrees.Count - 1);

        return foundSkill;
    }

    public List<SkillTree> GetSkillTreeList() => m_skillTrees;
    public SkillTree GetSkillTree(string skillTreeName)
    {
        SkillTree foundSkillTree = null;
        int index = 0;

        do
        {
            SkillTree temp = m_skillTrees[index];
            if (temp.GetSkillTreeName() == skillTreeName)
                foundSkillTree = temp;
            ++index;
        } while (foundSkillTree == null && index < m_skillTrees.Count - 1);

        return foundSkillTree;
    }
}
