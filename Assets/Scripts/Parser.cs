using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

public static class Parser {
  /*
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
    }*/

    public class ParserException : Exception
    {
        public List<string> messages;
        public ParserException(List<string> messages) : base(listToString(messages))
        {
            this.messages = messages;
        }
    }

    public class Command
    {
        public int index = -1;
        public string code = "";

        public Command(int index, string code){ this.index=index; this.code = code; }
    }

    public class Function
    {
        public Match match = null;
        public int index = -1;
        public int nameIndex = -1;
        public int headIndex = -1;
        public int bodyIndex = -1;

        public string line = "";
        public string type = "";
        public string name = "";
        public string head = "";
        public string body = "";

        public bool parsedHead = false;
        public bool parsedBody = false;

        public void setParentIndex(int parent_index) {
          if(this.index>=0) this.index += parent_index;
          if(this.nameIndex>=0) this.nameIndex += parent_index;
          if(this.headIndex>=0) this.headIndex += parent_index;
          if(this.bodyIndex>=0) this.bodyIndex += parent_index;
        }

        public Function(string type, Match match, int name, int head, int body)
        {
            var g = match.Groups;
            this.nameIndex = g[name].Captures[0].Index;
            this.headIndex = g[head].Captures[0].Index;
            this.bodyIndex = g[body].Captures[0].Index;
            this.set(type, match, g[name].Value, g[head].Value, g[body].Value);
        }

        public Function(string type, Match match, string name, int head, int body)
        {
            var g = match.Groups;
            this.headIndex = g[head].Captures[0].Index;
            this.bodyIndex = g[body].Captures[0].Index;
            this.set(type, match, name, g[head].Value, g[body].Value);
        }

        public Function(string type, Match match, string name, string head, int body)
        {
            var g = match.Groups;
            this.bodyIndex = g[body].Captures[0].Index;
            this.set(type, match, name, head, g[body].Value);
        }

        public Function(string type, Match match, int name, string head, int body)
        {
            var g = match.Groups;
            this.nameIndex = g[name].Captures[0].Index;
            this.bodyIndex = g[body].Captures[0].Index;
            this.set(type, match, g[name].Value, head, g[body].Value);
        }

        public Function(string type, Match match, string name, int head, string body)
        {
            var g = match.Groups;
            this.headIndex = g[head].Captures[0].Index;
            this.set(type, match, name, g[head].Value, body);
        }

        public Function(string type, Match match, int name, int head, string body)
        {
            var g = match.Groups;
            this.nameIndex = g[name].Captures[0].Index;
            this.headIndex = g[head].Captures[0].Index;
            this.set(type, match, g[name].Value, g[head].Value, body);
        }

        public Function(string type, Match match, string name, string head, string body)
        {
            this.set(type, match, name, head, body);
        }

        public void set(string type, Match match, string name, string head, string body)
        {
            this.match = match;
            this.index = match.Index;
            this.line = match.Groups[0].Value.Trim();
            this.type = type;
            this.name = name;
            this.head = head;
            this.body = body;

            this.parsedHead = false;
            this.parsedBody = false;
        }

        override
        public string ToString()
        {
            return name + (type == "VAR" ? " = " + body : "(" + head + ")");
        }

        public string ToCompleteString()
        {
            return name + (type == "VAR" ? " = " + body : "(" + head + "){" + body.Replace("\n",";") + "}");
        }
    }

    public class ParseNode
    {
        public ParseNode parent = null;

        public List<Function> functions = new List<Function>();

        public ParseNode(){}
        public ParseNode(ref ParseNode parent){ this.parent = parent;   }
    }

    public class ParserObject
    {
        public ParseNode root = new ParseNode();
        public string sourceCode = "";
        public char[] parsedCode = new char[]{};

        public int pointer = 0;
        public int lineNr = 0;
        public List<string> validCommands = new List<string>();
        public List<Command> commands = new List<Command>();
        public List<string> errors = new List<string>();

        public ParserObject(string sourceCode) {
          this.sourceCode=sourceCode;
          this.parsedCode=sourceCode.ToCharArray();
        }

        public string getParsedCode() { return new String(this.parsedCode); }

        public void replaceParsedCode(string find, string rpl) {
          this.parsedCode = this.getParsedCode().Replace(find,rpl).ToCharArray();
        }

        public void AddCommand(string code, int index) { commands.Add(new Command(index,code)); }

        public void AddError(string msg, string code, int index) {
            var r = (sourceCode.Substring(0,index)).Split('\n');
            var row = r.Length;
            var col = r[row-1].Length;
            if(row<1) row = 1;
            if(col<1) col = 1;

            AddError(msg, code, row, col);
        }

        public void AddError(string msg, string code, int row, int col) {
          errors.Add(msg + " \"" + code.Trim() + "\" on Line " + row + " at " + col);
        }
    }

    public static string listToString(List<string> strings)
    {
        string str = "";
        foreach(string s in strings) { if (str != "") str += "\n"; str += s; }
        return str;
    }

    public static string regexReplace(string text, string regex, string rpl)
    {
        Regex rgx = new Regex(regex);
        return rgx.Replace(text, rpl);
    }

    public static bool isMatch(string regex, string text)  {
      return new Regex(regex, RegexOptions.IgnoreCase).Match(text).Success;
    }

    public static List<Match> matchAll(string regex, string text)
    {
        List<Match> matchlist = new List<Match>();
        //string text = "One car red car blue car";
        //string pat = @"(\w+)\s+(car)";

        // Instantiate the regular expression object.
        Regex r = new Regex(regex, RegexOptions.IgnoreCase);

        // Match the regular expression pattern against a text string.
        Match m = r.Match(text);
        //int matchCount = m.Length;

        while (m.Success)
        {
          /*
            List<Group> list = new List<Group>();
            for (int i = 0; i < m.Groups.Count; i++)
            {
                Group g = m.Groups[i];
                list.Add(g.Value);

                //CaptureCollection cc = g.Captures;
                //for (int j = 0; j < cc.Count; j++) { Capture c = cc[j]; }
            }*/
            matchlist.Add(m);
            m = m.NextMatch();
        }

        return matchlist;
    }

    public static void updateBody(ref ParseNode node, int index, string value)
    {
        var func = node.functions[index];
        func.body = value;
        node.functions[index] = func;

        // debug
        if (node.functions[index].body != value) Debug.LogError(func.name + " not updated!");
    }

    public static bool isValidCommand(ref ParserObject obj, string content) {
        return obj.validCommands.Contains(new Regex(@"\([^\)]*\)").Replace(content,""));
    }

    public static bool parseVar(ref ParserObject obj, ref ParseNode node, int index, string content, out string value, bool isVar=true, int depth=4)
    {
        float fvalue = 0;
        value = "("+ content + " = not found)";

        if (float.TryParse(content, out fvalue)) { value=""+fvalue; return true; }
        else if(isMatch(@"true", content)) { value="1"; return true; }
        else if(isMatch(@"false", content)) { value="0"; return true; }
        else if(isValidCommand(ref obj, content)){ return true; }
      //else if(isMatch(@"w*", content)) { value=content; return true; }

        if(depth>0) foreach (var f in node.functions)
        {
          if(content == f.name) {
            if ((isVar && f.type == "VAR") || (!isVar && f.type == "CUSTOM"))
            {
                return parseOPS(ref obj, ref node, index, f.body, out value, isVar, depth-1);
            }
          }
        }

        // if not found -> look in parent
        if(node.parent != null) return parseOPS(ref obj, ref node.parent, index, content, out value, isVar, depth+1);

        return false;
    }

    public static bool parseFloat(ref ParserObject obj, ref ParseNode node, int index, string content, out float value, bool isVar=true, int depth=4){
      string svalue = "";
      value = 0;

      if(parseOPS(ref obj, ref node, index, content, out svalue, isVar, depth)) {
        return float.TryParse(svalue, out value);
      }

      return false;
    }

    public static bool parseBoolean(ref ParserObject obj, ref ParseNode node, int index, string content, out bool value, bool isVar=true, int depth=4){
      float fvalue = 0;
      value = false;

      if(parseFloat(ref obj, ref node, index, content, out fvalue, isVar, depth)){
        value = fvalue != 0;
        return true;
      }

      return false;
    }

    public static bool parseArguments(ref ParserObject obj, ref ParseNode node, int index, string content, out List<string> values, int depth=4)  {
      var list = matchAll(@"[^,]+", content);
      values = new List<string>();

      foreach (var m in list)
      {
          var name = m.Groups[0].Value;
          //float fvalue = 0;
          string svalue = "";

          if(parseOPS(ref obj, ref node, index, name, out svalue, true, depth)) {
            //content = content.Replace(name,fvalue);
            values.Add(svalue);
          } else {
            return false;
          }
      }

      return true;
    }

    public static bool parseOPS(ref ParserObject obj, ref ParseNode node, int index, string content, out string result, bool isVar=true, int depth=4)  {
      var list = matchAll(@"(\w+)[ \t]*([*/+-])[ \t]*(\w+)", content);

      result = "(not found)";

      if (list.Count > 0 && list[0].Groups.Count > 3)
      {
          var fndex = list[0].Index;
          var body = list[0].Groups;

          string s_op = body[2].Value;
          string s_left = body[1].Value;
          string s_right = body[3].Value;

          bool isLeftNumeric = parseVar(ref obj, ref node, index, s_left, out s_left, isVar, depth);
          bool isRightNumeric = parseVar(ref obj, ref node, index, s_right, out s_right, isVar, depth);

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
              else if (s_op == "==") { result += left == right; }
              else if (s_op == "/") {
                result += left / right;
                if(right == 0) obj.AddError( "Error: Cannot divide through zero", "'" + s_op + "' for (" + s_left + "," + s_right + ")", index);
              }
              else {
                  result = "(unknown " + s_op + ")";
                  obj.AddError( "Error: Invalid Operator", "'" + s_op + "' for (" + s_left + "," + s_right + ")", index);
                  return false;
              }
          } else
          {
              result = "(invalid Value)";
              obj.AddError( "Error: Invalid Value", "'" + s_op + "' for (" + s_left + "," + s_right + ")", index);
              return false;
          }
      }
      else
      {
          parseVar(ref obj, ref node, index, content, out result, isVar, depth);
      }

      return true;
    }

    public static void cleanCode(ref string code) {
        //p.replaceParsedCode("\r"," ");
        //p.replaceParsedCode("\t"," ");

        //c = c.Replace("\r",""); // remove windows '\r' character
        //c = c.Replace(";", "\n"); // replace ',' with linebreak

        // remove spaces and invalid signs
        //code = code.Replace("[^A-Za-z0-9()\"\n\t \\/]",""); // remove invalid signs

        // remove comments
        //c = new Regex(@"(?<comment>/\*(?s)((?:(?!\*/).)*)\*/)").Replace(c, ""); // remove block comment
        //c = new Regex(@"//[^\n]*").Replace(c, ""); // remove line comment

        removeCodeByRegex(ref code, @"(?<comment>/\*(?s)((?:(?!\*/).)*)\*/)");
        removeCodeByRegex(ref code, @"//[^\n]*");
    }

    public static List<string> parse(string sourceCode, List<string> validCommands){
        var removeFunctionSyntax = true;

        var obj = new ParserObject(sourceCode);
        obj.validCommands = validCommands;
        var code = obj.sourceCode;

        cleanCode(ref code);

        List<string> list = new List<string>();
        string result = "";
        string[] results = parsePart(ref obj, ref obj.root, code, 0, 0);
        string parsedCode = obj.getParsedCode();

        string find = results[0];
        string rest = results[1];
        string memo = results[2];
        memo = memo.Trim();
        memo = memo.Replace("\r","");
        memo = regexReplace(memo, @"[ ]+", " ");
        memo = regexReplace(memo, @"\t[ ]", "\t");
        memo = regexReplace(memo, @"\t+", "\t");
        memo = regexReplace(memo, @"\n[ \t]", "\n");
        memo = regexReplace(memo, @"\n+", "\n");

        string cmds = "";
        obj.commands.Sort((x,y) => x.index.CompareTo(y.index));
        foreach(var cmd in obj.commands){
          cmds += cmd.code+"\n";
          list.Add(cmd.code); //.ToLower().Replace("(","").Replace(")","")
        }

        int row = 0;
        int col = 0;
        bool skipUntilBreak = false;

        foreach (string line in rest.Split('\n')) {
          row+=1;
          col = 0;
          foreach (char c in line.ToCharArray()) {
            col+=1;
            if(!skipUntilBreak && c != ' ' && c != '\n') {
              obj.AddError("Invalid Code", line, row, col);
              skipUntilBreak = true;
            } else if(c == '\n' && skipUntilBreak) skipUntilBreak = false;
          }
        }

        string errors = string.Join("\n", obj.errors.ToArray());

        result += sourceCode;
        result += "____________________________________________________\n";
        result += "[FOUND]\n";
        result += "==========================\n";
        result += find;
        result += "__________________________\n";
        result += "[REST]\n";
        result += "==========================\n";
        result += rest;
        result += "[MEMORY]\n";
        result += "==========================\n";
        result += memo + "\n";
        result += "____________________________________________________\n";
        result += "[ERRORS]\n";
        result += errors + "\n";
        result += "____________________________________________________\n";
        result += "[RESULT]\n";
        result += cmds;

        //File.WriteAllText("parsed.txt", toString(commands));
        File.WriteAllText("parsed.txt", result);

        // syntax for valid commands, regular expression replace
        /*
        List<Regex> sregex = new List<Regex>();
        foreach (string vcmd in p.validCommands) {
            //.Replace("#",@"\w+") @"(((?<=([\'\""]))?.*?(?=\1))|(\w+))" @"[^,)]+"
            string v = removeFunctionSyntax ? vcmd.Replace("(","").Replace(")","") : vcmd.Replace("(","\\(").Replace(")","\\)").Replace("#", @"[^,)]+");
            Regex r = new Regex(v);
            sregex.Add(r);
        }*/

        /*
        cmds = new Regex(@"(?<var>var[\t ](\w+))[ \t]*\=[ \t]*([^\n]+)\n)").Replace(cmds, ""); // remove vars

        var count = countLines(sourceCode,cmds);
        // find invalid code lines
        var line = -1;
        foreach (string cmd in cmds.Split('\n')) {
            ++line;
            var c = removeFunctionSyntax ? cmd.Replace("(","").Replace(")","") : cmd;
            var err = true;
            //if(!p.validCommands.Contains(c.)) {
            foreach (Regex r in sregex) {
                if(r.Match(c).Success) {
                  if(r.Replace(c, "").Length<=0) {
                    err = false;
                    list.Add(c); // passed
                  }
                  break;
                }
            }
            if(!err) continue;
            errors.Add("Invalid Code \"" + c + "\" on Line " + (line + count));
        }*/

        // throw parser exception when invalid code lines are found
        if (obj.errors.Count > 0) {
            //List<string> errors = new List<string>();
            //foreach (string s in error.Split('\n')) { errors.Add(s); }
            throw new ParserException(obj.errors);
        }

        //foreach (string entry in list) { Debug.Log(entry); }

        return list;
    }

    public static int countLines(string before, string after) {
        int bcount = 0;
        int acount = 0;

        foreach (char c in before) if (c == '\n') bcount++;
        foreach (char c in after) if (c == '\n') acount++;

        return Math.Abs(acount - bcount);
    }

    const int LINE = 0;
    const int HEAD = 1;
    const int BODY = 2;
    const int HEAD_1 = HEAD + 1;
    const int BODY_1 = BODY + 1;

    public static string Repeat(char c, int n)
    {
        return new String(c, n);
    }

    public static void Clear(ref char[] list, int index, int length)
    {
      for(int i = index; i<index + length; i++) {
        if(list[i] != '\n') list[i] = ' ';
      }
    }

    public static void replaceCodeByRegex(ref ParserObject p, string regex, string rpl) {
      p.parsedCode = new Regex(regex).Replace(p.getParsedCode(), rpl).ToCharArray();
    }

    public static void removeCodeByRegex(ref string code, string regex) {
      var rxlist = matchAll(regex, code);
      char[] list = code.ToCharArray();

      foreach (var m in rxlist) {
        var line = m.Groups[0].Value;
        Clear(ref list, m.Index, line.Length);
      }

      code = new String(list);
    }

    public static void removeCode(ref string code, Function f) {
      removeCode(ref code, f.match.Index, f.line.Length);
    }

    public static void removeCode(ref string code, int index, int length) {
      char[] list = code.ToCharArray();
      Clear(ref list, index, length);
      code = new String(list);
    }


    public static Function findCustomFunction(ref ParseNode node, string name) {
        if(node == null) return null;

        foreach (var f in node.functions)
        {
            if (f.type != "CUSTOM") continue;
            if (f.name == name) return f;
        }

        return findCustomFunction(ref node.parent, name);
    }

    // FEHLER: LEERZEICHEN, KOMMENTARE, ZEILE, LOOP OHNE BREAK FEHLER
    public static string[] parsePart(ref ParserObject obj, ref ParseNode parent, string code, int parent_index, int depth)
    {
        var node = new ParseNode(ref parent);

        string found = "";
        string errors = "";
        string memo = "";

        // FIND FUNCTIONS
        if (depth == 0)
        {
            //@"function[ \t]+(\w+)\((([^(],)?([^)]+))*\)[ \t]*[\n;](?s)(.*)endFunction[ \t]*[\n;]"
            //functions = matchAll(@"(?<function>function[ \t]+(\w+)\(([^)]*)\)[ \t]*[\n;](?s)((.*)?)endFunction[ \t]*([\n;]|$))", code);
            var list = matchAll(@"(?<function>function[ \t]+(\w+)\(([^)]*)\)[ \t]*(?s)((?:(?!endFunction).)*)endFunction( |\t|\n|$))", code);

            if (list.Count > 0) found += "Functions(" + list.Count + ")\n";
            foreach (var m in list)
            {
                var f = new Function("CUSTOM", m, 1, 2, 3);
                f.setParentIndex(parent_index);

                node.functions.Add(f);
                found += "\t" + f.ToString() + "\n";

                removeCode(ref code, f);
            }
        }

        memo = code; // remove custom definitions of custom functions

        // FIND IF ELSEIF ELSE
        {
            var list = matchAll(@"(?<if>if[ \t]*\(([^)]*)\)[ \t]*\n(?s)((?:(?!((endif\n)|(elseif\()|(else\n))).)*)"+
            @"((elseif[ \t]*\(([^)]*)\)[ \t]*\n(?s)((?:(?!((endif\n)|(elseif\()|(else\n))).)*))*)"+
            @"(else[ \t]*\n(?s)((?:(?!endif).)*))?"+
            @"endif[ \t]*(\n|$))",code);

            const int ELSEIF = 7;
            const int ELSE = 15;
            const int ELSE_BODY = ELSE + 1;

            if (list.Count > 0) found += "if(" + list.Count + ")\n";
            int i = -1;
            foreach (var m in list)
            {
                int index = m.Index;
                var e =  m.Groups;
                ++i;
                var f = new Function("IF", m, "if" + i, HEAD, BODY);
                f.setParentIndex(parent_index);

                node.functions.Add(f);
                found += "\t" + f.ToString() + "\n";

                if(e[ELSEIF].Value != ""){
                  var elseif_list = matchAll(@"(?<elseif>elseif[ \t]*\(([^)]*)\)[ \t]*\n(?s)((?:(?!((endif\n)|(elseif\()|(else\n))).)*)"+
                  @"[ \t]*(\n|$))",e[ELSEIF].Value);

                  int j = -1;
                  foreach (var t in elseif_list) {
                    ++j;
                    var d = t.Groups;
                    var f2 = new Function("IF", t, "elseif" + i+"_"+j, HEAD, BODY);
                    f2.setParentIndex(parent_index);

                    node.functions.Add(f2);
                    found += "\t" + f2.ToString() + "\n";
                  }
                }

                if(e[ELSE].Value != ""){ // found_else
                  var f3 = new Function("IF", m, "else" + i, "", ELSE_BODY);
                  f3.setParentIndex(parent_index);

                  node.functions.Add(f3);
                  found += "\t" + f3.ToCompleteString() + "\n";
                }

                // found += "\tif:\n";
                // found += "----------------------\n";
                // var k = -1;
                // foreach (var l in e){
                //   k++;
                //   if(k==0) continue;
                //   found += "\t"+k+":\t" + l + "\n";
                // }
                // found += "----------------------\n";

                removeCode(ref code, f);
            }
        }

        // FIND LOOPS
        {
            var list = matchAll(@"(?<loop>loop[ \t]*\(([^)]*)\)[ \t]*\n(?s)((?:(?!endLoop).)*)endLoop( |\t|\n|$))", code);

            if(list.Count>0) found += "Loops(" + list.Count + ")\n";
            int i = -1;
            foreach (var m in list)
            {
                ++i;
                var f = new Function("LOOP", m, "loop" + i, 1, 2);
                f.setParentIndex(parent_index);

                node.functions.Add(f);

                found += "\t" + f.ToString() + "\n";

                removeCode(ref code, f);
            }
        }

        // FIND VARS
        {
            var list = matchAll(@"(?<var>(\w+)[ \t]*\=[ \t]*([^\n]*)?[ \t]*(\n|$))", code);

            if (list.Count > 0) found += "Vars(" + list.Count + ")\n";
            foreach (var m in list)
            {
                var f = new Function("VAR", m, 1, "", 2);
                f.setParentIndex(parent_index);

                node.functions.Add(f);

                found += "\t" + f.ToString() + "\n";

                removeCode(ref code, f);
            }
        }

        // FIND CALL
        {
            //var calls = matchAll(@"(\w+)\((([^(],)?([^)]+))*\)[ \t]*([\n;]|$)", code);
            var list = matchAll(@"(?<call>(\w+)\(([^\n]*)?\)[ \t]*(\n|$))", code);

            if (list.Count > 0) found += "Calls(" + list.Count + ")\n";
            foreach (var m in list)
            {
                var f = new Function("CALL", m, 1, 2, "");
                f.setParentIndex(parent_index);

                node.functions.Add(f);

                found += "\t" + f.ToString() + "\n";

                removeCode(ref code, f);
            }
        }

        // READ heads and bodies
        {
            // parse all vars
            int findex = -1;
            foreach (var f in node.functions)
            {
                findex++;
                if (f.type == "VAR")
                {
                    string result = "";
                    parseOPS(ref obj, ref node, parent_index + f.bodyIndex, f.body, out result);
                    updateBody(ref node, findex, result);
                    found = found.Replace(f.line, f.name + " = " + result);
                    memo = memo.Replace(f.line, f.name + " = " + result);
                }
            }

            foreach (var f in node.functions)
            {
                if (f.type == "CALL")
                {
                    //parseArguments(ref ParserObject obj, ref ParseNode node, int index, string content, out List<string> values, int depth=4)

                    bool valid = false;
                    int count = 0;

                    var f2 = findCustomFunction(ref node, f.name);

                    if (f2 != null)
                    {
                        string[] parsedParts = parsePart(ref obj, ref node, f2.body, parent_index + f2.bodyIndex, depth + 1);
                        string foundline = "(" + f.name + "):\n";
                        foundline += parsedParts[0];
                        foundline += "--------------------------\n";
                        foundline += parsedParts[1];
                        foundline += "__________________________\n";
                        found = found.Replace(f.ToString(), foundline);

                        //obj.AddCommand(f.index,f.line); // found and valid
                        memo = memo.Replace(f.line, parsedParts[2]);
                        valid = true;
                        break;
                    }

                    if(isValidCommand(ref obj, f.line)) {
                      obj.AddCommand(f.line,f.index); // found and valid
                      valid = true;
                    }

                    if(!valid) obj.AddError("Invalid Call"+count, f.line, f.index);
                    //errors += f.type + " : cannot parse loop on index " + f.index;
                }
            }

            foreach (var f in node.functions)
            {
                if (f.type == "LOOP")
                {
                    var list = matchAll(@"i[ \t]*=[ \t]*(\w+)\:(\w+)", f.head);

                    if (list.Count>0 && list[0].Groups.Count>2) {
                        var head = list[0].Groups;

                        string s_start = head[1].Value;
                        string s_end = head[2].Value;

                        bool isStartNumeric = parseOPS(ref obj, ref node, parent_index + f.index, s_start, out s_start);
                        bool isEndNumeric = parseOPS(ref obj, ref node, parent_index + f.index, s_end, out s_end);

                        if (isStartNumeric && isEndNumeric)
                        {
                            int start = 0;
                            int end = 0;

                            Int32.TryParse(s_start, out start);
                            Int32.TryParse(s_end, out end);

                            string[] parsedParts = parsePart(ref obj, ref node, f.body, parent_index + f.bodyIndex, depth + 1);

                            bool p1 = parsedParts[0].Length > 0;
                            bool p2 = parsedParts[1].Length > 0;
                            bool p3 = parsedParts[2].Length > 0;

                            string repeat1 = "";
                            string repeat2 = "";
                            string repeat3 = "";

                            if (start > 0 && end > 0) for (int index = start; index <= end; index++)
                            {
                                if (p1) repeat1 += parsedParts[0] + "\n";
                                if (p2) repeat2 += parsedParts[1] + "\n";
                                if (p3) repeat3 += parsedParts[2] + "\n";
                            }

                            string foundline = "(LOOP " + start + " -> " + end + "):\n";
                            foundline += repeat1;
                            foundline += "--------------------------\n";
                            foundline += repeat2;
                            foundline += "--------------------------\n";
                            foundline += repeat3;
                            foundline += "__________________________\n";

                            string result = repeat3;
                            updateBody(ref node, findex, result);
                            found = found.Replace(f.ToString(), foundline);

                            //obj.AddCommand(f.index,result); // found and valid
                            memo = memo.Replace(f.line, repeat3);
                        }
                        else
                        {
                            obj.AddError("Invalid Loop", "i = "+s_start+" : "+s_end, f.index);
                            //errors += f.type + " : cannot parse loop on Index " + head.match.Index;

                            // error cant parse line ...
                            // var cannot be found or is invalid
                        }
                    }
                }
            }

            bool skipIF = false;

            foreach (var f in node.functions)
            {
                if(f.type == "IF")
                {
                    var is_if = isMatch(@"^if[0-9]+.*", f.name);
                    var is_elseif = !is_if && isMatch(@"^elseif[0-9]+.*", f.name);
                    var is_else = !is_elseif && isMatch(@"^else[0-9]+.*", f.name);

                    if(is_if) skipIF = false;

                    if(!skipIF) {
                      if(is_else) skipIF = true;
                      else parseBoolean(ref obj, ref node, parent_index + f.index, f.head, out skipIF);

                      if(skipIF) {
                        string[] parsedParts = parsePart(ref obj, ref node, f.body, parent_index + f.bodyIndex, depth + 1);
                        string foundline = f.name + "(" + f.head + " == true):\n";
                        foundline += parsedParts[0];
                        foundline += "--------------------------\n";
                        foundline += parsedParts[1];
                        foundline += "__________________________\n";
                        found = found.Replace(f.ToString(), foundline);

                        //obj.AddCommand(f.index,parsedParts[2]); // found and valid
                        memo = memo.Replace(f.line, parsedParts[2]);
                      }
                    }
                }
            }
        }

        //cmds = new Regex(@"\n{2,}").Replace(cmds, "\n");
        var chars = matchAll(@"[^ \t\n]", code);
        if (chars.Count==0) { code = ""; }

        return new string[]{found,code,memo};
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
