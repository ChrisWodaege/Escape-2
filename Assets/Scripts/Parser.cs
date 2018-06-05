using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

public class Parser : MonoBehaviour {

    public class Command
    {
        string name = "";
        string args = "";

        public Command(string name, string args = "") { this.name = name; this.args = args; }

        public string ToString() { return name+"("+ args + ")"; }
    };

    List<Command> commands = null;

    public static string Obj2string(Command obj)
    {
        return obj.ToString();
    }

    public static string toString(List<Command> list)
    {
        //string str = "";
        //foreach(var obj in list) str += "," + obj.ToString();
        //return str;
        return string.Join("\n", list.ConvertAll<string>(new Converter<Command, string>(Obj2string)).ToArray());
    }

    // Use this for initialization
    void Start() { testParser(); }

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
            "moveVorward()\n" +
            "turnLeft()\n" +
            "turnLeft()\n" +
            "turnRight()\n" +
            "moveVorward()\n" +
            "endFunction\n" +
            "moveVorward(); turnLeft(1,2); inspect(\"\"); interact(\"sdsd\",\"aa\"); abc(1,2,3); cde(1,2,3)\n" +
            "loop(i = 1:5); moveVorward(); endLoop\n" +
            "loop(i = testA:5); moveVorward(); endLoop\n" +
            "loop(i = 1:testB); moveVorward(); endLoop\n" +
            "loop(i = testA:testB); moveVorward(); endLoop\n" +
            "loop(i = testA:testC); moveVorward(); endLoop\n" +
            "";
        parse(sourceCode);
    }

	// Update is called once per frame
	void Update () {
		
	}

    string regexReplace(string regex, string text, string rpl)
    {
        Regex rgx = new Regex(regex);
        return rgx.Replace(text, rpl);
    }

    struct RegMatch
    {
        public Match match;
        public List<string> list;

        public RegMatch(Match match, List<string> matched) { this.match = match; this.list = matched; }
    }

    List<RegMatch> matchAll(string regex, string text)
    {
        List<RegMatch> matchlist = new List<RegMatch>();
        //string text = "One car red car blue car";
        //string pat = @"(\w+)\s+(car)";

        // Instantiate the regular expression object.
        Regex r = new Regex(regex, RegexOptions.IgnoreCase);

        // Match the regular expression pattern against a text string.
        Match m = r.Match(text);
        //int matchCount = m.Length;

        while (m.Success)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < m.Groups.Count; i++)
            {
                Group g = m.Groups[i];
                list.Add(g.Value);
                //CaptureCollection cc = g.Captures;
                //for (int j = 0; j < cc.Count; j++) { Capture c = cc[j]; }
            }
            matchlist.Add(new RegMatch(m,list));
            m = m.NextMatch();
        }

        return matchlist;
    }

    struct Function
    {
        public string line;
        public string type;
        public string name;
        public string head;
        public string body;

        public bool parsedHead;
        public bool parsedBody;

        public Function(string type, string line, string name, string head, string body)
        {
            this.line = line.Trim();
            this.type = type;
            this.name = name;
            this.head = head;
            this.body = body;

            this.parsedHead = false;
            this.parsedBody = false;
        }
         /*
        public Function(Function f)
        {
            this.type = f.type;
            this.name = f.name;
            this.args = f.args;
            this.body = f.body;
        }*/

        override
        public string ToString()
        {
            return name + (type == "VAR" ? " = " + body : "(" + head + ")");
        }

        public string ToCompleteString()
        {
            return name + (type == "VAR" ? " = " + body : "(" + head + "){" + body + "}");
        }
    }

    bool parseVar(ref List<Function> functions, string name, out string value, int depth=4)
    {
        float fvalue = 0;
        if (float.TryParse(name, out fvalue)) { value=name; return true; }

        if(depth>0) foreach (var f in functions)
        {
            if (f.type == "VAR" && name == f.name)
            {
                return parseVar(ref functions, f.body, out value, depth-=1);
            }
        }

        // function not found
        value = name;
        return false;
    }

    void updateBody(ref List<Function> functions, int index, string value)
    {
        var func = functions[index];
        func.body = value;
        functions[index] = func;
    }

    public static string listToString(List<string> strings)
    {
        string str = "";
        foreach(string s in strings) { if (str != "") str += "\n"; str += s; }
        return str;
    }

    public class ParserException : Exception
    {
        public List<string> messages;
        public ParserException(List<string> messages) : base(listToString(messages))
        {
            this.messages = messages;
        }
    }


    public List<string> parse(string sourceCode)
    {
        List<string> list = new List<string>();
        string result = "";
        string[] results = parsePart(sourceCode,0);

        string find = results[0];
        string code = results[1];
        string cmds = results[2];
        string error = results[3];

        result += find;
        result += "--------------------------\n";
        result += code;
        result += "==========================\n";
        result += cmds;

        //File.WriteAllText("parsed.txt", toString(commands));
        File.WriteAllText("parsed.txt", result);

        foreach (string s in cmds.Split('\n')) { list.Add(s); }

        // an parser error occured
        if (error.Length > 0) {
            List<string> errors = new List<string>();
            foreach (string s in error.Split('\n')) { errors.Add(s); }
            throw new ParserException(errors);
        }

        return list;
    }

    string[] parsePart(string code, int depth)
    {
        List<Function> functions = new List<Function>();

        string found = "";
        string errors = "";

        code = code.Replace("\r","");
        code = new Regex(@"(\w+)[ \t]*([*/+-])[ \t]*(\w+)").Replace(code, "$2($1,$3)");
        code = new Regex(@"var[ \t]+(\w+)[ \t]+\=[ \t]+([^\n;]+)[ \t]*([\n;]|$)").Replace(code, "var_$1($2)\n");
        code = code.Replace(";", "\n");
        code = new Regex(@"(^|\n)[\t ]+(.*)").Replace(code, "$1$2");
        //new Regex(@"(?s)\n+").Replace(code, "\n");

        string cmds = code;
        //code = code.Replace("while(true)","");

        if (depth == 0)
        {
            //@"function[ \t]+(\w+)\((([^(],)?([^)]+))*\)[ \t]*[\n;](?s)(.*)endFunction[ \t]*[\n;]"
            var list = matchAll(@"(?<function>function[ \t]+(\w+)\(([^)]*)\)[ \t]*\n(?s)((?:(?!endFunction).)*)endFunction[ \t]*(\n|$))", code);
            //functions = matchAll(@"(?<function>function[ \t]+(\w+)\(([^)]*)\)[ \t]*[\n;](?s)((.*)?)endFunction[ \t]*([\n;]|$))", code);

            if (list.Count > 0) found += "Functions(" + list.Count + ")\n";
            foreach (var r in list)
            {
                var e = r.list;
                var f = new Function("CUSTOM", e[0], e[1], e[2], e[3]);
                functions.Add(f);

                found += "\t" + f.ToString() + "\n";
                code = code.Replace(e[0], ""); // remove
            }

            cmds = code; // remove custom definitions of custom functions
        }

        /* // parse mutliple if, elseif, last is else or nothing
        {
            var list = matchAll(@"(?<if>if[ \t]*\(([^)]*)\)[ \t]*\n(?s)((?:(?!endif)).)*)endif[ \t]*(\n|$))", code);

            if (list.Count > 0) found += "Loops(" + list.Count + ")\n";
            int i = -1;
            foreach (var e in list)
            {
                ++i;
                var f = new Function("LOOP", e[0], "loop" + i, e[1], e[2]);
                functions.Add(f);

                found += "\t" + f.ToString() + "\n";
                code = code.Replace(e[0], ""); // remove
            }
        }
        */

        {
            var list = matchAll(@"(?<loop>loop[ \t]*\(([^)]*)\)[ \t]*\n(?s)((?:(?!endLoop).)*)endLoop[ \t]*(\n|$))", code);

            if(list.Count>0) found += "Loops(" + list.Count + ")\n";
            int i = -1;
            foreach (var r in list)
            {
                var e = r.list;
                ++i;
                var f = new Function("LOOP", e[0], "loop" + i, e[1], e[2]);
                functions.Add(f);

                found += "\t" + f.ToString() + "\n";
                code = code.Replace(e[0], ""); // remove
            }
        }

        {
            //var vars = matchAll(@"var[ \t]+(\w+)[ \t]+\=[ \t]+([^\n;]+)[ \t]*([\n;]|$)", code);
            var list = matchAll(@"(?<var>var_(\w+)\(([^\n]*)?\)[ \t]*(\n|$))", code);

            if (list.Count > 0) found += "Vars(" + list.Count + ")\n";
            foreach (var r in list)
            {
                var e = r.list;
                var f = new Function("VAR", e[0], e[1], "", e[2]);
                functions.Add(f);

                found += "\t" + f.ToString() + "\n";
                code = code.Replace(e[0], ""); // remove

                //commands.Add(new Command("var_" + f.name, f.body));
            }
        }

        {
            //var calls = matchAll(@"(\w+)\((([^(],)?([^)]+))*\)[ \t]*([\n;]|$)", code);
            var list = matchAll(@"(?<call>(\w+)\(([^\n]*)?\)[ \t]*(\n|$))", code);

            if (list.Count > 0) found += "Calls(" + list.Count + ")\n";
            foreach (var r in list)
            {
                var e = r.list;
                var f = new Function("CALL", e[0], e[1], e[2], "");
                functions.Add(f);

                found += "\t" + f.ToString() + "\n";
                code = code.Replace(e[0], ""); // remove
                                               //commands.Add(new Command(f.name, f.head));
            }
        }

        // Parse heads and bodies
        {
            // parse all vars
            int findex = -1;
            foreach (var f in functions)
            {
                findex++;
                if (f.type == "VAR")
                {
                    var rbody = matchAll(@"([*/+-])\((\w+),(\w+)\)", f.body);
                    
                    var result = f.body;

                    if (rbody.Count > 0 && rbody[0].list.Count > 3)
                    {
                        var body = rbody[0];

                        string s_op = body.list[1];
                        string s_left = body.list[2];
                        string s_right = body.list[3];

                        bool isLeftNumeric = parseVar(ref functions, s_left, out s_left);
                        bool isRightNumeric = parseVar(ref functions, s_right, out s_right);

                        if (isLeftNumeric && isLeftNumeric)
                        {
                            float left = 0;
                            float right = 0;
                            float.TryParse(s_left, out left);
                            float.TryParse(s_right, out right);

                            result = "";
                            if (s_op == "+") { result += left + right; }
                            else if (s_op == "-") { result += left - right; }
                            else if (s_op == "*") { result += left * right; }
                            else if (s_op == "/") { result += left / right; }
                            else {
                                result = f.body;
                                errors += "Error: Unhandled Operator '" + s_op + "' for (" + left + "," + right + ") on Index " + body.match.Index;
                                //Debug.LogError("Error: Unhandled Operator '" + s_op + "' for (" + left + "," + right + ")");
                            }
                        } else
                        {
                            errors += name + " : (" + s_left + "," + s_right + ") is not updated on Index " + body.match.Index;
                            //Debug.LogError(name + " : (" + s_left + "," + s_right + ") is not updated!");
                        }
                    }
                    else
                    {
                        parseVar(ref functions, result, out result);
                    }

                    // update
                    updateBody(ref functions, findex, result);
                    if (functions[findex].body != result) Debug.LogError(f.name + " not updated!");
                    found = found.Replace(f.ToString(), f.name + " = " + result);
                }
            }

            foreach (var f in functions)
            {
                if (f.type == "CALL")
                {
                   /*
                   var head = matchAll(@"[^,]+", f.head);
                   foreach (var e in list)
                   {
                       var name = e[0];

                       foreach (var f2 in functions)
                       {
                           if (f2.type == "VAR" && f2.name == name)
                           {
                               f.head = f.head.Replace(f2.name, f2.body);
                               break;
                           }
                       }
                   }
                   found = found.Replace(f.ToString(), f.name + "(" + f.head + ")");
                   */
                    foreach (var f2 in functions)
                    {
                        if (f2.type == "CUSTOM" && f2.name == f.name)
                        {
                            string[] parsed = parsePart(f2.body, depth + 1);
                            string parsedline = "(" + f.name + "):\n";
                            parsedline += parsed[0];
                            parsedline += "--------------------------\n";
                            parsedline += parsed[1];
                            parsedline += "__________________________\n";
                            found = found.Replace(f.ToString(), parsedline);
                            break;
                        }
                    }
                }
                else if (f.type == "LOOP")
                {
                    var rhead = matchAll(@"i[ \t]*=[ \t]*(\w+)\:(\w+)", f.head);

                    if (rhead.Count>0 && rhead[0].list.Count>2) {
                        var head = rhead[0];

                        string s_start = head.list[1];
                        string s_end = head.list[2];

                        bool isStartNumeric = parseVar(ref functions, s_start, out s_start);
                        bool isEndNumeric = parseVar(ref functions, s_end, out s_end);

                        if (isStartNumeric && isEndNumeric)
                        {
                            int start = 0;
                            int end = 0;

                            Int32.TryParse(s_start, out start);
                            Int32.TryParse(s_end, out end);

                            string[] parsed = parsePart(f.body, depth + 1);
                            bool p1 = parsed[0].Length > 0;
                            bool p2 = parsed[1].Length > 0;
                            bool p3 = parsed[2].Length > 0;

                            string repeat1 = "";
                            string repeat2 = "";
                            string repeat3 = "";

                            if (start > 0 && end > 0) for (int index = start; index <= end; index++)
                            {
                                if (p1) repeat1 += parsed[0] + "\n";
                                if (p2) repeat2 += parsed[1] + "\n";
                                if (p3) repeat3 += parsed[2] + "\n";
                            }

                            string parsedline = "(LOOP " + start + " -> " + end + "):\n";
                            parsedline += repeat1;
                            parsedline += "--------------------------\n";
                            parsedline += repeat2;
                            parsedline += "__________________________\n";

                            string result = repeat3;
                            updateBody(ref functions, findex, result);
                            //if (functions[findex].body != result) Debug.LogError(f.name + " not updated!");
                            found = found.Replace(f.ToString(), parsedline);

                            var cmdline = functions[findex].body;
                            cmds = cmds.Replace(f.line, cmdline);
                        }
                        else
                        {
                            errors += name + " : cannot parse loop on Index " + head.match.Index;
                            // error cant parse line ...
                            // var cannot be found or is invalid
                        }
                    }
                }
                else if(f.type == "IF")
                {
                    // parse heads of mutliple if, elseif, last is else or nothing

                    //head
                    //body

                    //head
                    //body

                    //head
                    //body
                }
            }
        }

        cmds = new Regex(@"\n{2,}").Replace(cmds, "\n");
        var chars = matchAll(@"[^ \t\n]", code);
        if (chars.Count==0) { code = ""; }
        return new string[]{found,code,cmds,errors};
    }


    /*
    int pos = 0;
    int len = 0;
    string str = "";
    string cmd = "";
    List<string> find = null;
    int index = 0;

    while (notfound) {
        ++len;
        str = code.Substring(pos, len);
        index = str.IndexOfAny(" \n\r\t".ToCharArray());
        if(index != -1) { ++pos; len=0; }
        find = new List<string>(new string[] { "var", "function" });
        index = find.IndexOf(str);
        if(index != -1) { cmd=find[index]; ++pos; len=0; }
        if (str == "var") ;
        //var match = str.IndexOfAny("abcedfghijklmnopqrstuvxyz".ToCharArray()) != -1
        if (cmd == "function") ;
    }*/
}