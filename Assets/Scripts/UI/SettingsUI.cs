using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
	public Color color;
	public GameObject settingsPanel;
	
    // Use this for initialization
    void Start() {
		Color standard = Color.black;
		standard.a = 0.8f; 
		settingsPanel.GetComponent<Image>().color = standard;
	}

    // Update is called once per frame
    void Update() {}

	public void adjustUIColor() {
		settingsPanel.GetComponent<Image>().color = color;
		GameUI.color = color;
		MainMenuUI.color = color;
	}
	
	public void backToMenu() {
		Application.LoadLevel("Menu");
	}
}
