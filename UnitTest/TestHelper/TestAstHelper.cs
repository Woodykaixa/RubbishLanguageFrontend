#nullable enable
using System;
using RubbishLanguageFrontEnd.AST;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubbishLanguageFrontEnd.Util;

namespace UnitTest.TestHelper {
    public class TestAstHelper {
        private readonly BasicAstNode _expect;
        private readonly BasicAstNode _actual;

        public TestAstHelper(BasicAstNode expect, BasicAstNode actual) {
            _expect = expect;
            _actual = actual;
        }

        public void Test() {
            TestGeneral(_expect, _actual, "Program");
        }

        public void TestGeneral(BasicAstNode? expect, BasicAstNode? actual,
            string traceMessage) {
            if (expect is null && actual is null) {
                return;
            }

            if (expect is null || actual is null) {
                Assert.AreEqual(expect, actual, traceMessage);
            }

            Assert.AreEqual(expect!.GetType(), actual!.GetType());
            var actualType = actual.GetType();
            switch (expect) {
                case IntegerAstNode expectInt:
                    TestLiteral(expectInt, (LiteralAstNode<long>) actual, traceMessage);
                    break;
                case FloatAstNode expectFloat:
                    TestLiteral(expectFloat, (LiteralAstNode<double>) actual,
                        traceMessage);
                    break;
                case StringAstNode expectStr:
                    TestLiteral(expectStr, (LiteralAstNode<string>) actual, traceMessage);
                    break;
                case IdentifierAstNode expectId:
                    TestIdentifier(expectId, (IdentifierAstNode) actual, traceMessage);
                    break;
                case VariableDefineAstNode expectVar:
                    TestVariableDefine(expectVar, (VariableDefineAstNode) actual,
                        traceMessage);
                    break;
                case UnaryOperatorAstNode expectUOp:
                    TestUnaryOpExp(expectUOp, (UnaryOperatorAstNode) actual,
                        traceMessage);
                    break;
                case BinaryOperatorAstNode expectBOp:
                    TestBinaryOpExp(expectBOp, (BinaryOperatorAstNode) actual,
                        traceMessage);
                    break;
                case CodeBlockAstNode expectCb:
                    TestCodeBlock(expectCb, (CodeBlockAstNode) actual,
                        traceMessage);
                    break;
                case ReturnAstNode expectRet:
                    TestReturn(expectRet, (ReturnAstNode) actual, traceMessage);
                    break;
                case IfElseAstNode expectCond:
                    TestCondition(expectCond, (IfElseAstNode) actual, traceMessage);
                    break;
                case LoopAstNode expectLoop:
                    TestLoop(expectLoop, (LoopAstNode) actual, traceMessage);
                    break;
                case FunctionPrototypeAstNode expectProto:
                    TestPrototype(expectProto, (FunctionPrototypeAstNode) actual,
                        traceMessage);
                    break;
                case FunctionAstNode expectFunc:
                    TestFunction(expectFunc, (FunctionAstNode) actual, traceMessage);
                    break;
                case FunctionCallingAstNode expectCall:
                    TestFunctionCall(expectCall, (FunctionCallingAstNode) actual,
                        traceMessage);
                    break;
                case KeywordAstNode expectKw:
                    TestKeyword(expectKw, (KeywordAstNode) actual, traceMessage);
                    break;
                default:
                    throw new Exception($"Unknown AST type: {actualType}");
            }
        }

        public void TestLiteral<T>(LiteralAstNode<T> expect,
            LiteralAstNode<T> actual, string traceMessage) {
            Assert.AreEqual(expect.Value, actual.Value, traceMessage);
        }


        public void TestIdentifier(IdentifierAstNode expect, IdentifierAstNode actual,
            string traceMessage) {
            Assert.IsNotNull(actual, traceMessage);
            Assert.AreEqual(expect, actual, traceMessage);
        }

        public void TestVariableDefine(VariableDefineAstNode expect,
            VariableDefineAstNode actual, string traceMessage) {
            Assert.IsNotNull(actual, traceMessage);
            Assert.AreEqual(expect.Type, actual.Type, traceMessage);
            Assert.AreEqual(expect.Name, actual.Name, traceMessage);
            TestGeneral(expect.AssignAst, actual.AssignAst, traceMessage + " -> Assign");
        }

        public void TestUnaryOpExp(UnaryOperatorAstNode expect,
            UnaryOperatorAstNode actual, string traceMessage) {
            Assert.AreEqual(expect.Operator, expect.Operator,
                traceMessage + " -> Operator");
            TestGeneral(expect.Expr, actual.Expr, traceMessage + " -> UnaryExpr");
        }

        public void TestBinaryOpExp(BinaryOperatorAstNode expect,
            BinaryOperatorAstNode actual, string traceMessage) {
            Assert.AreEqual(expect.Operator, actual.Operator,
                traceMessage + " -> Operator");
            TestGeneral(expect.Left, actual.Left, traceMessage + " -> Left");
            TestGeneral(expect.Right, actual.Right, traceMessage + " -> Right");
        }

        public void TestCodeBlock(CodeBlockAstNode expect, CodeBlockAstNode actual,
            string traceMessage) {
            Assert.AreEqual(expect.Statements.Length, actual.Statements.Length);
            var len = expect.Statements.Length;
            for (var i = 0; i < len; i++) {
                TestGeneral(expect.Statements[i], actual.Statements[i],
                    $"{traceMessage} -> statements[{i}]");
            }
        }

        public void TestReturn(ReturnAstNode expect, ReturnAstNode actual,
            string traceMessage) {
            TestGeneral(expect.ReturnValue, actual.ReturnValue,
                traceMessage + " -> Return");
        }

        public void TestCondition(IfElseAstNode expect, IfElseAstNode actual,
            string traceMessage) {
            TestGeneral(expect.IfCondition, actual.IfCondition,
                traceMessage + " -> IfCondition");
            TestGeneral(expect.IfBlock, actual.IfBlock,
                traceMessage + " -> IfBlock");
            TestGeneral(expect.ElseBlock, actual.ElseBlock,
                traceMessage + " -> ElseBlock");
        }

        public void TestLoop(LoopAstNode expect, LoopAstNode actual,
            string traceMessage) {
            TestGeneral(expect.Condition, actual.Condition, traceMessage);
            TestGeneral(expect.Body, actual.Body, traceMessage);
        }

        public void TestPrototype(FunctionPrototypeAstNode expect,
            FunctionPrototypeAstNode actual, string traceMessage) {
            Assert.AreEqual(expect.Name, actual.Name, traceMessage);
            Assert.AreEqual(expect.ReturnType, actual.ReturnType, traceMessage);
            Assert.IsTrue(
                EqualCheckUtil.EnumerableEqual(expect.Attributes, actual.Attributes),
                traceMessage);
            Assert.IsTrue(
                EqualCheckUtil.EnumerableEqual(expect.Parameters, actual.Parameters),
                traceMessage);
        }

        public void TestFunction(FunctionAstNode expect, FunctionAstNode actual,
            string traceMessage) {
            TestGeneral(expect.Prototype, actual.Prototype,
                $"{traceMessage} -> Function {expect.Prototype.Name}");
            TestGeneral(expect.Body, actual.Body,
                $"{traceMessage} -> Function {expect.Prototype.Name} Body");
        }

        public void TestFunctionCall(FunctionCallingAstNode expect,
            FunctionCallingAstNode actual, string traceMessage) {
            Assert.AreEqual(expect.Callee, actual.Callee, traceMessage);
            Assert.IsTrue(
                EqualCheckUtil.EnumerableEqual(expect.Arguments, actual.Arguments),
                traceMessage);
        }

        public void TestKeyword(KeywordAstNode expect, KeywordAstNode actual,
            string traceMessage) {
            Assert.AreEqual(expect, actual, traceMessage);
        }
    }
}
