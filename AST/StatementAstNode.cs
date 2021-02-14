#nullable enable
using RubbishLanguageFrontEnd.Util;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RubbishLanguageFrontEnd.AST {
    using EqUtil = EqualCheckUtil;

    public class IdentifierAstNode : BasicAstNode {
        public string Name { get; }

        public IdentifierAstNode(string name) {
            Name = name;
        }

        public override string ToString() {
            return Name;
        }

        public bool Equals(IdentifierAstNode other) {
            return Name == other.Name;
        }

        public override bool Equals(object? obj) {
            return EqUtil.NotNullAndSameType(this, obj) &&
                   Equals((IdentifierAstNode) obj!);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Name);
        }
    }

    public class VariableDefineAstNode : BasicAstNode {
        public string Type { get; }
        public string Name { get; }
        public BasicAstNode? AssignAst { get; }

        public VariableDefineAstNode(string type, string name, BasicAstNode? assign) {
            Type = type;
            Name = name;
            AssignAst = assign;
        }

        public override string ToString() {
            var basic = $"{Type} {Name}";
            if (AssignAst == null) {
                return basic + ';';
            }

            return $"{basic} = {AssignAst};";
        }

        public bool Equals(VariableDefineAstNode other) {
            return Type == other.Type && Name == other.Name &&
                   Equals(AssignAst, other.AssignAst);
        }

        public override bool Equals(object? obj) {
            return EqUtil.NotNullAndSameType(this, obj) &&
                   Equals((VariableDefineAstNode) obj!);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Type, Name, AssignAst);
        }
    }

    public class ExpressionAstNode : BasicAstNode { }

    public class UnaryOperatorAstNode : ExpressionAstNode {
        public string Operator { get; }
        public BasicAstNode Expr { get; }

        public UnaryOperatorAstNode(string op, BasicAstNode expr) {
            Operator = op;
            Expr = expr;
        }

        public override string ToString() => $"{Operator} {Expr}";

        public bool Equals(UnaryOperatorAstNode other) {
            return Operator == other.Operator &&
                   Equals(Expr, other.Expr);
        }

        public override bool Equals(object? obj) {
            return EqUtil.NotNullAndSameType(this, obj) &&
                   Equals((UnaryOperatorAstNode) obj!);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Operator, Expr);
        }
    }

    public class BinaryOperatorAstNode : ExpressionAstNode {
        public string Operator { get; }
        public BasicAstNode Left { get; }
        public BasicAstNode Right { get; }

        public BinaryOperatorAstNode(string op, BasicAstNode left, BasicAstNode right) {
            Operator = op;
            Left = left;
            Right = right;
        }

        public override string ToString() => $"{Left} {Operator} {Right}";

        public bool Equals(BinaryOperatorAstNode other) {
            return Operator == other.Operator &&
                   Equals(Left, other.Left) &&
                   Equals(Right, other.Right);
        }

        public override bool Equals(object? obj) {
            return EqUtil.NotNullAndSameType(this, obj) &&
                   Equals((BinaryOperatorAstNode) obj!);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Operator, Left, Right);
        }
    }


    public class CodeBlockAstNode : BasicAstNode {
        public BasicAstNode?[] Statements { get; }

        public CodeBlockAstNode(BasicAstNode[] statements) {
            Statements = statements;
        }

        public override string ToString() {
            if (Statements.Length == 0) {
                return " ";
            }

            var sb = new StringBuilder();
            foreach (var statement in Statements) {
                if (statement == null)
                    continue;
                sb.Append('\t');
                sb.Append(statement);
                sb.Append('\n');
            }

            return sb.ToString();
        }

        public bool Equals(CodeBlockAstNode other) {
            return EqUtil.EnumerableEqual(Statements, other.Statements);
        }

        public override bool Equals(object? obj) {
            return EqUtil.NotNullAndSameType(this, obj) &&
                   Equals((CodeBlockAstNode) obj!);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Statements);
        }
    }


    public class ReturnAstNode : BasicAstNode {
        public BasicAstNode ReturnValue { get; }

        public ReturnAstNode(BasicAstNode rv) {
            ReturnValue = rv;
        }

        public override string ToString() {
            return $"return {ReturnValue}";
        }

        public bool Equals(ReturnAstNode other) {
            return Equals(ReturnValue, other.ReturnValue);
        }

        public override bool Equals(object? obj) {
            return EqUtil.NotNullAndSameType(this, obj) &&
                   Equals((ReturnAstNode) obj!);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), ReturnValue);
        }
    }
}
