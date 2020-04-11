using System;
using System.Collections;
using Scripts.Utils.Async.Hidden;
using UnityEngine;

namespace Scripts.Utils.Async
{
    public static class ThreadTools
    {
        private static ThreadToolsHelper helper = null;
        private static ThreadToolsHelper Helper
        {
            get
            {
                if (helper == null)
                {
                    helper = ThreadToolsHelper.Instance;
                    Initialize();
                }
                return helper;
            }
        }

        // Invoke on main thread
        public static void Initialize()
        {
            ThreadToolsHelper.Instance.Add(() => { });
        }

        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return Helper.StartCoroutine(coroutine);
        }

        public static void StopCoroutine(Coroutine toStop)
        {
            Helper.StopCoroutine(toStop);
        }

        public static void InvokeInMainThread(this Action action)
        {
            Helper.Add(action);
        }
        public static void InvokeInMainThread<T>(this Action<T> action, T param)
        {
            Action clojure = () => action(param);
            Helper.Add(clojure);
        }
        public static void InvokeInMainThread<T1, T2>(this Action<T1, T2> action, T1 p1, T2 p2)
        {
            Action clojure = () => action(p1, p2);
            Helper.Add(clojure);
        }
        public static void InvokeInMainThread<T1, T2, T3>(this Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3)
        {
            Action clojure = () => action(p1, p2, p3);
            Helper.Add(clojure);
        }
        public static void InvokeInMainThread<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            Action clojure = () => action(p1, p2, p3, p4);
            Helper.Add(clojure);
        }
    }
}
