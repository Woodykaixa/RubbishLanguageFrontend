#nullable enable
using RubbishLanguageFrontEnd.Util;

namespace RubbishLanguageFrontEnd.AST {
    public class BasicAstNode : IVisitable {
        public void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override bool Equals(object? obj) {
            return EqualCheckUtil.NotNullAndSameType(this, obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
