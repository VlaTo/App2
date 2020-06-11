using Windows.UI.Xaml.Markup;
using LibraProgramming.Windows.Interactivity;

namespace LibraProgramming.Windows.Interaction
{
    [ContentProperty(Name = "Actions")]
    public class InteractionRequestTrigger : EventTrigger
    {
        protected override string GetEventName()
        {
            return "Raised";
        }
    }
}