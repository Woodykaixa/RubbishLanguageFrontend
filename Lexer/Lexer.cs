using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using RubbishLanguageFrontEnd.Util;
using RubbishLanguageFrontEnd.Util.SourceReader;

namespace RubbishLanguageFrontEnd.Lexer {
    public class Lexer {
        private static readonly Dictionary<string, TokenType>
            KeywordsAndMultiCharOperators =
                new Dictionary<string, TokenType> {
                    {"i64", TokenType.Int64},
                    {"f64", TokenType.Float64},
                    {"str", TokenType.Str},
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

        private static readonly Dictionary<string, TokenType> SingleSymbolTokenMap =
            new Dictionary<string, TokenType> {
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

        private readonly ProxySourceReader _psReader;

        public Lexer(FileStream inputFile) {
            _psReader = new ProxySourceReader(inputFile);
        }

        private Token BuildToken(string value, TokenType type) {
            return new Token(type, value, _psReader.CurrentLine,
                _psReader.CurrentColumn);
        }

        private Token NextToken() {
            ProxyReadChar:

            _psReader.Read();
            if (_psReader.CurrentChar.IsEof) {
                return BuildToken("End of file", TokenType.Eof);
            }

            var current = _psReader.CurrentString;
            if (_psReader.CurrentString == "=" && _psReader.LookAheadChar.IsEq) {
                _psReader.Read();
                _psReader.Accept();
                return BuildToken("==", TokenType.OpEq);
            }

            if (current == "") {
                _psReader.Read();
                current = _psReader.CurrentString;
            }

            if (SingleSymbolTokenMap.ContainsKey(current)) {
                _psReader.Accept();
                return BuildToken(current, SingleSymbolTokenMap[current]);
            }

            if (KeywordsAndMultiCharOperators.ContainsKey(current)) {
                _psReader.Accept();
                return BuildToken(current, KeywordsAndMultiCharOperators[current]);
            }

            while (!_psReader.GotSeparator()) {
                _psReader.Read();
            }

            if (Regex.IsMatch(_psReader.CurrentString, "$[\\n\\t]*^")) {
                _psReader.Accept();
                goto ProxyReadChar;
            }

            current = _psReader.CurrentString[..^1];
            if (KeywordsAndMultiCharOperators.ContainsKey(current)) {
                _psReader.AcceptToLastOne();
                return BuildToken(current, KeywordsAndMultiCharOperators[current]);
            }


            if (Regex.IsMatch(current, @"^((0x[0-9A-Fa-f]+)|([0-9]+))$")) {
                _psReader.AcceptToLastOne();

                return BuildToken(current, TokenType.ValInt);
            }

            if (Regex.IsMatch(current, "@[a-zA-Z0-9_]+")) {
                _psReader.AcceptToLastOne();

                return BuildToken(current, TokenType.Attr);
            }

            if (Regex.IsMatch(current, "\"[^\"]*\"")) {
                _psReader.AcceptToLastOne();

                return BuildToken(current, TokenType.ValStr);
            }

            if (Regex.IsMatch(current, "[a-zA-Z_][a-zA-Z0-9_]*")) {
                _psReader.AcceptToLastOne();

                return BuildToken(current, TokenType.Identifier);
            }

            if (Regex.IsMatch(current, @"\d*\.\d+")) {
                _psReader.AcceptToLastOne();

                return BuildToken(current, TokenType.ValFloat);
            }

            _psReader.AcceptToLastOne();
            return BuildToken(current, TokenType.Unknown);
        }

        public Token[] Parse() {
            var tokenList = new List<Token>();
            var logger = Logger.GetByName("rbc-dev.log");
            logger.CopyToStdout = true;
            Token token;
            while ((token = NextToken()).Type != TokenType.Eof) {
                tokenList.Add(token);
                logger.WriteLine(token.ToString());
            }

            return tokenList.ToArray();
        }
    }
}