using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using RubbishLanguageFrontEnd.CodeGenerator;
using RubbishLanguageFrontEnd.Lexer;

namespace RubbishLanguageFrontEnd.Parser.AST {
    public class IdentifierAstNode : BasicAstNode, IEquatable<IdentifierAstNode> {
        public string Name { get; }

        public IdentifierAstNode(string name) {
            Name = name;
        }

        public override string ToString() {
            return Name;
        }

        public bool Equals(IdentifierAstNode other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IdentifierAstNode) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Name);
        }
    }

    public class VariableDefineAstNode : BasicAstNode, IEquatable<VariableDefineAstNode> {
        public string Type { get; }
        public string Name { get; }
        [MaybeNull] public BasicAstNode AssignAst { get; }

        public VariableDefineAstNode(string type, string name, BasicAstNode assign) {
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
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && Name == other.Name &&
                   Equals(AssignAst, other.AssignAst);
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VariableDefineAstNode) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Type, Name, AssignAst);
        }
    }

    public class ExpressionAstNode : BasicAstNode { }

    public class UnaryOperatorAstNode : ExpressionAstNode,
        IEquatable<UnaryOperatorAstNode> {
        public string Operator { get; }
        public BasicAstNode Expr { get; }

        public UnaryOperatorAstNode(string op, BasicAstNode expr) {
            Operator = op;
            Expr = expr;
        }

        public override string ToString() => $"{Operator} {Expr}";

        public bool Equals(UnaryOperatorAstNode other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Operator == other.Operator &&
                   Equals(Expr, other.Expr);
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UnaryOperatorAstNode) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Operator, Expr);
        }
    }

    public class BinaryOperatorAstNode : ExpressionAstNode,
        IEquatable<BinaryOperatorAstNode> {
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
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Operator == other.Operator &&
                   Equals(Left, other.Left) && Equals(Right, other.Right);
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BinaryOperatorAstNode) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Operator, Left, Right);
        }
    }

    public class FunctionParameter {
        public enum ParamType {
            Int64 = TokenType.Int64,
            Float64 = TokenType.Float64,
            Str = TokenType.Str,
            Void = TokenType.Void
        }


        public ParamType Type;
        public string Name;

        public override string ToString() => $"{Type} {Name}";
    }

    public class FunctionPrototypeAstNode : BasicAstNode,
        IEquatable<FunctionPrototypeAstNode> {
        public FunctionParameter[] Parameters { get; }
        public string Name { get; }
        public FunctionParameter.ParamType ReturnType { get; }
        [MaybeNull] public string[] Attributes { get; }

        public FunctionPrototypeAstNode(FunctionParameter[] parameters, string name,
            FunctionParameter.ParamType returnType, string[] attrs) {
            Parameters = parameters;
            Name = name;
            ReturnType = returnType;
            Attributes = attrs;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            if (Attributes != null) {
                foreach (var attribute in Attributes) {
                    sb.Append(attribute);
                    sb.Append('\n');
                }
            }

            sb.Append(ReturnType);
            sb.Append(' ');
            sb.Append(Name);

            if (Parameters.Length == 0) {
                return $"{sb} ()";
            }

            sb.Append(" (");
            sb.Append(Parameters[0]);
            if (Parameters.Length == 1) {
                return $"{sb})";
            }

            for (var i = 1; i < Parameters.Length; i++) {
                sb.Append(", ");
                sb.Append(Parameters[i]);
            }

            sb.Append(')');

            return sb.ToString();
        }

        public bool IsImported => Attributes != null && Attributes.Contains("@import");

        public override bool Equals(object? obj) {
            return Equals(obj as FunctionPrototypeAstNode);
        }

        public bool Equals(FunctionPrototypeAstNode other) {
            if (Name != other.Name || ReturnType != other.ReturnType) {
                return false;
            }

            if (Parameters.Length != other.Parameters.Length) {
                return false;
            }

            if (Parameters.Where((param, i) => param.Type != other.Parameters[i].Type)
                .Any()) {
                return false;
            }

            if (Attributes == null && other.Attributes == null) {
                return true;
            }

            if (Attributes == null || other.Attributes == null) {
                return false;
            }

            var attr = new List<string>(Attributes);
            var otherAttr = new List<string>(other.Attributes);
            attr.Sort();
            otherAttr.Sort();
            return attr.SequenceEqual(otherAttr);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Parameters, Name, (int) ReturnType, Attributes);
        }
    }


    public class CodeBlockAstNode : BasicAstNode, IEquatable<CodeBlockAstNode> {
        public BasicAstNode[] Statements;

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
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Statements.Length != other.Statements.Length) {
                return false;
            }

            return !Statements.Where((t, i) => !t.Equals(other.Statements[i])).Any();
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CodeBlockAstNode) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Statements);
        }
    }

    public class FunctionAstNode : BasicAstNode, IEquatable<FunctionAstNode> {
        public FunctionPrototypeAstNode Prototype { get; }
        public CodeBlockAstNode Body { get; }

        public FunctionAstNode(FunctionPrototypeAstNode prototype,
            CodeBlockAstNode body) {
            Prototype = prototype;
            Body = body;
        }

        public bool IsImported => Prototype.IsImported;

        public override string ToString() {
            var sb = new StringBuilder(Prototype.ToString());

            if (IsImported) {
                return sb.ToString();
            }

            sb.Append(" {\n");
            sb.Append(Body);
            sb.Append('}');
            return sb.ToString();
        }

        public bool Equals(FunctionAstNode other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Prototype, other.Prototype) &&
                   Equals(Body, other.Body);
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FunctionAstNode) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Prototype, Body);
        }
    }

    public class
        FunctionCallingAstNode : BasicAstNode, IEquatable<FunctionCallingAstNode> {
        public string Callee { get; }
        public BasicAstNode[] Arguments { get; }

        public FunctionCallingAstNode(string prototype,
            BasicAstNode[] args) {
            Callee = prototype;
            Arguments = args;
        }

        public override string ToString() {
            if (Arguments.Length == 0) {
                return $"{Callee}()";
            }

            var sb = new StringBuilder(Callee);
            sb.Append('(');
            foreach (var arg in Arguments) {
                sb.Append(arg);
                sb.Append(", ");
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(')');
            return sb.ToString();
        }

        public bool Equals(FunctionCallingAstNode other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Callee == other.Callee &&
                   Equals(Arguments, other.Arguments);
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FunctionCallingAstNode) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Callee, Arguments);
        }
    }

    public class ReturnAstNode : BasicAstNode, IEquatable<ReturnAstNode> {
        public BasicAstNode ReturnValue;

        public ReturnAstNode(BasicAstNode rv) {
            ReturnValue = rv;
        }

        public override string ToString() {
            return $"return {ReturnValue}";
        }

        public bool Equals(ReturnAstNode other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ReturnValue, other.ReturnValue);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReturnAstNode) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), ReturnValue);
        }
    }
}
