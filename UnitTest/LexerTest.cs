using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubbishLanguageFrontEnd.Lexer;
using UnitTest.TestHelper;

namespace UnitTest {
    using T = TokenHelper;

    [TestClass]
    public class LexerTest {
        [TestMethod]
        public void TestParseToken1() {
            Token[] expectedTokens = {
                T.Void(3, 1),
                T.Token(TokenType.Identifier, "modPlus", 3, 6),
                T.LeftParenthesis(3, 13),
                T.I64(3, 13),
                T.Token(TokenType.Identifier, "a", 3, 18),
                T.Comma(3, 19),
                T.I64(3, 21),
                T.Token(TokenType.Identifier, "m", 3, 25),
                T.RightParenthesis(3, 26),
                T.LeftBrace(3, 18),
                T.If(4, 1),
                T.LeftParenthesis(4, 4),
                T.Token(TokenType.Identifier, "a", 4, 5),
                T.Eq(4, 7),
                T.Token(TokenType.ValInt, "1", 4, 10),
                T.RightParenthesis(4, 11),
                T.Return(4, 12),
                T.Token(TokenType.Identifier, "a", 4, 19),
                T.Semi(4, 20),
                T.Return(5, 5),
                T.Token(TokenType.Identifier, "a", 5, 12),
                T.Mod(5, 13),
                T.Token(TokenType.Identifier, "m", 5, 15),
                T.Semi(5, 16),
                T.RightBrace(6, 1),
                T.Token(TokenType.Attr, "@import", 8, 1),
                T.Func(8, 11),
                T.I64(8, 16),
                T.Token(TokenType.Identifier, "veryNBFunction", 8, 20),
                T.LeftParenthesis(8, 36),
                T.I64(8, 37),
                T.Token(TokenType.Identifier, "a", 8, 41),
                T.Comma(8, 42),
                T.F64(8, 43),
                T.Token(TokenType.Identifier, "b", 8, 47),
                T.Comma(8, 48),
                T.Str(8, 49),
                T.Token(TokenType.Identifier, "c", 8, 53),
                T.RightParenthesis(8, 56),
                T.Semi(8, 57),
                T.Token(TokenType.Attr, "@entry", 10, 1),
                T.Func(11, 1),
                T.I64(11, 6),
                T.Token(TokenType.Identifier, "main", 11, 10),
                T.LeftParenthesis(11, 14),
                T.I64(11, 15),
                T.Token(TokenType.Identifier, "argc", 11, 19),
                T.Comma(11, 23),
                T.Str(11, 25),
                T.LeftBracket(11, 28),
                T.RightBracket(11, 29),
                T.Token(TokenType.Identifier, "argv", 11, 31),
                T.RightParenthesis(11, 35),
                T.LeftBrace(11, 37),
                T.Token(TokenType.Identifier, "veryNBFunction", 12, 5),
                T.LeftParenthesis(12, 19),
                T.Token(TokenType.Identifier, "a", 12, 20),
                T.Comma(12, 21),
                T.Token(TokenType.ValFloat, "114.514", 12, 22),
                T.Comma(12, 29),
                T.Token(TokenType.ValStr, "\"heng-aaaaaaa\"", 12, 31),
                T.RightParenthesis(12, 45),
                T.Semi(12, 46),
                T.Return(13, 5),
                T.Token(TokenType.ValInt, "0", 13, 12),
                T.Semi(13, 13),
                T.RightBrace(13, 14)
            };
            using var source = new FileStream("TestCode/TestLex1.rbl", FileMode.Open);
            var lexer = new RbLexer(source);
            TestCodeHelper.ParseTokenCode(expectedTokens, lexer);
        }

        [TestMethod]
        public void TestParseToken2() {
            Token[] expectedTokens = {
                T.Token(TokenType.ValInt, "12345678", 1, 1),
                T.Token(TokenType.ValInt, "0x1234ABCD", 2, 1),
                T.Token(TokenType.ValInt, "0xABCDEFGH", 3, 1),
                T.Token(TokenType.ValInt, "0x123.456", 4, 1),
                T.I64(6, 1),
                T.Token(TokenType.Identifier, "a", 6, 5),
                T.Assign(6, 7),
                T.Token(TokenType.ValInt, "0xcdcdcdcd", 6, 9),
                T.F64(7, 1),
                T.Token(TokenType.Identifier, "b", 7, 5),
                T.Assign(7, 7),
                T.Token(TokenType.ValFloat, ".0", 7, 8),
                T.F64(8, 1),
                T.Token(TokenType.Identifier, "c", 8, 5),
                T.Assign(8, 6),
                T.Token(TokenType.Unknown, "1.", 8, 8),
                T.Token(TokenType.Identifier, "abcdefg", 9, 1)
            };
            using var source = new FileStream("TestCode/TestLex2.rbl", FileMode.Open);
            var lexer = new RbLexer(source);
            TestCodeHelper.ParseTokenCode(expectedTokens, lexer);
        }
    }
}
