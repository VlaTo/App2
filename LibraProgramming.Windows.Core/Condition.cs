using System;

namespace LibraProgramming.Windows.Core
{
    internal static class Condition<TObject>
    {
        public static readonly Predicate<TObject> True;

        static Condition()
        {
            True = _ => true;
        }
    }
}