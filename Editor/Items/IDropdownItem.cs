namespace TNRD.Items
{
    internal interface IDropdownItem
    {
        internal ReferenceMode Mode { get; }
        object GetValue();
    }
}
