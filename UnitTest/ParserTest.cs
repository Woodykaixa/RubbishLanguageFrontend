using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubbishLanguageFrontEnd.Lexer;
using RubbishLanguageFrontEnd.Parser;
using RubbishLanguageFrontEnd.AST;

namespace UnitTest {
    [TestClass]
    public class ParserTest {
        [TestMethod]
        public void TestParseAst1() {
            using var sourceStream = new FileStream("TestCode/TestParse1.rbl",
                FileMode.Open);
            ILanguageLexer lexer = new RbLexer(sourceStream);
            ILanguageParser parser = new RbParser(lexer);
            var ast = (CodeBlockAstNode) parser.Parse();
            var expectedAst = new CodeBlockAstNode(new BasicAstNode[] {
                new FunctionAstNode(
                    new FunctionPrototypeAstNode(
                        new[] {
                            new FunctionParameter("str", "s")
                        }, "WriteLine", "void", new[] {"@import"}
                    ), null
                ),
                new VariableDefineAstNode("Str", "greeting",
                    new StringAstNode("\"Hello world!\\nI'm running on CLR!\"")),
                new FunctionAstNode(
                    new FunctionPrototypeAstNode(
                        new[] {
                            new FunctionParameter("i64", "n")
                        }, "fib", "i64", null
                    ), new CodeBlockAstNode(
                        new BasicAstNode[] {
                            new IfElseAstNode(
                                new BinaryOperatorAstNode("==",
                                    new IdentifierAstNode("n"),
                                    new IntegerAstNode(1)),
                                new CodeBlockAstNode(new BasicAstNode[] {
                                    new ReturnAstNode(new IntegerAstNode(1))
                                }), null
                            ),
                            new ReturnAstNode(new BinaryOperatorAstNode("*",
                                new IdentifierAstNode("n"),
                                new FunctionCallingAstNode("fib", new BasicAstNode[] {
                                    new BinaryOperatorAstNode("-",
                                        new IdentifierAstNode("n"), new IntegerAstNode(1))
                                })
                            ))
                        }
                    )
                ),
                new FunctionAstNode(
                    new FunctionPrototypeAstNode(
                        new[] {
                            new FunctionParameter("str", "s")
                        }, "foo", "i64", null
                    ), new CodeBlockAstNode(new BasicAstNode[] {
                        new FunctionCallingAstNode("WriteLine",
                            new BasicAstNode[] {new IdentifierAstNode("s")}),
                        new VariableDefineAstNode("Int64", "isNull",
                            new BinaryOperatorAstNode("+",
                                new IntegerAstNode(10),
                                new UnaryOperatorAstNode("not",
                                    new UnaryOperatorAstNode("address_of",
                                        new IdentifierAstNode("s")
                                    )
                                )
                            )
                        ),
                        new ReturnAstNode(
                            new FunctionCallingAstNode("fib", new BasicAstNode[] {
                                new IntegerAstNode(10)
                            }))
                    })
                ),
                new FunctionCallingAstNode("foo", new BasicAstNode[] {
                    new IdentifierAstNode("greeting")
                })
            });

            for (var i = 0; i < expectedAst.Statements.Length; i++) {
                Assert.AreEqual(expectedAst.Statements[i], ast.Statements[i],
                    $"\nassert failed at index: {i}\nexpect:\n{expectedAst.Statements[i]}\nactual:\n{ast.Statements[i]}");
            }
        }
    }
}
