namespace LibraProgramming.Windows.Interactivity
{
    public sealed class TriggerCollection : LoadableAttachableCollection<TriggerBase>
    {
        internal override void ItemAdded(TriggerBase item)
        {
            if (null == AttachedObject)
            {
                return;
            }

            item.Attach(AttachedObject);
        }

        internal override void ItemRemoved(TriggerBase item)
        {
            if (null == ((IAttachedObject)item).AttachedObject)
            {
                return;
            }

            item.Detach();
        }

        protected override void OnAttached()
        {
            foreach (var tigger in this)
            {
                tigger.Attach(AttachedObject);
            }
        }

        protected override void OnDetaching()
        {
            foreach (var tigger in this)
            {
                tigger.Detach();
            }
        }
    }
}
