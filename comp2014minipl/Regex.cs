using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2014minipl
{
    public class Regex
    {
        public NFA nfa;
        public Regex(String str)
        {
            int loc = 0;
            nfa = parseRegex(str, ref loc);
        }
        private NFA parseRegex(String str, ref int loc)
        {
            NFA builtNFA = new NFA("");
            while (loc < str.Length)
            {
                NFA nextNFA = parseOne(str, ref loc);
                builtNFA = builtNFA.conc(nextNFA);
            }
            return builtNFA;
        }
        public int recognize(String str)
        {
            int longestRecognition = 0;
            nfa.startRecognizing();
            for (int i = 0; i < str.Length; i++)
            {
                nfa.takeCharacter(str[i]);
                int recognizes = nfa.recognizesCurrent();
                if (recognizes > -1)
                {
                    longestRecognition = recognizes;
                }
            }
            return longestRecognition;
        }
        public List<Tuple<int, int>> recognizeAll(String str)
        {
            List<Tuple<int, int>> all = new List<Tuple<int, int>>();
            nfa.startRecognizing();
            for (int i = 0; i < str.Length; i++)
            {
                nfa.takeCharacter(str[i]);
                nfa.alsoRecognizeFromHere();
                int recognizes = nfa.recognizesCurrent();
                if (recognizes > -1)
                {
                    all.Add(new Tuple<int, int>(i + 1 - recognizes, i + 1));
                }
            }
            return all;
        }
        private NFA parseParenthesis(string str, ref int loc)
        {
            //str[loc] == '('
            loc++;
            NFA builtNFA = parseOne(str, ref loc);
            while (str[loc] != ')')
            {
                if (loc >= str.Length)
                {
                    throw new Exception("Regex has a ( without a matching )");
                }
                NFA nextNFA = parseOne(str, ref loc);
                builtNFA = builtNFA.conc(nextNFA);
            }
            loc++;
            //loc is first character after )
            return builtNFA;
        }
        private NFA parseOne(String str, ref int loc)
        {
            NFA retNFA;
            if (str[loc] == '(')
            {
                retNFA = parseParenthesis(str, ref loc);
            }
            else if (str[loc] == '[')
            {
                retNFA = parseAlternates(str, ref loc);
            }
            else if (str[loc] == '\\')
            {
                retNFA = parseSpecial(str, ref loc);
            }
            else
            {
                retNFA = new NFA(str.Substring(loc, 1));
                loc++;
            }
            if (loc < str.Length)
            {
                if (str[loc] == '*')
                {
                    retNFA = retNFA.closure();
                    loc++;
                }
                else if (str[loc] == '?')
                {
                    retNFA = retNFA.maybe();
                    loc++;
                }
            }
            return retNFA;
        }
        private NFA parseAlternates(String str, ref int loc)
        {
            //str[loc] == '['
            loc++;
            NFA builtNFA = parseOne(str, ref loc);
            while (str[loc] != ']')
            {
                if (loc >= str.Length)
                {
                    throw new Exception("Regex has a [ without a matching ]");
                }
                NFA nextNFA = parseOne(str, ref loc);
                builtNFA = builtNFA.or(nextNFA);
            }
            loc++;
            //loc is first character after ]
            return builtNFA;
        }
        private NFA parseSpecial(String str, ref int loc)
        {
            //str[loc] == '\\'
            loc++;
            if (loc >= str.Length)
            {
                throw new Exception("Regex ends with a special character marker");
            }
            NFA ret;
            switch(str[loc])
            {
                case '\\':
                    ret = new NFA("\\");
                    break;
                case 'n':
                    ret = new NFA("\n");
                    break;
                case '\'':
                    ret = new NFA("\'");
                    break;
                case '\"':
                    ret = new NFA("\"");
                    break;
                case '*':
                    ret = new NFA("*");
                    break;
                case '(':
                    ret = new NFA("(");
                    break;
                case ')':
                    ret = new NFA(")");
                    break;
                case '[':
                    ret = new NFA("[");
                    break;
                case ']':
                    ret = new NFA("]");
                    break;
                case '.':
                    ret = new NFA(".");
                    break;
                case '?':
                    ret = new NFA("?");
                    break;
                case '^':
                    ret = new NFA("^");
                    break;
                case '-':
                    ret = new NFA("-");
                    break;
                default:
                    throw new Exception("Unknown escape character in regex");
            }
            loc++;
            //loc is first character after special
            return ret;
        }
    }
}
