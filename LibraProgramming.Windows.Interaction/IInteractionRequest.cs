using System;

namespace LibraProgramming.Windows.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInteractionRequest
    {
        /// <summary>
        /// 
        /// </summary>
        event EventHandler<InteractionRequestedEventArgs> Raised;
    }
}