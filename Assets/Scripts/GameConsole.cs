using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

class GameConsole {
	public int contentLength;
	public static void Main(string[] args) {
		GameConsole gc = new GameConsole ();
		while(true){
			Console.Clear ();
			Console.WriteLine (gc.output());	
			gc.input(Console.ReadLine ());
		}
	}

	private GameConsoleState gcs;
	public GameConsole(){
		gcs = new Standby ();
		gcs.init ();
	}

	public Type getStateType(){
		return this.gcs.GetType();
	}

	public void runCommand(string command){
		gcs = gcs.input(command.ToLower().Split(new char[]{' '}));
		if (!gcs.initialised)gcs.init ();
	}

	public void input(string input){
		Debug.Log (input);
		input = input.Substring (this.contentLength);
		input = input.Substring (0,input.Length-1);	//Enter entfernen
		gcs = gcs.input(input.ToLower().Split(new char[]{' '}));
		if (!gcs.initialised)gcs.init ();
	}

	public string output()	{

		String[] entrys = gcs.getMenuEntrys();
		String text = gcs.title+"\n";
		if(gcs.text.Length>0)text += gcs.text +"\n";
		if(gcs.showHelp) {
			text += "Valid commands: ";
			for (int i = 0; i < entrys.Length; i++) {
				if(entrys [i].Length>0)text += entrys [i];
				if(i<entrys.Length-1)text += " | ";
			}
			text += "\n";
		}
		this.contentLength = text.Length;
		return text;
	}

	public class GameConsoleState {
		public string title,text;
		public bool initialised;
		public bool showHelp;
		public Dictionary<String, GameConsoleState> menuEntrys;

		public GameConsoleState() {
			title = "";
			text = "";
			initialised = false;
			this.menuEntrys = new Dictionary<String, GameConsoleState>();
		}

		public virtual void init(){
			this.title = "EMPTY";
		}

		public string[] getMenuEntrys(){
			string[] result = new string[this.menuEntrys.Count];
			this.menuEntrys.Keys.CopyTo (result, 0);
			return result;
		}

		public GameConsoleState input(String[] args) {
			if (this.menuEntrys.ContainsKey (args [0])) {
				return this.menuEntrys [args [0]];
			} else {
				showHelp = true;
				return this;
			}
		}
	}



	public class MainMenu : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####MAIN####";
			this.menuEntrys.Add("script",new Script());
			this.menuEntrys.Add("log",new Log());
			this.menuEntrys.Add("manual",new HelpMenu());
		}
	}

	public class Standby : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "STANDBY ...";
			this.menuEntrys.Add("boot",new MainMenu());
		}
	}

//	public class Boot : GameConsoleState {
//		public override void init() {
//			initialised = true;
//			this.title = "STANDBY ...";
//			this.text = "Ungültige Eingabe. Boot eingeben um das System zu starten";
//			this.menuEntrys.Add("boot",new MainMenu());
//		}
//	}

	public class Log : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####Log####";
			this.text = "Du bist mit deinem Raumschiff auf einem Planeten gestrandet.\nAktiviere den Wartungsroboter um das Schiff zu reparieren.";
			this.menuEntrys.Add("",new MainMenu());
		}
	}

	public class Script : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "##### SCRIPT - CONSOLE ####";
			this.text = "Mit dieser Konsole kannst du den Roboter steuern. Gib INSERT ein um in den Bearbeitungsmodus zu wechseln.\n" +
						"Mit ESC kannst du den Modus verlassen. Gib RUN ein um das Script auszuführen.\n" +
						"#########################################################################################################"+
				"\n\n //Hier kommt dann dein Code rein";
			this.menuEntrys.Add("",new MainMenu());
		}
	}

	public class HelpMenu : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####HELP####";
			this.menuEntrys.Add("move",new HelpMenu_MoveAction());
			this.menuEntrys.Add("turn",new HelpMenu_TurnAction());
			this.menuEntrys.Add("back",new MainMenu());
		}
	}

	public class HelpMenu_MoveAction : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####HELP - Move()####";
			this.text = "Der Befehl Move lässt deinen Roboter in die Richtung laufen, in welche er gerade schaut.\nMove wird nicht ausgeführt, wenn der Roboter vor einem Hindernis steht.";
			this.menuEntrys.Add("",new HelpMenu());
		}
	}

	public class HelpMenu_TurnAction : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####HELP - TurnLeft()/TurnRight()####";
			this.text = "Der Roboter kann neu ausgerichtet werden, indem er sich nach rechts oder links dreht. Dafür gibt es die Befehle turnLeft() und turnRight().";
			this.menuEntrys.Add("",new HelpMenu());
		}
	}
}

