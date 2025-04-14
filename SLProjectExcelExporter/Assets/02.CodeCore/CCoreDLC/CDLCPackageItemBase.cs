using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// DLC 에 포함된다. DLC 내부의 에셋 정보를 묘사하고 있다. 
abstract public class CDLCPackageItemBase : CMonoBase
{   
    [System.Serializable]
    public class SPreLoadItem
	{
        public string AssetName;
        public EAssetPoolType PoolType = EAssetPoolType.AlwaysLoad;
	}

    [System.Serializable]
    public class SPreLoadCategory
	{
        public string CategoryName;
        public List<SPreLoadItem> PreLoadList = new List<SPreLoadItem>();
	}

    [SerializeField]
    private string DLCPackageName = "None";    public string p_DLCPackageItemName { get { return DLCPackageName; } }
   
    [SerializeField]
    private List<SPreLoadCategory> PreLoadAssets = null;

    //--------------------------------------------------------------------------------------
    internal void InterDLCPackageLoadStart(UnityAction delFinish)
	{
        PrivDLCPreLoad(delFinish);
    }
     
    //------------------------------------------------------------------------------------
    private void PrivDLCPreLoad(UnityAction delFinish)
	{
        int iLoadCount = 0;
        int iTotalCount = 0;

        for (int i = 0; i < PreLoadAssets.Count; i++)
		{
            SPreLoadCategory pCategory = PreLoadAssets[i];

            for (int j = 0; j < pCategory.PreLoadList.Count; j++)
			{
                iTotalCount++;
				CManagerPrefabPoolUsageBase.Instance.LoadGameObject(pCategory.PreLoadList[j].PoolType, pCategory.PreLoadList[j].AssetName, (GameObject pLoadedObject) =>
				{
					iLoadCount++;
					if (iLoadCount == iTotalCount)
					{
						OnDLCPackageLoadWork(delFinish);
					}
				});
			}
		}

        if (iTotalCount == 0)
        {
            delFinish?.Invoke();
        }
	}
    
	//--------------------------------------------------------------------------------------
    protected virtual void OnDLCPackageLoadWork(UnityAction delFinish) { }
}
