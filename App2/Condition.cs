using System;

namespace App2
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