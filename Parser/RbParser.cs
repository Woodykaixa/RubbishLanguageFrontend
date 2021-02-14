using System;
using System.Collections.Generic;
using System.Linq;
using RubbishLanguageFrontEnd.AST;
using RubbishLanguageFrontEnd.Lexer;
using RubbishLanguageFrontEnd.Util.Logging;

namespace RubbishLanguageFrontEnd.Parser {
    public class RbParser : ILanguageParser {
        private readonly ILanguageLexer _tokenSource;
        private Token _currentToken;
        private Token _previousToken;
        private readonly ErrorLogger _error;
        private readonly Logger _logger;
        public bool HasError => !_error.Empty;

        public static readonly Dictionary<string, int> OperatorPrecedence = new() {
            {"=", 1},
            {"or", 2},
            {"and", 3},
            {"==", 4},
            {"<", 5},
            {"<=", 5},
            {">", 5},
            {">=", 5},
            {"+", 6},
            {"-", 6},
            {"*", 7},
            {"/", 7},
            {"%", 7},
            {"not", 8},
            {"address_of", 8},
        };


        public static readonly TokenType[] FunctionParameterTypes = {
            TokenType.Int64,
            TokenType.Float64,
            TokenType.Str,
            TokenType.Void
        };

        public static int GetOperatorPrecedence(string op) {
            return OperatorPrecedence.ContainsKey(op) ? OperatorPrecedence[op] : -1;
        }


        public RbParser(ILanguageLexer rbLexer) {
            _tokenSource = rbLexer;
            _currentToken = null;
            _error = new ErrorLogger();
            _logger = Logger.GetByName("rbc-dev.log");
            _logger.CopyToStdout = true;
            _previousToken = null;
        }

        public BasicAstNode Parse() {
            _logger.WriteLine("Parsing begin\n");
            var statements = new List<BasicAstNode>();
            NextToken();
            while (true) {
                BasicAstNode ast;
                switch (_currentToken.Type) {
                    case TokenType.Eof:
                        goto LoopOut;
                    case TokenType.Attr:
                        ast = ParseFunctionWithAttributes();

                        break;
                    case TokenType.Func:
                        ast = ParseFunction(null);

                        break;
                    case TokenType.Int64:
                    case TokenType.Float64:
                    case TokenType.Str:
                        ast = ParseVariableDefine();
                        break;
                    case TokenType.Identifier:
                        ast = ParseIdentifier();
                        break;
                    default:
                        NextToken();
                        ast = null;
                        break;
                }

                if (ast == null)
                    continue;
                _logger.WriteLine(ast.ToString());
                statements.Add(ast);
            }

            LoopOut:
            if (!_error.Empty) {
                _error.Print();
            }

            return new CodeBlockAstNode(statements.ToArray());
        }

        private BasicAstNode LogError(string msg) {
            _error.LogErrorMessage($"Line {_currentToken.SourceLineNumber}, {msg}");
            return null;
        }

        private void SkipTokenUntil(TokenType type) {
            while (NextToken().Type != type) { }
        }

        private void SkipTokenUntil(TokenType[] types) {
            while (types.Contains(NextToken().Type)) { }
        }

        private bool ExpectToken(TokenType type, string message) {
            if (_currentToken.Type == type)
                return true;
            LogError(message);
            return false;
        }

        private bool ExpectToken(TokenType type, string message, Action ignoreToken) {
            if (_currentToken.Type == type)
                return true;
            LogError(message);
            ignoreToken();
            return false;
        }

        private bool ExpectToken(IEnumerable<TokenType> types, string message,
            Action ignoreToken) {
            if (types.Contains(_currentToken.Type))
                return true;
            LogError(message);
            ignoreToken();
            return false;
        }

        private Token NextToken() {
            _previousToken = _currentToken;
            _currentToken = _tokenSource.NextToken();
            return _currentToken;
        }

        private bool ExpectNextToken(TokenType type, string message, Action ignoreToken) {
            NextToken();
            return ExpectToken(type, message, ignoreToken);
        }

        private bool ExpectNextToken(IEnumerable<TokenType> types, string message,
            Action ignoreToken) {
            NextToken();
            return ExpectToken(types, message, ignoreToken);
        }

        private void IgnoreThisLine() {
            while (_currentToken.Type != TokenType.Semi) {
                NextToken();
            }
        }

        private void IgnoreThisBlock() {
            while (_previousToken.Type != TokenType.RightBrace) {
                NextToken();
            }
        }

        private void IgnoreUntilRightParenthesis() {
            while (NextToken().Type != TokenType.RightParenthesis) { }
        }

        private IntegerAstNode ParseInteger() {
            IntegerAstNode result;
            try {
                result = IntegerAstNode.FromToken(_currentToken);
            } catch (ParseIntegerException e) {
                _error.LogException(e);
                result = null;
            }

            NextToken();
            return result;
        }

        private FloatAstNode ParseFloat() {
            FloatAstNode result;
            try {
                result = FloatAstNode.FromToken(_currentToken);
            } catch (ParseFloatException e) {
                _error.LogException(e);
                result = null;
            }

            NextToken();
            return result;
        }

        private StringAstNode ParseString() {
            var str = _currentToken.Value;
            NextToken();
            return new StringAstNode(str);
        }

        private BasicAstNode ParseIdentifier() {
            var idToken = _currentToken;

            if (NextToken().Type != TokenType.LeftParenthesis) {
                // Not function call, just variable
                return new IdentifierAstNode(idToken.Value);
            }

            NextToken(); // eat '('
            var paramList = new List<BasicAstNode>();
            do {
                var param = ParsePrimary();
                paramList.Add(ParseExpRhs(0, param));
                if (!ExpectToken(new[] {TokenType.Comma, TokenType.RightParenthesis},
                    "expect comma or parenthesis", IgnoreUntilRightParenthesis)
                ) {
                    NextToken(); // eat ')'
                    return null;
                }

                if (_currentToken.Type != TokenType.Comma) {
                    continue;
                }

                if (ExpectNextToken(
                    new[] {
                        TokenType.Identifier, TokenType.ValFloat, TokenType.ValInt,
                        TokenType.ValStr
                    }, "expect parameter", IgnoreUntilRightParenthesis)) {
                    continue;
                }

                NextToken();
                return null;
            } while (_currentToken.Type != TokenType.RightParenthesis);


            NextToken(); // eat ')'
            return new FunctionCallingAstNode(idToken.Value, paramList.ToArray());
        }

        private BasicAstNode ParseParenthesis() {
            NextToken(); // eat '('
            var expr = ParseBinaryOpExpression();
            if (expr == null) {
                return null;
            }

            if (!ExpectToken(TokenType.RightParenthesis, "expect right parenthesis",
                IgnoreUntilRightParenthesis)) {
                return null;
            }

            NextToken(); // eat ')'
            return expr;
        }

        private BasicAstNode ParseVariableDefine() {
            var type = _currentToken.Type.ToString();
            ExpectNextToken(TokenType.Identifier, "expect identifier",
                IgnoreThisLine);
            var name = _currentToken.Value;
            var variable = new IdentifierAstNode(_currentToken.Value);
            ExpectNextToken(new[] {TokenType.Semi, TokenType.OpAssign},
                "unexpected token", IgnoreThisLine);
            if (_currentToken.Type == TokenType.Semi) {
                return new VariableDefineAstNode(type, name, null);
            }

            var exp = (BinaryOperatorAstNode) ParseExpRhs(0, variable);
            return new VariableDefineAstNode(type, name, ParseExpRhs(0, exp.Right));
        }

        private ReturnAstNode ParseReturn() {
            NextToken(); // eat 'return'
            var ast = ParsePrimary();
            if (ast != null && ast.GetType() != typeof(ReturnAstNode))
                return new ReturnAstNode(ParseExpRhs(0, ast));
            LogError("expect literal, identifier or expression");
            IgnoreThisLine();
            return null;
        }

        private BasicAstNode ParsePrimary() {
            return _currentToken.Type switch {
                TokenType.Identifier => ParseIdentifier(),
                TokenType.ValInt => ParseInteger(),
                TokenType.ValFloat => ParseFloat(),
                TokenType.ValStr => ParseString(),
                TokenType.LeftParenthesis => ParseParenthesis(),
                TokenType.OpAddress => ParseUnaryOpExpression(),
                TokenType.Not => ParseUnaryOpExpression(),
                TokenType.Str => ParseVariableDefine(),
                TokenType.Int64 => ParseVariableDefine(),
                TokenType.Float64 => ParseVariableDefine(),
                TokenType.If => ParseCondition(),
                TokenType.Return => ParseReturn(),
                _ => LogError($"unexpected token, {_currentToken}")
            };
        }

        private IfElseAstNode ParseCondition() {
            NextToken(); // eat 'if'
            if (_currentToken.Type == TokenType.LeftParenthesis) {
                NextToken(); // eat '(', if current token is not '(', we assume it is condition part;
            }

            var lhs = ParsePrimary();
            var cond = ParseExpRhs(0, lhs);
            NextToken();
            CodeBlockAstNode ifBlock = null;
            if (_currentToken.Type == TokenType.LeftBrace) {
                ifBlock = ParseCodeBlock();
            } else {
                IgnoreThisBlock();
            }

            if (_currentToken.Type == TokenType.Else ||
                _currentToken.Type == TokenType.Elif) {
                NextToken();
                var elseBlock = ParseCodeBlock();
                return new IfElseAstNode(cond, ifBlock, elseBlock);
            }

            return new IfElseAstNode(cond, ifBlock, null);
        }

        private BasicAstNode ParseExpRhs(int exprPrecedence, BasicAstNode lhs) {
            while (true) {
                var currentTokPrec = GetOperatorPrecedence(_currentToken.Value);
                if (currentTokPrec < exprPrecedence) {
                    return lhs;
                }

                var binOperator = _currentToken;
                NextToken();
                var rhs = ParsePrimary();
                if (rhs == null) {
                    return null;
                }

                var nextTokPrec = GetOperatorPrecedence(_currentToken.Value);
                if (currentTokPrec < nextTokPrec) {
                    rhs = ParseExpRhs(currentTokPrec + 1, rhs);
                    if (rhs == null) {
                        return null;
                    }
                }

                lhs = new BinaryOperatorAstNode(binOperator.Value, lhs, rhs);
            }
        }

        private BasicAstNode ParseBinaryOpExpression() {
            var lhs = ParsePrimary();
            return lhs == null ? null : ParseExpRhs(0, lhs);
        }

        private BasicAstNode ParseUnaryOpExpression() {
            var op = _currentToken;
            NextToken(); // eat this unary operator;
            var expr = ParsePrimary();
            return (expr == null
                ? null
                : ParseExpRhs(0, new UnaryOperatorAstNode(op.Value, expr)));
        }


        private FunctionPrototypeAstNode ParseFunctionPrototype(string[] attributes) {
            var ignoreAllTokens = new Action(() => {
                if (attributes != null && attributes.Contains("@import")) {
                    IgnoreThisLine();
                } else {
                    IgnoreThisBlock();
                }
            });
            if (!ExpectNextToken(FunctionParameterTypes, "invalid return type",
                ignoreAllTokens)) {
                return null;
            }

            var returnType = _currentToken;
            if (!ExpectNextToken(TokenType.Identifier, "expect function name",
                ignoreAllTokens)) {
                return null;
            }

            var funcName = _currentToken.Value;
            if (!ExpectNextToken(TokenType.LeftParenthesis, "expect parenthesis",
                ignoreAllTokens)) {
                return null;
            }

            var stopLoopTokenTypes = new[] {TokenType.Comma, TokenType.RightParenthesis};
            var funcParams = new List<FunctionParameter>();
            while (NextToken().Type != TokenType.RightParenthesis) { // loop until got ')'
                switch (_currentToken.Type) {
                    case TokenType.Int64:
                    case TokenType.Float64:
                    case TokenType.Str:
                        var paramType = _currentToken;
                        if (!ExpectNextToken(TokenType.Identifier,
                            "expect parameter name", ignoreAllTokens)) {
                            return null;
                        }

                        var paramName = _currentToken;
                        var functionParameter =
                            new FunctionParameter(paramType.Value, paramName.Value);
                        funcParams.Add(functionParameter);
                        break;
                    default:
                        _error.LogException(new UnexpectedTokenException(new[] {
                            TokenType.Int64,
                            TokenType.Float64,
                            TokenType.Str
                        }, _currentToken));
                        ignoreAllTokens();
                        return null;
                }

                if (!ExpectNextToken(stopLoopTokenTypes, "expect comma or parenthesis",
                    ignoreAllTokens)) {
                    return null;
                }

                if (_currentToken.Type == TokenType.RightParenthesis) {
                    break;
                }
            }

            NextToken(); // eat ')'
            return new FunctionPrototypeAstNode(funcParams.ToArray(), funcName,
                returnType.Value, attributes);
        }

        private CodeBlockAstNode ParseCodeBlock() {
            var statementList = new List<BasicAstNode>();
            while (NextToken().Type != TokenType.RightBrace) {
                var statement = ParsePrimary();
                statementList.Add(statement);
            }

            return new CodeBlockAstNode(statementList.ToArray());
        }

        private FunctionAstNode ParseFunction(string[] attributes) {
            var prototype = ParseFunctionPrototype(attributes);
            if (prototype == null) {
                return null;
            }

            CodeBlockAstNode functionBody = null;
            if (prototype.IsImported) {
                goto DefineFunctionAndReturn;
            }

            if (!ExpectToken(TokenType.LeftBrace, "expect function body")) {
                return null;
            }

            functionBody = ParseCodeBlock();

            DefineFunctionAndReturn:
            return new FunctionAstNode(prototype, functionBody);
        }

        private FunctionAstNode ParseFunctionWithAttributes() {
            var attributes = new List<string> {
                _currentToken.Value
            };
            while (NextToken().Type == TokenType.Attr) {
                attributes.Add(_currentToken.Value);
            }

            ExpectToken(TokenType.Func, "expect function prototype");
            return ParseFunction(attributes.ToArray());
        }
    }
}
