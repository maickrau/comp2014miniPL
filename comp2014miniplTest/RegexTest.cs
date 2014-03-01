using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2014minipl;
using System.Collections.Generic;

namespace comp2014miniplTest
{
    [TestClass]
    public class RegexTest
    {
        [TestMethod]
        public void constructorWorks()
        {
            Regex reg = new Regex("abc");
            Assert.AreEqual(3, reg.recognize("abc"));
            Assert.AreEqual(3, reg.recognize("abcd"));
            Assert.AreEqual(0, reg.recognize("def"));
        }
        [TestMethod]
        public void alternateWorks()
        {
            Regex reg = new Regex("a[bcd]e");
            Assert.AreEqual(3, reg.recognize("abe"));
            Assert.AreEqual(3, reg.recognize("ace"));
            Assert.AreEqual(3, reg.recognize("ade"));
            Assert.AreEqual(0, reg.recognize("dbe"));
            Assert.AreEqual(0, reg.recognize("aee"));
        }
        [TestMethod]
        public void closureWorks()
        {
            Regex reg = new Regex("a*");
            Assert.AreEqual(3, reg.recognize("aaa"));
            Assert.AreEqual(1, reg.recognize("a"));
            Assert.AreEqual(0, reg.recognize("bbb"));
        }
        [TestMethod]
        public void parenthesisClosureWorks()
        {
            Regex reg = new Regex("a(bc)*");
            Assert.AreEqual(1, reg.recognize("a"));
            Assert.AreEqual(3, reg.recognize("abc"));
            Assert.AreEqual(7, reg.recognize("abcbcbc"));
            Assert.AreEqual(0, reg.recognize("bc"));
        }
        [TestMethod]
        public void alternateClosureWorks()
        {
            Regex reg = new Regex("a[bc]*d");
            Assert.AreEqual(4, reg.recognize("abcd"));
            Assert.AreEqual(14, reg.recognize("acbbcbccbcbbcd"));
            Assert.AreEqual(2, reg.recognize("ad"));
        }
        [TestMethod]
        public void recognizeAllWorks()
        {
            Regex reg = new Regex("abc");
            List<Tuple<int, int>> vals = reg.recognizeAll("dabcdefabc");
            CollectionAssert.Contains(vals, new Tuple<int, int>(1, 4));
            CollectionAssert.Contains(vals, new Tuple<int, int>(7, 10));
            CollectionAssert.DoesNotContain(vals, new Tuple<int, int>(5, 7));
        }
    }
}
