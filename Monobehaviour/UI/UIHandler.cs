using UnityEngine.UI;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [Header("Health Bar Display")]
    [SerializeField] private Image m_healthBar = null;
    [Header("Experience Bar Display")]
    [SerializeField] private Text m_levelDisplay = null;
    [SerializeField] private Image m_expBar = null;
    [SerializeField] private Text m_expPointsDisplay = null;
    [SerializeField] private Text m_expRequiredDisplay = null;
    [SerializeField] private Text m_corruptedEssenceCountDisplay = null;

    [Header("Player Reference")]
    [SerializeField] private Player m_player = null;
    private HealthHandler m_healthHandler;
    private LevelHandler m_levelHandler;

    // Start is called before the first frame update
    void Start()
    {
        if (m_player == null)
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        m_healthHandler = m_player.m_healthHandler;
        m_levelHandler = m_player.m_levelHandler;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
        UpdateStatsDisplay();
    }

    private void UpdateHealthBar()
    {
        m_healthBar.fillAmount = m_healthHandler.healthRatio;
    }

    private void UpdateStatsDisplay()
    {
        int playerSkillPoints = m_levelHandler.GetSkillPoints();
        int playerCorruptedEssence = m_levelHandler.GetCorruptedEssence();

        m_levelDisplay.text = "Level: " + m_levelHandler.GetLevel().ToString();
        m_expBar.fillAmount = m_levelHandler.expRatio;

        if (playerSkillPoints == 0)
        {
            m_expPointsDisplay.text = null;
        }
        else
        {
            m_expPointsDisplay.text = playerSkillPoints.ToString() + " Skill Point" +
            (playerSkillPoints != 1 ? "s Available!" : " Available");
        }

        m_corruptedEssenceCountDisplay.text = playerCorruptedEssence.ToString() + " Corrupted Essence";
        
        m_expRequiredDisplay.text = m_player.m_levelHandler.m_currentExperience.ToString() + "/" + m_player.m_levelHandler.m_experienceForNextLevel.ToString();
    }
}
