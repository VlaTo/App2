namespace LibraProgramming.Windows.Interactivity
{
    public sealed class BehaviorCollection : LoadableAttachableCollection<Behavior>
    {
        internal override void ItemAdded(Behavior item)
        {
            if (null == AttachedObject)
            {
                return;
            }

            item.Attach(AttachedObject);

        }

        internal override void ItemRemoved(Behavior item)
        {
            if (null == ((IAttachedObject) item).AttachedObject)
            {
                return;
            }

            item.Detach();

        }

        protected override void OnAttached()
        {
            foreach (var behavior in this)
            {
                behavior.Attach(AttachedObject);
            }
        }

        protected override void OnDetaching()
        {
            foreach (var behavior in this)
            {
                behavior.Detach();
            }
        }
    }
}
