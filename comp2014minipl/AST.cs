using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2014minipl
{
    public class SemanticError : Exception
    {
        public SemanticError(String str, SyntaxNode from) : base(str + ", line " + from.line + ":" + from.position) { }
    }
    public class ASTNode
    {
        public int line;
        public int position;
        public override bool Equals(object obj)
        {
            if (!(obj is ASTNode))
            {
                return false;
            }
            if (children.Count != ((ASTNode)obj).children.Count)
            {
                return false;
            }
            for (int i = 0; i < children.Count; i++)
            {
                if (!children[i].Equals(((ASTNode)obj).children[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public List<ASTNode> children;
        public ASTNode(SyntaxNode from)
        {
            this.line = from.line;
            this.position = from.position;
            children = new List<ASTNode>();
        }
        public ASTNode(ASTNode from)
        {
            this.line = from.line;
            this.position = from.position;
            children = new List<ASTNode>();
        }
    }
    public class TypeName : ASTNode
    {
        public TypeName(SyntaxNode from) : base(from) { }
        public Type type;
    }
    public class Expression : ASTNode
    {
        public Expression(SyntaxNode from) : base(from) { }
        public Expression(ASTNode from) : base(from) { }
        public Type type;
    }
    public class OperatorCall : Expression
    {
        public OperatorCall(SyntaxNode from) : base(from) { }
        public OperatorCall(ASTNode from) : base(from) { }
        public Operator op;
    }
    public class Variable : Expression
    {
        public Variable(SyntaxNode from) : base(from) { }
        public Variable(ASTNode from) : base(from) { }
        public Variable(ASTNode from, String name) : base(from)
        {
            this.name = name;
        }
        public Variable(SyntaxNode from, String name) : base(from)
        {
            this.name = name;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Variable))
            {
                return false;
            }
            return name == ((Variable)obj).name;
        }
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        public String name;
    }
    public class IntValue : Expression
    {
        public IntValue(SyntaxNode from) : base(from) { }
        public IntValue(ASTNode from) : base(from) { }
        public int value;
        public IntValue(ASTNode from, int value) : base(from)
        {
            this.value = value;
            type = typeof(int);
        }
        public IntValue(SyntaxNode from, int value) : base(from)
        {
            this.value = value;
            type = typeof(int);
        }
    }
    public class StringValue : Expression
    {
        public StringValue(SyntaxNode from) : base(from) { }
        public StringValue(ASTNode from) : base(from) { }
        public String value;
        public StringValue(ASTNode from, String value) : base(from)
        {
            this.value = value;
            type = typeof(String);
        }
        public StringValue(SyntaxNode from, String value) : base(from)
        {
            this.value = value;
            type = typeof(String);
        }
    }
    public class BoolValue : Expression
    {
        public BoolValue(SyntaxNode from) : base(from) { }
        public BoolValue(ASTNode from) : base(from) { }
        public BoolValue(ASTNode from, bool value) : base(from) 
        {
            this.value = value;
            type = typeof(bool);
        }
        public bool value;
        public BoolValue(SyntaxNode from, bool value) : base(from)
        {
            this.value = value;
            type = typeof(bool);
        }
    }
    public class Define : ASTNode
    {
        public Define(SyntaxNode from) : base(from) { }
        public Define(ASTNode from) : base(from) { }
    }
    public class Assign : ASTNode
    {
        public Assign(SyntaxNode from) : base(from) { }
        public Assign(ASTNode from) : base(from) { }
    }
    public class For : ASTNode
    {
        public For(SyntaxNode from) : base(from) { }
        public For(ASTNode from) : base(from) { }
    }
    public class Read : ASTNode
    {
        public Read(SyntaxNode from) : base(from) { }
        public Read(ASTNode from) : base(from) { }
    }
    public class Print : ASTNode
    {
        public Print(SyntaxNode from) : base(from) { }
        public Print(ASTNode from) : base(from) { }
    }
    public class Assertion : ASTNode
    {
        public Assertion(SyntaxNode from) : base(from) { }
        public Assertion(ASTNode from) : base(from) { }
    }
    public class AST
    {
        public ASTNode root;
        public MiniPLGrammar grammar;
        public Dictionary<String, Variable> vars;
        private HashSet<String> currentConstants;
        public AST(MiniPLGrammar grammar, SyntaxTree tree)
        {
            currentConstants = new HashSet<String>();
            vars = new Dictionary<String, Variable>();
            this.grammar = grammar;
            root = parse(tree.root);
        }
        private ASTNode parse(SyntaxNode node)
        {
            if (node.token.Equals(grammar.t["prog"]))
            {
                ASTNode ret = new ASTNode(node);
                ret.children.Add(parse(node.children[0]));
                ret.children.Add(parse(node.children[2]));
                return ret;
            }
            if (node.token.Equals(grammar.t["stmt"]))
            {
                return parseStatement(node);
            }
            if (node.token.Equals(grammar.t["stmts"]))
            {
                ASTNode ret = new ASTNode(node);
                if (node.children.Count > 0)
                {
                    ret.children.Add(parse(node.children[0]));
                    ret.children.Add(parse(node.children[2]));
                }
                return ret;
            }
            return new ASTNode(node);
        }
        private ASTNode parseStatement(SyntaxNode node)
        {
            if (!node.token.Equals(grammar.t["stmt"]))
            {
                throw new Exception("Trying to parse a non-statement " + node.token + " as a statement, line " + node.line + ":" + node.position);
            }
            if (node.children[0].token.Equals(grammar.t["var"]))
            {
                Define ret = new Define(node);
                Variable newVariable = parseVariable(node.children[1], true);
                TypeName type = parseType(node.children[3]);
                ret.children.Add(newVariable);
                ret.children.Add(type);
                newVariable.type = type.type;
                if (node.children[4].children.Count > 0)
                {
                    Expression initialValue = parseExpression(node.children[4].children[1]);
                    if (initialValue.type != newVariable.type)
                    {
                        throw new SemanticError("Variable " + newVariable.name + " defined with initial value of different type", node);
                    }
                    ret.children.Add(initialValue);
                }
                else
                {
                    ret.children.Add(null);
                }
                return ret;
            }
            if (node.children[0].token is Identifier)
            {
                Assign ret = new Assign(node);
                Variable var = parseVariable(node.children[0], false);
                if (currentConstants.Contains(var.name))
                {
                    throw new SemanticError("Assignment to loop variable " + var.name, node);
                }
                Expression value = parseExpression(node.children[2]);
                if (var.type != value.type)
                {
                    throw new SemanticError("Variable " + var.name + " assigned a value of different type", node);
                }
                ret.children.Add(var);
                ret.children.Add(value);
                return ret;
            }
            if (node.children[0].token.Equals(grammar.t["for"]))
            {
                For ret = new For(node);
                Variable var = parseVariable(node.children[1], false);
                Expression from = parseExpression(node.children[3]);
                Expression to = parseExpression(node.children[5]);
                if (var.type != typeof(int))
                {
                    throw new SemanticError("Only ints may be iterated on", node);
                }
                if (from.type != typeof(int))
                {
                    throw new SemanticError("Iteration start needs to be an int", node);
                }
                if (to.type != typeof(int))
                {
                    throw new SemanticError("Iteration end needs to be an int", node);
                }
                ret.children.Add(var);
                ret.children.Add(from);
                ret.children.Add(to);
                currentConstants.Add(var.name);
                ret.children.Add(parse(node.children[7]));
                ret.children.Add(parse(node.children[9]));
                currentConstants.Remove(var.name);
                return ret;
            }
            if (node.children[0].token.Equals(grammar.t["read"]))
            {
                Read ret = new Read(node);
                ret.children.Add(parseVariable(node.children[1], false));
                return ret;
            }
            if (node.children[0].token.Equals(grammar.t["print"]))
            {
                Print ret = new Print(node);
                ret.children.Add(parseExpression(node.children[1]));
                return ret;
            }
            if (node.children[0].token.Equals(grammar.t["assert"]))
            {
                Assertion ret = new Assertion(node);
                ret.children.Add(parseExpression(node.children[2]));
                return ret;
            }
            throw new Exception("Un-AST-ed syntax node, token " + node.token + ", first child " + node.children[0].token + ", line " + node.line + ":" + node.position);
        }
        private TypeName parseType(SyntaxNode node)
        {
            TypeName ret = new TypeName(node);
            if (node.children[0].token.Equals(grammar.t["int"]))
            {
                ret.type = typeof(int);
            }
            else if (node.children[0].token.Equals(grammar.t["string"]))
            {
                ret.type = typeof(String);
            }
            else if (node.children[0].token.Equals(grammar.t["bool"]))
            {
                ret.type = typeof(bool);
            }
            else
            {
                throw new Exception("Unknown type name: " + node.children[0].token + ", line " + node.line + ":" + node.position);
            }
            return ret;
        }
        private Variable parseVariable(SyntaxNode node, bool shouldCreate)
        {
            String varName = ((Identifier)node.token).value;
            if (vars.ContainsKey(varName))
            {
                if (shouldCreate)
                {
                    throw new SemanticError("Variable " + varName + " defined twice", node);
                }
                return vars[varName];
            }
            if (!shouldCreate)
            {
                throw new SemanticError("Variable " + varName + " referenced before definition", node);
            }
            Variable newVar = new Variable(node, varName);
            vars[varName] = newVar;
            return newVar;
        }
        private bool unaryOperatorMakesSense(Operator op, Type type)
        {
            if (op == grammar.t["!"] && type == typeof(bool))
            {
                return true;
            }
            if (op == grammar.o["-"] && type == typeof(int))
            {
                return true;
            }
            return false;
        }
        private bool binaryOperatorMakesSense(Operator op, Type lhs, Type rhs)
        {
            if (lhs != rhs)
            {
                return false;
            }
            switch(op.value)
            {
                case "-":
                    return lhs == typeof(int) || lhs == typeof(bool);
                case "+":
                    return lhs == typeof(int) || lhs == typeof(String) || lhs == typeof(bool);
                case "*":
                    return lhs == typeof(int) || lhs == typeof(bool);
                case "/":
                    return lhs == typeof(int);
                case "&":
                    return lhs == typeof(int) || lhs == typeof(bool);
                case "<":
                    return true;
                case "=":
                    return true;
                default:
                    throw new Exception("No case for binary operator makes-sense check " + op);
            }
        }
        private Type binaryOperatorResult(Operator op, Type lhs, Type rhs)
        {
            //this could be used eg. so "abc"+1 => string, but it's not.
            return lhs;
        }
        private Expression parseExpression(SyntaxNode node)
        {
            if (node.token.Equals(grammar.t["expr"]))
            {
                if (node.children[0].token.Equals(grammar.t["expr2"]))
                {
                    return parseExpression(node.children[0]);
                }
                if (node.children[0].token.Equals(grammar.t["unary_op"]))
                {
                    OperatorCall ret = new OperatorCall(node);
                    Expression operand = parseExpression(node.children[1]);
                    ret.op = parseOperator(node.children[0]);
                    if (!unaryOperatorMakesSense(ret.op, operand.type))
                    {
                        throw new SemanticError("Unary operator " + ret.op.ToString() + " can't be applied to " + operand.type, node);
                    }
                    ret.children.Add(operand);
                    ret.type = operand.type;
                    return ret;
                }
            }
            if (node.token.Equals(grammar.t["opnd"]))
            {
                return parseOperand(node);
            }
            //node.token is expr2
            if (node.children[1].children.Count > 0)
            {
                OperatorCall ret = new OperatorCall(node);
                ret.op = parseOperator(node.children[1].children[0]);
                Expression lhs = parseExpression(node.children[0]);
                Expression rhs = parseExpression(node.children[1].children[1]);
                if (!binaryOperatorMakesSense(ret.op, lhs.type, rhs.type))
                {
                    throw new SemanticError("Binary operator " + ret.op.ToString() + " can't be applied to " + lhs.type + " and " + rhs.type, node);
                }
                ret.children.Add(lhs);
                ret.children.Add(rhs);
                ret.type = binaryOperatorResult(ret.op, lhs.type, rhs.type);
                return ret;
            }
            return parseExpression(node.children[0]);
        }
        private Operator parseOperator(SyntaxNode node)
        {
            return (Operator)node.children[0].token;
        }
        private Expression parseOperand(SyntaxNode node)
        {
            if (node.children[0].token.Equals(grammar.t["("]))
            {
                return parseExpression(node.children[1]);
            }
            if (node.children[0].token is IntLiteral)
            {
                return new IntValue(node, ((IntLiteral)node.children[0].token).value);
            }
            if (node.children[0].token is StringLiteral)
            {
                return new StringValue(node, ((StringLiteral)node.children[0].token).value);
            }
            if (node.children[0].token is BoolLiteral)
            {
                return new BoolValue(node, ((BoolLiteral)node.children[0].token).value);
            }
            if (node.children[0].token is Identifier)
            {
                return parseVariable(node.children[0], false);
            }
            throw new Exception("oops! some operand type is missing, line " + node.line + ":" + node.position);
        }
    }
}
