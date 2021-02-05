// This part is not used now. Because I have problem when implementing DFA.

using RubbishLanguageFrontEnd.Util;

namespace RubbishLanguageFrontEnd.Lexer {
    public class LanguageInformation {
        public Regexp[] Keywords;
        public Regexp[] UnaryOperators;
        public Regexp[] BinaryOperators;


        public Regexp Identifier;
        public Regexp IntegerLiteral;
        public Regexp FloatLiteral;
        public Regexp LeftParenthesis;
        public Regexp RightParenthesis;
        public Regexp LeftBracket;
        public Regexp RightBracket;
        public Regexp LeftBrace;
        public Regexp RightBrace;

        public LanguageInformation() {
            Keywords = new Regexp[] {
                Regexp.Keyword("i64"),
                Regexp.Keyword("f64"),
                Regexp.Keyword("str"),
                Regexp.Keyword("void"),
                Regexp.Keyword("if"),
                Regexp.Keyword("else"),
                Regexp.Keyword("elif"),
                Regexp.Keyword("loop"),
                Regexp.Keyword("break"),
                Regexp.Keyword("continue"),
                Regexp.Keyword("func"),
                Regexp.Keyword("return")
            };

            UnaryOperators = new Regexp[] {
                Regexp.Keyword("not"),
                Regexp.Keyword("address_of"),
            };

            BinaryOperators = new Regexp[] {
                Regexp.Keyword("and"),
                Regexp.Keyword("or"),
                Regexp.Symbol('+'),
                Regexp.Symbol('-'),
                Regexp.Symbol('*'),
                Regexp.Symbol('/'),
                Regexp.Symbol('%'),
                Regexp.Symbol('<'),
                Regexp.Symbol('<') + Regexp.Symbol('='),
                Regexp.Symbol('='),
                Regexp.Symbol('>'),
                Regexp.Symbol('>') + Regexp.Symbol('='),
            };

            Identifier = Regexp.Range('a', 'z') // [a-zA-Z_][a-zA-Z0-9_]*
                             .Join(Regexp.Range('A', 'Z'))
                             .Join(Regexp.Symbol('_'))
                         + new StarExp(Regexp.Range('a', 'z')
                             .Join(Regexp.Range('A', 'Z'))
                             .Join(Regexp.Symbol('_'))
                             .Join(Regexp.Range('0', '9')));

            IntegerLiteral = Regexp.Keyword("0x").MaybeNull() // (0x)?[0-9a-fA-F]+
                             + Regexp.Range('0', '9')
                                 .Join(Regexp.Range('a', 'f'))
                                 .Join(Regexp.Range('A', 'F')).AtLeast1();

            FloatLiteral = Regexp.Range('0', '9').Many().MaybeNull() // [0-9]*.[0-9]+
                           + Regexp.Symbol('.')
                           + Regexp.Range('0', '9').AtLeast1();

            LeftParenthesis = Regexp.Symbol('(');
            RightParenthesis = Regexp.Symbol(')');
            LeftBracket = Regexp.Symbol('[');
            RightBracket = Regexp.Symbol(']');
            LeftBrace = Regexp.Symbol('{');
            RightBrace = Regexp.Symbol('}');
        }
    }
}
