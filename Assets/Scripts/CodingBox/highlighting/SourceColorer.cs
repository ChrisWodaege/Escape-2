using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
/// <summary>
/// A basic implementation of a pretty-printer or syntax highlighter for C# soure code.
/// </summary>
public class UnitySyntaxHighlighter
{
    // Some fairly secure token placeholders
    const string COMMENTS_TOKEN = "Â3¬Â3¬Â3¬Â3¬Â3¬";
    const string MULTILINECOMMENTS_TOKEN = "Â2¬Â2¬Â2¬Â2¬Â2¬";
    const string QUOTES_TOKEN = "Â1¬Â1¬Â1¬Â1¬Â1¬";

    private SyntaxColors _colors;
    private HashSet<string> _keywords;
    private HashSet<string> _customWords;

    public class SyntaxColors
    {
        public string CommentColor;
        public string KeywordColor;
        public string CustomWordColor;
        public string QuoteColor;
        public string TypeColor;
        public string MethodColor;
        public string NumberColor;
    }

    public UnitySyntaxHighlighter()
    {
        Initialize(new SyntaxColors()
        {
            CommentColor = "#aaaaaa",
            KeywordColor = "#CC86DF",
            CustomWordColor = "#CC86DF",
            QuoteColor = "#9CBB69",
            TypeColor = "#69B9EF",
            MethodColor = "#69B9EF",
            NumberColor = "#D4A375"
        });
    }

    public UnitySyntaxHighlighter(SyntaxColors colors, HashSet<string> keywords = null)
    {
        Initialize(colors, keywords);
    }

    private void Initialize(SyntaxColors colors, HashSet<string> keywords = null)
    {
        _colors = colors;

        _keywords = keywords != null ? keywords : new HashSet<string>()
        {
            "static", "using", "true", "false","new", "null",
            "namespace", "void", "private", "public", "protected",
            "bool", "string", "return", "class","internal", "interface",
            "const", "readonly", "int", "double","lock",
            "float", "if", "else", "while", "foreach", "for","var", "as", "is", "ref",
            "get","set","byte\\[\\]","char\\[\\]","int\\[\\]","string\\[\\]","float\\[\\]","double\\[\\]" // dumb array matching. Escaped as [] is regex syntax
        };

        _customWords = new HashSet<string>() { "Escape" };
    }

    /// <summary>
    /// Highlight the given content.
    /// </summary>
    /// <param name="content">The source code content.</param>
    /// <returns>The highlighted source code.</returns>
    public System.Collections.IEnumerator HighlightSourceCoroutine(string content, Action<string> highlightCompleteAction)
    {
        List<string> multiLineComments = new List<string>();
        List<string> quotes = new List<string>();
        List<string> comments = new List<string>();

        content = ReplaceMultilineCommentWithToken(content, ref multiLineComments);

        content = ReplaceQuotesWithToken(content, ref quotes);

        content = ReplaceCommentsWithToken(content, ref comments);

        content = HighlightSingleQuotes(content);

        content = HighlighMethods(content);

        content = HighlightNumbers(content);

        content = HighlightClasses(content);

        content = HighlightKeyWords(content, _keywords);

        content = HighlightCustomWords(content, _customWords);

        content = ReplaceTokenWithMultilineHighlighting(content, multiLineComments);

        content = replaceTokenWithQuoteHighlighting(content, quotes);

        content = ReplaceTokenWithCommentHighlighting(content, quotes, comments);

        highlightCompleteAction(content);

        yield return null;
    }

    public string HighlightNumbers(string content)
    {
        // Find numbers 10123012
        Regex regex = new Regex(@"(-| )(\d+)", RegexOptions.Singleline);
        List<string> highlightedClasses = new List<string>();
        if (regex.IsMatch(content))
        {
            foreach (Match item in regex.Matches(content))
            {
                string val = item.Value;
                if (!highlightedClasses.Contains(val))
                {
                    highlightedClasses.Add(val);
                    content = ReplaceWithCss(content, val, val, "", _colors.NumberColor);
                }
            }
        }

        return content;
    }

    private string ReplaceMultilineCommentWithToken(string content, ref List<string> multiLineComments)
    {
        // Remove /* */ quotes, taken from ostermiller.org
        Regex regex = new Regex(@"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/", RegexOptions.Singleline);
        return ReplaceTokenMatches(regex, content, ref multiLineComments, "", MULTILINECOMMENTS_TOKEN);
    }

    public string HighlighMethods(string content)
    {
        // Find upper and lower case words with '(' afterwards
        Regex regex = new Regex(@"[a-zA-Z]+\(", RegexOptions.Singleline);
        List<string> highlightedClasses = new List<string>();
        if (regex.IsMatch(content))
        {
            foreach (Match item in regex.Matches(content))
            {
                string val = item.Value;
                if (!highlightedClasses.Contains(val))
                {
                    highlightedClasses.Add(val);
                    string replaced = val.Replace("(", "");
                    content = ReplaceWithCss(content, val, replaced, "(", _colors.MethodColor);
                }
            }
        }

        return content;
    }

    public string ReplaceQuotesWithToken(string content, ref List<string> quotes)
    {
        // Remove the quotes, so they don't get highlighted
        bool onEscape = false;
        bool onComment1 = false;
        bool onComment2 = false;
        bool inQuotes = false;
        int start = -1;
        for (int i = 0; i < content.Length; i++)
        {
            if (content[i] == '/' && !inQuotes && !onComment1)
                onComment1 = true;
            else if (content[i] == '/' && !inQuotes && onComment1)
                onComment2 = true;
            else if (content[i] == '"' && !onEscape && !onComment2)
            {
                inQuotes = true; // stops cases of: var s = "// I'm a comment";
                if (start > -1)
                {
                    string quote = content.Substring(start, i - start + 1);
                    if (!quotes.Contains(quote))
                        quotes.Add(quote);
                    start = -1;
                    inQuotes = false;
                }
                else
                {
                    start = i;
                }
            }
            else if (content[i] == '\\' || content[i] == '\'')
                onEscape = true;
            else if (content[i] == '\n')
            {
                onComment1 = false;
                onComment2 = false;
            }
            else
            {
                onEscape = false;
            }
        }

        for (int i = 0; i < quotes.Count; i++)
        {
            content = ReplaceToken(content, quotes[i], QUOTES_TOKEN, i);
        }

        return content;
    }
    public string ReplaceCommentsWithToken(string content, ref List<string> comments)
    {
        // Remove the comments next, so they don't get highlighted
        Regex regex = new Regex("(/{2,3}.+)", RegexOptions.Multiline);
        return ReplaceTokenMatches(regex, content, ref comments, "\n", COMMENTS_TOKEN);
    }

    public string HighlightSingleQuotes(string content)
    {
        // Highlight single quotes
        return Regex.Replace(content, "('.{1,2}')", "<color=#ffff00>$1</color>", RegexOptions.Singleline); ;
    }

    public string HighlightClasses(string content)
    {
        // Highlight class names based on the logic: {space OR start of line OR >}{1 capital){alphanumeric}{space}
        Regex regex = new Regex(@"((?:\s|^)[A-Z]\w+(?:\s))", RegexOptions.Singleline);
        content = ReplaceMatches(regex, content, _colors.TypeColor, "");

        // Pass 2. Doing it in N passes due to my inferior regex knowledge of back/forwardtracking.
        // This does {space or [}{1 capital){alphanumeric}{]}
        regex = new Regex(@"(?:\s|\[)([A-Z]\w+)(?:\])", RegexOptions.Singleline);
        content = ReplaceMatches(regex, content, _colors.TypeColor, "");

        // Pass 3. Generics
        regex = new Regex(@"(?:\s|\[|\()([A-Z]\w+(?:<|&lt;))", RegexOptions.Singleline);
        content = ReplaceMatches(regex, content, _colors.TypeColor, "<");

        // Pass 4. new keyword with a type
        regex = new Regex(@"new\s+([A-Z]\w+)(?:\()", RegexOptions.Singleline);
        content = ReplaceMatches(regex, content, _colors.TypeColor, "");

        return content;
    }

    public string HighlightKeyWords(string content, HashSet<string> keywords)
    {
        // Highlight keywords
        foreach (string keyword in keywords)
        {
            Regex regexKeyword = new Regex("(" + keyword + @")(>|&gt;|\s|\n|;|<)", RegexOptions.Singleline);
            content = regexKeyword.Replace(content, "<color=" + _colors.KeywordColor + ">$1</color>$2");
        }
        return content;
    }

    public string HighlightCustomWords(string content, HashSet<string> customWords)
    {
        // Highlight custom words
        foreach (string customWord in customWords)
        {
            Regex regexKeyword = new Regex("(" + customWord + @")(>|&gt;|\s|\n|;|<)", RegexOptions.Singleline);
            content = regexKeyword.Replace(content, "<color=" + _colors.CustomWordColor + ">$1</color>$2");
        }
        return content;
    }

    public string ReplaceTokenWithMultilineHighlighting(string content, List<string> multiLineComments)
    {
        // Shove the multiline comments back in
        for (int i = 0; i < multiLineComments.Count; i++)
        {
            content = ReplaceTokenWithCss(content, multiLineComments[i], MULTILINECOMMENTS_TOKEN, i, _colors.CommentColor);
        }
        return content;
    }

    public string replaceTokenWithQuoteHighlighting(string content, List<string> quotes)
    {
        // Shove the quotes back in
        for (int i = 0; i < quotes.Count; i++)
        {
            content = ReplaceTokenWithCss(content, quotes[i], QUOTES_TOKEN, i, _colors.QuoteColor);
        }

        return content;
    }
    public string ReplaceTokenWithCommentHighlighting(string content, List<string> quotes, List<string> comments)
    {
        // Shove the single line comments back in
        for (int i = 0; i < comments.Count; i++)
        {
            string comment = comments[i];
            // Add comment quotes back in
            for (int n = 0; n < quotes.Count; n++)
            {
                comment = comment.Replace(string.Format("{0}{1}{0}", QUOTES_TOKEN, n), quotes[n]);
            }
            content = ReplaceTokenWithCss(content, comment, COMMENTS_TOKEN, i, _colors.CommentColor);
        }

        return content;
    }

    private string ReplaceTokenMatches(Regex regex, string content, ref List<string> listOfStrings, string suffixForContains, string token)
    {
        if (regex.IsMatch(content))
        {
            foreach (Match item in regex.Matches(content))
            {
                if (!listOfStrings.Contains(item.Value + suffixForContains))
                {
                    listOfStrings.Add(item.Value);
                }
            }
        }

        for (int i = 0; i < listOfStrings.Count; i++)
        {
            content = ReplaceToken(content, listOfStrings[i], token, i);
        }

        return content;
    }

    private string ReplaceMatches(Regex regex, string content, string color, string suffix)
    {
        List<string> highlightedClasses = new List<string>();
        if (regex.IsMatch(content))
        {
            foreach (Match item in regex.Matches(content))
            {
                string val = item.Groups[1].Value;
                if (!highlightedClasses.Contains(val))
                {
                    highlightedClasses.Add(val);
                    string replaced = val.Replace("<", "").Replace("&lt;", "");
                    content = ReplaceWithCss(content, val, replaced, suffix, color);
                }
            }
        }

        return content;
    }

    private string ReplaceWithCss(string content, string source, string replacement, string suffix, string cssClass)
    {
        return content.Replace(source, string.Format("<color={0}>{1}</color>{2}", cssClass, replacement, suffix));
    }

    private string ReplaceTokenWithCss(string content, string source, string token, int counter, string cssClass)
    {
        string formattedToken = String.Format("{0}{1}{0}", token, counter);
        return content.Replace(formattedToken, string.Format("<color={0}>{1}</color>", cssClass, source));
    }

    private string ReplaceToken(string content, string source, string token, int counter)
    {
        string formattedToken = String.Format("{0}{1}{0}", token, counter);
        return content.Replace(source, formattedToken);
    }

    public string getCodeWithoutRichText(String codeWithRichText)
    {
        //regex for <color=#******> 
        Regex openColorTagRegex = new Regex(@"((<color=#)[a-zA-Z0-9]{6,}>)", RegexOptions.Singleline);

        List<string> openTag = new List<string>();
        if (openColorTagRegex.IsMatch(codeWithRichText))
        {
            foreach (Match item in openColorTagRegex.Matches(codeWithRichText))
            {
                string val = item.Value;
                if (!openTag.Contains(val))
                {
                    openTag.Add(val);
                    codeWithRichText = codeWithRichText.Replace(val, "");
                }
            }
        }

        //regex for </color> 
        Regex closeColorTagRegex = new Regex(@"</color>", RegexOptions.Singleline);

        List<string> closeTag = new List<string>();
        if (closeColorTagRegex.IsMatch(codeWithRichText))
        {
            foreach (Match item in closeColorTagRegex.Matches(codeWithRichText))
            {
                string val = item.Value;
                if (!closeTag.Contains(val))
                {
                    closeTag.Add(val);
                    codeWithRichText = codeWithRichText.Replace(val, "");
                }
            }
        }

        return codeWithRichText;
    }
}