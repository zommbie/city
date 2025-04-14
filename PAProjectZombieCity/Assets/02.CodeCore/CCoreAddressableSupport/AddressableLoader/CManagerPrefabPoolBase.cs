using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
// 일반적인 오브젝트 풀링 기능 수행 

public abstract class CManagerPrefabPoolBase : CManagerAddressableBase<CManagerPrefabPoolBase, CAddressableProviderGameObject>
{   
	private class SPrefabPool
	{
		public string						Category;
		public string						AddressableName;
		public AsyncOperationHandle			OriginHandle;	
		public GameObject 					OriginInstance;
		public GameObject					RootParent;
		public bool							AutoRelease = false;				  // 모든 클론이 회수되면 Adressable을 통해 해재한다. 비디오 메모리는 UnloadUnuseAsset해야 내려간다.
		public Queue<GameObject>	CloneStock = new Queue<GameObject>();         // 생성은 되었으나 사용되지 않는 객체이다. 사용자에게 대여하거나 회수된다.
		public List<GameObject>		CloneList = new List<GameObject>();           // 생성된 모든 게임 오브젝트를 참조한다. 비디오 메모리 내릴때를 위한 항목		
	}

	private Dictionary<string, Dictionary<string, SPrefabPool>>	m_mapPrefabInstance   = new Dictionary<string, Dictionary<string, SPrefabPool>>();
	private Dictionary<GameObject, SPrefabPool>					m_mapPrefabCache      = new Dictionary<GameObject, SPrefabPool>(); // 검색 비용 절감을 위한 메모리 사용
	private Dictionary<string, GameObject>						m_mapPrefabParent	  = new Dictionary<string, GameObject>();
    //------------------------------------------------------------------
    public bool HasPoolPrefabLoaded(EAssetPoolType eAssetPoolType, string strPrefabName)
    {
        bool bLoaded = false;
        SPrefabPool pPrefabPool = FindPrefabPool(eAssetPoolType, strPrefabName);
        if (pPrefabPool != null)
        {
            bLoaded = true;
        }

        return bLoaded;
    }

    //-----------------------------------------------------------------
	/// <summary>
	/// _autoRelease 옵션의 경우 모든 클론이 반환될 경우 자동으로 메모리를 해재한다. 다음 호출때는 에셋번들에서 읽어오게 되므로 주의할것.
	/// </summary>
    protected void ProtPoolReserveInstance(string strCategory, string strName, int iReserveCount, UnityAction<string, GameObject> delFinish, bool bAutoRelease)
	{
		Dictionary<string, SPrefabPool> mapPrefabPool = FindPrefabPoolOrAlloc(strCategory);

		if (mapPrefabPool.ContainsKey(strName))
		{
			GameObject pCloneInstance = null;
			for (int i = 0; i < iReserveCount; i++)
			{
				pCloneInstance = ProtRequestClone(strCategory, strName);
			}
			delFinish?.Invoke(strName, pCloneInstance);
		}
		else
		{
			RequestLoad(strName, (string _LoadedAddressableName,  AsyncOperationHandle pLoadedHandle) =>
			{
				ReserveInternal(strCategory, mapPrefabPool, _LoadedAddressableName, pLoadedHandle, iReserveCount, bAutoRelease);

				if (iReserveCount > 0)
                {
                    GameObject CloneInstance = ProtRequestClone(strCategory, strName);
                    delFinish?.Invoke(_LoadedAddressableName, CloneInstance);
                }
				else
                {
					delFinish?.Invoke(_LoadedAddressableName, null);
                }

			}, null);
		}

		OnPrefabRequestOrigin(strName);
	}

    protected GameObject ProtRequestClone(string _category, string _addressableName)
	{
		GameObject Instance = null;
		Dictionary<string, SPrefabPool> PrefabInstance = FindPrefabPoolOrAlloc(_category);
		if (PrefabInstance.ContainsKey(_addressableName))
		{
			SPrefabPool PrefabPool = PrefabInstance[_addressableName];
			if (PrefabPool.CloneStock.Count == 0)
			{
				AllocateCloneInstance(PrefabPool, 1);
			}
			Instance = PrefabPool.CloneStock.Dequeue();
		}
		return Instance;
	}

    protected void ProtReturnClone(GameObject pReturnObject, bool bReleaseOrigin)
    {
        SPrefabPool pPrefabPool = FindPrefabPool(pReturnObject);
        if (pPrefabPool != null)
        {
            pPrefabPool.CloneStock.Enqueue(pReturnObject);

            pReturnObject.transform.SetParent(pPrefabPool.RootParent.transform, false);
            pReturnObject.transform.position = Vector3.zero;
            pReturnObject.transform.localPosition = Vector3.zero;
            pReturnObject.SetActive(false);

            if (bReleaseOrigin)
            {
                ProtRemoveInstance(pPrefabPool.Category, pPrefabPool.AddressableName, true);
            }
            else if (pPrefabPool.AutoRelease && pPrefabPool.CloneList.Count == pPrefabPool.CloneStock.Count) // 외부로 나간 클론이 모두 돌아오면 메모리를 해제한다.
            {
                ProtRemoveInstance(pPrefabPool.Category, pPrefabPool.AddressableName, true);
            }
        }		
	}

    protected void ProtRemoveInstance(string strCategory, string strAddressableName, bool bReleaseOrigin)
	{
		Dictionary<string, SPrefabPool> mapPrefabInstance = FindPrefabPoolOrAlloc(strCategory);
		if (mapPrefabInstance.ContainsKey(strAddressableName))
		{
			RemoveInternal(mapPrefabInstance[strAddressableName], bReleaseOrigin);
          
            if (bReleaseOrigin)
            {
                mapPrefabInstance.Remove(strAddressableName);
            }
        }
	}

	protected void ProtRemoveInstanceAll() // 모든 에셋을 해재한다. 비디오 메모리도 클리어 한다.
	{
		Dictionary<string, Dictionary<string, SPrefabPool>>.Enumerator it = m_mapPrefabInstance.GetEnumerator();

		while (it.MoveNext())
		{			
			RemoveCategoryInternal(it.Current.Value, true);
		}
		
		m_mapPrefabCache.Clear();
		Resources.UnloadUnusedAssets();
	}

	protected void ProtRemoveInstanceAllSoft()  // 프리팹만 해재한다. 에셋번들은 유지한다. 비디오 메모리도 유지한다.
	{
		Dictionary<string, Dictionary<string, SPrefabPool>>.Enumerator it = m_mapPrefabInstance.GetEnumerator();

		while (it.MoveNext())
		{
			RemoveCategoryInternal(it.Current.Value, false);
		}
	}

	protected void ProtRemoveCategory(string strCategory, bool bReleaseOrigin)
	{
		Dictionary<string, SPrefabPool> mapPrefabPool = FindPrefabPoolOrAlloc(strCategory);
		if (mapPrefabPool != null)
		{
            RemoveCategoryInternal(mapPrefabPool, bReleaseOrigin);
        }
	}
	
    //------------------------------------------------------------------
    private void AllocateCloneInstance(SPrefabPool pPrefabPool, int iCount)
	{		
        for (int i = 0; i < iCount; i++)
        {
            GameObject pCloneInstance = Instantiate(pPrefabPool.OriginInstance);
            AddCloneInstance(pPrefabPool, pCloneInstance);
        }      
    }

    private void AddCloneInstance(SPrefabPool pPrefabPool, GameObject pCloneInstance)
    {
        pCloneInstance.transform.SetParent(pPrefabPool.RootParent.transform);
        pCloneInstance.transform.position = Vector3.zero;
        pCloneInstance.transform.rotation = Quaternion.identity;
        pCloneInstance.transform.localPosition = Vector3.zero;
        pCloneInstance.transform.localRotation = Quaternion.identity;
        pCloneInstance.SetActive(false);

        RemoveCloneObjectName(pCloneInstance);

        pPrefabPool.CloneList.Add(pCloneInstance);
        pPrefabPool.CloneStock.Enqueue(pCloneInstance);

        m_mapPrefabCache.Add(pCloneInstance, pPrefabPool);

        OnPrefabCloneInstance(pPrefabPool.AddressableName, pCloneInstance);
    }

	private void ReserveInternal(string strCategory, Dictionary<string, SPrefabPool> mapPrefabPool, string strAddressableName, AsyncOperationHandle pLoadedHandle,  int iReserveCount, bool bAutoRelease)
	{
		if (pLoadedHandle.Result == null) return;

        SPrefabPool pNewInstance = null;

        if (mapPrefabPool.ContainsKey(strAddressableName))
		{
            pNewInstance = mapPrefabPool[strAddressableName];
            AddCloneInstance(pNewInstance, pLoadedHandle.Result as GameObject);
		}
		else
		{
            pNewInstance = new SPrefabPool();
			pNewInstance.Category = strCategory;
			pNewInstance.AutoRelease = bAutoRelease;
			pNewInstance.OriginHandle = pLoadedHandle;
			pNewInstance.OriginInstance = pLoadedHandle.Result as GameObject;
			pNewInstance.RootParent = FindParentOrAlloc(strCategory);
			pNewInstance.OriginInstance.transform.SetParent(pNewInstance.RootParent.transform);            
			pNewInstance.AddressableName = strAddressableName;
            RemoveCloneObjectName(pNewInstance.OriginInstance);

            mapPrefabPool[strAddressableName] = pNewInstance;
            AllocateCloneInstance(pNewInstance, iReserveCount);
            OnPrefabOriginLoaded(strAddressableName, pLoadedHandle);
        }        
    }

    private void RemoveInternal(SPrefabPool pRemovePool, bool bReleaseOrigin)
	{
		pRemovePool.CloneStock.Clear();

		for (int i = 0; i < pRemovePool.CloneList.Count; i++)
		{
			GameObject pInstance = pRemovePool.CloneList[i];			
			OnPrefabCloneRemove(pRemovePool.AddressableName, pInstance);      
            m_mapPrefabCache.Remove(pInstance);
            Destroy(pInstance);            
        }

		pRemovePool.CloneList.Clear();

		if (bReleaseOrigin)
		{ 
			OnPrefabOriginRemove(pRemovePool.AutoRelease, pRemovePool.AddressableName, pRemovePool.OriginHandle);
            Addressables.Release(pRemovePool.OriginHandle);           
            pRemovePool.OriginInstance = null;            
		}
	}

	private void RemoveCategoryInternal(Dictionary<string, SPrefabPool> pMapCategory, bool bReleaseOrigin)
	{
		Dictionary<string, SPrefabPool>.Enumerator it = pMapCategory.GetEnumerator();
		while(it.MoveNext())
		{
			RemoveInternal(it.Current.Value, bReleaseOrigin);
		}

        if (bReleaseOrigin)
        {
            pMapCategory.Clear();
        }
    }

	private SPrefabPool FindPrefabPool(GameObject _cloneInstance)
	{
		SPrefabPool PrefabPool = null;
		if (m_mapPrefabCache.ContainsKey(_cloneInstance))
		{
			PrefabPool = m_mapPrefabCache[_cloneInstance];
		}
		return PrefabPool;
	}

    private SPrefabPool FindPrefabPool(EAssetPoolType eAssetPoolType, string strPrefabName)
    {
        SPrefabPool pFindPrefabPool = null;
        Dictionary<string, SPrefabPool> mapPrefabPool = FindPrefabPoolOrAlloc(eAssetPoolType.ToString());
        if (mapPrefabPool != null)
        {
            mapPrefabPool.TryGetValue(strPrefabName, out pFindPrefabPool);
        }

        return pFindPrefabPool;
    }

	private Dictionary<string, SPrefabPool> FindPrefabPoolOrAlloc(string strCategoryName)
	{
		Dictionary<string, SPrefabPool> pFindCategory = null;
		if (m_mapPrefabInstance.ContainsKey(strCategoryName))
		{
			pFindCategory = m_mapPrefabInstance[strCategoryName];
		}
		else
		{
			pFindCategory = new Dictionary<string, SPrefabPool>();
			m_mapPrefabInstance.Add(strCategoryName, pFindCategory);
		}

		return pFindCategory;
	}

	private GameObject FindParentOrAlloc(string _category)
	{
		GameObject Find = null;
		if (m_mapPrefabParent.ContainsKey(_category))
		{
			Find = m_mapPrefabParent[_category];
		}
		else
		{
			Find = MakePrefabParent(_category);
		}

		return Find;
	}

	private GameObject MakePrefabParent(string _category)
	{
		GameObject Parent = new GameObject();
		Parent.name = _category;
		Parent.transform.SetParent(transform);
		m_mapPrefabParent.Add(_category, Parent);
		return Parent;
	}

	//------------------------------------------------------------------
	protected virtual void OnPrefabRequestOrigin(string _addressableName) { }
	protected virtual void OnPrefabOriginLoaded(string _addressableName, AsyncOperationHandle pLoadedHandle) { }
	protected virtual void OnPrefabOriginRemove(bool bAutoRelase, string strAddressableName, AsyncOperationHandle pRemovedHandle) { }
	protected virtual void OnPrefabCloneInstance(string _addressableName, GameObject _cloneInstance) { }
	protected virtual void OnPrefabCloneRemove(string _addressableName, GameObject _removeClone) { }   // 강제로 해재 되었기 때문에 하위 레이어에서는 이 게임 오브젝트의 소유자에게 이벤트를 전달해야 한다. 

}
