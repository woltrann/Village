using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    private Vector2 originalPosition; // ✅ İlk pozisyonu kaydetmek için

    protected override void Start()
    {
        base.Start();
        originalPosition = background.anchoredPosition; // ✅ İlk pozisyonu al
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.anchoredPosition = originalPosition; // ✅ İlk pozisyona dön
        base.OnPointerUp(eventData);
    }
}
