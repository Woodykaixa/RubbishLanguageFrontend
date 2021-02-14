using System;

namespace RubbishLanguageFrontEnd.Parser.AST {
    public class BasicAstNode : IEquatable<BasicAstNode> {
        public bool Equals(BasicAstNode other) {
            return true;
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BasicAstNode) obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
