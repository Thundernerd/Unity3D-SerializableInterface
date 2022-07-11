using UnityEditor.IMGUI.Controls;

namespace TNRD.Items
{
    public class NoneDropdownItem  : AdvancedDropdownItem, IDropdownItem
    {
        private ReferenceMode mode => ReferenceMode.Raw;
        public NoneDropdownItem() : base("None") { }

        ReferenceMode IDropdownItem.Mode => mode;

        public object GetValue()
        {
            return default;
        }
    }
}