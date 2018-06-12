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
			string sourceCode = ""+
					//"function a endFunction function b endFunction function c endFunction\n" +
					"var testA = 1 + 2\n"+
					"var testB = testA / testA\n" +
					"var testB = testA\n" +
					"function abc(a, b, c)\n" +
					"var z = 0\n" +
					"var a = 0\n" +
					"moveVorward()\n" +
					"endFunction\n" +
					"function cde(c, d, e)\n" +
					"/* var testA = 1 + 2\n"+
					"var testB = testA / testA */\n" +
					"//var testA = testA / testA\n" +
					"moveVorward()\n" +
					"turnLeft()\n" +
					"turnLeft()\n" +
					"turnRight()\n" +
					"moveVorward()\n" +
					"endFunction\n" +
					"moveVorward()\n turnLeft(1,2)\n inspect(\"\")\n interact(\"sdsd\",\"aa\")\n abc(1,2,3)\n cde(1,2,3)\n" +
					"loop(i = 1:5)\n moveVorward()\n endLoop\n" +
					"loop(i = testA:5)\n moveVorward()\n endLoop\n" +
					"loop(i = 1:testB)\n moveVorward()\n endLoop\n" +
					"loop(i = testA:testB)\n moveVorward()\n endLoop\n" +
					"loop(i = testA:testC)\n moveVorward()\n endLoop\n" +
					"";

			List<string> validCommands = new List<string>();

			validCommands.Add("moveVorward()");
			validCommands.Add("moveBack()");
			validCommands.Add("turnLeft()");
			validCommands.Add("turnRight()");
			validCommands.Add("turnLeft(#,#)");
			validCommands.Add("turnRight(#,#)");
			validCommands.Add("inspect(#)");
			validCommands.Add("interact(#,#)");

			Parser.parse(sourceCode, validCommands);
	}
}
