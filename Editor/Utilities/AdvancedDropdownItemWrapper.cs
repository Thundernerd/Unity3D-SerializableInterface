using UnityEditor.IMGUI.Controls;

namespace TNRD.Utilities
{
    internal sealed class AdvancedDropdownItemWrapper : AdvancedDropdownItem
    {
        /// <inheritdoc />
        public AdvancedDropdownItemWrapper(string name)
            : base(name)
        {
        }

        public new AdvancedDropdownItemWrapper AddChild(AdvancedDropdownItem child)
        {
            if (child != null)
            {
                base.AddChild(child);
            }
            return this;
        }

        public new AdvancedDropdownItemWrapper AddSeparator()
        {
            base.AddSeparator();
            return this;
        }
    }
}
