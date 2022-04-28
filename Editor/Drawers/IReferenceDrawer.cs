using UnityEngine;

namespace TNRD.Drawers
{
    internal interface IReferenceDrawer
    {
        float GetHeight();
        void OnGUI(Rect position);
    }
}
