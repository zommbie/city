using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("UICustom/CScroll Rect", 37)]

public class CScrollRect : ScrollRect
{
	private bool m_bDisableScrollInput = false;
    private bool m_bDrag = false;                       public bool IsDragging() { return m_bDrag; }
	private ScrollRect mParentsScrollRect = null;
	//---------------------------------------------------------------------------------------
	
	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
        m_bDrag = true;
		mParentsScrollRect?.OnBeginDrag(eventData);
	}

	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
        m_bDrag = true;
        mParentsScrollRect?.OnDrag(eventData);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
        m_bDrag = false;
        mParentsScrollRect?.OnEndDrag(eventData);
	}

	public override void OnInitializePotentialDrag(PointerEventData eventData)
	{
		base.OnInitializePotentialDrag(eventData);
		mParentsScrollRect?.OnInitializePotentialDrag(eventData);
	}

    public override void OnScroll(PointerEventData data)
    {
		if (m_bDisableScrollInput == false)
        {
            base.OnScroll(data);
        }    
    }

    //-------------------------------------------------------------------------------------
    public override void Rebuild(CanvasUpdate executing)
	{
		base.Rebuild(executing);
		if (verticalScrollbar is CScrollBar)
		{
			CScrollBar pScrollbar = verticalScrollbar as CScrollBar;
			pScrollbar.OnScrollRectRebuild();
		}
		else if (horizontalScrollbar is CScrollBar)
		{
			CScrollBar pScrollbar = horizontalScrollbar as CScrollBar;
			pScrollbar.OnScrollRectRebuild();
		}
	} 

	protected override void LateUpdate()
    {
		base.LateUpdate();
        if (verticalScrollbar is CScrollBar)
        {
            CScrollBar pScrollbar = verticalScrollbar as CScrollBar;
            pScrollbar.OnScrollRectLateUpdate();
        }
        else if (horizontalScrollbar is CScrollBar)
        {
            CScrollBar pScrollbar = horizontalScrollbar as CScrollBar;
            pScrollbar.OnScrollRectLateUpdate();
        }
    }

	//--------------------------------------------------------------------------------------
	public void FindParentsScrollRect()
	{
		Transform parentTransform = gameObject.transform;
		while(parentTransform = parentTransform.parent)
		{
			mParentsScrollRect = parentTransform.gameObject.GetComponent<ScrollRect>();
			if (mParentsScrollRect != null)
				break;
		}
	}

	public void SetScrollRectInputlDisable(bool bDisable)
    {
		m_bDisableScrollInput = bDisable;
    }
}
