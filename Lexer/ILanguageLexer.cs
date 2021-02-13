namespace RubbishLanguageFrontEnd.Lexer {
    public interface ILanguageLexer {
        public Token NextToken();
        public Token[] ParseTokens();
    }
}
