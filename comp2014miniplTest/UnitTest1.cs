using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2014minipl;

namespace comp2014miniplTest
{
    [TestClass]
    public class NFATests
    {
        [TestMethod]
        public void constructorWorks()
        {
            NFA test = new NFA("hello");
            Assert.IsTrue(test.recognizes("hello"));
            Assert.IsFalse(test.recognizes("hell"));
            Assert.IsFalse(test.recognizes("helloo"));
        }
        [TestMethod]
        public void orWorks()
        {
            NFA test = new NFA("hello");
            NFA test2 = new NFA("lul");
            NFA test3 = test.or(test2);
            Assert.IsTrue(test3.recognizes("hello"), "Recognizes first alternative");
            Assert.IsTrue(test3.recognizes("lul"), "Recognizes second alternative");
            Assert.IsFalse(test3.recognizes("abc"), "Doesn't recognize random stuff");
        }
        [TestMethod]
        public void closureWorks()
        {
            NFA test = new NFA("hello").closure();
            Assert.IsTrue(test.recognizes(""), "recognizes 0 elements");
            Assert.IsTrue(test.recognizes("hello"), "recognizes 1 element");
            Assert.IsTrue(test.recognizes("hellohello"), "recognizes 2 elements");
            Assert.IsTrue(test.recognizes("hellohellohellohellohello"), "recognizes 5 elements");
            Assert.IsFalse(test.recognizes("abc"), "doesn't recognize random stuff");
        }
        [TestMethod]
        public void closureWithOrWorks()
        {
            NFA a = new NFA("a");
            NFA b = new NFA("b");
            NFA test = a.or(b).closure();
            Assert.IsTrue(test.recognizes("ababaabbbabaaabababba"));
        }
        [TestMethod]
        public void concWorks()
        {
            NFA test = new NFA("hello");
            NFA test2 = new NFA("lul");
            NFA test3 = test.conc(test2);
            Assert.IsTrue(test3.recognizes("hellolul"), "Recognizes correct concatenation");
            Assert.IsFalse(test3.recognizes("lulhello"), "Doesn't recognize wrongly ordered concatenation");
            Assert.IsFalse(test3.recognizes("lul"), "Doesn't recognize original second part");
            Assert.IsFalse(test3.recognizes("hello"), "Doesn't recognize original first part");
        }
        [TestMethod]
        public void concClosureWorks()
        {
            NFA hello = new NFA("hello");
            NFA lul = new NFA("lul");
            NFA test = lul.conc(hello.closure());
            Assert.IsTrue(test.recognizes("lul"));
            Assert.IsTrue(test.recognizes("lulhello"));
            Assert.IsTrue(test.recognizes("lulhellohellohello"));
            Assert.IsFalse(test.recognizes("abc"));
        }
        [TestMethod]
        public void complexConcOrClosureWorks()
        {
            NFA hello = new NFA("hello");
            NFA lul = new NFA("lul");
            NFA test = hello.conc(lul.closure().or(hello)).conc(hello.closure());
            Assert.IsTrue(test.recognizes("hellohellohello"));
            Assert.IsTrue(test.recognizes("hellolullullul"));
            Assert.IsTrue(test.recognizes("hellolullullulhellohello"));
            Assert.IsTrue(test.recognizes("hellohello"));
            Assert.IsTrue(test.recognizes("hello"));
            Assert.IsFalse(test.recognizes("abc"));
        }
    }
}
