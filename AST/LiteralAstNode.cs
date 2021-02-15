using System;
using System.Collections.Generic;
using RubbishLanguageFrontEnd.Lexer;

namespace RubbishLanguageFrontEnd.AST {
    public class LiteralAstNode<T> : BasicAstNode, IEquatable<LiteralAstNode<T>> {
        public T Value { get; }

        public LiteralAstNode(T value) {
            Value = value;
        }

        public override string ToString() {
            return Value.ToString();
        }


        public bool Equals(LiteralAstNode<T> other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj) {
            return ReferenceEquals(this, obj) ||
                   obj is LiteralAstNode<T> other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Value);
        }
    }

    public class IntegerAstNode : LiteralAstNode<long> {
        public IntegerAstNode(long value) : base(value) { }

        public static IntegerAstNode FromToken(Token token) {
            var success = long.TryParse(token.Value, out var value);
            if (success) {
                return new IntegerAstNode(value);
            }

            throw new ParseIntegerException(token);
        }
    }

    public class FloatAstNode : LiteralAstNode<double> {
        public FloatAstNode(double value) : base(value) { }

        public static FloatAstNode FromToken(Token token) {
            var success = double.TryParse(token.Value, out var value);
            if (success) {
                return new FloatAstNode(value);
            }

            throw new ParseFloatException(token);
        }
    }

    public class StringAstNode : LiteralAstNode<string> {
        public StringAstNode(string value) : base(value) { }
    }
}
