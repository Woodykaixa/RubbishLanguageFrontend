using System;
using RubbishLanguageFrontEnd.AST;

namespace RubbishLanguageFrontEnd.CodeGenerator {
    internal class ClrIlGenerator {
        private Commands _commands;
        private Context _context;
        private SemanticChecker _checker;
        private CodeEmitter _emitter;
        private CodeBlockAstNode _programAst;


        private static Type FromRbType(string type) {
            return type switch {
                "i64" => typeof(long),
                "f64" => typeof(double),
                "str" => typeof(string),
                _ => typeof(void)
            };
        }

        public ClrIlGenerator(Commands cmd, CodeBlockAstNode programAst) {
            _commands = cmd;
            _context = new Context();
            var primitiveTypes = LanguageInformation.PrimitiveTypes;
            foreach (var type in primitiveTypes) {
                _context.AddType(type, FromRbType(type));
            }

            _programAst = programAst;
            _checker = new SemanticChecker(_context);
            _emitter = new CodeEmitter(cmd.OutputFile, cmd, _context);
        }

        public void Check() {
            _checker.Visit(_programAst);
        }
    }
}
