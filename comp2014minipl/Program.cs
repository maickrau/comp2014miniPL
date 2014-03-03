using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2014minipl
{
    public class MiniPLException : Exception
    {
        public MiniPLException(String str) : base(str) { }
    }
    class Program
    {
        static void interpret(String program)
        {
            System.Console.WriteLine("Parsing");
            MiniPLGrammar g = new MiniPLGrammar();
            AST ast;
            try
            {
                ast = new AST(g, g.parse(program));
            }
            catch (MiniPLException e)
            {
                System.Console.WriteLine("Error parsing program: " + e);
                return;
            }
            catch (ParserPredictException e)
            {
                System.Console.WriteLine("Error parsing program: " + e);
                return;
            }
            catch (ScannerException e)
            {
                System.Console.WriteLine("Error parsing program: " + e);
                return;
            }
            System.Console.WriteLine("Interpreting");
            Interpreter inter = new Interpreter();
            try
            {
                inter.run(ast);
            }
            catch (MiniPLException e)
            {
                System.Console.WriteLine("Error interpreting program: " + e);
                return;
            }
            System.Console.WriteLine("");
            System.Console.WriteLine("Done interpreting");
        }
        static String getProgramFromConsole()
        {
            System.Console.WriteLine("Write your program here.");
            System.Console.WriteLine("Press enter on an empty line when you are finished");
            String got = null;
            String program = "";
            int currentLine = 0;
            while (got != "")
            {
                System.Console.Write("" + currentLine + ": ");
                currentLine++;
                got = System.Console.ReadLine();
                program += got + "\r\n";
            }
            return program;
        }
        static void Main(string[] args)
        {
            interpret(getProgramFromConsole());
            System.Console.WriteLine("Press enter to end");
            System.Console.ReadLine();
        }
    }
}
