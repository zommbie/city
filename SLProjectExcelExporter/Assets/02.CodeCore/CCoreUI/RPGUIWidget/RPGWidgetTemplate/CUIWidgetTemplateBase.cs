using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public abstract class CUIWidgetTemplateBase : CUIWidgetBase
{
	[SerializeField]
	private CUIWidgetTemplateItemBase		TemplateItem = null;

	private List<CUIWidgetTemplateItemBase> m_listCloneInstance = new List<CUIWidgetTemplateItemBase>(); 
	//--------------------------------------------------------------------------------
	protected override void OnUnityAwake()
	{
		base.OnUnityAwake();
		if (TemplateItem != null)
		{
			TemplateItem.DoUITemplateItemShow(false);
		}
	}

    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {
        base.OnUIEntryInitialize(pParentFrame);
    }

    //--------------------------------------------------------------------------------
	public TEMPLATE DoUITemplateRequestItem<TEMPLATE>(Transform pParent = null) where TEMPLATE : CUIWidgetTemplateItemBase
    {
		return DoUITemplateRequestItem(pParent) as TEMPLATE;
	}

	public CUIWidgetTemplateItemBase DoUITemplateRequestItem(Transform pParent = null)
	{
		CUIWidgetTemplateItemBase pItem = null;

		for (int i = 0; i < m_listCloneInstance.Count; i++)
		{
			if (m_listCloneInstance[i].IsItemActivate() == false)
			{
				pItem = m_listCloneInstance[i];
				break;
			}
		}

		if (pItem == null)
		{
			pItem = MakeUITemplateItem();
		}

		if (pParent)
		{
			pItem.transform.SetParent(pParent, false);
		}
        else
        {
            pItem.transform.SetParent(transform, false);
        }

		pItem.DoUITemplateItemShow(true);
		pItem.InterUITemplateItemActivate();
		pItem.InterUIEntryOwner(this);

		OnUITemplateItemRequest(pItem);

		return pItem;
	}

	public void DoUITemplateReturnAll()
	{
		for (int i = 0; i < m_listCloneInstance.Count; i++)
		{
			PrivUITemplateReturn(m_listCloneInstance[i]);
		}
	}

	public void DoUITemplateReturn(CUIWidgetTemplateItemBase pItem) // 비용이 상당하므로 업데이트 호출 주의
	{
		PrivUITemplateReturn(pItem);
	}

	
	//--------------------------------------------------------
	private CUIWidgetTemplateItemBase MakeUITemplateItem()
	{
		CUIFrameWidgetBase pParentUIFrame = GetUIEntryParentsUIFrame() as CUIFrameWidgetBase;
		if (pParentUIFrame == null)
        {
			Debug.LogErrorFormat($"[UIWidgetTemplate] ParentsUI is NULL : Call OnUIWidgetInitializePost Logic instead of OnUIWidgetInitialize Fuction Call");
			return null;
        }

		GameObject NewInstance = Instantiate(TemplateItem.gameObject);
		CUIWidgetTemplateItemBase pNewItem = NewInstance.GetComponent<CUIWidgetTemplateItemBase>();
		m_listCloneInstance.Add(pNewItem);
		pParentUIFrame.InterUIWidgetAdd(pNewItem);
        return pNewItem;
	}

	private void PrivUITemplateReturn(CUIWidgetTemplateItemBase pItem)
	{
		CUIFrameWidgetBase pParentUIFrame = GetUIEntryParentsUIFrame() as CUIFrameWidgetBase;

		pItem.DoUITemplateItemShow(false);
		pItem.InterUITemplateItemReturn();
		pItem.transform.SetParent(TemplateItem.transform.parent, false);

		pParentUIFrame.InterUIWidgetDelete(pItem);

		OnUITemplateItemReturn(pItem);
	}
	//-------------------------------------------------------------
	protected List<CUIWidgetTemplateItemBase>.Enumerator IterUIWidgetTemplateList() { return m_listCloneInstance.GetEnumerator(); }

	//---------------------------------------------------------------
	protected virtual void OnUITemplateItemRequest(CUIWidgetTemplateItemBase pItem) { }
	protected virtual void OnUITemplateItemReturn(CUIWidgetTemplateItemBase pItem) { }
}
