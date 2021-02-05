using System;
using System.Collections.Generic;
using System.Text;

namespace RubbishLanguageFrontEnd {
    class UnexpectedEndOfArgsException : Exception {
        public string CurrentArg;

        public UnexpectedEndOfArgsException(string current) {
            CurrentArg = current;
        }

        public override string ToString() {
            return $"Unexpected end of argument: {CurrentArg}, expect more arguments";
        }
    }
}
