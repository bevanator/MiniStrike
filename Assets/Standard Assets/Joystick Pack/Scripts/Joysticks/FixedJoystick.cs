using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    public event Action<bool> PointerChange;

    public override void OnPointerDown(PointerEventData eventData)
    {
        PointerChange?.Invoke(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        PointerChange?.Invoke(false);
        base.OnPointerUp(eventData);
    }
}