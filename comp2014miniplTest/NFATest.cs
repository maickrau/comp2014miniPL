using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2014minipl;
using System.Collections.Generic;

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
        public void rangeConstructorWorks()
        {
            HashSet<char> range = new HashSet<char>();
            range.Add('a');
            range.Add('b');
            range.Add('c');
            NFA test = new NFA(range);
            Assert.IsTrue(test.recognizes("a"));
            Assert.IsTrue(test.recognizes("b"));
            Assert.IsTrue(test.recognizes("c"));
            Assert.IsFalse(test.recognizes("d"));
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
        [TestMethod]
        public void maybeWorks()
        {
            NFA maybe = new NFA("a").maybe();
            Assert.IsTrue(maybe.recognizes("a"));
            Assert.IsTrue(maybe.recognizes(""));
            Assert.IsFalse(maybe.recognizes("aa"));
        }
        [TestMethod]
        public void complementWorks()
        {
            NFA a = new NFA("a");
            NFA test = a.conc(a).conc(a.closure()).complement();
            Assert.IsTrue(test.recognizes("a"));
            Assert.IsFalse(test.recognizes("aa"));
            Assert.IsTrue(test.recognizes("b"));
        }
        [TestMethod]
        public void orComplementWorks()
        {
            NFA a = new NFA("a");
            NFA b = new NFA("b");
            NFA test = a.or(b).complement();
            Assert.IsTrue(test.recognizes("c"));
            Assert.IsTrue(test.recognizes("d"));
            Assert.IsTrue(test.recognizes(""));
            Assert.IsTrue(test.recognizes("sdbaas"));
            Assert.IsFalse(test.recognizes("a"));
            Assert.IsFalse(test.recognizes("b"));
        }
        [TestMethod]
        public void emptyMatchesNothing()
        {
            NFA test = new NFA("");
            Assert.IsTrue(test.recognizes(""));
            Assert.IsFalse(test.recognizes("a"));
        }
        [TestMethod]
        public void emptyComplementMatchesAnythingExceptNothing()
        {
            NFA test = new NFA("").complement();
            Assert.IsFalse(test.recognizes(""));
            Assert.IsTrue(test.recognizes("a"));
        }
        [TestMethod]
        public void orComplementExceptEmptyWorks()
        {
            NFA a = new NFA("a");
            NFA b = new NFA("b");
            NFA empty = new NFA("");
            NFA test = a.or(b).or(empty).complement();
            Assert.IsTrue(test.recognizes("d"));
            Assert.IsTrue(test.recognizes("c"));
            Assert.IsFalse(test.recognizes("a"));
            Assert.IsFalse(test.recognizes("b"));
            Assert.IsFalse(test.recognizes(""));
        }
        [TestMethod]
        public void anyOfLengthWorks()
        {
            NFA test = new NFA(2);
            Assert.IsTrue(test.recognizes("kj"));
            Assert.IsTrue(test.recognizes("ar"));
            Assert.IsTrue(test.recognizes("--"));
            Assert.IsFalse(test.recognizes(""));
            Assert.IsFalse(test.recognizes("a"));
            Assert.IsFalse(test.recognizes("agsdf"));
            Assert.IsFalse(test.recognizes("abc"));
        }
        [TestMethod]
        public void anyOfLengthComplementWorks()
        {
            NFA test = new NFA(1).complement();
            Assert.IsTrue(test.recognizes("fsaljk"));
            Assert.IsTrue(test.recognizes(""));
            Assert.IsTrue(test.recognizes("as"));
            Assert.IsFalse(test.recognizes("a"));
            Assert.IsFalse(test.recognizes("b"));
        }
    }
}
