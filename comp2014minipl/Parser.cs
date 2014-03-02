using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2014minipl
{
    public class NonTerminal : Token
    {
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is NonTerminal))
            {
                return false;
            }
            return value == ((NonTerminal)obj).value;
        }
        public NonTerminal(String name)
        {
            value = name;
        }
        public override String ToString()
        {
            return "Nonterminal:\"" + value + "\"";
        }
        String value;
    }
    public class SyntaxTree
    {
        public SyntaxNode root;
        public SyntaxTree(SyntaxNode root)
        {
            this.root = root;
        }
    }
    public class SyntaxNode
    {
        public Token token;
        public SyntaxNode parent;
        public List<SyntaxNode> children;
        public SyntaxNode(Token token)
        {
            this.token = token;
            children = new List<SyntaxNode>();
        }
    }
    public class Parser
    {
        Dictionary<Token, List<List<Token>>> productionRules;
        Dictionary<Tuple<Token, Token>, List<Token>> predict;
        HashSet<Token> languageTerminals;
        HashSet<Token> languageSymbols;
        Token startSymbol;
        Token eof;
        public void DebugPrint()
        {
            System.Console.WriteLine("Symbols:");
            foreach (Token t in languageSymbols)
            {
                System.Console.WriteLine(t);
            }
            System.Console.WriteLine("Terminals:");
            foreach (Token t in languageTerminals)
            {
                System.Console.WriteLine(t);
            }
            System.Console.WriteLine("Productions:");
            foreach (Token t in languageSymbols)
            {
                foreach (List<Token> p in productionRules[t])
                {
                    System.Console.Write("{0} =>", t);
                    foreach (Token t2 in p)
                    {
                        System.Console.Write(" {0}", t2);
                    }
                    System.Console.WriteLine();
                }
            }
            System.Console.WriteLine("Predict:");
            foreach (KeyValuePair<Tuple<Token, Token>, List<Token>> p in predict)
            {
                System.Console.Write("{0}, {1} =>", p.Key.Item1, p.Key.Item2);
                foreach (Token t in p.Value)
                {
                    System.Console.Write(" {0}", t);
                }
                System.Console.WriteLine();
            }
        }
        public Parser()
        {
            productionRules = new Dictionary<Token, List<List<Token>>>();
            languageSymbols = new HashSet<Token>();
            languageTerminals = new HashSet<Token>();
            predict = new Dictionary<Tuple<Token, Token>, List<Token>>();
            eof = new NonTerminal("eof");
            addSymbol(eof);
        }
        private void addSymbol(Token s)
        {
            languageSymbols.Add(s);
            if (!productionRules.Keys.Contains(s))
            {
                productionRules[s] = new List<List<Token>>();
            }
        }
        public void addProduction(Token token, List<Token> products)
        {
            addSymbol(token);
            foreach (Token t in products)
            {
                addSymbol(t);
            }
            productionRules[token].Add(new List<Token>(products));
        }
        public void setStartSymbol(Token symbol)
        {
            startSymbol = symbol;
        }
        private void calculateTerminals()
        {
            languageTerminals = new HashSet<Token>();
            foreach (Token t in languageSymbols)
            {
                if (productionRules[t].Count == 0)
                {
                    languageTerminals.Add(t);
                }
            }
        }
        private void calculatePredict()
        {
            predict = new Dictionary<Tuple<Token, Token>, List<Token>>();
            Dictionary<Token, HashSet<Token>> first = new Dictionary<Token, HashSet<Token>>();
            Dictionary<Token, HashSet<Token>> follow = new Dictionary<Token, HashSet<Token>>();
            HashSet<Token> epsilonables = new HashSet<Token>();
            foreach (Token t in languageSymbols)
            {
                first[t] = new HashSet<Token>();
                follow[t] = new HashSet<Token>();
            }
            follow[startSymbol].Add(eof);
            foreach (Token t in languageTerminals)
            {
                first[t].Add(t);
            }
            //calculate a => something (only one derivation)
            foreach (Token t in productionRules.Keys)
            {
                foreach (List<Token> p in productionRules[t])
                {
                    if (p.Count > 0)
                    {
                        first[t].Add(p[0]);
                    }
                    else
                    {
                        epsilonables.Add(t);
                    }
                }
            }
            //calculate all a =>* epsilon
            bool added = true;
            while (added)
            {
                added = false;
                foreach (Token t in productionRules.Keys)
                {
                    if (!epsilonables.Contains(t))
                    {
                        foreach (List<Token> p in productionRules[t])
                        {
                            bool thisEpsilons = true;
                            foreach (Token t2 in p)
                            {
                                if (!epsilonables.Contains(t2))
                                {
                                    thisEpsilons = false;
                                    break;
                                }
                            }
                            if (thisEpsilons)
                            {
                                epsilonables.Add(t);
                                added = true;
                            }
                        }
                    }
                }
            }
            //calculate first and follow
            added = true;
            while (added)
            {
                added = false;
                Dictionary<Token, int> oldFirstSize = new Dictionary<Token, int>();
                Dictionary<Token, int> oldFollowSize = new Dictionary<Token, int>();
                foreach (Token t in languageSymbols)
                {
                    oldFirstSize[t] = first[t].Count;
                    oldFollowSize[t] = follow[t].Count;
                }
                foreach (Token t in languageSymbols)
                {
                    HashSet<Token> newSymbols = new HashSet<Token>();
                    foreach (Token t2 in first[t])
                    {
                        newSymbols.UnionWith(first[t2]);
                    }
                    first[t].UnionWith(newSymbols);
                    foreach (List<Token> p in productionRules[t])
                    {
                        for (int i = 0; i < p.Count-1; i++)
                        {
                            follow[p[i]].UnionWith(first[p[i + 1]]);
                        }
                        for (int i = p.Count - 1; i > 0; i--)
                        {
                            if (!epsilonables.Contains(p[i]))
                            {
                                break;
                            }
                            follow[p[i-1]].UnionWith(follow[t]);
                        }
                        if (p.Count > 0)
                        {
                            follow[p[p.Count - 1]].UnionWith(follow[t]);
                        }
                    }
                }
                foreach (Token t in languageSymbols)
                {
                    if (first[t].Count != oldFirstSize[t])
                    {
                        added = true;
                        break;
                    }
                    if (follow[t].Count != oldFollowSize[t])
                    {
                        added = true;
                        break;
                    }
                }
            }
            //remove nonterminals from first, follow
            foreach (Token t in languageSymbols)
            {
                HashSet<Token> newFirst = new HashSet<Token>();
                foreach (Token f in first[t])
                {
                    if (languageTerminals.Contains(f))
                    {
                        newFirst.Add(f);
                    }
                }
                first[t] = newFirst;
                HashSet<Token> newFollow = new HashSet<Token>();
                foreach (Token f in follow[t])
                {
                    if (languageTerminals.Contains(f))
                    {
                        newFollow.Add(f);
                    }
                }
                follow[t] = newFollow;
            }
            //calculate predict
            foreach (Token t in languageSymbols)
            {
                foreach (List<Token> p in productionRules[t])
                {
                    bool allEpsilons = true;
                    for (int i = 0; i < p.Count; i++)
                    {
                        foreach (Token t2 in first[p[i]])
                        {
                            Tuple<Token, Token> key = new Tuple<Token, Token>(t, t2);
                            if (predict.ContainsKey(key) && predict[key] != p)
                            {
                                String old = "";
                                foreach (Token s in predict[key])
                                {
                                    old += s + " ";
                                }
                                String newS = "";
                                foreach (Token s in p)
                                {
                                    newS += s + " ";
                                }
                                throw new Exception("Parser: Grammar is not LL-1, predict(" + t + "," + t2 + ") happens at least twice (" + old + " and " + newS + ")");
                            }
                            predict[key] = p;
                            if (!epsilonables.Contains(t2))
                            {
                                allEpsilons = false;
                                i = p.Count;
                            }
                        }
                    }
                    if (allEpsilons)
                    {
                        foreach (Token f in follow[t])
                        {
                            Tuple<Token, Token> key = new Tuple<Token, Token>(t, f);
                            if (predict.ContainsKey(key) && predict[key] != p)
                            {
                                String old = "";
                                foreach (Token s in predict[key])
                                {
                                    old += s + " ";
                                }
                                String newS = "";
                                foreach (Token s in p)
                                {
                                    newS += s + " ";
                                }
                                throw new Exception("Parser: Grammar is not LL-1, predict(" + t + "," + f + ") happens at least twice (" + old + " and " + newS + ")");
                            }
                            predict[key] = p;
                        }
                    }
                }
            }
        }
        public void prepareForParsing()
        {
            calculateTerminals();
            calculatePredict();
        }
        public void parse(SyntaxNode currentNode, List<Token> tokens, ref int loc)
        {
            if (languageTerminals.Contains(currentNode.token))
            {
                loc++;
                return;
            }
            Token currentToken;
            if (loc < tokens.Count)
            {
                currentToken = tokens[loc];
            }
            else
            {
                currentToken = eof;
            }
            if (!predict.ContainsKey(new Tuple<Token, Token>(currentNode.token, currentToken)))
            {
                String expectedSymbols = "";
                foreach (Tuple<Token, Token> t in predict.Keys)
                {
                    if (t.Item1 == currentNode.token)
                    {
                        expectedSymbols += t.Item2 + " ";
                    }
                }
                throw new Exception("Parser: Can't predict " + currentToken + " from " + currentNode.token + ", expected one of: " + expectedSymbols);
            }
            List<Token> production = predict[new Tuple<Token, Token>(currentNode.token, currentToken)];
            foreach (Token t in production)
            {
                SyntaxNode child = new SyntaxNode(t);
                child.parent = currentNode;
                currentNode.children.Add(child);
                parse(child, tokens, ref loc);
            }
        }
        public SyntaxTree parse(List<Token> tokens)
        {
            if (startSymbol == null)
            {
                throw new Exception("Parser: Start symbol is not set");
            }
            SyntaxNode root = new SyntaxNode(startSymbol);
            int loc = 0;
            parse(root, tokens, ref loc);
            return new SyntaxTree(root);
        }
    }
}
