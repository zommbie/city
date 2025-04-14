using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// 높이 정렬만 지원하는 무한 스크롤 바
// 각 항목의 높이가 다를 경우에도 동작하며 런타임에 수정 가능 
// 항목의 갯수와 관계 없이 최소한의 연산을 수행하도록 내부 로직 개선

public abstract class CUIScrollRectInfiniteVerticalBase : CUIScrollRectBase
{
    private class SItemHeightMap
    {
        public int iIndex = 0;
        public float fPositionY = 0;
        public float fHeight = 0;
    }

    private int m_iScrollPositionStart = 0; protected int p_ScrollPositionStart { get { return m_iScrollPositionStart; } }
    private int m_iScrollPositionEnd = 0; protected int p_ScrollPositionEnd { get { return m_iScrollPositionEnd; } }
    private float m_fTotalScrollHeightPixel = 0;  protected float p_TotalScrollHeight { get { return m_fTotalScrollHeightPixel; } }
    private List<SItemHeightMap> m_listItemHeightSizeMap = new List<SItemHeightMap>();
    private CLinkedList<CUIWidgetTemplateItemBase> m_listItemContents = new CLinkedList<CUIWidgetTemplateItemBase>();
    //---------------------------------------------------------------------
    private void OnRectTransformDimensionsChange()
    {
        if (m_pScrollRect != null)
        {
            m_pScrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_fTotalScrollHeightPixel);
        }
    }

    //-----------------------------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase _UIFrameParent)
    {
        base.OnUIEntryInitialize(_UIFrameParent);     
        m_pScrollRect.movementType = ScrollRect.MovementType.Clamped;  
        m_pScrollRect.content.pivot = new Vector2(0f, 1f); 
    }

    protected override void OnUIScrollValueChange(Vector2 vecChangeValue)
    {
        base.OnUIScrollValueChange(vecChangeValue);
        PrivUIScrollInfiniteUpdate();
    }

    //-----------------------------------------------------------------------------
    protected void ProtUIScrollInfiniteVerticalInitilize(List<float> pListItemHeightMap)
    {
        if (pListItemHeightMap.Count == 0) return;
        PrivUIScrollInfiniteVerticalResetAll();
        PrivUIScrollInfiniteHeightMapMake(pListItemHeightMap);
        ProtUIScrollSensitivityRatio();
        PrivUIScrollInfiniteUpdate();      
    }

    protected void ProtUIScrollInfiniteVerticalPosition(int iIndex)
    {
        SItemHeightMap pItemHeight = FindUIScrollItemHeightMap(iIndex);
        if (pItemHeight != null)
        {
            PrivUIScrollInfiniteVerticalPosition(-pItemHeight.fPositionY);
        }
    }

    /// <summary>
    /// 좌상단이 0 Position
    /// </summary>  
    protected void ProtUIScrollInfiniteVerticalPosition(float fPosition)  
    {
        PrivUIScrollInfiniteVerticalPosition(fPosition);
    }

    protected void ProtUIScrollInfiniteResizeItem(int iIndex, float fResizeHeight)
    {
        SItemHeightMap pItemHeight = FindUIScrollItemHeightMap(iIndex);
        if (pItemHeight == null) return;

        float fResizeValue = pItemHeight.fHeight - fResizeHeight;
        m_fTotalScrollHeightPixel -= fResizeValue;     
        pItemHeight.fHeight = fResizeHeight;

        float fResizeCurrent = pItemHeight.fPositionY;
        for (int i = iIndex; i < m_listItemHeightSizeMap.Count; i++)
        {
            m_listItemHeightSizeMap[i].fPositionY = fResizeCurrent;
            fResizeCurrent -= m_listItemHeightSizeMap[i].fHeight;
        }

        PrivUIScrollResizeContentsHeight(m_fTotalScrollHeightPixel);
        PrivUIScrollInifiniteVericalResetItem();
        PrivUIScrollInfiniteUpdate();
    }

    //--------------------------------------------------------------------------------
    private void PrivUIScrollInfiniteVerticalPosition(float fPosition)
    {
        Vector2 vecViewPortSize = m_pScrollRect.viewport.rect.size;
        float fAnchorPosition = fPosition;
        if (fAnchorPosition + vecViewPortSize.y > m_fTotalScrollHeightPixel)
        {
            fAnchorPosition = m_fTotalScrollHeightPixel - vecViewPortSize.y;
        }

        m_pScrollRect.content.anchoredPosition = new Vector2(m_pScrollRect.content.anchoredPosition.x, fAnchorPosition);
        GetUIScrollContentsFollow()?.DoUIContentsFollowUpdate();
        PrivUIScrollInfiniteUpdate();
    }

    private void PrivUIScrollInfiniteVerticalResetAll()
    {
        PrivUIScrollInifiniteVericalResetItem();
        m_listItemHeightSizeMap.Clear();       
    }

    private void PrivUIScrollInifiniteVericalResetItem()
    {
        CLinkedList<CUIWidgetTemplateItemBase>.Enumerator it = m_listItemContents.GetEnumerator();
        while (it.MoveNext())
        {
            OnUIScrollInfiniteVerticalItemRemove(it.Current);
            it.Current.DoUITemplateItemReturn();
        }
        m_listItemContents.Clear();
    }

    private void PrivUIScrollInfiniteHeightMapMake(List<float> pListItemHeightMap)
    {
        float fTotalSize = 0;
        for (int i = 0; i < pListItemHeightMap.Count; i++)
        {
            SItemHeightMap pNewItem = new SItemHeightMap();
            pNewItem.fPositionY = -fTotalSize;
            pNewItem.fHeight = pListItemHeightMap[i];
            pNewItem.iIndex = i;
            m_listItemHeightSizeMap.Add(pNewItem);
            fTotalSize += pNewItem.fHeight;
        }

        PrivUIScrollResizeContentsHeight(fTotalSize);       
    }

    private void PrivUIScrollInfiniteUpdate()
    {
        Vector2 vecPosition = m_pScrollRect.content.anchoredPosition;
        Vector2 vecViewPortSize = m_pScrollRect.viewport.rect.size;
        PrivUIScrollInfiniteClippingItem(vecPosition.y, vecViewPortSize.y);
        PrivUIScrollInfiniteUpdateTailToBottom(vecPosition.y, vecViewPortSize.y);
        PrivUIScrollInfiniteUpdateHeadToTop(vecPosition.y, vecViewPortSize.y);
        PrivUIScrollInfiniteRefreshIndex();
    }

    private void PrivUIScrollInfiniteUpdateTailToBottom(float fAnchorPositionY, float fViewPortSizeY)
    {
        int iTailIndex = 0;
        float fTailPosition = 0;
        float fViewTopPosition = -fAnchorPositionY;
        float fViewBottomPosition = -(fAnchorPositionY + fViewPortSizeY);

        CUIWidgetTemplateItemBase pTailItem = PrivUIScrollExtractItemPositionY(false);
        if (pTailItem != null)
        {
            iTailIndex = pTailItem.GetUITemplateItemIndex() + 1;
            SItemHeightMap pItemHeightMap = FindUIScrollItemHeightMap(iTailIndex);
            if (pItemHeightMap == null)
            {
                return; // 스크롤 최 하단 도달
            }
            fTailPosition = pItemHeightMap.fPositionY;
        }

        if (fTailPosition > fViewTopPosition)
        {
            SItemHeightMap pItemHeightMap = FindUIScrollItemFromItemPosition(iTailIndex, fViewTopPosition, true);
            if (pItemHeightMap == null)
            {
                //Error!
                return;
            }
            else
            {
                fTailPosition = pItemHeightMap.fPositionY;
                iTailIndex = pItemHeightMap.iIndex;
            }
        }

        PrivUIScrollInfiniteFillItemDownward(fTailPosition, fViewBottomPosition, iTailIndex);
    }    

    private void PrivUIScrollInfiniteUpdateHeadToTop(float fAnchorPositionY, float fViewPortSizeY)
    {
        int  iHeadIndex = 0;
        float fHeadPosition = 0;
        float fViewTopPosition = -fAnchorPositionY;
        float fViewBottomPosition = -(fAnchorPositionY + fViewPortSizeY);

        CUIWidgetTemplateItemBase pHeadItem = PrivUIScrollExtractItemPositionY(true);
        if (pHeadItem != null)
        {
            iHeadIndex = pHeadItem.GetUITemplateItemIndex() - 1;
            SItemHeightMap pItemHeightMap = FindUIScrollItemHeightMap(iHeadIndex);
            if (pItemHeightMap == null)
            {
                return; // 스크롤 최 상단 도달
            }
            fHeadPosition = pItemHeightMap.fPositionY - pItemHeightMap.fHeight;
        }

        if (fHeadPosition < fViewBottomPosition)
        {
            SItemHeightMap pItemHeightMap = FindUIScrollItemFromItemPosition(iHeadIndex, fViewBottomPosition, false);
            if (pItemHeightMap == null)
            {
                //Error!
                return;
            }
            else
            {
                fHeadPosition = pItemHeightMap.fPositionY;
                iHeadIndex = pItemHeightMap.iIndex;
            }
        }

        PrivUIScrollInfiniteFillItemTopward(fHeadPosition, fViewTopPosition, iHeadIndex);
    }

    private void PrivUIScrollInfiniteFillItemDownward(float fFillPosition, float fFillEndPosition, int iStartIndex)
    {
        while (true)
        {
            if (fFillPosition >= fFillEndPosition)
            {
                float fAddSize = PrivUIScrollInifiniteRequestItem(iStartIndex++, false);
                if (fAddSize == 0)
                {
                    break;
                }
                else
                {
                    fFillPosition -= fAddSize;
                }
            }
            else
            {
                break;
            }
        }
    }

    private void PrivUIScrollInfiniteFillItemTopward(float fFillPosition, float fFillEndPosition, int iStartIndex)
    {
        float fAddSize = 0;
        while (true)
        {           
            if (fFillPosition <= fFillEndPosition)
            {
                fAddSize = PrivUIScrollInifiniteRequestItem(iStartIndex--, true);
                if (fAddSize == 0)
                {
                    break;
                }
                else
                {
                    fFillPosition += fAddSize;
                }
            }
            else
            {
                break;
            }
        }
    }

    private void PrivUIScrollInfiniteClippingItem(float fAnchorPositionY, float fViewPortHeight)
    {
        float fClippingStart = -fAnchorPositionY;
        float fClippingEnd = -(fAnchorPositionY + fViewPortHeight);
        CLinkedList<CUIWidgetTemplateItemBase>.Enumerator it = m_listItemContents.GetEnumerator();
        while (it.MoveNext())
        {
            float fClippingPositionTop = it.Current.GetUIPositionY() - it.Current.GetUIHeight();
            float fClippingPositionBottom = it.Current.GetUIPositionY();
            if (fClippingPositionTop > fClippingStart || fClippingPositionBottom < fClippingEnd)
            {
                OnUIScrollInfiniteVerticalItemRemove(it.Current);
                it.Current.DoUITemplateItemReturn();
                it.Remove();
            }            
        }
    }

    private void PrivUIScrollInfiniteRefreshIndex()
    {
        if (m_listItemContents.Count == 0) return;
        m_iScrollPositionStart = m_listItemContents.First.Value.GetUITemplateItemIndex();
        m_iScrollPositionEnd = m_listItemContents.First.Value.GetUITemplateItemIndex();
    }

    //---------------------------------------------------------------------------------
    private float PrivUIScrollInifiniteRequestItem(int iIndex, bool bHead)
    {
        if (iIndex < 0 || iIndex >= m_listItemHeightSizeMap.Count) return 0;

        SItemHeightMap pItemHeight = m_listItemHeightSizeMap[iIndex];
        CUIWidgetTemplateItemBase pNewItem = DoUITemplateRequestItem(m_pScrollRect.content);
        pNewItem.SetUIAnchor(EAnchorPresets.TopLeft);
        pNewItem.SetUIPivot(new Vector2(0, 1f));  // UpperLeft와 좌표 동기화를 위해
        pNewItem.SetUIPositionY(pItemHeight.fPositionY);
        pNewItem.SetUIHeight(pItemHeight.fHeight);

        pNewItem.DoUITemplateItemShow(true);
        pNewItem.DoUITemplateItemRefreshIndex(pItemHeight.iIndex);

        if (bHead)
        {
            pNewItem.transform.SetAsFirstSibling();
        }
        m_listItemContents.AddLast(pNewItem);
        OnUIScrollInfiniteVerticalItem(pItemHeight.iIndex, pNewItem);
        return pItemHeight.fHeight;
    }

    private CUIWidgetTemplateItemBase PrivUIScrollExtractItemPositionY(bool bHead)
    {
        if (m_pScrollRect.content.transform.childCount == 0) return null;

        CUIWidgetTemplateItemBase pItem = null;
        int iIndex = 0;
        if (bHead == false)
        {
            iIndex = m_pScrollRect.content.transform.childCount - 1;   
        }
        Transform pTransform = m_pScrollRect.content.transform.GetChild(iIndex);
        if (pTransform != null)
        {
            pItem = pTransform.gameObject.GetComponent<CUIWidgetTemplateItemBase>();         
        }

        return pItem;
    }

    private SItemHeightMap FindUIScrollItemHeightMap(int iIndex)
    {
        SItemHeightMap pItemHeight = null;
        if (iIndex >= 0 && iIndex < m_listItemHeightSizeMap.Count)
        {
            pItemHeight = m_listItemHeightSizeMap[iIndex];
        }

        return pItemHeight;
    }

    private SItemHeightMap FindUIScrollItemFromItemPosition(int iStartIndex, float fFindPosition, bool bFindDown)
    {
        SItemHeightMap pFindItem = null;
        int iFindIndex = iStartIndex;
       
        while(true)
        {
            SItemHeightMap pItemHeight = FindUIScrollItemHeightMap(iFindIndex);

            if (bFindDown)
            {
                iFindIndex++;
            }
            else
            {
                iFindIndex--;
            }

            if (pItemHeight == null)
            {
                break;
            }
            else
            {
                if (fFindPosition <= pItemHeight.fPositionY && fFindPosition >= pItemHeight.fPositionY - pItemHeight.fHeight)
                {
                    pFindItem = pItemHeight;
                    break;
                }
            }
        }

        return pFindItem;
    }

    private void PrivUIScrollResizeContentsHeight(float fTotalFixelSize)
    {
        m_fTotalScrollHeightPixel = fTotalFixelSize;
        m_pScrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_fTotalScrollHeightPixel);

        GetUIScrollContentsFollow()?.DoUIContentsFollowUpdate();
    }
    //------------------------------------------------------------------------
    protected virtual void OnUIScrollInfiniteVerticalItem(int iIndex, CUIWidgetTemplateItemBase pNewItem) { }
    protected virtual void OnUIScrollInfiniteVerticalItemRemove(CUIWidgetTemplateItemBase pRemoveItem) { }
}

