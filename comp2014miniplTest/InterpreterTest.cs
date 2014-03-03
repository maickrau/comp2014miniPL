using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2014minipl;
using System.Collections.Generic;
using System.IO;

namespace comp2014miniplTest
{
    [TestClass]
    public class InterpreterTest
    {
        private class TextReaderMock : TextReader
        {
            List<String> inputs;
            int currentInput;
            public TextReaderMock(List<String> inputs) : base()
            {
                this.inputs = inputs;
                currentInput = 0;
            }
            public override String ReadLine()
            {
                String ret = inputs[currentInput];
                currentInput++;
                return ret;
            }
        }
        private class TextWriterMock : TextWriter
        {
            List<String> got;
            public override System.Text.Encoding Encoding
            {
                get { return Console.Out.Encoding; }
            }
            public TextWriterMock() : base()
            {
                got = new List<String>();
            }
            public override void Write(String value)
            {
                got.Add(value);
            }
            public override void WriteLine(String line)
            {
                got.Add(line + "\r\n");
            }
            public bool gotAllWanted(List<String> wanted)
            {
                int loc = 0;
                foreach (String s in wanted)
                {
                    while (got[loc] != s)
                    {
                        loc++;
                        if (loc >= got.Count)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        private void runProgram(String program, List<String> inputs, List<String> wantedOutputs)
        {
            TextReaderMock input = new TextReaderMock(inputs);
            TextWriterMock output = new TextWriterMock();
            MiniPLGrammar g = new MiniPLGrammar();
            AST ast = new AST(g, g.parse(program));
            Interpreter inter = new Interpreter(input, output);
            inter.run(ast);
            Assert.AreEqual(true, output.gotAllWanted(wantedOutputs));
        }
        [TestMethod]
        public void runsSimpleProgram1()
        {
            String program = @"
     var X : int := 4 + (6 * 2);
     print X;
            ";
            runProgram(program, new List<String>(), new List<String> { "16" });
        }
        [TestMethod]
        public void runsProgram1()
        {
            String program = @"
     var nTimes : int := 0;
     print ""How many times?""; 
     read nTimes; 
     var x : int;
     for x in 0..nTimes-1 do 
         print x;
         print "" : Hello, World!\n"";
     end for;
     assert (x = nTimes);
            ";
            runProgram(program, new List<String>{"5"}, new List<String> {"0", " : Hello, World!\n", "1", "4"});
        }
        [TestMethod]
        public void runsProgram2()
        {
            String program = @"
     print ""Give a number""; 
     var n : int;
     read n;
     var f : int := 1;
     var i : int;
     for i in 1..n do 
         f := f * i;
     end for;
     print ""The result is: "";
     print f; 
            ";
            runProgram(program, new List<String>{"5"}, new List<String> {"120"});
        }
    }
}
