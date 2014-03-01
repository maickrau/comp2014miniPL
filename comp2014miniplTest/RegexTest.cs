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
        [TestMethod]
        public void specialsWork()
        {
            Regex reg = new Regex("\\(\\)\\\\\\[\\]\\*\\?\\.");
            Assert.AreEqual(8, reg.recognize("()\\[]*?."));
        }
        [TestMethod]
        public void maybeWorks()
        {
            Regex reg = new Regex("abc?d");
            Assert.AreEqual(4, reg.recognize("abcd"));
            Assert.AreEqual(3, reg.recognize("abd"));
            Assert.AreEqual(0, reg.recognize("abccd"));
        }
        [TestMethod]
        public void exclusionWorks()
        {
            Regex reg = new Regex("ab[^cd]e");
            Assert.AreEqual(4, reg.recognize("abhe"));
            Assert.AreEqual(4, reg.recognize("abee"));
            Assert.AreEqual(4, reg.recognize("abae"));
            Assert.AreEqual(0, reg.recognize("abce"));
            Assert.AreEqual(0, reg.recognize("abde"));
            Assert.AreEqual(0, reg.recognize("abe"));
        }
        [TestMethod]
        public void rangeWorks()
        {
            Regex reg = new Regex("[b-e]");
            Assert.AreEqual(0, reg.recognize("a"));
            Assert.AreEqual(1, reg.recognize("b"));
            Assert.AreEqual(1, reg.recognize("c"));
            Assert.AreEqual(1, reg.recognize("d"));
            Assert.AreEqual(1, reg.recognize("e"));
            Assert.AreEqual(0, reg.recognize("f"));
        }
        [TestMethod]
        public void numberWorks()
        {
            Regex reg = new Regex("[1-9][0-9]*");
            Assert.AreEqual(3, reg.recognize("100"));
            Assert.AreEqual(5, reg.recognize("12345"));
            Assert.AreEqual(0, reg.recognize("0100"));
        }
        [TestMethod]
        public void anythingWorks()
        {
            Regex reg = new Regex("abc.ef");
            Assert.AreEqual(6, reg.recognize("abcdef"));
            Assert.AreEqual(6, reg.recognize("abcaef"));
            Assert.AreEqual(6, reg.recognize("abc.ef"));
        }
        [TestMethod]
        public void parenthesisAlternatesWork()
        {
            Regex reg = new Regex("a(bc|de)*f");
            Assert.AreEqual(4, reg.recognize("abcf"));
            Assert.AreEqual(4, reg.recognize("adef"));
            Assert.AreEqual(14, reg.recognize("abcbcbcdedebcf"));
            Assert.AreEqual(2, reg.recognize("af"));
            Assert.AreEqual(0, reg.recognize("abgbcbcdedebcf"));
        }
    }
}
