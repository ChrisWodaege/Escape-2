using CSharpCompiler;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodingBoxController : MonoBehaviour, ICodingBoxController
{
    private const string FILE_SUBFOLDER = "LevelScripts";

    [SerializeField]
    private bool _autoSave = true;

    [SerializeField]
    private SyntaxColors _syntaxColors;

    [SerializeField]
    private float _autometedWritingTimeBetweenCharacters = 0.005f;

    [SerializeField]
    private TMP_InputField _codingBoxInputField;

    [SerializeField]
    private TMP_InputField _fileNameInputField;

    [SerializeField]
    private TMP_InputField _componentParentName;

    [SerializeField]
    private GameObject _defaultComponentParent;

    [SerializeField]
    private LevelScriptController _levelController;

    [SerializeField]
    private UnityEngine.UI.Button _runButton;

    private DeferredSynchronizeInvoke _synchronizedInvoke;
    private ScriptBundleLoader _loader;

    private FileController _fileController;
    private UnitySyntaxHighlighter _unitySyntaxHighlighter;

    private string _linesOfCodeOnTop;
    private string _codeToShow;
    private string _linesOfCodeOnBottom;
    private int _caretPositionOnHighlighting;
    private Color _caretColorOnHighlighting;
    private Coroutine _highlightCoroutine;

    private bool _allowRunningCode = true;
	private GameConsole gc;
    private string _currentAutomatedTextWriting;
    private Coroutine _automatedTextWritingCoroutine = null;

    private readonly Dictionary<GameObject, Dictionary<string, Component>> _gameobjectComponents
        = new Dictionary<GameObject, Dictionary<string, Component>>();

    public IDictionary<GameObject, Dictionary<string, Component>> GameobjectComponents { get { return _gameobjectComponents; } }

    [Serializable]
    public class SyntaxColors {
        public Color CommentColor = Color.white;
        public Color KeywordColor = Color.white;
        public Color CustomWordColor = Color.white;
        public Color QuoteColor = Color.white;
        public Color TypeColor = Color.white;
        public Color MethodColor = Color.white;
        public Color NumberColor = Color.white;
    }

	private MovePlayerController _movePlayerController;

    private string ComponentName {
        get {
            if (_levelController != null) {
                return _levelController.CurrentLevelFileName;
            }
            return "TestScript.cs";
        }
    }

    private GameObject ComponentParent {
        get {
            if (_componentParentName != null && !string.IsNullOrEmpty(_componentParentName.text)) {
                var parent = GameObject.Find(_componentParentName.text);

                if (parent != null) {
                    return parent;
                }
                else {
                    Debug.LogError("Can not find gameobject with name: " + _componentParentName.name);
                }
            }

            return _defaultComponentParent != null ? _defaultComponentParent : gameObject;
        }
    }


    public void Load() {
        LoadWithCoroutine();
    }

    public Coroutine LoadWithCoroutine() {
        return LoadWithCoroutine(ComponentName);
    }

    public Coroutine LoadWithCoroutine(string fileName = "", int hiddenLinesOfCodeOnTop = 0, int hiddenLinesOfCodeOnBottom = 0) {
        return StartCoroutine(LoadCoroutine(fileName, hiddenLinesOfCodeOnTop, hiddenLinesOfCodeOnBottom));
    }

    private IEnumerator LoadCoroutine(string fileName = "", int hiddenLinesOfCodeOnTop = 0, int hiddenLinesOfCodeOnBottom = 0) {
        string fileContent = _fileController.LoadTextFromFile(fileName);
        int topSplitIndex = 0;
        for (int i = 0; i < hiddenLinesOfCodeOnTop; i++) {
            topSplitIndex = fileContent.IndexOf('\n' /*Environment.NewLine*/, topSplitIndex + 1);
        }

        int bottomSplitIndex = fileContent.Length - 1;
        for (int i = 0; i < hiddenLinesOfCodeOnBottom; i++) {
            bottomSplitIndex = fileContent.Substring(0, bottomSplitIndex).LastIndexOf('\n'/*Environment.NewLine*/);
        }



        _linesOfCodeOnTop = fileContent.Substring(0, topSplitIndex);
        _codeToShow = fileContent.Substring(topSplitIndex, bottomSplitIndex - topSplitIndex);
        _linesOfCodeOnBottom = fileContent.Substring(bottomSplitIndex, fileContent.Length - bottomSplitIndex);

			//Console.Clear ();
		//Console.WriteLine (gc.output());	
		//gc.input(Console.ReadLine ());
		String gcOutput = gc.output();
		outputLength = gcOutput.Length;
		yield return WriteToCodingBox(gcOutput);
        //yield return WriteToCodingBox(_codeToShow);
    }

	int outputLength = -1;

    public void Reload() {
        _levelController.ReloadLevelScript();
    }

    public void Save() {
        SaveWithCoroutine();
    }

    public Coroutine SaveWithCoroutine() {
        return StartCoroutine(SaveCoroutine());
    }

    private IEnumerator SaveCoroutine() {
        //string codeToSave = _linesOfCodeOnTop + _unitySyntaxHighlighter.getCodeWithoutRichText(_codingBoxInputField.text) + _linesOfCodeOnBottom;
		//_fileController.SaveTextToFile(ComponentName, codeToSave);
        yield return null;
    }

    public void Run() {
		_codingBoxInputField.ActivateInputField ();
		String code = _unitySyntaxHighlighter.getCodeWithoutRichText (_codingBoxInputField.text);
		code = code.Substring (gc.contentLength);
		List<String> validCommands = new List<String> (new String[]{ "boot", "move", "turnleft", "turnright", "take", "drop" });
		List<Command> commands = stringToCommandMapper(Parser.parse (code,validCommands));
		_movePlayerController.sendCommand (commands);
    }

	private List<Command> stringToCommandMapper(List<String> source) {
		List<Command> cl = new List<Command> ();
		//cl.Add(new Command(CommandType.Boot)); //TODO raus nehmen
	
		foreach(String s in source){
			Debug.Log ("CommandMapper:" + s);
			switch (s) {
				case "boot":{ cl.Add(new Command(CommandType.Boot)); break;}
				case "move":{ cl.Add(new Command(CommandType.Move)); break;}
				case "turnleft":{ cl.Add(new Command(CommandType.TurnLeft)); break;}
				case "turnright":{ cl.Add(new Command(CommandType.TurnRight)); break;}
				case "take":{ cl.Add(new Command(CommandType.Take)); break;}
				case "drop":{ cl.Add(new Command(CommandType.Drop)); break;}
			}
		}
		return cl;
	}

    public Coroutine RunWithCoroutine() {
        if (_allowRunningCode) {
            return StartCoroutine(RunCoroutine());
        }
        return null;
    }

    private IEnumerator RunCoroutine() {
        if (_autoSave) {
            yield return SaveWithCoroutine();
        }

        var path = _fileController.GetSaveFilePath(ComponentName);

        if (!string.IsNullOrEmpty(path)) {
            ForbitRunningCode();
            _loader.LoadAndWatchScriptsBundle(new[] { path });
        }
        else {
            Debug.LogError("Can not run file " + ComponentName);
        }
    }

    public void AllowRunningCode() {
        _allowRunningCode = true;
        _runButton.interactable = true;
    }

    public void ForbitRunningCode() {
        _allowRunningCode = false;
        _runButton.interactable = false;
    }

    public void EnableComponent(Component component) {
        if (component != null && component is MonoBehaviour) (component as MonoBehaviour).enabled = true;
    }

    public void EnableComponent(string componentName) {
        foreach (var dicts in _gameobjectComponents.Values) {
            Component component;
            if (dicts.TryGetValue(componentName, out component)) {
                EnableComponent(component);
                return;
            }
        }
    }

    public void DisableComponent(Component component) {
        if (component != null && component is MonoBehaviour) (component as MonoBehaviour).enabled = false;
    }

    public void DisableComponent(string componentName) {
        foreach (var dicts in _gameobjectComponents.Values) {
            Component component;
            if (dicts.TryGetValue(componentName, out component)) {
                DisableComponent(component);
                return;
            }
        }
    }

    public void DestroyComponent(Component component) {
        _loader.destroyInstance(component);
    }

    public void DestroyComponent(string componentName) {
        foreach (var dicts in _gameobjectComponents.Values) {
            Component component;
            if (dicts.TryGetValue(componentName, out component)) {
                DestroyComponent(component);
                return;
            }
        }
    }

    public void BlockUserInputForCodingBox() {
        _codingBoxInputField.readOnly = true;
    }

    public void UnblockUserInputForCodingBox() {
        _codingBoxInputField.readOnly = false;
    }

    public void HideCaret() {
        if (_codingBoxInputField.caretColor != new Color(0, 0, 0, 0)) {
            _caretColorOnHighlighting = _codingBoxInputField.caretColor;
        }
        _codingBoxInputField.caretColor = new Color(0, 0, 0, 0);
    }

    public void ShowCaret() {
        _codingBoxInputField.caretColor = _caretColorOnHighlighting;
    }

    public void ClearCodingBox() {
        if (_automatedTextWritingCoroutine != null) {
            _currentAutomatedTextWriting = string.Empty;
            StopCoroutine(_automatedTextWritingCoroutine);
            _automatedTextWritingCoroutine = null;
        }
        _codingBoxInputField.text = string.Empty;
    }

	public void WriteToCodingBox(char character) {
		_codingBoxInputField.text += character;
	}

    public Coroutine WriteToCodingBox(string text, float timeBetweenCharacters = -1) {
		if (timeBetweenCharacters < 0) {
            timeBetweenCharacters = _autometedWritingTimeBetweenCharacters;
        }
        _currentAutomatedTextWriting += text;
        if (_automatedTextWritingCoroutine == null) {
            _automatedTextWritingCoroutine = StartCoroutine(WriteToCodingBoxCoroutine(timeBetweenCharacters));
        }
        return _automatedTextWritingCoroutine;
    }

    private IEnumerator WriteToCodingBoxCoroutine(float timeBetweenCharacters) {
        _codingBoxInputField.readOnly = true;

        ForbitRunningCode();
        Color caretColorBeforeWriting;
        if (_codingBoxInputField.caretColor != new Color(0, 0, 0, 0)) {
            caretColorBeforeWriting = _codingBoxInputField.caretColor;
        }
        else {
            caretColorBeforeWriting = _caretColorOnHighlighting;
        }

        _codingBoxInputField.caretColor = new Color(0, 0, 0, 0);
        Color selectionColorBeforWriting = _codingBoxInputField.selectionColor;
        _codingBoxInputField.selectionColor = new Color(0, 0, 0, 0);

        while (_currentAutomatedTextWriting.Length > 0) {
            WriteToCodingBox(_currentAutomatedTextWriting[0]);
            _currentAutomatedTextWriting = _currentAutomatedTextWriting.Remove(0, 1);
            yield return new WaitForSeconds(timeBetweenCharacters);
        }

        _codingBoxInputField.Select();
        _codingBoxInputField.caretPosition = _codingBoxInputField.text.Length;

        yield return new WaitForEndOfFrame();

        _codingBoxInputField.caretColor = caretColorBeforeWriting;
        _codingBoxInputField.selectionColor = selectionColorBeforWriting;
        _automatedTextWritingCoroutine = null;
        AllowRunningCode();
        _codingBoxInputField.readOnly = false;
    }

    private void Awake() {
		gc = new GameConsole ();
		_movePlayerController = GameObject.Find("Player").GetComponent<MovePlayerController>();
        _fileController = new FileController(FILE_SUBFOLDER);

        _unitySyntaxHighlighter = new UnitySyntaxHighlighter(new UnitySyntaxHighlighter.SyntaxColors() {
            CommentColor = _syntaxColors.CommentColor.ColorToHex(),
            KeywordColor = _syntaxColors.KeywordColor.ColorToHex(),
            CustomWordColor = _syntaxColors.CustomWordColor.ColorToHex(),
            QuoteColor = _syntaxColors.QuoteColor.ColorToHex(),
            TypeColor = _syntaxColors.TypeColor.ColorToHex(),
            MethodColor = _syntaxColors.MethodColor.ColorToHex(),
            NumberColor = _syntaxColors.NumberColor.ColorToHex()
        });

        _codingBoxInputField.onValueChanged.AddListener(ResetCodeHighlighting);
		//_codingBoxInputField.onSubmit.AddListener(MyTestSubmit);
        _synchronizedInvoke = new DeferredSynchronizeInvoke();
        _loader = new ScriptBundleLoader(_synchronizedInvoke) {
            logWriter = new UnityLogTextWriter(),
            createInstance = (Type type) => {
                if (typeof(Component).IsAssignableFrom(type)) {
                    var componentParent = ComponentParent;
                    var components = _gameobjectComponents.GetOrAdd(componentParent, new Dictionary<string, Component>());
                    string componentName = type.Name;// _fileNameInputField.text;
                    Component component;
                    if (components.TryGetValue(componentName, out component)) {
                        Destroy(component);
                    }
                    component = componentParent.AddComponent(type);
                    components.Update(componentName, component);
                    return component;
                }
                else {
                    return Activator.CreateInstance(type);
                }
            },
            destroyInstance = (object instance) => {
                if (instance != null && instance is Component) {
                    if (instance is MonoBehaviour) {
                        var component = (instance as MonoBehaviour);
                        Dictionary<string, Component> components;
                        if (_gameobjectComponents.TryGetValue(component.gameObject, out components)) {
                            components.Remove(component.GetType().ToString());
                        }
                        Destroy(component);
                    }
                }
            }
        };
    }

    private void Update() {
		_codingBoxInputField.ActivateInputField ();
		if(	_codingBoxInputField.caretPosition < gc.contentLength) { //TODO Nullreference
			_codingBoxInputField.caretPosition = gc.contentLength;
		}
//
//		if (Input.GetKeyDown (KeyCode.Backspace) || Input.GetKey(KeyCode.Backspace)) {
//			Debug.Log ("Backspace");
//			if (_codingBoxInputField.caretPosition < gc.contentLength + 1) {
//				_codingBoxInputField.text = gc.output ();
//				_codingBoxInputField.caretPosition = gc.contentLength+1;
//			} else if(_codingBoxInputField.text.Length<gc.contentLength) {
//				_codingBoxInputField.text = gc.output ();
//				_codingBoxInputField.caretPosition = gc.contentLength;
//			}
//		}


		if (Input.GetKeyDown (KeyCode.Delete)) {
			//Debug.Log ("Delete");
		}

		if(Input.GetKeyDown(KeyCode.Return)) {
			if(!scriptingEnabled && _allowRunningCode)MyTestSubmit ();
			//Eingabe bestätigen //Nur Console
			//Nicht wenn in Script
		}

		if (Input.GetKeyDown (KeyCode.F5) && scriptingEnabled) {
			this.Run ();
		}

		if(Input.GetKeyDown(KeyCode.Escape)){
			//Script schließen
			
			if (scriptingEnabled) {

				this.ClearCodingBox ();
				gc.runCommand ("");
				Debug.Log (gc.getStateType ().ToString());
				scriptingEnabled = false;

				String gcOutput = gc.output();
				Debug.Log (gcOutput);
				outputLength = gcOutput.Length;
				WriteToCodingBox(gcOutput);
			}
		}
			
        if (_codingBoxInputField.isFocused && Input.GetKey(KeyCode.LeftControl)
            && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))) {
            //Run();
        }
    }

    private void HighlightCode(string content) {
        _highlightCoroutine = StartCoroutine(_unitySyntaxHighlighter.HighlightSourceCoroutine(content, OnCodingTextHighlighted));
    }



    public void ResetCodeHighlighting(string content) {
		if(	_codingBoxInputField.caretPosition < gc.contentLength){
			if (Input.GetKeyDown (KeyCode.Backspace) || Input.GetKey (KeyCode.Backspace)) {
				//TODO Entfernen abfragen
				string oldtext = _codingBoxInputField.text.Substring (_codingBoxInputField.caretPosition);
				//oldtext="root";
				_codingBoxInputField.onValueChanged.RemoveListener(ResetCodeHighlighting);
				_codingBoxInputField.text = gc.output () + oldtext;
				_codingBoxInputField.onValueChanged.AddListener(ResetCodeHighlighting);
			}
			_codingBoxInputField.caretPosition = gc.contentLength;
		}

    }

	bool scriptingEnabled = false;
	public void MyTestSubmit() {
		String text = _unitySyntaxHighlighter.getCodeWithoutRichText (_codingBoxInputField.text);
		String input = text;
		Debug.Log (text);
		this.ClearCodingBox ();
		gc.input (input);
		if (gc.getStateType () == typeof(GameConsole.Script)) {
			scriptingEnabled = true;
		} else {
			scriptingEnabled = false;
		}

		String gcOutput = gc.output();
		outputLength = gcOutput.Length;
		WriteToCodingBox(gcOutput);
	}

    private void OnCodingTextHighlighted(string content) {
		Debug.Log("OnCodingTextHighlighted");	
        if (!_codingBoxInputField.text.Equals(content)) {
            _codingBoxInputField.onValueChanged.RemoveListener(ResetCodeHighlighting);
            bool isReadonly = _codingBoxInputField.readOnly;
            _codingBoxInputField.readOnly = true;

            _caretPositionOnHighlighting = _codingBoxInputField.caretPosition;
            _caretColorOnHighlighting = _codingBoxInputField.caretColor;
            _codingBoxInputField.caretColor = new Color(0, 0, 0, 0);

            int textLength = _codingBoxInputField.text.Length;
            _codingBoxInputField.text = content;
            StartCoroutine(SetCaret(content.Length >= textLength ? 1 : 0, isReadonly));
        }
    }

    private IEnumerator SetCaret(int offset, bool isReadonly) {
		Debug.Log("SetCaret");
        yield return new WaitForEndOfFrame();
        _codingBoxInputField.caretPosition = _caretPositionOnHighlighting + offset;
        _codingBoxInputField.caretColor = _caretColorOnHighlighting;
        yield return new WaitForEndOfFrame();
        _codingBoxInputField.readOnly = isReadonly;
        _codingBoxInputField.onValueChanged.AddListener(ResetCodeHighlighting);
        _highlightCoroutine = null;
    }
}
