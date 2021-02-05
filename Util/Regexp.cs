// This part is not used now. Because I have problem when implementing DFA.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RubbishLanguageFrontEnd.Util {
    public class Regexp {
        public enum ExpType {
            Join, // x|y
            Concat, // xy
            Star, // x*
            Symbol,
            Empty, // ε
            PlaceHolder
        }

        public ExpType Type { get; }

        public Regexp(ExpType type) {
            Type = type;
        }

        public JoinExp MaybeNull() {
            return new JoinExp(this, new EmptyExp());
        }

        public override string ToString() {
            return "";
        }

        public static SymbolExp Symbol(char sym) {
            return new SymbolExp(sym);
        }

        public static ConcatExp Keyword(string keyword) {
            var con = new ConcatExp(new Regexp(ExpType.PlaceHolder),
                new Regexp(ExpType.PlaceHolder));
            var _ = from exp in (from sym in keyword select Symbol(sym))
                select con.Concat(exp);
            return con;
        }

        public static ConcatExp operator +(Regexp left, Regexp right) {
            switch (left.Type) {
                case ExpType.Concat when right.Type == ExpType.Concat: {
                    var l = (ConcatExp) left;
                    var r = (ConcatExp) right;

                    foreach (var regexp in r.ExpList) {
                        l.ExpList.Add(regexp);
                    }

                    return l;
                }
                case ExpType.Concat:
                    return ((ConcatExp) left).Concat(right);

                default:
                    return right.Type == ExpType.Concat
                        ? ((ConcatExp) right).Concat(left)
                        : new ConcatExp(left, right);
            }
        }

        public static JoinExp Range(char begin, char end) {
            // assume end is always greater than begin
            var joinExp = new JoinExp(Symbol(begin), Symbol((char) (begin + 1)));
            for (char i = (char) (begin + 2); i <= end; i++) {
                joinExp.Join(Symbol(i));
            }

            return joinExp;
        }

        public StarExp Many() {
            if (Type == ExpType.Star) {
                return (StarExp) this;
            }

            return new StarExp(this);
        }


        public ConcatExp AtLeast1() {
            return this + new StarExp(this);
        }


        public virtual NfaComponent ToNfa() {
            throw new NotImplementedException("Invoking ToNfa() from base class!");
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public class JoinExp : Regexp {
        public List<Regexp> ExpList;

        public JoinExp(Regexp x, Regexp y) : base(ExpType.Join) {
            ExpList.Add(x);
            ExpList.Add(y);
        }

        public JoinExp Join(Regexp exp) {
            if (exp.Type == ExpType.PlaceHolder) {
                return this;
            }

            if (exp.Type != ExpType.Join) {
                ExpList.Add(exp);
            } else {
                ExpList.AddRange(((JoinExp) exp).ExpList);
            }

            return this;
        }

        public override string ToString() {
            return ExpList.Aggregate("", (current, regexp) => current + "|" + regexp)
                [1..];
        }

        public override NfaComponent ToNfa() {
            var dispatchState = new NfaState();
            var newEndingState = new NfaState() {
                IsFinalState = true
            };
            var result = new NfaComponent() {
                InputEdge = new NfaEdge(NfaEdge.SymEpsilon, dispatchState),
                Ending = newEndingState
            };

            var innerComponents = (from exp in ExpList select exp.ToNfa()).ToList();
            foreach (var com in innerComponents) {
                dispatchState.EdgesToOther.Add(com.InputEdge);
                com.Ending.EdgesToOther.Add(new NfaEdge(NfaEdge.SymEpsilon,
                    newEndingState));
                com.Ending.IsFinalState = false;
                result.AddState(com._innerStates);
            }

            return result;
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public class ConcatExp : Regexp {
        public List<Regexp> ExpList;

        public ConcatExp(Regexp x, Regexp y) : base(ExpType.Concat) {
            ExpList.Add(x);
            ExpList.Add(y);
        }

        public ConcatExp Concat(Regexp exp) {
            if (exp.Type != ExpType.PlaceHolder) {
                ExpList.Add(exp);
            }

            return this;
        }

        public override string ToString() {
            return $"{ExpList.Aggregate("", (current, regexp) => current + regexp)}";
        }

        public override NfaComponent ToNfa() {
            var comps = (from exp in ExpList select exp.ToNfa()).ToList();

            var result = new NfaComponent {
                InputEdge = comps[0].InputEdge,
                Ending = comps[^1].Ending
            };

            for (int i = 0; i < comps.Count; i++) {
                var from = comps[i];
                if (i + 1 < comps.Count) {
                    from.Ending.EdgesToOther.Add(comps[i + 1].InputEdge);
                    from.Ending.IsFinalState = false;
                }

                result.AddState(from._innerStates);
            }

            return result;
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public class StarExp : Regexp {
        public Regexp BaseExp { get; }

        public StarExp(Regexp baseExp) : base(ExpType.Star) {
            BaseExp = baseExp;
        }

        public override string ToString() {
            return $"({BaseExp})*";
        }

        public override NfaComponent ToNfa() {
            var newEndingState = new NfaState() {
                IsFinalState = true
            };
            var inner = BaseExp.ToNfa();
            inner.Ending.IsFinalState = false;
            inner.Ending.EdgesToOther.Add(new NfaEdge(NfaEdge.SymEpsilon,
                newEndingState));
            newEndingState.EdgesToOther.Add(inner.InputEdge);
            var result = new NfaComponent {
                InputEdge = new NfaEdge(NfaEdge.SymEpsilon, newEndingState),
                Ending = newEndingState
            };
            result.AddState(inner._innerStates);
            return result;
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public class SymbolExp : Regexp {
        public char Sym { get; }

        public SymbolExp(char sym) : base(ExpType.Symbol) {
            Sym = sym;
        }

        public override string ToString() {
            return Sym.ToString();
        }

        public override NfaComponent ToNfa() {
            var endingState = new NfaState() {
                IsFinalState = true
            };
            var edge = new NfaEdge(Sym, endingState);
            return new NfaComponent {Ending = endingState, InputEdge = edge};
        }
    }

    public class EmptyExp : Regexp {
        public EmptyExp() : base(ExpType.Empty) { }

        public override string ToString() {
            return "ε";
        }

        public override NfaComponent ToNfa() {
            var endingState = new NfaState() {
                IsFinalState = true
            };
            var edge = new NfaEdge(NfaEdge.SymEpsilon, endingState);
            return new NfaComponent {InputEdge = edge, Ending = endingState};
        }
    }
}
