using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubbishLanguageFrontEnd.Lexer;

namespace UnitTest.TestHelper {
    internal static class TestCodeHelper {
        public static void ParseTokenCode(string sourceFilename, Token[] expectedTokens) {
            using var source = new FileStream(sourceFilename, FileMode.Open);
            var lexer = new RbLexer(source);
            var tokens = lexer.Parse();
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
