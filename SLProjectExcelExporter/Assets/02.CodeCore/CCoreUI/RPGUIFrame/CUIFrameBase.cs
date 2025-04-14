using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public abstract class CUIFrameBase : CMonoBase, IPointerClickHandler
{
	[SerializeField] 
	private CManagerUIFrameFocusBase.EUIFrameFocusType FocusType = CManagerUIFrameFocusBase.EUIFrameFocusType.Panel;

	private bool					m_bShow = false;			public bool IsShow { get { return m_bShow; } }				// 현재 보이는지 안 보이는지 
	private bool					m_bDisAppear = false;		public bool IsAppear { get { return m_bDisAppear; } }	    // 다른 프레임에 의해 임으로 가려진 상태
	private int					    m_iSortOrder = 0;			public int  p_SortOrder { get { return m_iSortOrder; } }
	private Canvas                  m_pCanvas;	                public Canvas GetUIFrameCanvas() { return m_pCanvas; }
	private GraphicRaycaster        m_pGraphicRaycaster;
	private RectTransform           m_pRectTransform;
	private Vector2                 m_vecUIFrameSize = Vector2.zero;

	//--------------------------------------------------------------------------
	internal void InterUIFrameInitialize()	// UI프레임이 메니저에 의해 로드된 이후 호출
	{
		m_pCanvas = GetComponent<Canvas>();
		m_pRectTransform = GetComponent<RectTransform>();
		m_pGraphicRaycaster = GetComponent<GraphicRaycaster>();

		m_vecUIFrameSize.x = m_pRectTransform.rect.width;
		m_vecUIFrameSize.y = m_pRectTransform.rect.height;

		SetMonoActive(true);
		m_pCanvas.overrideSorting = true;
		m_pCanvas.sortingLayerName = LayerMask.LayerToName(gameObject.layer);
		SetMonoActive(false);
		OnUIFrameInitialize();
	}

	internal void InterUIFrameInitializePost()
	{
		OnUIFrameInitializePost();
	}

	internal void InterUIFrameScriptDataLoaded()
	{
		OnUIFrameScriptDataLoaded();
	}

	internal void InterUIFrameShow(int iOrder)
	{
		InterUIFrameRefreshOrder(iOrder);
		
		m_bShow = true;
		m_bDisAppear = false;

		if (gameObject.activeSelf == true)
		{
			OnUIFrameRefresh();
		}
		else
		{
			SetMonoActive(true);
			OnUIFrameShow();
		}
	}

	internal void InterUIFrameAppear()             // 다른 프레임에 의해 임으로 가려진 상태 내부 상태는 Show
	{
        if (m_bDisAppear)
		{
			SetMonoActive(true);
			m_bDisAppear = false;
			m_bShow = true;
			OnUIFrameAppear();			
		}
	}

	internal void InterUIFrameDisappear()
	{
		if (m_bDisAppear == false)
		{
			SetMonoActive(false);
			m_bDisAppear = true;
			OnUIFrameDisappear();
		}
	}

	internal void InterUIFrameRefreshOrder(int iOrder)	// 자신의 오더가 변경되었을 경우
	{
		if (iOrder != m_iSortOrder)
		{
			m_iSortOrder = iOrder;
			m_pCanvas.sortingOrder = iOrder;
			OnUIFrameChangeOrder(iOrder);
		}
    }

	internal void InterUIFrameHide()
	{
		SetMonoActive(false);
		m_bShow = false;
		m_bDisAppear = false;
		OnUIFrameHide();
	}

	internal void InterUIFrameForceHide()
	{
		SetMonoActive(false);
		m_bShow = false;
		OnUIFrameForceHide();
	}

	internal void InterUIFrameRemove()
	{
		OnUIFrameRemove();
	}

	internal void InterUIFrameClose()  // 자신이 아닌 외부에 의해 종료되었다. (디바이스 뒤로가기 버튼)
	{
		OnUIFrameClose();
	}

	public object SendUIFrameMessage(int hMessageID, int iArg = 0, float fArg = 0f, string strArg = null, params object [] aParams) // Inter 함수가 난립하는 것을 막기 위한 공용 수신 함수 
	{
		return OnUIFrameMessage(hMessageID, iArg, fArg, strArg, aParams);
	}
 
    //---------------------------------------------------------------------
    private void Update()
	{
		OnUIFrameUpdate();
	}

	//---------------------------------------------------------------------  
	public void DoUIFrameSelfHide()
    {
		CManagerUIFrameUsageBase.Instance.UIHide(this);
    }

	public void DoUIFrameSelfShow()
	{
		CManagerUIFrameUsageBase.Instance.UIShow(this);
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        OnUIFramePointerClick(eventData);
    }

	public void SetUIFrameInputEnable(bool bEnable)
	{
		m_pGraphicRaycaster.enabled = bEnable;
	}
   
    //--------------------------------------------------------------------------
    protected virtual void OnUIFrameInitialize() { }		// 이 단계에서는 다른 UIFrame을 인식 할 수 없다.
	protected virtual void OnUIFrameInitializePost() { }	// 모든 UIFrame 업로드되어 인식할 수 있다.
	protected virtual void OnUIFrameScriptDataLoaded() { }
	protected virtual void OnUIFrameShow() { }
	protected virtual void OnUIFrameRefresh() { }
	protected virtual void OnUIFrameChangeOrder(int iOrder) { }
	protected virtual void OnUIFrameHide() { }
	protected virtual void OnUIFrameForceHide() { }
	protected virtual void OnUIFrameRemove() { }
	protected virtual void OnUIFrameUpdate() { }
	protected virtual void OnUIFrameAppear() { }
	protected virtual void OnUIFrameDisappear() { }
	protected virtual void OnUIFrameClose() { }
	protected virtual void OnUIFramePointerClick(PointerEventData eventData) { }
	protected virtual object OnUIFrameMessage(int hMessageID, int iArg, float fArg, string strArg, params object[] aParams) { return null; }
	//--------------------------------------------------------------------------
	public CManagerUIFrameFocusBase.EUIFrameFocusType GetUIFrameFocusType() { return FocusType; }
}
