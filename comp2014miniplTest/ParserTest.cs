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
            StringLiteral a = new StringLiteral("\"a\"");
            StringLiteral b = new StringLiteral("\"b\"");
            StringLiteral c = new StringLiteral("\"c\"");
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
    }
}
