using System;
using System.Diagnostics;
using System.IO;

namespace RubbishLanguageFrontEnd.Util.SourceReader {
    public class CursorLocation {
        public ulong Line { get; set; }
        public ulong Column { get; set; }

        public CursorLocation() {
            Line = 1;
            Column = 1;
        }

        public override string ToString() {
            return $"Line: {Line}, Column: {Column}";
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            return typeof(CursorLocation) == obj.GetType() &&
                   Equals(obj as CursorLocation);
        }

        protected bool Equals(CursorLocation other) {
            return Line == other.Line && Column == other.Column;
        }

        public void CopyFrom(CursorLocation loc) {
            Line = loc.Line;
            Column = loc.Column;
        }
    }

    [DebuggerDisplay("{Char}")]
    public class ExtendedChar {
        public char Char { get; }

        public ExtendedChar(char ch) {
            Char = ch;
        }

        public ExtendedChar(int ch) {
            Char = (char) ch;
        }

        public override string ToString() {
            return Char.ToString();
        }

        public bool IsEof => Char == '\uffff';
        public bool IsLineBreak => Char == '\n' || Char == '\r';
        public bool IsSpace => Char == ' ' || Char == '\t' || IsLineBreak;

        public bool IsDigit => '0' <= Char && Char <= '9';
        public bool IsUpperAlpha => 'A' <= Char && Char <= 'Z';
        public bool IsLowerAlpha => 'a' <= Char && Char <= 'z';
        public bool IsAlpha => IsUpperAlpha || IsLowerAlpha;
        public bool IsUnderline => Char == '_';

        public bool IsCommentStarter => Char == '#';

        public bool IsSemi => Char == ';';

        public bool IsComma => Char == ',';

        public bool IsEq => Char == '=';

        public bool IsLeftParenthesis => Char == '(';
        public bool IsRightParenthesis => Char == ')';
        public bool IsParenthesis => IsLeftParenthesis || IsRightParenthesis;

        public bool IsLeftBracket => Char == '[';
        public bool IsRightBracket => Char == ']';
        public bool IsBracket => IsLeftBracket || IsRightBracket;

        public bool IsLeftBrace => Char == '{';
        public bool IsRightBrace => Char == '}';
        public bool IsBrace => IsLeftBrace || IsRightBrace;
        public bool IsOpPlus => Char == '+';
        public bool IsOpMinus => Char == '-';
        public bool IsOpMultiply => Char == '*';
        public bool IsOpDiv => Char == '/';
        public bool IsOpMod => Char == '%';
        public bool IsOpGt => Char == '>';
        public bool IsOpLt => Char == '<';
        public bool IsOpAssign => Char == '=';

        public bool IsOperator => IsOpPlus || IsOpMinus || IsOpMultiply || IsOpDiv ||
                                  IsOpMod || IsOpGt || IsOpLt || IsOpAssign;

        public bool IsSeparateSymbol => IsSemi || IsComma || IsSpace || IsEq ||
                                        IsParenthesis || IsBracket || IsBrace ||
                                        IsOperator;

        public bool IsQuote => Char == '"';
    }

    internal class SourceReader : IDisposable {
        private bool _meetCr;
        private bool _provideExtraSpaceWhenEof = true;
        private readonly StreamReader _reader;

        public bool EndOfStream => _reader.EndOfStream;
        public CursorLocation Cursor { get; }
        public CursorLocation LastCursor { get; }

        /// <summary>
        /// Read characters from a byte stream, and provide line and column number of the characters.
        /// As it is "source" reader, it will ignore comments.
        /// </summary>
        /// <param name="sourceStream">the stream to be read</param>
        public SourceReader(Stream sourceStream) {
            _reader = new StreamReader(sourceStream);
            Cursor = new CursorLocation();
            LastCursor = new CursorLocation();
            _meetCr = false;
        }


        private void NewLine() {
            LastCursor.CopyFrom(Cursor);
            Cursor.Line++;
            Cursor.Column = 1;
        }

        /// <summary>
        /// Reads the next char from input stream and advances the character position by one character.
        /// Also update the line and column information.
        /// </summary>
        /// <returns>The char wrapped in <see cref="ExtendedChar">ExtendedChar</see></returns>
        public ExtendedChar Read() {
            if (EndOfStream && _provideExtraSpaceWhenEof) {
                _provideExtraSpaceWhenEof = false;
                return new ExtendedChar(' ');
            }

            var ch = _reader.Read();
            while (ch == '#') { // ignore comments
                _reader.ReadLine();
                ch = _reader.Read();
                NewLine();
            }

            if (ch == '\r') {
                NewLine();
                _meetCr = true;
                ch = '\n'; // we always handle \r as \n
            } else {
                switch (ch) {
                    case '\n' when _meetCr: { // consider break with crlf, skip one char
                        ch = _reader.Read();
                        if (ch == '\r') {
                            ch = '\n';
                        }

                        while (ch == '#') {
                            _reader.ReadLine();
                            ch = _reader.Read();
                            NewLine();
                        }

                        break;
                    }
                    case '\n':
                        NewLine();
                        break;
                    case '\t':
                        LastCursor.CopyFrom(Cursor);
                        Cursor.Column += 4;
                        break;
                    default:
                        LastCursor.CopyFrom(Cursor);
                        Cursor.Column++;
                        break;
                }

                _meetCr = false;
            }


            return new ExtendedChar(ch);
        }

        public void Dispose() {
            _reader?.Dispose();
        }
    }
}
