﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2014minipl
{
    public abstract class Token
    {
    }
    public class Operator : Token
    {
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Operator))
            {
                return false;
            }
            return value == ((Operator)obj).value;
        }
        public Operator(String str)
        {
            value = str;
        }
        public String value;
    }
    public class Whitespace : Token
    {
        public override int GetHashCode()
        {
            return "".GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Whitespace))
            {
                return false;
            }
            return true;
        }
        public Whitespace(String str)
        {
        }
    }
    public class Keyword : Token
    {
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Keyword))
            {
                return false;
            }
            return value == ((Keyword)obj).value;
        }
        public Keyword(String str)
        {
            value = str;
        }
        public override String ToString()
        {
            return "Keyword:\"" + value + "\"";
        }
        public String value;
    }
    public class IntLiteral : Token
    {
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is IntLiteral))
            {
                return false;
            }
            return value == ((IntLiteral)obj).value;
        }
        public IntLiteral(String str)
        {
            value = Int32.Parse(str);
        }
        public int value;
    }
    public class StringLiteral : Token
    {
        public override string ToString()
        {
            return "Stringliteral:\"" + value + "\"";
        }
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is StringLiteral))
            {
                return false;
            }
            return value == ((StringLiteral)obj).value;
        }
        public StringLiteral(String str)
        {
            StringBuilder build = new StringBuilder();
            for (int i = 1; i < str.Length-1; i++)
            {
                if (str[i] == '\\')
                {
                    switch(str[i+1])
                    {
                        case 'n':
                            build.Append('\n');
                            break;
                        case '"':
                            build.Append('"');
                            break;
                        case '0':
                            build.Append('\0');
                            break;
                        case '\\':
                            build.Append('\\');
                            break;
                        case 't':
                            build.Append('\t');
                            break;
                        default:
                            throw new Exception("Unknown string escape character");
                    }
                }
                else
                {
                    build.Append(str[i]);
                }
            }
            value = build.ToString();
        }
        public String value;
    }
    public class BoolLiteral : Token
    {
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is BoolLiteral))
            {
                return false;
            }
            return value == ((BoolLiteral)obj).value;
        }
        public BoolLiteral(String str)
        {
            if (str.Equals("true"))
            {
                value = true;
            }
            else
            {
                value = false;
            }
        }
        public bool value;
    }
    public class Identifier : Token
    {
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Identifier))
            {
                return false;
            }
            return value == ((Identifier)obj).value;
        }
        public Identifier(String str)
        {
            value = str;
        }
        public override string ToString()
        {
            return "Identifier:\"" + value + "\"";
        }
        public String value;
    }
    public class Scanner
    {
        List<Tuple<Type, Regex>> scannables;
        Regex identifier;
        public Scanner(String numberRegex = "(0|-?[1-9][0-9]*)", String stringRegex = "\"([^\\\\]|\\[^\"])*\"", String whitespaceRegex = "[ \n\t\r]*", String identifierRegex = "[a-zA-Z_][0-9a-zA-Z_]*")
        {
            //just in case the syntax of a number changes tomorrow
            identifier = new Regex(identifierRegex);
            scannables = new List<Tuple<Type, Regex>>();
            Tuple<Type, Regex> whitespace = new Tuple<Type, Regex>(typeof(Whitespace), new Regex(whitespaceRegex));
            Tuple<Type, Regex> integer = new Tuple<Type, Regex>(typeof(IntLiteral), new Regex(numberRegex));
            Tuple<Type, Regex> stringg = new Tuple<Type, Regex>(typeof(StringLiteral), new Regex(stringRegex));
            Tuple<Type, Regex> truee = new Tuple<Type, Regex>(typeof(BoolLiteral), new Regex("true"));
            Tuple<Type, Regex> falsee = new Tuple<Type, Regex>(typeof(BoolLiteral), new Regex("false"));
            scannables.Add(whitespace);
            scannables.Add(integer);
            scannables.Add(stringg);
            scannables.Add(truee);
            scannables.Add(falsee);
        }
        public void addKeyword(String keywordRegex)
        {
            scannables.Add(new Tuple<Type, Regex>(typeof(Keyword), new Regex(keywordRegex)));
        }
        public void addOperator(String op)
        {
            scannables.Add(new Tuple<Type, Regex>(typeof(Operator), new Regex(op)));
        }
        public List<Token> parse(String text, bool outputWhitespace = false)
        {
            List<Token> ret = new List<Token>();
            int loc = 0;
            while (loc < text.Length)
            {
                Tuple<Type, int> longestMunch = new Tuple<Type, int>(null, -1);
                int longestIdentifier = identifier.recognize(text.Substring(loc));
                foreach (Tuple<Type, Regex> t in scannables)
                {
                    int length = t.Item2.recognize(text.Substring(loc));
                    if (length > longestMunch.Item2)
                    {
                        longestMunch = new Tuple<Type, int>(t.Item1, length);
                    }
                }
                int lengthMunched;
                if (longestIdentifier > longestMunch.Item2)
                {
                    lengthMunched = longestIdentifier;
                    ret.Add(new Identifier(text.Substring(loc, longestIdentifier)));
                }
                else
                {
                    lengthMunched = longestMunch.Item2;
                    if (longestMunch.Item1 != typeof(Whitespace) || outputWhitespace)
                    {
                        ret.Add((Token)System.Activator.CreateInstance(longestMunch.Item1, text.Substring(loc, longestMunch.Item2)));
                    }
                }
                if (lengthMunched == 0)
                {
                    throw new Exception("Scanner: Couldn't munch any more input at location " + loc);
                }
                loc += lengthMunched;
            }
            return ret;
        }
    }
}
