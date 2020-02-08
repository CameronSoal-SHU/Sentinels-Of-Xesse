using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// To be used on every button inside the skill menu
/// </summary>
public class SkillDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Linking")]
    //[SerializeField] private GameObject m_player = null;
    [SerializeField] private SkillBase m_linkedSkill = null;

    [Header("Skill UI Elements")]
    [SerializeField] private Text m_skillLevelDisplay = null;
    [SerializeField] private Image m_skillImageDisplay = null;

    // Mouse hovering information
    private EventSystem m_eventSys;
    [SerializeField] private RectTransform m_moreSkillInfoDisplay = null;
    private bool m_mouseHovering = false;

    // Player reference
    private Player m_player;

    void Awake()
    {
        m_moreSkillInfoDisplay = GameObject.FindGameObjectWithTag("MoreSkillInfo").GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Ensure a skill has been linked up to the UI element
        if (m_linkedSkill == null) Debug.Log("WARNING! A SKILL UI ELEMENT HAS NOT BEEN LINKED TO A SKILL");
        m_eventSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        m_moreSkillInfoDisplay.gameObject.SetActive(false);

        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        gameObject.GetComponent<Button>().onClick.AddListener(ActivateSkill);
        UpdateSkillUIElements();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSkillUIElements();
        CheckIfSkillUnlocked();
        HandleMouseHover();
    }

    /// <summary>
    /// Called whenever the button is clicked, 
    /// activates the linked skill in the skill tree
    /// </summary>
    private void ActivateSkill()
    {
        m_linkedSkill.LevelUpSkill();
    }

    /// <summary>
    /// On initialisation and every time a skill is changed, call this function
    /// </summary>
    private void UpdateSkillUIElements()
    {
        m_skillImageDisplay.sprite = m_linkedSkill.GetSkillImage();
        // Skill image will be red at level 0, green when levelled up at least once
        m_skillImageDisplay.color = m_linkedSkill.GetSkillLevel() != 0 ? Color.green : Color.red;
        m_skillLevelDisplay.text = m_linkedSkill.GetSkillLevel().ToString() + "/" + m_linkedSkill.GetMaxSkillLevel().ToString();

        switch (m_linkedSkill.GetSkillType())
        {
            case SkillBase.skill_type.LIGHT:
                // Is the skill able to be purchased
                gameObject.GetComponent<Button>().interactable = m_linkedSkill.GetSkillUnlocked() &&
                    (m_player.m_levelHandler.GetSkillPoints() != 0) && (m_linkedSkill.GetSkillLevel() != m_linkedSkill.GetMaxSkillLevel());
                break;
            case SkillBase.skill_type.DARK:
                // Is the skill able to be purchased
                gameObject.GetComponent<Button>().interactable = m_linkedSkill.GetSkillUnlocked() &&
                    (m_player.m_levelHandler.GetCorruptedEssence() != 0) && (m_linkedSkill.GetSkillLevel() != m_linkedSkill.GetMaxSkillLevel());
                break;
        }
    }

    private void CheckIfSkillUnlocked()
    {
        if (!m_linkedSkill.GetSkillUnlocked())
            m_skillImageDisplay.color = Color.cyan;
    }

    /// <summary>
    /// Replaces any special characters with their appropriate values
    /// \\ = newLine
    /// %val = placeholder for the skill effect value
    /// </summary>
    private string FormatString(string input, bool skillDescription = false)
    {
        if (input != null)
        {
            // Replace %val (if it exists) template with the proper value being applied to the player
            if (input.Contains("%val"))
                input = input.Replace("%val", m_linkedSkill.GetSkillEffectValue());

            // Newlines
            input = input.Replace("\\", "\n");

            if (skillDescription)
            {
                // Tell player what the skill does per level at level 0 as long as the skill isn't a toggle
                if (m_linkedSkill.GetSkillLevel() == 0 && m_linkedSkill.GetMaxSkillLevel() != 1)
                    input += " per level";
            }
        }

        return input;
    }

    private void HandleMouseHover()
    {
        if (m_mouseHovering)
        {
            m_moreSkillInfoDisplay.position = (Input.mousePosition) + new Vector3(25f, -25f);
            m_moreSkillInfoDisplay.Find("SkillNameContainer").Find("SkillName").GetComponent<Text>().text = m_linkedSkill.GetSkillName();
            m_moreSkillInfoDisplay.Find("SkillQuote").GetComponent<Text>().text = FormatString(m_linkedSkill.GetSkillQuote());
            m_moreSkillInfoDisplay.Find("SkillDescription").GetComponent<Text>().text = FormatString(m_linkedSkill.GetSkillDesc(), true);
        }
    }

    // Triggered when the mouse hovers over a skill
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Entered: " + gameObject.name);
        m_mouseHovering = true;
        m_moreSkillInfoDisplay.gameObject.SetActive(m_mouseHovering);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_mouseHovering = false;
        m_moreSkillInfoDisplay.gameObject.SetActive(m_mouseHovering);
    }
}
