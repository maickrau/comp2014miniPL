using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2014minipl
{
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
        public Variable(ASTNode from, bool value) : base(from)
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
        public AST(MiniPLGrammar grammar, SyntaxTree tree)
        {
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
                throw new MiniPLException("Trying to parse a non-statement " + node.token + " as a statement, line " + node.line + ":" + node.position);
            }
            if (node.children[0].token.Equals(grammar.t["var"]))
            {
                Define ret = new Define(node);
                ret.children.Add(parseVariable(node.children[1]));
                ret.children.Add(parseType(node.children[3]));
                if (node.children[4].children.Count > 0)
                {
                    ret.children.Add(parseExpression(node.children[4].children[1]));
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
                ret.children.Add(parseVariable(node.children[0]));
                ret.children.Add(parseExpression(node.children[2]));
                return ret;
            }
            if (node.children[0].token.Equals(grammar.t["for"]))
            {
                For ret = new For(node);
                ret.children.Add(parseVariable(node.children[1]));
                ret.children.Add(parseExpression(node.children[3]));
                ret.children.Add(parseExpression(node.children[5]));
                ret.children.Add(parse(node.children[7]));
                ret.children.Add(parse(node.children[9]));
                return ret;
            }
            if (node.children[0].token.Equals(grammar.t["read"]))
            {
                Read ret = new Read(node);
                ret.children.Add(parseVariable(node.children[1]));
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
            throw new MiniPLException("Un-AST-ed syntax node, token " + node.token + ", first child " + node.children[0].token + ", line " + node.line + ":" + node.position);
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
                throw new MiniPLException("Unknown type name: " + node.children[0].token + ", line " + node.line + ":" + node.position);
            }
            return ret;
        }
        private Variable parseVariable(SyntaxNode node)
        {
            return new Variable(node, ((Identifier)node.token).value);
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
                    ret.op = parseOperator(node.children[0]);
                    ret.children.Add(parseExpression(node.children[1]));
                    ret.type = ((Expression)ret.children[0]).type;
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
                ret.children.Add(parseExpression(node.children[0]));
                ret.children.Add(parseExpression(node.children[1].children[1]));
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
                return new Variable(node, ((Identifier)node.children[0].token).value);
            }
            throw new MiniPLException("oops! some operand type is missing, line " + node.line + ":" + node.position);
        }
    }
}
