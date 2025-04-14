using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Events;
using System.IO;
public abstract class CSceneAttacherBase : CMonoBase
{

    private bool m_bInitialize = false;
    //-----------------------------------------------------------
    private void Update()
    {
        if (m_bInitialize == false)
        {
            m_bInitialize = true;
            OnSceneAttacherFirstUpdate();
        }
    }
    //-----------------------------------------------------------
    protected void ProtSceneAttacherLoadResourcePrefab(string strPrefabPath, string strPrefabName, UnityAction delFinish)
	{
		GameObject Prefab = GameObject.Find(strPrefabName);
		if (Prefab == null)
		{
			PrivSceneAttacherLoadResourcePrefab($"{strPrefabPath}{Path.DirectorySeparatorChar}{strPrefabName}", delFinish);
		}
	} 

	protected void ProtSceneAttacherLoadAddressablePrefab(string strPrefabName, UnityAction<bool> delFinish)
	{
		GameObject Prefab = GameObject.Find(strPrefabName);
		if (Prefab == null)
		{
			PrivSceneAttacherLoadAddressablePrefab(strPrefabName, delFinish);
		}
		else
		{
			delFinish?.Invoke(false);
		}		
	} 

	protected void ProtSceneAttacherLoadUIScene(string strUISceneName, UnityAction delFinish)
	{
		CManagerUIFrameBase UIMananger = FindFirstObjectByType<CManagerUIFrameBase>();
		if (UIMananger == null)
		{
			PrivSceneAttacherLoadUIScene(strUISceneName, delFinish);
		}
		else 
		{
			delFinish?.Invoke();
		}
	}

	protected void ProtSceneAttacherLoadComponent<TEMPLATE>(string strPrefabName, UnityAction<TEMPLATE> delFinish) where TEMPLATE : Component
    {
		Addressables.LoadAssetAsync<TEMPLATE>(strPrefabName).Completed += (AsyncOperationHandle<TEMPLATE> pResult) =>
		{
			delFinish?.Invoke(pResult.Result);
		}; 
    }

	protected void ProtSceneAttacherDestroy()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject Child = transform.GetChild(i).gameObject;
			Destroy(Child);
		}

		Destroy(gameObject);
	}

	//----------------------------------------------------------
	private void PrivSceneAttacherLoadResourcePrefab(string strPrefabPath, UnityAction delFinish)
	{
		GameObject Prefab = Instantiate(Resources.Load(strPrefabPath), Vector3.zero, Quaternion.identity) as GameObject;
		if (Prefab == null)
		{
			Debug.LogError("[LoadResourcePrefab]");
		}
		else
		{
			RemoveCloneObjectName(Prefab);
			DontDestroyOnLoad(Prefab);
		}
		delFinish?.Invoke();
	}

	private void PrivSceneAttacherLoadAddressablePrefab(string strPrefabName, UnityAction<bool> delFinish)
	{
		Addressables.InitializeAsync().Completed += (AsyncOperationHandle<IResourceLocator> Result) =>
		{
			AsyncOperationHandle<GameObject> LoadObject = Addressables.InstantiateAsync(strPrefabName);
			LoadObject.Completed += (AsyncOperationHandle<GameObject> Result) =>
			{
				RemoveCloneObjectName(Result.Result);
				Result.Result.gameObject.SetActive(true);
				delFinish?.Invoke(true);
			};
		};
	}

	private void PrivSceneAttacherLoadUIScene(string strUISceneName, UnityAction delFinish)
	{
		Addressables.InitializeAsync().Completed += (AsyncOperationHandle<IResourceLocator> Result) =>
		{
			Addressables.LoadSceneAsync(strUISceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive).Completed += (AsyncOperationHandle<SceneInstance> Result) =>
			{
				if (Result.Status != AsyncOperationStatus.Succeeded)
				{
					Debug.LogError("No Exist UIScene");
                }
				delFinish?.Invoke();
			};
		};
	}
    //------------------------------------------------------------------
    protected virtual void OnSceneAttacherFirstUpdate() { }
}
