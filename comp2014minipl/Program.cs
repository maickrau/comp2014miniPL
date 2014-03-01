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
            Scanner s = new Scanner();
            List<Token> result = s.parse("\"abc\" ");
            System.Console.WriteLine("\"{0}\"", ((StringLiteral)result[0]).value);
            System.Console.ReadLine();
        }
    }
}
