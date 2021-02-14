using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubbishLanguageFrontEnd.Lexer;
using RubbishLanguageFrontEnd.Parser;
using RubbishLanguageFrontEnd.Parser.AST;

namespace UnitTest {
    [TestClass]
    public class ParserTest {
        [TestMethod]
        public void TestParseAst1() {
            using var sourceStream = new FileStream("TestCode/TestParse1.rbl",
                FileMode.Open);
            ILanguageLexer lexer = new RbLexer(sourceStream);
            ILanguageParser parser = new RbParser(lexer);
            var ast = parser.Parse();
            BasicAstNode expectedAst = new CodeBlockAstNode(new BasicAstNode[] {
                new FunctionAstNode(
                    new FunctionPrototypeAstNode(
                        new[] {
                            new FunctionParameter {
                                Name = "s", Type = FunctionParameter.ParamType.Str
                            }
                        }, "WriteLine", FunctionParameter.ParamType.Void,
                        new[] {"@import"}
                    ), null
                ),
                new VariableDefineAstNode("Str", "greeting",
                    new StringAstNode("\"Hello world!\\nI'm running on CLR!\"")),
                new FunctionAstNode(
                    new FunctionPrototypeAstNode(
                        new[] {
                            new FunctionParameter {
                                Name = "n", Type = FunctionParameter.ParamType.Int64
                            }
                        }, "fib", FunctionParameter.ParamType.Int64, null
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
                            new FunctionParameter {
                                Name = "s", Type = FunctionParameter.ParamType.Str
                            }
                        }, "foo", FunctionParameter.ParamType.Void, null
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

            Assert.AreEqual(expectedAst, ast);
        }
    }
}
