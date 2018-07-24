using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class LevelScriptSettings {
	[SerializeField]
	private string _fileName = "Level";

	[SerializeField]
	private int _hiddenLinesOfCodeOnTop = 3;

	[SerializeField]
	private int _hiddenLinesOfCodeOnBottom = 3;

	[SerializeField]
	private string _textOnLevelComplete = "Nice job! You did it!";

	[SerializeField]
	[Multiline]
	private string _helpText = "We like to help you!";

	[SerializeField]
	[Multiline]
	private string _instructionText = "You have to code!";

	[SerializeField]
	private string[] _webLinks = new string[] { "https://docs.microsoft.com/en-us/dotnet/csharp/quick-starts/" };

	public string FileName {
		get {
			if (!_fileName.EndsWith (".cs")) {
				_fileName += ".cs";
			}
			return _fileName;
		}
	}

	public int HiddenLinesOfCodeOnTop {
		get {
			return _hiddenLinesOfCodeOnTop;
		}
	}

	public int HiddenLinesOfCodeOnBottom {
		get {
			return _hiddenLinesOfCodeOnBottom;
		}
	}

	public string TextOnLevelComplete {
		get {
			return _textOnLevelComplete;
		}
	}

	public string HelpText {
		get {
			return _helpText;
		}
	}

	public string InstructionText {
		get {
			return _instructionText;
		}
	}

	public string[] WebLinks {
		get {
			return _webLinks;
		}
	}
}

public class LevelScriptController : MonoBehaviour {
	[SerializeField]
	private LevelScriptSettings[] _levelScriptSettings;

	[SerializeField]
	private CodingBoxController _codingBoxController;

	private int _currentLevel;

	public string CurrentLevelFileName { get { return GetLevelScriptSettings (_currentLevel).FileName; } }

	public string CurrentHelpText { get { return GetLevelScriptSettings (_currentLevel).HelpText; } }

	public string CurrentInstructionText { get { return GetLevelScriptSettings (_currentLevel).InstructionText; } }

	public string[] GetCurrentWebLinks { get { return GetLevelScriptSettings (_currentLevel).WebLinks; } }

	public LevelScriptSettings GetLevelScriptSettings(int index) {
		if (index < _levelScriptSettings.Length) {
			return _levelScriptSettings [index];
		} else {
			Debug.LogError ("The level script settings with the index: " + index + " doesn't exist!");
			return null;
		}
	}

	public void LoadLevelScript(int level) {
		var settings = GetLevelScriptSettings (level);
		Debug.Log ("Boot");
		_codingBoxController.LoadWithCoroutine (settings.FileName, settings.HiddenLinesOfCodeOnTop, settings.HiddenLinesOfCodeOnBottom);
		_currentLevel = level;
	}

	public void ReloadLevelScript() {
		_codingBoxController.ClearCodingBox ();

		LoadLevelScript (_currentLevel);
	}

	public void LoadNextLevel(bool lastLevelCompleted = true) {
		StartCoroutine (LoadNextLevelCoroutine (lastLevelCompleted));
	}

	public IEnumerator LoadNextLevelCoroutine(bool lastLevelCompleted = true) {
		_codingBoxController.ClearCodingBox ();

		if (lastLevelCompleted) {
			yield return CompleteLevel (_currentLevel);
		}

		LoadLevelScript (_currentLevel + 1);
	}

	public Coroutine CompleteLevel(int level) {
		string completeText = GetLevelScriptSettings (level).TextOnLevelComplete;
		if (!string.IsNullOrEmpty (completeText)) {
			//return _codingBoxController.WriteToCodingBox ("Test");
            return _codingBoxController.WriteToCodingBox("        // " + GetLevelScriptSettings(level).TextOnLevelComplete + "\n");
		} else {
			return null;
		}
	}

	public void AllowRunningCode() {
		_codingBoxController.AllowRunningCode ();
	}

	private void Start() {
		LoadLevelScript (0);

		//set camera to zoomed mode only during zeroth level
//		Animator _cameraController = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Animator> ();
//		_cameraController.SetBool ("Zoom", true);
//		_cameraController.Play ("FadeInZoom");

		/*
        //if you don't want the boot procedure
        MovePlayerController _playerController = FindObjectOfType<MovePlayerController>();
        Animator _playerAnimator = _playerController.GetComponentInChildren<Animator>();
        _playerAnimator.Play("Idle");
        */
	}
}