using RubbishLanguageFrontEnd.AST;
using RubbishLanguageFrontEnd.Util;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Console = System.Console;

namespace RubbishLanguageFrontEnd.CodeGenerator {
    internal class CodeEmitter : IAstVisitor {
        public readonly AssemblyBuilder AsmBuilder;
        public readonly ModuleBuilder ModuleBuilder;
        public ILGenerator Gen { get; set; }
        public readonly TypeBuilder ProgramBuilder;
        private readonly Context _context;


        public CodeEmitter(string assemblyName, Commands cmd, Context context) {
            _context = context;
            
            var asmName = new AssemblyName(assemblyName);
            AsmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName,
                AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder = AsmBuilder.DefineDynamicModule(cmd.OutputFile);
            ProgramBuilder = ModuleBuilder.DefineType("RbProgram",
                TypeAttributes.Public | TypeAttributes.Sealed);
            var mainBuilder = ProgramBuilder.DefineMethod("RbMain",
                MethodAttributes.Public | MethodAttributes.Static, typeof(void),
                Array.Empty<Type>());
            Gen = mainBuilder.GetILGenerator();
            Gen.EmitWriteLine("\n --------- Invoking from rbc --------- \n\n");
        }


        public void Visit(BasicAstNode visitable) {
            throw new NotImplementedException("Calling IAstVisitor.Visit(BasicAstNode)");
        }

        public void Visit(IntegerAstNode visitable) {
            Gen.Emit(OpCodes.Ldc_I8, visitable.Value);
        }

        public void Visit(FloatAstNode visitable) {
            Gen.Emit(OpCodes.Ldc_R8, visitable.Value);
        }

        public void Visit(StringAstNode visitable) {
            Gen.Emit(OpCodes.Ldstr, visitable.Value);
        }

        public void Visit(IdentifierAstNode visitable) { }

        public void Visit(VariableDefineAstNode visitable) {
            throw new System.NotImplementedException();
        }

        public void Visit(UnaryOperatorAstNode visitable) {
            if (visitable.Operator == "+" || visitable.Operator == "-") {
                BasicAstNode lhs;
                if (visitable.Expr.GetType() == typeof(IntegerAstNode)) {
                    lhs = new IntegerAstNode(0);
                } else {
                    lhs = new FloatAstNode(0);
                }

                Visit(new BinaryOperatorAstNode(visitable.Operator, lhs, visitable.Expr));
                return;
            }

            switch (visitable.Operator) {
                case "address_of":
                    throw new NotSupportedException(
                        "It seems that clr does not support pointer");
                case "not":
                    Visit(visitable.Expr);
                    Gen.Emit(OpCodes.Not);
                    break;
                default:
                    throw new Exception(
                        "Unknown unary operator: " + visitable.Operator);
            }
        }

        public void Visit(BinaryOperatorAstNode visitable) {
            if (!LanguageInformation.BinaryOperator.Contains(visitable.Operator)) {
                throw new Exception("Unknown binary operator: " + visitable.Operator);
            }

            Visit(visitable.Left);
            Visit(visitable.Right);
            switch (visitable.Operator) {
                case "+":
                    Gen.Emit(OpCodes.Add);
                    break;
                case "-":
                    Gen.Emit(OpCodes.Sub);
                    break;
                case "*":
                    Gen.Emit(OpCodes.Mul);
                    break;
                case "/":
                    Gen.Emit(OpCodes.Div);
                    break;
                case "%":
                    var lc0Less = Gen.DefineLabel();
                    var loadVar = Gen.DefineLabel();
                    var lc0 = Gen.DeclareLocal(typeof(long));
                    var lc1 = Gen.DeclareLocal(typeof(long));
                    Gen.Emit(OpCodes.Stloc_0);
                    Gen.Emit(OpCodes.Stloc_1);
                    Gen.MarkLabel(loadVar);
                    Gen.Emit(OpCodes.Ldloc_0); // lc0 == a
                    Gen.Emit(OpCodes.Ldloc_1); // lc1 == b
                    Gen.Emit(OpCodes.Blt, lc0Less); // if lc0 < lc1, goto lc0Less
                    Gen.Emit(OpCodes.Ldloc_0);
                    Gen.Emit(OpCodes.Ldloc_1);
                    Gen.Emit(OpCodes.Sub);
                    Gen.Emit(OpCodes.Stloc_0); // lc0 = lc0 - lc1
                    Gen.Emit(OpCodes.Jmp, loadVar); // goto loadVar
                    Gen.MarkLabel(lc0Less);
                    Gen.Emit(OpCodes.Ldloc_0);
                    break;
                case "and":
                    Gen.Emit(OpCodes.And);
                    break;
                case "or":
                    Gen.Emit(OpCodes.Or);
                    break;
                case "=":
                    break; // TODO: implement set local var
                case "<":
                    Gen.Emit(OpCodes.Clt);
                    break;
                case "==":
                    Gen.Emit(OpCodes.Ceq);
                    break;
                case ">":
                    Gen.Emit(OpCodes.Cgt);
                    break;
                case "<=":
                    Gen.DeclareLocal(typeof(int));
                    Gen.DeclareLocal(typeof(int));
                    var lt = Gen.DefineLabel();
                    var finish = Gen.DefineLabel();
                    Gen.Emit(OpCodes.Stloc_0);
                    Gen.Emit(OpCodes.Stloc_1);
                    Gen.Emit(OpCodes.Ldloc_0);  // lc0 == a
                    Gen.Emit(OpCodes.Ldloc_1);  // lc1 == b
                    Gen.Emit(OpCodes.Ble, lt);  // if lc0 <= lc1, goto lt
                    Gen.Emit(OpCodes.Ldc_I4_0); // else push 0, goto finish
                    Gen.Emit(OpCodes.Jmp, finish); 
                    Gen.MarkLabel(lt);                // lt: push 1
                    Gen.Emit(OpCodes.Ldc_I4_1);
                    Gen.MarkLabel(finish);            // finish:
                    break;
                case ">=":
                    Gen.DeclareLocal(typeof(int));
                    Gen.DeclareLocal(typeof(int));
                    var gt = Gen.DefineLabel();
                    finish = Gen.DefineLabel();
                    Gen.Emit(OpCodes.Stloc_0);
                    Gen.Emit(OpCodes.Stloc_1);
                    Gen.Emit(OpCodes.Ldloc_0);  // lc0 == a
                    Gen.Emit(OpCodes.Ldloc_1);  // lc1 == b
                    Gen.Emit(OpCodes.Bge, gt);  // if lc0 >= lc1, goto gt
                    Gen.Emit(OpCodes.Ldc_I4_0); // else push 0, goto finish
                    Gen.Emit(OpCodes.Jmp, finish);
                    Gen.MarkLabel(gt);                // gt: push 1
                    Gen.Emit(OpCodes.Ldc_I4_1);
                    Gen.MarkLabel(finish);            // finish:
                    break;
            }
        }

        public void Visit(CodeBlockAstNode visitable) {
            throw new System.NotImplementedException();
        }

        public void Visit(IfElseAstNode visitable) {
            throw new System.NotImplementedException();
        }

        public void Visit(LoopAstNode visitable) {
            throw new System.NotImplementedException();
        }

        public void Visit(ReturnAstNode visitable) {
            throw new System.NotImplementedException();
        }

        public void Visit(BreakAstNode visitable) {
            throw new System.NotImplementedException();
        }

        public void Visit(ContinueAstNode visitable) {
            throw new System.NotImplementedException();
        }

        public void Visit(FunctionAstNode visitable) {
            throw new System.NotImplementedException();
        }

        public void Visit(FunctionCallingAstNode visitable) {
            throw new System.NotImplementedException();
        }

        public void GenerateCil() {
        }

        public void Run() {
            var type = ProgramBuilder.CreateType();
            if (type == null) {
                Console.Error.WriteLine("type == null");
                return;
            }

            Console.WriteLine($"Runtime Version: {AsmBuilder.ImageRuntimeVersion}");
            var instance = Activator.CreateInstance(type);
            ProgramBuilder.InvokeMember("RbMain", BindingFlags.InvokeMethod, null,
                instance,
                new object[0]);
        }
    }
}
