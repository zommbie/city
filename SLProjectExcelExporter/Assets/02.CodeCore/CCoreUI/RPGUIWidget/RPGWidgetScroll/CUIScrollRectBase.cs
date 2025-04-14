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
    [SerializeField, Range(1f, 100f)] private float ScrollSensitivityScale = 1f;    // 100 = 100% 기준으로 한번 스크롤 바 입력이 들어올때 스크롤 할 양
    [SerializeField] private bool ParentDragEvent = false;                          // 스크롤 랙트가 스크롤 랙트를 포함하고 있을 경우	
    [SerializeField] private CUIContentsFollow ContentsFollow = null;               // 스크롤 바 아이템 위에 다양한 레이어 이미지를 출력하고자 할때
    protected CUIContentsFollow GetUIScrollContentsFollow() { return ContentsFollow; }

    private Canvas  m_pCanvas = null;
	protected CScrollRect m_pScrollRect = null;
	protected RectTransform m_pContentTransform = null;
	//-------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pUIFrameParent)
	{
		base.OnUIEntryInitialize(pUIFrameParent);
		m_pScrollRect = GetComponent<CScrollRect>();
        m_pCanvas = GetComponent<Canvas>();
        
        m_pCanvas.overrideSorting = true;
        m_pCanvas.sortingLayerName = pUIFrameParent.GetUIFrameCanvas().sortingLayerName;

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
        m_pCanvas.sortingOrder = iOrder + 1;
	}

	private void Update()
    {
        OnUIScrollUpdate();
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

	protected List<TEMPLATE> ProtUIScrollChildItem<TEMPLATE>() where TEMPLATE : CUIWidgetTemplateItemBase
	{
		List<TEMPLATE> pListTemplate = new List<TEMPLATE>();
		for (int i = 0; i < m_pScrollRect.content.childCount; i++)
        {
			TEMPLATE pTempate = m_pScrollRect.content.GetChild(i).gameObject.GetComponent<TEMPLATE>();
			if (pTempate != null)
            {
                pListTemplate.Add(pTempate);
            }
        }
		return pListTemplate;
    }

	protected void ProtUIScrollSensitivityRatio(float fAdjustOffset = 1000f)
    {
		m_pScrollRect.scrollSensitivity = m_pScrollRect.content.rect.height * (ScrollSensitivityScale / fAdjustOffset);
    }
    //-----------------------------------------------------------
	private void HandleUIScrollRatioChange(Vector2 vecChangeValue)
	{
        OnUIScrollValueChange(vecChangeValue);
	}


    //-----------------------------------------------------------
    protected virtual void OnUIScrollValueChange(Vector2 vecScrollRatio) { }
    protected virtual void OnUIScrollUpdate() { } 
}
