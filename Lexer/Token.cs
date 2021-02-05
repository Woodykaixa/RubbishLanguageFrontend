using System;

namespace RubbishLanguageFrontEnd.Lexer {
    public enum TokenType {
        Eof, // end of file

        ValInt, // i64 literals
        ValFloat, // f64 literals
        ValStr, // string literals
        Identifier,

        Int64, // i64
        Float64, // f64
        Char, // char
        Str, // str
        Void,

        If,
        Else,
        Elif,
        Loop,
        Break,
        Continue,
        And,
        Or,
        Not,

        OpPlus,
        OpMinus,
        OpStar, // *
        OpDiv,
        OpMod, // %
        OpAddress, // &
        OpLt, // <
        OpLe, // <=
        OpEq, // ==
        OpGt, // >
        OpGe, // >=
        OpAssign, // =
        LeftParenthesis, // (
        RightParenthesis, // )
        LeftBracket, // [
        RightBracket, // ]
        LeftBrace, // {
        RightBrace, // }
        Semi, // ;
        Colon, // :
        Comma, // ,

        Func,
        Return,
        Import,

        Attr, // @.+

        Unknown
    }

    public class Token {
        public TokenType Type { get; }
        public string Value { get; }
        public ulong SourceLineNumber { get; }
        public ulong SourceColumnNumber { get; }

        public Token(TokenType type, string value, ulong line, ulong col) {
            Type = type;
            Value = value;
            SourceLineNumber = line;
            SourceColumnNumber = col;
        }

        public override string ToString() {
            return
                $"Type: {Type}, Value: {Value}({Value.Length}), Line: {SourceLineNumber}, Col: {SourceColumnNumber}";
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            return obj.GetType() == this.GetType() && Equals(obj as Token);
        }

        protected bool Equals(Token other) {
            return Type == other.Type && Value == other.Value &&
                   SourceLineNumber == other.SourceLineNumber &&
                   SourceColumnNumber == other.SourceColumnNumber;
        }

        public override int GetHashCode() {
            return HashCode.Combine((int) Type, Value, SourceLineNumber,
                SourceColumnNumber);
        }
    }
}
