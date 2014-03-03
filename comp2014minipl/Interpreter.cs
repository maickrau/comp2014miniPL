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
            if (node == null)
            {
                return new Expression(node);
            }
            if (node is Assign)
            {
                assign((Assign)node);
                return new Expression(node);
            }
            if (node is Define)
            {
                define((Define)node);
                return new Expression(node);
            }
            if (node is For)
            {
                evalFor((For)node);
                return new Expression(node);
            }
            if (node is Read)
            {
                read((Read)node);
                return new Expression(node);
            }
            if (node is Print)
            {
                print((Print)node);
                return new Expression(node);
            }
            if (node is Assertion)
            {
                assert((Assertion)node);
                return new Expression(node);
            }
            if (node is Expression)
            {
                return expression((Expression)node);
            }
            foreach (ASTNode c in node.children)
            {
                eval(c);
            }
            return new Expression(node);
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
            if (node.children.Count == 1)
            {
                return unaryOperatorCall(node);
            }
            if (node.children.Count == 2)
            {
                return binaryOperatorCall(node);
            }
            throw new Exception("Operator call with " + node.children.Count + " operands");
        }
        private Expression unaryOperatorCall(OperatorCall node)
        {
            Expression val = eval(node.children[0]);
            if (val is IntValue)
            {
                return intOperatorCall(node.op, new IntValue(node, 0), (IntValue)val);
            }
            if (val is StringValue)
            {
                return stringOperatorCall(node.op, new StringValue(node, ""), (StringValue)val);
            }
            if (val is BoolValue)
            {
                return boolOperatorCall(node.op, new BoolValue(node, false), (BoolValue)val);
            }
            throw new MiniPLException("Unknown type for calling operator, line " + node.line + ":" + node.position);
        }
        private Expression binaryOperatorCall(OperatorCall node)
        {
            Expression val1 = eval(node.children[0]);
            Expression val2 = eval(node.children[1]);
            if (val1.type != val2.type)
            {
                throw new MiniPLException("Operator has different types, line " + node.line + ":" + node.position);
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
            throw new MiniPLException("Unknown type for calling operator, line " + node.line + ":" + node.position);
        }
        private Expression intOperatorCall(Operator op, IntValue val1, IntValue val2)
        {
            if (op.Equals(ast.grammar.o["+"]))
            {
                return new IntValue(val1, val1.value + val2.value);
            }
            if (op.Equals(ast.grammar.o["-"]))
            {
                return new IntValue(val1, val1.value - val2.value);
            }
            if (op.Equals(ast.grammar.o["*"]))
            {
                return new IntValue(val1, val1.value * val2.value);
            }
            if (op.Equals(ast.grammar.o["/"]))
            {
                return new IntValue(val1, val1.value / val2.value);
            }
            if (op.Equals(ast.grammar.o["&"]))
            {
                return new IntValue(val1, val1.value & val2.value);
            }
            if (op.Equals(ast.grammar.o["<"]))
            {
                return new BoolValue(val1, val1.value < val2.value);
            }
            if (op.Equals(ast.grammar.o["="]))
            {
                return new BoolValue(val1, val1.value == val2.value);
            }
            throw new MiniPLException("Unsupported operator for int: " + op);
        }
        private Expression boolOperatorCall(Operator op, BoolValue val1, BoolValue val2)
        {
            if (op.Equals(ast.grammar.o["+"]))
            {
                return new BoolValue(val1, val1.value | val2.value);
            }
            if (op.Equals(ast.grammar.o["-"]))
            {
                return new BoolValue(val1, val1.value & (!val2.value));
            }
            if (op.Equals(ast.grammar.o["*"]))
            {
                return new BoolValue(val1, val1.value | val2.value);
            }
            if (op.Equals(ast.grammar.o["&"]))
            {
                return new BoolValue(val1, val1.value & val2.value);
            }
            if (op.Equals(ast.grammar.o["<"]))
            {
                return new BoolValue(val1, !val1.value && val2.value);
            }
            if (op.Equals(ast.grammar.o["="]))
            {
                return new BoolValue(val1, val1.value == val2.value);
            }
            if (op.Equals(new Operator("!")))
            {
                return new BoolValue(val2, !val2.value);
            }
            throw new MiniPLException("Unsupported operator for bool: " + op);
        }
        private Expression stringOperatorCall(Operator op, StringValue val1, StringValue val2)
        {
            if (op.Equals(ast.grammar.o["+"]))
            {
                return new StringValue(val1, val1.value + val2.value);
            }
            if (op.Equals(ast.grammar.o["<"]))
            {
                return new BoolValue(val1, val1.value.CompareTo(val2.value) < 0);
            }
            if (op.Equals(ast.grammar.o["="]))
            {
                return new BoolValue(val1, val1.value == val2.value);
            }
            throw new MiniPLException("Unsupported operator for string: " + op);
        }
        private void assert(Assertion node)
        {
            Expression value = eval(node.children[0]);
            if (value is BoolValue)
            {
                if (!((BoolValue)value).value)
                {
                    System.Console.WriteLine("Assertion failed, line " + node.line + ":" + node.position);
                }
            }
        }
        private void print(Print node)
        {
            Expression value = eval(node.children[0]);
            if (value is IntValue)
            {
                System.Console.Write(((IntValue)value).value);
            }
            else if (value is StringValue)
            {
                System.Console.Write(((StringValue)value).value);
            }
            else if (value is BoolValue)
            {
                System.Console.Write(((BoolValue)value).value);
            }
            else
            {
                throw new MiniPLException("Unkown type " + value + " for printing, line " + node.line + ":" + node.position);
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
            Variable var = (Variable)scope.getVar(((Variable)(node.children[0])).name);
            if (var.type == typeof(String))
            {
                scope.set(var, new StringValue(node, word));
            }
            else if (var.type == typeof(int))
            {
                int val = Convert.ToInt32(word);
                scope.set(var, new IntValue(node, val));
            }
            else if (var.type == typeof(bool))
            {
                bool val = false;
                if (word == "true")
                {
                    val = true;
                }
                scope.set(var, new BoolValue(node, val));
            }
            else
            {
                throw new MiniPLException("Unknown type for variable " + var.name + ": " + var.type + ", line " + node.line + ":" + node.position);
            }
        }
        private void evalFor(For node)
        {
            if (!scope.hasVariable((Variable)node.children[0]))
            {
                throw new MiniPLException("Loop variable must be declared before loop, line " + node.line + ":" + node.position);
            }
            Expression start = eval(node.children[1]);
            Expression end = eval(node.children[2]);
            if (!(start is IntValue))
            {
                throw new MiniPLException("Loop start index must be an int, line " + node.line + ":" + node.position);
            }
            if (!(end is IntValue))
            {
                throw new MiniPLException("Loop end index must be an int, line " + node.line + ":" + node.position);
            }
            int endI = ((IntValue)end).value;
            int startI = ((IntValue)start).value;
            for (int i = startI; i <= endI; i++)
            {
                scope.set((Variable)node.children[0], new IntValue(node, i));
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
        private Expression defaultValue(Type type, ASTNode from)
        {
            if (type == typeof(int))
            {
                return new IntValue(from, 0);
            }
            if (type == typeof(String))
            {
                return new StringValue(from, "");
            }
            if (type == typeof(bool))
            {
                return new BoolValue(from, false);
            }
            throw new MiniPLException("Unknown type for default value, line " + from.line + ":" + from.position);
        }
        private void define(Define node)
        {
            if (node.children[2] != null)
            {
                scope.define((Variable)node.children[0], (TypeName)node.children[1], eval(node.children[2]));
            }
            else
            {
                scope.define((Variable)node.children[0], (TypeName)node.children[1], defaultValue(((TypeName)node.children[1]).type, node));
            }
        }
    }
}
