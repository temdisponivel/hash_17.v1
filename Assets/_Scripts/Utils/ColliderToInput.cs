using UnityEngine;
using UnityEngine.EventSystems;

namespace Hash17.Utils
{
    public class ColliderToInput : MonoBehaviour
    {
        public UIInput InputToFocusOn;

        void OnClick()
        {
            InputToFocusOn.selectAllTextOnFocus = false;
            InputToFocusOn.isSelected = true;
        }
    }
}