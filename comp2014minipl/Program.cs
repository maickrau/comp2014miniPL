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
            Regex reg = new Regex("a[bcd]e");
            reg.nfa.debugPrint();
            reg.recognize("abc");
            reg.nfa.debugPrint();
            System.Console.ReadLine();
        }
    }
}
