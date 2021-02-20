using RubbishLanguageFrontEnd.AST;

namespace RubbishLanguageFrontEnd.Util {
    public interface IVisitable {
        public void Accept(IAstVisitor astVisitor);
    }

    public interface IAstVisitor {
        public void Visit(BasicAstNode visitable);
        public void Visit(IntegerAstNode visitable);
        public void Visit(FloatAstNode visitable);
        public void Visit(StringAstNode visitable);
        public void Visit(IdentifierAstNode visitable);
        public void Visit(VariableDefineAstNode visitable);
        public void Visit(UnaryOperatorAstNode visitable);
        public void Visit(BinaryOperatorAstNode visitable);
        public void Visit(CodeBlockAstNode visitable);
        public void Visit(IfElseAstNode visitable);
        public void Visit(LoopAstNode visitable);
        public void Visit(ReturnAstNode visitable);
        public void Visit(BreakAstNode visitable);
        public void Visit(ContinueAstNode visitable);
        public void Visit(FunctionAstNode visitable);
        public void Visit(FunctionCallingAstNode visitable);

    }
}
