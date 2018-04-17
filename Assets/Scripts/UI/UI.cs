using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public GameObject titleScreen;

    public GameObject mainMenuPanel;
    public GameObject menuPanel;
    public GameObject menuBtn;

    public GameObject questlogPanel;
    public GameObject questlogBtn;
    public GameObject questlogBtn2;

    public GameObject singleQuestModal;

    public GameObject currQuestPanel;

    public GameObject miniMap;

    private Animator blinkText;
    private Animator menuAnim;
    private Animator questlogAnim;
    private bool menuOpen = false;
    private bool questlogOpen = false;
    private float timer = 0;

    private string[] quests = new string[] {
        "Aufgabe01_Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt.",
        "Aufgabe02_Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt.",
        "Aufgabe03_Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt."
    };

    private string[] questHelp = new string[] {
        "Aufgabe01_Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt.",
        "Aufgabe02_Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt.",
        "Aufgabe03_Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt."
    };

    private string[] questSolution = new string[] {
        "Aufgabe01_Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt.",
        "Aufgabe02_Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt.",
        "Aufgabe03_Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt."
    };

    // Use this for initialization
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            blinkText = titleScreen.GetComponent<Animator>();
            blinkText.enabled = false;
        }

        menuAnim = menuPanel.GetComponent<Animator>();
        menuAnim.enabled = false;
        questlogAnim = questlogPanel.GetComponent<Animator>();
        questlogAnim.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            resetTimer();
            titleScreen.SetActive(false);
        }
        else
        {
            tick();
        }

        if (titleScreen != null && blinkText != null)
        {
            if (timer > 10)
            {
                titleScreen.SetActive(true);
                blinkText.enabled = true;
                blinkText.Play("BlinkingText");
            }
        }
    }

    // Function for Screensaver
    private void resetTimer()
    {
        timer = 0;
    }

    private void tick()
    {
        timer += Time.deltaTime;
    }

    // Start Game
    public void startGame()
    {
        Application.LoadLevel("Main");
    }

    // Reload Gamescene
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Save Game as .bin
    public void saveGame()
    {
        SaveLoad.Save();
        //SaveLoad.Load();
        //Debug.Log(GameData.playerPos.x);
    }

    // Quit to Menu-Screen
    public void quitCurrGame()
    {
        Application.LoadLevel("Menu");
    }

    // Quit Game
    public void quitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Start Game
    public void openSettings()
    {
        Application.LoadLevel("Settings");
    }

    public void setCurrentQuest(int quest)
    {
        currQuestPanel.transform.GetChild(0).GetComponent<Text>().text = quests[quest].Split('_')[1];
    }

    // Open a Quest-Modal
    public void openQuestModal(int quest)
    {
        singleQuestModal.transform.GetChild(0).GetComponent<Text>().text = quests[quest].Split('_')[0];
        singleQuestModal.transform.GetChild(1).GetComponent<Text>().text = quests[quest].Split('_')[1];
        singleQuestModal.SetActive(true);
    }

    // Close a Quest-Modal
    public void closeQuestModal()
    {
        singleQuestModal.SetActive(false);
    }

    // Toggle Menu
    public void toggleSlideMenu()
    {
        if (!menuOpen)
        {
            menuOpen = true;
            questlogBtn.SetActive(false);
            currQuestPanel.SetActive(false);
            miniMap.SetActive(false);
            menuAnim.enabled = true;
            menuAnim.Play("SlideMenu");
        }
        else
        {
            menuOpen = false;
            menuAnim.Play("SlideMenuBack");
            StartCoroutine(QuestPanelActive());
        }
    }

    public void toggleQuestlog()
    {
        if (!questlogOpen)
        {
            questlogOpen = true;
            menuBtn.SetActive(false);
            questlogBtn.SetActive(false);
            currQuestPanel.SetActive(false);
            miniMap.SetActive(false);
            questlogAnim.enabled = true;
            questlogAnim.Play("SlideQuestlog");
        }
        else
        {
            questlogOpen = false;
            questlogAnim.Play("SlideQuestlogBack");
            StartCoroutine(QuestPanelActive());
        }
    }

    IEnumerator QuestPanelActive()
    {
        yield return new WaitForSeconds(0.5f);
        currQuestPanel.SetActive(true);
        menuBtn.SetActive(true);
        questlogBtn.SetActive(true);
        miniMap.SetActive(true);
    }
}
