using System.Threading;
using UnityEngine;

namespace Project.Scripts.Runtime.Utils
{
    public static class Log
    {
        public static void D(string tag, string msg, params object[] args)
        {
            Debug.LogFormat("({0})[{1}] {2}", tag, Thread.CurrentThread.ManagedThreadId, string.Format(msg, args));
        }
    }
}