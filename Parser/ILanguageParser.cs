using RubbishLanguageFrontEnd.AST;

namespace RubbishLanguageFrontEnd.Parser {
    public interface ILanguageParser {
        public bool HasError { get; }
        public BasicAstNode Parse();
        void PrintError();
    }
}
