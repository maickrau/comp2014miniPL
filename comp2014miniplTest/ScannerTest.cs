﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2014minipl;
using System.Collections.Generic;

namespace comp2014miniplTest
{
    [TestClass]
    public class ScannerTest
    {
        [TestMethod]
        public void scansOneLine()
        {
            Scanner s = new Scanner();
            s.addKeyword("for");
            s.addKeyword("in");
            s.addKeyword("to");
            List<Token> result = s.parse("for i in 1 to 10");
            Assert.AreEqual(new Keyword("for"), result[0]);
            Assert.AreEqual(new Whitespace(""), result[1]);
            Assert.AreEqual(new Identifier("i"), result[2]);
            Assert.AreEqual(new Whitespace(""), result[3]);
            Assert.AreEqual(new Keyword("in"), result[4]);
            Assert.AreEqual(new Whitespace(""), result[5]);
            Assert.AreEqual(new IntLiteral("1"), result[6]);
            Assert.AreEqual(new Whitespace(""), result[7]);
            Assert.AreEqual(new Keyword("to"), result[8]);
            Assert.AreEqual(new Whitespace(""), result[9]);
            Assert.AreEqual(new IntLiteral("10"), result[10]);
        }
        [TestMethod]
        public void scansLiterals()
        {
            Scanner s = new Scanner();
            List<Token> result = s.parse("\"abc\" 1 true false -50 0");
            Assert.AreEqual(new StringLiteral("\"abc\""), result[0]);
            Assert.AreEqual("abc", ((StringLiteral)result[0]).value);
            Assert.AreEqual(new Whitespace(""), result[1]);
            Assert.AreEqual(new IntLiteral("1"), result[2]);
            Assert.AreEqual(1, ((IntLiteral)result[2]).value);
            Assert.AreEqual(new Whitespace(""), result[3]);
            Assert.AreEqual(new BoolLiteral("true"), result[4]);
            Assert.AreEqual(true, ((BoolLiteral)result[4]).value);
            Assert.AreEqual(new Whitespace(""), result[5]);
            Assert.AreEqual(new BoolLiteral("false"), result[6]);
            Assert.AreEqual(false, ((BoolLiteral)result[6]).value);
            Assert.AreEqual(new Whitespace(""), result[7]);
            Assert.AreEqual(new IntLiteral("-50"), result[8]);
            Assert.AreEqual(-50, ((IntLiteral)result[8]).value);
            Assert.AreEqual(new Whitespace(""), result[9]);
            Assert.AreEqual(new IntLiteral("0"), result[10]);
            Assert.AreEqual(0, ((IntLiteral)result[10]).value);
        }
    }
}
