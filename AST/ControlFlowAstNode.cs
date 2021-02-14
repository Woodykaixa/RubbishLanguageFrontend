#nullable enable
using RubbishLanguageFrontEnd.Util;
using System;
using System.Text;

namespace RubbishLanguageFrontEnd.AST {
    public class IfElseAstNode : BasicAstNode {
        public BasicAstNode IfCondition { get; }
        public CodeBlockAstNode IfBlock { get; }
        public CodeBlockAstNode? ElseBlock { get; }

        public IfElseAstNode(BasicAstNode cond, CodeBlockAstNode ifBlock,
            CodeBlockAstNode? elseBlock) {
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
            return Equals(IfCondition, other.IfCondition) &&
                   Equals(IfBlock, other.IfBlock) &&
                   Equals(ElseBlock, other.ElseBlock);
        }

        public override bool Equals(object? obj) {
            return EqualCheckUtil.NotNullAndSameType(this, obj) &&
                   Equals((IfElseAstNode) obj!);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), IfCondition, IfBlock, ElseBlock);
        }
    }

    public class LoopAstNode : BasicAstNode {
        public BasicAstNode Condition { get; }
        public CodeBlockAstNode Body { get; }

        public LoopAstNode(BasicAstNode condition, CodeBlockAstNode body) {
            Condition = condition;
            Body = body;
        }

        public bool Equals(LoopAstNode other) {
            return Equals(Condition, other.Condition) &&
                   Equals(Body, other.Body);
        }

        public override bool Equals(object? obj) {
            return EqualCheckUtil.NotNullAndSameType(this, obj) &&
                   Equals((LoopAstNode) obj!);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Condition, Body);
        }

        public override string ToString() {
            var sb = new StringBuilder("loop (");
            sb.Append(Condition);
            sb.Append(") {\n\t");
            sb.Append(Body);
            sb.Append('}');
            return sb.ToString();
        }
    }
}
