using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2014minipl
{
    class Interpreter
    {
        AST ast;
        Scope scope;
        Queue<String> unusedInputs;
        public Interpreter()
        {
        }
        public void run(AST ast)
        {
            unusedInputs = new Queue<String>();
            this.ast = ast;
            scope = new Scope();
            eval(ast.root);
        }
        private Expression eval(ASTNode node)
        {
            if (node is Assign)
            {
                assign((Assign)node);
                return new Expression();
            }
            if (node is Define)
            {
                define((Define)node);
                return new Expression();
            }
            if (node is For)
            {
                evalFor((For)node);
                return new Expression();
            }
            if (node is Read)
            {
                read((Read)node);
                return new Expression();
            }
            if (node is Print)
            {
                print((Print)node);
                return new Expression();
            }
            if (node is Assertion)
            {
                assert((Assertion)node);
                return new Expression();
            }
            if (node is Expression)
            {
                return expression((Expression)node);
            }
            foreach (ASTNode c in node.children)
            {
                eval(c);
            }
            return new Expression();
        }
        private Expression expression(Expression node)
        {
            if (node is OperatorCall)
            {
                return operatorCall((OperatorCall)node);
            }
            if (node is Variable)
            {
                return scope.getValue(((Variable)node).name);
            }
            return node;
        }
        private Expression operatorCall(OperatorCall node)
        {
            Expression val1 = eval(node.children[0]);
            Expression val2 = eval(node.children[1]);
            if (val1.type != val2.type)
            {
                throw new Exception("Operator has different types");
            }
            if (val1 is IntValue)
            {
                return intOperatorCall(node.op, (IntValue)val1, (IntValue)val2);
            }
            if (val1 is StringValue)
            {
                return stringOperatorCall(node.op, (StringValue)val1, (StringValue)val2);
            }
            if (val1 is BoolValue)
            {
                return boolOperatorCall(node.op, (BoolValue)val1, (BoolValue)val2);
            }
            throw new Exception("Unknown type for calling operator");
        }
        private Expression intOperatorCall(Operator op, IntValue val1, IntValue val2)
        {
            if (op.Equals(ast.grammar.o["+"]))
            {
                return new IntValue(val1.value + val2.value);
            }
            if (op.Equals(ast.grammar.o["-"]))
            {
                return new IntValue(val1.value - val2.value);
            }
            if (op.Equals(ast.grammar.o["*"]))
            {
                return new IntValue(val1.value * val2.value);
            }
            if (op.Equals(ast.grammar.o["/"]))
            {
                return new IntValue(val1.value / val2.value);
            }
            if (op.Equals(ast.grammar.o["&"]))
            {
                return new IntValue(val1.value & val2.value);
            }
            if (op.Equals(ast.grammar.o["<"]))
            {
                return new BoolValue(val1.value < val2.value);
            }
            if (op.Equals(ast.grammar.o["="]))
            {
                return new BoolValue(val1.value == val2.value);
            }
            throw new Exception("Unsupported operator for int: " + op);
        }
        private Expression boolOperatorCall(Operator op, BoolValue val1, BoolValue val2)
        {
            if (op.Equals(ast.grammar.o["+"]))
            {
                return new BoolValue(val1.value | val2.value);
            }
            if (op.Equals(ast.grammar.o["-"]))
            {
                return new BoolValue(val1.value & (!val2.value));
            }
            if (op.Equals(ast.grammar.o["*"]))
            {
                return new BoolValue(val1.value | val2.value);
            }
            if (op.Equals(ast.grammar.o["&"]))
            {
                return new BoolValue(val1.value & val2.value);
            }
            if (op.Equals(ast.grammar.o["<"]))
            {
                return new BoolValue(!val1.value && val2.value);
            }
            if (op.Equals(ast.grammar.o["="]))
            {
                return new BoolValue(val1.value == val2.value);
            }
            throw new Exception("Unsupported operator for int: " + op);
        }
        private Expression stringOperatorCall(Operator op, StringValue val1, StringValue val2)
        {
            if (op.Equals(ast.grammar.o["+"]))
            {
                return new StringValue(val1.value + val2.value);
            }
            if (op.Equals(ast.grammar.o["<"]))
            {
                return new BoolValue(val1.value.CompareTo(val2.value) < 0);
            }
            if (op.Equals(ast.grammar.o["="]))
            {
                return new BoolValue(val1.value == val2.value);
            }
            throw new Exception("Unsupported operator for string: " + op);
        }
        private void assert(Assertion node)
        {
            Expression value = eval(node.children[0]);
            if (value is BoolValue)
            {
                if (!((BoolValue)value).value)
                {
                    System.Console.WriteLine("Assertion failed (what and where? who knows...)");
                }
            }
        }
        private void print(Print node)
        {
            Expression value = eval(node.children[0]);
            if (value is IntValue)
            {
                System.Console.WriteLine(((IntValue)value).value);
            }
            if (value is StringValue)
            {
                System.Console.WriteLine(((StringValue)value).value);
            }
            if (value is BoolValue)
            {
                System.Console.WriteLine(((BoolValue)value).value);
            }
        }
        private void read(Read node)
        {
            if (unusedInputs.Count == 0)
            {
                String read = System.Console.ReadLine();
                String[] words = read.Split(' ');
                foreach (String s in words)
                {
                    unusedInputs.Enqueue(s);
                }
            }
            String word = unusedInputs.Dequeue();
            Variable var = (Variable)scope.get((Variable)(node.children[0]));
            if (var.type == typeof(String))
            {
                scope.set(var, new StringValue(word));
            }
            if (var.type == typeof(int))
            {
                int val = Convert.ToInt32(word);
                scope.set(var, new IntValue(val));
            }
            if (var.type == typeof(bool))
            {
                bool val = false;
                if (word == "true")
                {
                    val = true;
                }
                scope.set(var, new BoolValue(val));
            }
        }
        private void evalFor(For node)
        {
            if (!scope.hasVariable((Variable)node.children[0]))
            {
                throw new Exception("Loop variable must be declared before loop");
            }
            Expression start = eval(node.children[1]);
            Expression end = eval(node.children[2]);
            if (!(start is IntValue))
            {
                throw new Exception("Loop start index must be an int");
            }
            if (!(end is IntValue))
            {
                throw new Exception("Loop end index must be an int");
            }
            int endI = ((IntValue)end).value;
            int startI = ((IntValue)start).value;
            for (int i = startI; i < endI; i++)
            {
                scope.set((Variable)node.children[0], new IntValue(i));
                scope.setConstant((Variable)node.children[0], true);
                for (int a = 3; a < node.children.Count; a++)
                {
                    eval(node.children[a]);
                }
                scope.setConstant((Variable)node.children[0], false);
            }
        }
        private void assign(Assign node)
        {
            scope.set((Variable)node.children[0], eval(node.children[1]));
        }
        private void define(Define node)
        {
            scope.define((Variable)node.children[0], (TypeName)node.children[1], eval(node.children[2]));
        }
    }
}
