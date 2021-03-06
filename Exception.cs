using System;
using System.Collections.Generic;
using System.Text;
using RubbishLanguageFrontEnd.Lexer;

namespace RubbishLanguageFrontEnd {
    internal class UnexpectedEndOfArgsException : Exception {
        public string CurrentArg;

        public UnexpectedEndOfArgsException(string current) {
            CurrentArg = current;
        }

        public override string ToString() {
            return $"Unexpected end of argument: {CurrentArg}, expect more arguments";
        }
    }

    internal class ParseTokenException : Exception {
        public Token ErrorToken { get; }

        public ParseTokenException(Token token) {
            ErrorToken = token;
        }

        public override string Message => $"Line: {ErrorToken.SourceLineNumber}";
    }

    internal class ParseIntegerException : ParseTokenException {
        public ParseIntegerException(Token token) : base(token) { }

        public override string Message =>
            $"{ErrorToken.Value} is not an integer. {base.Message}";
    }

    internal class ParseFloatException : ParseTokenException {
        public ParseFloatException(Token token) : base(token) { }

        public override string Message =>
            $"{ErrorToken.Value} is not a float point number. {base.Message}";
    }

    internal class UnexpectedTokenException : ParseTokenException {
        public TokenType[] Expect { get; }

        public UnexpectedTokenException(TokenType expect, Token got) : base(got) {
            Expect = new[] {expect};
        }

        public UnexpectedTokenException(TokenType[] expect, Token got) : base(got) {
            Expect = expect;
        }

        public override string Message {
            get {
                if (Expect.Length == 1) {
                    return
                        $"Unexpected token. Expect {Expect[0]}, got {ErrorToken.Type}. " +
                        base.Message;
                }

                var sb = new StringBuilder("one of [");
                sb.Append(Expect[0]);
                for (int i = 1; i < Expect.Length; i++) {
                    sb.Append(", ");
                    sb.Append(Expect[i]);
                }

                sb.Append("].");
                return $"Unexpected token. Expect {sb}, got {ErrorToken.Type}. " +
                       base.Message;
            }
        }
    }

    internal class SemanticException : Exception {

        public SemanticException() { }

        public SemanticException(string message) {
            Message = message;
        }
        public override string Message { get; }

    }

    internal class UnknownTypeException : SemanticException {
        public string Typename { get; }

        public UnknownTypeException(string typename) {
            Typename = typename;
        }

        public override string Message => $"unknown type: {Typename}";
    }

    internal class UndefinedVariableException : SemanticException {
        public string VarName { get; }

        public UndefinedVariableException(string variableName) {
            VarName = variableName;
        }

        public override string Message => $"undefined variable: {VarName}";
    }

    internal class TypeRedefineException : SemanticException {
        public string TypeName { get; }

        public TypeRedefineException(string typename) {
            TypeName = typename;
        }

        public override string Message => $"type redefined: {TypeName}";
    }

    internal class VariableRedefineException : SemanticException {
        public string VarName { get; }

        public VariableRedefineException(string variableName) {
            VarName = variableName;
        }

        public override string Message => $"variable redefined: {VarName}";
    }

    internal class WrongTypeException : SemanticException {
        public override string Message { get; }

        public WrongTypeException(string message) {
            Message = message;
        }
    }
}
