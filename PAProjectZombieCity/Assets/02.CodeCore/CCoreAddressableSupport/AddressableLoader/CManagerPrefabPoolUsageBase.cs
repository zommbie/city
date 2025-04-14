using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public enum EAssetPoolType
{
	UI,
    UnitActor,
    Character,
	Projectile,
    StageProp,
	Effect,       // 항상 로드 되어 있는 이펙트
    UIBackGround, // 스토리나 월드맵에 사용되는 백그라운드
	AlwaysLoad,   // 메모리에 항상 로드 되어 있어야 하는 자원
}

public abstract class CManagerPrefabPoolUsageBase : CManagerPrefabAutoReleaseBase
{
    public static new CManagerPrefabPoolUsageBase Instance { get { return CManagerPrefabAutoReleaseBase.Instance as CManagerPrefabPoolUsageBase; }}
    //--------------------------------------------------------------	
    public void LoadGameObject(EAssetPoolType ePoolType, string strAddressableName, UnityAction<GameObject> delFinish, int iReserveCount = 1, bool bAutoRelease = false)
	{
		ProtPoolReserveInstance(ePoolType.ToString(), strAddressableName, iReserveCount, (string strLoadedAddressable, GameObject pLoadedObject) =>
		{
			delFinish?.Invoke(pLoadedObject);
		}, bAutoRelease);
	}
     
	public void LoadComponent<COMPONENT>(EAssetPoolType ePoolType, string strAddressableName, UnityAction<COMPONENT> delFinish, int iReserveCount = 1, bool bAutoRelease = false) where COMPONENT : Component
	{
		ProtPoolReserveInstance(ePoolType.ToString(), strAddressableName, iReserveCount, (string strLoadedAddressable, GameObject pLoadedObject) =>
		{
			COMPONENT Component = pLoadedObject.GetComponent<COMPONENT>();
			delFinish?.Invoke(Component);
		}, bAutoRelease);
	}
	/// <summary>
	/// 별도의 Clone을 생성하지 않는다.  
	/// </summary>
	public void LoadGameObjectNoInstance(EAssetPoolType ePoolType, string strAddressableName, UnityAction delFinish, bool bAutoRelease = false)
	{
		ProtPoolReserveInstance(ePoolType.ToString(), strAddressableName, 0, (string _loadedAddressable, GameObject _loadedObject) =>
		{           
			delFinish?.Invoke();
		}, bAutoRelease);
	}
	//-----------------------------------------------------------------
	/// <summary>
	/// 간편하게 사용할수 있는 동기함수. Reserve가 되어 있지 않으면 Null을 반환
	/// </summary>
	public GameObject FindClone(EAssetPoolType ePoolType, string strAddressableName)
	{
		GameObject CloneInstance = ProtRequestClone(ePoolType.ToString(), strAddressableName);
		if (CloneInstance == null)
		{
			Debug.LogWarning($"[PoolMananger] There is no reserve instance.  call LoadGameObjectInstance() and reserve an Instance. | {strAddressableName}");
		}
		return CloneInstance;
	}

	public COMPONENT FindClone<COMPONENT>(EAssetPoolType ePoolType, string strAddressableName) where COMPONENT : Component
	{
		GameObject pCloneInstance = FindClone(ePoolType, strAddressableName);
		COMPONENT pClone = null;
		if (pCloneInstance != null)
        {
			pClone = pCloneInstance.GetComponent<COMPONENT>();
		}
		return pClone;
	}

	//----------------------------------------------------------

	public void ClearCategory(EAssetPoolType ePoolType, bool bReleaseOrigin = false)
	{
		ProtRemoveCategory(ePoolType.ToString(), bReleaseOrigin);
	}

	public void ClearAll(bool bReleaseAll = true) 
	{
		if (bReleaseAll)
		{
            ProtRemoveInstanceAll();
		}
		else
		{
            ProtRemoveInstanceAllSoft();
        }
    }

	public void Return(GameObject eReturnObject, bool bReleaseOrigin = false)
	{
		ProtReturnClone(eReturnObject, bReleaseOrigin);
	}

	//-------------------------------------------------------------	
}
