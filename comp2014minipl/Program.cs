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
            /*
            NFA a = new NFA("a");
            NFA b = new NFA("b");
            NFA empty = new NFA("");
            NFA test = new NFA(1).complement();
            test.debugPrint();
            test.recognizes("a");
            test.debugPrint();
            */

            Regex reg = new Regex("a[b-d]e");
            reg.nfa.debugPrint();
            reg.recognize("a");
            reg.nfa.debugPrint();
            
            System.Console.ReadLine();
        }
    }
}
