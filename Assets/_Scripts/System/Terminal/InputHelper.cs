using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hash17.Utils;

namespace MockSystem.Term
{
    public class InputHelper : MonoBehaviour
    {
        #region Inner types

        public enum InputType
        {
            KeyUp,
            KeyDown,
        }

        [Serializable]
        public class TupleKeyCode : Tuple<KeyCode, InputType>
        {
            
        }

        [Serializable]
        public class TupleEvents : Tuple<TupleKeyCode, List<EventDelegate>>
        {
            
            public TupleEvents()
            {
                Key = new TupleKeyCode();
                Value = new List<EventDelegate>();
            }
        }

        #endregion

        public List<TupleEvents> EventList = new List<TupleEvents>();

        void Update()
        {
            for (int i = 0; i < EventList.Count; i++)
            {
                var type = EventList[i].Key.Value;
                var keyCode = EventList[i].Key.Key;
                var notify = EventList[i].Value;

                Func<KeyCode, bool> functionToValidate = null;
                switch (type)
                {
                    case InputType.KeyUp:
                        functionToValidate = Input.GetKeyUp;
                        break;
                    case InputType.KeyDown:
                        functionToValidate = Input.GetKeyDown;
                        break;
                }

                if (functionToValidate(keyCode))
                {
                    EventDelegate.Execute(notify);
                }
            }
        }
    }
}