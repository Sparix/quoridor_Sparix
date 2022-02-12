using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Classes {
    public static class Utils {
        public static bool IsEven(this int num) => num % 2 == 0;
        public static bool IsOdd(this int num) => num % 2 == 1;

        public static List<T> ToList<T>(this T[,] arr) {
            var result = new List<T>();
            var n = arr.GetLength(0);
            var m = arr.GetLength(1);
            for (var i = 0; i < n; i++) {
                for (var j = 0; j < m; j++) {
                    result.Add(arr[i, j]);
                }
            }

            return result;
        }

        public static T[,] DeepCopy<T>(this T[,] obj) where T : ICloneable {
            var n = obj.GetLength(0);
            var m = obj.GetLength(1);
            var result = new T[n, m];
            for (var i = 0; i < n; i++) {
                for (var j = 0; j < m; j++) {
                    result[i, j] = (T) obj[i, j].Clone();
                }
            }

            return result;
        }

        public static T GetNextCycled<T>(this IEnumerator<T> enumerator) {
            if (enumerator.MoveNext()) {
                return enumerator.Current;
            }

            enumerator.Reset();
            enumerator.MoveNext();
            return enumerator.Current;
        }

        public static Action
            AddHandlerOnIndex(this Action eventHandler, int index, Action newHandler) {
            Action result = null;
            var handlers = eventHandler?.GetInvocationList()
                .OfType<Action>()
                .ToList();
            var handlersLen = handlers?.Count ?? 0;
            if (index >= handlersLen) {
                result = eventHandler;
                result += newHandler;
            }
            else {
                for (var i = 0; i < handlersLen; i++) {
                    if (i == index) {
                        result += newHandler;
                    }
                    result += handlers[i];
                }
            }

            return result;
        }
    }
}