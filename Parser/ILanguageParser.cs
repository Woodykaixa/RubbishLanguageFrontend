using RubbishLanguageFrontEnd.Parser.AST;

namespace RubbishLanguageFrontEnd.Parser {
    public interface ILanguageParser {
        public bool HasError { get; }
        public BasicAstNode Parse();
    }
}
