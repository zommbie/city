using UnityEngine;
using UnityEngine.UI;

// 켄버스를 사용하므로 드로우콜을 발생시킴
// 자동으로 부모 프레임의 쏘트 오더를 확인하여 갱신
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public abstract class CUIWidgetCanvasBase : CUIWidgetBase
{
	[SerializeField]
	private int SortingOrderOffset = 1;

	private int m_iUIFrameOrder = 0;
	private Canvas m_pWidgetCanvas = null;

	//-------------------------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);
		m_pWidgetCanvas = GetComponent<Canvas>();		
		Canvas pCanvas = pParentFrame.GetUIFrameCanvas();
		m_iUIFrameOrder = pCanvas.sortingOrder;

        PrivUIWidgetCanvasSortingOrder();
    }

	protected override void OnUIEntryChangeOrder(int iOrder)
	{
		base.OnUIEntryChangeOrder(iOrder);
		m_iUIFrameOrder = iOrder;
		if (IsShow)
        {
			PrivUIWidgetCanvasSortingOrder();
		}
	}

    protected override void OnUIWidgetShowHide(bool bShow)
    {
        base.OnUIWidgetShowHide(bShow);
		if (bShow)
        {
			PrivUIWidgetCanvasSortingOrder();
		}
    }

    protected override void OnUIEntryUIFrameShowHide(bool bShow)
    {
		if (bShow)
        {
			PrivUIWidgetCanvasSortingOrder();
		}
    }

    protected override void OnUIWidgetAdd(CUIFrameBase pParentFrame)
	{
		base.OnUIWidgetAdd(pParentFrame);
	}

    protected override void OnUnityStart()
    {
        PrivUIWidgetCanvasSortingOrder();
    }
    //----------------------------------------------------------------------
    private void PrivUIWidgetCanvasSortingOrder()
    {
        if (m_pWidgetCanvas != null)
        {
            m_pWidgetCanvas.overrideSorting = true;
            m_pWidgetCanvas.sortingOrder = m_iUIFrameOrder + SortingOrderOffset;
        }
	}
}
