using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hash17.Terminal_
{
    public class InputHelper : MonoBehaviour
    {
        public enum InputType
        {
            KeyUp,
            KeyDown,
        }

        public List<KeyCode> KeyCode;
        public InputType Type = InputType.KeyDown;

        public List<EventDelegate> Notify;

        void Update()
        {
            Func<KeyCode, bool> functionToValidate = null;
            switch (Type)
            {
                case InputType.KeyUp:
                    functionToValidate = Input.GetKeyUp;
                    break;
                case InputType.KeyDown:
                    functionToValidate = Input.GetKeyDown;
                    break;
            }

            bool validated = false;
            for (int i = 0; i < KeyCode.Count; i++)
            {
                if (functionToValidate(KeyCode[i]))
                {
                    validated = true;
                    break;
                }
            }

            if (validated)
            {
                EventDelegate.Execute(Notify);
            }
        }
    }
}