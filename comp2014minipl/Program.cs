using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace comp2014minipl
{
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
            } //these exceptions are the user's fault
            catch (SemanticError e)
            {
                System.Console.WriteLine("Error parsing program\n" + e.Message);
                return;
            }
            catch (ParseError e)
            {
                System.Console.WriteLine("Error parsing program\n" + e.Message);
                return;
            }
            catch (ScannerException e)
            {
                System.Console.WriteLine("Error parsing program\n" + e.Message);
                return;
            }
            //every other exception is a bug in this program
            System.Console.WriteLine("Interpreting");
            Interpreter inter = new Interpreter();
            try
            {
                inter.run(ast);
            }
            catch (InterpreterException e) //user's fault
            {
                System.Console.WriteLine("Error interpreting program: " + e.Message);
                return;
            }
            //everything else is a bug
            System.Console.WriteLine("");
            System.Console.WriteLine("Done interpreting");
        }
        static String getProgramFromFile(String fileName)
        {
            StringBuilder str = new StringBuilder();
            using (StreamReader reader = new StreamReader(fileName))
            {
                String line = reader.ReadLine();
                while (line != null)
                {
                    str.AppendLine(line);
                    line = reader.ReadLine();
                }
            }
            return str.ToString();
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
            if (args.Count() == 0)
            {
                interpret(getProgramFromConsole());
            }
            else if (args.Count() == 1)
            {
                interpret(getProgramFromFile(args[0]));
            }
            else
            {
                System.Console.WriteLine("Too many arguments, expected 0 or 1");
            }
            System.Console.WriteLine("Press enter to end");
            System.Console.ReadLine();
        }
    }
}
