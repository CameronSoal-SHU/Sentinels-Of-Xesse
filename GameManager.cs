using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Controls;

public class GameManager : MonoBehaviour
{
    public enum game_state { MAIN_MENU, IN_GAME, GAME_OVER };

    [Header("Game State")]
    [SerializeField] private game_state m_gameState = game_state.MAIN_MENU;

    public static GameManager gameManagerInstance { get; set; } = null;
    private GameObject m_pauseMenu;
    private GameObject m_skillMenu;
    private GameObject m_skillMenuBackground;
    private HealthHandler m_player;

    private void Awake()
    {
        // Make sure there's only one instance of the game manager
        if (gameManagerInstance != null)
            Destroy(gameObject);
        else
            gameManagerInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(ControlScheme.m_toggleMainMenu))
        {
            m_pauseMenu.SetActive(!m_pauseMenu.activeSelf); // Invert if the object is active

            if (m_pauseMenu.activeSelf)
            {
                Time.timeScale = 0;
                m_skillMenu.SetActive(false);
            }
            else
            {
                Time.timeScale = 1;
            }

            Debug.Log(m_pauseMenu.activeSelf ? "Main Menu Active" : "Main Menu Inactive");
        }

        if (m_player!=null && m_player.isDead)
            Death();
    }

    private void OnStateChange(game_state newGameState)
    {
        if (m_gameState != newGameState)
        {
            switch (newGameState)
            {
                case game_state.MAIN_MENU:
                    break;

                case game_state.IN_GAME:
                    Time.timeScale = 1;
                    break;
            }

            m_gameState = newGameState;
        }
    }

    // Check when the scene is loaded and unloaded
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        switch (loadedScene.buildIndex)
        {
            // Main Menu
            case 0:
                //GameObject.Find("Start Button").GetComponent<Button>().onClick.AddListener(() => StartGame());
                break;
            // In-game
            case 1:
                m_pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
                m_skillMenu = GameObject.FindGameObjectWithTag("SkillMenu");
                m_skillMenuBackground = GameObject.Find("SkillMenuBackground");
                m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseCharacter>().m_healthHandler;
                m_pauseMenu.SetActive(false);
                m_skillMenu.SetActive(false);
                m_skillMenuBackground.SetActive(false);
                break;
            case 2:
                GameObject.Find("Return").GetComponent<Button>().onClick.AddListener(() => QuitToMainMenu());
                break;
        }
        Debug.Log("Level Loaded");

    }

    public void StartGame()
    { 
        SceneManager.LoadScene("MainGame");  // Load the main game
        OnStateChange(game_state.IN_GAME);  // Set the game state to in game
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void QuitToMainMenu()
    {
        Debug.Log("Called");
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        OnStateChange(game_state.MAIN_MENU);
    }

    public void Death()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1f;
        OnStateChange(game_state.GAME_OVER);
    }

    public void UnPauseGame()
    {
        m_pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        Time.timeScale = 1f;
        m_pauseMenu.SetActive(false);   // Disable the pause menu
    }

    private void CursorVisible(bool isVisible, CursorLockMode cursorLockMode)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = cursorLockMode;
    }
}