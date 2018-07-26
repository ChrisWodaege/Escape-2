using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ParserExecutor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		 testParser();
	}

	// Update is called once per frame
	void Update () {

	}

	void testParser() {
			var testString = "abc 123(xaf4 7iukz(788ztg?fd dwht)text(9hfe jrfs(76hgf fe54)))dfs4 4wr";
			File.WriteAllText("regex.txt", Parser.regexReplace(testString, @"\((?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)","(found)"));

			string sourceCode = //"moveVorward()\nturnLeft()\nturnRight()";
					//"function a endFunction function b endFunction function c endFunction\n" +
					"testA = 1 + 2\n"+
					"testB = testA / testA\n" +
					"testB = testA\n" +
					"testC = testB - testA\n" +
					"turnLeft()\n"+
					"print(\"root\")\n"+

					"function abc(a, b, c)\n" +
					"print(\"custom abc ->\")\n"+
					"z = 0\n" +
					"a = 0\n" +
					"moveVorward()\n" +
					"print(\"<- custom abc\")\n"+
					"endfunction\n" +

					"function cde(c, d, e)\n" +
					"print(\"custom cde ->\")\n"+
					"/* testA = 1 + 2\n"+
					"testB = testA / testA */\n" +
					"moveVorward()\n" +
					"//testA = testA / testA\n" +
					"turnLeft()\n" +
					"turnLeft()\n" +
					"turnRight()\n" +
					"moveVorward()\n" +
					"print(\"<- custom cde\")\n"+
					"endfunction\n" +

					"abc()\n" +
					//"moveVorward()\n turnLeft(1,2)\n inspect(\"\")\n interact(\"sdsd\",\"aa\")\n abc(1,2,3)\n cde(1,2,3)\n" +
					"loop(1:5)\n print(\"loop 1:5 ->\")\n moveVorward()\n print(\"<- loop 1:5\")\n endloop\n" +
					"loop(testA:5)\n print(\"loop testA:5 ->\")\n cde()\n 56g\n print(\"<- loop testA:5\")\n endloop\n" +
					"loop(1:testB)\n print(\"loop 1:testB ->\")\n moveVorward()\n print(\"<- loop 1:testB\")\n endloop\n" +
					"loop(testA:testB)\n print(\"loop testA:testB ->\")\n moveVorward()\n print(\"<- loop testA:testB\")\n endloop\n" +
					"loop(testA:testC)\n print(\"loop testA:testC ->\")\n moveVorward()\n print(\"<- loop testA:testC\")\n endloop\n" +

          "if(testA == 3)\n"+
					" print(\"1. IF ->\")\n"+
          " turnRight()\n"+
					" print(\"<- 1. IF\")\n"+
          "elseif(true)\n"+
					" print(\"1.1 ELSEIF\")\n"+
          " moveVorward()\n"+
          " moveVorward()\n"+
					" print(\"<- 1.1 ELSEIF\")\n"+
					"endif\n"+
					"                        sdsd                   \n"+
          "if(true)\n"+
					" print(\"2. IF ->\")\n"+
          " turnLeft()\n"+
          " turnRight()\n"+
					" hhh\n"+
					" print(\"<- 2. IF\")\n"+
          "elseif(true)\n"+
					" print(\"2.1 ELSEIF\")\n"+
					" turnRight()\n"+
          " turnLeft()\n"+
          " moveVorward()\n"+
					" print(\"<- 2.2 ELSEIF\")\n"+
					"else\n"+
					" print(\"2. ELSE ->\")\n"+
          " turnLeft()\n"+
          " moveVorward()\n"+
					" print(\"<- 2. ELSE\")\n"+
					"endif\n"+
					"                        fur                   \n"+
					"if(testC + 2)\n"+
					" print(\"3. IF ->\")\n"+
          " moveVorward()\n"+
          " turnRight()\n"+
					" print(\"<- 3. IF\")\n"+
          "else\n"+
					//" abc()\n"+
					" print(\"3. ELSE ->\")\n"+
          " turnRight()\n"+
					" print(\"<- 3. ELSE\")\n"+
          "endif\n" +
					"";

			List<string> validCommands = new List<string>();
			validCommands.Add("print"); //()
			validCommands.Add("moveVorward"); //()
			validCommands.Add("moveBack"); //()
			validCommands.Add("turnLeft"); //()
			validCommands.Add("turnRight"); //()
			validCommands.Add("turnLeft"); //(#,#)
			validCommands.Add("turnRight"); //(#,#)
			validCommands.Add("inspect"); //(#)
			validCommands.Add("interact"); //(#,#)

			Parser.parse(sourceCode, validCommands);
	}
}
