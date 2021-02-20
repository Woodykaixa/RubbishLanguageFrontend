using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RubbishLanguageFrontEnd.AST;

namespace RubbishLanguageFrontEnd.Util {
    public class AstVisitorDispatcher {
        private readonly IAstVisitor _visitor;
        private readonly Dictionary<Type, MethodInfo> _autoDispatchMethods;

        public AstVisitorDispatcher(IAstVisitor visitor) {
            _visitor = visitor;
            var methods = visitor.GetType().GetMethods();

            _autoDispatchMethods = (
                from method in methods
                let parameters = method.GetParameters()
                where method.Name.Equals("Visit") && parameters.Length == 1 &&
                      parameters[0].ParameterType.IsSubclassOf(typeof(BasicAstNode))
                select method
            ).ToDictionary(method => method.GetParameters()[0].ParameterType,
                method => method);
        }

        public void CallVisitMethod(BasicAstNode node) {
            var nodeType = node.GetType();

            if (!_autoDispatchMethods.ContainsKey(nodeType)) {
                throw new Exception($"Unsupported Ast type: {nodeType.Name}");
            }

            _autoDispatchMethods[nodeType].Invoke(_visitor, new object[] {node});
        }
    }
}
