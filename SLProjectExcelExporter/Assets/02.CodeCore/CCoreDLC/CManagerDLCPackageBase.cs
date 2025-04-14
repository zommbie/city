using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// DownLoaderble Contents를 관리하기 위한 레이어이다. 
abstract public class CManagerDLCPackageBase : CManagerTemplateBase<CManagerDLCPackageBase>
{
    private Dictionary<string, CDLCPackageItemBase> m_mapDLCReference = new Dictionary<string, CDLCPackageItemBase>();
    //-------------------------------------------------------------------


    //-------------------------------------------------------------------
    public void DoMgrDLCPackageLoad(string strDLCName, UnityAction delFinish)
	{
        if (m_mapDLCReference.ContainsKey(strDLCName)) return;

        CManagerPrefabPoolUsageBase.Instance.LoadComponent(EAssetPoolType.DLC, strDLCName, (CDLCPackageItemBase pLoadedItem) =>
        {
            PrivMgrDLCPackageRegist(pLoadedItem, delFinish);
        });

	}
       
    //--------------------------------------------------------------------
    private void PrivMgrDLCPackageRegist(CDLCPackageItemBase pDLCPackageItem, UnityAction delFinish)
	{
        m_mapDLCReference.Add(pDLCPackageItem.p_DLCPackageItemName, pDLCPackageItem);
        pDLCPackageItem.InterDLCPackageLoadStart(() => {
            delFinish?.Invoke();
            OnMgrDLCPackageAdd(pDLCPackageItem);
        });
	}

    //---------------------------------------------------------------------
    protected virtual void OnMgrDLCPackageAdd(CDLCPackageItemBase pDLCReference) {}
     
}
