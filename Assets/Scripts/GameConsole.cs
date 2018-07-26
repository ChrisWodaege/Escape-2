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
		Debug.Log ("Eingabe: " + input);
		input = input.Substring (this.contentLength);
		if((int)input[input.Length-1]==10) input = input.Substring (0,input.Length-1);	//Enter entfernen
		Debug.Log ("Gefiltert: " + input);
		gcs = gcs.input(input.ToLower().Split(new char[]{' '}));
		if (!gcs.initialised)gcs.init ();
	}

	public string output()	{

		String[] entrys = gcs.getMenuEntrys();
		String text = gcs.title+"\n";
		if(gcs.text.Length>0)text += gcs.text +"\n";
		if(gcs.showHelp) {
			text += "Valide Kommandos: ";
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

	public class Log : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####Log####";
			this.text = "Eines unserer Aufklärungsraumschiffe ist auf dem Planeten >>Erde<< beim Versuch einer Kontaktaufnahme, in einem schwer zugänglichen Gebiet, abgestürzt. Versuche den Wartungsroboter zu starten und das Schiff zu reparieren";
			this.menuEntrys.Add("",new MainMenu());
		}
	}

	public class Script : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "##### SCRIPT - CONSOLE ####";
			this.text = "Valide Kommandos: boot() | move() | turnleft() | turnright() | take() | drop()\n" +
				"F5 - RUN SCRIPT | ESC - EXIT.\n\n" +
				"\n\n //Hier kommt dann dein Code rein";
			this.menuEntrys.Add("",new MainMenu());
		}
	}

	public class HelpMenu : GameConsoleState {
		public override void init() {
			initialised = true;
			showHelp = true;
			this.title = "#####Manual####";
			this.menuEntrys.Add("boot",new HelpMenu_BootAction());
			this.menuEntrys.Add("move",new HelpMenu_MoveAction());
			this.menuEntrys.Add("turn",new HelpMenu_TurnAction());
			this.menuEntrys.Add("take",new HelpMenu_TakeAction());
			this.menuEntrys.Add("drop",new HelpMenu_DropAction());
			this.menuEntrys.Add("loop",new HelpMenu_LoopAction());
			this.menuEntrys.Add("branch",new HelpMenu_BranchAction());
			this.menuEntrys.Add("back",new MainMenu());
		}
	}


	public class HelpMenu_LoopAction : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####Manual - Loop####";
			this.text = "Mit loop kannst du dem Roboter die Anweisung geben Befehle in einer Schleife zu wiederholen.\nBsp: Den Roboter 5 mal laufen lassen\nloop(1:5)\nmove()\nendloop";
			this.menuEntrys.Add("",new HelpMenu());
		}
	}

	public class HelpMenu_BranchAction : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####Manual - Branches####";
			this.text = "Branches ermöglichen es dir Aktionen deines Roboters anhand von Bedingungen zu verzweigen.\nBsp: Wenn die Variable \"bedingung\" true ist, wird move() ausgeführt,ansonstent turnleft().\n\n bedingung = true\n if(bedingung)\nmove()\nelse\nturnleft()\nendIf";
			this.menuEntrys.Add("",new HelpMenu());
		}
	}

	public class HelpMenu_TakeAction : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####Manual - take()####";
			this.text = "Der Befehl take() gib dem Roboter die Anweisung einen vor ihm befindlichen Gegenstand (Item/Stein) aufzunehmen.\n Der Roboter kann immer nur einen Gegenstand halten.";
			this.menuEntrys.Add("",new HelpMenu());
		}
	}

	public class HelpMenu_DropAction : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####Manual - drop()####";
			this.text = "Der Befehl drop() gib dem Roboter die Anweisung einen vor ihm befindlichen Gegenstand (Item/Stein) abzulegen.\nSteine können im Wasser abgelegt werden, um so Wasser zu überqueren.";

			this.menuEntrys.Add("",new HelpMenu());
		}
	}

	public class HelpMenu_BootAction : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####Manual - boot()####";
			this.text = "Der Befehl boot() aktiviert den Roboter.";
			this.menuEntrys.Add("",new HelpMenu());
		}
	}

	public class HelpMenu_MoveAction : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####Manual - Move()####";
			this.text = "Der Befehl Move lässt deinen Roboter in die Richtung laufen, in welche er gerade schaut.\nMove wird nicht ausgeführt, wenn der Roboter vor einem Hindernis steht.";
			this.menuEntrys.Add("",new HelpMenu());
		}
	}

	public class HelpMenu_TurnAction : GameConsoleState {
		public override void init() {
			initialised = true;
			this.title = "#####Manual - TurnLeft()/TurnRight()####";
			this.text = "Der Roboter kann neu ausgerichtet werden, indem er sich nach rechts oder links dreht. Dafür gibt es die Befehle turnleft() und turnright().";
			this.menuEntrys.Add("",new HelpMenu());
		}
	}
}

