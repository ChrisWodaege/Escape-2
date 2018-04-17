using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public static Color color;
	
    public GameObject menuPanel;
    public GameObject menuBtn;

    public GameObject questlogPanel;
    public GameObject questlogBtn;
    public GameObject questlogBtn2;

    public GameObject singleQuestModal;
	public GameObject singleQuest;
	public GameObject singleQuestHelpModal;
	public GameObject singleQuestHelp;

    public GameObject currQuestPanel;
	public GameObject currQuest;
	public GameObject currQuestHelpModal;
	public GameObject currQuestHelp;
	
	public GameObject inventoryPanel;

    public GameObject miniMap;

    private Animator menuAnim;
    private Animator questlogAnim;
    private bool menuOpen = false;
    private bool questlogOpen = false;
    private float timer = 0;
	
	private string firstQuest;
	private string firstQuestHelp;

    // Use this for initialization
    void Start()
    {
        menuAnim = menuPanel.GetComponent<Animator>();
        menuAnim.enabled = false;
        questlogAnim = questlogPanel.GetComponent<Animator>();
        questlogAnim.enabled = false;
		
		if(color.r != 0 && color.g != 0 && color.b != 0) {
			menuPanel.GetComponent<Image>().color = color;
			questlogPanel.GetComponent<Image>().color = color;
			inventoryPanel.GetComponent<Image>().color = color;
			currQuestPanel.GetComponent<Image>().color = color;
		} else {
			Color standard = Color.black;
			standard.a = 0.8f; 
			menuPanel.GetComponent<Image>().color = standard;
			questlogPanel.GetComponent<Image>().color = standard; 
			inventoryPanel.GetComponent<Image>().color = standard;
			currQuestPanel.GetComponent<Image>().color = standard;
		}
		
		firstQuest = GameObject.Find("LevelController").GetComponent<LevelScriptController>().CurrentInstructionText;
		firstQuestHelp = GameObject.Find("LevelController").GetComponent<LevelScriptController>().CurrentHelpText;
    }

    // Update is called once per frame
    void Update() {
		currQuest.GetComponent<Text>().text = GameObject.Find("LevelController").GetComponent<LevelScriptController>().CurrentInstructionText;
		currQuestHelp.GetComponent<Text>().text = GameObject.Find("LevelController").GetComponent<LevelScriptController>().CurrentHelpText;
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
	
	// Open Help-Modal
	public void openHelpModal() {
		currQuestHelpModal.SetActive(true);
	}
	
	// Close Help-Modal
	public void closeHelpModal() {
		currQuestHelpModal.SetActive(false);
	}

    // Open QuestLog-Quest
    public void openQuestModal(int quest)
    {
		if(quest == 0) {
			singleQuest.GetComponent<Text>().text = firstQuest;
		} else {
			singleQuest.GetComponent<Text>().text = "This is a next Task for you to solve";
		}
   
        singleQuestModal.SetActive(true);
    }
	
	public void closeQuestModal() 
	{
		singleQuestModal.SetActive(false);
	}
	
	// Open QuestLog-Quest-Help
	public void openQuestHelpModal(int quest) 
	{
		if(quest == 0) {
			singleQuestHelp.GetComponent<Text>().text = firstQuestHelp;
		} else {
			singleQuestHelp.GetComponent<Text>().text = "You should do it like this!";
		}
		
		singleQuestHelpModal.SetActive(true);
	}
	
	public void closeQuestHelpModal() {
		singleQuestHelpModal.SetActive(false);
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
