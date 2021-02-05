// This part is not used now. Because I have problem when implementing DFA.

using System.Collections.Generic;
using System.Linq;

namespace RubbishLanguageFrontEnd.Util {
    public class DfaState {
        public HashSet<int> SymbolSet { get; }
        public List<DfaState> EdgesToOther { get; }

        public DfaState() {
            SymbolSet = new HashSet<int>();
            EdgesToOther = new List<DfaState>();
        }
    }

    class ConvertionHelper {
        private ConvertionHelper() { }

        public static HashSet<NfaState> Closure(NfaState s) {
            var closure = new HashSet<NfaState>(s.EdgesToOther.Count);
            var checkingList = new List<NfaEdge>(s.EdgesToOther);

            while (checkingList.Count != 0) {
                var toCheck = checkingList[0];
                checkingList.Remove(toCheck);
                if (toCheck.Sym != NfaEdge.SymEpsilon) {
                    continue;
                }

                closure.Add(toCheck.Target);
                checkingList.AddRange(toCheck.Target.EdgesToOther);
            }

            return closure;
        }

        /// <summary>
        /// Edge(s, symbol) - A collection of NfaState which can be shifted from s when accepting symbol
        /// </summary>
        /// <see cref="https://www.cnblogs.com/Ninputer/archive/2011/06/10/2077991.html"/>
        /// <param name="s">the starting state</param>
        /// <param name="symbol">the accepted symbol</param>
        /// <returns>All the states shifted from s</returns>
        public static HashSet<NfaState> Edge(NfaState s, int symbol) {
            var resultSet = new HashSet<NfaState>(s.EdgesToOther.Count);
            var checkingList = new List<NfaEdge>(s.EdgesToOther);

            while (checkingList.Count != 0) {
                var toCheck = checkingList[0];
                checkingList.Remove(toCheck);
                if (toCheck.Sym != symbol) {
                    continue;
                }

                resultSet.Add(toCheck.Target);
            }

            return resultSet;
        }

        /// <summary>
        /// DFAEdge(states, symbol) = closure{Union[from d in states select Edge(d, symbol)]}
        /// A collection of NfaState which can be shifted from one of s, when accepting symbol
        /// </summary>
        /// <param name="states"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static HashSet<NfaState> DfaEdges(NfaState[] states, int symbol) {
            var resultSet = new HashSet<NfaState>(states.Length);
            foreach (var state in states) {
                foreach (var finalState in Closure(state)
                    .SelectMany(cState => Edge(cState, symbol).SelectMany(Closure))) {
                    resultSet.Add(finalState);
                }
            }

            return resultSet;
        }
    }

    public class DfaModel {
        public static DfaModel NfaToDfa(NfaComponent nfa) {
            char[] charset = new char[255];
            for (int i = 1; i < 256; i++) { // 0 is epsilon
                charset[i - 1] = (char) i;
            }

            var states = new List<HashSet<NfaState>> {
                new HashSet<NfaState>(),
                ConvertionHelper.Closure(nfa.InputEdge.Target)
            };
            var p = 1;
            var j = 0;
            while (j <= p) {
                foreach (var c in charset) {
                    var e = ConvertionHelper.DfaEdges(states[j].ToArray(), c);
                    bool setEq = false;
                    for (int i = 0; i <= p; i++) {
                        if (e.SetEquals(states[i])) {
                            setEq = true;
                            // trans[j, c] <- i
                        }
                    }

                    if (!setEq) {
                        p++;
                        states.Add(e);
                        // trans[j, c] <- p
                    }
                }

                j++;
            }

            return null;
        }
    }
}
