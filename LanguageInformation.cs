using System.Collections.Generic;
using RubbishLanguageFrontEnd.Lexer;

namespace RubbishLanguageFrontEnd {
    public class LanguageInformation {
        public static readonly IReadOnlyCollection<string> PrimitiveTypes = new[]
            {"i64", "f64", "str"};

        public static readonly IReadOnlyCollection<string> UnaryOperator =
            new[] {"address_of", "not"};

        public static readonly IReadOnlyCollection<string> BinaryOperator = new[] {
            "+", "-", "*", "/", "%", "and", "or", "==", "<", "<=", "=", ">", ">="
        };

        private static LanguageInformation _instance;

        public static LanguageInformation Instance =>
            _instance ??= new LanguageInformation();

        public readonly Dictionary<string, TokenType> KeywordAndMultiCharOperatorMapping;

        public readonly Dictionary<string, TokenType> SingleCharMapping;

        private static TokenType PrimitiveTypeToTokenType(string primitive) {
            return primitive switch {
                "i64" => TokenType.Int64,
                "f64" => TokenType.Float64,
                "str" => TokenType.Str,
                _ => TokenType.Unknown
            };
        }

        private LanguageInformation() {
            KeywordAndMultiCharOperatorMapping = new Dictionary<string, TokenType> {
                {"void", TokenType.Void},
                {"if", TokenType.If},
                {"else", TokenType.Else},
                {"elif", TokenType.Elif},
                {"loop", TokenType.Loop},
                {"break", TokenType.Break},
                {"continue", TokenType.Continue},
                {"and", TokenType.And},
                {"or", TokenType.Or},
                {"not", TokenType.Not},
                {"address_of", TokenType.OpAddress},
                {"<=", TokenType.OpLe},
                {"==", TokenType.OpEq},
                {">=", TokenType.OpGe},
                {"func", TokenType.Func},
                {"return", TokenType.Return},
            };
            foreach (var type in PrimitiveTypes) {
                KeywordAndMultiCharOperatorMapping[type] = PrimitiveTypeToTokenType(type);
            }

            SingleCharMapping = new Dictionary<string, TokenType> {
                {"+", TokenType.OpPlus},
                {"-", TokenType.OpMinus},
                {"*", TokenType.OpStar},
                {"/", TokenType.OpDiv},
                {"%", TokenType.OpMod},
                {"<", TokenType.OpLt},
                {">", TokenType.OpGt},
                {"=", TokenType.OpAssign},
                {"(", TokenType.LeftParenthesis},
                {")", TokenType.RightParenthesis},
                {"[", TokenType.LeftBracket},
                {"]", TokenType.RightBracket},
                {"{", TokenType.LeftBrace},
                {"}", TokenType.RightBrace},
                {";", TokenType.Semi},
                {":", TokenType.Colon},
                {",", TokenType.Comma}
            };
        }
    }
}
