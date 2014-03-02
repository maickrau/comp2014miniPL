using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2014minipl;

namespace comp2014miniplTest
{
    [TestClass]
    public class MiniPLGrammarTest
    {
        [TestMethod]
        public void TestSimple()
        {
            String program = "     var X : int := 4 + (6 * 2);\n     print X;";
            MiniPLGrammar g = new MiniPLGrammar();
            SyntaxTree got = g.parse(program);
            Assert.AreEqual(g.t["prog"], got.root.token);
            Assert.AreEqual(g.t["stmt"], got.root.children[0].token);
            Assert.AreEqual(g.t["var"], got.root.children[0].children[0].token);
            Assert.AreEqual(new Identifier("X"), got.root.children[0].children[1].token);
            Assert.AreEqual(g.t[":"], got.root.children[0].children[2].token);
            Assert.AreEqual(g.t["type"], got.root.children[0].children[3].token);
            Assert.AreEqual(g.t["int"], got.root.children[0].children[3].children[0].token);
            Assert.AreEqual(g.t["maybeAssign"], got.root.children[0].children[4].token);
            Assert.AreEqual(g.t[":="], got.root.children[0].children[4].children[0].token);
            Assert.AreEqual(g.t["expr"], got.root.children[0].children[4].children[1].token);
            Assert.AreEqual(g.t["expr2"], got.root.children[0].children[4].children[1].children[0].token);
            Assert.AreEqual(g.t["opnd"], got.root.children[0].children[4].children[1].children[0].children[0].token);
            Assert.AreEqual(new IntLiteral("4"), got.root.children[0].children[4].children[1].children[0].children[0].children[0].token);

            Assert.AreEqual(g.t["stmts"], got.root.children[2].token);
            Assert.AreEqual(g.t["stmt"], got.root.children[2].children[0].token);
            Assert.AreEqual(g.t["print"], got.root.children[2].children[0].children[0].token);
            //eh, it's probably noticed by now if something's wrong
        }
    }
}
