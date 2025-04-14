using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public abstract class CUIWidgetTextDisplayItemBase : CUIWidgetTemplateItemBase
{
    private enum ETextDirection
    {
        None,
        Left,
        Right,
    }

    [SerializeField]
    private CTextMeshProUGUI TMPInstance = null;
    [SerializeField]
    private float MinTextWidth = 10;
    [SerializeField]
    private float PingPongDelay = 0.5f;

    private int   m_iDisplayIndex = 0;        public int GetTextDisplayItemIndex() { return m_iDisplayIndex; }
    private float m_fTextSizeWidth = 0;     public float GetTextDisplayTextWidth() { return m_fTextSizeWidth; }  // UISize는 갱신이 늦게 되므로 이 값을 써야함
    private float m_fSpeedValue = 0;

    private bool m_bDisplayShowRight = false;
    private bool m_bDisplayShowLeft = false;
    private bool m_bDisplayFinish = false;
    private bool m_bDisplayEnd = false;
    private bool m_bUpdateDisplayMove = true;

    private Vector2 m_vecDisplaySize = Vector2.zero;
    private ETextDirection m_eTextDirection = ETextDirection.None;
    private CUIWidgetTextDisplayBase.IEventTextDisplay m_pEventHandler = null;
    private CUIWidgetTextDisplayBase.ETextDisplayType m_eTextDisplayType = CUIWidgetTextDisplayBase.ETextDisplayType.None;
    //---------------------------------------------------------------------
    internal void InterTextDisplayStart(CUIWidgetTextDisplayBase.IEventTextDisplay pEventHandler, CUIWidgetTextDisplayBase.ETextDisplayType eTextDisplayType, string strTextMessage, int iDisplayIndex, float fSpeedValue, Vector2 vecStartLocalPosition, Vector2 vecDisplaySize)
    {
        m_pEventHandler = pEventHandler;
        m_eTextDisplayType = eTextDisplayType;
        m_iDisplayIndex = iDisplayIndex;
        m_fSpeedValue = fSpeedValue;
        m_vecDisplaySize = vecDisplaySize;
        DoUIWidgetShowHide(true);
        SetUIPosition(vecStartLocalPosition);
        PrivTextDisplayItemReset();
        PrivTextDisplayItemSetDirection(eTextDisplayType);
        PrivTextDisplayItemMessage(strTextMessage);       
        OnTextDisplayItemStart();
    }

    //-------------------------------------------------------------------

    public bool UpdateTextDisplayItem(float fDelta)
    {
        if (m_bUpdateDisplayMove)
        {
            UpdateTextDisplayItemPosition(m_eTextDirection, fDelta);
            OnTextDisplayItemUpdate(fDelta);
        }
        return m_bDisplayEnd;
    }
    //---------------------------------------------------------------------
    private void PrivTextDisplayItemReset()
    {
        CancelInvoke();
        m_bDisplayShowRight = false;
        m_bDisplayShowLeft = false;
        m_bDisplayFinish = false;
        m_bDisplayEnd = false;
        m_bUpdateDisplayMove = true;
    }

    private void PrivTextDisplayItemSetDirection(CUIWidgetTextDisplayBase.ETextDisplayType eTextDisplayType)
    {
        if (eTextDisplayType == CUIWidgetTextDisplayBase.ETextDisplayType.None)
        {
            m_eTextDirection = ETextDirection.None;
        }
        else if (eTextDisplayType == CUIWidgetTextDisplayBase.ETextDisplayType.MoveSlideLeft)
        {
            m_eTextDirection = ETextDirection.Left;
        }
        else if (eTextDisplayType == CUIWidgetTextDisplayBase.ETextDisplayType.StopNoMove)
        {
            m_eTextDirection = ETextDirection.None;
        }
        else if (eTextDisplayType == CUIWidgetTextDisplayBase.ETextDisplayType.StopSlidePingPong)
        {
            m_eTextDirection = ETextDirection.Left; //오른쪽 먼저 
        }
    }

    private void PrivTextDisplayItemMessage(string strTextMessage)
    {
        TMPInstance.text = strTextMessage;
        m_fTextSizeWidth = TMPInstance.preferredWidth < MinTextWidth ? MinTextWidth : TMPInstance.preferredWidth;
    }

    private void UpdateTextDisplayItemPosition(ETextDirection eTextDirection, float fDelta)
    {
        float fMoveValue = m_fSpeedValue * fDelta;
        Vector2 vecPosition = GetUIPosition();
        if (eTextDirection == ETextDirection.Left)
        {
            vecPosition.x -= fMoveValue;
        }   
        else if (eTextDirection == ETextDirection.Right)
        {
            vecPosition.x += fMoveValue;
        }
        SetUIPosition(vecPosition);
        PrivTextDisplayItemCheckEvent(vecPosition);
    }

    private void PrivTextDisplayItemCheckEvent(Vector2 vecPosition)
    {       
        if (m_eTextDisplayType == CUIWidgetTextDisplayBase.ETextDisplayType.MoveSlideLeft)
        {
            PrivTextDisplayItemCheckEventMoveSlideLeft(vecPosition);
        }
        else if (m_eTextDisplayType == CUIWidgetTextDisplayBase.ETextDisplayType.StopSlidePingPong)
        {
            PrivTextDisplayItemCheckEventStopSlidePingPong(vecPosition);
        }
    }

    private void PrivTextDisplayItemCheckEventMoveSlideLeft(Vector2 vecPosition)
    {
        float fOffsetX = vecPosition.x + m_fTextSizeWidth;
        if (fOffsetX <= m_vecDisplaySize.x && m_bDisplayShowRight == false)
        {
            PrivTextDisplayItemShowRight();
            m_pEventHandler.ITextDisplayNext(this, m_iDisplayIndex);
        }

        if (fOffsetX <= 0 && m_bDisplayFinish == false)
        {
            PrivTextDisplayItemEnd();
        }
    }

    private void PrivTextDisplayItemCheckEventStopSlidePingPong(Vector2 vecPosition)
    {
        if (m_eTextDirection == ETextDirection.Left)
        {
            float fOffsetX = vecPosition.x + m_fTextSizeWidth;
            if (fOffsetX <= m_vecDisplaySize.x && m_bDisplayShowRight == false)
            {
                PrivTextDisplayItemShowRight();
                m_bDisplayShowLeft = false;
                m_eTextDirection = ETextDirection.Right;
                PrivTextDisplayItemPauseMove();
            }
        }
        else if (m_eTextDirection == ETextDirection.Right)
        {
            if (vecPosition.x >= 0 && m_bDisplayShowLeft == false)
            {
                PrivTextDisplayItemShowLeft();
                m_bDisplayShowRight = false;
                m_eTextDirection = ETextDirection.Left;
                PrivTextDisplayItemPauseMove();
            }
        }
    }

    private void PrivTextDisplayItemPauseMove()
    {
        m_bUpdateDisplayMove = false;
        Invoke("HandleTextDisplayItemResumeUpdate", PingPongDelay);
    }

    private void PrivTextDisplayItemShowRight()
    {
        m_bDisplayShowRight = true;
        OnTextDisplayShowRight();
    }

    private void PrivTextDisplayItemShowLeft()
    {
        m_bDisplayShowLeft = true;
        OnTextDisplayShowLeft();
    }

    private void PrivTextDisplayItemEnd()
    {
        m_bDisplayEnd = true;
        m_pEventHandler.ITextDisplayShowEnd(this, m_iDisplayIndex);
        OnTextDisplayEnd();
    }

    //--------------------------------------------------------------------
    private void HandleTextDisplayItemResumeUpdate()
    {
        m_bUpdateDisplayMove = true;
    }

    //---------------------------------------------------------------------
    protected virtual void OnTextDisplayItemStart() { }
    protected virtual void OnTextDisplayItemUpdate(float fDelta) { }
    protected virtual void OnTextDisplayShowRight() { }
    protected virtual void OnTextDisplayShowLeft() { }
    protected virtual void OnTextDisplayEnd() { }
    
}
