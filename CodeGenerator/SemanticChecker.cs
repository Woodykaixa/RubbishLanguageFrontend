using System;
using System.Linq;
using System.Net.Sockets;
using RubbishLanguageFrontEnd.AST;
using RubbishLanguageFrontEnd.Util;

namespace RubbishLanguageFrontEnd.CodeGenerator {
    public class SemanticChecker : IAstVisitor {
        private readonly Context _context;
        private LanguageInformation _lang;
        private string _exprType;
        private bool _functionExpectReturn;
        private bool _functionHasReturn;
        private bool _functionAllPathReturn;
        private string _functionReturnType;
        private readonly AstVisitorDispatcher _dispatcher;

        public SemanticChecker(Context context) {
            _context = context;
            _lang = LanguageInformation.Instance;
            _dispatcher = new AstVisitorDispatcher(this);
        }

        public void Visit(BasicAstNode visitable) {
            _dispatcher.CallVisitMethod(visitable);
        }

        public void Visit(IntegerAstNode visitable) {
            _exprType = "i64";
        }

        public void Visit(FloatAstNode visitable) {
            _exprType = "f64";
        }

        public void Visit(StringAstNode visitable) {
            _exprType = "str";
        }

        public void Visit(IdentifierAstNode visitable) {
            if (!_context.HasVariable(visitable.Name)) {
                throw new UndefinedVariableException(visitable.Name);
            }

            _exprType = _context.GetVariableType(visitable.Name);
        }

        public void Visit(VariableDefineAstNode visitable) {
            _context.AddVariable(visitable.Type, visitable.Name);
            _exprType = visitable.Type;
            if (visitable.AssignAst is null) {
                return;
            }

            Visit(visitable.AssignAst);
            if (visitable.Type != _exprType) {
                throw new WrongTypeException(
                    $"Cannot convert {_exprType} expression to {visitable.Type}");
            }
        }

        public void Visit(UnaryOperatorAstNode visitable) {
            if (visitable.Operator == "address_of") {
                throw new SemanticException("address_of operator is not implemented");
            }

            Visit(visitable.Expr);
            if (_exprType != "i64") {
                throw new WrongTypeException(
                    $"Cannot use {visitable.Operator} on type: {_exprType}");
            }
        }

        public void Visit(BinaryOperatorAstNode visitable) {
            Visit(visitable.Left);
            var leftType = _exprType;
            Visit(visitable.Right);
            var op = visitable.Operator;
            if (leftType != _exprType) {
                throw new WrongTypeException(
                    $"Cannot apply operator {op} on different types(left: {leftType}, right:{_exprType})");
            }
        }

        public void Visit(CodeBlockAstNode visitable) {
            _context.CreateScope();
            foreach (var statement in visitable.Statements) {
                Visit(statement);
            }

            _context.DestroyScope();
        }

        public void Visit(IfElseAstNode visitable) {
            if (_functionExpectReturn) {
                _functionAllPathReturn = false;
                _functionHasReturn = false;
            }

            Visit(visitable.IfCondition);
            if (_exprType != "i64") {
                throw new WrongTypeException($"Cannot convert {_exprType} to boolean");
            }

            Visit(visitable.IfBlock);
            _functionAllPathReturn = false;
            if (visitable.ElseBlock is not null) {
                Visit(visitable.ElseBlock);
            }
        }

        public void Visit(LoopAstNode visitable) {
            Visit(visitable.Condition);
            if (_exprType != "i64") {
                throw new WrongTypeException($"Cannot convert {_exprType} to boolean");
            }

            Visit(visitable.Body);
        }

        public void Visit(ReturnAstNode visitable) {
            if (!_functionExpectReturn) {
                throw new SemanticException("Unexpected return statement");
            }

            Visit(visitable.ReturnValue);
            _functionHasReturn = true;
            _functionAllPathReturn = true;
            if (_functionExpectReturn && _exprType != _functionReturnType) {
                throw new WrongTypeException(
                    $"Function should return {_functionReturnType}, but {_exprType} actually");
            }
        }

        public void Visit(BreakAstNode visitable) { }

        public void Visit(ContinueAstNode visitable) { }

        public void Visit(FunctionAstNode visitable) {
            _functionReturnType = visitable.Prototype.ReturnType;
            if (_functionReturnType != "void") {
                _functionExpectReturn = true;
                _functionAllPathReturn = false;
                _functionHasReturn = false;
            } else {
                _functionExpectReturn = false;
            }


            if (!visitable.IsImported) {
                foreach (var parameter in visitable.Prototype.Parameters) {
                    if (!_context.HasType(parameter.Type)) {
                        throw new UnknownTypeException(parameter.Type);
                    }

                    _context.FunctionArgQueue.Add(
                        new Tuple<string, string>(parameter.Type, parameter.Name));
                }

                Visit(visitable.Body);
                if (_functionExpectReturn) {
                    if (!_functionHasReturn) {
                        throw new SemanticException(
                            $"expect return in function {visitable.Prototype.Name}");
                    }

                    if (!_functionAllPathReturn) {
                        throw new SemanticException(
                            $"Not all path have a return statement in function {visitable.Prototype.Name}");
                    }
                }
            }

            _context.AddFunction(visitable.Prototype);
        }

        public void Visit(FunctionCallingAstNode visitable) {
            if (!_context.HasFunction(visitable.Callee)) {
                throw new UndefinedVariableException($"function {visitable.Callee}");
            }

            var args = visitable.Arguments;
            var prototypes = _context.GetFunction(visitable.Callee);
            var sameCountPrototypes = (from proto in prototypes
                where proto.Parameters.Length == (args?.Length ?? 0)
                select proto).ToArray();
            if (args is null || args.Length == 0) {
                if (sameCountPrototypes.Length != 1) {
                    throw new VariableRedefineException($"function {visitable.Callee}");
                }

                return;
            }

            for (int i = 0; i < args.Length; i++) {
                Visit(args[i]);
                var i1 = i;
                var protoToReserve = (from proto in sameCountPrototypes
                    where proto.Parameters[i1].Type == _exprType
                    select proto).ToArray();

                if (protoToReserve.Length == 0) {
                    throw new SemanticException(
                        $"no matching function: {visitable.Callee}");
                }

                sameCountPrototypes = protoToReserve;
            }

            if (sameCountPrototypes.Length != 1) {
                throw new VariableRedefineException($"function {visitable.Callee}");
            }
        }
    }
}
