using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2014minipl;
using System.Collections.Generic;

namespace comp2014miniplTest
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void ParsesSimple()
        {
            //corrected grammar from week 5 exercise 3
            Parser p = new Parser();
            NonTerminal L = new NonTerminal("L");
            NonTerminal Q = new NonTerminal("Q");
            NonTerminal R = new NonTerminal("R");
            NonTerminal Q2 = new NonTerminal("Q2");
            NonTerminal R2 = new NonTerminal("R2");
            Keyword a = new Keyword("a");
            Keyword b = new Keyword("b");
            Keyword c = new Keyword("c");
            List<Token> prod1 = new List<Token>();
            prod1.Add(R);
            prod1.Add(a);
            p.addProduction(L, prod1);
            List<Token> prod2 = new List<Token>();
            prod2.Add(Q);
            prod2.Add(b);
            prod2.Add(a);
            p.addProduction(L, prod2);
            List<Token> prod3 = new List<Token>();
            prod3.Add(b);
            prod3.Add(Q2);
            p.addProduction(Q, prod3);
            List<Token> prod4 = new List<Token>();
            prod4.Add(b);
            prod4.Add(c);
            p.addProduction(Q2, prod4);
            List<Token> prod5 = new List<Token>();
            prod5.Add(c);
            p.addProduction(Q2, prod5);
            List<Token> prod6 = new List<Token>();
            prod6.Add(a);
            prod6.Add(b);
            prod6.Add(a);
            prod6.Add(R2);
            p.addProduction(R, prod6);
            List<Token> prod7 = new List<Token>();
            prod7.Add(c);
            prod7.Add(a);
            prod7.Add(b);
            prod7.Add(a);
            prod7.Add(R2);
            p.addProduction(R, prod7);
            List<Token> prod8 = new List<Token>();
            prod8.Add(b);
            prod8.Add(c);
            prod8.Add(R2);
            p.addProduction(R2, prod8);
            List<Token> prod9 = new List<Token>();
            p.addProduction(R2, prod9);
            List<Token> parseThese = new List<Token>();
            parseThese.Add(a);
            parseThese.Add(b);
            parseThese.Add(a);
            parseThese.Add(b);
            parseThese.Add(c);
            parseThese.Add(b);
            parseThese.Add(c);
            parseThese.Add(a);
            p.setStartSymbol(L);
            p.prepareForParsing();
            SyntaxTree parsed = p.parse(parseThese);
            Assert.AreEqual(L, parsed.root.token);
            Assert.AreEqual(R, parsed.root.children[0].token);
            Assert.AreEqual(a, parsed.root.children[0].children[0].token);
            Assert.AreEqual(b, parsed.root.children[0].children[1].token);
            Assert.AreEqual(a, parsed.root.children[0].children[2].token);
            Assert.AreEqual(R2, parsed.root.children[0].children[3].token);
            Assert.AreEqual(b, parsed.root.children[0].children[3].children[0].token);
            Assert.AreEqual(c, parsed.root.children[0].children[3].children[1].token);
            Assert.AreEqual(R2, parsed.root.children[0].children[3].children[2].token);
            Assert.AreEqual(b, parsed.root.children[0].children[3].children[2].children[0].token);
            Assert.AreEqual(c, parsed.root.children[0].children[3].children[2].children[1].token);
            Assert.AreEqual(a, parsed.root.children[1].token);
        }
        [TestMethod]
        public void parsesIdentifiersAndLiteral()
        {
            Parser p = new Parser();
            Token a = new Keyword("=");
            Token b = new Keyword(";");
            Token s = new NonTerminal("S");
            List<Token> production = new List<Token>();
            production.Add(new Identifier(""));
            production.Add(a);
            production.Add(new IntLiteral("0"));
            production.Add(b);
            p.addProduction(s, production);
            p.setStartSymbol(s);
            p.prepareForParsing();
            List<Token> parseThese = new List<Token>();
            parseThese.Add(new Identifier("abc"));
            parseThese.Add(a);
            parseThese.Add(new IntLiteral("15"));
            parseThese.Add(b);
            SyntaxTree parsed = p.parse(parseThese);
            Assert.AreEqual(new Identifier("abc"), parsed.root.children[0].token);
            Assert.AreNotEqual(new Identifier(""), parsed.root.children[0].token);
            Assert.AreEqual(a, parsed.root.children[1].token);
            Assert.AreEqual(new IntLiteral("15"), parsed.root.children[2].token);
            Assert.AreNotEqual(new IntLiteral("0"), parsed.root.children[2].token);
            Assert.AreEqual(b, parsed.root.children[3].token);
        }
        [TestMethod]
        public void parsesLiterals()
        {
            Parser p = new Parser();
            Token a = new IntLiteral("0");
            Token b = new StringLiteral("");
            Token c = new BoolLiteral("");
            Token s = new NonTerminal("s");
            List<Token> production = new List<Token>();
            production.Add(a);
            production.Add(b);
            production.Add(c);
            p.addProduction(s, production);
            p.setStartSymbol(s);
            p.prepareForParsing();
            List<Token> parseThese = new List<Token>();
            parseThese.Add(new IntLiteral("-50"));
            parseThese.Add(new StringLiteral("abc"));
            parseThese.Add(new BoolLiteral("true"));
            SyntaxTree parsed = p.parse(parseThese);
            Assert.AreEqual(new IntLiteral("-50"), parsed.root.children[0].token);
            Assert.AreNotEqual(new IntLiteral("50"), parsed.root.children[0].token);
            Assert.AreEqual(new StringLiteral("abc"), parsed.root.children[1].token);
            Assert.AreNotEqual(new StringLiteral("def"), parsed.root.children[1].token);
            Assert.AreEqual(new BoolLiteral("true"), parsed.root.children[2].token);
            Assert.AreNotEqual(new BoolLiteral("false"), parsed.root.children[2].token);
        }
    }
}
