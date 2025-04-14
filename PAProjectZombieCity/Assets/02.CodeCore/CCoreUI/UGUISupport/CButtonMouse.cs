using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CButtonMouse : Button , IDragHandler, IDropHandler, IPointerUpHandler, IBeginDragHandler, IPointerMoveHandler , IPointerEnterHandler
{
    [System.Serializable]
    public class ButtonClickedEventData : UnityEvent<PointerEventData> { }

    [FormerlySerializedAs("onClickRight")]
    [SerializeField]
    private ButtonClickedEventData m_OnPointDownLeft = new ButtonClickedEventData();
    [FormerlySerializedAs("onClickLeft")]
    [SerializeField]
    private ButtonClickedEventData m_OnPointDownRight = new ButtonClickedEventData();

    [FormerlySerializedAs("onLeftUp")]
    [SerializeField]
    private ButtonClickedEventData m_OnPointerUpLeft = new ButtonClickedEventData();
    [FormerlySerializedAs("onRightUp")]
    [SerializeField]
    private ButtonClickedEventData m_OnPointerUpRight = new ButtonClickedEventData();
    


    [FormerlySerializedAs("onButtonDragStart")]
    [SerializeField]
    private ButtonClickedEventData m_OnButtonDragStart = new ButtonClickedEventData();

    [FormerlySerializedAs("onButtonDragging")]
    [SerializeField]
    private ButtonClickedEventData m_OnButtonDragging = new ButtonClickedEventData();

    [FormerlySerializedAs("onButtonDragDrop")]
    [SerializeField]
    private ButtonClickedEventData m_OnButtonDragDrop = new ButtonClickedEventData();

    [FormerlySerializedAs("onMouseMove")]
    [SerializeField]
    private ButtonClickedEventData m_OnMouseMove = new ButtonClickedEventData();

    [FormerlySerializedAs("onMouseEnter")]
    [SerializeField]
    private ButtonClickedEventData m_OnMouseEnter = new ButtonClickedEventData();

    [FormerlySerializedAs("onMouseExit")]
    [SerializeField]
    private ButtonClickedEventData m_OnMouseExit = new ButtonClickedEventData();


    public ButtonClickedEventData OnPointDownLeft
	{
		get { return m_OnPointDownLeft; }
		set { m_OnPointDownLeft = value; }
	}

	public ButtonClickedEventData OnPointDownRight
    {
        get { return m_OnPointDownRight; }
        set { m_OnPointDownRight = value; }
    }

	public ButtonClickedEventData OnPointUpLeft
	{
		get { return m_OnPointerUpLeft; }
		set { m_OnPointerUpLeft = value; }
	}

	public ButtonClickedEventData OnPointUpRight
	{
		get { return m_OnPointerUpRight; }
		set { m_OnPointerUpRight = value; }
	}

    public ButtonClickedEventData OnPointEnter
    {
        get { return m_OnMouseEnter; }
        set { m_OnMouseEnter = value; }
    }

    public ButtonClickedEventData OnPointExit
    {
        get { return m_OnMouseExit; }
        set { m_OnMouseExit = value; }
    }


    //------------------------------------------------------------------
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData); // Left / Middle / Right 모두 Click으로 취급
    }


    public override void OnPointerUp(PointerEventData eventData)
    {
        PointerEventData.InputButton eButtonType = eventData.button;  // UGUI 에서는 레프트만 취급 (터치용으로 개발했는지 마우스 대응 없음)
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.button = PointerEventData.InputButton.Left;
        }
        base.OnPointerUp(eventData);

		if (!IsActive() || !IsInteractable())
			return;
		if (eButtonType == PointerEventData.InputButton.Left)
		{
            m_OnPointerUpLeft.Invoke(eventData);			
		}
		else if (eButtonType == PointerEventData.InputButton.Right)
		{
            m_OnPointerUpRight.Invoke(eventData);			
		}
	}
     
	public override void OnPointerDown(PointerEventData eventData)
	{
		PointerEventData.InputButton eButtonType = eventData.button;
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			eventData.button = PointerEventData.InputButton.Left;
		}
		base.OnPointerDown(eventData);

		if (!IsActive() || !IsInteractable())
			return;
		if (eButtonType == PointerEventData.InputButton.Left)
		{
            m_OnPointDownLeft.Invoke(eventData);			
		}
		else if (eButtonType == PointerEventData.InputButton.Right)
		{
            m_OnPointDownRight.Invoke(eventData);
		}
	}

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsActive() || !IsInteractable())
            return;
        base.OnPointerEnter(eventData);
        m_OnMouseEnter?.Invoke(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!IsActive() || !IsInteractable())
            return;
        base.OnPointerExit(eventData);
        m_OnMouseExit?.Invoke(eventData);
    }

    //-------------------------------------------------------------------------------------
    public void OnDrag(PointerEventData eventData)
    {
        if (!IsActive() || !IsInteractable())
            return;
        m_OnButtonDragging?.Invoke(eventData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!IsActive() || !IsInteractable())
            return;
        m_OnButtonDragDrop?.Invoke(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsActive() || !IsInteractable())
            return;
        m_OnButtonDragStart?.Invoke(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!IsActive() || !IsInteractable())
            return;

        m_OnMouseMove?.Invoke(eventData);
    }
}
