using System;
using System.Collections.Generic;
using System.Linq;
using RubbishLanguageFrontEnd.AST;

namespace RubbishLanguageFrontEnd.CodeGenerator {
    public class Context {
        /// <summary>
        /// A hashset to store defined types. For now, rblang cannot define new types. But it is designed for
        /// extending purpose.
        /// </summary>
        private readonly HashSet<string> _definedTypes;

        private readonly Dictionary<string, Type> _typeMapping;

        private readonly List<Dictionary<string, HashSet<string>>> _scopeVariables;
        private readonly List<HashSet<string>> _allVariables;
        private readonly Dictionary<string, List<FunctionPrototypeAstNode>> _functions;

        public Context() {
            _definedTypes = new HashSet<string>();
            _typeMapping = new Dictionary<string, Type>();
            _allVariables = new List<HashSet<string>>();
            _functions = new Dictionary<string, List<FunctionPrototypeAstNode>>();
            _scopeVariables = new List<Dictionary<string, HashSet<string>>>();
            FunctionArgQueue = new List<Tuple<string, string>>();
            CreateScope();
        }

        /// <summary>
        /// When parsing a function, put its parameter type and name into this queue, and
        /// will be add to variable set when creating a new scope.
        /// </summary>
        public readonly List<Tuple<string, string>> FunctionArgQueue;

        /// <summary>
        /// When parsing a code block, create a new scope of variables, and
        /// destroy this scope when parsing is finished.
        /// </summary>
        public void CreateScope() {
            var scopeVars = new Dictionary<string, HashSet<string>>(_definedTypes.Count);
            foreach (var typename in _definedTypes) {
                scopeVars[typename] =
                    new HashSet<string>(); // define variable set for each type
            }

            _scopeVariables.Add(scopeVars);
            _allVariables.Add(new HashSet<string>());
            if (FunctionArgQueue.Count == 0) {
                return;
            }

            foreach (var (type, name) in FunctionArgQueue) {
                AddVariable(type, name);
            }

            FunctionArgQueue.Clear();
        }

        public void DestroyScope() {
            _allVariables.RemoveAt(_allVariables.Count - 1);
            _scopeVariables.RemoveAt(_scopeVariables.Count - 1);
        }

        public bool IsTopScope => _scopeVariables.Count == 1;

        public bool HasType(string typename) => _definedTypes.Contains(typename);


        public void AddType(string typename, Type type) {
            if (HasType(typename)) {
                throw new TypeRedefineException(typename);
            }

            _definedTypes.Add(typename);
            _typeMapping.Add(typename, type);
        }

        public Type GetType(string typename) {
            if (!HasType(typename)) {
                throw new UnknownTypeException(typename);
            }

            return _typeMapping[typename];
        }

        public void AddVariable(string type, string variableName) {
            if (!HasType(type)) {
                throw new UnknownTypeException(type);
            }

            if (_allVariables[^1].Contains(variableName)) {
                // only check current scope, because it is okay to redefine vars in new scope
                throw new VariableRedefineException(variableName);
            }

            var currentScope = _scopeVariables[^1];
            if (currentScope.ContainsKey(type)) {
                currentScope[type].Add(variableName);
            } else {
                currentScope[type] = new HashSet<string> {variableName};
            }

            _allVariables[^1].Add(variableName);
        }

        public bool HasVariable(string variableName) {
            return _allVariables.Any(scope => scope.Contains(variableName));
        }

        public string GetVariableType(string variableName) {
            if (!HasVariable(variableName)) {
                throw new UndefinedVariableException(variableName);
            }

            for (var i = _scopeVariables.Count - 1; i >= 0; i--) {
                foreach (var varSet in _scopeVariables[i]
                    .Where(varSet => varSet.Value.Contains(variableName))) {
                    return varSet.Key;
                }
            }

            throw new UndefinedVariableException(variableName);
        }

        public bool HasFunction(string name) =>
            _functions.ContainsKey(name);

        public void AddFunction(FunctionPrototypeAstNode function) {
            var funcName = function.Name;
            var hasFuncName = _functions.ContainsKey(funcName);
            switch (hasFuncName) {
                case true when _functions[funcName].Contains(function):
                    throw new VariableRedefineException($"function {function.Name}");
                case true:
                    _functions[funcName].Add(function);
                    break;
                default:
                    _functions[funcName] = new List<FunctionPrototypeAstNode> {function};
                    break;
            }
        }

        public FunctionPrototypeAstNode[] GetFunction(string name) {
            return HasFunction(name) ? _functions[name].ToArray() : null;
        }
    }
}
