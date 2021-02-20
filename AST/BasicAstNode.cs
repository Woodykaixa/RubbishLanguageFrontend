#nullable enable
using RubbishLanguageFrontEnd.Util;

namespace RubbishLanguageFrontEnd.AST {
    public class BasicAstNode : IVisitable {
        public void Accept(IAstVisitor astVisitor) {
            astVisitor.Visit(this);
        }

        public override bool Equals(object? obj) {
            return EqualCheckUtil.NotNullAndSameType(this, obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
