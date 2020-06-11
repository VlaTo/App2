using System;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    internal sealed class NameResolver
    {
        private string name;
        private FrameworkElement nameScopeElement;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                var obj = Object;

                name = value;

                UpdateObjectFromName(obj);
            }
        }

        public DependencyObject Object
        {
            get
            {
                if (String.IsNullOrEmpty(Name) && HasAttempedResolve)
                {
                    return NameScopeElement;
                }
                
                return ResolvedObject;
            }
        }

        public FrameworkElement NameScopeElement
        {
            get
            {
                return nameScopeElement;
            }
            set
            {
                var referenceElement = NameScopeElement;

                nameScopeElement = value;

                OnNameScopeElementChanged(referenceElement);
            }
        }

        public event EventHandler<NameResolvedEventArgs> ResolvedElementChanged;

        private FrameworkElement ActualNameScopeElement
        {
            get
            {
                if (null == NameScopeElement || !Interaction.IsElementLoaded(NameScopeElement))
                {
                    return null;
                }
                
                return GetActualNameScopeReference(NameScopeElement);
            }
        }

        private DependencyObject ResolvedObject
        {
            get; 
            set;
        }

        private bool PendingReferenceElementLoad
        {
            get; 
            set;
        }

        private bool HasAttempedResolve
        {
            get; 
            set;
        }

        private FrameworkElement GetActualNameScopeReference(FrameworkElement initialReferenceElement)
        {
            var element = initialReferenceElement;

            if (IsNameScope(initialReferenceElement))
            {
                element = initialReferenceElement.Parent as FrameworkElement ?? element;
            }

            return element;
        }
        
        private void UpdateObjectFromName(DependencyObject source)
        {
            DependencyObject dependencyObject = null;

            ResolvedObject = null;

            if (null != NameScopeElement)
            {
                if (!Interaction.IsElementLoaded(NameScopeElement))
                {
                    NameScopeElement.Loaded += OnNameScopeLoaded;
                    PendingReferenceElementLoad = true;

                    return;
                }
                
                if (!String.IsNullOrEmpty(Name))
                {
                    var referenceElement = ActualNameScopeElement;

                    if (referenceElement != null)
                    {
                        dependencyObject = referenceElement.FindName(Name) as DependencyObject;
                    }
                }
            }

            HasAttempedResolve = true;
            ResolvedObject = dependencyObject;

            if (source == Object)
            {
                return;
            }

            OnObjectChanged(source, Object);
        }

        private bool IsNameScope(FrameworkElement element)
        {
            var parent = element.Parent as FrameworkElement;

            if (null != parent)
            {
                return null != parent.FindName(Name);
            }
            
            return false;

        }
        
        private void OnNameScopeElementChanged(FrameworkElement element)
        {
            if (PendingReferenceElementLoad)
            {
                element.Loaded -= OnNameScopeLoaded;
                PendingReferenceElementLoad = false;
            }

            HasAttempedResolve = false;
            
            UpdateObjectFromName(Object);
        }

        private void OnObjectChanged(DependencyObject previous, DependencyObject current)
        {
            var handler = ResolvedElementChanged;

            if (null != handler)
            {
                handler(this, new NameResolvedEventArgs(previous, current));
            }
        }
    
        private void OnNameScopeLoaded(object sender, RoutedEventArgs e)
        {
            PendingReferenceElementLoad = false;

            NameScopeElement.Loaded -= OnNameScopeLoaded;

            UpdateObjectFromName(Object);
        }
    }
}
