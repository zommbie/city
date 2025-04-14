using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

abstract public class CManagerUIFrameUsageBase : CManagerUIFrameFocusBase
{
    public static new CManagerUIFrameUsageBase Instance { get { return CManagerUIFrameFocusBase.Instance as CManagerUIFrameUsageBase; } }
    //------------------------------------------------------------------------
    public TEMPLATE UIShow<TEMPLATE>() where TEMPLATE : CUIFrameBase
	{
		return ProtMgrUIFrameFocusShow(typeof(TEMPLATE).Name) as TEMPLATE;
	}

    public void UIShow(CUIFrameBase pFrameInstance)
	{
        ProtMgrUIFrameFocusShow(pFrameInstance);
	}

	public TEMPLATE UIHide<TEMPLATE>() where TEMPLATE : CUIFrameBase
	{
		return ProtMgrUIFrameFocusHide(typeof(TEMPLATE).Name) as TEMPLATE;
	}

	public void UIHide(CUIFrameBase pUIFrameHide)
	{
		ProtMgrUIFrameFocusHide(pUIFrameHide);
	}
     
	public void UIClose()
	{
		ProtMgrUIFrameFocusClose();
	}

    public TEMPLATE UIFind<TEMPLATE>() where TEMPLATE : CUIFrameBase
    {
        return FindUIFrame(typeof(TEMPLATE).Name) as TEMPLATE;
    }
     
    public void UIFind<TEMPLATE>(UnityAction<TEMPLATE> delFinish) where TEMPLATE : CUIFrameBase
    {
        string strFrameName = typeof(TEMPLATE).Name;
        CUIFrameBase pUIFrame = FindUIFrame(strFrameName);
        if (pUIFrame)
        {
            delFinish?.Invoke(pUIFrame as TEMPLATE);
        } 
        else
        {
            Addressables.LoadAssetAsync<TEMPLATE>(strFrameName).Completed += (AsyncOperationHandle<TEMPLATE> pLoadedHandle) =>
            {
                TEMPLATE pLoadedUIFrame = pLoadedHandle.Result as TEMPLATE;
                if (pUIFrame != null)
                {
                    ProtMgrUIFrameLoadDynamic(pLoadedUIFrame);
                    delFinish?.Invoke(pLoadedUIFrame);
                }
            };
        }
    }

    public void UIHidePanelAll()
    {
        ProtMgrUIFrameFocusPanelHideAll();
    }
}
