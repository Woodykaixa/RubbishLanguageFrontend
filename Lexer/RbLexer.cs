using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using RubbishLanguageFrontEnd.Util;
using RubbishLanguageFrontEnd.Util.Logging;
using RubbishLanguageFrontEnd.Util.SourceReader;

namespace RubbishLanguageFrontEnd.Lexer {
    public class RbLexer : ILanguageLexer {
        private static readonly Dictionary<string, TokenType>
            KeywordsAndMultiCharOperators = LanguageInformation.Instance
                .KeywordAndMultiCharOperatorMapping;

        private static readonly Dictionary<string, TokenType> SingleSymbolTokenMap =
            LanguageInformation.Instance.SingleCharMapping;

        private readonly ProxySourceReader _psReader;

        public RbLexer(FileStream inputFile) {
            _psReader = new ProxySourceReader(inputFile);
        }

        private Token BuildToken(string value, TokenType type) {
            return new Token(type, value, _psReader.CurrentLine,
                _psReader.CurrentColumn);
        }

        public Token NextToken() {
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


            if (Regex.IsMatch(current, @"^((0x.+)|(\d+))$")) {
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

            if (Regex.IsMatch(current, "^[a-zA-Z_][a-zA-Z0-9_]*$")) {
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

        public Token[] ParseTokens() {
            return Parse();
        }
    }
}
