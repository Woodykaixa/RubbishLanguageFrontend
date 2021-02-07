using RubbishLanguageFrontEnd.Lexer;

namespace UnitTest.TestHelper {
    internal static class TokenHelper {
        public static Token Token(TokenType type, string value, ulong line, ulong col) {
            return new Token(type, value, line, col);
        }

        public static Token Void(ulong line, ulong col) {
            return Token(TokenType.Void, "void", line, col);
        }

        public static Token I64(ulong line, ulong col) {
            return Token(TokenType.Int64, "i64", line, col);
        }

        public static Token F64(ulong line, ulong col) {
            return Token(TokenType.Float64, "f64", line, col);
        }

        public static Token Str(ulong l, ulong c) {
            return Token(TokenType.Str, "str", l, c);
        }

        public static Token LeftParenthesis(ulong l, ulong c) {
            return Token(TokenType.LeftParenthesis, "(", l, c);
        }

        public static Token RightParenthesis(ulong l, ulong c) {
            return Token(TokenType.RightParenthesis, ")", l, c);
        }

        public static Token LeftBracket(ulong l, ulong c) {
            return Token(TokenType.LeftBracket, "[", l, c);
        }

        public static Token RightBracket(ulong l, ulong c) {
            return Token(TokenType.RightBracket, "]", l, c);
        }

        public static Token LeftBrace(ulong l, ulong c) {
            return Token(TokenType.LeftBrace, "{", l, c);
        }

        public static Token RightBrace(ulong l, ulong c) {
            return Token(TokenType.RightBrace, "}", l, c);
        }

        public static Token Comma(ulong l, ulong c) {
            return Token(TokenType.Comma, ",", l, c);
        }

        public static Token Semi(ulong l, ulong c) {
            return Token(TokenType.Semi, ";", l, c);
        }

        public static Token If(ulong l, ulong c) {
            return Token(TokenType.If, "if", l, c);
        }

        public static Token Else(ulong l, ulong c) {
            return Token(TokenType.Else, "else", l, c);
        }

        public static Token Return(ulong l, ulong c) {
            return Token(TokenType.Return, "return", l, c);
        }

        public static Token Func(ulong l, ulong c) {
            return Token(TokenType.Func, "func", l, c);
        }

        public static Token Eq(ulong l, ulong c) {
            return Token(TokenType.OpEq, "==", l, c);
        }

        public static Token Plus(ulong l, ulong c) {
            return Token(TokenType.OpPlus, "+", l, c);
        }

        public static Token Minus(ulong l, ulong c) {
            return Token(TokenType.OpMinus, "-", l, c);
        }

        public static Token Mod(ulong l, ulong c) {
            return Token(TokenType.OpMod, "%", l, c);
        }

        public static Token Assign(ulong l, ulong c) {
            return Token(TokenType.OpAssign, "=", l, c);
        }
    }
}
