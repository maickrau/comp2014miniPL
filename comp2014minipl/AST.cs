using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2014minipl
{
    public class ASTNode
    {
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
        public ASTNode()
        {
            children = new List<ASTNode>();
        }
    }
    public class TypeName : ASTNode
    {
        public Type type;
    }
    public class Expression : ASTNode
    {
        public Type type;
    }
    public class OperatorCall : Expression
    {
        public Operator op;
    }
    public class Variable : Expression
    {
        public Variable(String name)
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
        public int value;
        public IntValue(int value)
        {
            this.value = value;
            type = typeof(int);
        }
    }
    public class StringValue : Expression
    {
        public String value;
        public StringValue(String value)
        {
            this.value = value;
            type = typeof(String);
        }
    }
    public class BoolValue : Expression
    {
        public bool value;
        public BoolValue(bool value)
        {
            this.value = value;
            type = typeof(bool);
        }
    }
    public class Define : ASTNode
    {
    }
    public class Assign : ASTNode
    {
    }
    public class For : ASTNode
    {
    }
    public class Read : ASTNode
    {
    }
    public class Print : ASTNode
    {
    }
    public class Assertion : ASTNode
    {
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
                ASTNode ret = new ASTNode();
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
                ASTNode ret = new ASTNode();
                if (node.children.Count > 0)
                {
                    ret.children.Add(parse(node.children[0]));
                    ret.children.Add(parse(node.children[2]));
                }
                return ret;
            }
            return new ASTNode();
        }
        private ASTNode parseStatement(SyntaxNode node)
        {
            if (!node.token.Equals(grammar.t["stmt"]))
            {
                throw new Exception("Trying to parse a non-statement " + node.token + " as a statement");
            }
            if (node.children[0].token.Equals(grammar.t["var"]))
            {
                Define ret = new Define();
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
                Assign ret = new Assign();
                ret.children.Add(parseVariable(node.children[0]));
                ret.children.Add(parseExpression(node.children[2]));
                return ret;
            }
            if (node.children[0].token.Equals(grammar.t["for"]))
            {
                For ret = new For();
                ret.children.Add(parseVariable(node.children[1]));
                ret.children.Add(parseExpression(node.children[3]));
                ret.children.Add(parseExpression(node.children[5]));
                ret.children.Add(parse(node.children[7]));
                return ret;
            }
            if (node.children[0].token.Equals(grammar.t["read"]))
            {
                Read ret = new Read();
                ret.children.Add(parseVariable(node.children[1]));
                return ret;
            }
            if (node.children[0].token.Equals(grammar.t["print"]))
            {
                Print ret = new Print();
                ret.children.Add(parseExpression(node.children[1]));
                return ret;
            }
            if (node.children[0].token.Equals(grammar.t["assert"]))
            {
                Assertion ret = new Assertion();
                ret.children.Add(parseExpression(node.children[2]));
                return ret;
            }
            throw new Exception("Un-AST-ed syntax node, token " + node.token + ", first child " + node.children[0].token);
        }
        private TypeName parseType(SyntaxNode node)
        {
            TypeName ret = new TypeName();
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
                throw new Exception("Unknown type name: " + node.children[0].token);
            }
            return ret;
        }
        private Variable parseVariable(SyntaxNode node)
        {
            return new Variable(((Identifier)node.token).value);
        }
        private Expression parseExpression(SyntaxNode node)
        {
            if (node.token.Equals(grammar.t["expr"]))
            {
                if (node.children[0].token.Equals(grammar.t["expr2"]))
                {
                    return parseExpression(node.children[0]);
                }
                if (node.children[0].token.Equals(new Operator("!")))
                {
                    OperatorCall ret = new OperatorCall();
                    ret.op = (Operator)node.children[0].token;
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
                OperatorCall ret = new OperatorCall();
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
                return new IntValue(((IntLiteral)node.children[0].token).value);
            }
            if (node.children[0].token is StringLiteral)
            {
                return new StringValue(((StringLiteral)node.children[0].token).value);
            }
            if (node.children[0].token is BoolLiteral)
            {
                return new BoolValue(((BoolLiteral)node.children[0].token).value);
            }
            if (node.children[0].token is Identifier)
            {
                return new Variable(((Identifier)node.children[0].token).value);
            }
            throw new Exception("oops! some operand type is missing");
        }
    }
}
