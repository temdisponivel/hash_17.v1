using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHelper : MonoBehaviour
{
    public enum InputType
    {
        KeyUp,
        KeyDown,
    }

    public KeyCode KeyCode = KeyCode.None;
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

        if (functionToValidate(KeyCode))
        {
            EventDelegate.Execute(Notify);
        }
    }
}