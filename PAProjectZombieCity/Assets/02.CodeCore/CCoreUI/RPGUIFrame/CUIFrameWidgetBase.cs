using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 자식 위젯을 관리하기 위한 레이어
abstract public class CUIFrameWidgetBase : CUIFrameBase
{
    private List<CUIWidgetBase> m_listWidgetInstance = new List<CUIWidgetBase>();
    //--------------------------------------------------------------
    protected override void OnUIFrameInitialize() 
    {
       	PrivUIFrameInitializeUIWidget(); 
    }

    protected override void OnUIFrameInitializePost()
    {
        PrivUIFrameInitializeUIWidgetPost();
    }

    protected override void OnUIFrameChangeOrder(int iOrder) 
    {
        for (int i = 0; i < m_listWidgetInstance.Count; i++)
        {
            m_listWidgetInstance[i].InterUIEntryChangeOrder(iOrder);
        }
    }

    protected override void OnUIFrameShow()
    {
        for (int i = 0; i < m_listWidgetInstance.Count; i++)
        {
            m_listWidgetInstance[i].InterUIEntryUIFramwShowHide(true);
        }
    }

    protected override void OnUIFrameHide()
    {
        for (int i = 0; i < m_listWidgetInstance.Count; i++)
        {
            m_listWidgetInstance[i].InterUIEntryUIFramwShowHide(false);
        }
    }

    //------------------------------------------------------------------
    internal void InterUIWidgetAdd(CUIWidgetBase pWidget)
    {
        PrivUIFrameChildWidgetAdd(pWidget);
    }

    internal void InterUIWidgetDelete(CUIWidgetBase pWidget)
    {
        PrivUIFrameChildWidgetRemove(pWidget);
    }

    //-------------------------------------------------------------------------
    private void PrivUIFrameInitializeUIWidget()
    {
        GetComponentsInChildren(true, m_listWidgetInstance);

        for (int i = 0; i < m_listWidgetInstance.Count; i++)
        {
            m_listWidgetInstance[i].InterUIEntryInitialize(this);
        }
    }

    private void PrivUIFrameInitializeUIWidgetPost()
    {
        for (int i = 0; i < m_listWidgetInstance.Count; i++)
        {
            m_listWidgetInstance[i].InterUIEntryInitializePost(this);
        }
    }

    private bool PrivUIFrameCloseWindow()
    {
        bool bAllClose = true;

        return bAllClose;
    }

    private void PrivUIFrameChildWidgetAdd(CUIWidgetBase pWidgetAdd)
	{
        List<CUIWidgetBase> pListWidget = new List<CUIWidgetBase>();
        pWidgetAdd.GetComponentsInChildren(true, pListWidget);

        for (int i = 0; i < pListWidget.Count; i++)
		{
            m_listWidgetInstance.Add(pListWidget[i]);
            pListWidget[i].InterUIWidgetAdd(this);
		}

		for (int i = 0; i < pListWidget.Count; i++)
		{			
			pListWidget[i].InterUIEntryInitialize(this);
            pListWidget[i].InterUIEntryChangeOrder(GetUIFrameSortOrder());
		}

		for (int i = 0; i < pListWidget.Count; i++)
		{
			pListWidget[i].InterUIEntryInitializePost(this);
		}
	}

	private void PrivUIFrameChildWidgetRemove(CUIWidgetBase pWidgetDelete)
	{
		List<CUIWidgetBase> pListWidget = new List<CUIWidgetBase>();
        pWidgetDelete.GetComponentsInChildren(pListWidget);

		for (int i = 0; i < pListWidget.Count; i++)
		{
			m_listWidgetInstance.Remove(pListWidget[i]);
			pListWidget[i].InterUIWidgetRemove(this);
		}
	}

    //---------------------------------------------------
    
}
