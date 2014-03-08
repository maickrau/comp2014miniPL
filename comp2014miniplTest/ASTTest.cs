using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2014minipl;

namespace comp2014miniplTest
{
    [TestClass]
    public class ASTTest
    {
        [TestMethod]
        public void testSimple()
        {
            String program = "     var X : int := 4 + (6 * 2);\n     print X;";
            MiniPLGrammar g = new MiniPLGrammar();
            AST ast = new AST(g, g.parse(program));
            Assert.AreEqual(2, ast.root.children.Count);
            Assert.IsInstanceOfType(ast.root.children[0], typeof(Define));
            Assert.IsInstanceOfType(ast.root.children[0].children[0], typeof(Variable));
            Assert.IsInstanceOfType(ast.root.children[0].children[1], typeof(TypeName));
            Assert.IsInstanceOfType(ast.root.children[0].children[2], typeof(Expression));
            Assert.IsInstanceOfType(ast.root.children[1], typeof(ASTNode));
            Assert.IsInstanceOfType(ast.root.children[1].children[0], typeof(Print));
            Assert.IsInstanceOfType(ast.root.children[1].children[0].children[0], typeof(Variable));
            Assert.AreEqual(typeof(int), ((TypeName)ast.root.children[0].children[1]).type);
        }
        [TestMethod]
        public void doesntStopAtFirstError()
        {
            String program = "print \"a\"-(\"b\"*3);";
            MiniPLGrammar g = new MiniPLGrammar();
            try
            {
                AST ast = new AST(g, g.parse(program));
                Assert.Fail("Didn't throw an exception");
            }
            catch (SemanticError e)
            {
                Assert.IsTrue(e.Message.Contains("0:9 Semantic error: "));
                Assert.IsTrue(e.Message.Contains("0:14 Semantic error: "));
            }
        }
    }
}
