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
		public GameObject					Parent;
		public bool							AutoRelease = false;				  // 모든 클론이 회수되면 자동으로 비디오 메모리를 해재한다.
		public Queue<GameObject>	CloneStock = new Queue<GameObject>();         // 생성은 되었으나 사용되지 않는 객체이다. 사용자에게 대여하거나 회수된다.
		public List<GameObject>		CloneList = new List<GameObject>();           // 생성된 모든 게임 오브젝트를 참조한다. 비디오 메모리 내릴때를 위한 항목		
	}

	private Dictionary<string, Dictionary<string, SPrefabPool>>	m_dicPrefabInstance   = new Dictionary<string, Dictionary<string, SPrefabPool>>();
	private Dictionary<GameObject, SPrefabPool>					m_dicPrefabMap		  = new Dictionary<GameObject, SPrefabPool>(); // 검색 비용 절감을 위한 메모리 사용
	private Dictionary<string, GameObject>						m_dicPrefabParent	  = new Dictionary<string, GameObject>();
    //------------------------------------------------------------------

	/// <summary>
	/// _autoRelease 옵션의 경우 모든 클론이 반환될 경우 자동으로 메모리를 해재한다. 다음 호출때는 에셋번들에서 읽어오게 되므로 주의할것.
	/// </summary>
    protected void ProtPoolReserveInstance(string strCategory, string strName, int iReserveCount, UnityAction<string, GameObject> delFinish, bool bAutoRelease)
	{
		Dictionary<string, SPrefabPool> mapPrefabPool = FindPrefabPoolOrAlloc(strCategory);

		if (iReserveCount <= 0)
		{
			bAutoRelease = false;  // 프리로드 상태일경우 무조건 자율해제가 되므로 강제로 꺼준다.
		} 

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
				AllocateCloneInstance(PrefabPool);
			}
			Instance = PrefabPool.CloneStock.Dequeue();
		}
		return Instance;
	}

    protected void ProtReturnClone(GameObject _returnObject)
	{
		SPrefabPool Prefab = FindPrefabPool(_returnObject);
		if (Prefab != null)
		{
			_returnObject.transform.SetParent(Prefab.Parent.transform, false);
			Prefab.Parent.transform.position = Vector3.zero;
			_returnObject.SetActive(false);
			Prefab.CloneStock.Enqueue(_returnObject);

			if (Prefab.AutoRelease)
			{
				if (Prefab.CloneList.Count == Prefab.CloneStock.Count) // 외부로 나간 클론이 모두 돌아오면 메모리를 해제한다.
				{
					ProtRemoveInstance(Prefab.Category, Prefab.AddressableName, false);
				}
			}
		}		
	}

    protected void ProtRemoveInstance(string _category, string _addressable, bool bOnlyRemovePrefab)
	{
		Dictionary<string, SPrefabPool> PrefabInstance = FindPrefabPoolOrAlloc(_category);
		if (PrefabInstance.ContainsKey(_addressable))
		{
			RemoveInternal(PrefabInstance[_addressable], bOnlyRemovePrefab);
			PrefabInstance.Remove(_addressable);
		}
	}

	protected void ProtRemoveInstanceAll() // 모든 에셋을 해재한다. 비디오 메모리도 클리어 한다.
	{
		Dictionary<string, Dictionary<string, SPrefabPool>>.Enumerator it = m_dicPrefabInstance.GetEnumerator();

		while (it.MoveNext())
		{
			DeletePrefabParent(it.Current.Key);
			RemoveCategoryInternal(it.Current.Value, false);
		}

		m_dicPrefabInstance.Clear();
		m_dicPrefabMap.Clear();
		Resources.UnloadUnusedAssets();
	}

	protected void ProtRemoveInstanceAllSoft()  // 프리팹만 해재한다. 에셋번들은 유지한다. 비디오 메모리도 유지한다.
	{
		Dictionary<string, Dictionary<string, SPrefabPool>>.Enumerator it = m_dicPrefabInstance.GetEnumerator();

		while (it.MoveNext())
		{
			RemoveCategoryInternal(it.Current.Value, true);
		}
	}

	protected void ProtRemoveCategory(string _category)
	{
		Dictionary<string, SPrefabPool> PrefabPool = FindPrefabPoolOrAlloc(_category);
		if (PrefabPool != null)
		{
			DeletePrefabParent(_category);
			RemoveCategoryInternal(PrefabPool, false);
			m_dicPrefabInstance.Remove(_category);
		}
	}
	
    //------------------------------------------------------------------
    private void AllocateCloneInstance(SPrefabPool pPrefabPool)
	{
		GameObject pNewInstance = Instantiate(pPrefabPool.OriginInstance, pPrefabPool.Parent.transform);

        pNewInstance.transform.position = Vector3.zero;
        pNewInstance.transform.rotation = Quaternion.identity;
        pNewInstance.transform.localPosition = Vector3.zero;
        pNewInstance.transform.localRotation = Quaternion.identity;
		pNewInstance.SetActive(false);

		pPrefabPool.CloneList.Add(pNewInstance);
		pPrefabPool.CloneStock.Enqueue(pNewInstance);

		m_dicPrefabMap.Add(pNewInstance, pPrefabPool);

		OnPrefabCloneInstance(pPrefabPool.AddressableName, pNewInstance);
    }

	private void ReserveInternal(string _category, Dictionary<string, SPrefabPool> mapPrefabPool, string _addressableName, AsyncOperationHandle pLoadedHandle,  int _reserveCount, bool _autoRelease)
	{
		if (pLoadedHandle.Result == null) return;

		SPrefabPool NewInstance = null;

		if (mapPrefabPool.ContainsKey(_addressableName))
		{
			NewInstance = mapPrefabPool[_addressableName];
		}
		else
		{
			NewInstance = new SPrefabPool();
			NewInstance.Category = _category;
			NewInstance.AutoRelease = _autoRelease;
			NewInstance.OriginHandle = pLoadedHandle;
			NewInstance.OriginInstance = pLoadedHandle.Result as GameObject;
			NewInstance.Parent = FindParentOrAlloc(_category);
			NewInstance.OriginInstance.transform.SetParent(NewInstance.Parent.transform);
			NewInstance.AddressableName = _addressableName;			
            mapPrefabPool[_addressableName] = NewInstance;
		}

		for (int i = 0; i < _reserveCount; i++)
		{
			AllocateCloneInstance(NewInstance);
		}

		OnPrefabOriginLoaded(_addressableName, pLoadedHandle);
	}

    private void RemoveInternal(SPrefabPool pRemovePool, bool bOnlyRemovePrefab)
	{
		pRemovePool.CloneStock.Clear();

		for (int i = 0; i < pRemovePool.CloneList.Count; i++)
		{
			GameObject gameObjectInstance = pRemovePool.CloneList[i];			
			OnPrefabCloneRemove(pRemovePool.AddressableName, gameObjectInstance);
			m_dicPrefabMap.Remove(gameObjectInstance);
			Destroy(gameObjectInstance);
		}
		pRemovePool.CloneList.Clear();

		if (bOnlyRemovePrefab == false)
		{
			OnPrefabOriginRemove(pRemovePool.AutoRelease, pRemovePool.AddressableName, pRemovePool.OriginHandle);
			Addressables.ReleaseInstance(pRemovePool.OriginInstance);			
		}
	}

	private void RemoveCategoryInternal(Dictionary<string, SPrefabPool> _category, bool bOnlyRemovePrefab)
	{
		Dictionary<string, SPrefabPool>.Enumerator it = _category.GetEnumerator();
		while(it.MoveNext())
		{
			RemoveInternal(it.Current.Value, bOnlyRemovePrefab);
		}
		_category.Clear();
	}

	private SPrefabPool FindPrefabPool(GameObject _cloneInstance)
	{
		SPrefabPool PrefabPool = null;
		if (m_dicPrefabMap.ContainsKey(_cloneInstance))
		{
			PrefabPool = m_dicPrefabMap[_cloneInstance];
		}
		return PrefabPool;
	}

	private Dictionary<string, SPrefabPool> FindPrefabPoolOrAlloc(string _category)
	{
		Dictionary<string, SPrefabPool> Find = null;
		if (m_dicPrefabInstance.ContainsKey(_category))
		{
			Find = m_dicPrefabInstance[_category];
		}
		else
		{
			Find = new Dictionary<string, SPrefabPool>();
			m_dicPrefabInstance.Add(_category, Find);
		}

		return Find;
	}

	private GameObject FindParentOrAlloc(string _category)
	{
		GameObject Find = null;
		if (m_dicPrefabParent.ContainsKey(_category))
		{
			Find = m_dicPrefabParent[_category];
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
		m_dicPrefabParent.Add(_category, Parent);
		return Parent;
	}

	private void DeletePrefabParent(string _category)
	{
		if (m_dicPrefabParent.ContainsKey(_category))
		{
			GameObject Parent = m_dicPrefabParent[_category];
			m_dicPrefabParent.Remove(_category);
			Destroy(Parent);
		}
	}

	//------------------------------------------------------------------
	protected virtual void OnPrefabRequestOrigin(string _addressableName) { }
	protected virtual void OnPrefabOriginLoaded(string _addressableName, AsyncOperationHandle pLoadedHandle) { }
	protected virtual void OnPrefabOriginRemove(bool bAutoRelase, string strAddressableName, AsyncOperationHandle pRemovedHandle) { }
	protected virtual void OnPrefabCloneInstance(string _addressableName, GameObject _cloneInstance) { }
	protected virtual void OnPrefabCloneRemove(string _addressableName, GameObject _removeClone) { }   // 강제로 해재 되었기 때문에 하위 레이어에서는 이 게임 오브젝트의 소유자에게 이벤트를 전달해야 한다. 

}
