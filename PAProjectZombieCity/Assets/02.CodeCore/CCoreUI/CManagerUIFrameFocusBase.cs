using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public abstract class CManagerUIFrameFocusBase : CManagerUIFrameBase
{
	private const int c_OrderGap = 10;


	public enum EUIFrameFocusType
	{
		None,
		Invisible,          // 항상 보여진다. 별도로 관리되지 않는다. 주로 Image없이 기능적인 작동만 하는 경우에 사용한다.
		ToolTip,            // 항상 Topmost 로 배열된다. 툴팁은 하나만 보여진다. 
		Panel,              // Show 순서에 따라 정렬되어 출력된다.
		PanelChain,         // 마지막 프레임만 Show된다. Hide시 이전 패널이 Show된다. 클로즈 버튼에 대응한다.
		PanelTopMost,       // 패널중 항상 위쪽에 위치한다. 
		PanelFullScreen,    // Show시 모든  Panel을 Hide 시킨다. 하나의 포커스만 존재한다. 		
		Popup,              // Panel보다 항상 위에 그려진다.   
		PopupExclusive,     // Popup을 모두 Hide시키고 자신을 보여준다. 닫힐 경우 이전 Popup이 출력된다. 
	}

	private int m_iCanvasOrder = 0; // UGUI 인스펙터에서 셋팅된 켄버스 오프셋 
    private CUIFrameBase m_pFullScreenUIFrame = null;

    private LinkedList<CUIFrameBase> m_listFrameOrderPanel = new LinkedList<CUIFrameBase>();
	private LinkedList<CUIFrameBase> m_listFrameOrderPopup = new LinkedList<CUIFrameBase>();
	private LinkedList<CUIFrameBase> m_listFramePanelChain = new LinkedList<CUIFrameBase>();
	private LinkedList<CUIFrameBase> m_listFrameTopMost = new LinkedList<CUIFrameBase>();

    private List<CUIFrameBase> m_listUIFrameNote = new List<CUIFrameBase>();
	//--------------------------------------------------------------
	protected override void OnUIMgrInitializeCanvas(Canvas pRootCanvas)
	{
		base.OnUIMgrInitializeCanvas(pRootCanvas);
		m_iCanvasOrder = pRootCanvas.sortingOrder;
	}
	//--------------------------------------------------------------
	protected void ProtMgrUIFrameFocusShow(CUIFrameBase pUIFrame)
	{
		PrivMgrUIFrameFocusShow(pUIFrame);
	}

	protected CUIFrameBase ProtMgrUIFrameFocusShow(string strFrameName)
	{
		CUIFrameBase pUIFrame = FindUIFrame(strFrameName);
		if (pUIFrame != null)
		{
			PrivMgrUIFrameFocusShow(pUIFrame);
		}
		else
		{
			Debug.LogError("[UIFrame] Invalid UIFrame : " + strFrameName);
		}
		return pUIFrame;
	}

	protected void ProtMgrUIFrameFocusHide(CUIFrameBase pUIFrame)
	{
		PrivMgrUIFrameFocusHide(pUIFrame);
	}

	protected CUIFrameBase ProtMgrUIFrameFocusHide(string strFrameName)
	{
		CUIFrameBase pUIFrame = FindUIFrame(strFrameName);
		if (pUIFrame != null)
		{
			PrivMgrUIFrameFocusHide(pUIFrame);
		}
		return pUIFrame;
	}

	protected void ProtMgrUIFrameFocusClose()
	{
		if (m_listFramePanelChain.Last != null)
		{                    
            PrivMgrUIFrameFocusClose();
		}
		else
		{
			OnMgrUIFramePanelChainEmpty();
		}
	}

	protected void ProtMgrUIFrameFocusPanelHideAll()
	{
		LinkedList<CUIFrameBase>.Enumerator itOrderPanel = m_listFrameOrderPanel.GetEnumerator();
		while (itOrderPanel.MoveNext())
		{
			itOrderPanel.Current.InterUIFrameForceHide();
		}
		m_listFrameOrderPanel.Clear();

		LinkedList<CUIFrameBase>.Enumerator itTopMost = m_listFrameTopMost.GetEnumerator();
		while (itTopMost.MoveNext())
		{
			itTopMost.Current.InterUIFrameForceHide();
		}
		m_listFrameTopMost.Clear();

		m_listFramePanelChain.Clear();

		if (m_pFullScreenUIFrame != null)
		{
			m_pFullScreenUIFrame.InterUIFrameForceHide();
			m_pFullScreenUIFrame = null;
		}
	}

	//---------------------------------------------------------------
	private void PrivMgrUIFrameFocusShow(CUIFrameBase pUIFrame)
	{
		EUIFrameFocusType eFocusType = pUIFrame.GetUIFrameFocusType();
		switch (eFocusType)
		{
			case EUIFrameFocusType.Invisible:
				PrivMgrUIFrameFocusInvisibleShow(pUIFrame);
				break;
			case EUIFrameFocusType.ToolTip:
				break;
			case EUIFrameFocusType.Panel:
				PrivMgrUIFrameFocusPanelShow(pUIFrame);
				break;
			case EUIFrameFocusType.PanelChain:
				PrivMgrUIFrameFocusPanelChainShow(pUIFrame);
				break;
			case EUIFrameFocusType.PanelFullScreen:
				PrivMgrUIFrameFocusFullScreenShow(pUIFrame);
				break;
			case EUIFrameFocusType.PanelTopMost:
				PrivMgrUIFrameFocusPannelTopMostShow(pUIFrame);
				break;
			case EUIFrameFocusType.Popup:
				PrivMgrUIFrameFocusPopupShow(pUIFrame);
				break;
			case EUIFrameFocusType.PopupExclusive:
				PrivMgrUIFrameFocusPopupShow(pUIFrame);
				break;
		}
	}

	private void PrivMgrUIFrameFocusHide(CUIFrameBase pUIFrame)
	{
		if (pUIFrame.IsShow == false) return;

		EUIFrameFocusType eFocusType = pUIFrame.GetUIFrameFocusType();
		switch (eFocusType)
		{
			case EUIFrameFocusType.Invisible: // 인비지블은 하이드 되지 않는다.
				break;
			case EUIFrameFocusType.ToolTip:
				break;
			case EUIFrameFocusType.Panel:
				PrivMgrUIFrameFocusPanelHide(pUIFrame);
				break;
			case EUIFrameFocusType.PanelChain:
				PrivMgrUIFrameFocusPanelChainHide(pUIFrame);
				break;
			case EUIFrameFocusType.PanelFullScreen:
				PrivMgrUIFrameFocusFullScreenHide(pUIFrame);
				break;
			case EUIFrameFocusType.PanelTopMost:
				PrivMgrUIFrameFocusPanelTopMostHide(pUIFrame);
				break;
			case EUIFrameFocusType.Popup:
				PrivMgrUIFrameFocusPopupHide(pUIFrame);
				break;
			case EUIFrameFocusType.PopupExclusive:
				PrivMgrUIFrameFocusPopupHide(pUIFrame);
				break;
		}
	}

	//---------------------------------------------------------------

	private void PrivMgrUIFrameFocusPanelShow(CUIFrameBase pUIFrame)
	{
		m_listFrameOrderPanel.Remove(pUIFrame);
		m_listFrameOrderPanel.AddLast(pUIFrame);

        pUIFrame.InterUIFrameShow(pUIFrame.GetUIFrameSortOrder());

        PrivMgrUIFrameArrageRefresh(m_listFrameOrderPanel, m_iCanvasOrder);
		PrivMgrUIFrameArrageRefresh(m_listFrameTopMost, ExtractUIOrderPanelTopMost());

		OnMgrUIFrameShow(pUIFrame);
	}

	private void PrivMgrUIFrameFocusFullScreenShow(CUIFrameBase pUIFrame)
	{
		if (m_pFullScreenUIFrame)
		{
			PrivMgrUIFrameFocusPanelHide(m_pFullScreenUIFrame);
		}

		m_pFullScreenUIFrame = pUIFrame;

		PrivMgrUIFrameAppearDisappear(m_listFrameOrderPanel, false);
		PrivMgrUIFrameAppearDisappear(m_listFrameTopMost, false);

		m_pFullScreenUIFrame.InterUIFrameShow(ExtractUIOrderFullScreenPanel());

		CManagerStageCameraBase.Instance?.DoStageCameraRenderEnableAll(false);

		OnMgrUIFrameShow(pUIFrame);
	}

	private void PrivMgrUIFrameFocusPanelChainShow(CUIFrameBase pUIFrame)
	{
		PrivMgrUIFrameFocusPanelShow(pUIFrame);
		m_listFramePanelChain.Remove(pUIFrame);
		m_listFramePanelChain.AddLast(pUIFrame);
	}

	private void PrivMgrUIFrameFocusPannelTopMostShow(CUIFrameBase pUIFrame)
	{
		m_listFrameTopMost.Remove(pUIFrame);
		m_listFrameTopMost.AddLast(pUIFrame);
		int iOrder = ExtractUIOrderPanelTopMost();
		PrivMgrUIFrameArrageRefresh(m_listFrameTopMost, iOrder);
		pUIFrame.InterUIFrameShow(pUIFrame.GetUIFrameSortOrder());
		OnMgrUIFrameShow(pUIFrame);
	}

	private void PrivMgrUIFrameFocusInvisibleShow(CUIFrameBase pUIFrame)
	{
		pUIFrame.InterUIFrameShow(0);
	}

	private void PrivMgrUIFrameFocusPanelHide(CUIFrameBase pUIFrame)
	{
		m_listFrameOrderPanel.Remove(pUIFrame);
		pUIFrame.InterUIFrameHide();
		PrivMgrUIFrameArrageRefresh(m_listFrameOrderPanel, m_iCanvasOrder);
		OnMgrUIFrameHide(pUIFrame);
	}

	private void PrivMgrUIFrameFocusPanelChainHide(CUIFrameBase pUIFrameClose)
    {
        m_listFramePanelChain.Remove(pUIFrameClose);
        pUIFrameClose.InterUIFrameClose();
        OnMgrUIFrameClose(pUIFrameClose);
        PrivMgrUIFrameFocusPanelHide(pUIFrameClose);		
	}

	private void PrivMgrUIFrameFocusFullScreenHide(CUIFrameBase pUIFrame)
	{
		if (m_pFullScreenUIFrame == pUIFrame)
		{
			PrivMgrUIFrameAppearDisappear(m_listFrameOrderPanel, true);
			PrivMgrUIFrameAppearDisappear(m_listFrameTopMost, true);

			PrivMgrUIFrameArrageRefresh(m_listFrameOrderPanel, m_iCanvasOrder);
			PrivMgrUIFrameArrageRefresh(m_listFrameTopMost, m_iCanvasOrder);

			pUIFrame.InterUIFrameHide();
			m_pFullScreenUIFrame = null;
			CManagerStageCameraBase.Instance?.DoStageCameraRenderEnableAll(true);
			OnMgrUIFrameHide(pUIFrame);
		}
	}

	private void PrivMgrUIFrameFocusPanelTopMostHide(CUIFrameBase pUIFrame)
	{
		m_listFrameTopMost.Remove(pUIFrame);
		pUIFrame.InterUIFrameHide();
		PrivMgrUIFrameArrageRefresh(m_listFrameTopMost, ExtractUIOrderPanelTopMost());
	}

	private void PrivMgrUIFrameFocusClose()
	{
        CUIFrameBase pUIFrameClose = m_listFramePanelChain.Last.Value;      
        PrivMgrUIFrameFocusPanelChainHide(m_listFramePanelChain.Last.Value);            
	}

	private void PrivMgrUIFrameFocusPopupShow(CUIFrameBase pUIFrame)
	{
		m_listFrameOrderPopup.Remove(pUIFrame);
		m_listFrameOrderPopup.AddLast(pUIFrame);

		PrivMgrUIFrameArrageRefresh(m_listFrameOrderPopup, ExtractUIOrderPopup());

		pUIFrame.InterUIFrameShow(pUIFrame.GetUIFrameSortOrder());
		OnMgrUIFrameShow(pUIFrame);
	}

	private void PrivMgrUIFrameFocusPopupHide(CUIFrameBase pUIFrame)
	{
		m_listFrameOrderPopup.Remove(pUIFrame);
		pUIFrame.InterUIFrameHide();
		PrivMgrUIFrameArrageRefresh(m_listFrameOrderPopup, ExtractUIOrderPopup());
		OnMgrUIFrameHide(pUIFrame);
	}
	//------------------------------------------------------------------
	private int PrivMgrUIFrameArrageRefresh(LinkedList<CUIFrameBase> pListUIFrameOrder, int iBaseOrder)
	{
        LinkedList<CUIFrameBase>.Enumerator it = pListUIFrameOrder.GetEnumerator();
        int iOrder = iBaseOrder;
        while (it.MoveNext())
        { 
            it.Current.InterUIFrameRefreshOrder(iOrder);
            iOrder += c_OrderGap;
        }

        return iOrder;
	}

	private void PrivMgrUIFrameAppearDisappear(LinkedList<CUIFrameBase> pListUIFrame, bool bAppear)
	{
		LinkedList<CUIFrameBase>.Enumerator it = pListUIFrame.GetEnumerator();
		while (it.MoveNext())
		{
			if (bAppear)
			{
				it.Current.InterUIFrameAppear();
			}
			else
			{
				it.Current.InterUIFrameDisappear();
			}
		}
	}

	//-----------------------------------------------------------
	private int ExtractUIOrderPanel()
	{
		return m_listFrameOrderPanel.Count * c_OrderGap + m_iCanvasOrder;
	}

	private int ExtractUIOrderPanelTopMost()
	{
		return ExtractUIOrderPanel() + c_OrderGap;
	}

	private int ExtractUIOrderFullScreenPanel()
	{
		return ExtractUIOrderPanelTopMost() + (c_OrderGap * c_OrderGap);
	}

	private int ExtractUIOrderPopup()
	{
		return ExtractUIOrderFullScreenPanel() + c_OrderGap;
	}
	
	//------------------------------------------------------------------
	protected virtual void OnMgrUIFrameShow(CUIFrameBase pUIFrame) { }	
	protected virtual void OnMgrUIFrameHide(CUIFrameBase pUIFrame) { }
	protected virtual void OnMgrUIFrameClose(CUIFrameBase pUIFrame) { }
	protected virtual void OnMgrUIFrameShowFromOther(CUIFrameBase pUIFrame) { }
	protected virtual void OnMgrUIFrameHideFromOther(CUIFrameBase pUIFrame) { }
	protected virtual void OnMgrUIFramePanelChainEmpty() { }	
}
