using System;
using System.IO;

namespace RubbishLanguageFrontEnd.Util.SourceReader {
    public class ProxySourceReader {
        private readonly SourceReader _reader;
        private bool _updateLineAndCol;
        private bool _skipSpace;
        private bool _parsingStringLiteral;
        private ExtendedChar _lastChar;

        public ulong CurrentLine { get; private set; }
        public ulong CurrentColumn { get; private set; }
        public ExtendedChar CurrentChar { get; private set; }
        public ExtendedChar LookAheadChar { get; private set; }
        public string CurrentString { get; private set; }
        public bool EndOfStream => _reader.EndOfStream;

        public ProxySourceReader(Stream stream) {
            _reader = new SourceReader(stream);
            _updateLineAndCol = false;
            _skipSpace = false;
            _parsingStringLiteral = false;
            CurrentString = "";
            CurrentChar = null;
            _lastChar = null;
            LookAheadChar = _reader.Read();
            CurrentLine = _reader.LastCursor.Line;
            CurrentColumn = _reader.LastCursor.Column;
        }

        ~ProxySourceReader() {
            _reader.Dispose();
        }

        private void ReadChar() {
            if (_lastChar != null) {
                CurrentChar = _lastChar;
                _lastChar = null;
                return;
            }

            CurrentChar = LookAheadChar;
            LookAheadChar = _reader.Read();
        }

        private void SkipSpace() {
            while (CurrentChar.IsSpace) {
                ReadChar();
            }
        }

        public bool GotSeparator() {
            return !_parsingStringLiteral && CurrentChar.IsSeparateSymbol;
        }


        public void Read() {
            ReadChar();

            if (_skipSpace) {
                SkipSpace();
                _skipSpace = false;
            }

            if (_updateLineAndCol) {
                var cursor = LookAheadChar.IsLineBreak
                    ? _reader.LastCursor
                    : _reader.Cursor;
                CurrentLine = cursor.Line;
                CurrentColumn = cursor.Column;
                _updateLineAndCol = false;
            }

            if (CurrentChar.IsQuote) {
                _parsingStringLiteral = !_parsingStringLiteral;
            }


            CurrentString += CurrentChar;
        }

        /// <summary>
        /// Accept <see cref="ProxySourceReader.CurrentString">CurrentString</see>[..^1] as a valid token.
        /// Clear CurrentString, but keep the last char.
        /// </summary>
        public void AcceptToLastOne() {
            _updateLineAndCol = true;
            _skipSpace = true;
            _lastChar = new ExtendedChar(CurrentString[^1]);
            CurrentString = "";
        }

        /// <summary>
        /// Accept <see cref="ProxySourceReader.CurrentString">CurrentString</see> as a valid token.
        /// Clear CurrentString, prepare for next token.
        /// </summary>
        public void Accept() {
            _updateLineAndCol = true;
            _skipSpace = true;

            CurrentString = "";
        }
    }
}
