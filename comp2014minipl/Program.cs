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
            NFA hello = new NFA("hello");
            NFA lul = new NFA("lul");
            NFA test = lul.conc(hello.closure());
            test.debugPrint();
            test.deEpsilonate();
            test.debugPrint();
            System.Console.ReadLine();
        }
    }
}
