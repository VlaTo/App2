using System;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;

namespace LibraProgramming.Windows.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    internal class EventRegistration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remove"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IDisposable CreateRoutedEventRegistration(Action<EventRegistrationToken> @remove, Delegate handler)
        {
            return new DisposableToken(() =>
            {
                WindowsRuntimeMarshal.RemoveEventHandler(@remove, handler);
            });
        }

        public static IDisposable CreateEventRegistration(EventInfo @event, object target, Delegate handler)
        {
            return new DisposableToken(() =>
            {
                @event.RemoveEventHandler(target, handler);
            });
        }
    }
}