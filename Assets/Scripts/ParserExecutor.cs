using System.Collections;
using System.Collections.Generic;
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
					"endFunction\n" +
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
					"endFunction\n" +
					"abc()\n" +
					//"moveVorward()\n turnLeft(1,2)\n inspect(\"\")\n interact(\"sdsd\",\"aa\")\n abc(1,2,3)\n cde(1,2,3)\n" +
					"loop(i = 1:5)\n moveVorward()\n print(\"5x loop\")\n endLoop\n" +
					"loop(i = testA:5)\n cde()\n print(\"testA:5 loop\")\n endLoop\n" +
					"loop(i = 1:testB)\n moveVorward()\n print(\"1:testB loop\")\n endLoop\n" +
					"loop(i = testA:testB)\n moveVorward()\n print(\"testA:testB loop\")\n endLoop\n" +
					"loop(i = testA:testC)\n moveVorward()\n print(\"testA:testC loop\")\n endLoop\n" +

          "if(true)\n"+
					//" print(\"1. IF\")\n"+
          " turnRight()\n"+
          "elseif(true)\n"+
					//" print(\"1. ELSEIF\")\n"+
          " moveVorward()\n"+
          " moveVorward()\n"+
					"endif\n"+
					"                        sdsd                   \n"+
          "if(true)\n"+
					" print(\"2. IF\")\n"+
          " turnLeft()\n"+
          " turnRight()\n"+
          "elseif(true)\n"+
					" print(\"2. ELSEIF\")\n"+
					" turnRight()\n"+
          " turnLeft()\n"+
          " moveVorward()\n"+
					"else\n"+
					//" print(\"3. ELSE\")\n"+
          " turnLeft()\n"+
          " moveVorward()\n"+
					"endif\n"+
					"                        fur                   \n"+
					"if(testC + 2)\n"+
					" print(\"3. IF\")\n"+
          " moveVorward()\n"+
          " turnRight()\n"+
          "else\n"+
					//" abc()\n"+
					" print(\"3. ELSE\")\n"+
          " turnRight()\n"+
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
