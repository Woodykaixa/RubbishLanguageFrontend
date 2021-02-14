using System;
using System.Collections.Generic;
using System.IO;

namespace RubbishLanguageFrontEnd.Util.Logging {
    public class Logger {
        private readonly StreamWriter _writer;

        private static readonly Dictionary<string, Logger> LoggerMap =
            new Dictionary<string, Logger>();

        private Logger(string logFilename) {
            _writer = new StreamWriter(logFilename) {AutoFlush = true};
        }

        ~Logger() {
            _writer.Dispose();
        }

        public bool CopyToStdout = false;

        public void WriteLine(string str) {
            _writer.WriteLine(str);
            if (CopyToStdout) {
                Console.WriteLine(str);
            }
        }

        public void WriteLine() {
            _writer.WriteLine();
            if (CopyToStdout) {
                Console.WriteLine();
            }
        }

        public void Write(string str) {
            _writer.Write(str);
            if (CopyToStdout) {
                Console.Write(str);
            }
        }

        public static Logger GetByName(string logFilename) {
            if (LoggerMap.ContainsKey(logFilename)) {
                return LoggerMap[logFilename];
            }

            try {
                var logger = new Logger(logFilename);
                LoggerMap[logFilename] = logger;
                return logger;
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
