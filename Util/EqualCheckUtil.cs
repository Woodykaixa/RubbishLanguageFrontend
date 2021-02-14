using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace RubbishLanguageFrontEnd.Util {
    public static class EqualCheckUtil {
        public static bool NotNullAndSameType<T>(T source, object? toCheck) {
            if (toCheck is null) {
                return false;
            }

            if (ReferenceEquals(source, toCheck)) {
                return true;
            }

            return typeof(T) == toCheck.GetType();
        }

        public static bool BothNullOrNotNull<T1, T2>(T1? a, T2? b, out bool isNull) {
            if (a is null && b is null) {
                isNull = true;
                return true;
            }

            isNull = false;
            return a is not null && b is not null;
        }

        public static bool EnumerableEqual<T>(IEnumerable<T>? e1, IEnumerable<T>? e2) {
            if (e1 is null && e2 is null) {
                return true;
            }

            return e1 is not null && e2 is not null && e1.SequenceEqual(e2);
        }

        public static bool SortedEnumerableEqual<T>(
            IEnumerable<T>? e1, IEnumerable<T>? e2) {
            if (e1 is null && e2 is null) {
                return true;
            }

            if (e1 is null || e2 is null) {
                return false;
            }

            var a1 = e1 as T[] ?? e1.ToArray();
            var a2 = e2 as T[] ?? e2.ToArray();
            Array.Sort(a1);
            Array.Sort(a2);
            return a1.SequenceEqual(a2);
        }
    }
}
