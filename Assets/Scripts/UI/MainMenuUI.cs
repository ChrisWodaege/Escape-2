using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
	public static Color color;

    public GameObject titleScreen;
	
	public Text titleScreen_text;

    public GameObject mainMenuPanel;

    private Animator blinkText;
    private Animator menuAnim;
    private float timer = 0;

    // Use this for initialization
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            blinkText = titleScreen_text.GetComponent<Animator>();
            blinkText.enabled = false;
        }
		
		if(color.r != 0 && color.g != 0 && color.b != 0) {
			mainMenuPanel.GetComponent<Image>().color = color;
		} else {
			Color standard = Color.black;
			standard.a = 0.8f; 
			mainMenuPanel.GetComponent<Image>().color = standard;
		}
		
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

        if (timer > 10)
        {
            titleScreen.SetActive(true);
            blinkText.enabled = true;
            blinkText.Play("BlinkingText");
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
	
	// Open Settings
    public void openSettings()
    {
        Application.LoadLevel("Settings");
    }

    // Quit Game
    public void quitGame()
    {
        Application.Quit();
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#endif
    }
}
