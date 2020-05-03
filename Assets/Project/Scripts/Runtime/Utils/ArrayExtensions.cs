using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Scripts.Runtime.Utils
{
    public static class ArrayExtensions
    {
        public static List<Exception> TryForeach<T>(this ICollection<T> array, Action<T> block)
        {
            var ex = new List<Exception>();
            foreach (var a in array)
            {
                try
                {
                    block(a);
                }
                catch (Exception exception)
                {
                    ex.Add(exception);
                }
            }
            return ex;
        }
    }
}