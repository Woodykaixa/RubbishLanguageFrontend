// This part is not used now. Because I have problem when implementing DFA.

using System.Collections.Generic;

namespace RubbishLanguageFrontEnd.Util {
    public class NfaState {
        public List<NfaEdge> EdgesToOther;
        public bool IsFinalState { get; internal set; }
    }

    public class NfaEdge {
        public int Sym { get; }
        public NfaState Target { get; }

        public NfaEdge(int symbol, NfaState targetState) {
            Sym = symbol;
            Target = targetState;
        }

        public static readonly int SymEpsilon = 0;
    }


    public class NfaComponent {
        private NfaState _endingState;

        public NfaState Ending {
            get => _endingState;
            set {
                _innerStates.Remove(_endingState); // doc says the param can be null
                _innerStates.Add(value);
                _endingState = value;
            }
        }

        public NfaEdge InputEdge;

        internal List<NfaState> _innerStates;

        public NfaComponent() {
            _innerStates = new List<NfaState>();
        }


        public NfaComponent AddState(NfaState state) {
            _innerStates.Add(state);
            return this;
        }

        public NfaComponent AddState(IEnumerable<NfaState> states) {
            foreach (var state in states) {
                AddState(state);
            }

            return this;
        }
    }
}
