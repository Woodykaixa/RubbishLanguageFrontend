#nullable enable
using System;
using RubbishLanguageFrontEnd.Util;

namespace RubbishLanguageFrontEnd.AST {
    public class KeywordAstNode : BasicAstNode {
        public override bool Equals(object? obj) {
            return EqualCheckUtil.NotNullAndSameType(this, obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }

    public class BreakAstNode : KeywordAstNode {
        public override string ToString() {
            return "break";
        }

        public override bool Equals(object? obj) {
            return EqualCheckUtil.NotNullAndSameType(this, obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }

    public class ContinueAstNode : KeywordAstNode {
        public override string ToString() {
            return "continue";
        }

        public override bool Equals(object? obj) {
            return EqualCheckUtil.NotNullAndSameType(this, obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
