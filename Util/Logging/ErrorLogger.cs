using System;
using System.Collections.Generic;
using System.IO;

namespace RubbishLanguageFrontEnd.Util.Logging {
    internal class ErrorLogger {
        private readonly List<string> _errorMessages;
        private readonly TextWriter _errorStream;

        public ErrorLogger() {
            _errorMessages = new List<string>();
            _errorStream = Console.Error;
        }

        public void LogException(ParseTokenException e) {
            _errorMessages.Add(e.Message);
        }

        public void LogErrorMessage(string msg) {
            _errorMessages.Add(msg);
        }

        public void Print() {
            foreach (var err in _errorMessages) {
                _errorStream.WriteLine(err);
            }
        }

        public bool Empty => _errorMessages.Count == 0;
    }
}
