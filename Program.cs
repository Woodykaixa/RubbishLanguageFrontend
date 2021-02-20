using RubbishLanguageFrontEnd.AST;
using RubbishLanguageFrontEnd.CodeGenerator;
using RubbishLanguageFrontEnd.Lexer;
using RubbishLanguageFrontEnd.Parser;
using RubbishLanguageFrontEnd.Util.Logging;
using System;
using System.IO;

namespace RubbishLanguageFrontEnd {
    internal class Commands {
        public string InputFile;
        public string OutputFile;
        public bool NeedHelp;
        public FileStream InputFileStream;
        public FileStream OutputFileStream;

        public Commands() {
            InputFile = "";
            OutputFile = "";
            NeedHelp = false;
        }
    }

    public class Program {
        static void Help() {
            Console.WriteLine("RubbishLanguage(rblang) Compiler frontend");
            Console.WriteLine("Usage: rbc.exe [options] source");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("\n--output\tThe name of output file");
        }

        static string NextArg(int i, string[] args) {
            if (i < args.Length) {
                return args[i];
            }

            throw new UnexpectedEndOfArgsException(args[i - 1]);
        }

        static Commands ParseArgs(string[] args) {
            var cmd = new Commands();
            if (args.Length < 2) {
                throw new Exception("rbc expect at least 1 argument.");
            }

            for (var i = 0; i < args.Length; i++) {
                switch (args[i]) {
                    case "--help":
                        cmd.NeedHelp = true;
                        break;
                    case "--output":
                        cmd.OutputFile = NextArg(i++, args);
                        break;
                    default: {
                        if (cmd.InputFile != "") {
                            Console.WriteLine(
                                $"You are reassigning input file({cmd.InputFile}) to {args[i]}");
                        }

                        cmd.InputFile = args[i];
                        break;
                    }
                }
            }

            return cmd;
        }

        static void Main(string[] args) {
            Commands cmd;
            try {
                cmd = ParseArgs(args);
            } catch (UnexpectedEndOfArgsException e) {
                Console.Error.WriteLine(e.ToString());
                Help();
                return;
            } catch (Exception e) {
                Console.Error.WriteLine(e.Message);
                Help();
                return;
            }

            if (cmd.NeedHelp) {
                Help();
                return;
            }

            if (!cmd.InputFile.EndsWith(".rbl")) {
                Console.Error.WriteLine("rbc expect files ends with .rbl");
                return;
            }

            try {
                cmd.InputFileStream = new FileStream(cmd.InputFile, FileMode.Open);
            } catch (Exception e) {
                Console.Error.WriteLine(
                    $"rbc thrown an exception when opening file: {cmd.InputFile}");
                Console.Error.WriteLine(e.Message);
                return;
            }

            cmd.OutputFileStream = new FileStream(cmd.OutputFile, FileMode.OpenOrCreate);
            var logger = Logger.GetByName("rbc-dev.log");
            logger.CopyToStdout = true;
            var lexer = new RbLexer(cmd.InputFileStream);
            var parser = new RbParser(lexer);
            var codes = parser.Parse();
            if (parser.HasError) {
                return;
            }

            var _generator = new ClrIlGenerator(cmd, (CodeBlockAstNode) codes);
            _generator.Check();
        }
    }
}
