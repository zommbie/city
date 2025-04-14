using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

[RequireComponent(typeof(CScrollRect))]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))] // 드로우 최적화를 위해 켄버스를 분리. 분리 하지 않으면  UIFrame 전체가 리드로우 된다.	
abstract public class CUIScrollRectBase : CUIWidgetTemplateBase
{
    [Header("[Misc]")]
    [SerializeField] private int SortOrderOffset = 0;
    [SerializeField, Range(1f, 100f)] private float ScrollSensitivityScale = 1f;    // 100 = 100% 기준으로 한번 스크롤 바 입력이 들어올때 스크롤 할 양
    [SerializeField] private bool ParentDragEvent = false;                          // 스크롤 랙트가 스크롤 랙트를 포함하고 있을 경우	
    [SerializeField] private CUIContentsFollow ContentsFollow = null;               // 스크롤 바 아이템 위에 다양한 레이어 이미지를 출력하고자 할때
    
    protected CUIContentsFollow GetUIScrollContentsFollow() { return ContentsFollow; }

    private   Canvas  m_pCanvas = null;
    private   LayoutGroup       m_pContentLayoutGroup = null;
    private   ContentSizeFitter   m_pContentSizeFitter = null;
    protected CScrollRect m_pScrollRect = null;
	protected RectTransform m_pContentTransform = null;
	//-------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pUIFrameParent)
	{
		base.OnUIEntryInitialize(pUIFrameParent);
		m_pScrollRect = GetComponent<CScrollRect>();
        m_pContentLayoutGroup = m_pScrollRect.content.GetComponent<LayoutGroup>();
        m_pContentSizeFitter = m_pScrollRect.content.GetComponent<ContentSizeFitter>();
        m_pCanvas = GetComponent<Canvas>();
          
		m_pScrollRect.onValueChanged.AddListener(HandleUIScrollRatioChange);
		m_pContentTransform = m_pScrollRect.content;
        
        if (m_pScrollRect.viewport == null)
		{
			m_pScrollRect.viewport = m_pScrollRect.transform as RectTransform;
		}

		if (ParentDragEvent)
		{
			m_pScrollRect.FindParentsScrollRect();
		}
    }

	protected override void OnUIEntryChangeOrder(int iOrder)
	{
		base.OnUIEntryChangeOrder(iOrder);
        if (m_pCanvas.overrideSorting)
        {
            m_pCanvas.sortingLayerName = GetUIEntryParentsUIFrame().GetUIFrameCanvas().sortingLayerName;
            m_pCanvas.sortingOrder = iOrder + SortOrderOffset;
        }
	}

    protected override void OnUITemplateItemRequest(CUIWidgetTemplateItemBase pItem)
    {
        PrivUIScrollRefreshContentsLayout();
    }

    private void Update()
    {
        OnUIScrollUpdate();
	}

    private void LateUpdate()
    {
        OnUIScrollLateUpdate();
    }

    //--------------------------------------------------------
    public Vector2 GetUIScrollViewportSize()
    {
		return m_pScrollRect.viewport.rect.size;
    }

	public Vector2 GetUIScrollPosition()
    {
		return m_pScrollRect.content.anchoredPosition;
    }

    public Vector2 GetUIScrollRatio()
    {
        return m_pScrollRect.normalizedPosition;
    }

    public RectTransform GetUIScrollContents()
    {
        return m_pContentTransform;
    }


    public void SetUIScrollRatio(Vector2 vecScrollRatio)
    {
        vecScrollRatio.x = Mathf.Clamp(vecScrollRatio.x, 0, 1f);
        vecScrollRatio.y = Mathf.Clamp(vecScrollRatio.y, 0, 1f);
        m_pScrollRect.normalizedPosition = vecScrollRatio;
    }

    public void SetUIScrollPosition(Vector2 vecScrollPosition)
    {
        m_pScrollRect.content.anchoredPosition = vecScrollPosition;
    }

    //-------------------------------------------------------
    protected CUIWidgetTemplateItemBase ProtUIScrollChildItem(int iIndex)
	{
		CUIWidgetTemplateItemBase pChildItem = null;

		if ( iIndex < m_pScrollRect.content.childCount)
		{
			pChildItem = m_pScrollRect.content.GetChild(iIndex).gameObject.GetComponent<CUIWidgetTemplateItemBase>();
		}

		return pChildItem;
	}

	protected void ProtUIScrollSensitivityRatio(float fAdjustOffset = 1000f)
    {
		m_pScrollRect.scrollSensitivity = m_pScrollRect.content.rect.height * (ScrollSensitivityScale / fAdjustOffset);
    }

    protected void ProtUIScrollContentsLayoutRefresh()
    {
        PrivUIScrollRefreshContentsLayout();
    }

    protected void ProtUIScrollPositionReset()
    {        
        m_pScrollRect.content.anchoredPosition = Vector2.zero;
    }

    //  초기화 중에는 LateUpdate에서 호출할것
    protected void ProtUIScrollCenterPositionY(float fPositionY) 
    {
        PrivUIScrollPositionCenterClippingY(fPositionY);
    }
    // 초기화 중에는 LateUpdate에서 호출할것
    protected void ProtUIScrollCenterPositionX(float fPositionX)
    {       
        PrivUIScrollPositionCenterClippingX(fPositionX);
    }

    protected TEMPLATE ProtUIScrollRequestItem<TEMPLATE>() where TEMPLATE : CUIWidgetTemplateItemBase
    {
        TEMPLATE pInstance = DoUITemplateRequestItem<TEMPLATE>(m_pContentTransform);
        PrivUIScrollRefreshContentsLayout();
        return pInstance;
    }
    //---------------------------------------------------------
    private void PrivUIScrollRefreshContentsLayout()
    {
        if (m_pContentSizeFitter != null)
        {
            m_pContentSizeFitter.enabled = false;
            m_pContentSizeFitter.enabled = true;
        }
        m_pContentTransform.ForceUpdateRectTransforms(); // 실제 좌표 갱신은 LateUpdate에서 확인 가능 
    }
   
    // 사용시 해당 포지션은 중앙정렬이 아닌 사이드 정렬을 한 좌표를 입력할것
    private void PrivUIScrollPositionCenterClippingX(float fPositionX)
    {
        float fClipStart = m_pScrollRect.viewport.rect.width / 2f;
        float fClipEnd = m_pContentTransform.rect.width - fClipStart;
        bool bReverse = m_pContentTransform.anchorMax.x == 0 ? true : false;
        float fClipPosition = ExtractUIScrollPositionCliping(fPositionX, fClipStart, fClipEnd, bReverse);
        Vector2 vecPosition = m_pContentTransform.anchoredPosition;
        vecPosition.x = fClipPosition;
        m_pContentTransform.anchoredPosition = vecPosition;
    }

    private void PrivUIScrollPositionCenterClippingY(float fPositionY)
    {
        float fClipStart = m_pScrollRect.viewport.rect.height / 2f;
        float fClipEnd = m_pContentTransform.rect.height - fClipStart;
        bool bReverse = m_pContentTransform.anchorMax.y == 0 ? true : false;
        float fClipPosition = ExtractUIScrollPositionCliping(fPositionY, fClipStart, fClipEnd, bReverse);
       
        Vector2 vecPosition = m_pContentTransform.anchoredPosition;
        vecPosition.y = fClipPosition;
        m_pContentTransform.anchoredPosition = vecPosition;
    }

    private float ExtractUIScrollPositionCliping(float fPosition, float fClipStart, float fClipEnd, bool bReverse)
    {
        float fAbsPosition = Mathf.Abs(fPosition);
        float fScrollPosition = 0;
        float fClipPosition = 0;
        if (fAbsPosition >= fClipStart && fAbsPosition <= fClipEnd)
        {
            if (bReverse)
            {
                fScrollPosition = fAbsPosition + fClipStart;               
            }
            else
            {
                fScrollPosition = fAbsPosition - fClipStart;
            }
        }
        else if (fAbsPosition > fClipEnd)
        {
            if (bReverse)
            {
                fScrollPosition = fClipEnd;
            }
            else
            {
                fScrollPosition = -fClipEnd;
            }
        }

        if (bReverse)
        {
            fClipPosition = -(m_pContentTransform.rect.height - fScrollPosition);
        }
        else
        {
            fClipPosition = -fScrollPosition;
        }

        return fClipPosition;
    }

    //-----------------------------------------------------------
    private void HandleUIScrollRatioChange(Vector2 vecChangeValue)
	{
        OnUIScrollValueChange(vecChangeValue);
	}
    //-----------------------------------------------------------
    protected virtual void OnUIScrollValueChange(Vector2 vecScrollRatio) { }
    protected virtual void OnUIScrollUpdate() { } 
    protected virtual void OnUIScrollLateUpdate() { }
}
