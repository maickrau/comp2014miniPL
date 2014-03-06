using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2014minipl;

namespace comp2014miniplTest
{
    [TestClass]
    public class DecommenterTest
    {
        [TestMethod]
        public void decommentsMultiLine()
        {
            Decommenter d = new Decommenter();
            String s = "aa/*aa*/aa";
            Assert.AreEqual("aaaa", d.decomment(s));
        }
        [TestMethod]
        public void decommentsMultiLine2()
        {
            Decommenter d = new Decommenter();
            String s = "aa/*aa*/aa/*aa*/aa";
            Assert.AreEqual("aaaaaa", d.decomment(s));
        }
        [TestMethod]
        public void decommentsOneLine()
        {
            Decommenter d = new Decommenter();
            String s = "aa//abcde\naa";
            Assert.AreEqual("aaaa", d.decomment(s));
        }
        [TestMethod]
        public void decommentsOneLineAndMultiLine()
        {
            Decommenter d = new Decommenter();
            String s = "aa//abc/*de\naa*/aa";
            Assert.AreEqual("aaaa", d.decomment(s));
        }
        [TestMethod]
        public void decommentsOneLineAndMultiLine2()
        {
            Decommenter d = new Decommenter();
            String s = "aa/*aa//abc*/de\naa";
            Assert.AreEqual("aaaa", d.decomment(s));
        }
        [TestMethod]
        public void decommentsNested()
        {
            Decommenter d = new Decommenter();
            String s = "aa/*aa/*abc*/aa*/aa";
            Assert.AreEqual("aaaa", d.decomment(s));
        }
        [TestMethod]
        public void preservesCommentless()
        {
            Decommenter d = new Decommenter();
            String s = "ascnjsdkla snd\n\nklaa\nnjskasda\t\t\nafsjnsda   ";
            Assert.AreEqual(s, d.decomment(s));
        }
        [TestMethod]
        public void doesntCommentInsideStrings()
        {
            Decommenter d = new Decommenter();
            String s = "abc\"/*abc*/\"def";
            Assert.AreEqual("abc\"/*abc*/\"def", d.decomment(s));
        }
        [TestMethod]
        public void doesntCommentInsideStrings2()
        {
            Decommenter d = new Decommenter();
            String s = "abc\"\\\"/*abc*/\"def";
            Assert.AreEqual("abc\"\\\"/*abc*/\"def", d.decomment(s));
        }
    }
}
