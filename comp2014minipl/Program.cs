using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2014minipl
{
    class Program
    {
        static void Main(string[] args)
        {
            String str = "     var X : int := 4 + (6 * 2);     print X;";
            MiniPLGrammar g = new MiniPLGrammar();
            AST ast = new AST(g, g.parse(str));
            Interpreter inter = new Interpreter();
            inter.run(ast);
            System.Console.ReadLine();
        }
    }
}
