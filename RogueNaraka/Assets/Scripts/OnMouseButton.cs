using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("CustomButton/OnMouseButton")]
public class OnMouseButton : Button {

    [SerializeField]
    ButtonEvent _onDown = new ButtonEvent();
    [SerializeField]
    ButtonEvent _onUp = new ButtonEvent();
    [SerializeField]
    ButtonEvent _onEnter = new ButtonEvent();
    [SerializeField]
    ButtonEvent _onExit = new ButtonEvent();

    protected OnMouseButton() { }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _onDown.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _onUp.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _onEnter.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _onExit.Invoke();
    }

    public ButtonEvent onDown
    {
        get { return _onDown; }
        set { _onDown = value; }
    }
    public ButtonEvent onUp
    {
        get { return _onUp; }
        set { _onUp = value; }
    }

    public ButtonEvent onEnter
    {
        get { return _onEnter; }
        set { _onEnter = value; }
    }
    public ButtonEvent onExit
    {
        get { return _onExit; }
        set { _onExit = value; }
    }
}
