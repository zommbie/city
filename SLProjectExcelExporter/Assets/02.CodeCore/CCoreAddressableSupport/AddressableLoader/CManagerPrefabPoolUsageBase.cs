using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public enum EAssetPoolType
{
	UI,
	Projectile,
	Effect,       // 항상 로드 되어 있는 이펙트

	DLC,
	AlwaysLoad,   // 메모리에 항상 로드 되어 있어야 하는 자원
}

public abstract class CManagerPrefabPoolUsageBase : CManagerPrefabAutoReleaseBase
{
    public static new CManagerPrefabPoolUsageBase Instance { get { return CManagerPrefabAutoReleaseBase.Instance as CManagerPrefabPoolUsageBase; }}
    //--------------------------------------------------------------	
    public void LoadGameObject(EAssetPoolType _poolType, string _addressableName, UnityAction<GameObject> _eventFinish, int _reserveCount = 1, bool _autoRelease = false)
	{
		ProtPoolReserveInstance(_poolType.ToString(), _addressableName, _reserveCount, (string _loadedAddressable, GameObject _loadedObject) =>
		{
			_eventFinish?.Invoke(_loadedObject);
		}, _autoRelease);
	}
     
	public void LoadComponent<COMPONENT>(EAssetPoolType _poolType, string _addressableName, UnityAction<COMPONENT> _evenFinish, int _reserveCount = 1, bool _autoRelease = false) where COMPONENT : Component
	{
		ProtPoolReserveInstance(_poolType.ToString(), _addressableName, _reserveCount, (string _loadedAddressable, GameObject _loadedObject) =>
		{
			COMPONENT Component = _loadedObject.GetComponent<COMPONENT>();
			_evenFinish?.Invoke(Component);
		}, _autoRelease);
	}
	/// <summary>
	/// 별도의 Clone을 생성하지 않는다.  에셋번들에서 텍스처등의 로딩만 수행한다. 
	/// </summary>
	public void LoadGameObjectNoInstance(EAssetPoolType _poolType, string _addressableName, UnityAction _eventFinish)
	{
		ProtPoolReserveInstance(_poolType.ToString(), _addressableName, 0, (string _loadedAddressable, GameObject _loadedObject) =>
		{
			_eventFinish?.Invoke();
		}, false);
	}
	//-----------------------------------------------------------------
	/// <summary>
	/// 간편하게 사용할수 있는 동기함수. Reserve가 되어 있지 않으면 Null을 반환
	/// </summary>
	public GameObject FindClone(EAssetPoolType _poolType, string _addressableName)
	{
		GameObject CloneInstance = ProtRequestClone(_poolType.ToString(), _addressableName);
		if (CloneInstance == null)
		{
			Debug.LogWarning($"[ZPoolMananger] There is no reserve instance.  call LoadGameObjectInstance() and reserve an Instance. | {_addressableName}");
		}
		return CloneInstance;
	}

	public COMPONENT FindClone<COMPONENT>(EAssetPoolType _poolType, string _addressableName) where COMPONENT : Component
	{
		GameObject CloneInstance = FindClone(_poolType, _addressableName);
		COMPONENT pClone = null;
		if (CloneInstance != null)
        {
			pClone = CloneInstance.GetComponent<COMPONENT>();
		}
		return pClone;
	}

	//----------------------------------------------------------
	public void ClearAll(EAssetPoolType _poolType, string _addressableName, bool bOnlyRemovePrefab = false)
	{
		ProtRemoveInstance(_poolType.ToString(), _addressableName, bOnlyRemovePrefab);
	}

	public void ClearCategory(EAssetPoolType _poolType)
	{
		ProtRemoveCategory(_poolType.ToString());
	}

	public void ClearAll(bool bOnlyRemovePrefab = false) 
	{
		if (bOnlyRemovePrefab)
		{
			ProtRemoveInstanceAllSoft();
		}
		else
		{
			ProtRemoveInstanceAll(); 
		}
	}

	public void Return(GameObject _returnObject)
	{
		ProtReturnClone(_returnObject);
	}

	//-------------------------------------------------------------	
}
