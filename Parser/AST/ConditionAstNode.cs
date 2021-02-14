using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RubbishLanguageFrontEnd.Parser.AST {
    public class IfElseAstNode : BasicAstNode, IEquatable<IfElseAstNode> {
        public BasicAstNode IfCondition { get; }
        public CodeBlockAstNode IfBlock { get; }
        [MaybeNull] public CodeBlockAstNode ElseBlock { get; }

        public IfElseAstNode(BasicAstNode cond, CodeBlockAstNode ifBlock,
            CodeBlockAstNode elseBlock) {
            IfCondition = cond;
            IfBlock = ifBlock;
            ElseBlock = elseBlock;
        }

        public override string ToString() {
            var sb = new StringBuilder("if (");
            sb.Append(IfCondition);
            sb.Append(") {\n");
            sb.Append(IfBlock);
            sb.Append('}');
            if (ElseBlock == null)
                return sb.ToString();
            sb.Append(" else {\n");
            sb.Append(ElseBlock);
            sb.Append('}');

            return sb.ToString();
        }

        public bool Equals(IfElseAstNode other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(IfCondition, other.IfCondition) &&
                   Equals(IfBlock, other.IfBlock) && Equals(ElseBlock, other.ElseBlock);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IfElseAstNode) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), IfCondition, IfBlock, ElseBlock);
        }
    }
}
