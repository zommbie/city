using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIScrollRectSpringBase : CUIScrollRectBase
{
    [Header("[AnchorSpring]")]
    [SerializeField] private bool AnchorSpring = false;
    [SerializeField, Range(0, 1f)] private float SpringOffsetRatioX = 0f;
    [SerializeField, Range(0, 1f)] private float SpringOffsetRatioY = 0f;
    [SerializeField] private float SpringUnderVelocity = 500f;
    [SerializeField] private AnimationCurve SpringCurve = null;

    [Header("[JumpSpring]")]
    [SerializeField] private float JumpSpringTime = 2f;
    [SerializeField] private AnimationCurve JumpSpringCurve;

    private List<CUIWidgetTemplateItemBase> m_pListScrollItems = new List<CUIWidgetTemplateItemBase>();
    private CUIWidgetTemplateItemBase m_pJumpItem = null;

    private bool m_bAnchorSpringX = false;
    private bool m_bAnchorSpringY = false;
    private float m_fAnchorSpringXTimeCurrent = 0f;
    private float m_fAnchorSpringYTimeCurrent = 0f;
    private Vector2 m_vecAnchorSpringOffset = Vector2.zero;

    private bool m_bJumpingToItem = false;
    private Vector2 m_fJumpStartPosition;
    private Vector2 m_fJumpDestPosition;
    private float m_fMaxScrollPositionX;
    private float m_fJumpSpringCurrentTime = 0;
    private bool m_bAnchorSpringCurrent;
    //---------------------------------------------------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase pUIFrameParent)
    {
        base.OnUIEntryInitialize(pUIFrameParent);

        UpdateUIScrollSpringOffset();

        PrivUIScrollRefreshScrollItems(false);

        m_bAnchorSpringCurrent = AnchorSpring;
        if (SpringCurve == null && AnchorSpring) SpringCurve = AnimationCurve.Linear(0, 0, 1, 1);
        if (JumpSpringCurve == null) JumpSpringCurve = AnimationCurve.Linear(0,0,1,1);
    }

    protected override void OnUITemplateItemReturn(CUIWidgetTemplateItemBase pItem)
    {
        base.OnUITemplateItemReturn(pItem);

        m_pListScrollItems.Remove(pItem);
    }

    protected override void OnUITemplateItemRequest(CUIWidgetTemplateItemBase pItem)
    {
        base.OnUITemplateItemRequest(pItem);

        m_pListScrollItems.Add(pItem);
    }

    protected override void OnUIScrollValueChange(Vector2 vecScrollRatio)
    {
        base.OnUIScrollValueChange(vecScrollRatio);
        if (AnchorSpring)
        {
            PrivUIScrollAnchorSpringUpdateOffset();
        }
    }

    protected override void OnUIScrollUpdate()
    {
        base.OnUIScrollUpdate();

        if (AnchorSpring)
        {
            UpdateUIScrollAnchorSpring();
        }

        UpdateUIScrollJumpSpringToItemHorizontal();
    }

    //---------------------------------------------------------------------------------------------
    public void DoUIScrollJumpSpringToItem(CUIWidgetTemplateItemBase pTrackItem, Vector2 vecTrackPositionOffset)
    {
        PrivUIScrollJumpSpringToItem(pTrackItem, vecTrackPositionOffset);
    }

    public void DoUIScrollJumpSpringStop()
    {
        PrivUIScrollJumpSpringStop();
    }

    //---------------------------------------------------------------------------------------------

    protected virtual CUIWidgetTemplateItemBase ProtUIScrollGetAnchorSpringItemByViewportOffset(Vector2 vecViewportOffset)
    {
        CUIWidgetTemplateItemBase pFindItem = null;

        if (m_bJumpingToItem == true)
            return null;

        Vector2 vecContentsPosition = GetUIPositionLeftTop(m_pContentTransform);
        for (int i = 0; i < m_pListScrollItems.Count; i++)
        {
            Vector2 vecLocalPosition = m_pListScrollItems[i].GetUIPositionLeftTop() + vecContentsPosition;
            Vector2 vecSize = m_pListScrollItems[i].GetUISize();

            Vector2 vecStart = vecLocalPosition;
            Vector2 vecEnd = new Vector2(vecStart.x + vecSize.x, vecStart.y - vecSize.y);

            if (vecViewportOffset.x >= vecStart.x && vecViewportOffset.x < vecEnd.x)
            {
                if (vecViewportOffset.y <= vecStart.y && vecViewportOffset.y > vecEnd.y)
                {
                    pFindItem = m_pListScrollItems[i];
                    break;
                }
            }
        }

        return pFindItem;
    }

    //---------------------------------------------------------------------------------------------
    private void PrivUIScrollJumpSpringToItem(CUIWidgetTemplateItemBase pJumpItem, Vector2 vecTrackPositionOffset)
    {
        AnchorSpring = false;

        m_bJumpingToItem = true;
        m_pJumpItem = pJumpItem;
        m_pScrollRect.enabled = false;

        m_fMaxScrollPositionX = -(m_pContentTransform.rect.width - m_pScrollRect.viewport.rect.width);
        m_fJumpDestPosition = -pJumpItem.GetUIPositionLeftTop() + vecTrackPositionOffset;
        m_fJumpStartPosition = m_pScrollRect.content.anchoredPosition;

        OnUIScrollJumpSpringStart(pJumpItem);
    }

    private void UpdateUIScrollAnchorSpring()
    {
        if (m_pScrollRect.IsDragging())
        {
            PrivUIScrollAnchorSpringReset();
        }
        else
        {
            UpdateUIScrollSpringOffset();
            UpdateUIScrollAnchorSpringFilter();
        }
    }

    private void UpdateUIScrollSpringOffset()
    {
        m_vecAnchorSpringOffset.x = m_pScrollRect.viewport.rect.width * SpringOffsetRatioX;
        m_vecAnchorSpringOffset.y = m_pScrollRect.viewport.rect.height * SpringOffsetRatioY * -1;  // Pivot 0 / 1 고정이기에 y축 값들은 음수 값을 가짐
    }

    private void UpdateUIScrollJumpSpringToItemHorizontal()
    {
        if (m_pJumpItem == null)
            return;

        Vector2 fCurrentScrollPosition = m_pScrollRect.content.anchoredPosition;
        m_fJumpSpringCurrentTime += Time.deltaTime;

        float fPercent = m_fJumpSpringCurrentTime / JumpSpringTime;
        float fCurvePosition = JumpSpringCurve.Evaluate(fPercent);

        fCurrentScrollPosition.x = Mathf.Lerp(m_fJumpStartPosition.x, m_fJumpDestPosition.x, fCurvePosition);

        if (m_fJumpSpringCurrentTime >= JumpSpringTime)
        {
            PrivUIScrollJumpSpringItemEnd(m_fJumpDestPosition);
        }

        if (fCurrentScrollPosition.x < m_fMaxScrollPositionX)
        {
            PrivUIScrollJumpSpringItemEnd(m_fJumpDestPosition);
        }

        m_pScrollRect.content.anchoredPosition = fCurrentScrollPosition;
    }

    //---------------------------------------------------------------------------------------------
    private void PrivUIScrollAnchorSpringReset()
    {
        m_bAnchorSpringX = false;
        m_bAnchorSpringY = false;
        m_fAnchorSpringXTimeCurrent = 0;
        m_fAnchorSpringYTimeCurrent = 0;
    }

    private void UpdateUIScrollAnchorSpringFilter()
    {
        CUIWidgetTemplateItemBase pItem = ProtUIScrollGetAnchorSpringItemByViewportOffset(m_vecAnchorSpringOffset);

        if (pItem == null) return;

        Vector2 vecVelocity = m_pScrollRect.velocity;
        Vector2 vecContentPosition = GetUIPositionLeftTop(m_pContentTransform);
        Vector2 vecSize = pItem.GetUISize();

        Vector2 vecOffsetPositionStart = (pItem.GetUIPositionLeftTop() + vecContentPosition);
        Vector2 vecOffsetPositionEnd = new Vector2(vecOffsetPositionStart.x + vecSize.x, vecOffsetPositionStart.y - vecSize.y);

        Vector2 vecOffsetPosition = PrivUIScrollGetCloestVecFromOffset(vecOffsetPositionStart, vecOffsetPositionEnd);

        Vector2 vecDestPosition = m_vecAnchorSpringOffset - vecOffsetPosition + vecContentPosition;

        if (m_pScrollRect.horizontal)
        {
            if (PrivUIScrollCheckAnchorActivity(true))
            {
                if (vecDestPosition.x != m_pContentTransform.anchoredPosition.x)
                {
                    if (Mathf.Abs(vecVelocity.x) < SpringUnderVelocity) // 스크롤의 가속도를 정지시키고 별도의 커브로 이동시켜준다.
                    {
                        vecVelocity.x = 0;
                        m_bAnchorSpringX = true;
                    }
                }
            }
        }

        if (m_pScrollRect.vertical)
        {
            if (PrivUIScrollCheckAnchorActivity(false))
            {
                if (vecDestPosition.y != m_pContentTransform.anchoredPosition.y)
                {
                    if (Mathf.Abs(vecVelocity.y) < SpringUnderVelocity)
                    {
                        vecVelocity.y = 0;
                        m_bAnchorSpringY = true;
                    }
                }
            }
        }

        m_pScrollRect.velocity = vecVelocity;

        if (m_bAnchorSpringX)
        {
            UpdateUIScrollAnchorSpringHorizontal(pItem, vecContentPosition, vecDestPosition.x);
        }
        if (m_bAnchorSpringY)
        {
            UpdateUIScrollAnchorSpringVertical(pItem, vecContentPosition, vecDestPosition.y);
        }
    }

    private void UpdateUIScrollAnchorSpringHorizontal(CUIWidgetTemplateItemBase pItem, Vector2 vecAnchorPosition, float fDestPosition)
    {
        float fCurveValue = 0;
        float fMoveValue = 0;
        float fVelocity = 0;

        m_fAnchorSpringXTimeCurrent += Time.deltaTime;
        fCurveValue = SpringCurve.Evaluate(m_fAnchorSpringXTimeCurrent);
        fVelocity = m_pScrollRect.decelerationRate * SpringUnderVelocity * fCurveValue;

        if (fDestPosition - vecAnchorPosition.x > 0)
        {
            fMoveValue = vecAnchorPosition.x + fVelocity;
            fMoveValue = Mathf.Clamp(fMoveValue, vecAnchorPosition.x, fDestPosition);
        }
        else if (fDestPosition - vecAnchorPosition.x < 0)
        {
            fMoveValue = vecAnchorPosition.x - fVelocity;
            fMoveValue = Mathf.Clamp(fMoveValue, fDestPosition, vecAnchorPosition.x);
        }


        if (fMoveValue == fDestPosition)
        {
            m_bAnchorSpringX = false;
            m_fAnchorSpringXTimeCurrent = 0;
            OnUIScrollAnchorSpringEnd();
        }
        else
        {
            OnUIScrollAnchorSpringRemainOffsetX(pItem, fDestPosition - fMoveValue);
        }
        vecAnchorPosition.x = fMoveValue;
        m_pContentTransform.anchoredPosition = vecAnchorPosition;
    }

    private void UpdateUIScrollAnchorSpringVertical(CUIWidgetTemplateItemBase pItem, Vector2 vecAnchorPosition, float fDestPosition)
    {
        float fCurveValue = 0;
        float fMoveValue = 0;
        float fVelocity = 0;

        m_fAnchorSpringYTimeCurrent += Time.deltaTime;
        fCurveValue = SpringCurve.Evaluate(m_fAnchorSpringYTimeCurrent);
        fVelocity = m_pScrollRect.decelerationRate * SpringUnderVelocity * fCurveValue;

        if (fDestPosition - vecAnchorPosition.y > 0)
        {
            fMoveValue = vecAnchorPosition.y + fVelocity;
            fMoveValue = Mathf.Clamp(fMoveValue, vecAnchorPosition.y, fDestPosition);
        }
        else if (fDestPosition - vecAnchorPosition.y < 0)
        {
            fMoveValue = vecAnchorPosition.y - fVelocity;
            fMoveValue = Mathf.Clamp(fMoveValue, fDestPosition, vecAnchorPosition.y);
        }


        if (fMoveValue == fDestPosition)
        {
            m_bAnchorSpringY = false;
            m_fAnchorSpringXTimeCurrent = 0;
            OnUIScrollAnchorSpringEnd();
        }
        else
        {
            OnUIScrollAnchorSpringRemainOffsetY(pItem, fDestPosition - fMoveValue);
        }

        vecAnchorPosition.y = fMoveValue;
        m_pContentTransform.anchoredPosition = vecAnchorPosition;
    }

    private Vector2 PrivUIScrollGetCloestVecFromOffset(Vector2 vecStart, Vector2 vecEnd)
    {
        float startDis;
        float endDis;
        if (m_pScrollRect.horizontal)
        {
            startDis = m_vecAnchorSpringOffset.x - vecStart.x;
            endDis = vecEnd.x - m_vecAnchorSpringOffset.x;
        }
        else
        {
            startDis = vecStart.y - m_vecAnchorSpringOffset.y;
            endDis = m_vecAnchorSpringOffset.y - vecEnd.y;
        }
        return startDis < endDis ? vecStart : vecEnd;
    }

    private void PrivUIScrollAnchorSpringUpdateOffset()
    {
        int iChildTotal = m_pScrollRect.content.childCount;
        Vector2 vecAnchorOffset = GetUIPositionLeftTop(m_pContentTransform);
        vecAnchorOffset.x = Mathf.Abs(vecAnchorOffset.x) + m_vecAnchorSpringOffset.x;
        vecAnchorOffset.y = Mathf.Abs(vecAnchorOffset.y) + m_vecAnchorSpringOffset.y;

        for (int i = 0; i < iChildTotal; i++)
        {
            CUIWidgetTemplateItemBase pItem = m_pScrollRect.content.GetChild(i).GetComponent<CUIWidgetTemplateItemBase>();
            if (pItem != null)
            {
                OnUIScrollAnchorSpringUpdatePosition(pItem, vecAnchorOffset);
            }
        }
    }

    private bool PrivUIScrollCheckAnchorActivity(bool bHorizontal)
    {
        bool bActivity = false;
        if (bHorizontal)
        {
            if (m_pScrollRect.normalizedPosition.x != 1f && m_pScrollRect.normalizedPosition.x != 0f)
            {
                bActivity = true;
            }
        }
        else
        {
            if (m_pScrollRect.normalizedPosition.y != 1f && m_pScrollRect.normalizedPosition.y != 0f)
            {
                bActivity = true;
            }
        }
        return bActivity;
    }

    private void PrivUIScrollJumpSpringItemEnd(Vector2 vecStopPosition)
    {
        m_pScrollRect.content.anchoredPosition = vecStopPosition;

        if (m_bJumpingToItem == true)
        {
            m_pJumpItem = null;
            m_pScrollRect.enabled = true;
            m_fJumpSpringCurrentTime = 0;
            m_bJumpingToItem = false;
            AnchorSpring = m_bAnchorSpringCurrent;
            OnUIScrollJumpSpringEnd();
        }
    }

    private void PrivUIScrollRefreshScrollItems(bool bClearList = true)
    {
        if (bClearList) m_pListScrollItems.Clear();

        int Total = m_pScrollRect.content.childCount;
        for (int i = 0; i < Total; i++)
        {
            m_pListScrollItems.Add(m_pScrollRect.content.GetChild(i).gameObject.GetComponent<CUIWidgetTemplateItemBase>());
        }
    }

    private void PrivUIScrollJumpSpringStop()
    {
        PrivUIScrollJumpSpringItemEnd(m_pScrollRect.content.anchoredPosition);
    }

    //-----------------------------------------------------------
    protected virtual void OnUIScrollAnchorSpringRemainOffsetX(CUIWidgetTemplateItemBase pItem, float fRemainOffsetX) { }
    protected virtual void OnUIScrollAnchorSpringRemainOffsetY(CUIWidgetTemplateItemBase pItem, float fRemainOffsetY) { }
    protected virtual void OnUIScrollAnchorSpringUpdatePosition(CUIWidgetTemplateItemBase pItem, Vector2 vecAnchorPosition) { }
    protected virtual void OnUIScrollAnchorSpringEnd() { }
    protected virtual void OnUIScrollJumpSpringStart(CUIWidgetTemplateItemBase pItem) { }
    protected virtual void OnUIScrollJumpSpringEnd() { }
}
