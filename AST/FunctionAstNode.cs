#nullable enable
using RubbishLanguageFrontEnd.Util;
using System;
using System.Linq;
using System.Text;

namespace RubbishLanguageFrontEnd.AST {
    using EqUtil = EqualCheckUtil;

    public record FunctionParameter {
        public string Type { get; }
        public string Name { get; }

        public FunctionParameter(string type, string name) {
            Type = type;
            Name = name;
        }

        public override string ToString() => $"{Type} {Name}";
    }

    public class FunctionPrototypeAstNode {
        public FunctionParameter[] Parameters { get; }
        public string Name { get; }
        public string ReturnType { get; }
        public string[]? Attributes { get; }

        public FunctionPrototypeAstNode(FunctionParameter[] parameters, string name,
            string returnType, string[]? attrs) {
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

        public bool IsImported => Attributes?.Contains("@import") ?? false;

        public override bool Equals(object? obj) {
            return EqUtil.NotNullAndSameType(this, obj) &&
                   Equals((FunctionPrototypeAstNode) obj!);
        }

        public bool Equals(FunctionPrototypeAstNode other) {
            if (Name != other.Name || ReturnType != other.ReturnType) {
                return false;
            }

            return Name == other.Name && ReturnType == other.ReturnType &&
                   EqUtil.EnumerableEqual(Parameters, other.Parameters) &&
                   EqUtil.SortedEnumerableEqual(Attributes, other.Attributes);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Parameters, Name, ReturnType, Attributes);
        }
    }

    public class FunctionAstNode : BasicAstNode {
        public FunctionPrototypeAstNode Prototype { get; }
        public CodeBlockAstNode? Body { get; }

        public FunctionAstNode(FunctionPrototypeAstNode prototype,
            CodeBlockAstNode? body) {
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
            return Equals(Prototype, other.Prototype) &&
                   Equals(Body, other.Body);
        }

        public override bool Equals(object? obj) {
            return EqUtil.NotNullAndSameType(this, obj) &&
                   Equals((FunctionAstNode) obj!);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Prototype, Body);
        }
    }

    public class FunctionCallingAstNode : BasicAstNode {
        public string Callee { get; }
        public BasicAstNode[]? Arguments { get; }

        public FunctionCallingAstNode(string prototype,
            BasicAstNode[]? args) {
            Callee = prototype;
            Arguments = args;
        }

        public override string ToString() {
            if (Arguments is null || Arguments.Length == 0) {
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
            return Callee == other.Callee &&
                   EqUtil.EnumerableEqual(Arguments, other.Arguments);
        }

        public override bool Equals(object? obj) {
            return EqUtil.NotNullAndSameType(this, obj) &&
                   Equals((FunctionCallingAstNode) obj!);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Callee, Arguments);
        }
    }
}
