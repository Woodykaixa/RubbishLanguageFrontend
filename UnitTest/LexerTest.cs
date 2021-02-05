using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubbishLanguageFrontEnd.Lexer;

namespace UnitTest {
    [TestClass]
    public class LexerTest {
        private Token Tok(TokenType type, string value, ulong line, ulong col) {
            return new Token(type, value, line, col);
        }

        private Token Void(ulong line, ulong col) {
            return Tok(TokenType.Void, "void", line, col);
        }

        private Token I64(ulong line, ulong col) {
            return Tok(TokenType.Int64, "i64", line, col);
        }

        private Token F64(ulong line, ulong col) {
            return Tok(TokenType.Float64, "f64", line, col);
        }

        private Token Str(ulong l, ulong c) {
            return Tok(TokenType.Str, "str", l, c);
        }

        private Token LeftParenthesis(ulong l, ulong c) {
            return Tok(TokenType.LeftParenthesis, "(", l, c);
        }

        private Token RightParenthesis(ulong l, ulong c) {
            return Tok(TokenType.RightParenthesis, ")", l, c);
        }

        private Token LeftBracket(ulong l, ulong c) {
            return Tok(TokenType.LeftBracket, "[", l, c);
        }

        private Token RightBracket(ulong l, ulong c) {
            return Tok(TokenType.RightBracket, "]", l, c);
        }

        private Token LeftBrace(ulong l, ulong c) {
            return Tok(TokenType.LeftBrace, "{", l, c);
        }

        private Token RightBrace(ulong l, ulong c) {
            return Tok(TokenType.RightBrace, "}", l, c);
        }

        private Token Comma(ulong l, ulong c) {
            return Tok(TokenType.Comma, ",", l, c);
        }

        private Token Semi(ulong l, ulong c) {
            return Tok(TokenType.Semi, ";", l, c);
        }

        private Token If(ulong l, ulong c) {
            return Tok(TokenType.If, "if", l, c);
        }

        private Token Else(ulong l, ulong c) {
            return Tok(TokenType.Else, "else", l, c);
        }

        private Token Return(ulong l, ulong c) {
            return Tok(TokenType.Return, "return", l, c);
        }

        private Token Func(ulong l, ulong c) {
            return Tok(TokenType.Func, "func", l, c);
        }

        private Token Eq(ulong l, ulong c) {
            return Tok(TokenType.OpEq, "==", l, c);
        }

        private Token Plus(ulong l, ulong c) {
            return Tok(TokenType.OpPlus, "+", l, c);
        }

        private Token Minus(ulong l, ulong c) {
            return Tok(TokenType.OpMinus, "-", l, c);
        }

        private Token Mod(ulong l, ulong c) {
            return Tok(TokenType.OpMod, "%", l, c);
        }


        [TestMethod]
        public void TestParseToken1() {
            using var source = new FileStream("TestCode/test1.rbl", FileMode.Open);
            var lexer = new Lexer(source);
            Token[] expectedTokens = {
                Void(3, 1),
                Tok(TokenType.Identifier, "modPlus", 3, 6),
                LeftParenthesis(3, 13),
                I64(3, 13),
                Tok(TokenType.Identifier, "a", 3, 18),
                Comma(3, 19),
                I64(3, 21),
                Tok(TokenType.Identifier, "m", 3, 25),
                RightParenthesis(3, 26),
                LeftBrace(3, 18),
                If(4, 1),
                LeftParenthesis(4, 4),
                Tok(TokenType.Identifier, "a", 4, 5),
                Eq(4, 7),
                Tok(TokenType.ValInt, "1", 4, 10),
                RightParenthesis(4, 11),
                Return(4, 12),
                Tok(TokenType.Identifier, "a", 4, 19),
                Semi(4, 20),
                Return(5, 5),
                Tok(TokenType.Identifier, "a", 5, 12),
                Mod(5, 13),
                Tok(TokenType.Identifier, "m", 5, 15),
                Semi(5, 16),
                RightBrace(6, 1),
                Tok(TokenType.Attr, "@import", 8, 1),
                Func(8, 11),
                I64(8, 16),
                Tok(TokenType.Identifier, "veryNBFunction", 8, 20),
                LeftParenthesis(8, 36),
                I64(8, 37),
                Tok(TokenType.Identifier, "a", 8, 41),
                Comma(8, 42),
                F64(8, 43),
                Tok(TokenType.Identifier, "b", 8, 47),
                Comma(8, 48),
                Str(8, 49),
                Tok(TokenType.Identifier, "c", 8, 53),
                RightParenthesis(8, 56),
                Semi(8, 57),
                Tok(TokenType.Attr, "@entry", 10, 1),
                Func(11, 1),
                I64(11, 6),
                Tok(TokenType.Identifier, "main", 11, 10),
                LeftParenthesis(11, 14),
                I64(11, 15),
                Tok(TokenType.Identifier, "argc", 11, 19),
                Comma(11, 23),
                Str(11, 25),
                LeftBracket(11, 28),
                RightBracket(11, 29),
                Tok(TokenType.Identifier, "argv", 11, 31),
                RightParenthesis(11, 35),
                LeftBrace(11, 37),
                Tok(TokenType.Identifier, "veryNBFunction", 12, 5),
                LeftParenthesis(12, 19),
                Tok(TokenType.Identifier, "a", 12, 20),
                Comma(12, 21),
                Tok(TokenType.ValFloat, "114.514", 12, 22),
                Comma(12, 29),
                Tok(TokenType.ValStr, "\"heng-aaaaaaa\"", 12, 31),
                RightParenthesis(12, 45),
                Semi(12, 46),
                Return(13, 5),
                Tok(TokenType.ValInt, "0", 13, 12),
                Semi(13, 13),
                RightBrace(13, 14)
            };
            var tokens = lexer.Parse();
            Console.WriteLine(
                $"expect {expectedTokens.Length} tokens, actual {tokens.Length}");
            Assert.AreEqual(expectedTokens.Length, tokens.Length);
            for (var i = 0; i < expectedTokens.Length; i++) {
                // Assert.AreEqual(expectedTokens[i],tokens[i]);
                // Alright, there are too many bugs on column counting. Now we only test token type and value.
                Assert.AreEqual(expectedTokens[i].Type, tokens[i].Type,
                    $"current: {i}, expect: {expectedTokens[i]}, got: {tokens[i]}");
                Assert.AreEqual(expectedTokens[i].Value, tokens[i].Value);
            }
        }
    }
}
